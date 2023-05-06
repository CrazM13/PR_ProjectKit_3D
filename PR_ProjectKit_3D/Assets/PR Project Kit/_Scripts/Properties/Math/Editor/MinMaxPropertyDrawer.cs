using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxPropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);

		// Create property fields.

		SerializedProperty min = property.FindPropertyRelative("min");
		SerializedProperty max = property.FindPropertyRelative("max");

		EditorGUI.LabelField(GetRectByLine(position, 0), label);
		EditorGUI.indentLevel++;

		Rect[] newRects = SplitRect(GetRectByLine(position, 1, true), 2);

		EditorGUI.PropertyField(newRects[0], min);
		EditorGUI.PropertyField(newRects[1], max);

		EditorGUI.indentLevel--;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
	}

	private Rect GetRectByLine(Rect position, int line, bool indented = false) {
		Rect newRect = new Rect(position.min.x, position.min.y + (EditorGUIUtility.singleLineHeight * line) + (EditorGUIUtility.standardVerticalSpacing * (line - 1)), position.size.x, EditorGUIUtility.singleLineHeight);
		if (indented) newRect = EditorGUI.IndentedRect(newRect);
		return newRect;
	}

	private Rect[] SplitRect(Rect source, int count) {

		Rect[] rects = new Rect[count];
		float width = source.size.x / count;

		for (int i = 0; i < count; i++) {
			rects[i] = new Rect(source.min.x + (width * i), source.min.y, width, source.size.y);
		}
		
		return rects;
	}

}
