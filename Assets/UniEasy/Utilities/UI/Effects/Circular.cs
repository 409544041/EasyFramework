﻿using UnityEngine.UI;
using UnityEngine;

namespace UniEasy
{
	[AddComponentMenu ("UI/Effects/Circular")]
	public class Circular : BaseMeshEffect
	{
		public float thickness = 24;
		[Range (4, 128)]
		public int segments = 24;
		[MinMaxRange (0, 1)]
		public RangedFloat fillAmount = new RangedFloat (0, 1);

		public override void ModifyMesh (VertexHelper vh)
		{
			if (IsActive ()) {
				var count = vh.currentVertCount;
				if (count == 0) {
					return;
				}
				var degreeDelta = (float)(2 * Mathf.PI / segments);
				var curVertice = Vector2.zero;
				var curDegree = 0f;
				var height = graphic.rectTransform.rect.height;
				var width = graphic.rectTransform.rect.width;
				var uv = new Vector4 (0, 0, 1, 1);
				var uvCenterX = (uv.x + uv.z) * 0.5f;
				var uvCenterY = (uv.y + uv.w) * 0.5f;
				var uvScaleX = (uv.z - uv.x) / width;
				var uvScaleY = (uv.w - uv.y) / height;
				var outerRadius = new Vector2 (graphic.rectTransform.pivot.x * width, graphic.rectTransform.pivot.y * height);
				var innerRadius = new Vector2 (Mathf.Clamp (outerRadius.x - thickness, 0, float.MaxValue), Mathf.Clamp (outerRadius.y - thickness, 0, float.MaxValue));
			
				if (innerRadius == Vector2.zero) {
					var verticeCount = segments + 1;
					var triangleCount = 3 * segments;
					var uiVertex = new UIVertex ();
					uiVertex.color = graphic.color;
					uiVertex.position = curVertice;
					uiVertex.uv0 = new Vector2 (curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
					vh.Clear ();
					vh.AddVert (uiVertex);

					for (int i = 1; i < verticeCount; i++) {
						var cosA = Mathf.Cos (curDegree);
						var sinA = Mathf.Sin (curDegree);
						curVertice = new Vector2 (cosA * outerRadius.x, sinA * outerRadius.y);
						
						uiVertex = new UIVertex ();
						uiVertex.color = graphic.color;
						uiVertex.position = curVertice;
						uiVertex.uv0 = new Vector2 (curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
						vh.AddVert (uiVertex);
						
						curDegree += degreeDelta;
					}

					var min = Mathf.FloorToInt (fillAmount.min * verticeCount);
					var max = Mathf.FloorToInt (fillAmount.max * verticeCount);
					for (int i = 0, j = 1; i < triangleCount - 3; i += 3, j++) {
						if (min <= j && max >= j + 1) {
							vh.AddTriangle (j, 0, j + 1);
						}
					}
					if (fillAmount.max >= 1) {
						vh.AddTriangle (verticeCount - 1, 0, 1);
					}
				} else {
					var verticeCount = 2 * segments;
					var triangleCount = 3 * 2 * segments;
					vh.Clear ();
					for (int i = 0; i < verticeCount; i += 2) {
						var cosA = Mathf.Cos (curDegree);
						var sinA = Mathf.Sin (curDegree);
						curVertice = new Vector3 (cosA * innerRadius.x, sinA * innerRadius.y);

						var uiVertex = new UIVertex ();
						uiVertex.color = graphic.color;
						uiVertex.position = curVertice;
						uiVertex.uv0 = new Vector2 (curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
						vh.AddVert (uiVertex);

						curVertice = new Vector3 (cosA * outerRadius.x, sinA * outerRadius.y);
						uiVertex = new UIVertex ();
						uiVertex.color = graphic.color;
						uiVertex.position = curVertice;
						uiVertex.uv0 = new Vector2 (curVertice.x * uvScaleX + uvCenterX, curVertice.y * uvScaleY + uvCenterY);
						vh.AddVert (uiVertex);

						curDegree += degreeDelta;
					}

					var min = Mathf.FloorToInt (fillAmount.min * verticeCount);
					var max = Mathf.FloorToInt (fillAmount.max * verticeCount);
					for (int i = 0, j = 0; i < triangleCount - 6; i += 6, j += 2) {
						if (min <= j && max >= j + 3) {
							vh.AddTriangle (j + 1, j, j + 3);
							vh.AddTriangle (j, j + 2, j + 3);
						}
					}
					if (fillAmount.max >= 1) {
						vh.AddTriangle (verticeCount - 1, verticeCount - 2, 1);
						vh.AddTriangle (verticeCount - 2, 0, 1);
					}
				}
			}
		}
	}
}
