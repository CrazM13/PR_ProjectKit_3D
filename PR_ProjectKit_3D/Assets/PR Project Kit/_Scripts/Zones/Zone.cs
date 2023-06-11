using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

	private int zoneLevel;

	public bool IsInZone => zoneLevel > 0;

	public void EnterZone() {
		int oldZoneLevel = zoneLevel;

		zoneLevel++;

		if (oldZoneLevel == 0) SendMessage("OnZoneEnter", this, SendMessageOptions.DontRequireReceiver);
	}

	public void ExitZone() {
		zoneLevel--;

		if (!IsInZone) SendMessage("OnZoneExit", this, SendMessageOptions.DontRequireReceiver);
	}

}
