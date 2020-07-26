using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class CMSInputDropdown : CMSInputBase {
	[SerializeField] string addOptionText;

	public override string Text { get => dropdown.options.Count > 0 ? dropdown.options[dropdown.value].text : string.Empty; }

	string oldText = string.Empty;
	TMP_Dropdown dropdown;
	int lastValue = 0;

	private void Awake() {
		dropdown = GetComponent<TMP_Dropdown>();
		dropdown.onValueChanged.AddListener(ValueChanged);
	}

	protected override void InitializeCustom(string startText) {
		int foundIndex = dropdown.options.FindIndex(findOpt => findOpt.text == startText);	// -1 if not found
		dropdown.SetValueWithoutNotify(Mathf.Max(foundIndex, 0));
	}

	public override void SetSearchSpace(List<string> options) {
		dropdown.ClearOptions();
		List<string> optionsToSet = new List<string>(options);
		if (!string.IsNullOrEmpty(addOptionText)) optionsToSet.Add(addOptionText);
		dropdown.AddOptions(optionsToSet);
	}

	void ValueChanged(int newValue) {
		OnConfirmEntry?.Invoke(this);
		int undoValue = lastValue;
		lastValue = dropdown.value;
		CMSManager.Instance.AddUndoAction(this, () => dropdown.SetValueWithoutNotify(undoValue));
	}

	public override void SetCharacterLimit(int charLimit) { }
}