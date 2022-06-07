namespace SMPL.Graphics
{
	internal class QuickText : Thing
	{
		internal string FontPath { get; set; }
		internal string Text { get; set; } = "Hello, World!";
		internal Color Color { get; set; } = Color.White;
		internal uint CharacterSize { get; set; } = 32;
		internal float CharacterSpace { get; set; } = 1;
		internal float LineSpace { get; set; } = 1;
		internal Color OutlineColor { get; set; } = Color.Black;
		internal float OutlineSize { get; set; } = 1;
		internal Text.Styles Style { get; set; }

		internal Vector2 OriginUnit { get; set; } = new(0.5f);

		internal QuickText(string uid, string fontPath, string text = "Hello, World!", Vector2 position = default) : base(uid)
		{
			Text = text;
			FontPath = fontPath;
			Position = position;
		}

		internal void Draw(Camera camera = null)
		{
			if(Font == null)
				return;

			camera ??= Scene.MainCamera;

			UpdateGlobalText();
			camera.renderTexture.Draw(textInstance);
		}

		internal Font Font => FontPath != null && Scene.CurrentScene.Fonts.ContainsKey(FontPath) ? Scene.CurrentScene.Fonts[FontPath] : null;
		internal static Text textInstance = new();

		internal void UpdateGlobalText()
		{
			var text = textInstance;
			text.Font = Font;
			text.CharacterSize = CharacterSize;
			text.FillColor = Color;
			text.LetterSpacing = CharacterSpace;
			text.LineSpacing = LineSpace;
			text.OutlineColor = OutlineColor;
			text.OutlineThickness = OutlineSize;
			text.Style = Style;
			text.DisplayedString = Text;
			text.Position = Position.ToSFML();
			text.Rotation = Angle;
			text.Scale = new(Scale, Scale);

			var local = text.GetLocalBounds(); // has to be after everything
			text.Origin = new(local.Width * OriginUnit.X, local.Height * OriginUnit.Y * 1.4f);
			text.Position = text.Position;
		}
	}
}
