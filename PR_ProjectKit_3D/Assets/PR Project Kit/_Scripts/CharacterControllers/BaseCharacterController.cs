using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour {

	#region Enums
	private enum MovementType {
		Transform,
		Rigidbody
	}
	#endregion

	[Header("References")]
	[SerializeField] private new Rigidbody rigidbody;

	[Header("Settings")]
	[SerializeField] private MovementType movementType;

	[SerializeField] private float movementSpeed = 1;
	[SerializeField] private AnimationCurve acceleration = AnimationCurve.Constant(0, 1, 1);

	[SerializeField] private string timeChannel = "CharacterTime";

	private float movementTime = 0;

	private Vector3? targetPosition;

	public float CurrentSpeed => acceleration.Evaluate(movementTime) * movementSpeed;

	#region Init
	void Start() {
		if (!GameTime.DoesChannelExist(timeChannel)) {
			GameTime.RegisterChannel(timeChannel);
		}
	}
	#endregion

	#region Movement
	// Update is called once per frame
	private void Update() {
		if (movementType == MovementType.Transform) {
			if (targetPosition.HasValue) UpdateTransformMovement();
			else movementTime = 0;
		}
	}

	private void FixedUpdate() {
		if (movementType == MovementType.Rigidbody) {
			if (targetPosition.HasValue) UpdateRigidbodyMovement();
			else movementTime = 0;
		}
	}

	private void UpdateTransformMovement() {
		movementTime += GameTime.GetDeltaTime(timeChannel);

		Vector3 frameTarget = Vector3.MoveTowards(transform.position, targetPosition.Value, CurrentSpeed * GameTime.GetDeltaTime(timeChannel));

		transform.position = frameTarget;

		if (transform.position == targetPosition.Value) targetPosition = null;
	}

	private void UpdateRigidbodyMovement() {
		movementTime += GameTime.GetDeltaTime(timeChannel);

		Vector3 frameTarget = Vector3.MoveTowards(rigidbody.position, targetPosition.Value, CurrentSpeed * GameTime.GetFixedDeltaTime(timeChannel));

		rigidbody.MovePosition(frameTarget);

		if (rigidbody.position == targetPosition.Value) targetPosition = null;
	}
	#endregion

	#region Interface
	public void MoveTo(Vector3 newPosition) {
		targetPosition = newPosition;
	}

	public void StopMoving() {
		targetPosition = null;
	}

	public void WarpTo(Vector3 newPosition) {
		if (movementType == MovementType.Transform) transform.position = newPosition;
		else if (movementType == MovementType.Rigidbody) rigidbody.position = newPosition;
		targetPosition = null;
	}
	#endregion

}
