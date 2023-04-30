using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChannel {

	public string Parent { get; private set; }
	public string Name { get; private set; }

	public float TimeScale { get; set; }

	public TimeChannel(string name, string parent = null, float defaultTimeScale = 1) {
		Name = name;
		Parent = parent;
		TimeScale = defaultTimeScale;
	}

}
