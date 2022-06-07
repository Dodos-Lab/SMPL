namespace SMPL.Graphics
{
	internal class Camera : Thing
	{
		internal RenderTexture RenderTexture => renderTexture;

		internal new Vector2 Position
		{
			get => renderTexture.GetView().Center.ToSystem();
			set
			{
				var view = renderTexture.GetView();
				base.Position = value;
				view.Center = value.ToSFML();
				renderTexture.SetView(view);
			}
		}
		internal new float Angle
		{
			get => renderTexture.GetView().Rotation;
			set
			{
				var view = renderTexture.GetView();
				view.Rotation = value;
				base.Angle = value;
				renderTexture.SetView(view);
			}
		}
		internal new float Scale
		{
			get => scale;
			set
			{
				scale = value;
				var view = renderTexture.GetView();
				view.Size = new Vector2f(renderTexture.Size.X, renderTexture.Size.Y) * scale;
				base.Scale = scale;
				renderTexture.SetView(view);
			}
		}
		internal Vector2 Resolution { get; private set; }
		internal bool IsSmooth { get => renderTexture.Smooth; set => renderTexture.Smooth = value; }
		[JsonIgnore]
		internal Texture Texture => renderTexture.Texture;
		internal Vector2 MouseCursorPosition
		{
			get { var p = Mouse.GetPosition(Game.Window); return PointToCamera(new(p.X, p.Y)); }
			set { var p = PointToWorld(value); Mouse.SetPosition(new((int)p.X, (int)p.Y), Game.Window); }
		}

		internal Camera(string uid, Vector2 resolution) : base(uid) =>
			Init((uint)resolution.X.Limit(0, Texture.MaximumSize), (uint)resolution.Y.Limit(0, Texture.MaximumSize));
		internal Camera(string uid, uint resolutionX, uint resolutionY) : base(uid) =>
			Init(resolutionX, resolutionY);

		internal override void OnDestroy()
			=> renderTexture.Dispose();

		internal bool Captures(Hitbox hitbox)
		{
			return GetScreenHitbox().ConvexContains(hitbox);
		}
		internal bool Captures(Vector2 point)
		{
			return GetScreenHitbox().ConvexContains(point);
		}
		internal void Snap(string imagePath)
		{
			var img = Texture.CopyToImage();
			var result = img.SaveToFile(imagePath);
			img.Dispose();

			if(result == false)
				Console.LogError(1, $"Could not save the image at '{imagePath}'.");
		}
		internal void Fill(Color color = default)
		{
			renderTexture.Clear(color);
		}
		internal void Display()
		{
			renderTexture.Display();
		}
		public override Vector2 CornerClockwise(int index)
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

		private float scale = 1;
		internal RenderTexture renderTexture;

		internal Vector2 PointToCamera(Vector2 worldPoint)
		{
			return Game.Window.MapPixelToCoords(new((int)worldPoint.X, (int)worldPoint.Y), renderTexture.GetView()).ToSystem();
		}
		internal Vector2 PointToWorld(Vector2 cameraPoint)
		{
			var p = Game.Window.MapCoordsToPixel(cameraPoint.ToSFML(), renderTexture.GetView());
			return new(p.X, p.Y);
		}

		private Hitbox GetScreenHitbox()
		{
			return new Hitbox(CornerClockwise(0), CornerClockwise(1), CornerClockwise(2), CornerClockwise(3), CornerClockwise(4));
		}
		private void Init(uint resolutionX, uint resolutionY)
		{
			resolutionX = (uint)((int)resolutionX).Limit(0, (int)Texture.MaximumSize);
			resolutionY = (uint)((int)resolutionY).Limit(0, (int)Texture.MaximumSize);
			renderTexture = new(resolutionX, resolutionY);
			Position = new();
			Resolution = new(resolutionX, resolutionY);
		}
		internal static void DrawMainCameraToWindow()
		{
			Scene.MainCamera.Display();
			var texSz = Scene.MainCamera.renderTexture.Size;
			var viewSz = Game.Window.GetView().Size;
			var verts = new Vertex[]
			{
				new(-viewSz * 0.5f, new Vector2f()),
				new(new Vector2f(viewSz.X * 0.5f, -viewSz.Y * 0.5f), new Vector2f(texSz.X, 0)),
				new(viewSz * 0.5f, new Vector2f(texSz.X, texSz.Y)),
				new(new Vector2f(-viewSz.X * 0.5f, viewSz.Y * 0.5f), new Vector2f(0, texSz.Y))
			};

			Game.Window.Draw(verts, PrimitiveType.Quads, new(Scene.MainCamera.Texture));
		}
	}
}
