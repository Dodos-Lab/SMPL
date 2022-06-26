namespace SMPL.Graphics
{
	internal class Text : Visual
	{
		public string FontPath { get; set; }
		public string Value { get; set; } = "Hello, World!";
		public Color Color { get; set; } = Color.White;
		public uint CharacterSize { get; set; } = 32;
		public float CharacterSpace { get; set; } = 1;
		public float LineSpace { get; set; } = 1;
		public Color OutlineColor { get; set; } = Color.Black;
		public float OutlineSize { get; set; } = 1;
		public SFML.Graphics.Text.Styles Style { get; set; }
		public Vector2 OriginUnit { get; set; } = new(0.5f);

		[JsonConstructor]
		internal Text() { }
		internal Text(string uid) : base(uid) { }

		internal static SFML.Graphics.Text textInstance = new();

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(GetFont() == null)
				return;

			var camera = Get<Camera>(CameraUID);
			if(camera != null)
				renderTarget = camera.GetRenderTexture();

			UpdateGlobalText();
			renderTarget.Draw(textInstance, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));
		}
		internal override Hitbox GetBoundingBox()
		{
			throw new NotImplementedException();
		}

		internal void UpdateGlobalText()
		{
			var text = textInstance;
			text.Font = GetFont();
			text.CharacterSize = CharacterSize;
			text.FillColor = Color;
			text.LetterSpacing = CharacterSpace;
			text.LineSpacing = LineSpace;
			text.OutlineColor = OutlineColor;
			text.OutlineThickness = OutlineSize;
			text.Style = Style;
			text.DisplayedString = Value;
			text.Position = Position.ToSFML();
			text.Rotation = Angle;
			text.Scale = new(Scale, Scale);

			var local = text.GetLocalBounds(); // has to be after everything
			text.Origin = new(local.Width * OriginUnit.X, local.Height * OriginUnit.Y * 1.4f);
			text.Position = text.Position;
		}
		internal Font GetFont()
		{
			var fonts = Scene.CurrentScene.Fonts;
			var path = FontPath.ToBackslashPath();
			return path != null && fonts.ContainsKey(path) ? fonts[path] : null;
		}
	}
}
