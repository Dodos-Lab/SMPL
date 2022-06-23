namespace SMPL.Graphics
{
	internal class SpriteBorder : Sprite
	{
		internal float BorderSize { get; set; } = 16;

		internal SpriteBorder(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			var camera = Get<Camera>(CameraUID);
			if(camera != null)
				renderTarget = camera.renderTexture;

			var Texture = GetTexture();
			var w = Texture == null ? 0 : Texture.Size.X;
			var h = Texture == null ? 0 : Texture.Size.Y;
			var topLeft = new Vector2f(w * TexCoordsUnitA.X, h * TexCoordsUnitA.Y);
			var bottomRight = new Vector2f(w * TexCoordsUnitB.X, h * TexCoordsUnitB.Y);
			var topRight = new Vector2f(bottomRight.X, topLeft.Y);
			var bottomLeft = new Vector2f(topLeft.X, bottomRight.Y);
			var verts = new Vertex[]
			{
				// top left
				new(CornerBorderClockwise(0).ToSFML(), Tint, topLeft),
				new(CornerBorderClockwise(0).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, 0)),
				new(CornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(0).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(0, BorderSize)),

				// top
				new(CornerClockwise(0).PointMoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, 0)),
				new(CornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerClockwise(1).PointMoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, 0)),

				// top right
				new(CornerBorderClockwise(1).ToSFML(), Tint, topRight),
				new(CornerBorderClockwise(1).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, 0)),
				new(CornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerClockwise(1).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(0, BorderSize)),

				// left
				new(CornerClockwise(0).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(0, BorderSize)),
				new(CornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CornerClockwise(3).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(0, -BorderSize)),

				// center
				new(CornerClockwise(0).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),

				// right
				new(CornerClockwise(1).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerClockwise(1).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(0, BorderSize)),
				new(CornerClockwise(2).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, bottomRight + new Vector2f(0, -BorderSize)),
				new(CornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),

				// bot left
				new(CornerClockwise(3).PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(0, -BorderSize)),
				new(CornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CornerClockwise(3).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, 0)),
				new(CornerBorderClockwise(3).ToSFML(), Tint, bottomLeft),

				// bot
				new(CornerClockwise(3).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(2).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, 0)),
				new(CornerClockwise(3).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, 0)),

				// bot right
				new(CornerClockwise(2).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CornerClockwise(2).PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(0, BorderSize)),
				new(CornerBorderClockwise(2).ToSFML(), Tint, bottomRight),
				new(CornerClockwise(2).PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, 0)),
			};

			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));
		}
		internal override Hitbox GetBoundingBox()
		{
			var borderSz = new Vector2(BorderSize, BorderSize) / Scale;
			var hitbox = new Hitbox(
				-Origin - borderSz,
				new Vector2(LocalSize.X + borderSz.X, -borderSz.Y) - Origin,
				LocalSize - Origin + borderSz,
				new Vector2(-borderSz.X, LocalSize.Y + borderSz.Y) - Origin,
				-Origin - borderSz);
			hitbox.TransformLocalLines(UID);
			return hitbox;
		}

		internal Vector2 CornerBorderClockwise(int index)
		{
			index = index.Limit(0, 4, Extensions.Limitation.Overflow);
			return index switch
			{
				0 => CornerClockwise(0).PointMoveAtAngle(Angle + 270, BorderSize, false).PointMoveAtAngle(Angle + 180, BorderSize, false),
				1 => CornerClockwise(1).PointMoveAtAngle(Angle + 270, BorderSize, false).PointMoveAtAngle(Angle, BorderSize, false),
				2 => CornerClockwise(2).PointMoveAtAngle(Angle, BorderSize, false).PointMoveAtAngle(Angle + 90, BorderSize, false),
				3 => CornerClockwise(3).PointMoveAtAngle(Angle + 180, BorderSize, false).PointMoveAtAngle(Angle + 90, BorderSize, false),
				_ => default,
			};
		}
	}
}
