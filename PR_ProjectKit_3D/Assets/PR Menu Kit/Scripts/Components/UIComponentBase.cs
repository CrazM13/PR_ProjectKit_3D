using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIComponentBase : MonoBehaviour {

	protected UIElement element;

	[Header("UI Component Settings")]
	[SerializeField] protected ActivationMode activationMode;

	#region Events
	public UnityEvent OnEffectStart { get; set; } = new UnityEvent();
	public UnityEvent OnEffectContinue { get; set; } = new UnityEvent();
	public UnityEvent OnEffectEnd { get; set; } = new UnityEvent();
	#endregion

	public bool IsPlaying { get; private set; } = false;

	private void Awake() {
		element = GetComponent<UIElement>();
	}

	private void Update() {
		bool shouldActivate = ShouldActivate();
		if (shouldActivate) {
			if (!IsPlaying) {
				OnEffectStart.Invoke();
				IsPlaying = true;
			} else {
				OnEffectContinue.Invoke();
			}
		} else if (IsPlaying) {
			OnEffectEnd.Invoke();
			IsPlaying = false;
		}
	}

	protected bool ShouldActivate() {
		return activationMode switch {
			ActivationMode.ALWAYS => true,
			ActivationMode.ON_HOVER => element.IsHovering,
			_ => false,
		};
	}

}
