using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;
using TMPro;

public class CMSDecision : MonoBehaviour {
	[SerializeField] CMSInputField decisionTextField, speakerField;
	[SerializeField] CMSLayoutSetup choicesLayoutSetup;
	[SerializeField] CMSChoice choicePrefab;

	public Decision Decision { get; private set; }

	CMSLayout<CMSChoice> choicesLayout;

	private void Awake() {
		if (choicesLayoutSetup) {
			choicesLayout = new CMSLayout<CMSChoice>(choicesLayoutSetup, choicePrefab);
			choicesLayout.OnCellAddedOrRemoved += ChoiceAddedOrRemoved;
		}

		//if (decisionTextField) decisionTextField.OnConfirmEntry += field => Decision.decisionText = field.Text;
		//if (speakerField) speakerField.OnConfirmEntry += field => Decision.speakerName = field.Text;
	}

	public void Initialize(Decision dec = null) {
		Decision = dec ?? new Decision();
		decisionTextField.Initialize(Decision.decisionText);
		speakerField.Initialize(Decision.speakerName);
		choicesLayout.Clear();
		for (int c = 0; c < Decision.choices.Count; c++) choicesLayout.Add().Initialize(Decision.choices[c]);
		UpdateChoiceLabels();
		choicesLayout.Initialize();
	}

	void ChoiceAddedOrRemoved(CMSChoice changedChoice, bool wasAdded) {
		if (wasAdded) changedChoice.Initialize();
		UpdateChoiceLabels();
	}

	void UpdateChoiceLabels() {
		for (int c = 0; c < choicesLayout.Elements.Count; c++) choicesLayout.Elements[c].UpdateChoiceLabel(c);
	}

	public void RefreshDecision() {
		Decision.decisionText = decisionTextField.Text;
		Decision.speakerName = speakerField.Text;

		Decision.choices.Clear();
		choicesLayout.Elements.ForEach(choice => {
			choice.RefreshChoice();
			Decision.choices.Add(choice.Choice);
		});
	}
}