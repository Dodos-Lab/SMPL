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

		public void SetCustomShader(ThingManager.CodeGLSL shaderCode)
		{
			Effect = ThingManager.Effects.Custom;
			SetShader(shaderCode);
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
		#region Backend
		private ThingManager.Effects effect;
		private int depth;
		private Shader shader;
		internal readonly static SortedDictionary<int, List<Visual>> visuals = new();
		internal readonly static Dictionary<ThingManager.Effects, ThingManager.CodeGLSL> shaders = new()
		{
			{ ThingManager.Effects.None, default },
			{ ThingManager.Effects.ColorFill, new()
			{
				FragmentUniforms = @"
uniform vec4 Color = vec4(1.0);
uniform float ThresholdOpacity = 0.5;",
				FragmentCode = @"
if (FinalColor.a > ThresholdOpacity)
	FinalColor = Color;", } },
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
uniform bool ReplaceBright = true;
uniform bool ReplaceDark = true;
uniform vec4 ColorInBright = vec4(0.0, 0.0, 0.0, 1.0);
uniform vec4 ColorInDark = vec4(1.0);
uniform float ThresholdBright = 0.5;
uniform float ThresholdDark = 0.5;",
				FragmentCode = @"
vec3 l = vec3(0.2125, 0.7154, 0.0721);
float dark = dot(l, FinalColor.rgb);
float bright = -dot(l, FinalColor.rgb);
dark = max(0.0, dark - ThresholdDark);
bright = max(0.0, bright + (1 - ThresholdBright));
if (ReplaceDark && dark == 0.0)
	FinalColor = ColorInDark;
if (ReplaceBright && bright == 0.0)
	FinalColor = ColorInBright;"
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
uniform vec2 Strength = 0.5;",
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
uniform float Speed = 0.5;
uniform float TargetOpacity;
uniform float OpacityThreshold = 0.5;",
				FragmentCode = @"
if (FinalColor.a > OpacityThreshold)
	FinalColor.a = max(1 + cos(Time * Speed * 10), TargetOpacity) / 2;"
			} },
			{ ThingManager.Effects.Edge, new()
			{
				FragmentUniforms = @"
uniform float Thickness = 0.2;
uniform float Threshold = 0.7;
uniform vec4 Color = vec4(1.0);",
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
	float value = (1 - Threshold) * 2;
   if (edge > value)
      pixel.rgb = Color.rgb;
	FinalColor = pixel;"
			} },
			{ ThingManager.Effects.Earthquake, new()
			{
				FragmentUniforms = @"
uniform vec2 Strength = vec2(0.4, 0.3);",
				FragmentCode = @"
TextureCoords.x += sin(radians(3000 * Time + TextureCoords.x * 0)) * cos(Time) * Strength.x / 100;
TextureCoords.y += cos(radians(3000 * Time + TextureCoords.y * 0)) * sin(Time) * Strength.y / 100;
FinalColor = GetPixelColor(Texture, TextureCoords);"
			} },
			{ ThingManager.Effects.Lights, new()
			{
				FragmentUniforms = @"
uniform vec4 AmbientColor = vec4(0.2, 0.2, 0.2, 1.0);

//uniform vec2 Direction;
//uniform vec4 DirectionalColor;
//uniform float DirectionalIntensity;

uniform vec2 Positions[50];
uniform vec4 Colors[50];
uniform float Scales[50];",
				FragmentCode = @"
vec3 result = AmbientColor.rgb;
//float value =
//	sin(TextureCoords.x * 2 - (Direction.x - 0.28) * 2) * DirectionalIntensity +
//	sin(TextureCoords.y * 2 + (Direction.y + 0.28) * 2) * DirectionalIntensity;
//result += value;

float cameraRatio = CameraSize.x / CameraSize.y;

for(int i = 0; i < 50; i++)
{
	float radius = Scales[i] * 0.05;
	float scale = CameraResolution.x / CameraSize.x;
	float dist = distance(vec2(CameraCoords.x * cameraRatio, CameraCoords.y), vec2(Positions[i].x * cameraRatio, 1 - Positions[i].y) / CameraSize);
	radius *= scale;
	dist /= scale;
	if (dist < radius)
	{
		float value = Map(dist, radius, 0, 0, Colors[i].a * 5);
		result += value * Colors[i].rgb;
	}
}
FinalColor.rgb *= result;"
			} },
			{ ThingManager.Effects.Grid, new()
			{
				FragmentUniforms = @"
uniform vec4 ColorX = vec4(1.0);
uniform vec4 ColorY = vec4(1.0);
uniform vec2 GridSize = 2;
uniform vec2 GridSpacing = 10;
uniform float ThresholdOpacity = 0.5;",
				FragmentCode = @"
GridSpacing -= GridSize;
if (FinalColor.a > ThresholdOpacity && mod(TextureCoords.x * TextureSize.x, round(GridSize.x) + GridSpacing.x) > round(GridSpacing.x))
	FinalColor = ColorX;
if (FinalColor.a > ThresholdOpacity && mod(TextureCoords.y * TextureSize.y, round(GridSize.y) + GridSpacing.y) > round(GridSpacing.y))
	FinalColor = ColorY;"
			} },
			{ ThingManager.Effects.Outline, new()
			{
				FragmentUniforms = @"
uniform vec4 Color = vec4(1.0);
uniform float Thickness = 2;
uniform float ThresholdOpacity = 0.5;",
				FragmentCode = @"
float off = Thickness / 1000;
vec4 col = Color;
if (FinalColor.a < ThresholdOpacity)
{
	float u = GetPixelColor(Texture, vec2(TextureCoords.x, TextureCoords.y - off)).a;
	float d = GetPixelColor(Texture, vec2(TextureCoords.x, TextureCoords.y + off)).a;
	float l = GetPixelColor(Texture, vec2(TextureCoords.x - off, TextureCoords.y)).a;
	float r = GetPixelColor(Texture, vec2(TextureCoords.x + off, TextureCoords.y)).a;

	if (u > 0.0 || d > 0.0 || l > 0.0 || r > 0.0)
		FinalColor = col;
}"
			} },
			{ ThingManager.Effects.Pixelate, new()
			{
				FragmentUniforms = @"
uniform float Strength = 4;",
				FragmentCode = @"
float factor = 1.0 / (Strength / 500 + 0.001);
vec2 pos = floor(TextureCoords * factor + 0.5) / factor;
FinalColor = GetPixelColor(Texture, pos);"
			} },
			{ ThingManager.Effects.Screen, new()
			{
				FragmentUniforms = @"
uniform float Speed = 0.1;
uniform float Size = 1.0;",
				FragmentCode = @"
FinalColor.rgb *= 1 + sin((-Time * Speed * 100.0) + TextureCoords.y * (10.0 / Size)) / 2;"
			} },
			{ ThingManager.Effects.Water, new()
			{
				FragmentUniforms = @"
uniform vec2 Speed = vec2(0.3, 0.3);
uniform vec2 Strength = vec2(0.3, 0.2);",
				FragmentCode = @"
Strength *= 20;
Speed *= 2;
TextureCoords.x += sin(radians(2000 * Time * Speed.x / 10 + TextureCoords.y * 250)) * 0.02 * Strength.x / 10;
TextureCoords.y += cos(radians(2000 * Time * Speed.y / 10 + TextureCoords.x * 500)) * 0.03 * Strength.y / 10;
FinalColor = GetPixelColor(Texture, TextureCoords);"
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

		internal Texture GetTexture()
		{
			var textures = Scene.CurrentScene.Textures;
			var path = TexturePath.ToBackslashPath();
			return path != null && textures.ContainsKey(path) ? textures[path] : null;
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
				var frag = shaderCode.GetFragment();
				var vert = shaderCode.GetVertex();
				shader = Shader.FromString(vert, null, frag);
			}
			catch(Exception)
			{
				Effect = ThingManager.Effects.None;
				Console.LogError(0, "Could not set custom shader.", "Check the shader code for errors.");
			}
		}
		internal Shader GetShader(RenderTarget renderTarget)
		{
			if(Shader.IsAvailable == false || shader == default)
				return default;

			var tex = GetTexture();
			if(tex != null)
			{
				shader?.SetUniform("Texture", tex);
				shader?.SetUniform("TextureSize", new Vec2(tex.Size.X, tex.Size.Y));
			}

			shader?.SetUniform("HasTexture", tex != null);
			shader?.SetUniform("Time", AgeSeconds);

			var sz = renderTarget.GetView().Size;
			var res = renderTarget.Size;
			shader?.SetUniform("CameraSize", new Vec2(sz.X, sz.Y));
			shader?.SetUniform("CameraResolution", new Vec2(res.X, res.Y));
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
