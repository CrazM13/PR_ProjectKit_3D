using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PRSceneManager : MonoBehaviour {

	#region Constants
	private const string WARNING_MULTIPLE_LOAD_CALLS = "[Scene Manager] Warning! Load Scene called while actively transitioning. Call discarded.";
	#endregion

	public UnityEvent<float> OnSceneChangeQueued { get; private set; } = new UnityEvent<float>();
	public UnityEvent OnSceneChange { get; private set; } = new UnityEvent();
	private bool isTransitioning = false;

	private float forceWait = 0;

	#region Interface
	public void LoadScene(string sceneName, float delay = 0, UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single) {
		if (isTransitioning) {
			Debug.LogWarning(WARNING_MULTIPLE_LOAD_CALLS);
			return;
		}

		OnSceneChangeQueued.Invoke(delay);


		StartCoroutine(LoadSceneAfterDelay(sceneName, Mathf.Max(delay, forceWait), loadMode));

		forceWait = 0;
		isTransitioning = true;
	}

	public void LoadScene(int sceneIndex, float delay = 0, UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single) {
		if (isTransitioning) {
			Debug.LogWarning(WARNING_MULTIPLE_LOAD_CALLS);
			return;
		}

		OnSceneChangeQueued.Invoke(delay);


		StartCoroutine(LoadSceneAfterDelay(sceneIndex, Mathf.Max(delay, forceWait), loadMode));

		forceWait = 0;
		isTransitioning = true;
	}

	public void ForceSceneLoad(int sceneIndex, UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single) {
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex, loadMode);
	}

	public void ForceSceneLoad(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadMode = UnityEngine.SceneManagement.LoadSceneMode.Single) {
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, loadMode);
	}

	public void ExtendTransition(float waitTime) {
		if (waitTime > this.forceWait) this.forceWait = waitTime;
	}

	public int GetCurrentSceneIndex() {
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
	}

	public string GetCurrentSceneName() {
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	}
	#endregion

	private IEnumerator LoadSceneAfterDelay(string sceneName, float delay, UnityEngine.SceneManagement.LoadSceneMode loadMode) {
		yield return new WaitForSeconds(delay);

		OnSceneChange.Invoke();
		ForceSceneLoad(sceneName, loadMode);
	}

	private IEnumerator LoadSceneAfterDelay(int sceneIndex, float delay, UnityEngine.SceneManagement.LoadSceneMode loadMode) {
		yield return new WaitForSeconds(delay);

		OnSceneChange.Invoke();
		ForceSceneLoad(sceneIndex, loadMode);
	}

}
