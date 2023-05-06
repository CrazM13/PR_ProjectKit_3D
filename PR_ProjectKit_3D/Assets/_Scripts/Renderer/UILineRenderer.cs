using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic {

	[Header("Line")]
	[SerializeField] private Space coordinateSpace;
	[SerializeField] private List<Vector2> points;
	[SerializeField] private float thickness = 1;

	[Header("Caps")]
	[SerializeField] private float capThickness;

	protected override void OnPopulateMesh(VertexHelper vh) {
		base.OnPopulateMesh(vh);

		vh.Clear();

		if (points.Count < 2) {
			return;
		} else {
			DrawLines(vh);
			DrawCaps(vh);
		}

	}

	private void DrawLines(VertexHelper vh) {
		float angle = 0;
		for (int i = 0; i < points.Count; i++) {
			if (i < points.Count - 1) angle = GetAngle(points[i], points[i + 1]) + 45f;
			DrawLineVerts(i, angle, vh);
		}

		for (int i = 0; i < points.Count - 1; i++) {
			int index = i * 2;
			vh.AddTriangle(index + 0, index + 1, index + 3);
			vh.AddTriangle(index + 3, index + 2, index + 0);
		}

	}

	private void DrawCaps(VertexHelper vh) {
		for (int i = 0; i < points.Count; i++) {
			DrawCapVerts(points[i], vh);
		}


		int lastLineIndex = (points.Count) * 2;
		for (int i = 0; i < points.Count; i++) {
			int index = lastLineIndex + (i * 4);
			vh.AddTriangle(index + 0, index + 1, index + 2);
			vh.AddTriangle(index + 2, index + 3, index + 0);
		}
	}

	private void DrawLineVerts(int index, float angle, VertexHelper vh) {
		Vector3 point = points[index];

		UIVertex vertex = UIVertex.simpleVert;

		vertex.color = color;

		// From Angle
		vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2f, 0);
		vertex.position += point;
		if (coordinateSpace == Space.World) vertex.position = transform.InverseTransformPoint(vertex.position);
		
		vh.AddVert(vertex);

		vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2f, 0);
		vertex.position += point;
		if (coordinateSpace == Space.World) vertex.position = transform.InverseTransformPoint(vertex.position);

		vh.AddVert(vertex);
	}

	private float GetAngle(Vector2 lh, Vector2 rh) {
		return (float) (Mathf.Atan2(rh.y - lh.y, rh.x - lh.x) * (180 / Mathf.PI));
	}

	private void DrawCapVerts(Vector2 point, VertexHelper vh) {
		UIVertex vertex = UIVertex.simpleVert;

		vertex.color = color;

		vertex.position = new Vector3(-capThickness / 2f, -capThickness / 2f);
		vertex.position += (Vector3) point;
		if (coordinateSpace == Space.World) vertex.position = transform.InverseTransformPoint(vertex.position);

		vh.AddVert(vertex);

		vertex.position = new Vector3(-capThickness / 2f, capThickness / 2f);
		vertex.position += (Vector3) point;
		if (coordinateSpace == Space.World) vertex.position = transform.InverseTransformPoint(vertex.position);

		vh.AddVert(vertex);

		vertex.position = new Vector3(capThickness / 2f, capThickness / 2f);
		vertex.position += (Vector3) point;
		if (coordinateSpace == Space.World) vertex.position = transform.InverseTransformPoint(vertex.position);

		vh.AddVert(vertex);

		vertex.position = new Vector3(capThickness / 2f, -capThickness / 2f);
		vertex.position += (Vector3) point;
		if (coordinateSpace == Space.World) vertex.position = transform.InverseTransformPoint(vertex.position);

		vh.AddVert(vertex);

	}

}
