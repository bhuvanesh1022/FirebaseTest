using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace DecisionFramework {
	[System.Serializable]
	public class Decision {
		public string decisionText, speakerName;
		public ImageReference decisionImage = new ImageReference();
		public List<Choice> choices = new List<Choice>(); // { new Choice(), new Choice() };
		public List<Requirement> requirements = new List<Requirement>();
	}

	[System.Serializable]
	public class Choice {
		public string choiceText;
		public List<Property> attributeEffects = new List<Property>();
		public List<Consequence> consequences = new List<Consequence>();
		public List<UnlockEffect> unlocks = new List<UnlockEffect>();
	}

	[System.Serializable]
	public class ChoiceLink : IEquatable<ChoiceLink> {
		public Choice Choice1 { get; private set; }
		public Choice Choice2 { get; private set; }

		private ChoiceLink() { }

		public static ChoiceLink Link(Choice link1, Choice link2) {
			return new ChoiceLink() { Choice1 = link1, Choice2 = link2 };
		}

		public bool Equals(ChoiceLink other) {
			return other != null &&
				   EqualityComparer<Choice>.Default.Equals(Choice1, other.Choice1) &&
				   EqualityComparer<Choice>.Default.Equals(Choice2, other.Choice2);
		}

		public override int GetHashCode() {
			var hashCode = 582511193;
			hashCode = hashCode * -1521134295 + EqualityComparer<Choice>.Default.GetHashCode(Choice1);
			hashCode = hashCode * -1521134295 + EqualityComparer<Choice>.Default.GetHashCode(Choice2);
			return hashCode;
		}
	}

	public enum PropertyType { NONE, STAT, ATTRIBUTE, TRAIT, FLAG }

	[System.Serializable]
	public class Property {
		public PropertyType type;
		public string name;
		public int value = 1;
	}

	[System.Serializable]
	public class Consequence {
		public List<Property> statEffects = new List<Property>();
		public string consequenceText = string.Empty;
		public ImageReference consequenceImage = new ImageReference();
	}

	[System.Serializable]
	public class Requirement {
		public Property check;
		public enum CheckType { PRESENT, ABSENT, EQUALS, LESS_THAN, GREATER_THAN }
		public CheckType checkType;
	}

	[System.Serializable]
	public class UnlockEffect {
		public Property property;
		public bool doUnlock;
	}

	[System.Serializable]
	public class Attribute {
		[System.Serializable]
		public class WeightedEffect {
			public string name;
			public float weight;
		}

		public string name;
		public List<WeightedEffect> affectedTraits = new List<WeightedEffect>(), affectedAttributes = new List<WeightedEffect>();

		public override bool Equals(object obj) {
			if (obj.GetType().Equals(typeof(Attribute))) return string.Equals(name, (obj as Attribute).name);
			else return base.Equals(obj);
		}

		public override int GetHashCode() {
			var hashCode = -939562750;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
			hashCode = hashCode * -1521134295 + EqualityComparer<List<WeightedEffect>>.Default.GetHashCode(affectedTraits);
			hashCode = hashCode * -1521134295 + EqualityComparer<List<WeightedEffect>>.Default.GetHashCode(affectedAttributes);
			return hashCode;
		}
	}

	[System.Serializable]
	public class ImageReference {
		public string description, filename;
	}

	[System.Serializable]
	public class UserGameData {
		public List<GameRecord> Records {
			get { return records; }
		}

		[SerializeField] List<GameRecord> records;

		public UserGameData(Dictionary<string, int> gameData) {
			records = new List<GameRecord>();
			foreach (var kvp in gameData) {
				records.Add(new GameRecord() {
					name = kvp.Key, value = kvp.Value
				});
			}
		}

		public string GetJson() {
			return JsonUtility.ToJson(this);
		}

		public string Display() {
			string retval = string.Empty;
			bool isFirst = true;
			records.ForEach(rec => {
				if (!isFirst) retval += "\n";
				else isFirst = false;
				retval += string.Format("{0}: {1}", rec.name, rec.value);
			});
			return retval;
		}

		public static UserGameData CreateFromJson(string json) {
			return JsonUtility.FromJson<UserGameData>(json);
		}

		[System.Serializable]
		public class GameRecord {
			public string name;
			public int value;
		}
	}
}