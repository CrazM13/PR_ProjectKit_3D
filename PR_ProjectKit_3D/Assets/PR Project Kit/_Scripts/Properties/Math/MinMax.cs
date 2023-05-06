using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MinMax {

	[SerializeField] public float min;
	[SerializeField] public float max;
	
	public float Clamp(float value) {
		return Mathf.Clamp(value, min, max);
	}

}
