using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;
using TMPro;
using UnityEngine.UI;

public class CMSChoice : CMSGridCellElement {
	[SerializeField] TMP_Text choiceLabel;
	[SerializeField] CMSInputField choiceTextField;
	[SerializeField] CMSLayoutSetup attributesGridSetup, effectsGridSetup;
	[SerializeField] CMSNameNumber attributePrefab;
	[SerializeField] CMSConsequence effectPrefab;

	public Choice Choice { get; private set; }

	CMSLayout<CMSNameNumber> attributesGrid;
	CMSLayout<CMSConsequence> effectsGrid;

	protected override void Awake() {
		base.Awake();
		if (attributesGridSetup) {
			attributesGrid = new CMSLayout<CMSNameNumber>(attributesGridSetup, attributePrefab);
			attributesGrid.OnCellAddedOrRemoved += (att, wasAdded) => {
				if (wasAdded) att.Initialize();
			};
		}
		if (effectsGridSetup) {
			effectsGrid = new CMSLayout<CMSConsequence>(effectsGridSetup, effectPrefab);
			effectsGrid.OnCellAddedOrRemoved += (eff, wasAdded) => {
				if (wasAdded) eff.Initialize();
			};
		}

		//if (choiceTextField) choiceTextField.OnConfirmEntry += field => Choice.choiceText = field.Text;
	}

	public void Initialize(Choice choice = null) {
		Choice = choice ?? new Choice();
		attributesGrid.Clear();
		choiceTextField.Initialize(Choice.choiceText);
		Choice.attributeEffects.ForEach(att => attributesGrid.Add().Initialize(att.name, att.value));
		Choice.consequences.ForEach(con => effectsGrid.Add().Initialize(con));
	}

	public void UpdateChoiceLabel(int choiceIndex) {
		if (choiceLabel) choiceLabel.text = "Choice " + ((char)('A' + choiceIndex)).ToString();
	}

	public void RefreshChoice() {
		Choice.choiceText = choiceTextField.Text;

		Choice.attributeEffects.Clear();
		attributesGrid.Elements.ForEach(att => Choice.attributeEffects.Add(new Property() {
			name = att.Name, value = att.Number, type = PropertyType.ATTRIBUTE
		}));

		Choice.consequences.Clear();
		effectsGrid.Elements.ForEach(con => {
			con.RefreshConsequence();
			Choice.consequences.Add(con.Consequence);
		});
	}
}