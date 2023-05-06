using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomInputSet {

	public enum AxisOverrideType {
		AxisOnly = 0,
		KeycodeOnly = 1,
		AxisAndKeycode = 2
	}

	[SerializeField] private string axisName;
	[SerializeField] private AxisOverrideType overrideAxis;
	[SerializeField] private KeyCode positiveKey;
	[SerializeField] private KeyCode negativeKey;

	public float Value {
		get {
			float output = 0;

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

}
