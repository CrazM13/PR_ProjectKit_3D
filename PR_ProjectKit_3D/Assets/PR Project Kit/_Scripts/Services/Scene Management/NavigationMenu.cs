using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMenu : MonoBehaviour {

	public void LoadSceneByName(string name) {
		ServiceLocator.SceneManager.LoadScene(name);
	}

	public void OpenWebpage(string url) {
		Application.OpenURL(url);
	}

	public void QuitGame() {
		Application.Quit();
	}
}
