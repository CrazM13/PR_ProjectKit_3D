using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviour {

	#region Singleton
	private ServiceLocator instance;
	private void Awake() {
		if (!instance) {
			instance = this;

			LocateServices();
		} else {
			Destroy(this);
		}
	}

	private void OnDestroy() {
		ForgetServices();
	}
	#endregion

	#region Services
	public static PRSceneManager SceneManager;
	public static PRAudioManager AudioManager;
	#endregion

	#region Load/Unload Services
	private void LocateServices() {
		SceneManager = FindObjectOfType<PRSceneManager>();
		AudioManager = FindObjectOfType<PRAudioManager>();
	}

	private void ForgetServices() {
		SceneManager = null;
		AudioManager = null;
	}
	#endregion
}
