using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TMPro.TMP_Text))]
public class UIBouncingCharacters : UIComponentBase {

	private TMPro.TMP_Text textMesh;

	private string cachedString = string.Empty;
	private Mesh mesh;
	private Vector3[] verticies;

	[Header("Bounce")]
	[SerializeField] private AnimationCurve bounceOverTime;
	[SerializeField] private float strength = 1;
	[SerializeField] private float speed = 1;
	[SerializeField] private float characterInfluence = 0.2f;

	void Start() {
		textMesh = GetComponent<TMPro.TMP_Text>();

		OnEffectStart.AddListener(InitializeTextMesh);
		OnEffectContinue.AddListener(UpdateTextMesh);
		OnEffectEnd.AddListener(EndEffect);
	}

	private void EndEffect() {
		InitializeTextMesh();
	}

	private void InitializeTextMesh() {
		textMesh.ForceMeshUpdate();
		mesh = textMesh.mesh;
		verticies = mesh.vertices;

		cachedString = textMesh.text;
	}

	private void UpdateTextMesh() {
		if (cachedString.Length != textMesh.textInfo.characterCount || cachedString != textMesh.text) {
			// If the text changed, reinitialize
			InitializeTextMesh();
		}

		Vector3[] newVerticies = new Vector3[verticies.Length];
		verticies.CopyTo(newVerticies, 0);

		for (int i = 0; i < textMesh.textInfo.characterCount; i++) {
			int vertexIndex = textMesh.textInfo.characterInfo[i].vertexIndex;

			Vector3 offset = Vector3.up * GetCharacterYOffset(i);
			newVerticies[vertexIndex] += offset;
			newVerticies[vertexIndex + 1] += offset;
			newVerticies[vertexIndex + 2] += offset;
			newVerticies[vertexIndex + 3] += offset;
		}

		mesh.vertices = newVerticies;
		textMesh.canvasRenderer.SetMesh(mesh);
	}

	private float GetCharacterYOffset(int characterIndex) {
		return bounceOverTime.Evaluate((element.TimeAlive + (characterIndex * characterInfluence)) * speed) * strength;
	}

}
