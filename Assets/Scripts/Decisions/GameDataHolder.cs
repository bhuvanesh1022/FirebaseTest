using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionFramework {
	[System.Serializable]
	public class GameDataHolder {
		public List<Decision> decisions;
		public ImagesHolder arts;
		public ListsHolder lists;

		public Decision this[int i] {
			get { return decisions[i]; }
		}
	}

	[System.Serializable]
	public class ImagesHolder {
		static ImagesHolder instance = null;
		static Dictionary<string, Sprite> imageNameMap = null;

		[System.Serializable]
		class ImageLookup {
			public string name;
			public Sprite sprite;
		}

		[SerializeField] List<ImageLookup> artLookup;

		public ImagesHolder() {
			instance = this;
		}

		public static Sprite GetSprite(string artName) {
			if (imageNameMap == null) {
				if (instance == null) return null;
				imageNameMap = new Dictionary<string, Sprite>();
				instance.artLookup.ForEach(art => imageNameMap[art.name] = art.sprite);
			}
			return imageNameMap.ContainsKey(artName) ? imageNameMap[artName] : null;
		}
	}

	public enum ListType { NONE, ATTRIBUTE, SPEAKER, STAT, FLAG }

	[System.Serializable]
	public class ListsHolder {
		[SerializeField] List<Attribute> attributes = new List<Attribute>();
		[SerializeField] List<string> stats = new List<string>(), speakers = new List<string>(), flags = new List<string>();

		public bool HasChanged { get; set; } = false;

		List<string> attributeNames = new List<string>();

		public List<string> GetList(ListType type) {
			switch (type) {
			case ListType.ATTRIBUTE:
				if (attributeNames.Count == 0) attributes.ForEach(att => attributeNames.Add(att.name));
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
			if (string.IsNullOrEmpty(entry.Trim())) return;
			switch (type) {
			case ListType.ATTRIBUTE:
				if (!attributeNames.Contains(entry)) {
					attributes.Add(new Attribute() { name = entry });
					attributeNames.Add(entry);
					HasChanged = true;
				}
				break;
			case ListType.STAT:
			case ListType.SPEAKER:
			case ListType.FLAG:
				List<string> list = GetList(type);
				if (!list.Contains(entry)) {
					list.Add(entry);
					HasChanged = true;
				}
				break;
			default:
				break;
			}
		}
	}
}