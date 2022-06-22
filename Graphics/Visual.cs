namespace SMPL.Graphics
{
	internal abstract class Visual : Thing
	{
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
		public ThingManager.Effects Effect
		{
			get => effect;
			set
			{
				effect = value;

				if(effect != ThingManager.Effects.Custom)
					SetShader(effect);
			}
		}
		public string CameraUID { get; set; }
		public ThingManager.BlendModes BlendMode { get; set; } = ThingManager.BlendModes.Alpha;
		public Hitbox Hitbox { get; } = new();
		public Hitbox BoundingBox => GetBoundingBox();

		public void SetCustomShader(ThingManager.CodeGLSL shaderCode)
		{
			Effect = ThingManager.Effects.Custom;
			SetShader(shaderCode);
		}
		public void SetShaderFloatArray(string uniformName, float[] value)
		{
			shader?.SetUniformArray(uniformName, value);
		}
		public void SetShaderVector2Array(string uniformName, Vector2[] value)
		{
			var array = new Vec2[value.Length];
			for(int i = 0; i < array.Length; i++)
				array[i] = value[i].ToGLSL();

			shader?.SetUniformArray(uniformName, array);
		}
		public void SetShaderVector3Array(string uniformName, Vector3[] value)
		{
			var array = new Vec3[value.Length];
			for(int i = 0; i < array.Length; i++)
				array[i] = value[i].ToGLSL();

			shader?.SetUniformArray(uniformName, array);
		}
		public void SetShaderVector4Array(string uniformName, Vector4[] value)
		{
			var array = new Vec4[value.Length];
			for(int i = 0; i < array.Length; i++)
				array[i] = value[i].ToGLSL();

			shader?.SetUniformArray(uniformName, array);
		}
		public void SetShaderColorArray(string uniformName, Color[] value)
		{
			var array = new Vec4[value.Length];
			for(int i = 0; i < array.Length; i++)
				array[i] = value[i].ToGLSL();

			shader?.SetUniformArray(uniformName, array);
		}
		public void SetShaderFloat(string uniformName, float value)
		{
			shader?.SetUniform(uniformName, value);
		}
		public void SetShaderBool(string uniformName, bool value)
		{
			shader?.SetUniform(uniformName, value);
		}
		public void SetShaderInt(string uniformName, int value)
		{
			shader?.SetUniform(uniformName, value);
		}
		public void SetShaderVector2(string uniformName, Vector2 value)
		{
			shader?.SetUniform(uniformName, value.ToGLSL());
		}
		public void SetShaderVector3(string uniformName, Vector3 value)
		{
			shader?.SetUniform(uniformName, value.ToGLSL());
		}
		public void SetShaderVector4(string uniformName, Vector4 value)
		{
			shader?.SetUniform(uniformName, value.ToGLSL());
		}
		public void SetShaderColor(string uniformName, Color value)
		{
			shader?.SetUniform(uniformName, value.ToGLSL());
		}

		#region Backend
		private ThingManager.Effects effect;
		private int depth;
		private Shader shader;
		internal readonly static SortedDictionary<int, List<Visual>> visuals = new();
		internal readonly static Dictionary<ThingManager.Effects, ThingManager.CodeGLSL> shaders = new()
		{
			{ ThingManager.Effects.None,
				default },
			{ ThingManager.Effects.ColorFill, new()
			{
				FragmentUniforms = @"
uniform vec4 Color = vec4(1.0);
uniform float ThresholdOpacity;",
				FragmentCode = @"
if (finalColor.a > ThresholdOpacity)
	finalColor = Color;", } },
			{ ThingManager.Effects.ColorAdjust, new()
			{
				FragmentUniforms = @"
uniform float Gamma;
uniform float Desaturation;
uniform float Inversion;
uniform float Contrast;
uniform float Brightness;",
				FragmentCode = @"
vec3 color = FinalColor.rgb;
color = pow(color, vec3(1.0 / (1 - Gamma)));
color = mix(color, vec3(0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b), clamp(Desaturation, 0, 1));
color = mix(color, vec3(1.0) - color, clamp(Inversion, 0, 1));
color = (color - vec3(0.5)) * ((Contrast + 1) / (1 - Contrast)) + 0.5;
color = vec3(clamp(color.r, 0, 1), clamp(color.g, 0, 1), clamp(color.b, 0, 1));
color += vec3(Brightness);
color = vec3(clamp(color.r, 0, 1), clamp(color.g, 0, 1), clamp(color.b, 0, 1));
FinalColor = vec4(color, FinalColor.a);", } },
			{ ThingManager.Effects.ColorReplaceLight, new()
			{
				FragmentUniforms = @"
uniform vec4 ColorsInBright;
uniform vec4 ColorsInDark;
uniform float ThresholdBright;
uniform float ThresholdDark;",
				FragmentCode = @"
vec3 luminanceVector = vec3(0.2125, 0.7154, 0.0721);
float luminance = dot(luminanceVector, FinalColor.rgb);
float luminance2 = -dot(luminanceVector, FinalColor.rgb);
luminance = max(0.0, luminance - ThresholdDark);
luminance2 = max(0.0, luminance2 + (1 - ThresholdBright));
if (luminance == 0) FinalColor = ColorInDark;
if (luminance2 == 0) FinalColor = ColorInBright;"
			} },
			{ ThingManager.Effects.ColorsSwap, new()
			{
				FragmentUniforms = @"
uniform vec4 ColorsA[10];
uniform vec4 ColorsB[10];
uniform float RangesA[10];
uniform float RangesB[10];",
				FragmentCode = @"
for(int i = 0; i < 10; i++)
{
	if (ColorEqualsColor(FinalColor, ColorsA[i], RangesA[i])) FinalColor = ColorsB[i];
	else if (ColorEqualsColor(FinalColor, ColorsB[i], RangesB[i])) FinalColor = ColorsA[i];
}"
			} },
			{ ThingManager.Effects.ColorsTint, new()
			{
				FragmentUniforms = @"
uniform vec4 Colors[10];
uniform vec4 ColorTints[10];
uniform float Ranges[10];",
				FragmentCode = @"
for(int i = 0; i < 10; i++)
	if (ColorEqualsColor(FinalColor, Colors[i], Ranges[i]))
		FinalColor *= ColorTints[i];"
			} },
			{ ThingManager.Effects.ColorsReplace, new()
			{
				FragmentUniforms = @"
uniform vec4 Colors[10];
uniform vec4 NewColors[10];
uniform float Ranges[10];",
				FragmentCode = @"
for(int i = 0; i < 10; i++)
	if (ColorEqualsColor(FinalColor, Colors[i], Ranges[i]))
		FinalColor = NewColors[i];"
			} },
			{ ThingManager.Effects.Blur, new()
			{
				FragmentUniforms = @"
uniform vec2 Strength;",
				FragmentCode = @"
vec2 off = Strength / 100.0;
vec4 pixel =
	GetPixelColor(Texture, TextureCoords) * 4.0 +
	GetPixelColor(Texture, TextureCoords - off.x) * 2.0 +
	GetPixelColor(Texture, TextureCoords + off.x) * 2.0 +
	GetPixelColor(Texture, TextureCoords - off.y) * 2.0 +
	GetPixelColor(Texture, TextureCoords + off.y) * 2.0 +
	GetPixelColor(Texture, TextureCoords - off.x - off.y) * 1.0 +
	GetPixelColor(Texture, TextureCoords - off.x + off.y) * 1.0 +
	GetPixelColor(Texture, TextureCoords + off.x - off.y) * 1.0 +
	GetPixelColor(Texture, TextureCoords + off.x + off.y) * 1.0;
FinalColor = (pixel / 16.0);"
			} },
			{ ThingManager.Effects.Blink, new()
			{
				FragmentUniforms = @"
uniform float Speed;
uniform float TargetOpacity;",
				FragmentCode = @"
FinalColor.a = max(1 + cos(Time * Speed), TargetOpacity) / 2;"
			} },
			{ ThingManager.Effects.Edge, new()
			{
				FragmentUniforms = @"
uniform float Thickness;
uniform float Threshold;
uniform vec4 Color;",
				FragmentCode = @"
float offset = Thickness / 100.0;
vec2 offx = vec2(offset, 0.0);
vec2 offy = vec2(0.0, offset);
vec4 hEdge = GetPixelColor(Texture, TextureCoords - offy) * -2.0 +
	GetPixelColor(Texture, TextureCoords + offy) *  2.0 +
	GetPixelColor(Texture, TextureCoords - offx - offy) * -1.0 +
	GetPixelColor(Texture, TextureCoords - offx + offy) *  1.0 +
	GetPixelColor(Texture, TextureCoords + offx - offy) * -1.0 +
	GetPixelColor(Texture, TextureCoords + offx + offy) *  1.0;

	vec4 vEdge = GetPixelColor(Texture, TextureCoords - offx) *  2.0 +
	GetPixelColor(Texture, TextureCoords + offx) * -2.0 +
	GetPixelColor(Texture, TextureCoords - offx - offy) *  1.0 +
	GetPixelColor(Texture, TextureCoords - offx + offy) * -1.0 +
	GetPixelColor(Texture, TextureCoords + offx - offy) *  1.0 +
	GetPixelColor(Texture, TextureCoords + offx + offy) * -1.0;

   vec3 result = sqrt(hEdge.rgb * hEdge.rgb + vEdge.rgb * vEdge.rgb);
   float edge = length(result);
   vec4 pixel = GetPixelColor(Texture, TextureCoords);
	float value = Map(Threshold, 1.1, 0.0, 0.0, 2);
   if (edge > value)
      pixel.rgb = Color.rgb;
	FinalColor = pixel;"
			} },
			{ ThingManager.Effects.Earthquake, new()
			{
				FragmentUniforms = @"
uniform vec2 Strength;",
				FragmentCode = @"
TextureCoords.x += sin(radians(3000 * Time + TextureCoords.x * 0)) * cos(Time) * Strength.x / 100;
TextureCoords.y += cos(radians(3000 * Time + TextureCoords.y * 0)) * sin(Time) * Strength.y / 100;
FinalColor = GetPixelColor(Texture, TextureCoords);"
			} },
			{ ThingManager.Effects.Lights, new()
			{
				FragmentUniforms = @"
uniform vec4 AmbientColor;
uniform vec2 Positions[50];
uniform vec4 Colors[50];
uniform float Strengths[50];
uniform float Radiuses[50];",
				FragmentCode = @"
vec4 result = AmbientColor;
for(int i = 0; i < 50; i++)
{
	float dist = distance(CameraCoords, vec2(Positions[i].x, 1 - Positions[i].y));
	if (dist < Radiuses[i])
	{
		float value = Map(dist, Radiuses[i], 0, 0, Strengths[i]);
		result += value * Colors[i];
	}
}
FinalColor *= result;"
			} },
		};

		[JsonConstructor]
		internal Visual() { }
		internal Visual(string uid) : base(uid)
		{
			Depth = 0;
		}

		internal void Draw(RenderTarget renderTarget) => OnDraw(renderTarget);
		internal abstract void OnDraw(RenderTarget renderTarget);
		internal abstract Hitbox GetBoundingBox();

		internal Texture GetTexture()
		{
			var textures = Scene.CurrentScene.Textures;
			var path = TexturePath;
			if(string.IsNullOrWhiteSpace(path))
				return default;

			path = path.Replace("/", "\\");
			return textures.ContainsKey(path) ? textures[path] : null;
		}
		internal void SetShader(ThingManager.Effects effect)
		{
			if(Shader.IsAvailable == false)
			{
				Console.LogError(0, $"Could not set the *{effect}* shader. This device does not support shaders.");
				return;
			}

			var code = shaders[effect];
			SetShader(code);
		}
		internal void SetShader(ThingManager.CodeGLSL shaderCode)
		{
			shader?.Dispose();
			shader = null;

			if(shaderCode.FragmentCode == null && shaderCode.VertexCode == null)
				return;

			try
			{
				var frag = shaderCode.GetFragCode();
				var vert = shaderCode.GetVertCode();
				shader = Shader.FromString(vert, null, frag);
			}
			catch(Exception)
			{
				Effect = ThingManager.Effects.None;
				Console.LogError(0, "Could not set custom shader.", "Check the shader code for errors.");
			}
		}
		internal Shader GetShader()
		{
			if(Shader.IsAvailable == false)
				return default;

			var tex = GetTexture();
			if(tex != null)
				shader?.SetUniform("Texture", tex);

			shader?.SetUniform("HasTexture", tex != null);
			shader?.SetUniform("Time", Time.GameClock);

			var camera = Get<Camera>(CameraUID);
			if(camera == null)
				camera = Get<Camera>(Scene.MAIN_CAMERA_UID);

			var sz = camera.renderTexture.GetView().Size;
			shader?.SetUniform("CameraSize", new Vec2(sz.X, sz.Y));
			return shader;
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
		#endregion
	}
}
