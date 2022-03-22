using SFML.Graphics;
using System.Collections.Generic;
using System.Numerics;

namespace SMPL
{
	public struct Hitbox
	{
		public List<Line> Lines { get; set; }

		public Hitbox(params Vector2[] points)
		{
			Lines = new();

			for (int i = 1; i < points?.Length; i++)
				Lines.Add(new(points[i - 1], points[i]));
		}

		public void Draw(RenderTarget renderTarget, Color color, float width = 2)
		{
			for (int i = 0; i < Lines.Count; i++)
				Lines[i].Draw(renderTarget, color, width);
		}

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
		public bool ConvexOverlaps(Hitbox hitbox)
		{
			return Crosses(hitbox) || ConvexContains(hitbox);
		}
		public bool ConvexContains(Vector2 point)
		{
			if (Lines == null || Lines.Count < 3)
				return false;

			var crosses = 0;
			var outsidePoint = Lines[0].A.PercentTowardTarget(Lines[0].B, new(-500, -500));

			for (int i = 0; i < Lines.Count; i++)
				if (Lines[i].Crosses(new(point, outsidePoint)))
					crosses++;

			return crosses % 2 == 1;
		}
		public bool Crosses(Hitbox hitbox)
		{
			for (int i = 0; i < Lines.Count; i++)
				for (int j = 0; j < hitbox.Lines.Count; j++)
					if (Lines[i].Crosses(hitbox.Lines[j]))
						return true;

			return false;
		}
		public bool ConvexContains(Hitbox hitbox)
		{
			for (int i = 0; i < hitbox.Lines.Count; i++)
				if (ConvexContains(hitbox.Lines[i].A) == false || ConvexContains(hitbox.Lines[i].B) == false)
					return false;
			return true;
		}
	}
}
