using SFML.Graphics;

namespace SMPL
{
	public abstract class Visual : Object
	{
		public Color Color { get; set; }
		public RenderTarget RenderTarget { get; set; }
		public Shader Shader { get; set; }
		public BlendMode BlendMode { get; set; }
		public bool IsHidden { get; set; }
		public Texture Texture { get; set; }

		public Visual()
		{
			Color = Color.White;
			RenderTarget = Game.Window;
			BlendMode = BlendMode.Alpha;
		}
		public abstract void Draw();
	}
}
