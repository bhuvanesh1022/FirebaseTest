using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CMSToggleButton : CMSGridCellElement {
	[SerializeField] string toggleBoolParam;
	[SerializeField] TMP_Text text;

	public delegate void SelectedEvent(CMSToggleButton selected);
	public SelectedEvent OnSelected;

	public Toggle Toggle { get; private set; }
	
	Animator anim;
	
	protected override void Awake() {
		base.Awake();
		Toggle = GetComponent<Toggle>();
		anim = GetComponent<Animator>();
		if (Toggle && anim) Toggle.onValueChanged.AddListener(val => {
			if (val) OnSelected?.Invoke(this);
			anim.SetBool(toggleBoolParam, val);
		});
	}

	public void SetText(string buttonText = "") {
		if (text) text.text = buttonText;
	}

	public void SetToggleAnimState(bool isOn) {
		anim.SetBool(toggleBoolParam, isOn);
	}
}