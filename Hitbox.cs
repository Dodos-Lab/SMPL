using SFML.Graphics;
using System.Collections.Generic;
using System.Numerics;

namespace SMPL
{
	/// <summary>
	/// A <see cref="Line"/> collection used to determine whether it interacts in any way with other hitboxes/points in the world.
	/// </summary>
	public class Hitbox
	{
		/// <summary>
		/// This list is used by <see cref="UpdateLines(Object)"/> which transforms and moves its contents into <see cref="Lines"/>.
		/// </summary>
		public List<Line> LocalLines { get; } = new();
		/// <summary>
		/// This list is used by <see cref="UpdateLines(Object)"/> for writing and all the rest of the methods for reading.
		/// </summary>
		public List<Line> Lines { get; } = new();

		/// <summary>
		/// Constructs both <see cref="LocalLines"/> and <see cref="Lines"/> from between the <paramref name="points"/>.
		/// </summary>
		public Hitbox(params Vector2[] points)
		{
			for (int i = 1; i < points?.Length; i++)
			{
				LocalLines.Add(new(points[i - 1], points[i]));
				Lines.Add(new(points[i - 1], points[i]));
			}
		}

		/// <summary>
		/// Takes <see cref="LocalLines"/>, applies <paramref name="obj"/>'s transformations on them and puts the result into <see cref="Lines"/>
		/// for the rest of the methods to use.<br></br>
		/// - Note: Any previous changes to the <see cref="Lines"/> list will be erased.
		/// </summary>
		public void TransformLocalLines(Object obj)
		{
			Lines.Clear();

			for (int i = 0; i < LocalLines.Count; i++)
				Lines.Add(new(obj.GetPositionFromSelf(LocalLines[i].A), obj.GetPositionFromSelf(LocalLines[i].B)));
		}
		/// <summary>
		/// Draws all <see cref="Lines"/> onto <paramref name="renderTarget"/> with <paramref name="color"/> and <paramref name="width"/>.
		/// </summary>
		public void Draw(RenderTarget renderTarget, Color color, float width = 2)
		{
			for (int i = 0; i < Lines.Count; i++)
				Lines[i].Draw(renderTarget, color, width);
		}
		/// <summary>
		/// Gets all the cross points (if any) produced between <see cref="Lines"/> and <paramref name="hitbox"/>'s <see cref="Lines"/>.
		/// </summary>
		public List<Vector2> CrossPoints(Hitbox hitbox)
		{
			var result = new List<Vector2>();
			for (int i = 0; i < Lines.Count; i++)
				for (int j = 0; j < hitbox.Lines.Count; j++)
				{
					var p = Lines[i].CrossPoint(hitbox.Lines[j]);
					if (p.IsNaN() == false)
						result.Add(p);
				}
			return result;
		}
		/// <summary>
		/// A shortcut for
		/// <code>var overlaps = Crosses(hitbox) || ConvexContains(hitbox);</code>
		/// </summary>
		public bool ConvexOverlaps(Hitbox hitbox)
		{
			return Crosses(hitbox) || ConvexContains(hitbox);
		}
		/// <summary>
		/// Whether <see cref="Lines"/> surround <paramref name="point"/>.
		/// Or in other words: whether this <see cref="Hitbox"/> contains <paramref name="point"/>.<br></br>
		/// - Note: Some of the results will be wrong if <see cref="Lines"/> are forming a concave shape.
		/// </summary>
		public bool ConvexContains(Vector2 point)
		{
			if (LocalLines == null || LocalLines.Count < 3)
				return false;

			var crosses = 0;
			var outsidePoint = LocalLines[0].A.PercentTowardTarget(LocalLines[0].B, new(-500, -500));

			for (int i = 0; i < LocalLines.Count; i++)
				if (LocalLines[i].Crosses(new(point, outsidePoint)))
					crosses++;

			return crosses % 2 == 1;
		}
		/// <summary>
		/// Whether <see cref="Lines"/> cross with <paramref name="hitbox"/>'s <see cref="Lines"/>.
		/// </summary>
		public bool Crosses(Hitbox hitbox)
		{
			for (int i = 0; i < LocalLines.Count; i++)
				for (int j = 0; j < hitbox.LocalLines.Count; j++)
					if (LocalLines[i].Crosses(hitbox.LocalLines[j]))
						return true;

			return false;
		}
		/// <summary>
		/// Whether <see cref="Lines"/> completely surround <paramref name="hitbox"/>'s <see cref="Lines"/>.
		/// Or in other words: whether this <see cref="Hitbox"/> contains <paramref name="hitbox"/>.<br></br>
		/// - Note: Some of the results will be wrong if <see cref="Lines"/> or <paramref name="hitbox"/>'s <see cref="Lines"/> are forming a concave shape.
		/// </summary>
		public bool ConvexContains(Hitbox hitbox)
		{
			for (int i = 0; i < hitbox.LocalLines.Count; i++)
				if (ConvexContains(hitbox.LocalLines[i].A) == false || ConvexContains(hitbox.LocalLines[i].B) == false)
					return false;
			return true;
		}
	}
}
