using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour {

	[SerializeField] private Zone parent;

	private void OnTriggerEnter(Collider other) {
		parent.EnterZone();
	}

	private void OnTriggerExit(Collider other) {
		parent.ExitZone();
	}

}
