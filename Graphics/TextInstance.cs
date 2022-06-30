namespace SMPL.Graphics
{
	internal class TextInstance : VisualInstance
	{
		public string FontPath { get; set; }
		public string Value { get; set; } = "Hello, World!";
		public Color Color { get; set; } = Color.White;
		public uint SymbolSize { get; set; } = 32;
		public float SymbolSpace { get; set; } = 1;
		public float LineSpace { get; set; } = 1;
		public Color OutlineColor { get; set; } = Color.Black;
		public float OutlineSize { get; set; } = 1;
		public Text.Styles Style { get; set; }
		public Vector2 OriginUnit { get; set; } = new(0.5f);

		#region Backend
		internal static Text textInstance = new();

		[JsonConstructor]
		internal TextInstance() { }
		internal TextInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(GetFont() == null)
				return;

			UpdateGlobalText();
			renderTarget.Draw(textInstance, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));
		}
		internal override Hitbox GetBoundingBox()
		{
			UpdateGlobalText();
			var bounds = textInstance.GetLocalBounds();
			var tl = new Vector2(bounds.Left, bounds.Top);
			var tr = new Vector2(bounds.Left + bounds.Width, bounds.Top);
			var br = new Vector2(bounds.Left + bounds.Width, bounds.Top + bounds.Height);
			var bl = new Vector2(bounds.Left, bounds.Top + bounds.Height);
			return new Hitbox(tl, tr, br, bl, tl);
		}

		internal void UpdateGlobalText()
		{
			var text = textInstance;
			text.Font = GetFont();
			text.CharacterSize = SymbolSize;
			text.FillColor = Color.Tint(Tint, 0.5f);
			text.LetterSpacing = SymbolSpace;
			text.LineSpacing = LineSpace;
			text.OutlineColor = OutlineColor.Tint(Tint, 0.5f);
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
		#endregion
	}
}
