using UnityEngine;
using UnityEditor;
using DecisionFramework;

[CustomPropertyDrawer(typeof(Property))]
[CanEditMultipleObjects]
public class PropertyEditor : PropertyDrawer {
	const float PROP_HEIGHT = 19f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		Rect propRect = position;
		propRect.height = PROP_HEIGHT;
		EditorGUI.PropertyField(propRect, property.FindPropertyRelative("type"));
		propRect.y += PROP_HEIGHT;
		EditorGUI.PropertyField(propRect, property.FindPropertyRelative("name"));
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return PROP_HEIGHT * 2;
	}
}