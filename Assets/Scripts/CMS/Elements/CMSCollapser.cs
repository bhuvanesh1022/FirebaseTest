using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CMSToggleButton))]
public class CMSCollapser : MonoBehaviour {
	[SerializeField] Transform toCollapse;
	[SerializeField] bool collapseX, collapseY, startCollapsed;

	public delegate void CollapseToggleEvent(bool isExpanded);
	public CollapseToggleEvent OnCollapseToggle;

	public bool IsExpanded { get => toggle.IsOn; }

	CMSToggleButton toggle;
	Vector3 fullScale, collapsedScale;

	private void Awake() {
		toggle = GetComponent<CMSToggleButton>();
	}

	private void Start() {
		if (!toCollapse || !toggle) return;

		collapsedScale = fullScale = toCollapse.localScale;
		if (collapseX) collapsedScale.x = 0;
		if (collapseY) collapsedScale.y = 0;

		if (toggle) toggle.Toggle.onValueChanged.AddListener(isOn => {
			toCollapse.localScale = isOn ? fullScale : collapsedScale;
			CMSLayoutSetup.UpdateLayouts();
			OnCollapseToggle?.Invoke(isOn);
		});

		toggle.Toggle.isOn = !startCollapsed;
	}
}
