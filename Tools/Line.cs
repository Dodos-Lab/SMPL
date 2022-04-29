using SFML.Graphics;
using SMPL.Core;
using System.Numerics;

namespace SMPL.Tools
{
	/// <summary>
	/// Lines are useful for collision detection, debugging, raycasting and much more.
	/// </summary>
	public struct Line
	{
		/// <summary>
		/// The first (starting) point of the line.
		/// </summary>
		public Vector2 A { get; set; }
		/// <summary>
		/// The second (ending) point of the line.
		/// </summary>
		public Vector2 B { get; set; }
		/// <summary>
		/// The distance between <see cref="A"/> and <see cref="B"/>.
		/// </summary>
		public float Length => Vector2.Distance(A, B);
		/// <summary>
		/// The angle between <see cref="A"/> and <see cref="B"/>.
		/// </summary>
		public float Angle => A.AngleBetweenPoints(B);
		/// <summary>
		/// The direction between <see cref="A"/> and <see cref="B"/>.
		/// </summary>
		public Vector2 Direction => Angle.AngleToDirection();

		/// <summary>
		/// Creates the line from two points: <paramref name="a"/> and  <paramref name="b"/>.
		/// </summary>
		public Line(Vector2 a, Vector2 b)
		{
			A = a;
			B = b;
		}

		/// <summary>
		/// Draws this line to a <paramref name="camera"/> with <paramref name="color"/> having some
		/// <paramref name="width"/>. The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if no
		/// <paramref name="camera"/> is passed. The default <paramref name="color"/> is assumed to be white if no
		/// <paramref name="color"/> is passed.
		/// </summary>
		public void Draw(Camera camera = default, Color color = default, float width = 4)
		{
			camera ??= Scene.MainCamera;
			color = color == default ? Color.White : color;

			width /= 2;
			var startLeft = A.PointMoveAtAngle(Angle - 90, width, false);
			var startRight = A.PointMoveAtAngle(Angle + 90, width, false);
			var endLeft = B.PointMoveAtAngle(Angle - 90, width, false);
			var endRight = B.PointMoveAtAngle(Angle + 90, width, false);

			var vert = new Vertex[]
			{
				new(new(startLeft.X, startLeft.Y), color),
				new(new(startRight.X, startRight.Y), color),
				new(new(endRight.X, endRight.Y), color),
				new(new(endLeft.X, endLeft.Y), color),
			};
			camera.renderTexture.Draw(vert, PrimitiveType.Quads);
		}
		/// <summary>
		/// Returns the point where this line and another <paramref name="line"/> cross. Returns an invalid vector
		/// (<see cref="Extensions.NaN(Vector2)"/>) if there is no such point.
		/// </summary>
		public Vector2 GetCrossPoint(Line line)
		{
			var p = CrossPoint(A, B, line.A, line.B);
			return Contains(p) && line.Contains(p) ? p : new(float.NaN, float.NaN);
		}
		/// <summary>
		/// Returns whether this line and another <paramref name="line"/> cross.
		/// </summary>
		public bool Crosses(Line line)
		{
			var result = GetCrossPoint(line);
			return float.IsNaN(result.X) == false && float.IsNaN(result.Y) == false;
		}
		/// <summary>
		/// Returns whether a <paramref name="point"/> is on top this line. The margin of error is 0.01 units.
		/// </summary>
		public bool Contains(Vector2 point)
		{
			var length = Vector2.Distance(A, B);
			var sum = Vector2.Distance(A, point) + Vector2.Distance(B, point);
			return sum.IsBetween(length - 0.01f, length + 0.01f);
		}
		/// <summary>
		/// Tries to go from <see cref="A"/> to <see cref="B"/> by getting around <paramref name="hitbox"/>
		/// [<see cref="A"/> -> returned point -> <see cref="B"/>] with a certain amount of <paramref name="tries"/> by picking
		/// random points. This is good for getting around a single wall/edge/shape. Calculating it multiple times in a row
		/// (in <see cref="Scene.OnUpdate"/> for example) will result in the best option. Returns an invalid vector (<see cref="Extensions.NaN(Vector2)"/>
		/// if the calculation fails to find the best point. Returns <see cref="B"/> if the path [<see cref="A"/> -> <see cref="B"/>]
		/// has no obstacles.<br></br><br></br>
		/// May be used as:<br></br>
		/// - Setting <see cref="A"/> to be the position of some <see cref="Object"/>/<see cref="Sprite"/>.<br></br>
		/// - Setting <see cref="B"/> to be some target point destination (for example <see cref="Scene.MouseCursorPosition"/>).<br></br>
		/// - Checking whether there is a clear path between them by calling this method and its result being invalid or not.<br></br>
		/// - If invalid then this means it is time to handle the case where there is no path/the path is too complex to be taken
		/// (with more than one turn).<br></br>
		/// - Otherwise move toward the result.<br></br>
		/// - Move directly toward <see cref="B"/> once the path is clear.
		/// </summary>
		public Vector2 GetPathfindResult(uint tries, Hitbox hitbox)
		{
			if (tries == 0)
				return new Vector2().NaN();

			if (IsCrossing(A, B, B) == false)
				return B;

			var bestPoint = new Vector2().NaN();
			var bestDist = double.MaxValue;
			for (int i = 0; i < tries; i++)
			{
				var randPoint = A.PointMoveAtAngle(i * (360 / tries), Vector2.Distance(A, B), false);
				var sumDist = Vector2.Distance(A, randPoint) + Vector2.Distance(randPoint, B);
				if (IsCrossing(A, randPoint, B) == false && sumDist < bestDist)
				{
					bestDist = sumDist;
					bestPoint = randPoint;
				}
			}

			for (int i = 0; i < tries; i++)
			{
				var curP = A.PointPercentTowardPoint(bestPoint, new(100 / tries * i, 100 / tries * i));
				if (IsCrossing(curP, B, B) == false)
					return curP;
			}

			return bestPoint;

			bool IsCrossing(Vector2 p1, Vector2 p2, Vector2 p3)
			{
				for (int i = 0; i < hitbox.Lines.Count; i++)
				{
					var cross1 = hitbox.Lines[i].Crosses(new(p1, p2));
					var cross2 = hitbox.Lines[i].Crosses(new(p2, p3));
					if (cross1 || cross2)
						return true;
				}
				return false;
			}
		}
		/// <summary>
		/// Returns the closest point on the line to a <paramref name="point"/>.
		/// </summary>
		public Vector2 GetClosestPoint(Vector2 point)
		{
			var AP = point - A;
			var AB = B - A;

			float magnitudeAB = AB.LengthSquared();
			float ABAPproduct = Vector2.Dot(AP, AB);
			float distance = ABAPproduct / magnitudeAB;

			return distance < 0 ?
				A : distance > 1 ?
				B : A + AB * distance;
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
