using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;

public class CMSUnlock : CMSLayoutElement {
	[SerializeField] CMSInputBase flagField;
	[SerializeField] CMSToggleButton doUnlockToggle;

	public UnlockEffect Unlock { get; private set; }

	public override void Initialize() {
		Initialize(null);
	}

	public void Initialize(UnlockEffect unlock = null) {
		Unlock = unlock ?? new UnlockEffect() {
			property = new Property() { type = PropertyType.FLAG },
			doUnlock = true
		};
		flagField.Initialize(Unlock.property.name);
		doUnlockToggle.isOn = Unlock.doUnlock;
	}

	public void Refresh() {
		Unlock.property.name = flagField.Text;
		Unlock.doUnlock = doUnlockToggle.isOn;
	}
}