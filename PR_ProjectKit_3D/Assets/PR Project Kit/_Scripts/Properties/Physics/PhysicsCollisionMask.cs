using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhysicsCollisionMask {

	[SerializeField] private LayerMask layerMask;
	[SerializeField] private string tag;

	
	public bool DoesPassMask(GameObject other) {
		return false;
	}

}
