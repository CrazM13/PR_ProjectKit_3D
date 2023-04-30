using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPulse : UIComponentBase {

	[Header("Scale")]
	[SerializeField] private AnimationCurve scaleOverTime = AnimationCurve.Constant(0, 1, 1);
	[SerializeField] private float scaleStrength = 1;
	[SerializeField] private float scaleSpeed = 1;

	private Vector3 realScale;

	void Start() {
		OnEffectStart.AddListener(StoreScale);
		OnEffectContinue.AddListener(UpdatePulse);
		OnEffectEnd.AddListener(ResetScale);
	}

	private void StoreScale() {
		realScale = transform.localScale;
	}

	private void UpdatePulse() {
		float scaleMultiplier = scaleOverTime.Evaluate(element.TimeAlive * scaleSpeed) * scaleStrength;

		transform.localScale = realScale * scaleMultiplier;
	}

	private void ResetScale() {
		transform.localScale = realScale;
	}

}
