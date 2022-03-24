using SFML.Graphics;
using System;
using System.Numerics;

namespace SMPL
{
	public class VisualSprite : Visual
	{
		private const float BASE_SIZE = 100;

		public Texture Texture { get; set; }
		public Vector2 TexCoordsUnitA { get; set; }
		public Vector2 TexCoordsUnitB { get; set; }

		public Vector2 TopLeft
		{ get => Position.MoveAtAngle(Angle + Position.AngleToPoint(Position - Origin), Origin.Length() * Scale.Length() * 0.7071f, false); }
		public Vector2 TopRight
		{ get => TopLeft.MoveAtAngle(Angle, Texture == null ? BASE_SIZE : Texture.Size.X * Scale.X, false); }
		public Vector2 BottomRight
		{ get => TopRight.MoveAtAngle(Angle + 90, Texture == null ? BASE_SIZE : Texture.Size.Y * Scale.Y, false); }
		public Vector2 BottomLeft
		{ get => BottomRight.MoveAtAngle(Angle + 180, Texture == null ? BASE_SIZE : Texture.Size.X * Scale.X, false); }

		public bool IsCaptured
		{
			get => Camera.Captures(TopLeft) || Camera.Captures(TopRight) || Camera.Captures(BottomLeft) || Camera.Captures(BottomRight);
		}

		public VisualSprite()
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
