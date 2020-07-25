using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMSZoom : MonoBehaviour {
	[SerializeField] Canvas canvas;
	[SerializeField] CMSButton zoomIn, zoomOut, zoomReset;
	[SerializeField] float zoomStep = 0.1f;

	const string ZOOM_LAST = "Zoom scale factor", ZOOM_RESET = "Zoom scale reset";

	float CurScale {
		get => canvas.scaleFactor;
		set => PlayerPrefs.SetFloat(ZOOM_LAST, canvas.scaleFactor = value);
	}

	private void Awake() {
		float zoomOutStep = 1 - zoomStep;
		float zoomInStep = 1f / zoomOutStep;
		if (zoomIn) zoomIn.OnClick.AddListener(() => CurScale *= zoomInStep);
		if (zoomOut) zoomOut.OnClick.AddListener(() => CurScale *= zoomOutStep);
		if (zoomReset) zoomReset.OnClick.AddListener(() => CurScale = PlayerPrefs.GetFloat(ZOOM_RESET));
	}

	private void Start() {
		if (!PlayerPrefs.HasKey(ZOOM_RESET)) PlayerPrefs.SetFloat(ZOOM_RESET, CurScale);
		if (PlayerPrefs.HasKey(ZOOM_LAST)) canvas.scaleFactor = PlayerPrefs.GetFloat(ZOOM_LAST);
	}

#if UNITY_EDITOR
	private void OnGUI() {
		if (GUILayout.Button("Clear prefs")) PlayerPrefs.DeleteAll();
	}
#endif
}