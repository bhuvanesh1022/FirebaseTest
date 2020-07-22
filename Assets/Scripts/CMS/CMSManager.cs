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
public enum CMSSearchType { NONE, ATTRIBUTE, SPEAKER, STAT }

public abstract class CMSInputField : MonoBehaviour {
	[SerializeField] CMSTextType textType;
	[SerializeField] CMSSearchType searchType;

	public CMSTextType TextType {
		get { return textType; }
	}
	public CMSSearchType SearchType {
		get { return searchType; }
	}

	public abstract string Text { get; }
	public abstract void SetSearchSpace(List<string> searchSpace);
	public virtual void SetCharacterLimit(int charLimit) { }

	public delegate void ConfirmEntryEvent(CMSInputField field);
	public ConfirmEntryEvent OnConfirmEntry;

	public void Initialize(string startText) {
		CMSManager.Instance.RegisterListElement(this);
		InitializeCustom(startText);
	}

	protected abstract void InitializeCustom(string startText);
}
	

public class CMSManager : MonoBehaviour {
	public static CMSManager Instance { get; private set; }

	[SerializeField] GUISkin guiSkin;
	[SerializeField] DecisionsHolder decisionsHolder;
	[SerializeField] GameObject introTextObject;
	[SerializeField] CMSAnimatedActivatable loadingOverlay;
	[SerializeField] string reportingURL;

	// UI elements
	[SerializeField] CMSDecision decisionUI;
	[SerializeField] CMSButton revertButton, uploadChangesButton, addDecisionButton, removeDecisionButton, resetButton, resetLocalButton;
	[SerializeField] CMSDialog commonDialog;
	[SerializeField] CMSLayoutSetup decisionsLayoutSetup;
	[SerializeField] CMSToggleButton decisionButtonPrefab;
	[SerializeField] ToggleGroup decisionListGroup;

	public delegate void CMSInputEvent(CMSKeypress input);
	public static CMSInputEvent OnCMSInput;

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

	List<Decision> decisions = new List<Decision>();
	ListsHolder listsHolder = new ListsHolder();
	List<int> changedDecIndices = new List<int>();
	int curDecIndex = -1;
	string decisionsURL, listsURL;
	Stack<UndoAction> undoStack = new Stack<UndoAction>();

	const string DECISIONS_SUFFIX = "decisions",
		LISTS_SUFFIX = "lists",
		JSON_SUFFIX = ".json",
		DATABASE_URL = "https://test-project-bcd07.firebaseio.com/game-data";
		//DATABASE_URL = "https://reignslike-prototype.firebaseio.com/game-data";

	// Public methods

	public void RegisterListElement(CMSInputField listElement) {
		List<string> curList = listsHolder.GetList(listElement.SearchType);
		listElement.SetSearchSpace(curList);
		listElement.SetCharacterLimit(listsHolder.GetCharacterLimit(listElement.TextType));

		if (listElement.GetType() == typeof(CMSInputField)) (listElement as CMSInputField).OnConfirmEntry += field => {
			if (!curList.Contains(field.Text)) { // Adding a new list entry
				// TODO: Add confirmation dialog
				listsHolder.AddEntry(field.SearchType, field.Text);
				RestClient.Put(url: listsURL + JSON_SUFFIX, listsHolder);
			}
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
		decisionsURL = DATABASE_URL + "/" + DECISIONS_SUFFIX;
		listsURL = DATABASE_URL + "/" + LISTS_SUFFIX;
		Instance = this;

		if (resetButton || resetLocalButton) try {
#if UNITY_EDITOR
				resetButton.OnClick.AddListener(ResetFromLocal);
				resetLocalButton.OnClick.AddListener(ResetFromRemote);
#else
				resetButton.gameObject.SetActive(false);
				resetLocalButton.gameObject.SetActive(false);
#endif
			} catch (System.Exception) { }

		if (uploadChangesButton) uploadChangesButton.OnClick.AddListener(UploadChanges);
		if (revertButton) revertButton.OnClick.AddListener(() => RevertChanges());
		if (addDecisionButton) addDecisionButton.OnClick.AddListener(AddDecision);
		if (removeDecisionButton) removeDecisionButton.OnClick.AddListener(RemoveCurrentDecision);

		if (decisionsLayoutSetup) {
			decisionsLayout = new CMSLayout<CMSToggleButton>(decisionsLayoutSetup, decisionButtonPrefab);
			if (decisionListGroup) {
				decisionsLayout.OnCellAddedOrRemoved += (decToggle, wasAdded) => {
					if (wasAdded) {
						decToggle.Toggle.group = decisionListGroup;
						// TODO: Select this new decision and navigate to it
					}
				};
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

	bool doUpdateDecisionUI = false, selectLastAddedDecision = false;

	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) OnCMSInput?.Invoke(CMSKeypress.UP);
		if (Input.GetKeyDown(KeyCode.DownArrow)) OnCMSInput?.Invoke(CMSKeypress.DOWN);
		if (Input.GetKeyDown(KeyCode.Return)) OnCMSInput?.Invoke(CMSKeypress.CONFIRM);
		//if (Input.GetKeyUp(KeyCode.Z) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))) Undo();

		if (doUpdateDecisionUI) {
			doUpdateDecisionUI = false;
			decisionsLayout.Clear();

			for (int d = 0; d < decisions.Count; d++) {
				Decision dec = decisions[d];
				CMSToggleButton newButton = decisionsLayout.Add();
				newButton.SetText(dec.decisionText);
				newButton.Toggle.group = decisionListGroup;
				int curIndex = d;
				newButton.OnSelected += t => SetCurrentDecision(curIndex);
			}

			if (selectLastAddedDecision) {
				SetCurrentDecision(decisions.Count - 1);
				selectLastAddedDecision = false;
			}
			else if (curDecIndex >= 0) SetCurrentDecision(curDecIndex);
		}
	}

	void Undo() {
		if (undoStack.Count == 0) return;
		var action = undoStack.Pop();
		if (action.nullCheck != null) action.doOnUndo();
	}

	void UpdateDecisionListUI() {
		doUpdateDecisionUI = true;	// We need it in the update loop or else an extra decision button is created
	}

	void SetCurrentDecision(int decIndex) {
		decIndex = Mathf.Clamp(decIndex, 0, decisions.Count - 1);
		UpdateCurrentDecision();
		curDecIndex = decIndex;
		if (!decisionUI.gameObject.activeSelf) decisionUI.gameObject.SetActive(true);
		decisionUI.Initialize(decisions[curDecIndex]);
		decisionsLayout.Elements[curDecIndex].SetToggleAnimState(true);
	}

	void ResetFromLocal() {
		ShowLoading = true;
		RestClient.Put(url: DATABASE_URL + JSON_SUFFIX, decisionsHolder).Then(_ => RevertChanges());
	}

	void ResetFromRemote() {
		if (decisionsHolder) {
			decisionsHolder.decisions = decisions;
			decisionsHolder.lists = listsHolder;
		}
	}

	void RevertChanges(int setIndex = -1) {
		ShowLoading = true;
		RestClient.Get(url: DATABASE_URL + JSON_SUFFIX).Then(res => {
			//Debug.LogError("Parsing data");
			JObject dataObj = JObject.Parse(res.Text);
			//Debug.LogError("Creating decisions");
			decisions = dataObj[DECISIONS_SUFFIX].ToObject<List<Decision>>();
			//Debug.LogError("Creating lists");
			listsHolder = JsonUtility.FromJson<ListsHolder>(dataObj[LISTS_SUFFIX].ToString());
			//Debug.LogError("Done");

			UpdateDecisionListUI();

			ShowLoading = false;
		});
	}

	void AddDecision() {
		decisions.Add(new Decision() {
			decisionText = "New decision",
			choices = new List<Choice>() { new Choice(), new Choice() }
		});
		selectLastAddedDecision = true;
		UpdateDecisionListUI();
	}

	void UpdateCurrentDecision() {
		if (curDecIndex < 0 || curDecIndex >= decisions.Count) return;
		decisionUI.Refresh();
		if (!changedDecIndices.Contains(curDecIndex)) changedDecIndices.Add(curDecIndex);
		decisionsLayout.Elements[curDecIndex].SetText(decisions[curDecIndex].decisionText);
	}

	void UploadChanges() {
		ShowLoading = true;
		UpdateCurrentDecision();
		DoTheThing();

		void DoTheThing() {
			if (changedDecIndices.Count <= 0) {
				ShowLoading = false;
				return;
			}
			int curIndex = changedDecIndices[0];
			changedDecIndices.RemoveAt(0);
			RestClient.Put(url: decisionsURL + "/" + curIndex + JSON_SUFFIX, decisions[curIndex]).Then(_ => DoTheThing());	// Recurse
		}
	}

	void RemoveCurrentDecision() {
		if (decisions.Count > 0 && curDecIndex >= 0) {
			ShowLoading = true;
			RestClient.Delete(url: decisionsURL + "/" + curDecIndex + JSON_SUFFIX).Then(res => RevertChanges()).Then(() => ShowLoading = false);
		}
	}
}