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
			public bool IsPinned { get; set; }

			public Point(Vector2 position, bool isPinned = false)
			{
				Position = position;
				IsPinned = isPinned;
				prevPosition = position;
			}

			public override string ToString()
			{
				var locked = IsPinned ? " (locked)" : "";
				return $"{Position}{locked}";
			}

			internal Vector2 prevPosition;
			internal bool skip;
		}

		public List<Point> Points { get; } = new();
		public ReadOnlyCollection<Line> Lines
		{
			get
			{
				var lines = new List<Line>();

				foreach(var kvp in segments)
					lines.Add(new Line(kvp.Value.a.Position, kvp.Value.b.Position));
				return lines.AsReadOnly();
			}
		}
		public Vector2 Position { get; set; }
		public Vector2 Gravity { get; set; } = new Vector2(0, 1);
		public Vector2 Force { get; set; }

		public Rope(Vector2 position, int segmentCount, float segmentLength)
		{
			Position = position;
			segmentCount = Math.Max(1, segmentCount);
			segmentLength = MathF.Max(1, segmentLength);

			var start = new Point(position, true);
			Points.Add(start);

			for(int i = 1; i < segmentCount + 1; i++)
				Points.Add(new Point(position + new Vector2(0, i * segmentLength)));

			for(int i = 1; i < segmentCount + 1; i++)
			{
				var p1 = Points[i - 1];
				var p2 = Points[i];
				segments[(p1, p2)] = new(p1, p2);
			}
		}
		public void Draw(RenderTarget renderTarget = default, Color color = default, float width = 4f)
		{
			Lines.Draw(renderTarget, color, width);
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

			pointA.skip = true;
			pointB.skip = true;

			if(removeTie == false)
			{
				var newB = new Point(pointB.Position, pointB.IsPinned);
				Points.Add(newB);
				segments[(pointA, newB)] = new(pointA, newB);
			}
		}

		public void Update()
		{
			if(Points.Count > 0)
				Points[0].Position = Position;

			for(int i = 0; i < Points.Count; i++)
			{
				var p = Points[i];

				if(p.IsPinned)
					continue;

				var prev = p.Position;
				p.Position += p.Position - p.prevPosition;
				p.Position += Gravity * Time.Delta * 10;
				p.Position += Force * Time.Delta * 10;
				p.prevPosition = prev;
			}

			//var untie = (default(Point), default(Point));
			foreach(var kvp in segments)
			{
				var s = kvp.Value;
				var a = s.a.Position;
				var b = s.b.Position;
				//if(a.DistanceBetweenPoints(b) > s.length * 3 && untie.Item1 == null && untie.Item2 == null)
				//	untie = (s.a, s.b);

				for(int j = 0; j < 5; j++)
				{
					var center = (a + b) / 2;
					var dir = Vector2.Normalize(a - b);
					var length = (a - b).Length();

					if(length <= s.length)
						continue;

					if(s.a.IsPinned == false)
						s.a.Position = center + dir * s.length / 2;
					if(s.b.IsPinned == false)
						s.b.Position = center - dir * s.length / 2;
				}
			}

			//Untie(untie.Item1, untie.Item2, true);
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
