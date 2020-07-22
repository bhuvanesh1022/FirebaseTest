using DecisionFramework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CMSRequirement : CMSLayoutElement {
	[System.Serializable]
	class PropSelect {
		public PropertyType type;
		public CMSInputDropdown nameSelectPrefab;
	}

	[System.Serializable]
	class CheckTypeDescription {
		public Requirement.CheckType type;
		public string description, summary;
	}

	[SerializeField] Transform propSelectParent;
	[SerializeField] TMP_Dropdown propTypeMenu, checkTypeMenu;
	[SerializeField] TMP_InputField valueField;
	[SerializeField] List<PropSelect> propOptions;
	[SerializeField] List<CheckTypeDescription> checkTypeDescriptions;

	public Requirement Requirement { get; private set; }

	Dictionary<PropertyType, CMSInputDropdown> propOptionsMap = new Dictionary<PropertyType, CMSInputDropdown>();
	Dictionary<Requirement.CheckType, CheckTypeDescription> checkTypeDescriptionMap = new Dictionary<Requirement.CheckType, CheckTypeDescription>();
	List<Requirement.CheckType> checkTypeLookup = new List<Requirement.CheckType>();
	CMSInputDropdown curPropNameSelect;

	protected override void Awake() {
		base.Awake();
		checkTypeDescriptions.ForEach(td => {
			if (string.IsNullOrEmpty(td.summary)) td.summary = td.description;
			checkTypeDescriptionMap[td.type] = td;
		});

		if (valueField) valueField.contentType = TMP_InputField.ContentType.IntegerNumber;
		if (propTypeMenu) {
			propTypeMenu.ClearOptions();
			propTypeMenu.AddOptions(propOptions.ConvertAll(co => co.type.ToString()));
			propTypeMenu.onValueChanged.AddListener(val => {
				Requirement.check.type = propOptions[val].type;
				Requirement.check.name = string.Empty;
				UpdateCheckType();
			});
		}
		propOptions.ForEach(po => propOptionsMap[po.type] = po.nameSelectPrefab);
	}

	public void Initialize(Requirement req) {
		Requirement = req;
		UpdateCheckType();
	}

	void UpdateCheckType() {
		PropertyType newType = Requirement.check.type;

		if (curPropNameSelect) Destroy(curPropNameSelect.gameObject);
		if (propOptionsMap.ContainsKey(newType)) {
			curPropNameSelect = Instantiate(propOptionsMap[newType], propSelectParent);
			curPropNameSelect.Initialize(Requirement.check.name);
		}

		switch (newType) {
		case PropertyType.FLAG:
			UpdateMenus(Requirement.CheckType.PRESENT, Requirement.CheckType.ABSENT);
			break;
		case PropertyType.ATTRIBUTE:
		case PropertyType.STAT:
			UpdateMenus(Requirement.CheckType.EQUALS, Requirement.CheckType.GREATER_THAN, Requirement.CheckType.LESS_THAN);
			break;
		default:
			UpdateMenus();
			break;
		}

		void UpdateMenus(params Requirement.CheckType[] checkTypes) {
			if (!checkTypeMenu) return;
			checkTypeLookup.Clear();
			if (checkTypes.Length > 0) {
				checkTypeLookup.AddRange(checkTypes);
				checkTypeMenu.options.Clear();
				checkTypeMenu.AddOptions(checkTypeLookup.ConvertAll(ct => checkTypeDescriptionMap[ct].description));
			}
			checkTypeMenu.SetValueWithoutNotify(checkTypeLookup.IndexOf(Requirement.checkType));
		}

		if (valueField) {
			bool showValueField = newType == PropertyType.ATTRIBUTE || newType == PropertyType.STAT;
			valueField.gameObject.SetActive(showValueField);
			if (showValueField) valueField.SetTextWithoutNotify(Requirement.check.value.ToString());
		}
	}

	public string SummaryText {
		get {
			string retval = curPropNameSelect.Text + " " + checkTypeDescriptionMap[checkTypeLookup[checkTypeMenu.value]].summary;
			if (valueField.gameObject.activeSelf) retval += " " + valueField.text;
			return retval;
		}
	}

	public void Refresh() {
		Requirement.check = new Property() {
			name = curPropNameSelect.Text,
			type = propOptions[propTypeMenu.value].type,
			value = int.Parse(valueField.text)
		};
		Requirement.checkType = checkTypeLookup[checkTypeMenu.value];
	}
}