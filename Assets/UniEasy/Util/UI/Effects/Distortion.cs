using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

namespace UniEasy
{
	[AddComponentMenu ("UI/Effects/Distortion")]
	public class Distortion : BaseMeshEffect
	{
		public AnimationCurve top = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
		public AnimationCurve bottom = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 0));
		public float scale = 1f;

		public override void ModifyMesh (VertexHelper vh)
		{
			if (!IsActive () || top == null || top.length < 2 || bottom == null || bottom.length < 2) {
				return;
			}

			var count = vh.currentVertCount;
			if (count == 0)
				return;

			var vertexs = new List<UIVertex> ();
			for (int i = 0; i < count; i++) {
				var vertex = new UIVertex ();
				vh.PopulateUIVertex (ref vertex, i);
				vertexs.Add (vertex);
			}

			var topLastTime = top.keys.Last ().time;
			var bottomLastTime = bottom.keys.Last ().time;
			var length = (int)(0.25f * vertexs.Count);
			var average = 0.5f / length;
			for (int i = 0; i < length; i++) {
				var time = (2 * i + 1) * average;
				for (int j = 0; j < 4; j++) {
					var vertex = vertexs [4 * i + j];
					var topEvaluate = topLastTime >= time ? top.Evaluate (time) : bottom.Evaluate (time);
					var bottomEvaluate = bottomLastTime >= time ? bottom.Evaluate (time) : top.Evaluate (time);
					vertex.position += scale * (j > 1 ? bottomEvaluate : topEvaluate) * transform.up;    
					vh.SetUIVertex (vertex, 4 * i + j);
				}
			}
		}
	}
}
