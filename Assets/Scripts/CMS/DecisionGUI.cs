using UnityEngine;
using DecisionFramework;

public class DecisionGUI {
	const float PADDING = 20f, LABEL_WIDTH = 80f, EFFECT_LABEL_WIDTH = 50f;

	static Vector2 scrollPos;

	public static void ShowDecision(ref Decision decision) {
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
		for (int c = 0; c < decision.choices.Count; c++) {
			Separator(c > 0);

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

			for (int e = 0; e < decision.choices[c].effects.Count; e++) {

				// Effect data
				void RemoveButton(ref Decision dec, float buttonWidth = 80f) {
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
						dec.choices[c].effects.RemoveAt(e);
				}

				switch (decision.choices[c].effects[e].property.type) {
				case Property.Type.TRAIT:
					GUILayout.BeginHorizontal();
					GUILayout.Label("Trait", GUILayout.Width(EFFECT_LABEL_WIDTH));
					decision.choices[c].effects[e].property.name = GUILayout.TextField(decision.choices[c].effects[e].property.name, GUILayout.Width(100f));
					decision.choices[c].effects[e].amount = int.Parse(GUILayout.TextField(decision.choices[c].effects[e].amount.ToString(), GUILayout.Width(50f)));
					RemoveButton(ref decision);
					GUILayout.EndHorizontal();
					break;

				case Property.Type.STAT:
					GUILayout.Space(15f);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Stat", GUILayout.Width(EFFECT_LABEL_WIDTH));
					decision.choices[c].effects[e].property.name = GUILayout.TextField(decision.choices[c].effects[e].property.name, GUILayout.Width(100f));
					decision.choices[c].effects[e].amount = int.Parse(GUILayout.TextField(decision.choices[c].effects[e].amount.ToString(), GUILayout.Width(50f)));
					RemoveButton(ref decision);
					GUILayout.EndHorizontal();
					decision.choices[c].effects[e].consequenceText = GUILayout.TextArea(decision.choices[c].effects[e].consequenceText, GUILayout.MinHeight(45f));
					break;

				default:
					break;
				}
			}

			GUILayout.Space(15f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add trait effect", GUILayout.Width(150f))) {
				decision.choices[c].effects.Insert(decision.choices[c].effects.FindLastIndex(effect => effect.property.type == Property.Type.TRAIT) + 1,
					new Effect() {
						property = new Property() {
							type = Property.Type.TRAIT
						}
					});
			}
			if (GUILayout.Button("Add stat effect", GUILayout.Width(150f))) {
				decision.choices[c].effects.Add(new Effect() {
					property = new Property() {
						type = Property.Type.STAT
					}
				});
			}
			GUILayout.EndHorizontal();
		}

		Separator();
		if (GUILayout.Button("Add choice", GUILayout.Width(100f))) {
			decision.choices.Add(new Choice());
		}

		GUILayout.Space(PADDING);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
	}
}