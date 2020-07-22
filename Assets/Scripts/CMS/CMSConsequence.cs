using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DecisionFramework;

public class CMSConsequence : CMSLayoutElement {
	[SerializeField] CMSLayoutSetup statEffectsGridSetup;
	[SerializeField] CMSNameNumber statEffectPrefab, statEffect;
	[SerializeField] CMSInputField effectTextField;

	public Consequence Consequence { get; private set; }

	CMSLayout<CMSNameNumber> statEffectsLayout = null;

	protected override void Awake() {
		base.Awake();
		if (statEffectsGridSetup) statEffectsLayout = new CMSLayout<CMSNameNumber>(statEffectsGridSetup, statEffectPrefab);
	}

	public void Initialize(Consequence cons = null) {
		Consequence = cons ?? new Consequence() { statEffects = new List<Property>() { new Property() { type = PropertyType.STAT } } };
		effectTextField.Initialize(Consequence.consequenceText);
		for (int c = 0; c < Consequence.statEffects.Count; c++) {
			Property effect = Consequence.statEffects[c];
			if (c == 0) statEffect.Initialize(effect.name, effect.value);
			else if (statEffectsLayout == null) break;
			else statEffectsLayout.Add().Initialize(effect.name, effect.value);
		}
	}

	public void Refresh() {
		Consequence.consequenceText = effectTextField.Text;
		Consequence.statEffects.Clear();
		Consequence.statEffects.Add(new Property() {
			name = statEffect.Name, type = PropertyType.STAT, value = statEffect.Number
		});
		if (statEffectsLayout != null) statEffectsLayout.Elements.ForEach(effect => Consequence.statEffects.Add(new Property() {
			name = effect.Name, type = PropertyType.STAT, value = effect.Number
		}));
	}
}