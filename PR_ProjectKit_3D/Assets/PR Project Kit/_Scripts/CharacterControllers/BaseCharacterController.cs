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

	public enum RotationType {
		None = 0,
		Movement = 1,
		Camera = 2
	}
	#endregion

	#region Inspector
	[Header("References")]
	[SerializeField] private new Rigidbody rigidbody;
	[SerializeField] private new CapsuleCollider collider;
	[SerializeField] private new CameraControllerBase camera;

	[Header("Settings")]
	[SerializeField] private LayerMask movementCollisionLayers;
	[SerializeField] private float movementSpeed = 1;
	[SerializeField] private AnimationCurve acceleration = AnimationCurve.Constant(0, 1, 1);

	[SerializeField] private RotationType rotationType;
	[SerializeField] private float stepHeight;
	[SerializeField] private float jumpHeight;
	[SerializeField] private LayerMask groundCollisionLayers;

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

	private Vector3? movementDirection;

	public float CurrentSpeed => acceleration.Evaluate(movementTime) * movementSpeed * SpeedModifier;

	public float SpeedModifier => currentMovementType switch {
		MovementType.Sprint => sprintingSpeedPercentage,
		MovementType.Crouch => crouch.speedPercentage,
		MovementType.Prone => prone.speedPercentage,
		MovementType.LedgeGrabbing => ledgeGrab.speedPercentage,
		_ => 1
	};

	private Vector3 FootPoint => new Vector3(collider.bounds.center.x, collider.bounds.min.y, collider.bounds.center.z);

	// Update is called once per frame
	private void Update() {
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

	private bool WillCollide(Vector3 position, Vector3 direction, float distance, float padding = 0, int mask = -1) {

		float height = collider.height - (collider.radius * 2f);
		float radius = collider.radius - padding;
		float normalHeight = Mathf.Max(height, 0);
		Vector3 pointOffset = (0.5f * normalHeight * Vector3.up);

		RaycastHit[] hits;
		hits = Physics.CapsuleCastAll(position + pointOffset, position - pointOffset, radius, direction, distance + radius, mask, QueryTriggerInteraction.Ignore);
		
		foreach (RaycastHit hit in hits) {
			if (hit.collider != collider) {
				return true;
			}
		}
		return false;
	}

	private void ProcessStandardMovement() {
		if (movementDirection.HasValue) {
			movementTime += GameTime.GetDeltaTime(timeChannel);

			float speed = GameTime.GetDeltaTime(timeChannel) * CurrentSpeed;
			Vector3 attemptMove = speed * movementDirection.Value;
			if (!WillCollide(collider.bounds.center, movementDirection.Value, speed, 0.1f, movementCollisionLayers)) {
				transform.localPosition += attemptMove;
			} else if (IsGrounded && !WillCollide(collider.bounds.center + (stepHeight * Vector3.up), movementDirection.Value, speed, 0.1f, movementCollisionLayers)) {
				transform.localPosition += attemptMove + (stepHeight * Vector3.up);

				SnapToFloor(stepHeight + 1f);
			}

			if (rotationType == RotationType.Movement) {
				// Rotation
				transform.rotation = Quaternion.LookRotation(movementDirection.Value, Vector3.up);
			}

			movementDirection = null;
		} else movementTime = 0;

		if (rotationType == RotationType.Camera) {
			transform.rotation = Quaternion.LookRotation(camera.GetForwardDirection(), Vector3.up);
		}

		ApplyJump();
	}

	private GrabbableLedge currentLedge;
	private float ledgeDistance = 0;
	private float ledgeCooldown = 0;

	private void ProcessLedgeMovement() {
		if (currentLedge) {
			if (movementDirection.HasValue) {
				movementTime += GameTime.GetDeltaTime(timeChannel);

				Vector3 directionToMove = CurrentSpeed * GameTime.GetDeltaTime(timeChannel) * movementDirection.Value;

				Vector3 ledgeDirection = currentLedge.GetLedgeDirection();

				Vector3 movementOnLedge = Vector3.Project(directionToMove, ledgeDirection);

				if (!WillCollide(transform.localPosition + movementOnLedge, ledgeDirection, 0.1f)) {
					ledgeDistance += (movementOnLedge.x + movementOnLedge.y + movementOnLedge.z);
					if (Mathf.Abs(ledgeDistance) > currentLedge.GetMaxDistance()) {
						ledgeCooldown = ledgeGrab.regrabCooldown;
						IsGrounded = false;
						SetMovementType(MovementType.Standard);
					}
				}

				movementDirection = null;
			} else movementTime = 0;
		}

		transform.localPosition = currentLedge.transform.localPosition + (currentLedge.GetLedgeDirection() * ledgeDistance) + transform.TransformVector(ledgeGrab.holdingOffset);

		IsGrounded = true;

		// Rotation
		transform.rotation = Quaternion.LookRotation(-currentLedge.GetNormalVector(), Vector3.up);

		if (nextJumpForce != 0) {
			// Dismount Ledge
			IsGrounded = false;
			SetMovementType(MovementType.Standard);
			ledgeCooldown = ledgeGrab.regrabCooldown;
			nextJumpForce *= 2;
		}
	}

	private void UpdateMovementCooldowns() {
		if (ledgeCooldown > 0) {
			ledgeCooldown -= GameTime.GetDeltaTime(timeChannel);
			if (ledgeCooldown <= 0) {
				currentLedge = null;
			}
		}
	}
	#endregion

	#region Jump
	public bool IsGrounded { get; private set; } = true;
	private float nextJumpForce;
	private float currentJumpVelocity;
	private void ApplyJump() {
		if (nextJumpForce != 0) {
			currentJumpVelocity = Mathf.Sqrt(-2.0f * Physics2D.gravity.y * (jumpHeight * nextJumpForce));
			nextJumpForce = 0;
			IsGrounded = false;
		}

		if (!IsGrounded) {
			if (currentJumpVelocity <= 0 && CheckGrounded()) {
				IsGrounded = true;

				SnapToFloor(currentJumpVelocity);
				currentJumpVelocity = 0;
			} else {
				currentJumpVelocity += Physics.gravity.y * GameTime.GetDeltaTime(timeChannel);
				transform.localPosition += currentJumpVelocity * GameTime.GetDeltaTime(timeChannel) * Vector3.up;
			}
		} else {
			if (!CheckGrounded()) {
				IsGrounded = false;
			}

			currentJumpVelocity = 0;
		}
	}

	private bool CheckGrounded() {
		float distance = stepHeight;
		if (currentJumpVelocity < 0) {
			distance = Mathf.Max(distance, -currentJumpVelocity);
		}

		return WillCollide(collider.bounds.center, Vector3.down, distance * GameTime.GetDeltaTime(timeChannel), 0, groundCollisionLayers);
	}

	private void SnapToFloor(float maxDistance) {
		maxDistance = Mathf.Abs(maxDistance);

		float newTargetY = GetFloorCollisionHeight(collider.bounds.center, maxDistance, 0.1f);
		newTargetY += (collider.height * 0.5f) - collider.center.y;
		if (newTargetY < transform.position.y) {
			transform.position = new Vector3(transform.position.x, newTargetY, transform.position.z);
		}
	}

	private float GetFloorCollisionHeight(Vector3 position, float distance, float padding = 0) {
		RaycastHit[] hits;

		float height = collider.height - (collider.radius * 2f);
		float radius = collider.radius - padding;
		float normalHeight = Mathf.Max(height, 0);
		Vector3 pointOffset = (0.5f * normalHeight * Vector3.up);

		hits = Physics.CapsuleCastAll(position + pointOffset, position - pointOffset, radius, Vector3.down, distance + (2f * padding), -1, QueryTriggerInteraction.Ignore);

		float yHeight = position.y - distance;

		foreach (RaycastHit hit in hits) {
			if (hit.collider != collider) {
				yHeight = Mathf.Max(hit.point.y, yHeight);
			}
		}

		return yHeight;
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

	private void OnDrawGizmos() {
		Color colour = Gizmos.color;

		Gizmos.DrawCube(FootPoint, Vector3.one * 0.1f);

		Gizmos.color = colour;
	}
	#endregion

	#region Interface
	public void Move(Vector3 newDirection) {
		movementDirection = newDirection.normalized;
	}

	public void StopMoving() {
		movementDirection = null;
	}

	public void WarpTo(Vector3 newPosition, bool cancelMovement = true) {
		rigidbody.position = newPosition;
		if (cancelMovement) movementDirection = null;
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
			Vector3 worldGrabPosition = transform.localPosition - transform.TransformVector(ledgeGrab.holdingOffset);
			Vector3 ledgeDirection = ledge.GetLedgeDirection().normalized;
			Vector3 point = worldGrabPosition - ledge.transform.localPosition;
			ledgeDistance = Vector3.Dot(point, ledgeDirection);
		}
	}

	public MovementType GetMovementType() => currentMovementType;
	#endregion

}
