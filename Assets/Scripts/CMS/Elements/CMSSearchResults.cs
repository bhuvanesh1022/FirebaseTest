using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CMSSearchResults : MonoBehaviour {
	[SerializeField] Button resultPrefab;
	[SerializeField] LayoutGroup resultsLayout;
	[SerializeField] MaskableGraphic listBG;
	[SerializeField] GameObject noResultsPrompt;

	public delegate void ResultSelectedEvent(string selection);
	public ResultSelectedEvent OnResultSelected;

	public string CurResult {
		get { return HighlightedResult ? HighlightedResult.Label : string.Empty; }
	}

	[RequireComponent(typeof(Button))]
	public class Result : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler {
		[SerializeField] TMP_Text label;

		public string Label {
			get { return label ? label.text : null; }
			set { if (label) label.text = value; }
		}

		public delegate void SelectedEvent(Result clicked);
		public SelectedEvent OnSelected;

		public delegate void MouseoverEvent(Result mousedOver);
		public MouseoverEvent OnMouseover;

		Button button;
		Color origColor;
		ColorBlock buttonColors;

		private void Awake() {
			button = GetComponent<Button>();
			if (!label) label = GetComponentInChildren<TMP_Text>();
			origColor = (buttonColors = button.colors).normalColor;
		}

		public void SetFakeHighlight(bool isOn) {
			buttonColors.normalColor = isOn ? button.colors.highlightedColor : origColor;
			button.colors = buttonColors;
		}

		public void Select() {
			OnSelected?.Invoke(this);
		}

		public void OnPointerDown(PointerEventData eventData) {
			Select();
		}
		
		public void OnPointerEnter(PointerEventData eventData) {
			OnMouseover?.Invoke(this);
		}
	}
	
	List<string> resultsSpace = new List<string>();
	List<Result> curResults = new List<Result>();
	int _highlightIndex = -1;

	bool ShowNoResultsPrompt {
		get { return noResultsPrompt ? noResultsPrompt.activeSelf : false; }
		set { if (noResultsPrompt) noResultsPrompt.SetActive(value); }
	}

	int HighlightIndex {
		get { return _highlightIndex; }
		set {
			Result oldResult = HighlightedResult;
			_highlightIndex = value;
			if (curResults.Count == 0) return;
			if (oldResult) oldResult.SetFakeHighlight(false);
			if (value < 0 || value >= curResults.Count) return;
			curResults[value].SetFakeHighlight(true);
		}
	}

	Result HighlightedResult {
		get { return _highlightIndex < 0 || _highlightIndex >= curResults.Count ? null : curResults[_highlightIndex]; }
	}

	private void Awake() {
		ShowNoResultsPrompt = false;
		if (listBG) listBG.enabled = false;
	}

	private void OnEnable() {
		CMSManager.Instance.OnCMSInput += OnInput;
	}

	private void OnDisable() {
		CMSManager.Instance.OnCMSInput -= OnInput;
	}

	public void Initialize(List<string> results = null) {
		if (results == null) resultsSpace.Clear();
		else {
			resultsSpace = results;
			resultsSpace.Sort();
		}
		ClearResults();
		ShowNoResultsPrompt = false;
	}

	public void Search(string text) {
		if (!resultPrefab || !resultsLayout || resultsSpace.Count == 0) return;
		ClearResults();
		List<string> curResultStrings = string.IsNullOrEmpty(text) ? resultsSpace : resultsSpace.FindAll(result => result.Contains(text));
		ShowNoResultsPrompt = curResultStrings.Count == 0;
		if (listBG) listBG.enabled = curResultStrings.Count > 0;
		curResultStrings.ForEach(opt => {
			Result newRes = Instantiate(resultPrefab, resultsLayout.transform).gameObject.AddComponent<Result>();
			curResults.Add(newRes);
			newRes.Label = opt;
			newRes.OnSelected += res => {
				OnResultSelected?.Invoke(res.Label);
				ClearResults();
			};
			newRes.OnMouseover += res => HighlightIndex = -1;
		});
		HighlightIndex = -1;
	}

	public void ClearResults() {
		curResults.ForEach(ob => {
			if (ob) Destroy(ob.gameObject);
		});
		curResults.Clear();
		ShowNoResultsPrompt = false;
		if (listBG) listBG.enabled = false;
	}

	void OnInput(CMSKeypress input) {
		if (curResults.Count == 0) return;
		switch (input) {
		case CMSKeypress.DOWN:
			HighlightIndex = (HighlightIndex + 1) % curResults.Count;
			break;
		case CMSKeypress.UP:
			if (HighlightIndex > 0) HighlightIndex--;
			else HighlightIndex = curResults.Count - 1;
			break;
		case CMSKeypress.CONFIRM:
			HighlightedResult.Select();
			break;
		default:
			break;
		}
	}
}