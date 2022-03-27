using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
	public class Sprite : Visual
	{
		private const float BASE_SIZE = 100;

		public Vector2 TexCoordsUnitA { get; set; }
		public Vector2 TexCoordsUnitB { get; set; }

		public Vector2 TopLeft => GetPosition(-Origin);
		public Vector2 TopRight => GetPosition((Texture == null ? new(BASE_SIZE, 0) : new(Texture.Size.X, 0)) - Origin);
		public Vector2 BottomRight => GetPosition((Texture == null ? new(BASE_SIZE, BASE_SIZE) : new(Texture.Size.X, Texture.Size.Y)) - Origin);
		public Vector2 BottomLeft => GetPosition((Texture == null ? new(0, BASE_SIZE) : new(0, Texture.Size.Y)) - Origin);

		public bool IsCaptured
		{
			get => Camera.Captures(TopLeft) || Camera.Captures(TopRight) || Camera.Captures(BottomLeft) || Camera.Captures(BottomRight);
		}

		public Sprite()
		{
			TexCoordsUnitB = new(1, 1);
		}

		public override void Draw()
		{
			if (IsHidden || IsCaptured == false)
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
