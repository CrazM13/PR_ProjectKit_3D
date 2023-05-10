using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(CustomInputSet))]
public class CustomInputSetPropertyDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		EditorGUI.indentLevel++;

		// Create property fields.

		SerializedProperty axisName = property.FindPropertyRelative("axisName");
		SerializedProperty overrideAxis = property.FindPropertyRelative("overrideAxis");
		SerializedProperty positiveKey = property.FindPropertyRelative("positiveKey");
		SerializedProperty negativeKey = property.FindPropertyRelative("negativeKey");

		EditorGUI.LabelField(GetRectByLine(position, 0), label);

		int currentLine = 1;
		EditorGUI.PropertyField(GetRectByLine(position, currentLine, true), overrideAxis);
		currentLine++;

		CustomInputSet.AxisOverrideType overrideType = (CustomInputSet.AxisOverrideType) overrideAxis.enumValueIndex;
		if (overrideType == CustomInputSet.AxisOverrideType.AxisOnly || overrideType == CustomInputSet.AxisOverrideType.AxisAndKeycode) {
			EditorGUI.PropertyField(GetRectByLine(position, currentLine, true), axisName);
			currentLine++;
		}

		if (overrideType == CustomInputSet.AxisOverrideType.KeycodeOnly || overrideType == CustomInputSet.AxisOverrideType.AxisAndKeycode) {
			EditorGUI.PropertyField(GetRectByLine(position, currentLine, true), positiveKey);
			EditorGUI.PropertyField(GetRectByLine(position, currentLine + 1, true), negativeKey);
		}

		EditorGUI.indentLevel--;
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		SerializedProperty overrideAxis = property.FindPropertyRelative("overrideAxis");
		CustomInputSet.AxisOverrideType overrideType = (CustomInputSet.AxisOverrideType) overrideAxis.enumValueIndex;

		int totalLines = 2;
		if (overrideType == CustomInputSet.AxisOverrideType.AxisOnly) totalLines += 1;
		if (overrideType == CustomInputSet.AxisOverrideType.KeycodeOnly) totalLines += 2;
		if (overrideType == CustomInputSet.AxisOverrideType.AxisAndKeycode) totalLines += 3;

		return (EditorGUIUtility.singleLineHeight * totalLines) + (EditorGUIUtility.standardVerticalSpacing * (totalLines - 1));
	}

	private Rect GetRectByLine(Rect position, int line, bool indented = false) {
		Rect newRect = new Rect(position.min.x, position.min.y + (EditorGUIUtility.singleLineHeight * line) + (EditorGUIUtility.standardVerticalSpacing * (line - 1)), position.size.x, EditorGUIUtility.singleLineHeight);
		if (indented) newRect = EditorGUI.IndentedRect(newRect);
		return newRect;
	}

}
