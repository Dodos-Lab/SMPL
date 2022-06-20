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
		internal readonly static SortedDictionary<int, List<Visual>> visuals = new();

		public Color Tint { get; set; } = Color.White;
		public bool IsHidden { get; set; }
		public int Depth
		{
			get => depth;
			set
			{
				TryCreateDepth(depth);

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
		public ThingManager.BlendModes BlendMode { get; set; } = ThingManager.BlendModes.Alpha;
		public Hitbox Hitbox { get; } = new();

		[JsonConstructor]
		internal Visual() { }
		internal Visual(string uid) : base(uid)
		{
			Depth = 0;
		}

		internal void Draw(RenderTarget renderTarget) => OnDraw(renderTarget);
		internal abstract void OnDraw(RenderTarget renderTarget);

		internal Texture GetTexture()
		{
			var textures = Scene.CurrentScene.Textures;
			var path = TexturePath;
			if(string.IsNullOrWhiteSpace(path))
				return default;

			path = path.Replace("/", "\\");
			return textures.ContainsKey(path) ? textures[path] : null;
		}
		internal Shader GetShader()
		{
			var shaders = Scene.CurrentScene.Shaders;
			var path = ShaderPath;
			if(string.IsNullOrWhiteSpace(path))
				return default;

			path = path.Replace("/", "\\");
			return shaders.ContainsKey(path) ? shaders[path] : null;
		}
		internal BlendMode GetBlendMode()
		{
			return BlendMode switch
			{
				ThingManager.BlendModes.Alpha => SFML.Graphics.BlendMode.Alpha,
				ThingManager.BlendModes.Add => SFML.Graphics.BlendMode.Add,
				ThingManager.BlendModes.Multiply => SFML.Graphics.BlendMode.Multiply,
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
