using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyCameraController : CameraControllerBase {

	#region Subclasses
	[System.Serializable]
	private class InputSet {
		[Header("Sensitivity")]
		[SerializeField] public float translateSensitivity;
		[SerializeField] public float rotationSensitivity;
		[SerializeField] public float zoomSensitivity;
	}

	[System.Serializable]
	private class KeyboardInputSet : InputSet {
		[Header("Axis")]
		[SerializeField] public CustomInputSet translateHorizontalAxis;
		[SerializeField] public CustomInputSet translateVerticalAxis;
		[SerializeField] public CustomInputSet rotationAxis;
		[SerializeField] public CustomInputSet zoomAxis;
	}

	[System.Serializable]
	private class MouseInputSet : InputSet {
		[Header("Mouse Buttons")]
		[SerializeField] public CustomMouseInput translateMouseButton;
		[SerializeField] public CustomMouseInput rotateMouseButton;
	}

	[System.Serializable] 
	private class FollowInputSet {
		[Header("Reference")]
		[SerializeField] public Transform followTarget;
		[SerializeField] public bool rotateWithTarget;
	}

	#endregion

	#region Enums
	[System.Flags]
	public enum InputType {
		Mouse = 1,
		Keyboard = 2,
		Follow = 4
	}
	#endregion

	#region Inspector
	[Header("References")]
	[SerializeField] private Transform cameraRig;
	[SerializeField] private Transform cameraTransform;

	[Header("Settings")]
	[SerializeField] private Space coordinateSpace;
	[SerializeField] private float movementSpeed;
	[SerializeField] private bool adjustMovementToZoom;
	[SerializeField] private MinMax zoomContraints;

	[Header("Inputs")]
	[SerializeField] private InputType inputMethod;
	[SerializeField] private FollowInputSet follow;
	[SerializeField] private KeyboardInputSet keyboard;
	[SerializeField] private MouseInputSet mouse;
	#endregion

	#region Variables
	private Camera mainCamera;

	private Vector3 focusPoint;
	private float rotation;
	private float zoom;

	private Vector3 dragStartPosition;
	private Vector3 rotateStartPosition;
	#endregion

	#region Movement
	// Start is called before the first frame update
	void Start() {
		mainCamera = Camera.main;

		focusPoint = cameraRig.position;
		rotation = transform.rotation.eulerAngles.y;
		zoom = LocalPositionToZoom(cameraTransform.localPosition);
	}

	// Update is called once per frame
	void LateUpdate() {

		if (inputMethod.HasFlag(InputType.Follow)) HandleFollow();
		if (inputMethod.HasFlag(InputType.Mouse)) HandleMouseInput();
		if (inputMethod.HasFlag(InputType.Keyboard)) HandleKeyboardInput();

		cameraRig.SetPositionAndRotation(Vector3.Lerp(cameraRig.position, focusPoint, Time.deltaTime * movementSpeed), Quaternion.Lerp(cameraRig.rotation, Quaternion.AngleAxis(rotation, Vector3.up), Time.deltaTime * movementSpeed));
		cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, ZoomToLocalPosition(zoom), Time.deltaTime * movementSpeed);
	}

	private void HandleKeyboardInput() {
		zoom += keyboard.zoomAxis.Value * keyboard.zoomSensitivity;
		zoom = zoomContraints.Clamp(zoom);

		float zoomSensitivity = adjustMovementToZoom ? (zoom / zoomContraints.max) : 1;

		Vector3 translateInput = (keyboard.translateSensitivity * zoomSensitivity) * new Vector3(keyboard.translateHorizontalAxis.Value, 0, keyboard.translateVerticalAxis.Value);
		if (coordinateSpace == Space.World) translateInput = cameraRig.TransformVector(translateInput);

		focusPoint += translateInput;

		rotation += keyboard.rotationAxis.Value * keyboard.rotationSensitivity;
	}

	private void HandleMouseInput() {

		// Translate
		if (mouse.translateMouseButton.IsButtonDown) {
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (plane.Raycast(ray, out float entry)) {
				dragStartPosition = ray.GetPoint(entry);
			}
		} else if (mouse.translateMouseButton.IsButton) {
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

			if (plane.Raycast(ray, out float entry)) {
				Vector3 dragCurrentPosition = ray.GetPoint(entry);

				focusPoint = cameraRig.position + ((dragStartPosition - dragCurrentPosition) * mouse.translateSensitivity);
			}
		}

		// Rotation
		if (mouse.rotateMouseButton.IsButtonDown) {
			rotateStartPosition = Input.mousePosition;
		} else if (mouse.rotateMouseButton.IsButton) {
			Vector3 rotateCurrentPosition = Input.mousePosition;
			Vector3 rotateDifference = rotateStartPosition - rotateCurrentPosition;
			rotateStartPosition = rotateCurrentPosition;

			rotation += -rotateDifference.x  * mouse.rotationSensitivity;

		}

		// Zoom
		if (Input.mouseScrollDelta.y != 0) {
			zoom += -Input.mouseScrollDelta.y * mouse.zoomSensitivity;
			zoom = zoomContraints.Clamp(zoom);
		}
	}

	private void HandleFollow() {
		if (!follow.followTarget) return;

		focusPoint = new Vector3(follow.followTarget.position.x, 0, follow.followTarget.position.z);

		if (follow.rotateWithTarget) rotation = follow.followTarget.rotation.eulerAngles.y;
	}

	private Vector3 ZoomToLocalPosition(float zoom) {
		return new Vector3(0, zoom, -zoom);
	}

	private float LocalPositionToZoom(Vector3 position) {
		return position.y;
	}
	#endregion

	#region Interface

	public void ActivateInputMethod(InputType inputType) {
		inputMethod |= inputType;
	}

	public void ActivateInputMethod(params InputType[] inputTypes) {
		foreach (InputType inputType in inputTypes) inputMethod |= inputType;
	}

	public void DeactivateInputMethod(InputType inputType) {
		inputMethod &= ~inputType;
	}

	public void DeactivateInputMethod(params InputType[] inputTypes) {
		foreach (InputType inputType in inputTypes) inputMethod &= ~inputType;
	}

	public void SetFocus(Transform target, bool rotateWithTarget = false) {
		follow.followTarget = target;
		follow.rotateWithTarget = rotateWithTarget;
	}

	public void SetFocus(Vector3 position, float rotation) {
		focusPoint = position;
		this.rotation = rotation;
	}

	public void SetFocus(Vector3 position) {
		focusPoint = position;
	}

	public override Vector3 GetForwardDirection() {
		return cameraRig.TransformDirection(Vector3.forward);
	}

	#endregion

}
