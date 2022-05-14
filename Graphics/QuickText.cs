using Newtonsoft.Json;
using SFML.Graphics;
using SMPL.Tools;
using System.Numerics;

namespace SMPL.Graphics
{
	public class QuickText : Object
	{
		internal static Text textInstance = new();

		/// <summary>
		/// See <see cref="Font"/> for info.
		/// </summary>
		public string FontPath { get; set; }
		/// <summary>
		/// The <see cref="string"/> text itself used upon drawing.
		/// </summary>
		public string Text { get; set; } = "Hello, World!";
		/// <summary>
		/// The color of the text.
		/// </summary>
		public Color Color { get; set; } = Color.White;
		/// <summary>
		/// The character size as provided in the <see cref="Font"/>.
		/// </summary>
		public uint CharacterSize { get; set; } = 32;
		/// <summary>
		/// The spacing inbetween characters.
		/// </summary>
		public float CharacterSpace { get; set; } = 1;
		/// <summary>
		/// The spacing inbetween lines.
		/// </summary>
		public float LineSpace { get; set; } = 1;
		/// <summary>
		/// The color of the outline of the <see cref="Text"/>. Make sure to have non-zero <see cref="OutlineSize"/>.
		/// </summary>
		public Color OutlineColor { get; set; } = Color.Black;
		/// <summary>
		/// The size of the outline of the <see cref="Text"/>.
		/// </summary>
		public float OutlineSize { get; set; } = 1;
		/// <summary>
		/// The style of the <see cref="Text"/>. May also have multiple as so:
		/// <code>Style = Styles.Bold | Styles.Underlined;</code>
		/// </summary>
		public Text.Styles Style { get; set; }
		/// <summary>
		/// The <see cref="SFML.Graphics.Font"/> is retrieved by the <see cref="FontPath"/> from the <see cref="CurrentScene"/>'s loaded fonts and is
		/// used to draw the <see cref="Text"/>.
		/// </summary>
		[JsonIgnore]
		public Font Font => FontPath != null && Scene.CurrentScene.Fonts.ContainsKey(FontPath) ? Scene.CurrentScene.Fonts[FontPath] : null;

		public Vector2 OriginUnit { get; set; } = new(0.5f);

		/// <summary>
		/// Create the <see cref="QuickText"/> with a certain <paramref name="fontPath"/>.
		/// </summary>
		public QuickText(string fontPath, string text = "Hello, World!")
		{
			Text = text;
			FontPath = fontPath;
		}

		public void Draw(Camera camera = null)
		{
			if (Font == null)
				return;

			camera ??= Scene.MainCamera;

			UpdateGlobalText();
			camera.renderTexture.Draw(textInstance);
		}

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
			text.Origin = new(local.Width * OriginUnit.X, local.Height * OriginUnit.Y);
			text.Position = text.Position.ToSystem().PointMoveAtAngle(text.Rotation - 90, local.Top, false).ToSFML();
		}
	}
}
