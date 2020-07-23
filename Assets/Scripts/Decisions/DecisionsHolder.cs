using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionFramework {
	[CreateAssetMenu(fileName = "Decisions", menuName = "Reignslike/Decisions")]
	public class DecisionsHolder : ScriptableObject {
		public List<Decision> decisions;
		public ArtHolder arts;
		public ListsHolder lists;

		public Decision this[int i] {
			get { return decisions[i]; }
		}
	}

	[System.Serializable]
	public class ArtHolder {
		static ArtHolder instance = null;
		static Dictionary<string, Sprite> artNameMap = null;

		[System.Serializable]
		class ArtLookup {
			public string name;
			public Sprite sprite;
		}

		[SerializeField] List<ArtLookup> artLookup;

		public ArtHolder() {
			instance = this;
		}

		public static Sprite GetSprite(string artName) {
			if (artNameMap == null) {
				if (instance == null) return null;
				artNameMap = new Dictionary<string, Sprite>();
				instance.artLookup.ForEach(art => artNameMap[art.name] = art.sprite);
			}
			return artNameMap.ContainsKey(artName) ? artNameMap[artName] : null;
		}
	}

	
	public enum ListType { NONE, ATTRIBUTE, SPEAKER, STAT, FLAG }

	[System.Serializable]
	public class ListsHolder {
		[SerializeField] List<Attribute> attributes;
		[SerializeField] List<string> stats, speakers, flags;

		List<string> attributeNames = new List<string>();

		public List<string> GetList(ListType type) {
			switch (type) {
			case ListType.ATTRIBUTE:
				if (attributeNames.Count==0) attributes.ForEach(att => attributeNames.Add(att.name));
				return attributeNames;
			case ListType.SPEAKER:
				return speakers;
			case ListType.STAT:
				return stats;
			case ListType.FLAG:
				return flags;
			default:
				return null;
			}
		}

		public void AddEntry(ListType type, string entry) {
			switch (type) {
			case ListType.ATTRIBUTE:
				if (!attributeNames.Contains(entry)) {
					attributes.Add(new Attribute() { name = entry });
					attributeNames.Add(entry);
				}
				break;
			case ListType.STAT:
			case ListType.SPEAKER:
			case ListType.FLAG:
				List<string> list = GetList(type);
				if (!list.Contains(entry)) list.Add(entry);
				break;
			default:
				break;
			}
		}
	}
}