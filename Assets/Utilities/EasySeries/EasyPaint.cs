using UnityEngine;

public class EasyPaint
{
	private Texture2D canvas;
	private Texture2D brush;
	private Color color;
	private Color[] field;
	private Color[] brushwork;

	public Texture2D Canvas {
		set {
			canvas = value;
		}
		get {
			return canvas;
		}
	}

	public Color Color {
		set {
			color = value;
		}
		get {
			return color;
		}
	}

	public Texture2D Brush {
		set {
			brush = value;
			brushwork = brush.GetPixels (0, 0, brush.width, brush.height);
		}
		get {
			return brush;
		}
	}

	public EasyPaint (Texture2D canvas, Texture2D brush, Color color)
	{
		Canvas = canvas;
		Brush = brush;
		Color = color;
	}

	public void Draw (Vector2 start, Vector2 end)
	{
		float x = Mathf.Clamp (start.x, 0f, 1f);
		float y = Mathf.Clamp (start.y, 0f, 1f);
		x = Mathf.RoundToInt (x * canvas.width - 0.5f * brush.width);
		y = Mathf.RoundToInt (y * canvas.height - 0.5f * brush.height);
		start = new Vector2 (x, y);
		x = Mathf.Clamp (end.x, 0f, 1f);
		y = Mathf.Clamp (end.y, 0f, 1f);
		x = Mathf.RoundToInt (x * canvas.width - 0.5f * brush.width);
		y = Mathf.RoundToInt (y * canvas.height - 0.5f * brush.height);
		end = new Vector2 (x, y);

		Vector2 distance = end - start;
		int length = Mathf.RoundToInt (Vector2.Distance (start, end));
		if (length > 0) {
			for (int i = 0; i < length; i++) {
				Vector2 point = start + i / length * distance;
				SetPixels (Mathf.RoundToInt (point.x), Mathf.RoundToInt (point.y));
			}
		} else {
			SetPixels (Mathf.RoundToInt (end.x), Mathf.RoundToInt (end.y));
		}
	}

	public void Refresh ()
	{
		canvas.Apply ();
	}

	private void SetPixels (int x, int y)
	{
		x = Mathf.Clamp (x, 0, canvas.width - brush.width);
		y = Mathf.Clamp (y, 0, canvas.height - brush.height);

		field = canvas.GetPixels (x, y, brush.width, brush.height, 0);
		for (int i = 0; i < brushwork.Length; i++) {
			if (brushwork [i].a != 0) {
				field [i] = Color.Lerp (field [i], color, 0.33f * brushwork [i].a);
			}
		}
		canvas.SetPixels (x, y, brush.width, brush.height, field, 0);
	}
}
