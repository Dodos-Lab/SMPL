namespace SMPL.Graphics
{
	internal abstract class Visual : Thing
	{
		internal enum BlendModes { None, Alpha, Add, Multiply }

		internal Color Tint { get; set; } = Color.White;
		internal bool IsAdditive { get; set; }
		internal bool IsHidden { get; set; }
		internal string TexturePath { get; set; }
		internal string ShaderPath { get; set; }
		internal BlendModes BlendMode { get; set; } = BlendModes.Alpha;

		[JsonIgnore]
		internal Texture Texture => TexturePath != null && Scene.CurrentScene.Textures.ContainsKey(TexturePath) ? Scene.CurrentScene.Textures[TexturePath] : null;
		[JsonIgnore]
		internal Shader Shader => ShaderPath != null && Scene.CurrentScene.Shaders.ContainsKey(ShaderPath) ? Scene.CurrentScene.Shaders[ShaderPath] : null;

		internal Visual(string uid) : base(uid) { }

		internal abstract void Draw(Camera camera = null);

		internal BlendMode GetBlendMode()
		{
			return BlendMode switch
			{
				BlendModes.Alpha => SFML.Graphics.BlendMode.Alpha,
				BlendModes.Add => SFML.Graphics.BlendMode.Add,
				BlendModes.Multiply => SFML.Graphics.BlendMode.Multiply,
				_ => SFML.Graphics.BlendMode.None,
			};
		}
	}
}
