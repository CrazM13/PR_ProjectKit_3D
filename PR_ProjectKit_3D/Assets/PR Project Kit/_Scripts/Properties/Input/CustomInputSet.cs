using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomInputSet {

	public enum AxisOverrideType {
		AxisOnly = 0,
		KeycodeOnly = 1,
		AxisAndKeycode = 2,
		Disabled = 9
	}

	[SerializeField] private string axisName;
	[SerializeField] private AxisOverrideType overrideAxis;
	[SerializeField] private KeyCode positiveKey;
	[SerializeField] private KeyCode negativeKey;

	public float RawValue {
		get {
			float output = 0;

			if (overrideAxis == AxisOverrideType.Disabled) {
				return 0;
			}

			if (overrideAxis != AxisOverrideType.KeycodeOnly) {
				output = Input.GetAxisRaw(axisName);
			}

			if (overrideAxis != AxisOverrideType.AxisOnly) {
				output += Input.GetKey(positiveKey) ? 1 : Input.GetKey(negativeKey) ? -1 : 0;
			}

			output = Mathf.Clamp(output, -1, 1);
			return output;
		}
	}

	public float Value {
		get {
			float output = 0;

			if (overrideAxis == AxisOverrideType.Disabled) {
				return 0;
			}

			if (overrideAxis != AxisOverrideType.KeycodeOnly) {
				output = Input.GetAxis(axisName);
			}

			if (overrideAxis != AxisOverrideType.AxisOnly) {
				output += Input.GetKey(positiveKey) ? 1 : Input.GetKey(negativeKey) ? -1 : 0;
			}

			output = Mathf.Clamp(output, -1, 1);
			return output;
		}
	}

	public bool IsButtonDown {
		get {
			bool value = false;

			if (overrideAxis == AxisOverrideType.Disabled) {
				return false;
			}

			if (overrideAxis != AxisOverrideType.KeycodeOnly) {
				value = Input.GetButtonDown(axisName);
			}

			if (overrideAxis != AxisOverrideType.AxisOnly) {
				value |= Input.GetKeyDown(positiveKey) || Input.GetKeyDown(negativeKey);
			}

			return value;
		}
	}

	public bool IsButton {
		get {
			bool value = false;

			if (overrideAxis == AxisOverrideType.Disabled) {
				return false;
			}

			if (overrideAxis != AxisOverrideType.KeycodeOnly) {
				value = Input.GetButton(axisName);
			}

			if (overrideAxis != AxisOverrideType.AxisOnly) {
				value |= Input.GetKey(positiveKey) || Input.GetKey(negativeKey);
			}

			return value;
		}
	}

	public bool IsButtonUp {
		get {
			bool value = false;

			if (overrideAxis == AxisOverrideType.Disabled) {
				return false;
			}

			if (overrideAxis != AxisOverrideType.KeycodeOnly) {
				value = Input.GetButtonUp(axisName);
			}

			if (overrideAxis != AxisOverrideType.AxisOnly) {
				value |= Input.GetKeyUp(positiveKey) || Input.GetKeyUp(negativeKey);
			}

			return value;
		}
	}

}
