using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingMusic : MonoBehaviour {

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) ServiceLocator.AudioManager.MusicController.PlayMusic(0);
		if (Input.GetKeyDown(KeyCode.Alpha2)) ServiceLocator.AudioManager.MusicController.PlayMusic(1);
	}
}
