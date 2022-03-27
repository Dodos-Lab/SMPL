using SFML.Graphics;
using SFML.System;
using System.Numerics;

namespace SMPL
{
	public class SpriteBorder : Visual
	{
		private const float BASE_SIZE = 100;

		public Vector2 TexCoordsUnitA { get; set; }
		public Vector2 TexCoordsUnitB { get; set; }

		public Vector2 BorderTopLeft
		{ get => CenterTopLeft.MoveAtAngle(Angle + 270, BorderSize, false).MoveAtAngle(Angle + 180, BorderSize, false); }
		public Vector2 BorderTopRight
		{ get => CenterTopRight.MoveAtAngle(270 + Angle, BorderSize, false).MoveAtAngle(Angle, BorderSize, false); }
		public Vector2 BorderBottomRight
		{ get => CenterBottomRight.MoveAtAngle(Angle, BorderSize, false).MoveAtAngle(Angle + 90, BorderSize, false); }
		public Vector2 BorderBottomLeft
		{ get => CenterBottomLeft.MoveAtAngle(Angle + 180, BorderSize, false).MoveAtAngle(Angle + 90, BorderSize, false); }

		public Vector2 CenterTopLeft
		{ get => Position.MoveAtAngle(Angle + Position.AngleToPoint(Position - Origin), Origin.Length() * Scale.Length() * 0.7071f, false); }
		public Vector2 CenterTopRight
		{ get => CenterTopLeft.MoveAtAngle(Angle, Texture == null ? BASE_SIZE : Texture.Size.X * Scale.X, false); }
		public Vector2 CenterBottomRight
		{ get => CenterTopRight.MoveAtAngle(Angle + 90, Texture == null ? BASE_SIZE : Texture.Size.Y * Scale.Y, false); }
		public Vector2 CenterBottomLeft
		{ get => CenterBottomRight.MoveAtAngle(Angle + 180, Texture == null ? BASE_SIZE : Texture.Size.X * Scale.X, false); }

		public bool IsCaptured
		{
			get => Camera.Captures(CenterTopLeft) || Camera.Captures(CenterTopRight) ||
				Camera.Captures(CenterBottomLeft) || Camera.Captures(CenterBottomRight);
		}
		public float BorderSize { get; set; } = 16;

		public SpriteBorder()
		{
			TexCoordsUnitB = new(1, 1);
		}

		public override void Draw()
		{
			if (IsHidden || IsCaptured == false)
				return;

			var w = Texture == null ? 0 : Texture.Size.X;
			var h = Texture == null ? 0 : Texture.Size.Y;

			var topLeft = new Vector2f(w * TexCoordsUnitA.X, h * TexCoordsUnitA.Y);
			var bottomRight = new Vector2f(w * TexCoordsUnitB.X, h * TexCoordsUnitB.Y);
			var topRight = new Vector2f(bottomRight.X, topLeft.Y);
			var bottomLeft = new Vector2f(topLeft.X, bottomRight.Y);

			var verts = new Vertex[]
			{
				// top left
				new(BorderTopLeft.ToSFML(), Color, topLeft),
				new(BorderTopLeft.MoveAtAngle(Angle, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(BorderSize, 0)),
				new(CenterTopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CenterTopLeft.MoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(0, BorderSize)),

				// top
				new(CenterTopLeft.MoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(BorderSize, 0)),
				new(CenterTopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CenterTopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CenterTopRight.MoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(-BorderSize, 0)),

				// top right
				new(BorderTopRight.ToSFML(), Color, topRight),
				new(BorderTopRight.MoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(-BorderSize, 0)),
				new(CenterTopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CenterTopRight.MoveAtAngle(Angle, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(0, BorderSize)),

				// left
				new(CenterTopLeft.MoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(0, BorderSize)),
				new(CenterTopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CenterBottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CenterBottomLeft.MoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(0, -BorderSize)),

				// center
				new(CenterTopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CenterTopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CenterBottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CenterBottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),

				// right
				new(CenterTopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CenterTopRight.MoveAtAngle(Angle, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(0, BorderSize)),
				new(CenterBottomRight.MoveAtAngle(Angle, BorderSize, false).ToSFML(), Color, bottomRight + new Vector2f(0, -BorderSize)),
				new(CenterBottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),

				// bot left
				new(CenterBottomLeft.MoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(0, -BorderSize)),
				new(CenterBottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CenterBottomLeft.MoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, 0)),
				new(BorderBottomLeft.ToSFML(), Color, bottomLeft),

				// bot
				new(CenterBottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CenterBottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CenterBottomRight.MoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomRight - new Vector2f(BorderSize, 0)),
				new(CenterBottomLeft.MoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, 0)),

				// bot right
				new(CenterBottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CenterBottomRight.MoveAtAngle(Angle, BorderSize, false).ToSFML(), Color, bottomRight - new Vector2f(0, BorderSize)),
				new(BorderBottomRight.ToSFML(), Color, bottomRight),
				new(CenterBottomRight.MoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomRight - new Vector2f(BorderSize, 0)),
			};

			RenderTarget?.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
	}
}
