using UnityEngine;
using DecisionFramework;
using System.Collections.Generic;

public class CMSGUIDecision {
	const float PADDING = 20f, LABEL_WIDTH = 100f, EFFECT_LABEL_WIDTH = 75f;

	static Vector2 scrollPos;

	public static void ShowDecision(ref Decision decision, float choiceWidth = 320f) {
		if (decision == null) {
			scrollPos = Vector2.zero;
			return;
		}

		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.BeginVertical();
		GUILayout.Space(PADDING);

		// Decision data
		GUILayout.BeginHorizontal();
		GUILayout.Label("Speaker", GUILayout.Width(LABEL_WIDTH));
		decision.speakerName = GUILayout.TextField(decision.speakerName);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Dialogue", GUILayout.Width(LABEL_WIDTH));
		decision.decisionText = GUILayout.TextArea(decision.decisionText, GUILayout.MinHeight(60f));
		GUILayout.EndHorizontal();

		void Separator(bool withBar = false) {
			if (withBar) {
				GUILayout.Space(25f);
				GUILayout.Box(GUIContent.none, GUILayout.Height(1f));
				GUILayout.Space(25f);
			}
			else GUILayout.Space(40f);
		}

		// Choice data
		GUILayout.BeginHorizontal();

		for (int c = 0; c < decision.choices.Count; c++) {
			//Separator(c > 0);
			GUILayout.BeginVertical(GUILayout.Width(choiceWidth));

			GUILayout.BeginHorizontal();
			GUILayout.Label("Choice " + (c + 1).ToString(), GUILayout.Width(LABEL_WIDTH));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Remove choice", GUILayout.Width(120f))) {
				decision.choices.RemoveAt(c);
				break;
			}
			GUILayout.EndHorizontal();
			decision.choices[c].choiceText = GUILayout.TextField(decision.choices[c].choiceText);

			GUILayout.Space(15f);

			void PropertyListGUI(string label, ref List<Property> props, float spacing = 0) {
				for (int p = 0; p < props.Count; p++) {
					if (p > 0) GUILayout.Space(spacing);
					GUILayout.BeginHorizontal();
					GUILayout.Label(label, GUILayout.Width(EFFECT_LABEL_WIDTH));
					props[p].name = GUILayout.TextField(props[p].name, GUILayout.Width(100f));
					props[p].value = int.Parse(GUILayout.TextField(props[p].value.ToString(), GUILayout.Width(50f)));
					if (GUILayout.Button("-", GUILayout.Width(30f))) props.RemoveAt(p);
					GUILayout.EndHorizontal();
				}
			}

			// Show trait effects
			GUILayout.BeginVertical();
			PropertyListGUI("Trait effect", ref decision.choices[c].attributeEffects, 5f);
			if (GUILayout.Button("Add trait effect")) decision.choices[c].attributeEffects.Add(new Property() {
				type = PropertyType.ATTRIBUTE, value = 0
			});
			GUILayout.EndVertical();

			GUILayout.Space(15f);

			// Show consequences
			for (int e = 0; e < decision.choices[c].consequences.Count; e++) {
				GUILayout.BeginVertical();
				if (GUILayout.Button("Remove consequence", GUILayout.Width(150f))) decision.choices[c].consequences.RemoveAt(e);

				PropertyListGUI("Stat effect", ref decision.choices[c].consequences[e].statEffects, 10f);
				if (GUILayout.Button("Add stat effect")) decision.choices[c].consequences[e].statEffects.Add(new Property() {
					type = PropertyType.STAT, value = 0
				});

				decision.choices[c].consequences[e].consequenceText = GUILayout.TextArea(decision.choices[c].consequences[e].consequenceText, GUILayout.MinHeight(45f));
				GUILayout.EndVertical();
			}

			GUILayout.Space(10f);
			if (GUILayout.Button("Add consequence")) decision.choices[c].consequences.Add(new Consequence());

			GUILayout.EndVertical();
			GUILayout.Space(20f);
		}

		//Separator();
		if (GUILayout.Button("Add choice", GUILayout.Width(100f))) {
			decision.choices.Add(new Choice());
		}

		GUILayout.EndHorizontal();

		GUILayout.Space(PADDING);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
	}
}