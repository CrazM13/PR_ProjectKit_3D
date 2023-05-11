using UnityEngine;
using UnityEngine.Events;

public class BaseCharacterController : MonoBehaviour {

	#region Subclasses
	[System.Serializable]
	private class CrouchSettings {
		[SerializeField] public float speedPercentage = 1;
		[SerializeField, Min(0.5f)] public float heightPercentage = 1;
	}

	[System.Serializable]
	private class LedgeGrabbingSettings {
		[SerializeField] public float speedPercentage = 1;
		[SerializeField] public float regrabCooldown = 1;
		[SerializeField] public Vector3 holdingOffset;
	}
	#endregion

	#region Enums
	public enum MovementType {
		Standard = 0,
		Sprint = 1,
		Crouch = 2,
		Prone = 3,
		LedgeGrabbing = 11
	}
	#endregion

	#region Inspector
	[Header("References")]
	[SerializeField] private new Rigidbody rigidbody;
	[SerializeField] private new CapsuleCollider collider;

	[Header("Settings")]
	[SerializeField] private float movementSpeed = 1;
	[SerializeField] private AnimationCurve acceleration = AnimationCurve.Constant(0, 1, 1);

	[SerializeField] private float stepHeight;
	[SerializeField] private float jumpHeight;

	[SerializeField] private string timeChannel = "CharacterTime";

	[Header("Sprinting")]
	[SerializeField] private float sprintingSpeedPercentage = 1.25f;

	[Header("Crouching")]
	[SerializeField] private CrouchSettings crouch;

	[Header("Prone")]
	[SerializeField] private CrouchSettings prone;

	[Header("Ledge Grabbing")]
	[SerializeField] private LedgeGrabbingSettings ledgeGrab;
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
		MovementType.LedgeGrabbing => ledgeGrab.speedPercentage,
		_ => 1
	};

	// Update is called once per frame
	private void FixedUpdate() {
		UpdateMovement();
	}

	private void UpdateMovement() {

		switch (currentMovementType) {
			case MovementType.LedgeGrabbing:
				ProcessLedgeMovement();
				break;

			default:
				ProcessStandardMovement();
				break;
		}

		UpdateMovementCooldowns();
	}

	private bool WillCollide(Vector3 position, Vector3 direction, float padding) {
		float height = collider.height - stepHeight - padding;
		float radius = collider.radius - (padding * 0.25f);
		float maxDistance = CurrentSpeed * GameTime.GetFixedDeltaTime(timeChannel);

		Vector3 stepPosition = position + (Vector3.up * stepHeight);

		RaycastHit[] hits = Physics.CapsuleCastAll(stepPosition + (0.25f * height * Vector3.up), stepPosition - (0.25f * height * Vector3.up), radius, direction, maxDistance, -1, QueryTriggerInteraction.Ignore);

		foreach (RaycastHit hit in hits) if (hit.collider != collider) return true;
		return false;
	}

	private void ProcessStandardMovement() {
		if (targetPosition.HasValue) {
			movementTime += GameTime.GetDeltaTime(timeChannel);

			Vector3 directionToMove = (targetPosition.Value - transform.position).normalized;
			Vector3 moveToPosition = transform.position;

			Vector3 attemptMove = GameTime.GetFixedDeltaTime(timeChannel) * CurrentSpeed * directionToMove;
			if (!WillCollide(moveToPosition + attemptMove, directionToMove, 0.1f)) {
				moveToPosition += attemptMove;
			}

			// Rotation
			Vector3 lookTarget = new Vector3(directionToMove.x, 0, directionToMove.z);
			transform.rotation = Quaternion.LookRotation(lookTarget, Vector3.up);

			rigidbody.MovePosition(moveToPosition);
			if (transform.position == targetPosition.Value) targetPosition = null;
		} else movementTime = 0;

		ApplyJump();
	}

	private GrabbableLedge currentLedge;
	private float ledgeDistance = 0;
	private float ledgeCooldown = 0;

	private void ProcessLedgeMovement() {
		if (currentLedge) {
			if (targetPosition.HasValue) {
				movementTime += GameTime.GetDeltaTime(timeChannel);

				Vector3 directionToMove = (targetPosition.Value - transform.position).normalized * CurrentSpeed * GameTime.GetFixedDeltaTime(timeChannel);

				Vector3 ledgeDirection = currentLedge.GetLedgeDirection();

				Vector3 movementOnLedge = new Vector3(directionToMove.x * ledgeDirection.x, directionToMove.y * ledgeDirection.y, directionToMove.z * ledgeDirection.z);

				if (!WillCollide(transform.position + movementOnLedge, ledgeDirection, 0.1f)) {
					ledgeDistance += movementOnLedge.x + movementOnLedge.y + movementOnLedge.z;
					if (Mathf.Abs(ledgeDistance) > currentLedge.GetMaxDistance()) {
						ledgeCooldown = ledgeGrab.regrabCooldown;
						IsGrounded = false;
						SetMovementType(MovementType.Standard);
					}
				}

				if (transform.position == targetPosition.Value) targetPosition = null;
			} else movementTime = 0;
		}

		rigidbody.MovePosition(currentLedge.transform.position + (currentLedge.GetLedgeDirection() * ledgeDistance) + transform.TransformVector(ledgeGrab.holdingOffset));

		IsGrounded = true;
		rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

		// Rotation
		transform.rotation = Quaternion.LookRotation(-currentLedge.GetNormalVector(), Vector3.up);

		if (nextJumpForce != 0) {
			// Dismount Ledge
			IsGrounded = false;
			SetMovementType(MovementType.Standard);
			ledgeCooldown = ledgeGrab.regrabCooldown;
		}
	}

	private void UpdateMovementCooldowns() {
		if (ledgeCooldown > 0) {
			ledgeCooldown -= GameTime.GetFixedDeltaTime(timeChannel);
			if (ledgeCooldown <= 0) {
				currentLedge = null;
			}
		}
	}
	#endregion

	#region Jump
	public bool IsGrounded { get; private set; } = true;
	private float nextJumpForce;
	
	private void ApplyJump() {
		if (nextJumpForce != 0) {
			rigidbody.velocity += Vector3.up * (Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight * nextJumpForce)));
			nextJumpForce = 0;
			IsGrounded = false;
		}

		if (!IsGrounded) {
			if (rigidbody.velocity.y <= 0 && CheckGrounded()) {
				IsGrounded = true;
				rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
			}
		} else {
			if (!CheckGrounded()) {
				IsGrounded = false;
			}
		
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
		}
	}

	private bool CheckGrounded() {
		RaycastHit[] hits = Physics.SphereCastAll(new Vector3(collider.bounds.center.x, collider.bounds.center.y - ((0.25f * collider.height) + (collider.radius * 0.25f)), collider.bounds.center.z), collider.radius * 0.75f, Vector3.down, 0.01f, -1, QueryTriggerInteraction.Ignore);
		
		foreach (RaycastHit hit in hits) if (hit.collider != collider) return true;
		return false;
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

	public void WarpTo(Vector3 newPosition, bool cancelMovement = true) {
		rigidbody.position = newPosition;
		if (cancelMovement) targetPosition = null;
	}

	public void Jump(float force) {
		nextJumpForce = force;
	}

	public void ForceGroundedState(bool isGrounded) {
		this.IsGrounded = isGrounded;
	}

	public void SetMovementType(MovementType newMovementType) {
		MovementType oldMovementType = currentMovementType;
		currentMovementType = newMovementType;

		OnMovementTypeChange.Invoke(oldMovementType, currentMovementType);
	}

	public void SnapToLedge(GrabbableLedge ledge, bool overwriteCooldown = false) {
		if (currentLedge != ledge || overwriteCooldown) {
			currentLedge = ledge;
			SetMovementType(MovementType.LedgeGrabbing);
			ledgeCooldown = -1;

			// Get Distance on Ledge
			Vector3 worldGrabPosition = transform.position - transform.TransformVector(ledgeGrab.holdingOffset);
			Vector3 ledgeDirection = ledge.GetLedgeDirection().normalized;
			Vector3 point = worldGrabPosition - ledge.transform.position;
			ledgeDistance = Vector3.Dot(point, ledgeDirection);
		}
	}

	public MovementType GetMovementType() => currentMovementType;
	#endregion

}
