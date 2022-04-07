using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
	public struct Line
	{
		public Vector2 A { get; set; }
		public Vector2 B { get; set; }
		public float Length => Vector2.Distance(A, B);
		public float Angle => A.AngleBetweenPoints(B);
		public Vector2 Direction => Angle.AngleToDirection();

		public Line(Vector2 a, Vector2 b)
		{
			A = a;
			B = b;
		}

		public void Draw(RenderTarget renderTarget, Color color, float width = 2)
		{
			width /= 2;
			var startLeft = A.MovePointAtAngle(Angle - 90, width, false);
			var startRight = A.MovePointAtAngle(Angle + 90, width, false);
			var endLeft = B.MovePointAtAngle(Angle - 90, width, false);
			var endRight = B.MovePointAtAngle(Angle + 90, width, false);

			var vert = new Vertex[]
			{
				new(new(startLeft.X, startLeft.Y), color),
				new(new(startRight.X, startRight.Y), color),
				new(new(endRight.X, endRight.Y), color),
				new(new(endLeft.X, endLeft.Y), color),
			};
			renderTarget.Draw(vert, PrimitiveType.Quads);
		}
		public Vector2 CrossPoint(Line line)
		{
			var p = CrossPoint(A, B, line.A, line.B);
			return Contains(p) && line.Contains(p) ? p : new(float.NaN, float.NaN);
		}
		public bool Crosses(Line line)
		{
			var result = CrossPoint(line);
			return float.IsNaN(result.X) == false && float.IsNaN(result.Y) == false;
		}
		public bool Contains(Vector2 point)
		{
			var length = Vector2.Distance(A, B);
			var sum = Vector2.Distance(A, point) + Vector2.Distance(B, point);
			return sum.IsBetween(length - 0.01f, length + 0.01f);
		}
		public Vector2 Pathfind(uint tries, params Line[] lines)
		{
			if (tries == 0)
				return new Vector2().NaN();

			if (IsCrossing(A, B, B) == false)
				return B;

			var bestPoint = new Vector2().NaN();
			var bestDist = double.MaxValue;
			for (int i = 0; i < tries; i++)
			{
				var randPoint = A.MovePointAtAngle(i * (360 / tries), Vector2.Distance(A, B), false);
				var sumDist = Vector2.Distance(A, randPoint) + Vector2.Distance(randPoint, B);
				if (IsCrossing(A, randPoint, B) == false && sumDist < bestDist)
				{
					bestDist = sumDist;
					bestPoint = randPoint;
				}
			}

			for (int i = 0; i < tries; i++)
			{
				var curP = A.PercentTowardTarget(bestPoint, new(100 / tries * i, 100 / tries * i));
				if (IsCrossing(curP, B, B) == false)
					return curP;
			}

			return bestPoint;

			bool IsCrossing(Vector2 p1, Vector2 p2, Vector2 p3)
			{
				for (int i = 0; i < lines.Length; i++)
				{
					var cross1 = lines[i].Crosses(new(p1, p2));
					var cross2 = lines[i].Crosses(new(p2, p3));
					if (cross1 || cross2)
						return true;
				}
				return false;
			}
		}

		private static Vector2 CrossPoint(Vector2 A, Vector2 B, Vector2 C, Vector2 D)
		{
			var a1 = B.Y - A.Y;
			var b1 = A.X - B.X;
			var c1 = a1 * (A.X) + b1 * (A.Y);
			var a2 = D.Y - C.Y;
			var b2 = C.X - D.X;
			var c2 = a2 * (C.X) + b2 * (C.Y);
			var determinant = a1 * b2 - a2 * b1;

			if (determinant == 0)
				return new Vector2(float.NaN, float.NaN);
			else
			{
				var x = (b2 * c1 - b1 * c2) / determinant;
				var y = (a1 * c2 - a2 * c1) / determinant;
				return new Vector2(x, y);
			}
		}
	}
}
