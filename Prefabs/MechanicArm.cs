namespace SMPL.Prefabs
{
	/// <summary>
	/// Original concept by Sebastian Lague (github.com/SebLague)
	/// </summary>
	public class MechanicArm
	{
		public ReadOnlyCollection<Vector2> Points => points.AsReadOnly();
		public Vector2 Position
		{
			get => position;
			private set { position = value; Update(); }
		}
		public Vector2 TargetPosition
		{
			get => targetPosition;
			set { targetPosition = value; Update(); }
		}

		public MechanicArm(Vector2 originPosition, params float[] segmentLengths)
		{
			lengths = (float[])segmentLengths.Clone();
			points.Add(originPosition);
			for(int i = 1; i < segmentLengths.Length; i++)
				points.Add(points[i - 1] + new Vector2(segmentLengths[i - 1], 0));

			Position = originPosition;

			Update();
		}

		public void Draw(RenderTarget renderTarget = default, Color color = default, float width = 4f)
		{
			for(int i = 0; i < points.Count - 1; i++)
				new Line(points[i], points[i + 1]).Draw(renderTarget, color, width);
		}

		#region Backend
		private readonly List<Vector2> points = new();
		private readonly float[] lengths;
		private Vector2 position, targetPosition;

		private void Update()
		{
			points[0] = position;
			var originPoint = points[0];

			for(int i = 0; i < 16; i++)
			{
				var startingFromTarget = i % 2 == 0;
				points.Reverse();
				Array.Reverse(lengths);
				points[0] = startingFromTarget ? targetPosition : originPoint;

				for(int j = 1; j < points.Count; j++)
				{
					if(points[j] == points[j - 1])
						points[j] += new Vector2(0.01f, -0.02f);

					var dir = (points[j] - points[j - 1]).NormalizeDirection();
					points[j] = points[j - 1] + dir * lengths[j - 1];
				}
				var dstToTarget = (points[^1] - targetPosition).Length();
				if(startingFromTarget == false && dstToTarget <= 0.01) return;
			}
		}
		#endregion
	}
}
