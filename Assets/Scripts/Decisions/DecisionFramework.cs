using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DecisionFramework {
	[System.Serializable]
	public class Decision {
		public string decisionText, speakerName;
		public List<Choice> choices = new List<Choice>();
		public List<Requirement> requirements = new List<Requirement>();
	}

	[System.Serializable]
	public class Choice {
		public string choiceText;
		public List<Effect> effects = new List<Effect>();
	}
	
	[System.Serializable]
	public class Property {
		public enum Type { NONE, STAT, TRAIT, FLAG }
		public Type type;
		public string name;
	}

	[System.Serializable]
	public class Effect {
		public Property property;
		public int amount;
		public string consequenceText;
	}

	[System.Serializable]
	public class Requirement {
		public Property property;
		public bool checkMin = false, checkMax = false;
		public int minValue, maxValue;
		//public bool checkExists = false, doesExist;
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