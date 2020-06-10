using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DecisionFramework {
	[CreateAssetMenu(fileName = "Decisions", menuName = "Reignslike/Decisions")]
	public class DecisionsHolder : ScriptableObject {
		public List<Decision> decisions;

		public Decision this[int i] {
			get { return decisions[i]; }
		}
	}
}