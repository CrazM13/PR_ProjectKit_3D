using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectControlController : MonoBehaviour {

	[SerializeField] private BaseCharacterController character;
	[SerializeField] private new CameraControllerBase camera;

	[SerializeField] private CustomInputSet horizontalMovement;
	[SerializeField] private CustomInputSet verticalMovement;

	[SerializeField] private CustomInputSet jumpControl;
	[SerializeField] private CustomInputSet crouchControl;
	[SerializeField] private CustomInputSet sprintControl;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		UpdateMovementType();

		if (horizontalMovement.Value != 0 || verticalMovement.Value != 0) {
			UpdateMovement();
		}

		if (jumpControl.RawValue != 0 && character.IsGrounded) {
			character.Jump(1);
		}
	}

	private void UpdateMovement() {
		Vector3 forward = camera.GetForwardDirection();
		Vector3 right = Quaternion.AngleAxis(90, Vector3.up) * forward;

		Vector3 movement = (forward * verticalMovement.Value + right * horizontalMovement.Value).normalized;

		character.Move(movement);
	}

	private void UpdateMovementType() {

		if (crouchControl.IsButtonDown) {
			if (character.GetMovementType() == BaseCharacterController.MovementType.Crouch) {
				character.SetMovementType(BaseCharacterController.MovementType.Prone);
			} else if (character.GetMovementType() == BaseCharacterController.MovementType.Prone) {
				character.SetMovementType(BaseCharacterController.MovementType.Standard);
			} else if (character.GetMovementType() != BaseCharacterController.MovementType.Crouch) {
				character.SetMovementType(BaseCharacterController.MovementType.Crouch);
			}
		}

		if (sprintControl.Value > 0) {
			if (character.GetMovementType() != BaseCharacterController.MovementType.Sprint) {
				character.SetMovementType(BaseCharacterController.MovementType.Sprint);
			}
		} else {
			if (character.GetMovementType() == BaseCharacterController.MovementType.Sprint) {
				character.SetMovementType(BaseCharacterController.MovementType.Standard);
			}
		}
	}

}
