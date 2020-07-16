using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionFramework {
	[CreateAssetMenu(fileName = "Decisions", menuName = "Reignslike/Decisions")]
	public class DecisionsHolder : ScriptableObject {
		public List<Decision> decisions;
		public ListsHolder lists;

		public Decision this[int i] {
			get { return decisions[i]; }
		}
	}

	[System.Serializable]
	public class ListsHolder {
		[SerializeField] List<Attribute> attributes;
		[SerializeField] List<string> stats, speakers;
		[SerializeField] int decisionCharLimit = 100, choiceCharLimit = 70, consequenceCharLimit = 150;

		List<string> attributeNames = new List<string>();

		public List<string> GetList(CMSSearchType type) {
			switch (type) {
			case CMSSearchType.ATTRIBUTE:
				if (attributeNames.Count==0) attributes.ForEach(att => attributeNames.Add(att.name));
				return attributeNames;
			case CMSSearchType.SPEAKER:
				return speakers;
			case CMSSearchType.STAT:
				return stats;
			default:
				return null;
			}
		}

		public int GetCharacterLimit(CMSTextType textType) {
			switch (textType) {
			case CMSTextType.DECISION: return decisionCharLimit;
			case CMSTextType.CHOICE: return choiceCharLimit;
			case CMSTextType.CONSEQUENCE: return consequenceCharLimit;
			default: return 0;
			}
		}

		public void AddEntry(CMSSearchType type, string entry) {
			switch (type) {
			case CMSSearchType.ATTRIBUTE:
				if (!attributeNames.Contains(entry)) {
					attributes.Add(new Attribute() { name = entry });
					attributeNames.Add(entry);
				}
				break;
			case CMSSearchType.SPEAKER:
				if (!speakers.Contains(entry)) speakers.Add(entry);
				break;
			default:
				break;
			}
		}
	}
}