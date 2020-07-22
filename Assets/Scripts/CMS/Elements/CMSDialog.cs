using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class CMSDialog : MonoBehaviour {
	[SerializeField] CMSButton confirmButton, cancelButton;
	[SerializeField] TMP_Text dialogText;
	[SerializeField] Transform toScale;
	[SerializeField] float showTime = 0.4f, hideTIme = 0.25f;
	[SerializeField] MaskableGraphic blocker;

	public enum Response { CONFIRM, CANCEL }

	public delegate void ResponseEvent(Response response);
	public ResponseEvent OnResponse;

	System.Action confirmAction = null;
	Vector3 dialogScale;
	Color blockerColor;
	Tween scaleTween = null;

	private void Awake() {
		if (confirmButton) confirmButton.OnClick.AddListener(() => ButtonPressed(Response.CONFIRM));
		if (cancelButton) cancelButton.OnClick.AddListener(() => ButtonPressed(Response.CANCEL));

		if (blocker) {
			blockerColor = blocker.color;
			blocker.gameObject.SetActive(false);
		}
		if (!toScale) toScale = transform;
		dialogScale = toScale.localScale;
		toScale.localScale = Vector3.zero;
	}

	public void Show(string text, string confirm, System.Action doOnConfirm = null) {
		Show(text, confirm, string.Empty, doOnConfirm);
	}

	public void Show(string text, string confirm, string cancel, System.Action doOnConfirm = null) {
		if (dialogText) dialogText.text = text;
		if (confirmButton) confirmButton.Text = confirm;
		if (cancelButton) {
			cancelButton.gameObject.SetActive(!string.IsNullOrEmpty(cancel));
			cancelButton.Text = cancel;
		}
		confirmAction = doOnConfirm;
		ShowHide(true);
	}

	void ButtonPressed(Response response) {
		OnResponse?.Invoke(response);
		if (response == Response.CONFIRM && confirmAction != null) confirmAction.Invoke();
		confirmAction = null;
		ShowHide(false);
	}

	void ShowHide(bool doShow) {
		if (scaleTween != null && scaleTween.IsPlaying()) scaleTween.Kill();

		if (doShow) toScale.DOScale(dialogScale, showTime).SetEase(Ease.OutBounce);
		else toScale.DOScale(Vector3.zero, hideTIme).SetEase(Ease.Linear);

		if (blocker) {
			Tween blockerTween = blocker.DOColor(doShow ? blockerColor : Color.clear, Mathf.Min(showTime, hideTIme)).SetEase(Ease.Linear);
			if (doShow) blocker.gameObject.SetActive(true);
			else blockerTween.OnComplete(() => blocker.gameObject.SetActive(false));
		}
	}
}