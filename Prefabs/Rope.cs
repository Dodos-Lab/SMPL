namespace SMPL.Prefabs
{
	public class Rope
	{
		public Vector2 Position { get; set; }
		public Vector2 Gravity { get; set; } = new Vector2(0, 1);
		public Vector2 Force { get; set; }

		public Rope(Vector2 position, int segmentCount = 5, float segmentLength = 100f)
		{
			Position = position;
			var step = new Vector2(segmentLength, 0);
			for(int i = 0; i < segmentCount; i++)
			{
				var segment = i == 0 ?
					new Segment(new(position, true), new(position + step)) :
					new Segment(segments[i - 1].b, new(position + step * (i + 1)));

				segments.Add(segment);
			}
		}
		public void Draw(RenderTarget renderTarget = default, Color color = default, float width = 4f)
		{
			for(int i = 0; i < segments.Count; i++)
				new Line(segments[i].a.position, segments[i].b.position).Draw(renderTarget, color, width);
		}

		public void Connect(int segmentIndex, Rope rope, int targetSegmentIndex)
		{
			var s1 = segments[segmentIndex - 1];
			var s2 = rope.segments[targetSegmentIndex];

			s1.b = s2.a;
			segments[segmentIndex].a = s2.a;
		}
		public void Update()
		{
			if(segments.Count > 0)
				segments[0].a.position = Position;

			for(int i = 0; i < segments.Count; i++)
			{
				var s = segments[i];

				CalculatePoint(s.a);
				CalculatePoint(s.b);

				for(int j = 0; j < 5; j++)
				{
					var a = s.a.position;
					var b = s.b.position;
					var center = (a + b) / 2;
					var dir = Vector2.Normalize(a - b);
					var length = (a - b).Length();

					if(length <= s.length)
						continue;

					if(s.a.isLocked == false)
						s.a.position = center + dir * s.length / 2;
					if(s.b.isLocked == false)
						s.b.position = center - dir * s.length / 2;
				}

				void CalculatePoint(Point p)
				{
					if(p.isLocked)
						return;

					var prev = p.position;
					p.position += p.position - p.prevPosition;
					p.position += Gravity * Time.Delta * 7;
					p.position += Force * Time.Delta * 7;
					p.prevPosition = prev;
				}
			}
		}

		private class Segment
		{
			public Point a, b;
			public float length;

			public Segment(Point a, Point b)
			{
				this.a = a;
				this.b = b;
				length = Vector2.Distance(a.position, b.position);
			}
		}
		private class Point
		{
			public Vector2 position, prevPosition;
			public bool isLocked;

			public Point(Vector2 position, bool isLocked = false)
			{
				this.position = position;
				this.isLocked = isLocked;
				prevPosition = position;
			}
		}
		private readonly List<Segment> segments = new();
	}
}
