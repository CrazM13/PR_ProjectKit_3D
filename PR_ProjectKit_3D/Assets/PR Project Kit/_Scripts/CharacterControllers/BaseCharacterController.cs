using UnityEngine;
using UnityEngine.Events;

public class BaseCharacterController : MonoBehaviour {

	#region Subclasses
	[System.Serializable]
	private class CrouchSettings {
		[SerializeField] public float speedPercentage = 1;
		[SerializeField, Min(0.5f)] public float heightPercentage = 1;
	}
	#endregion

	#region Enums
	public enum MovementType {
		Standard = 0,
		Sprint = 1,
		Crouch = 2,
		Prone = 3
	}
	#endregion

	#region Inspector
	[Header("References")]
	[SerializeField] private new Rigidbody rigidbody;
	[SerializeField] private new CapsuleCollider collider;

	[Header("Settings")]
	[SerializeField] private float movementSpeed = 1;
	[SerializeField] private AnimationCurve acceleration = AnimationCurve.Constant(0, 1, 1);

	[SerializeField] private float jumpHeight;

	[SerializeField] private string timeChannel = "CharacterTime";

	[Header("Sprinting")]
	[SerializeField] private float sprintingSpeedPercentage = 1.25f;

	[Header("Crouching")]
	[SerializeField] private CrouchSettings crouch;

	[Header("Prone")]
	[SerializeField] private CrouchSettings prone;
	#endregion

	#region Events
	public UnityEvent<MovementType, MovementType> OnMovementTypeChange { get; private set; } = new UnityEvent<MovementType, MovementType>();
	#endregion

	#region Init
	void Start() {
		if (!GameTime.DoesChannelExist(timeChannel)) {
			GameTime.RegisterChannel(timeChannel);
		}

		InitHitbox();

		OnMovementTypeChange.AddListener(UpdateHitbox);
	}
	#endregion

	#region Movement
	private float movementTime = 0;

	private MovementType currentMovementType = MovementType.Standard;

	private Vector3? targetPosition;

	public float CurrentSpeed => acceleration.Evaluate(movementTime) * movementSpeed * SpeedModifier;

	public float SpeedModifier => currentMovementType switch {
		MovementType.Sprint => sprintingSpeedPercentage,
		MovementType.Crouch => crouch.speedPercentage,
		MovementType.Prone => prone.speedPercentage,
		_ => 1
	};

	// Update is called once per frame
	private void FixedUpdate() {
		UpdateMovement();
	}

	private void UpdateMovement() {

		if (targetPosition.HasValue) {
			movementTime += GameTime.GetDeltaTime(timeChannel);

			Vector3 moveToPosition = Vector3.MoveTowards(transform.position, targetPosition.Value, CurrentSpeed * GameTime.GetFixedDeltaTime(timeChannel));

			rigidbody.MovePosition(moveToPosition);
			if (transform.position == targetPosition.Value) targetPosition = null;
		} else movementTime = 0;

		ApplyJump();
	}
	#endregion

	#region Jump
	public bool IsGrounded { get; private set; } = true;
	
	private void ApplyJump() {
		if (!IsGrounded) {
			//rigidbody.velocity += Physics.gravity * GameTime.GetDeltaTime(timeChannel);

			if (rigidbody.velocity.y <= 0 && CheckGrounded()) {
				IsGrounded = true;
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
			}

		}
	}

	private bool CheckGrounded() {
		return Physics.CheckSphere(new Vector3(collider.bounds.center.x, collider.bounds.min.y - 0.01f, collider.bounds.center.z), 0.01f);
	}
	#endregion

	#region Hitbox
	private Vector3 hitboxCenter;
	private float hitboxHeight;

	private void InitHitbox() {
		hitboxCenter = collider.center;
		hitboxHeight = collider.height;
	}

	private void UpdateHitbox(MovementType oldMovementType, MovementType newMovementType) {
		switch (newMovementType) {
			case MovementType.Crouch:
				collider.center = GetNewHitboxCenter(crouch.heightPercentage);
				collider.height = hitboxHeight * crouch.heightPercentage;
				break;
			case MovementType.Prone:
				collider.center = GetNewHitboxCenter(prone.heightPercentage);
				collider.height = hitboxHeight * prone.heightPercentage;
				break;
			default:
				collider.center = hitboxCenter;
				collider.height = hitboxHeight;
				break;
		}
	}

	private Vector3 GetNewHitboxCenter(float modifier) {
		float newHeight = hitboxHeight * modifier;
		float difference = hitboxHeight - newHeight;

		return hitboxCenter - (0.5f * difference * Vector3.up);
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
		rigidbody.position = newPosition;
		targetPosition = null;
	}

	public void Jump(float force) {
		rigidbody.velocity += Vector3.up * (Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight * force)));
		IsGrounded = false;
	}

	public void SetMovementType(MovementType newMovementType) {
		MovementType oldMovementType = currentMovementType;
		currentMovementType = newMovementType;

		OnMovementTypeChange.Invoke(oldMovementType, currentMovementType);
	}

	public MovementType GetMovementType() => currentMovementType;
	#endregion

}
