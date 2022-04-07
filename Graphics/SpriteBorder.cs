using SFML.Graphics;
using SFML.System;
using System.Numerics;

namespace SMPL
{
	public class SpriteBorder : Sprite
	{
		public Vector2 BorderTopLeft
		{ get => TopLeft.MovePointAtAngle(Angle + 270, BorderSize, false).MovePointAtAngle(Angle + 180, BorderSize, false); }
		public Vector2 BorderTopRight
		{ get => TopRight.MovePointAtAngle(270 + Angle, BorderSize, false).MovePointAtAngle(Angle, BorderSize, false); }
		public Vector2 BorderBottomRight
		{ get => BottomRight.MovePointAtAngle(Angle, BorderSize, false).MovePointAtAngle(Angle + 90, BorderSize, false); }
		public Vector2 BorderBottomLeft
		{ get => BottomLeft.MovePointAtAngle(Angle + 180, BorderSize, false).MovePointAtAngle(Angle + 90, BorderSize, false); }

		public float BorderSize { get; set; } = 16;

		public SpriteBorder()
		{
			TexCoordsUnitB = new(1, 1);
		}

		public override void Draw()
		{
			if (IsHidden)
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
				new(BorderTopLeft.MovePointAtAngle(Angle, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(BorderSize, 0)),
				new(TopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(TopLeft.MovePointAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(0, BorderSize)),

				// top
				new(TopLeft.MovePointAtAngle(Angle + 270, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(BorderSize, 0)),
				new(TopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(TopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(TopRight.MovePointAtAngle(Angle + 270, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(-BorderSize, 0)),

				// top right
				new(BorderTopRight.ToSFML(), Color, topRight),
				new(BorderTopRight.MovePointAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(-BorderSize, 0)),
				new(TopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(TopRight.MovePointAtAngle(Angle, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(0, BorderSize)),

				// left
				new(TopLeft.MovePointAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, topLeft + new Vector2f(0, BorderSize)),
				new(TopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(BottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(BottomLeft.MovePointAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(0, -BorderSize)),

				// center
				new(TopLeft.ToSFML(), Color, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(TopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(BottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(BottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),

				// right
				new(TopRight.ToSFML(), Color, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(TopRight.MovePointAtAngle(Angle, BorderSize, false).ToSFML(), Color, topRight + new Vector2f(0, BorderSize)),
				new(BottomRight.MovePointAtAngle(Angle, BorderSize, false).ToSFML(), Color, bottomRight + new Vector2f(0, -BorderSize)),
				new(BottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),

				// bot left
				new(BottomLeft.MovePointAtAngle(Angle + 180, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(0, -BorderSize)),
				new(BottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(BottomLeft.MovePointAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, 0)),
				new(BorderBottomLeft.ToSFML(), Color, bottomLeft),

				// bot
				new(BottomLeft.ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(BottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(BottomRight.MovePointAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomRight - new Vector2f(BorderSize, 0)),
				new(BottomLeft.MovePointAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomLeft + new Vector2f(BorderSize, 0)),

				// bot right
				new(BottomRight.ToSFML(), Color, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(BottomRight.MovePointAtAngle(Angle, BorderSize, false).ToSFML(), Color, bottomRight - new Vector2f(0, BorderSize)),
				new(BorderBottomRight.ToSFML(), Color, bottomRight),
				new(BottomRight.MovePointAtAngle(Angle + 90, BorderSize, false).ToSFML(), Color, bottomRight - new Vector2f(BorderSize, 0)),
			};

			RenderTarget?.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
		public override void SetDefaultHitbox()
		{
			var borderSz = new Vector2(BorderSize, BorderSize) / Scale;
			Hitbox.LocalLines.Clear();
			Hitbox.LocalLines.Add(new(-Origin - borderSz, new Vector2(LocalSize.X + borderSz.X, -borderSz.Y) - Origin));
			Hitbox.LocalLines.Add(new(new Vector2(LocalSize.X + borderSz.X, -borderSz.Y) - Origin, LocalSize - Origin + borderSz));
			Hitbox.LocalLines.Add(new(LocalSize - Origin + borderSz, new Vector2(-borderSz.X, LocalSize.Y + borderSz.Y) - Origin));
			Hitbox.LocalLines.Add(new(new Vector2(-borderSz.X, LocalSize.Y + borderSz.Y) - Origin, -Origin - borderSz));
		}
	}
}
