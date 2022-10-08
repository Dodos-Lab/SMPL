namespace SMPL
{
	internal class CameraInstance : ThingInstance
	{
		[JsonProperty]
		public Vector2 Resolution
		{
			get => res;
			private set
			{
				res = value;

				renderTexture.Dispose();
				renderTexture = new((uint)value.X, (uint)value.Y);

				shadowMap.Dispose();
				shadowMap = new((uint)value.X, (uint)value.Y);

				Position = new();
			}
		}

		public bool IsSmooth
		{
			get => renderTexture.Smooth;
			set => renderTexture.Smooth = value;
		}
		[JsonIgnore]
		public Vector2 MousePosition
		{
			get
			{
				if(Game.Window == null)
					return new Vector2().NaN();

				var p = Mouse.GetPosition(Game.Window);
				return WorldToCamera(new(p.X, p.Y));
			}
			set
			{
				var p = CameraToWorld(value);
				var val = new Vector2i((int)p.X, (int)p.Y);

				if(Game.Window == null)
				{
					Mouse.SetPosition(val);
					return;
				}

				Mouse.SetPosition(val, Game.Window);
			}
		}
		[JsonIgnore]
		public RenderTexture RenderTexture
		{
			get
			{
				var view = renderTexture.GetView();
				view.Center = Position.ToSFML();
				view.Rotation = Angle;
				view.Size = new Vector2f(renderTexture.Size.X, renderTexture.Size.Y) * Scale;
				renderTexture.SetView(view);

				Scene.CurrentScene.Textures[UID] = renderTexture.Texture;
				return renderTexture;
			}
		}

		public Color BackgroundColor { get; set; }

		public override Hitbox BoundingBox
		{
			get
			{
				var sz = renderTexture.GetView().Size * 0.5f;
				var tl = new Vector2(-sz.X, -sz.Y);
				var tr = new Vector2(sz.X, -sz.Y);
				var br = new Vector2(sz.X, sz.Y);
				var bl = new Vector2(-sz.X, sz.Y);

				bb.Lines.Clear();
				bb.LocalLines.Clear();
				bb.LocalLines.Add(new(tl, tr));
				bb.LocalLines.Add(new(tr, br));
				bb.LocalLines.Add(new(br, bl));
				bb.LocalLines.Add(new(bl, tl));
				bb.TransformLocalLines(UID);
				return bb;
			}
		}

		public void Snap(string imagePath)
		{
			var img = renderTexture.Texture.CopyToImage();
			var result = img.SaveToFile(imagePath);
			img.Dispose();

			if(result == false)
				Console.LogError(1, $"Could not save the image at '{imagePath}'.");
		}

		public Vector2 WorldToCamera(Vector2 worldPoint)
		{
			return Game.Window == null ? default : Game.Window.MapPixelToCoords(new((int)worldPoint.X, (int)worldPoint.Y), renderTexture.GetView()).ToSystem();
		}
		public Vector2 CameraToWorld(Vector2 cameraPoint)
		{
			var p = Game.Window.MapCoordsToPixel(cameraPoint.ToSFML(), renderTexture.GetView());
			return Game.Window == null ? default : new(p.X, p.Y);
		}
		public Vector2 PointToParallax(Vector2 point, float parallaxUnit)
		{
			return point.PercentToTarget(Position, new(parallaxUnit * 100f));
		}

		#region Backend
		internal static readonly List<CameraInstance> cameras = new();
		internal readonly VertexArray shadowMapVerts = new(PrimitiveType.Quads);

		private Vector2 res;
		private RenderTexture renderTexture = new(1, 1), shadowMap = new(1, 1);
		private Shader shadowMapShader;

		[JsonConstructor]
		internal CameraInstance() => Init();
		internal CameraInstance(string uid, Vector2 resolution) : base(uid)
		{
			Init();

			var resolutionX = (uint)((int)resolution.X).Limit(0, (int)Texture.MaximumSize);
			var resolutionY = (uint)((int)resolution.Y).Limit(0, (int)Texture.MaximumSize);
			Position = new();
			Resolution = new(resolutionX, resolutionY);
		}
		private void Init()
		{
			cameras.Add(this);
			var shaderCode = VisualInstance.shaders[Thing.Effect.Lights];
			var frag = shaderCode.GetFragment();
			var vert = shaderCode.GetVertex();
			shadowMapShader = Shader.FromString(vert, null, frag);
		}

		internal void DrawShadowMap()
		{

			var amb = Thing.ShadowColor;
			var ambientVec = new Vector3(amb.R, amb.G, amb.B);
			var lerp = Vector3.Lerp(new(), ambientVec, amb.A / 255f);
			var resultCol = new Color((byte)lerp.X, (byte)lerp.Y, (byte)lerp.Z);
			var tex = shadowMap.Texture;
			shadowMapShader.SetUniform("Texture", tex);
			shadowMapShader.SetUniform("TextureSize", new Vec2(tex.Size.X, tex.Size.Y));

			shadowMapShader.SetUniform("HasTexture", tex != null);
			shadowMapShader.SetUniform("Time", Age);

			var sz = renderTexture.GetView().Size;
			var res = renderTexture.Size;
			shadowMapShader.SetUniform("CameraSize", new Vec2(sz.X, sz.Y));
			shadowMapShader.SetUniform("CameraResolution", new Vec2(res.X, res.Y));

			shadowMapShader.SetUniform("AmbientColor", Color.Red.ToGLSL());
			shadowMapShader.SetUniformArray("Colors", LightInstance.colors.ToGLSL());
			shadowMapShader.SetUniformArray("Positions", LightInstance.positions.ToGLSL());
			shadowMapShader.SetUniformArray("Scales", LightInstance.scales);

			var view = renderTexture.GetView();
			shadowMap.SetView(view);
			shadowMap.Clear(Color.Transparent);
			shadowMap.Draw(shadowMapVerts);
			shadowMap.Display();

			var texSz = renderTexture.Size;
			var bb = BoundingBox.Lines;
			var verts = new Vertex[]
			{
				new(bb[0].A.ToSFML(), Color.White, new Vector2f()),
				new(bb[1].A.ToSFML(), Color.White, new Vector2f(texSz.X, 0)),
				new(bb[2].A.ToSFML(), Color.White, new Vector2f(texSz.X, texSz.Y)),
				new(bb[3].A.ToSFML(), Color.White, new Vector2f(0, texSz.Y))
			};

			renderTexture.Draw(verts, PrimitiveType.Quads, new(BlendMode.Alpha, Transform.Identity, shadowMap.Texture, shadowMapShader));
		}
		internal static void DrawMainCameraToWindow(RenderWindow window, bool mainCameraIsWindow)
		{
			if(mainCameraIsWindow)
			{
				window.Display();
				return;
			}

			Scene.MainCamera.renderTexture.Display();
			var texSz = Scene.MainCamera.renderTexture.Size;
			var viewSz = window.GetView().Size;
			var verts = new Vertex[]
			{
				new(-viewSz * 0.5f, new Vector2f()),
				new(new Vector2f(viewSz.X * 0.5f, -viewSz.Y * 0.5f), new Vector2f(texSz.X, 0)),
				new(viewSz * 0.5f, new Vector2f(texSz.X, texSz.Y)),
				new(new Vector2f(-viewSz.X * 0.5f, viewSz.Y * 0.5f), new Vector2f(0, texSz.Y))
			};

			window.Clear();
			window.Draw(verts, PrimitiveType.Quads, new(Scene.MainCamera.renderTexture.Texture));
			window.Display();
		}
		#endregion
	}
}
