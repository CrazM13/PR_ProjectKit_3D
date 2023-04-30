using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour {
	private const string TIME_CHANNEL = "SCENE_MANAGEMENT";
	private enum TransitionState {
		IDLE = 0,
		IN = 1,
		OUT = 2
	}

	[Header("References")]
	[SerializeField] private CanvasGroup transitionUI;
	[Header("Settings")]
	[SerializeField] private AnimationCurve presenceOverTime;

	private TransitionState transitionState;
	private float transitionTime = 0;
	private float TransitionDuration => presenceOverTime.keys[presenceOverTime.length - 1].time;

	public float PercentThroughTransition { 
		get {
			if (TransitionDuration == 0) return 0;
			return transitionTime / TransitionDuration;
		}
	}

	private void Start() {
		ServiceLocator.SceneManager.OnSceneChangeQueued.AddListener(PlayTransitionOut);
		transitionState = TransitionState.IN;
		transitionTime = TransitionDuration;

		if (!GameTime.DoesChannelExist(TIME_CHANNEL)) {
			GameTime.RegisterChannel(TIME_CHANNEL);
		}
	}

	private void Update() {
		if (transitionState == TransitionState.IDLE) return;

		transitionTime += GameTime.GetDeltaTime(TIME_CHANNEL) * (transitionState == TransitionState.IN ? -1 : 1);

		if ((transitionState == TransitionState.IN && PercentThroughTransition <= 0) 
			|| (transitionState == TransitionState.OUT && PercentThroughTransition >= 1)) {
			transitionState = TransitionState.IDLE;
		}

		// Update UI
		transitionUI.alpha = presenceOverTime.Evaluate(transitionTime);
		transitionUI.blocksRaycasts = presenceOverTime.Evaluate(transitionTime) > 0;
	}

	private void PlayTransitionOut(float delay) {
		transitionTime = -delay;
		ServiceLocator.SceneManager.ExtendTransition(delay + TransitionDuration);
		transitionState = TransitionState.OUT;
	}

}
