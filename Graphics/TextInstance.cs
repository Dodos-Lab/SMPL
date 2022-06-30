namespace SMPL.Graphics
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
			UpdateGlobalText();
			if(textInstance.Font == null)
				return new Hitbox(Global(-50, -50), Global(50, -50), Global(50, 50), Global(-50, 50), Global(-50, -50));

			var bounds = textInstance.GetLocalBounds();
			bounds.Left -= textInstance.Origin.X;
			bounds.Top -= textInstance.Origin.Y;
			var tl = Global(bounds.Left, bounds.Top);
			var tr = Global(bounds.Left + bounds.Width, bounds.Top);
			var br = Global(bounds.Left + bounds.Width, bounds.Top + bounds.Height);
			var bl = Global(bounds.Left, bounds.Top + bounds.Height);
			return new Hitbox(tl, tr, br, bl, tl);

			Vector2 Global(float x, float y) => GetPositionFromSelf(new(x, y));
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
