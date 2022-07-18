namespace SMPL.Prefabs
{
	/// <summary>
	/// Original concept by Sebastian Lague (github.com/SebLague)
	/// </summary>
	public class MechanicArm
	{
		public ReadOnlyCollection<Vector2> Points => points.AsReadOnly();
		public ReadOnlyCollection<Line> Lines
		{
			get
			{
				var lines = new List<Line>();
				for(int i = 0; i < points.Count - 1; i++)
					lines.Add(new Line(points[i], points[i + 1]));
				return lines.AsReadOnly();
			}
		}
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

		public MechanicArm(Vector2 originPosition, int segmentCount = 5, float segmentLength = 50f)
		{
			points.Add(originPosition);
			length = segmentLength;
			for(int i = 1; i < segmentCount + 1; i++)
				points.Add(points[i - 1] + new Vector2(segmentLength, 0));

			Position = originPosition;

			Update();
		}

		public void Draw(RenderTarget renderTarget = default, Color color = default, float width = 4f)
		{
			Lines.Draw(renderTarget, color, width);
		}

		#region Backend
		private readonly List<Vector2> points = new();
		private readonly float length;
		private Vector2 position, targetPosition;

		private void Update()
		{
			points[0] = position;
			var originPoint = points[0];

			for(int i = 0; i < 16; i++)
			{
				var startingFromTarget = i % 2 == 0;
				points.Reverse();
				points[0] = startingFromTarget ? targetPosition : originPoint;

				for(int j = 1; j < points.Count; j++)
				{
					if(points[j] == points[j - 1])
						points[j] += new Vector2(0.01f, -0.02f);

					var dir = (points[j] - points[j - 1]).NormalizeDirection();
					points[j] = points[j - 1] + dir * length;
				}
				var dstToTarget = (points[^1] - targetPosition).Length();
				if(startingFromTarget == false && dstToTarget <= 0.01)
					return;
			}
		}
		#endregion
	}
}
