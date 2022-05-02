using SFML.Graphics;
using SFML.System;
using SMPL.Core;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;
using System.Numerics;

namespace SMPL.Graphics
{
	/// <summary>
	/// Inherit chain:  <see cref="SpriteBorder"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
	/// A <see cref="Sprite"/> surrounded by 8 square fragments acting as a border. Resizing the sprite will keep the border untouched in most cases.
	/// </summary>
	public class SpriteBorder : Sprite
	{
		/// <summary>
		/// Initially this is the top left corner of the border in the world.
		/// </summary>
		public Vector2 CornerBorderA
		{ get => CornerA.PointMoveAtAngle(Angle + 270, BorderSize, false).PointMoveAtAngle(Angle + 180, BorderSize, false); }
		/// <summary>
		/// Initially this is the top right corner of the border in the world.
		/// </summary>
		public Vector2 CornerBorderB
		{ get => CornerB.PointMoveAtAngle(270 + Angle, BorderSize, false).PointMoveAtAngle(Angle, BorderSize, false); }
		/// <summary>
		/// Initially this is the bottom right corner of the border in the world.
		/// </summary>
		public Vector2 CornerBorderC
		{ get => CornerC.PointMoveAtAngle(Angle, BorderSize, false).PointMoveAtAngle(Angle + 90, BorderSize, false); }
		/// <summary>
		/// Initially this is the bottom left corner of the border in the world.
		/// </summary>
		public Vector2 CornerBorderD
		{ get => CornerD.PointMoveAtAngle(Angle + 180, BorderSize, false).PointMoveAtAngle(Angle + 90, BorderSize, false); }

		/// <summary>
		/// The size of the border which determines each of the 8 fragment sizes and their texture coordinates
		/// (spacing from each side of the <see cref="Visual.Texture"/>).
		/// </summary>
		public float BorderSize { get; set; } = 16;

		/// <summary>
		/// Draws the <see cref="Sprite"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/> and <see cref="Sprite"/> and <see cref="SpriteBorder"/> parameters.
		/// </summary>
		public override void Draw()
		{
			if (IsHidden)
				return;

			DrawTarget ??= Scene.MainCamera;

			var w = Texture == null ? 0 : Texture.Size.X;
			var h = Texture == null ? 0 : Texture.Size.Y;

			var topLeft = new Vector2f(w * TexCoordsUnitA.X, h * TexCoordsUnitA.Y);
			var bottomRight = new Vector2f(w * TexCoordsUnitB.X, h * TexCoordsUnitB.Y);
			var topRight = new Vector2f(bottomRight.X, topLeft.Y);
			var bottomLeft = new Vector2f(topLeft.X, bottomRight.Y);

			var verts = new Vertex[]
			{
				// top left
				new(CornerBorderA.ToSFML(), Tint, topLeft),
				new(CornerBorderA.PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, 0)),
				new(CornerA.ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerA.PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(0, BorderSize)),

				// top
				new(CornerA.PointMoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(BorderSize, 0)),
				new(CornerA.ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerB.ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerB.PointMoveAtAngle(Angle + 270, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, 0)),

				// top right
				new(CornerBorderB.ToSFML(), Tint, topRight),
				new(CornerBorderB.PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(-BorderSize, 0)),
				new(CornerB.ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerB.PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(0, BorderSize)),

				// left
				new(CornerA.PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, topLeft + new Vector2f(0, BorderSize)),
				new(CornerA.ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerD.ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CornerD.PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(0, -BorderSize)),

				// center
				new(CornerA.ToSFML(), Tint, topLeft + new Vector2f(BorderSize, BorderSize)),
				new(CornerB.ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerC.ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CornerD.ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),

				// right
				new(CornerB.ToSFML(), Tint, topRight + new Vector2f(-BorderSize, BorderSize)),
				new(CornerB.PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, topRight + new Vector2f(0, BorderSize)),
				new(CornerC.PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, bottomRight + new Vector2f(0, -BorderSize)),
				new(CornerC.ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),

				// bot left
				new(CornerD.PointMoveAtAngle(Angle + 180, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(0, -BorderSize)),
				new(CornerD.ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CornerD.PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, 0)),
				new(CornerBorderD.ToSFML(), Tint, bottomLeft),

				// bot
				new(CornerD.ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, -BorderSize)),
				new(CornerC.ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CornerC.PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, 0)),
				new(CornerD.PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomLeft + new Vector2f(BorderSize, 0)),

				// bot right
				new(CornerC.ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, BorderSize)),
				new(CornerC.PointMoveAtAngle(Angle, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(0, BorderSize)),
				new(CornerBorderC.ToSFML(), Tint, bottomRight),
				new(CornerC.PointMoveAtAngle(Angle + 90, BorderSize, false).ToSFML(), Tint, bottomRight - new Vector2f(BorderSize, 0)),
			};

			DrawTarget.renderTexture.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
		/// <summary>
		/// Sets a rectangular <see cref="Hitbox"/> in <see cref="Hitbox.LocalLines"/>. This takes into account <see cref="Sprite.OriginUnit"/>,
		/// <see cref="Sprite.Size"/> and <see cref="BorderSize"/> so this should be called after each change on these in order to maintain
		/// the proper shape of the default <see cref="Hitbox"/>.
		/// </summary>
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
