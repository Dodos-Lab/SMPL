namespace SMPL.Prefabs
{
	/// <summary>
	/// Original concept by Sebastian Lague (github.com/SebLague)
	/// </summary>
	public class Rope
	{
		public class Point
		{
			public Vector2 Position { get; set; }
			public bool IsLocked { get; set; }

			public Point(Vector2 position, bool isLocked = false)
			{
				this.Position = position;
				this.IsLocked = isLocked;
				prevPosition = position;
			}

			public override string ToString()
			{
				var locked = IsLocked ? " (locked)" : "";
				return $"{Position}{locked}";
			}

			internal Vector2 prevPosition;
		}

		public List<Point> Points { get; } = new();
		public Vector2 Position { get; set; }
		public Vector2 Gravity { get; set; } = new Vector2(0, 1);
		public Vector2 Force { get; set; }

		public Rope(Vector2 position, params Point[] points)
		{
			Position = position;

			if(points != null && points.Length > 0)
			{
				this.Points = points.ToList();

				var start = new Point(position, true);
				segments[(new(start, points[0]))] = new(start, points[0]);
				for(int i = 1; i < points.Length; i++)
					segments[(points[i - 1], points[i])] = new(points[i - 1], points[i]);
			}
		}
		public void Draw(RenderTarget renderTarget = default, Color color = default, float width = 4f)
		{
			var lines = new List<Line>();

			foreach(var kvp in segments)
				lines.Add(new Line(kvp.Value.a.Position, kvp.Value.b.Position));

			lines.Draw(renderTarget, color, width);
		}

		public void Tie(Point pointA, Point pointB, bool mergePoints = false)
		{
			if(mergePoints)
			{
				pointA.Position = pointB.Position;
				pointA.prevPosition = pointB.prevPosition;
			}

			segments[(pointA, pointB)] = new(pointA, pointB);
		}
		public void Untie(Point pointA, Point pointB, bool removeTie = false)
		{
			segments.Remove((pointA, pointB));
			segments.Remove((pointB, pointA));

			if(removeTie == false)
			{
				var newB = new Point(pointB.Position, pointB.IsLocked);
				Points.Add(newB);
				segments[(pointA, newB)] = new(pointA, newB);
			}
		}

		public void Update()
		{
			foreach(var kvp in segments)
			{
				kvp.Value.a.Position = Position;
				break;
			}

			for(int i = 0; i < Points.Count; i++)
			{
				var p = Points[i];

				if(p.IsLocked)
					continue;

				var prev = p.Position;
				p.Position += p.Position - p.prevPosition;
				p.Position += Gravity * Time.Delta * 10;
				p.Position += Force * Time.Delta * 10;
				p.prevPosition = prev;
			}

			foreach(var kvp in segments)
			{
				var s = kvp.Value;
				for(int j = 0; j < 5; j++)
				{
					var a = s.a.Position;
					var b = s.b.Position;
					var center = (a + b) / 2;
					var dir = Vector2.Normalize(a - b);
					var length = (a - b).Length();

					if(length <= s.length)
						continue;

					if(s.a.IsLocked == false)
						s.a.Position = center + dir * s.length / 2;
					if(s.b.IsLocked == false)
						s.b.Position = center - dir * s.length / 2;
				}
			}
		}

		#region Backend
		private class Segment
		{
			public Point a, b;
			public float length;

			public Segment(Point a, Point b)
			{
				this.a = a;
				this.b = b;
				length = Vector2.Distance(a.Position, b.Position);
			}
		}

		private readonly Dictionary<(Point, Point), Segment> segments = new();
		private List<(Point, Point)> GetPairsContainingPoint(Point p)
		{
			var pairs = new List<(Point, Point)>();
			foreach(var kvp in segments)
				if(kvp.Key.Item1 == p || kvp.Key.Item2 == p)
					pairs.Add(kvp.Key);
			return pairs;
		}
		#endregion
	}
}
