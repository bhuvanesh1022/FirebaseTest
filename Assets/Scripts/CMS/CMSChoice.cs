using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;
using TMPro;
using UnityEngine.UI;

public class CMSChoice : CMSLayoutElement {
	[SerializeField] TMP_Text choiceLabel;
	[SerializeField] CMSInputBase choiceTextField;
	[SerializeField] CMSLayoutSetup attributesSetup, effectsSetup, unlocksSetup;
	[SerializeField] CMSNameNumber attributePrefab;
	[SerializeField] CMSConsequence effectPrefab;
	[SerializeField] CMSUnlock unlockPrefab;

	public Choice Choice { get; private set; }

	CMSLayout<CMSNameNumber> attributesLayout;
	CMSLayout<CMSConsequence> effectsLayout;
	CMSLayout<CMSUnlock> unlocksLayout;

	protected override void Awake() {
		base.Awake();

		void InitializeSetup<T>(ref CMSLayout<T> layout, T prefab, CMSLayoutSetup setup) where T : CMSLayoutElement {
			if (!setup || !prefab) return;
			layout = new CMSLayout<T>(setup, prefab);
			layout.OnCellAddedOrRemoved += (cell, wasAdded) => {
				if (wasAdded) cell.Initialize();
			};
		}

		InitializeSetup(ref attributesLayout, attributePrefab, attributesSetup);
		InitializeSetup(ref effectsLayout, effectPrefab, effectsSetup);
		InitializeSetup(ref unlocksLayout, unlockPrefab, unlocksSetup);
	}

	public override void Initialize() {
		Initialize(null);
	}

	public void Initialize(Choice choice = null) {
		Choice = choice ?? new Choice();
		attributesLayout.Clear();
		choiceTextField.Initialize(Choice.choiceText);
		Choice.attributeEffects.ForEach(att => attributesLayout.Add().Initialize(att.name, att.value));
		Choice.consequences.ForEach(con => effectsLayout.Add().Initialize(con));
		Choice.unlocks.ForEach(unlock => unlocksLayout.Add().Initialize(unlock));
	}

	public void UpdateChoiceLabel(int choiceIndex) {
		if (choiceLabel) choiceLabel.text = "Choice " + ((char)('A' + choiceIndex)).ToString();
	}

	public void Refresh() {
		Choice.choiceText = choiceTextField.Text;

		Choice.attributeEffects.Clear();
		attributesLayout.Elements.ForEach(att => Choice.attributeEffects.Add(new Property() {
			name = att.Name, value = att.Number, type = PropertyType.ATTRIBUTE
		}));

		Choice.consequences.Clear();
		effectsLayout.Elements.ForEach(con => {
			con.Refresh();
			Choice.consequences.Add(con.Consequence);
		});

		Choice.unlocks.Clear();
		unlocksLayout.Elements.ForEach(ul => {
			ul.Refresh();
			if (!string.IsNullOrEmpty(ul.Unlock.property.name)) Choice.unlocks.Add(ul.Unlock);
		});
	}
}