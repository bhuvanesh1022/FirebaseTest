using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DecisionFramework;

public class DecisionDisplay : MonoBehaviour {
	[SerializeField] TMP_Text decisionText, effectsText;
	[SerializeField] List<TextButton> choiceButtons;
	[SerializeField] TextButton nextButton;
	[SerializeField] List<GameObject> showDuringDecision, showDuringEffects;

	public delegate void DecisionTakenEvent(Choice choice);
	public DecisionTakenEvent OnDecisionTaken;

	public delegate void DecisionEndEvent();
	public DecisionEndEvent OnDecisionEnd;

	private void Awake() {
		choiceButtons.ForEach(cb => cb.OnButtonPressed += OnChoiceMade);
		if (nextButton) nextButton.OnButtonPressed += _ => OnDecisionEnd?.Invoke();
	}

	private void Start() {
		SetEffectsVisibility(false);
	}

	Decision curDecision;
	Dictionary<TextButton, Choice> choiceMap = new Dictionary<TextButton, Choice>();

	public void ShowDecision(Decision decision) {
		curDecision = decision;

		string newText = string.Empty;
		if (!string.IsNullOrEmpty(decision.speakerName)) newText = string.Format("<line-height=150%><i>{0}</i>\n<line-height=100%>", decision.speakerName);
		newText += "<margin=1em>" + decision.decisionText;
		decisionText.text = newText;

		for (int c = 0; c < choiceButtons.Count; c++) {
			choiceButtons[c].gameObject.SetActive(c < decision.choices.Count);
			if (c >= decision.choices.Count) continue;
			choiceButtons[c].ButtonText = decision.choices[c].choiceText;
			choiceMap[choiceButtons[c]] = decision.choices[c];
		}

		SetEffectsVisibility(false);
		Utilities.AppearifyText(decisionText);
	}

	void OnChoiceMade(TextButton button) {
		Choice curChoice = choiceMap[button];
		OnDecisionTaken?.Invoke(curChoice);
		string newText = string.Empty;
		foreach (var con in curChoice.consequences) {
			foreach (var stat in con.statEffects) {
				if (stat.type != PropertyType.STAT || string.IsNullOrEmpty(stat.name)) continue;
				newText += string.Format("<b>{0} {1}</b>\n", stat.name, stat.value.ToString("+0;-#"));
			}
			if (!string.IsNullOrEmpty(con.consequenceText)) newText += con.consequenceText + "\n\n";
		}
		effectsText.text = newText;
		SetEffectsVisibility(true);
		Utilities.AppearifyText(effectsText);
	}

	void SetEffectsVisibility(bool showEffects) {
		showDuringDecision.ForEach(go => go.SetActive(!showEffects));
		showDuringEffects.ForEach(go => go.SetActive(showEffects));
	}
}