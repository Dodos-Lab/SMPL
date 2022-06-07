namespace SMPL.Graphics
{
	internal class SpriteBorder : Sprite
	{
		internal float BorderSize { get; set; } = 16;

		internal SpriteBorder(string uid) : base(uid) { }

		internal override void Draw(Camera camera = null)
		{
			if(IsHidden)
				return;

			camera ??= Scene.MainCamera;

			var w = Texture == null ? 0 : Texture.Size.X;
			var h = Texture == null ? 0 : Texture.Size.Y;
			var topLeft = new Vector2f(w * TexCoordsUnitA.X, h * TexCoordsUnitA.Y);
			var bottomRight = new Vector2f(w * TexCoordsUnitB.X, h * TexCoordsUnitB.Y);
			var topRight = new Vector2f(bottomRight.X, topLeft.Y);
			var bottomLeft = new Vector2f(topLeft.X, bottomRight.Y);
			var verts = new Vertex[]
			{
				// top left
				new(GetCornerBorderClockwise(0).ToSFML(), Tint, topLeft),
				new(GetCornerBorderClockwise(0).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, 0)),
				new(GetCornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(0).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(0, BorderSize)),

				// top
				new(GetCornerClockwise(0).PointMoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, 0)),
				new(GetCornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(GetCornerClockwise(1).PointMoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, 0)),

				// top right
				new(GetCornerBorderClockwise(1).ToSFML(), Tint, topRight),
				new(GetCornerBorderClockwise(1).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, 0)),
				new(GetCornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(GetCornerClockwise(1).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(0, BorderSize)),

				// left
				new(GetCornerClockwise(0).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(0, BorderSize)),
				new(GetCornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(GetCornerClockwise(3).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(0, -BorderSize)),

				// center
				new(GetCornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(GetCornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),

				// right
				new(GetCornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(GetCornerClockwise(1).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(0, BorderSize)),
				new(GetCornerClockwise(2).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, bottomRight + new Vector2f(0, -BorderSize)),
				new(GetCornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),

				// bot left
				new(GetCornerClockwise(3).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(0, -BorderSize)),
				new(GetCornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(GetCornerClockwise(3).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, 0)),
				new(GetCornerBorderClockwise(3).ToSFML(), Tint, bottomLeft),

				// bot
				new(GetCornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(GetCornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(2).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, 0)),
				new(GetCornerClockwise(3).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, 0)),

				// bot right
				new(GetCornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(GetCornerClockwise(2).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(0, BorderSize)),
				new(GetCornerBorderClockwise(2).ToSFML(), Tint, bottomRight),
				new(GetCornerClockwise(2).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, 0)),
			};

			camera.renderTexture.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, Texture, Shader));
		}
		internal override void SetDefaultHitbox()
		{
			var borderSz = new Vector2(BorderSize, BorderSize) / Scale;
			Hitbox.LocalLines.Clear();
			Hitbox.LocalLines.Add(new(-Origin - borderSz, new Vector2(LocalSize.X + borderSz.X, -borderSz.Y) - Origin));
			Hitbox.LocalLines.Add(new(new Vector2(LocalSize.X + borderSz.X, -borderSz.Y) - Origin, LocalSize - Origin + borderSz));
			Hitbox.LocalLines.Add(new(LocalSize - Origin + borderSz, new Vector2(-borderSz.X, LocalSize.Y + borderSz.Y) - Origin));
			Hitbox.LocalLines.Add(new(new Vector2(-borderSz.X, LocalSize.Y + borderSz.Y) - Origin, -Origin - borderSz));
		}

		internal Vector2 GetCornerBorderClockwise(int index)
		{
			index = index.Limit(0, 4, Extensions.Limitation.Overflow);
			return index switch
			{
				0 => GetCornerClockwise(0).PointMoveAtAngle(Angle + 270, BorderSize, false).PointMoveAtAngle(Angle + 180, BorderSize, false),
				1 => GetCornerClockwise(1).PointMoveAtAngle(Angle + 270, BorderSize, false).PointMoveAtAngle(Angle, BorderSize, false),
				2 => GetCornerClockwise(2).PointMoveAtAngle(Angle, BorderSize, false).PointMoveAtAngle(Angle + 90, BorderSize, false),
				3 => GetCornerClockwise(3).PointMoveAtAngle(Angle + 180, BorderSize, false).PointMoveAtAngle(Angle + 90, BorderSize, false),
				_ => default,
			};
		}
	}
}
