using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanorama : MonoBehaviour {

	[Header("Components")]
	[SerializeField] private new Transform camera;

	[Header("Settings")]
	[SerializeField] private float speed;
	[SerializeField] private float distance;
	[SerializeField] private AnimationCurve angleOverTime;

	private float timer;

	void Start() {

	}

	void Update() {
		timer += Time.deltaTime * speed;
		timer %= 1;

		float newAngle = angleOverTime.Evaluate(timer) * 360f;
		camera.localPosition = Quaternion.AngleAxis(newAngle, Vector3.up) * (Vector3.forward * distance);
		camera.localRotation = Quaternion.AngleAxis(newAngle, Vector3.up);
	}

	private void OnDrawGizmos() {
		float maxLines = 25f;
		for (int i = 1; i < maxLines; i++) {
			float oldAngle = angleOverTime.Evaluate((i - 1) / maxLines) * 360f;
			float newAngle = angleOverTime.Evaluate(i / maxLines) * 360f;

			Vector3 oldPoint = Quaternion.AngleAxis(oldAngle, Vector3.up) * (Vector3.forward * distance);
			Vector3 newPoint = Quaternion.AngleAxis(newAngle, Vector3.up) * (Vector3.forward * distance);

			Gizmos.DrawLine(transform.TransformPoint(oldPoint), transform.TransformPoint(newPoint));

		}
	}

}
