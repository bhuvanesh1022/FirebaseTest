using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Utilities {
	public static void AppearifyText(TMP_Text text, int lettersPerSecond = 60, float fadeTime = 0.2f) {
		text.StartCoroutine(AppearifyCR());

		IEnumerator AppearifyCR() {
			text.ForceMeshUpdate();
			Color32[] newVertexColors;
			newVertexColors = text.textInfo.meshInfo[text.textInfo.characterInfo[0].materialReferenceIndex].colors32;
			for (int v = 0; v < newVertexColors.Length; v++) newVertexColors[v].a = 0;
			text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
			yield return null;

			int curCharIndex = 0;
			do {
				int lastCharIndex = curCharIndex;
				curCharIndex = Mathf.Min(curCharIndex + Mathf.CeilToInt(Time.deltaTime * lettersPerSecond), text.textInfo.characterCount);
				for (int c = lastCharIndex; c < curCharIndex; c++) {
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
		}
	}
}