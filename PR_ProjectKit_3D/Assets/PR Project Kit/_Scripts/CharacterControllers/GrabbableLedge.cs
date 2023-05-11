using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableLedge : MonoBehaviour {

	[SerializeField] private Vector3 normalVector;
	[SerializeField] private Vector3 movementDirection;
	[SerializeField] private float maxDistance;

	private void OnDrawGizmos() {
		Color savedColor = Gizmos.color;

		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position + (movementDirection * maxDistance), transform.position - (movementDirection * maxDistance));

		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + normalVector);

		Gizmos.color = savedColor;
	}

	public float GetMaxDistance() => maxDistance;
	public Vector3 GetLedgeDirection() => movementDirection;
	public Vector3 GetNormalVector() => normalVector;

	private void OnTriggerEnter(Collider other) {
		BaseCharacterController character = other.GetComponent<BaseCharacterController>();
		if (character && character.GetMovementType() != BaseCharacterController.MovementType.LedgeGrabbing) {
			character.SnapToLedge(this);
		}
	}

}
