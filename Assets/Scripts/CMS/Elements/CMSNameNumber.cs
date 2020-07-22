using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CMSNameNumber : CMSLayoutElement {
	[SerializeField] CMSInputField nameField;
	[SerializeField] TMP_Text numText;
	[SerializeField] Button upButton, downButton;
	[SerializeField] int changeBy = 1;
	[SerializeField] bool skipZero = false;

	public string Name {
		get { return nameField == null ? string.Empty : nameField.Text; }
	}
	public int Number { get; private set; }

	public delegate void ValueChangeEvent(CMSNameNumber nameValue);
	public ValueChangeEvent OnValueChanged;

	protected override void Awake() {
		base.Awake();
		if (upButton) upButton.onClick.AddListener(() => ValueUpDown(true));
		if (downButton) downButton.onClick.AddListener(() => ValueUpDown(false));
	}

	public void Initialize(string startText = "", int startNum = 1) {
		nameField.Initialize(startText);
		Number = startNum;
		UpdateNumText();
	}

	void ValueUpDown(bool wentUp) {
		void DoTheThing() {
			Number += wentUp ? changeBy : -changeBy;
		}
		DoTheThing();
		if (skipZero && Number == 0) DoTheThing();
		UpdateNumText();
		OnValueChanged?.Invoke(this);
	}

	void UpdateNumText() {
		if (!numText) return;
		numText.text = Number.ToString(Number > 0 ? "+0" : "#0");
	}
}