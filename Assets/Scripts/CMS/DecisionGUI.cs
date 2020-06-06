using UnityEngine;
using DecisionFramework;

public class DecisionGUI {
	public Decision ShownDecision { get; private set; }

	public DecisionGUI(Decision initializer = null) {
		ShownDecision = initializer;
		if (initializer == null) ShownDecision = new Decision();
	}

	const float PADDING = 20f, LABEL_WIDTH = 80f, EFFECT_LABEL_WIDTH = 50f;

	public void OnGUI() {
		GUILayout.BeginVertical();
		GUILayout.Space(PADDING);

		// Decision data
		GUILayout.BeginHorizontal();
		GUILayout.Label("Speaker", GUILayout.Width(LABEL_WIDTH));
		ShownDecision.speakerName = GUILayout.TextField(ShownDecision.speakerName);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Dialogue", GUILayout.Width(LABEL_WIDTH));
		ShownDecision.decisionText = GUILayout.TextArea(ShownDecision.decisionText, GUILayout.MinHeight(60f));
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
		for (int c = 0; c < ShownDecision.choices.Count; c++) {
			Separator(c > 0);

			GUILayout.BeginHorizontal();
			GUILayout.Label("Choice " + (c + 1).ToString(), GUILayout.Width(LABEL_WIDTH));
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Remove choice", GUILayout.Width(120f))) {
				ShownDecision.choices.RemoveAt(c);
				break;
			}
			GUILayout.EndHorizontal();
			ShownDecision.choices[c].choiceText = GUILayout.TextField(ShownDecision.choices[c].choiceText);

			GUILayout.Space(15f);

			for (int e = 0; e < ShownDecision.choices[c].effects.Count; e++) {

				// Effect data
				void RemoveButton(float buttonWidth = 80f) {
					GUILayout.FlexibleSpace();
					if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
						ShownDecision.choices[c].effects.RemoveAt(e);
				}

				switch (ShownDecision.choices[c].effects[e].property.type) {
				case Property.Type.TRAIT:
					GUILayout.BeginHorizontal();
					GUILayout.Label("Trait", GUILayout.Width(EFFECT_LABEL_WIDTH));
					ShownDecision.choices[c].effects[e].property.name = GUILayout.TextField(ShownDecision.choices[c].effects[e].property.name, GUILayout.Width(100f));
					ShownDecision.choices[c].effects[e].amount = int.Parse(GUILayout.TextField(ShownDecision.choices[c].effects[e].amount.ToString(), GUILayout.Width(50f)));
					RemoveButton();
					GUILayout.EndHorizontal();
					break;

				case Property.Type.STAT:
					GUILayout.Space(15f);
					GUILayout.BeginHorizontal();
					GUILayout.Label("Stat", GUILayout.Width(EFFECT_LABEL_WIDTH));
					ShownDecision.choices[c].effects[e].property.name = GUILayout.TextField(ShownDecision.choices[c].effects[e].property.name, GUILayout.Width(100f));
					ShownDecision.choices[c].effects[e].amount = int.Parse(GUILayout.TextField(ShownDecision.choices[c].effects[e].amount.ToString(), GUILayout.Width(50f)));
					RemoveButton();
					GUILayout.EndHorizontal();
					ShownDecision.choices[c].effects[e].consequenceText = GUILayout.TextArea(ShownDecision.choices[c].effects[e].consequenceText, GUILayout.MinHeight(45f));
					break;

				default:
					break;
				}
			}

			GUILayout.Space(15f);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add trait effect", GUILayout.Width(150f))) {
				ShownDecision.choices[c].effects.Insert(ShownDecision.choices[c].effects.FindLastIndex(effect => effect.property.type == Property.Type.TRAIT) + 1,
					new Effect() {
						property = new Property() {
							type = Property.Type.TRAIT
						}
					});
			}
			if (GUILayout.Button("Add stat effect", GUILayout.Width(150f))) {
				ShownDecision.choices[c].effects.Add(new Effect() {
					property = new Property() {
						type = Property.Type.STAT
					}
				});
			}
			GUILayout.EndHorizontal();
		}

		Separator();
		if (GUILayout.Button("Add choice", GUILayout.Width(100f))) {
			ShownDecision.choices.Add(new Choice());
		}

		GUILayout.Space(PADDING);
		GUILayout.EndVertical();
	}
}