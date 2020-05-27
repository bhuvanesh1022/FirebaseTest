using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DecisionFramework {
	[System.Serializable]
	public class Decision {
		public string decisionText, speakerName;
		public List<Choice> choices;
		public List<Requirement> requirements;
	}

	[System.Serializable]
	public class Choice {
		public string choiceText;
		public List<Effect> effects;
	}
	
	[System.Serializable]
	public class Property {
		public enum Type { NONE, STAT, TRAIT, FLAG }
		public string name;
		public Type type;
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
}