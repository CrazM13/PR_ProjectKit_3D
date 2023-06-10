using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingMusic : MonoBehaviour {

	[SerializeField] MusicPlayer player;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) player.PlayMusic(0);
		if (Input.GetKeyDown(KeyCode.Alpha2)) player.PlayMusic(1);
	}
}
