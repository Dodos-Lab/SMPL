using SFML.Graphics;
using System;
using System.Numerics;

namespace SMPL
{
	public class VisualSprite : Visual
	{
		private const float BASE_SIZE = 100;
		private Vector2 repeats;

		public Texture Texture { get; set; }

		public Vector2 TopLeft
		{ get => Position.MoveAtAngle(Angle + Position.AngleToPoint(Position - Origin), Origin.Length() * Scale.Length() * 0.7071f, false); }
		public Vector2 TopRight
		{ get => TopLeft.MoveAtAngle(Angle, Texture == null ? BASE_SIZE : Texture.Size.X * Scale.X, false); }
		public Vector2 BottomRight
		{ get => TopRight.MoveAtAngle(Angle + 90, Texture == null ? BASE_SIZE : Texture.Size.Y * Scale.Y, false); }
		public Vector2 BottomLeft
		{ get => BottomRight.MoveAtAngle(Angle + 180, Texture == null ? BASE_SIZE : Texture.Size.X * Scale.X, false); }

		public bool IsFlippedX { get; set; }
		public bool IsFlippedY { get; set; }
		public bool IsCaptured
		{
			get
			{
				var view = RenderTarget.GetView();
				var screen = new FloatRect(view.Center - view.Size * 0.5f, view.Size);
				return screen.Contains(TopLeft.X, TopLeft.Y) || screen.Contains(TopRight.X, TopRight.Y) ||
					screen.Contains(BottomLeft.X, BottomLeft.Y) || screen.Contains(BottomRight.X, BottomRight.Y);
			}
		}
		public Vector2 Repeats
		{
			get => repeats;
			set { repeats = new(MathF.Max(1, value.X), MathF.Max(1, value.Y)); }
		}

		public VisualSprite()
		{
			Repeats = new(1, 1);
		}

		public override void Draw()
		{
			if (IsHidden || IsCaptured == false)
				return;

			var w = Texture == null ? 0 : Texture.Size.X * Repeats.X;
			var h = Texture == null ? 0 : Texture.Size.Y * Repeats.Y;
			var w0 = IsFlippedX ? w : 0;
			var ww = IsFlippedX ? 0 : w;
			var h0 = IsFlippedY ? h : 0;
			var hh = IsFlippedY ? 0 : h;

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
