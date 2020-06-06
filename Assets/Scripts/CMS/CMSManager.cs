using DecisionFramework;
using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMSManager : MonoBehaviour {
	const string AUTH_KEY = "AIzaSyBUfg4avo3uIxInDSOYDnGeiMDCSu4B6C0",
		//DATABASE_URL = "https://test-project-bcd07.firebaseio.com/game-data",
		DATABASE_URL = "https://reignslike-prototype.firebaseio.com/game-data",
		DECISIONS_SUFFIX = "/decisions/",
		TRAITS_SUFFIX = "/traits/",
		JSON_SUFFIX = ".json";

	string decisionsURL, traitsURL;

	[SerializeField] GUISkin guiSkin;
	[SerializeField] DecisionsHolder holder;

	private void Awake() {
		decisionsURL = DATABASE_URL + DECISIONS_SUFFIX;
		traitsURL = DATABASE_URL + TRAITS_SUFFIX;
	}

	void ResetDecisions() {
		RestClient.Put(url: DATABASE_URL + JSON_SUFFIX, holder).Then(res => {
			if (!string.IsNullOrEmpty(res.Error)) Debug.LogError(res.Error);
		});
	}

	void GetDecision() {
		RestClient.Get(url: DATABASE_URL + DECISIONS_SUFFIX + replaceIndex + JSON_SUFFIX).Then(res => {
			try {
				Decision newDec = JsonUtility.FromJson<Decision>(res.Text);
				decGUI = new DecisionGUI(newDec);
			} catch (System.Exception) {
				print("Can't fetch decision");
			}
		});
	}

	// CMS GUI
	string replaceIndex = "0";
	DecisionGUI decGUI = null;
	Vector2 scrollPos;
	const float GUI_WIDTH = 200f, GUI_SPACING = 50f;

	private void OnGUI() {
		if (guiSkin) GUI.skin = guiSkin;

		float guiWidth = 800f;
		Rect areaRect = new Rect(0, 0, guiWidth, guiWidth / Camera.main.aspect);
		GUILayout.BeginArea(areaRect);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / areaRect.width, Screen.height / areaRect.height, 1));
		GUILayout.BeginHorizontal();

		GUILayout.BeginVertical(GUILayout.Width(GUI_WIDTH));

		if (GUILayout.Button("Reset")) ResetDecisions();
		replaceIndex = GUILayout.TextField(replaceIndex);
		if (GUILayout.Button("Get")) GetDecision();
		if (GUILayout.Button("Update decision")) {
			RestClient.Put(url: DATABASE_URL + DECISIONS_SUFFIX + replaceIndex + JSON_SUFFIX, decGUI.ShownDecision);
		}
		GUILayout.EndVertical();
		GUILayout.Space(GUI_SPACING);

		scrollPos = GUILayout.BeginScrollView(scrollPos);
		decGUI?.OnGUI();

		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}