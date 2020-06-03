using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Utilities {
	class UtilitiesInstance : MonoBehaviour {
		public static UtilitiesInstance Instance {
			get {
				if (!_instance) {
					_instance = new GameObject("Utilities", typeof(UtilitiesInstance)).GetComponent<UtilitiesInstance>();
					DontDestroyOnLoad(_instance.gameObject);
				}
				return _instance;
			}
		}
		static UtilitiesInstance _instance;
	}

	public delegate void AppearifyEndEvent();

	public static AppearifyEndEvent AppearifyText(TMP_Text text, float lettersPerSecond = 250, float fadeTime = 0.2f) {
		AppearifyEndEvent onAppearifyEnd = delegate { };
		UtilitiesInstance.Instance.StartCoroutine(AppearifyCR());
		return onAppearifyEnd;

		IEnumerator AppearifyCR() {
			yield return null;
			text.ForceMeshUpdate();
			Color32[] newVertexColors;
			newVertexColors = text.textInfo.meshInfo[text.textInfo.characterInfo[0].materialReferenceIndex].colors32;
			for (int v = 0; v < newVertexColors.Length; v++) newVertexColors[v].a = 0;
			text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
			yield return null;

			float curCharIndex = 0;
			do {
				float lastCharIndex = curCharIndex;
				curCharIndex = curCharIndex + (Time.deltaTime * lettersPerSecond);
				int fromIndex = Mathf.RoundToInt(lastCharIndex), toIndex = Mathf.RoundToInt(curCharIndex);
				for (int c = fromIndex; c < toIndex; c++) {
					int vertexIndex = text.textInfo.characterInfo[c].vertexIndex;
					for (int v = 0; v < 4; v++) newVertexColors[vertexIndex + v].a = 255;
				}
				text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
				yield return null;
			}
			while (curCharIndex < text.textInfo.characterCount);

			//float curTime = 0;
			//bool isDone = false;
			//do {
			//	for (int c = 0; c < text.textInfo.characterCount; c++) {
			//		int vertexIndex = text.textInfo.characterInfo[c].vertexIndex;
			//		byte newAlpha = (byte)Mathf.RoundToInt(Mathf.Lerp(0, 255, curTime - (c / (float)lettersPerSecond)));
			//		for (int v = 0; v < 4; v++) newVertexColors[vertexIndex + v].a = newAlpha;
			//		if (c == text.textInfo.characterCount - 1) isDone = newAlpha == 255;
			//	}
			//	text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
			//	curTime += Time.deltaTime / fadeTime;
			//	yield return null;
			//}
			//while (!isDone);

			onAppearifyEnd?.Invoke();
		}
	}
}