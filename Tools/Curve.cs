namespace SMPL.Tools
{
	public class Curve
	{
		public List<Vector2> Points { get; } = new();
		public float TotalLength
		{
			get
			{
				var result = 0f;
				for(int i = 0; i < Points.Count; i++)
					result += GetSegmentLength(i);
				return result;
			}
		}
		public bool IsLooped { get; set; }

		public Curve(bool isLooped, params Vector2[] points)
		{
			IsLooped = isLooped;

			if(points == null || points.Length == 0)
				return;

			Points = points.ToList();
		}

		public Vector2 GetPoint(float t)
		{
			if(Points.Count == 0)
				return new Vector2().NaN();

			int p0, p1, p2, p3;
			if(IsLooped)
			{
				p1 = (int)t;
				p2 = (p1 + 1) % Points.Count;
				p3 = (p2 + 1) % Points.Count;
				p0 = p1 >= 1 ? p1 - 1 : Points.Count - 1;
			}
			else
			{
				if(TryNonLoopError())
					return new Vector2().NaN();

				t = t.Limit(0, 1, Extensions.Limitation.Overflow);

				p1 = (int)t + 1;
				p2 = p1 + 1;
				p3 = p2 + 1;
				p0 = p1 - 1;
			}

			t -= (int)t;

			var tt = t * t;
			var ttt = tt * t;

			var q1 = -ttt + 2.0f * tt - t;
			var q2 = 3.0f * ttt - 5.0f * tt + 2.0f;
			var q3 = -3.0f * ttt + 4.0f * tt + t;
			var q4 = ttt - tt;

			var tx = 0.5f * (Points[p0].X * q1 + Points[p1].X * q2 + Points[p2].X * q3 + Points[p3].X * q4);
			var ty = 0.5f * (Points[p0].Y * q1 + Points[p1].Y * q2 + Points[p2].Y * q3 + Points[p3].Y * q4);

			return new(tx, ty);
		}
		public Vector2 GetGradient(float t)
		{
			if(Points.Count == 0)
				return new Vector2().NaN();

			int p0, p1, p2, p3;
			if(IsLooped)
			{
				p1 = (int)t;
				p2 = (p1 + 1) % Points.Count;
				p3 = (p2 + 1) % Points.Count;
				p0 = p1 >= 1 ? p1 - 1 : Points.Count - 1;
			}
			else
			{
				if(TryNonLoopError())
					return new Vector2().NaN();

				t = t.Limit(0, 1, Extensions.Limitation.Overflow);

				p1 = (int)t + 1;
				p2 = p1 + 1;
				p3 = p2 + 1;
				p0 = p1 - 1;
			}

			t -= (int)t;

			var tt = t * t;

			var q1 = -3.0f * tt + 4.0f * t - 1;
			var q2 = 9.0f * tt - 10.0f * t;
			var q3 = -9.0f * tt + 8.0f * t + 1.0f;
			var q4 = 3.0f * tt - 2.0f * t;

			var tx = 0.5f * (Points[p0].X * q1 + Points[p1].X * q2 + Points[p2].X * q3 + Points[p3].X * q4);
			var ty = 0.5f * (Points[p0].Y * q1 + Points[p1].Y * q2 + Points[p2].Y * q3 + Points[p3].Y * q4);

			return new(tx, ty);
		}
		public float GetSegmentLength(int node)
		{
			var length = 0.0f;
			var stepSize = 0.005f;

			Vector2 oldPoint, newPoint;
			oldPoint = GetPoint(node);

			for(float t = 0; t < 1.0f; t += stepSize)
			{
				newPoint = GetPoint(node + t);
				length += MathF.Sqrt((newPoint.X - oldPoint.X) * (newPoint.X - oldPoint.X) +
					(newPoint.Y - oldPoint.Y) * (newPoint.Y - oldPoint.Y));
				oldPoint = newPoint;
			}

			return length;
		}
		public float GetNormalisedOffset(float p)
		{
			// Which node is the base?
			var i = 0;
			while(p > Points[i].Length())
			{
				p -= Points[i].Length();
				i++;
			}

			// The fractional is the offset 
			return i + (p / Points[i].Length());
		}

		/// <summary>
		/// Draws the curve to a <paramref name="renderTarget"/> with <paramref name="color"/> having some
		/// <paramref name="size"/>. The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if no
		/// <paramref name="camera"/> is passed. The default <paramref name="color"/> is assumed to be white if no
		/// <paramref name="color"/> is passed.
		/// </summary>
		public void Draw(RenderTarget renderTarget = default, float step = 0.1f, Color color = default, float size = 4)
		{
			renderTarget ??= Scene.MainCamera.RenderTexture;
			color = color == default ? Color.White : color;

			size /= 2;
		}

		private bool TryNonLoopError()
		{
			if(Points.Count == 4 || IsLooped)
				return false;

			Console.LogError(2, $"A non-looping {nameof(Curve)} expects exactly 4 {nameof(Points)}.");
			return true;
		}
	}
}