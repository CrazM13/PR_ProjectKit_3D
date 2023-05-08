using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectControlController : MonoBehaviour {

	[SerializeField] private BaseCharacterController character;
	[SerializeField] private new CameraControllerBase camera;

	[SerializeField] private CustomInputSet horizontalMovement;
	[SerializeField] private CustomInputSet verticalMovement;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (horizontalMovement.Value != 0 || verticalMovement.Value != 0) {
			UpdateMovement();
		} else {
			character.StopMoving();
		}

	}

	private void UpdateMovement() {
		Vector3 forward = camera.GetForwardDirection();
		Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;

		Vector3 movement = (forward * verticalMovement.Value + right * horizontalMovement.Value).normalized;

		character.MoveTo(transform.position + (movement * character.CurrentSpeed));
	}

}
