using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class CMSInputText : CMSInputField {
	[SerializeField] CMSSearchResults searchResults;
	[SerializeField] TMP_Text charLimitDisplay;
	[SerializeField] bool enforceCharacterLimit = false;
	[SerializeField] Material overLimitMaterial;

	public override string Text { get => inputField.text; }
	
	TMP_InputField inputField;
	string lastText = string.Empty;
	int characterLimit = 0;
	Material underLimitMaterial;

	private void Awake() {
		inputField = GetComponent<TMP_InputField>();
		inputField.onSelect.AddListener(OnTextChanged);
		inputField.onValueChanged.AddListener(OnTextChanged);
		inputField.onSubmit.AddListener(_ => {
			if (searchResults && !string.IsNullOrEmpty(searchResults.CurResult)) inputField.text = searchResults.CurResult;
			OnFocusLost();
		});
		inputField.onDeselect.AddListener(_ => Invoke("OnFocusLost", 0.1f));
		if (searchResults) searchResults.OnResultSelected += text => inputField.text = text;
		if (charLimitDisplay) {
			charLimitDisplay.gameObject.SetActive(false);
			underLimitMaterial = charLimitDisplay.fontSharedMaterial;
		}
	}

	protected override void InitializeCustom(string startText) {
		StartCoroutine(InitializeCR(startText));
	}

	IEnumerator InitializeCR(string startText) {
		yield return null;
		inputField.SetTextWithoutNotify(lastText = startText);
	}

	public override void SetSearchSpace(List<string> searchSpace) {
		if (searchResults) searchResults.Initialize(searchSpace);
	}

	public override void SetCharacterLimit(int charLimit) {
		characterLimit = charLimit;
		inputField.characterLimit = enforceCharacterLimit ? charLimit : 0;
	}

	void OnTextChanged(string text) {
		if (searchResults) searchResults.Search(text);
		if (charLimitDisplay && characterLimit > 0) {
			charLimitDisplay.gameObject.SetActive(true);
			charLimitDisplay.text = string.Format("{0} / {1}", Text.Length, characterLimit);
			if (overLimitMaterial) {
				bool isOverLimit = Text.Length > characterLimit;
				if (isOverLimit != (charLimitDisplay.fontSharedMaterial == overLimitMaterial))  // Set font material for over limit
					charLimitDisplay.fontSharedMaterial = isOverLimit ? overLimitMaterial : underLimitMaterial;
			}
		}
	}

	void OnFocusLost() {
		OnConfirmEntry?.Invoke(this);
		if (searchResults) searchResults.ClearResults();
		if (charLimitDisplay) charLimitDisplay.gameObject.SetActive(false);

		// Add undo action
		string setTextTo = lastText;
		lastText = inputField.text;
		CMSManager.Instance.AddUndoAction(this, () => {
			inputField.text = setTextTo;
		});
	}
}