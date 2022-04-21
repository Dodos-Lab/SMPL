using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
   /// <summary>
   /// Inherit chain: <see cref="Textbox"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// A class that draws customizable text on a hidden <see cref="Camera"/> and dispays the result on a <see cref="Sprite"/>'s <see cref="Texture"/>.
   /// This allows for things such as cutting of text (as in a chat), aligning the text in a certain rectangle, further tying it 
   /// to a <see cref="Visual"/> by having the same effects and  <see cref="Object"/> transformations etc.
   /// <br></br><br></br>
   /// - Note: The <see cref="Textbox"/> is an expensive class performance-wise and it shouldn't be recreated frequently if at all.
   /// </summary>
   public class Textbox : Sprite
   {
      /// <summary>
      /// The 9 possible text alignments: the 4 sides, the 4 corners and the center of the <see cref="Textbox"/>.
      /// </summary>
      public enum Alignments
      {
         TopLeft, Top, TopRight,
         Left, Center, Right,
         BottomLeft, Bottom, BottomRight
      }

      internal static readonly Text text = new();
      private Camera camera;
      private string left, center, right;
      private Font font;

      /// <summary>
      /// The font used to draw the text.
      /// </summary>
      public Font Font
      {
         get => font;
         set
         {
            font = value;
            Text = left; // recalculate alignments
         }
      }
      /// <summary>
      /// The <see cref="string"/> text itself used upon drawing the <see cref="Textbox"/>.
      /// </summary>
      public string Text
      {
         get => left;
         set
         {
            left = value;

            text.Font = Font;
            text.CharacterSize = CharacterSize;
            text.LetterSpacing = CharacterSpace;
            text.LineSpacing = LineSpace;
            text.Style = Style;
            text.DisplayedString = " ";
            text.Position = new();
            text.Rotation = 0;
            text.Scale = new(1, 1);

            var spaceWidth = text.GetGlobalBounds().Width;

            right = "";
            center = "";

            var lines = left.Split('\n');
            LineCount = (uint)lines.Length;
            for (int i = 0; i < lines.Length; i++)
            {
               var line = lines[i];
               text.DisplayedString = line;
               var spaces = (int)((LineWidth - text.GetGlobalBounds().Width) / spaceWidth).Limit(0, 9999).Round() - 1;

               right += $"{" ".Repeat(spaces)}{line}\n";
               center += $"{" ".Repeat(spaces / 2)}{line}{" ".Repeat(spaces / 2)}\n";
            }
         }
      }
      /// <summary>
      /// The character size as provided in the <see cref="Font"/>.
      /// </summary>
      public uint CharacterSize { get; set; } = 32;
      /// <summary>
      /// The spacing inbetween characters.
      /// </summary>
      public float CharacterSpace { get; set; } = 1;
      /// <summary>
      /// The maximum size of a line when aligning the <see cref="Text"/>. Any line that is wider than this will not be aligned.
      /// This value is set to the X of the <see cref="Textbox"/> resolution upon creation meaning that the <see cref="Text"/> will be
      /// aligned to the edges of the <see cref="Textbox"/>.
      /// </summary>
      public uint LineWidth { get; set; }
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
      public float OutlineSize { get; set; }
      /// <summary>
      /// To which side/corner/center of this <see cref="Textbox"/> should the <see cref="Text"/> stick to.
      /// </summary>
      public Alignments Alignment { get; set; }
      /// <summary>
      /// The style of the <see cref="Text"/>. May also have multiple as so:
      /// <code>Style = Styles.Bold | Styles.Underlined;</code>
      /// </summary>
      public Text.Styles Style { get; set; }
      /// <summary>
      /// The amount of lines or '\n' characters in <see cref="Text"/>.
      /// </summary>
      public uint LineCount { get; private set; }

      /// <summary>
		/// Create the <see cref="Textbox"/> with a certain resolution size of [<paramref name="resolutionX"/>, <paramref name="resolutionY"/>].
		/// </summary>
      public Textbox(uint resolutionX, uint resolutionY) => Init(resolutionX, resolutionY);
      /// <summary>
		/// Create the <see cref="Textbox"/> with a certain <paramref name="resolution"/>.
		/// </summary>
      public Textbox(Vector2 resolution) => Init((uint)resolution.X, (uint)resolution.Y);
      ~Textbox() => text.Dispose();

      /// <summary>
		/// Draws the <see cref="Textbox"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/> and <see cref="Textbox"/> parameters.
		/// </summary>
      public override void Draw()
      {
         camera.renderTexture.Clear(Color.Red);

         text.Position = new();
         text.Rotation = 0;
         text.Scale = new(1, 1);
         text.Font = Font;
         text.CharacterSize = CharacterSize;
         text.FillColor = Color;
         text.LetterSpacing = CharacterSpace;
         text.LineSpacing = LineSpace;
         text.OutlineColor = OutlineColor;
         text.OutlineThickness = OutlineSize;
         text.Style = Style;

         switch (Alignment)
         {
            case Alignments.TopLeft: Left(); break;
            case Alignments.TopRight: Right(); break;
            case Alignments.Top: CenterX(); break;
            case Alignments.Left: Left(); break;
            case Alignments.Right: Right(); break;
            case Alignments.Center: CenterX(); break;
            case Alignments.BottomLeft: Left(); break;
            case Alignments.BottomRight: Right(); break;
            case Alignments.Bottom: CenterX(); break;
         }
         var b = text.GetGlobalBounds();
         var l = text.GetLocalBounds().Top;
         var sz = camera.renderTexture.Size;
         switch (Alignment)
         {
            case Alignments.TopLeft: Top(); break;
            case Alignments.TopRight: Top(); break;
            case Alignments.Top: Top(); break;
            case Alignments.Left: CenterY(); break;
            case Alignments.Right: CenterY(); break;
            case Alignments.Center: CenterY(); break;
            case Alignments.BottomLeft: Bottom(); break;
            case Alignments.BottomRight: Bottom(); break;
            case Alignments.Bottom: Bottom(); break;
         }

         camera.renderTexture.Draw(text, new(BlendMode, Transform.Identity, Texture, Shader));
         camera.Display();
         base.Draw();

         void Top() => text.Position = new(-sz.X * 0.5f, -camera.renderTexture.Size.Y * 0.5f);
         void CenterX() => text.DisplayedString = center;
         void CenterY() => text.Position = new(-sz.X * 0.5f, -b.Height * 0.5f);
         void Bottom() => text.Position = new(-sz.X * 0.5f, camera.renderTexture.Size.Y * 0.5f - b.Height + l * 2);
         void Left() => text.DisplayedString = left;
         void Right() => text.DisplayedString = right;
      }

      private void Init(uint resolutionX, uint resolutionY)
      {
         camera = new(resolutionX, resolutionY);
         Texture = camera.Texture;
         LineWidth = resolutionX;
         left = "Hello, World!";
      }
   }
}
