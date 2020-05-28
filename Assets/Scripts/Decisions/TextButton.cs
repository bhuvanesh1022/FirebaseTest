using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DecisionFramework;

[RequireComponent(typeof(Button))]
public class TextButton : MonoBehaviour {
	[SerializeField] TMP_Text buttonText;
	[SerializeField] float timeBeforeAppearing = 0.5f;

	public delegate void ButtonPressedEvent(TextButton button);
	public ButtonPressedEvent OnButtonPressed;

	Button button;
	List<MaskableGraphic> renderers = new List<MaskableGraphic>();

	public string ButtonText {
		get { return buttonText ? buttonText.text : null; }
		set { if (buttonText) buttonText.text = value; }
	}

	private void Awake() {
		button = GetComponent<Button>();
		button.onClick.AddListener(Clicked);
		GetComponentsInChildren(false, renderers);
	}

	private void OnEnable() {
		StartCoroutine(PostEnableCR());
	}

	IEnumerator PostEnableCR() {
		yield return new WaitForSeconds(timeBeforeAppearing);
		renderers.ForEach(rend => rend.enabled = true);
	}

	private void OnDisable() {
		renderers.ForEach(rend => rend.enabled = false);
	}

	void Clicked() {
		OnButtonPressed?.Invoke(this);
	}
}
