using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomMouseInput {

	public enum MouseInputType {
		WhenPressed = 0,
		AlwaysActive = 1,
		Disabled = 9
	}

	[SerializeField] private int mouseButton;
	[SerializeField] private MouseInputType inputType;

	private bool alwaysOverrideWasDown = false;

	public bool IsButtonDown {
		get {
			if (inputType == MouseInputType.Disabled) return false;

			if (inputType == MouseInputType.AlwaysActive && !alwaysOverrideWasDown) {
				alwaysOverrideWasDown = true;
				return true;
			}

			return Input.GetMouseButtonDown(mouseButton);
		}
	}

	public bool IsButton {
		get {
			if (inputType == MouseInputType.Disabled) return false;

			if (inputType == MouseInputType.AlwaysActive) {
				return true;
			} else {
				alwaysOverrideWasDown = false;
			}

			return Input.GetMouseButton(mouseButton);
		}
	}

	public bool IsButtonUp {
		get {
			if (inputType == MouseInputType.Disabled) return false;
			if (inputType == MouseInputType.AlwaysActive) return false;

			return Input.GetMouseButtonUp(mouseButton);
		}
	}
}
