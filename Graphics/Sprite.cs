using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
	/// <summary>
	/// Inherit chain: <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
	/// The №1 used <see cref="Visual"/> in games. A simple texture with some extra effects accompanying it.
	/// </summary>
	public class Sprite : Visual
	{
		/// <summary>
		/// <see cref="Draw"/> uses this vector and <see cref="TexCoordsUnitB"/> to determine the visible square on <see cref="Visual.Texture"/>.<br></br>
		/// - Note: [0, 0] is the top left and [1, 1] is the bottom right of the <see cref="Visual.Texture"/>
		/// no matter the <see cref="Texture.Size"/>.<br></br>
		/// - Example: Switching the X values will flip the texture on the X axis, negative values and values bigger than 1 will wrap around etc.
		/// </summary>
		public Vector2 TexCoordsUnitA { get; set; }
		/// <summary>
		/// See <see cref="TexCoordsUnitA"/> for info.
		/// </summary>
		public Vector2 TexCoordsUnitB { get; set; } = new(1, 1);

		/// <summary>
		/// Size relative to <see cref="Object.Scale"/>.
		/// </summary>
		public Vector2 LocalSize { get; set; } = new(100, 100);
		/// <summary>
		/// Size in the world.
		/// </summary>
		public Vector2 Size
		{
			get => LocalSize * Scale;
			set => LocalSize = value / Scale;
		}

		/// <summary>
		/// This determines the positional offset from <see cref="Object.Position"/> as a vector.<br></br>
		/// Note: [0, 0] is the top left and [1, 1] is the bottom right corner of the <see cref="Sprite"/> (no matter the <see cref="Size"/>).
		/// Values can also go bellow 0 and above 1.
		/// </summary>
		public Vector2 OriginUnit { get; set; } = new(0.5f, 0.5f);
		/// <summary>
		/// This determines the positional offset from <see cref="Object.Position"/> as a point vector.<br></br>
		/// </summary>
		public Vector2 Origin
		{ get => OriginUnit * LocalSize; set => OriginUnit = value / LocalSize; }
		
		/// <summary>
		/// The top left corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 TopLeft => GetPositionFromSelf(-Origin);
		/// <summary>
		/// The top right corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 TopRight => GetPositionFromSelf(new Vector2(LocalSize.X, 0) - Origin);
		/// <summary>
		/// The bottom right corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 BottomRight => GetPositionFromSelf(LocalSize - Origin);
		/// <summary>
		/// The bottom left corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 BottomLeft => GetPositionFromSelf(new Vector2(0, LocalSize.Y) - Origin);

		/// <summary>
		/// A set of <see cref="Hitbox.Lines"/> that determine if and where this <see cref="Sprite"/> interacts with other <see cref="SMPL.Hitbox"/>es.
		/// Useful for collision detection, checking whether it is hovered by the mouse cursor etc.<br></br>
		/// - Note: Use <see cref="Hitbox.UpdateLines(Object)"/> by passing this <see cref="Sprite"/> in order for the <see cref="Object"/>
		/// transformations to affect this <see cref="Hitbox"/>.
		/// </summary>
		public Hitbox Hitbox { get; set; } = new();

		/// <summary>
		/// Draws the <see cref="Sprite"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/> and <see cref="Sprite"/> parameters.
		/// </summary>
		public override void Draw()
		{
			if (IsHidden)
				return;

			DrawTarget ??= Scene.MainCamera;

			var w = Texture == null ? 0 : Texture.Size.X;
			var h = Texture == null ? 0 : Texture.Size.Y;
			var w0 = w * TexCoordsUnitA.X;
			var ww = w * TexCoordsUnitB.X;
			var h0 = h * TexCoordsUnitA.Y;
			var hh = h * TexCoordsUnitB.Y;

			var verts = new Vertex[]
			{
				new(TopLeft.ToSFML(), Color, new(w0, h0)),
				new(TopRight.ToSFML(), Color, new(ww, h0)),
				new(BottomRight.ToSFML(), Color, new(ww, hh)),
				new(BottomLeft.ToSFML(), Color, new(w0, hh)),
			};

			DrawTarget.renderTexture.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
		/// <summary>
		/// Sets a rectangular <see cref="SMPL.Hitbox"/> in <see cref="Hitbox.LocalLines"/>. This takes into account <see cref="OriginUnit"/> and
		/// <see cref="Size"/> so this should be called after each change on these in order to maintain the proper shape of
		/// the default <see cref="Hitbox"/>.
		/// </summary>
		public virtual void SetDefaultHitbox()
		{
			Hitbox.LocalLines.Clear();
			Hitbox.LocalLines.Add(new(-Origin, new Vector2(LocalSize.X, 0) - Origin));
			Hitbox.LocalLines.Add(new(new Vector2(LocalSize.X, 0) - Origin, LocalSize - Origin));
			Hitbox.LocalLines.Add(new(LocalSize - Origin, new Vector2(0, LocalSize.Y) - Origin));
			Hitbox.LocalLines.Add(new(new Vector2(0, LocalSize.Y) - Origin, -Origin));
		}
	}
}
