namespace SMPL.Graphics
{
	internal abstract class Visual : Thing
	{
		internal const string DEFAULT_FRAG = @"
uniform sampler2D texture;

void main()
{
    vec4 pixel = texture2D(texture, gl_TexCoord[0].xy);
    gl_FragColor = gl_Color * pixel;
}";
		internal const string DEFAULT_VERT = @"
void main()
{
    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
    gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
    gl_FrontColor = gl_Color;
}";

		private int depth;
		internal readonly static Dictionary<int, List<Visual>> visuals = new();

		public enum BlendModes { None, Alpha, Add, Multiply }

		public Color Tint { get; set; } = Color.White;
		public bool IsAdditive { get; set; }
		public bool IsHidden { get; set; }
		public int Depth
		{
			get => depth;
			set
			{
				TryCreateDepth(depth);

				if(visuals[depth].Contains(this))
					visuals[depth].Remove(this);

				depth = value;

				TryCreateDepth(depth);
				visuals[depth].Add(this);

				void TryCreateDepth(int depth)
				{
					if(visuals.ContainsKey(depth) == false)
						visuals[depth] = new();
				}
			}
		}
		public string TexturePath { get; set; }
		public string ShaderPath { get; set; }
		public string CameraUID { get; set; }
		public BlendModes BlendMode { get; set; } = BlendModes.Alpha;

		[JsonIgnore]
		internal Texture Texture => TexturePath != null && Scene.CurrentScene.Textures.ContainsKey(TexturePath) ? Scene.CurrentScene.Textures[TexturePath] : null;
		[JsonIgnore]
		internal Shader Shader => ShaderPath != null && Scene.CurrentScene.Shaders.ContainsKey(ShaderPath) ? Scene.CurrentScene.Shaders[ShaderPath] : null;

		internal Visual(string uid) : base(uid)
		{
			Depth = 0;
		}

		internal void Draw(RenderTarget renderTarget) => OnDraw(renderTarget);
		internal abstract void OnDraw(RenderTarget renderTarget);

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
		internal override void OnDestroy()
		{
			base.OnDestroy();
			visuals[depth].Remove(this);
		}
	}
}
