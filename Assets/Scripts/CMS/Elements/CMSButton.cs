using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CMSButton : MonoBehaviour {
	[SerializeField] TMP_Text buttonText;
	[SerializeField] Image buttonImage;
	[SerializeField] bool showDialog;
	[SerializeField] string dialogText;

	public string Text {
		get { return buttonText ? buttonText.text : null; }
		set { if (buttonText) buttonText.text = value; }
	}

	public Sprite Image {
		get { return buttonImage ? buttonImage.sprite : null; }
		set { if (buttonImage) buttonImage.sprite = value; }
	}

	Button button;

	public Button.ButtonClickedEvent OnClick { get; private set; } = new Button.ButtonClickedEvent();

	private void Awake() {
		button = GetComponent<Button>();
	}

	private void Start() {
		if (string.IsNullOrEmpty(dialogText)) dialogText = "<b>" + Text + "</b>\nAre you sure?";

		if (showDialog && CMSManager.Instance.CommonDialog)
			button.onClick.AddListener(() => CMSManager.Instance.CommonDialog.Show(dialogText, "Yes", "No", () => OnClick?.Invoke()));
		else
			button.onClick.AddListener(() => OnClick?.Invoke());
	}
}
