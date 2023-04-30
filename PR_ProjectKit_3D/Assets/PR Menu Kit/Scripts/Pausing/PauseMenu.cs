using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

	[SerializeField] private CanvasGroup pauseCanvas;

	private bool isShowing = false;
	public bool IsShowing {

		get => isShowing;
		set {
			pauseCanvas.alpha = value ? 1 : 0;
			pauseCanvas.interactable = value;
			pauseCanvas.blocksRaycasts = value;
		}
	}



	
}
