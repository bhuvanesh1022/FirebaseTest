using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;

public class CMSDecision : MonoBehaviour {
	[SerializeField] CMSInputBase decisionTextField, speakerField;
	[SerializeField] CMSImageReference image;
	[SerializeField] CMSRequirementsHolder requirements;
	[SerializeField] CMSLayoutSetup choicesSetup;
	[SerializeField] CMSChoice choicePrefab;

	public Decision Decision { get; private set; }

	CMSLayout<CMSChoice> choicesLayout;

	private void Awake() {
		if (choicesSetup) {
			choicesLayout = new CMSLayout<CMSChoice>(choicesSetup, choicePrefab);
			choicesLayout.OnCellAddedOrRemoved += ChoiceAddedOrRemoved;
		}
	}

	public void Initialize(Decision dec = null) {
		Decision = dec ?? new Decision();
		decisionTextField.Initialize(Decision.decisionText);
		speakerField.Initialize(Decision.speakerName);
		image.Initialize(dec.decisionImage);
		requirements.Initialize(dec.requirements);
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

	public void Refresh() {
		Decision.decisionText = decisionTextField.Text;
		Decision.speakerName = speakerField.Text;

		image.Refresh();
		Decision.decisionImage = image.ImageReference;

		requirements.Refresh();
		Decision.requirements = requirements.Requirements;

		Decision.choices.Clear();
		choicesLayout.Elements.ForEach(choice => {
			choice.Refresh();
			Decision.choices.Add(choice.Choice);
		});
	}
}