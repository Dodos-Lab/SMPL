namespace SMPL.Tools
{
	/// <summary>
	/// A curve that can be traversed over, formed by points.<br></br><br></br>
	/// Original concept by javidx9 (OneLoneCoder.com).
	/// </summary>
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
		public bool IsLooping { get; set; }

		public Curve(bool isLooped, params Vector2[] points)
		{
			IsLooping = isLooped;

			if(points == null || points.Length == 0)
				return;

			Points = points.ToList();
		}

		public Vector2 GetPoint(float index)
		{
			if(Points.Count == 0)
				return new Vector2().NaN();

			int p0, p1, p2, p3;
			if(IsLooping)
			{
				index = index.Limit(0, Points.Count, Extensions.Limitation.Overflow);

				p1 = (int)index;
				p2 = (p1 + 1) % Points.Count;
				p3 = (p2 + 1) % Points.Count;
				p0 = p1 >= 1 ? p1 - 1 : Points.Count - 1;
			}
			else
			{
				if(TryNonLoopError())
					return new Vector2().NaN();

				index = index.Limit(0, Points.Count);
				index = index.Map(0, Points.Count, 0, Points.Count - 3);

				p1 = (int)index + 1;
				p2 = p1 + 1;
				p3 = p2 + 1;
				p0 = p1 - 1;
			}

			index -= (int)index;

			var tt = index * index;
			var ttt = tt * index;

			var q1 = -ttt + 2.0f * tt - index;
			var q2 = 3.0f * ttt - 5.0f * tt + 2.0f;
			var q3 = -3.0f * ttt + 4.0f * tt + index;
			var q4 = ttt - tt;

			if(p3 == Points.Count)
				return Points[^2];

			var tx = 0.5f * (Points[p0].X * q1 + Points[p1].X * q2 + Points[p2].X * q3 + Points[p3].X * q4);
			var ty = 0.5f * (Points[p0].Y * q1 + Points[p1].Y * q2 + Points[p2].Y * q3 + Points[p3].Y * q4);

			return new(tx, ty);
		}
		public Vector2 GetDirection(float index)
		{
			if(Points.Count == 0)
				return new Vector2().NaN();

			int p0, p1, p2, p3;
			if(IsLooping)
			{
				index = index.Limit(0, Points.Count, Extensions.Limitation.Overflow);

				p1 = (int)index;
				p2 = (p1 + 1) % Points.Count;
				p3 = (p2 + 1) % Points.Count;
				p0 = p1 >= 1 ? p1 - 1 : Points.Count - 1;
			}
			else
			{
				if(TryNonLoopError())
					return new Vector2().NaN();

				index = index.Limit(0, Points.Count);
				index = index.Map(0, Points.Count, 0, Points.Count - 3);

				p1 = (int)index + 1;
				p2 = p1 + 1;
				p3 = p2 + 1;
				p0 = p1 - 1;
			}

			index -= (int)index;

			var tt = index * index;

			var q1 = -3.0f * tt + 4.0f * index - 1;
			var q2 = 9.0f * tt - 10.0f * index;
			var q3 = -9.0f * tt + 8.0f * index + 1.0f;
			var q4 = 3.0f * tt - 2.0f * index;

			if(p3 == Points.Count)
				return Points[^2];

			var tx = 0.5f * (Points[p0].X * q1 + Points[p1].X * q2 + Points[p2].X * q3 + Points[p3].X * q4);
			var ty = 0.5f * (Points[p0].Y * q1 + Points[p1].Y * q2 + Points[p2].Y * q3 + Points[p3].Y * q4);

			return new Vector2(tx, ty).NormalizeDirection();
		}
		public float GetAngle(float index) => GetDirection(index).DirectionToAngle();
		public float GetSegmentLength(float index)
		{
			var length = 0.0f;
			var stepSize = 0.005f;

			index = (int)index;

			Vector2 oldPoint, newPoint;
			oldPoint = GetPoint(index);

			for(float t = 0; t < 1.0f; t += stepSize)
			{
				newPoint = GetPoint(index + t);
				length += oldPoint.DistanceBetweenPoints(newPoint);
				oldPoint = newPoint;
			}

			return length;
		}
		public float GetIndex(float distanceFromStart)
		{
			var lengths = new float[Points.Count];
			for(int j = 0; j < lengths.Length; j++)
				lengths[j] = GetSegmentLength(j);

			var i = 0;
			while(distanceFromStart > lengths[i])
			{
				distanceFromStart -= lengths[i];

				if(i >= lengths.Length - 1)
					return Points.Count;
				i++;
			}

			return i + (distanceFromStart / lengths[i]);
		}

		/// <summary>
		/// Draws the curve to a <paramref name="renderTarget"/> with <paramref name="color"/> having some
		/// <paramref name="width"/>. The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if no
		/// <paramref name="camera"/> is passed. The default <paramref name="color"/> is assumed to be white if no
		/// <paramref name="color"/> is passed.
		/// </summary>
		public void Draw(RenderTarget renderTarget = default, float detail = 10, Color color = default, float width = 4f)
		{
			if(IsLooping == false && Points.Count < 4)
				return;

			renderTarget ??= Scene.MainCamera.RenderTexture;
			color = color == default ? Color.White : color;
			width /= 2;
			var lines = 1f / (Points.Count - (IsLooping ? 0 : 3));

			for(float i = 0f; i <= Points.Count + lines; i += lines)
			{
				if(i == 0 && IsLooping == false)
					continue; // don't draw loop ('0 - step' loops back to the last index)

				var p1 = GetPoint(i);
				var p2 = GetPoint(i - lines);
				var line = new Line(p1, p2);
				line.Draw(renderTarget, color, width);
			}
		}
		/// <summary>
		/// Draws the points that are making the curve to a <paramref name="renderTarget"/> with <paramref name="color"/> having some
		/// <paramref name="width"/>. The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if no
		/// <paramref name="camera"/> is passed. The default <paramref name="color"/> is assumed to be white if no
		/// <paramref name="color"/> is passed.
		/// </summary>
		public void DrawPoints(RenderTarget renderTarget = default, Color color = default, float width = 4f)
		{
			for(int i = 0; i < Points.Count; i++)
				Points[i].DrawPoint(renderTarget, color, width);
		}

		#region Backend
		private bool TryNonLoopError()
		{
			if(Points.Count > 3 || IsLooping)
				return false;

			Console.LogError(2, $"A non-looping {nameof(Curve)} expects 4 or more {nameof(Points)}.");
			return true;
		}
		#endregion
	}
}