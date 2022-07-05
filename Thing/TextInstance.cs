namespace SMPL
{
	internal class TextInstance : VisualInstance
	{
		public string FontPath { get; set; }
		public string Value { get; set; } = "Hello, World!";
		public Color Color { get; set; } = Color.White;
		public int SymbolSize
		{
			get => symbolSize;
			set => symbolSize = value.Limit(0, 2048, Extensions.Limitation.Overflow);
		}
		public float SymbolSpace { get; set; } = 1;
		public float LineSpace { get; set; } = 1;
		public Color OutlineColor { get; set; } = Color.Black;
		public float OutlineSize { get; set; } = 1;
		public Text.Styles Style { get; set; }
		public Vector2 OriginUnit { get; set; } = new(0.5f);

		#region Backend
		private int symbolSize = 32;
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
			if(GetFont() == null)
				return base.GetBoundingBox();

			UpdateGlobalText();
			var bounds = textInstance.GetLocalBounds();
			bounds.Left -= textInstance.Origin.X;
			bounds.Top -= textInstance.Origin.Y;
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
			text.CharacterSize = (uint)SymbolSize.Limit(0, int.MaxValue);
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
