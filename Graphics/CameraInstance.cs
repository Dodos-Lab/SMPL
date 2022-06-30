namespace SMPL.Graphics
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
				Position = new();
			}
		}

		public bool IsSmooth
		{
			get => renderTexture.Smooth;
			set => renderTexture.Smooth = value;
		}
		[JsonIgnore]
		public Vector2 MouseCursorPosition
		{
			get { var p = Mouse.GetPosition(Game.Window); return PointToCamera(new(p.X, p.Y)); }
			set { var p = PointToWorld(value); Mouse.SetPosition(new((int)p.X, (int)p.Y), Game.Window); }
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

		public void Snap(string imagePath)
		{
			var img = renderTexture.Texture.CopyToImage();
			var result = img.SaveToFile(imagePath);
			img.Dispose();

			if(result == false)
				Console.LogError(1, $"Could not save the image at '{imagePath}'.");
		}
		public override Vector2 GetCornerClockwise(int index)
		{
			index = index.Limit(0, 4, Extensions.Limitation.Overflow);
			var sz = renderTexture.GetView().Size * 0.5f;
			return index switch
			{
				0 => GetPositionFromSelf(new(-sz.X, -sz.Y)),
				1 => GetPositionFromSelf(new(sz.X, -sz.Y)),
				2 => GetPositionFromSelf(new(sz.X, sz.Y)),
				3 => GetPositionFromSelf(new(-sz.X, sz.Y)),
				_ => default,
			};
		}

		public Vector2 PointToCamera(Vector2 worldPoint)
		{
			return Game.Window == null ? default : Game.Window.MapPixelToCoords(new((int)worldPoint.X, (int)worldPoint.Y), renderTexture.GetView()).ToSystem();
		}
		public Vector2 PointToWorld(Vector2 cameraPoint)
		{
			var p = Game.Window.MapCoordsToPixel(cameraPoint.ToSFML(), renderTexture.GetView());
			return Game.Window == null ? default : new(p.X, p.Y);
		}

		#region Backend
		internal static readonly List<CameraInstance> cameras = new();

		private Vector2 res;
		private RenderTexture renderTexture = new(1, 1);

		[JsonConstructor]
		internal CameraInstance()
		{
			cameras.Add(this);
		}
		internal CameraInstance(string uid, Vector2 resolution) : base(uid)
		{
			cameras.Add(this);

			var resolutionX = (uint)((int)resolution.X).Limit(0, (int)Texture.MaximumSize);
			var resolutionY = (uint)((int)resolution.Y).Limit(0, (int)Texture.MaximumSize);
			renderTexture = new(resolutionX, resolutionY);
			Position = new();
			Resolution = new(resolutionX, resolutionY);
		}

		internal override void OnDestroy()
		{
			base.OnDestroy();

			cameras.Remove(this);
			if(Scene.CurrentScene.Textures.ContainsKey(UID))
				Scene.CurrentScene.Textures.Remove(UID, out _);
			renderTexture.Dispose();
		}
		internal override Hitbox GetBoundingBox()
		{
			return new Hitbox(
				GetCornerClockwise(0),
				GetCornerClockwise(1),
				GetCornerClockwise(2),
				GetCornerClockwise(3),
				GetCornerClockwise(0));
		}

		internal static void DrawMainCameraToWindow()
		{
			Scene.MainCamera.RenderTexture.Display();
			var texSz = Scene.MainCamera.renderTexture.Size;
			var viewSz = Game.Window.GetView().Size;
			var verts = new Vertex[]
			{
				new(-viewSz * 0.5f, new Vector2f()),
				new(new Vector2f(viewSz.X * 0.5f, -viewSz.Y * 0.5f), new Vector2f(texSz.X, 0)),
				new(viewSz * 0.5f, new Vector2f(texSz.X, texSz.Y)),
				new(new Vector2f(-viewSz.X * 0.5f, viewSz.Y * 0.5f), new Vector2f(0, texSz.Y))
			};

			Game.Window.Clear();
			Game.Window.Draw(verts, PrimitiveType.Quads, new(Scene.MainCamera.renderTexture.Texture));
			Game.Window.Display();
		}
		#endregion
	}
}
