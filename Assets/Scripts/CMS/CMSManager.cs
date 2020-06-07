using DecisionFramework;
using Proyecto26;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class CMSManager : MonoBehaviour {
	const string DECISIONS_SUFFIX = "/decisions",
		TRAITS_SUFFIX = "/traits",
		JSON_SUFFIX = ".json",
		DATABASE_URL = "https://test-project-bcd07.firebaseio.com/game-data";
		//DATABASE_URL = "https://reignslike-prototype.firebaseio.com/game-data";

	public static List<Decision> GetAllDecisions() {
		List<Decision> retval = new List<Decision>();
		RestClient.Get(url: DATABASE_URL + DECISIONS_SUFFIX + JSON_SUFFIX).Then(res => {
			JObject decsObj = JObject.Parse(res.Text);
			List<JToken> tokens = new List<JToken>(decsObj.Children());
			tokens.ForEach(token => retval.Add(JsonUtility.FromJson<Decision>(token.First.ToString())));
		});
		return retval;
	}

	[SerializeField] GUISkin guiSkin;
	[SerializeField] DecisionsHolder holder;
	[SerializeField] float guiWidth = 800f;

	[System.Serializable]
	class DecisionIDHolder {
		public string id;
		public Decision decision;
	}
	List<DecisionIDHolder> decisions = new List<DecisionIDHolder>();
	int curDecIndex = -1;
	string decisionsURL, traitsURL;

	private void Awake() {
		decisionsURL = DATABASE_URL + DECISIONS_SUFFIX;
		traitsURL = DATABASE_URL + TRAITS_SUFFIX;
	}

	void ResetDecisions() {
		holder.decisions.ForEach(dec => {
			RestClient.Post(url: decisionsURL + JSON_SUFFIX, dec);
		});
		RefreshDecisions(0);
	}

	void RefreshDecisions(int setIndex = -1) {
		RestClient.Get(url: decisionsURL + JSON_SUFFIX).Then(res => {
			JObject decsObj = JObject.Parse(res.Text);
			List<JToken> tokens = new List<JToken>(decsObj.Children());
			decisions.Clear();
			tokens.ForEach(token => {
				decisions.Add(new DecisionIDHolder() {
					id = token.Path,
					decision = JsonUtility.FromJson<Decision>(token.First.ToString())
				});
			});
			if (setIndex < 0) setIndex = curDecIndex;
			curDecIndex = Mathf.Clamp(setIndex, 0, decisions.Count - 1);
		});
	}

	void AddDecision() {
		RestClient.Post(url: decisionsURL + JSON_SUFFIX, new Decision()).Then(res => RefreshDecisions(int.MaxValue));
	}

	void UpdateCurrentDecision() {
		RestClient.Put(url: decisionsURL + "/" + decisions[curDecIndex].id + JSON_SUFFIX, decisions[curDecIndex].decision);
	}

	void RemoveCurrentDecision() {
		if (decisions.Count > 0 && curDecIndex >= 0) RestClient.Delete(url: decisionsURL + "/" + decisions[curDecIndex].id + JSON_SUFFIX).Then(res => RefreshDecisions());
	}

	// CMS GUI
	const float MENU_WIDTH = 200f, GUI_SPACING = 50f, GUI_MARGIN = 20f;

	private void OnGUI() {
		if (guiSkin) GUI.skin = guiSkin;
		
		Rect areaRect = new Rect(0, 0, guiWidth, guiWidth / Camera.main.aspect);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(Screen.width / areaRect.width, Screen.height / areaRect.height, 1));
		GUILayout.BeginArea(areaRect);
		GUILayout.BeginHorizontal();

		GUILayout.Space(GUI_MARGIN);
		GUILayout.BeginVertical(GUILayout.Width(MENU_WIDTH));
		GUILayout.Space(GUI_MARGIN);

#if UNITY_EDITOR
		if (GUILayout.Button("<color=red>Reset (upload defaults)</color>")) ResetDecisions();
#endif

		if (GUILayout.Button(curDecIndex < 0 ? "Download" : "Refresh")) RefreshDecisions();
		GUILayout.FlexibleSpace();

		if (curDecIndex >= 0 && GUILayout.Button("Add decision")) AddDecision();

		if (decisions.Count > 1) {
			GUILayout.Label(string.Format("  Decision {0}/{1}\n  <color=#aaaaaa>{2}</color>", curDecIndex + 1, decisions.Count, decisions[curDecIndex].id));

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("< Prev")) {
				curDecIndex--;
				if (curDecIndex < 0) curDecIndex = decisions.Count - 1;
			}
			if (GUILayout.Button("Next >")) curDecIndex = (curDecIndex + 1) % decisions.Count;
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Update this decision")) UpdateCurrentDecision();
			if (GUILayout.Button("Remove this decision")) RemoveCurrentDecision();
		}
		GUILayout.Space(GUI_MARGIN);
		GUILayout.EndVertical();

		GUILayout.Space(GUI_SPACING);

		if (curDecIndex >= 0 && curDecIndex < decisions.Count) DecisionGUI.ShowDecision(ref decisions[curDecIndex].decision);

		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
}