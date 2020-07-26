using DecisionFramework;
using Proyecto26;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public enum CMSKeypress { CONFIRM, UP, DOWN }
public enum CMSTextType { NONE, DECISION, CHOICE, CONSEQUENCE }

public abstract class CMSInputBase : MonoBehaviour {
	[SerializeField] CMSTextType textType;
	[SerializeField] ListType searchType;

	public CMSTextType TextType {
		get { return textType; }
	}
	public ListType SearchType {
		get { return searchType; }
	}

	public abstract string Text { get; }
	public abstract void SetSearchSpace(List<string> searchSpace);
	public virtual void SetCharacterLimit(int charLimit) { }

	public delegate void ConfirmEntryEvent(CMSInputBase field);
	public ConfirmEntryEvent OnConfirmEntry;

	public void Initialize(string startText) {
		CMSManager.Instance.RegisterListElement(this);
		InitializeCustom(startText);
	}

	protected abstract void InitializeCustom(string startText);
}


public class CMSManager : MonoBehaviour {
	public static CMSManager Instance { get; private set; }

	[SerializeField] GameObject introTextObject;
	[SerializeField] CMSAnimatedActivatable loadingOverlay;
	[SerializeField] string reportingURL;

	// UI elements
	[SerializeField] CMSDecision decisionUI;
	[SerializeField] CMSButton revertButton, uploadChangesButton, addDecisionButton, removeDecisionButton, reportButton, resetSceneButton;
	[SerializeField] CMSDialog commonDialog;
	[SerializeField] CMSLayoutSetup decisionsLayoutSetup;
	[SerializeField] CMSToggleButton decisionButtonPrefab;
	[SerializeField] ToggleGroup decisionListGroup;

	public delegate void CMSInputEvent(CMSKeypress input);
	public CMSInputEvent OnCMSInput;

	public CMSDialog CommonDialog { get => commonDialog; }

	CMSLayout<CMSToggleButton> decisionsLayout;

	class UndoAction {
		public Object nullCheck;
		public System.Action doOnUndo;
	}

	bool ShowLoading {
		get { return loadingOverlay ? loadingOverlay.IsActive : false; }
		set {
			if (loadingOverlay) loadingOverlay.SetActive(value);
			if (introTextObject) introTextObject.SetActive(!value);
		}
	}

	[System.Serializable]
	class CMSParams {
		[SerializeField] int decisionCharLimit = 100, choiceCharLimit = 70, consequenceCharLimit = 150;

		public int GetCharacterLimit(CMSTextType textType) {
			switch (textType) {
			case CMSTextType.DECISION: return decisionCharLimit;
			case CMSTextType.CHOICE: return choiceCharLimit;
			case CMSTextType.CONSEQUENCE: return consequenceCharLimit;
			default: return 0;
			}
		}
	}

	GameDataHolder dataHolder = new GameDataHolder();
	CMSParams cmsParams = new CMSParams();

	Dictionary<int, string> decisionIDMap = new Dictionary<int, string>();
	HashSet<int> changedDecIndices = new HashSet<int>();
	int curDecIndex = -1;
	string decisionsURL, listsURL;
	Stack<UndoAction> undoStack = new Stack<UndoAction>();

	const string DECISIONS_SUFFIX = "decisions",
		LISTS_SUFFIX = "lists",
		JSON_SUFFIX = ".json",
		CMS_PARAMS_URL = "https://test-project-bcd07.firebaseio.com/cmsParams",
		GAME_DATA_URL = "https://test-project-bcd07.firebaseio.com/game-data";
		//DATABASE_URL = "https://reignslike-prototype.firebaseio.com/game-data";

	// Public methods

	public void RegisterListElement(CMSInputBase listElement) {
		List<string> curList = dataHolder.lists.GetList(listElement.SearchType);
		listElement.SetSearchSpace(curList);
		listElement.SetCharacterLimit(cmsParams.GetCharacterLimit(listElement.TextType));

		listElement.OnConfirmEntry += field => {
			// Adding a new list entry
			// TODO: Add confirmation dialog
			dataHolder.lists.AddEntry(field.SearchType, field.Text);
		};
	}
	
	public void AddUndoAction(Object from, System.Action action) {
		undoStack.Push(new UndoAction() {
			nullCheck = from,
			doOnUndo = action
		});
	}

	public void ResetScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OpenReportingURL() {
		if (!string.IsNullOrEmpty(reportingURL)) Application.OpenURL(reportingURL);
	}

	// Private methods
	
	void Awake() {
		decisionsURL = GAME_DATA_URL + "/" + DECISIONS_SUFFIX;
		listsURL = GAME_DATA_URL + "/" + LISTS_SUFFIX;
		Instance = this;

		if (uploadChangesButton) uploadChangesButton.OnClick.AddListener(() => UploadChanges());
		if (revertButton) revertButton.OnClick.AddListener(RevertChanges);
		if (addDecisionButton) addDecisionButton.OnClick.AddListener(AddDecision);
		if (removeDecisionButton) removeDecisionButton.OnClick.AddListener(RemoveCurrentDecision);
		if (reportButton) reportButton.OnClick.AddListener(OpenReportingURL);
		if (resetSceneButton) resetSceneButton.OnClick.AddListener(ResetScene);

		if (decisionsLayoutSetup) {
			decisionsLayout = new CMSLayout<CMSToggleButton>(decisionsLayoutSetup, decisionButtonPrefab);
			if (decisionListGroup) {
				decisionListGroup.allowSwitchOff = true;
				decisionListGroup.SetAllTogglesOff(false);
			}
		}

		if (decisionUI) decisionUI.gameObject.SetActive(false);
		ShowLoading = false;
	}

	private void Start() {
		RevertChanges();
	}

	bool doUpdateDecisionUI = false, selectLastDecisionAfterUpdate = false;

	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) OnCMSInput?.Invoke(CMSKeypress.UP);
		if (Input.GetKeyDown(KeyCode.DownArrow)) OnCMSInput?.Invoke(CMSKeypress.DOWN);
		if (Input.GetKeyDown(KeyCode.Return)) OnCMSInput?.Invoke(CMSKeypress.CONFIRM);
		//if (Input.GetKeyUp(KeyCode.Z) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))) Undo();

		if (doUpdateDecisionUI) {
			decisionsLayout.Clear();

			dataHolder.decisions.ForEach(dec => {
				CMSToggleButton newButton = decisionsLayout.Add();
				newButton.SetText(dec.decisionText);
				if (decisionListGroup) newButton.Toggle.group = decisionListGroup;
				newButton.OnSelected += _ => {
					SetCurrentDecision(dataHolder.decisions.IndexOf(dec));
					if (decisionListGroup) decisionListGroup.allowSwitchOff = false;
				};
			});

			if (selectLastDecisionAfterUpdate) {
				SetCurrentDecision(dataHolder.decisions.Count - 1);
				selectLastDecisionAfterUpdate = false;
			}
			else if (curDecIndex >= 0) SetCurrentDecision(curDecIndex);

			doUpdateDecisionUI = false;
		}
	}

	// Not currently being used
	void Undo() {
		if (undoStack.Count == 0) return;
		var action = undoStack.Pop();
		if (action.nullCheck != null) action.doOnUndo();
	}

	void UpdateDecisionListUI() {
		doUpdateDecisionUI = true;	// We need it in the update loop or else an extra decision button is created
	}

	void SetCurrentDecision(int decIndex) {
		decIndex = Mathf.Clamp(decIndex, 0, dataHolder.decisions.Count - 1);
		if (!doUpdateDecisionUI) RefreshCurrentDecision();
		curDecIndex = decIndex;
		if (!decisionUI.gameObject.activeSelf) decisionUI.gameObject.SetActive(true);
		decisionUI.Initialize(dataHolder.decisions[curDecIndex]);
		decisionsLayout.Elements[curDecIndex].isOn = true;
		//decisionsLayout.Elements[curDecIndex].SetToggleAnimState(true);
	}

	void RevertChanges() {
		ShowLoading = true;
		
		RestClient.Get(url: CMS_PARAMS_URL + JSON_SUFFIX).Then(res => {
			JObject dataObj = JObject.Parse(res.Text);
			try {
				cmsParams = JsonUtility.FromJson<CMSParams>(dataObj.ToString());
			} catch (System.Exception) {
				Debug.Log("Error parsing CMS params from DB");
			}
		});

		RestClient.Get(url: GAME_DATA_URL + JSON_SUFFIX).Then(res => {
			JObject dataObj = JObject.Parse(res.Text);
			try {
				dataHolder = JsonUtility.FromJson<GameDataHolder>(dataObj.ToString());
			} catch (System.Exception) {
				Debug.Log("Error parsing decisions from DB");
			}
			UpdateDecisionListUI();
			ShowLoading = false;
		});
	}

	void RefreshCurrentDecision() {
		if (curDecIndex < 0 || curDecIndex >= dataHolder.decisions.Count) return;
		changedDecIndices.Add(curDecIndex);
		decisionUI.Refresh();
		decisionsLayout.Elements[curDecIndex].SetText(dataHolder.decisions[curDecIndex].decisionText);
	}

	void UploadChanges(System.Action thenWhat = null) {
		ShowLoading = true;
		RefreshCurrentDecision();
		List<int> cdiList = new List<int>(changedDecIndices);
		
		DoTheThing();

		void DoTheThing() {
			if (cdiList.Count <= 0) {
				ShowLoading = false;
				thenWhat?.Invoke();
				return;
			}
			int curIndex = cdiList[0];
			changedDecIndices.Remove(curIndex);
			cdiList.RemoveAt(0);
			UpdateLists();
			RestClient.Put(url: decisionsURL + "/" + curIndex + JSON_SUFFIX, dataHolder.decisions[curIndex]).Then(_ => DoTheThing());	// Recurse
		}
	}

	void AddDecision() {
		UploadChanges(() => {
			ShowLoading = true;
			RestClient.Put(url: decisionsURL + "/" + dataHolder.decisions.Count + JSON_SUFFIX, new Decision() {
				decisionText = "New decision",
				choices = new List<Choice>() { new Choice(), new Choice() }
			}).Then(res => {
				selectLastDecisionAfterUpdate = true;
				RevertChanges();
			});
		});
	}

	void RemoveCurrentDecision() {
		if (curDecIndex < 0 || curDecIndex >= dataHolder.decisions.Count) return;
		UploadChanges(() => {
			ShowLoading = true;
			dataHolder.decisions.RemoveAt(curDecIndex);
			RestClient.Put(url: GAME_DATA_URL + JSON_SUFFIX, dataHolder)
			//RestClient.Delete(url: decisionsURL + "/" + curDecIndex + JSON_SUFFIX)
				.Then(res => RevertChanges());
		});
	}

	void UpdateLists() {
		if (dataHolder.lists.HasChanged) {
			RestClient.Put(url: listsURL + JSON_SUFFIX, dataHolder.lists);
			dataHolder.lists.HasChanged = false;
		}
	}
}