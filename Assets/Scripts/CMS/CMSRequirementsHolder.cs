using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionFramework;
using TMPro;

public class CMSRequirementsHolder : MonoBehaviour {
	[SerializeField] CMSLayoutSetup reqsLayoutSetup;
	[SerializeField] CMSRequirement requirementPrefab;
	[SerializeField] CMSCollapser collapser;
	[SerializeField] TMP_Text summaryText;
	[SerializeField] string noReqsText = "None";

	CMSLayout<CMSRequirement> reqsLayout;

	public List<Requirement> Requirements {
		get { return reqsLayout.Elements.ConvertAll(req => req.Requirement); }
	}

	private void Awake() {
		if (reqsLayoutSetup) {
			reqsLayout = new CMSLayout<CMSRequirement>(reqsLayoutSetup, requirementPrefab);
			reqsLayout.OnCellAddedOrRemoved += (req, wasAdded) => {
				if (wasAdded) req.Initialize(new Requirement() {
					check = new Property() {
						type = PropertyType.STAT
					}
				});
			};
		}

		if (summaryText) {
			if (collapser) collapser.OnCollapseToggle += isExpanded => {
				summaryText.gameObject.SetActive(!isExpanded);
				if (summaryText.gameObject.activeSelf) RefreshSummaryText();
			};
			summaryText.text = noReqsText;
		}
	}

	void RefreshSummaryText() {
		if (!summaryText) return;
		if (reqsLayout.Elements.Count == 0) summaryText.text = noReqsText;
		else {
			string setText = string.Empty;
			reqsLayout.Elements.ForEach(req => setText += (string.IsNullOrEmpty(setText) ? "" : ",  ") + req.SummaryText);
			summaryText.text = setText;
		}
	}

	public void Initialize(List<Requirement> requirements) {
		reqsLayout.Clear();
		if (requirements != null) requirements.ForEach(req => reqsLayout.Add().Initialize(req));
		if (summaryText && (!collapser || !collapser.IsExpanded)) RefreshSummaryText();
	}

	public void Refresh() {
		reqsLayout.Elements.ForEach(req => req.Refresh());
	}
}