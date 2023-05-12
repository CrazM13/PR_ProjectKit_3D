using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCameraController : CameraControllerBase {

	[Header("References")]
	[SerializeField] private Transform cameraRig;
	[SerializeField] private Transform cameraTransform;

	[Header("Settings")]
	[SerializeField] private MinMax pitchContraints;
	[SerializeField] private Vector2 sensitivity;
	[SerializeField] private bool invertY;

	private float yawRotation;
	private float pitchRotation;

	// Update is called once per frame
	void LateUpdate() {
		yawRotation += Input.GetAxis("Mouse X") * sensitivity.x;
		pitchRotation += -Input.GetAxis("Mouse Y") * sensitivity.y * (invertY ? -1 : 1);
		pitchRotation = pitchContraints.Clamp(pitchRotation);

		cameraRig.rotation = Quaternion.AngleAxis(yawRotation, Vector3.up);
		cameraTransform.localRotation = Quaternion.AngleAxis(pitchRotation, Vector3.right);
	}

	public override Vector3 GetForwardDirection() {
		return cameraRig.TransformDirection(Vector3.forward);
	}
}
