using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;

public class CMSConsequence : CMSLayoutElement {
	[SerializeField] CMSLayoutSetup statEffectsSetup;
	[SerializeField] CMSNameNumber statEffectPrefab, statEffect;
	[SerializeField] CMSInputBase effectTextField;
	[SerializeField] CMSImageReference image;

	public Consequence Consequence { get; private set; }

	CMSLayout<CMSNameNumber> statEffectsLayout = null;

	protected override void Awake() {
		base.Awake();
		if (statEffectsSetup && statEffectPrefab) statEffectsLayout = new CMSLayout<CMSNameNumber>(statEffectsSetup, statEffectPrefab);
	}

	public override void Initialize() {
		Initialize(null);
	}

	public void Initialize(Consequence cons) {
		Consequence = cons ?? new Consequence() { statEffects = new List<Property>() { new Property() { type = PropertyType.STAT } } };
		effectTextField.Initialize(Consequence.consequenceText);
		image.Initialize(Consequence.consequenceImage);
		for (int c = 0; c < Consequence.statEffects.Count; c++) {
			Property effect = Consequence.statEffects[c];
			// We're just using the first stat effect for now, hence this hack
			if (c == 0) statEffect.Initialize(effect.name, effect.value);
			else if (statEffectsLayout == null) break;
			else statEffectsLayout.Add().Initialize(effect.name, effect.value);
		}
	}

	public void Refresh() {
		Consequence.consequenceText = effectTextField.Text;

		image.Refresh();
		Consequence.consequenceImage = image.ImageReference;

		Consequence.statEffects.Clear();
		Consequence.statEffects.Add(new Property() {
			name = statEffect.Name, type = PropertyType.STAT, value = statEffect.Number
		});
		if (statEffectsLayout != null) statEffectsLayout.Elements.ForEach(effect => Consequence.statEffects.Add(new Property() {
			name = effect.Name, type = PropertyType.STAT, value = effect.Number
		}));
	}
}