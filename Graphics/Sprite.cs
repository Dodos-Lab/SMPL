using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
	public class Sprite : Visual
	{
		/// <summary>
		/// <see cref="Draw"/> uses this unit vector and <see cref="TexCoordsUnitB"/> to determine the visible square on <see cref="Visual.Texture"/>.<br></br>
		/// - Note: [0, 0] is the top left and [1, 1] is the bottom right of the <see cref="Visual.Texture"/>
		/// no matter the <see cref="Texture.Size"/>.<br></br>
		/// - Example: Switching the X values will flip the texture on the X axis, negative values and values bigger than 1 will wrap around etc.
		/// </summary>
		public Vector2 TexCoordsUnitA { get; set; }
		/// <summary>
		/// See <see cref="TexCoordsUnitA"/> for info.
		/// </summary>
		public Vector2 TexCoordsUnitB { get; set; }

		/// <summary>
		/// Size relative to <see cref="Object.Scale"/>.
		/// </summary>
		public Vector2 LocalSize { get; set; }
		/// <summary>
		/// Size in the world.
		/// </summary>
		public Vector2 Size
		{
			get => LocalSize * Scale;
			set => LocalSize = value / Scale;
		}

		/// <summary>
		/// This determines the positional offset from <see cref="Object.Position"/> as a unit vector.<br></br>
		/// Note: [0, 0] is the top left and [1, 1] is the bottom right corner of the <see cref="Sprite"/> (no matter the <see cref="Size"/>).
		/// Values can also go bellow 0 and above 1.
		/// </summary>
		public Vector2 OriginUnit { get; set; }
		/// <summary>
		/// This determines the positional offset from <see cref="Object.Position"/> as a point vector.<br></br>
		/// </summary>
		public Vector2 Origin
		{ get => OriginUnit * LocalSize; set => OriginUnit = value / LocalSize; }
		
		/// <summary>
		/// The top left corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 TopLeft => GetPosition(-Origin);
		/// <summary>
		/// The top right corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 TopRight => GetPosition(new Vector2(LocalSize.X, 0) - Origin);
		/// <summary>
		/// The bottom right corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 BottomRight => GetPosition(LocalSize - Origin);
		/// <summary>
		/// The bottom left corner of the <see cref="Sprite"/> in the world.
		/// </summary>
		public Vector2 BottomLeft => GetPosition(new Vector2(0, LocalSize.Y) - Origin);

		public Sprite()
		{
			TexCoordsUnitB = new(1, 1);
			LocalSize = new(100, 100);
			OriginUnit = new(0.5f, 0.5f);
		}

		/// <summary>
		/// Draws the <see cref="Sprite"/> on the <see cref="Visual.RenderTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/> and <see cref="Sprite"/> parameters.
		/// </summary>
		public override void Draw()
		{
			if (IsHidden)
				return;

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

			RenderTarget?.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
	}
}
