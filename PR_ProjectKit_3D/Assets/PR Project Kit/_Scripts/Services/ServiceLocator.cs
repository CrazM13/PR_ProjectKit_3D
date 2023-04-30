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
	#endregion

	#region Load/Unload Services
	private void LocateServices() {
		SceneManager = FindAnyObjectByType<PRSceneManager>();
	}

	private void ForgetServices() {
		SceneManager = null;
	}
	#endregion
}
