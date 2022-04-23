using SFML.Graphics;
using System.Collections.Generic;
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
      private readonly List<(int, int)> formatSpaceRangesRight = new(), formatSpaceRangesCenter = new();
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
            text.Position = new();
            text.Rotation = 0;
            text.Scale = new(1, 1);

            text.DisplayedString = " ";
            var spaceWidth = text.GetGlobalBounds().Width;

            formatSpaceRangesCenter.Clear();
            formatSpaceRangesRight.Clear();
            right = "";
            center = "";

            var lines = left.Split('\n');
            LineCount = (uint)lines.Length;
            for (int i = 0; i < lines.Length; i++)
            {
               var line = lines[i];
               text.DisplayedString = line;
               var width = LineWidth - text.GetGlobalBounds().Width;
               var spaces = spaceWidth == 0 ? 0 : (int)(width / spaceWidth).Limit(0, 9999).Round() - 1;
               var centerLeft = (int)(((float)spaces) / 2).Round(priority: Extensions.RoundWhenMiddle.TowardZero);
               var centerRight = (int)(((float)spaces) / 2).Round(priority: Extensions.RoundWhenMiddle.AwayFromZero);

               formatSpaceRangesRight.Add((right.Length, right.Length - 1 + spaces));
               right += $"{" ".Repeat(spaces)}{line}\n";

               formatSpaceRangesCenter.Add((center.Length, center.Length - 1 + centerLeft));
               center += $"{" ".Repeat(centerLeft)}{line}";
               formatSpaceRangesCenter.Add((center.Length, center.Length - 1 + centerRight));
               center += $"{" ".Repeat(centerRight)}\n";
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
      /// The size of the "window" the <see cref="Text"/> is "viewed" through.
      /// </summary>
      public Vector2 Resolution => new(camera.renderTexture.Size.X, camera.renderTexture.Size.Y);

      /// <summary>
		/// Create the <see cref="Textbox"/> with a certain resolution size of [<paramref name="resolutionX"/>, <paramref name="resolutionY"/>]
      /// and a <paramref name="font"/>.
		/// </summary>
      public Textbox(uint resolutionX, uint resolutionY, Font font) => Init(resolutionX, resolutionY, font);
      /// <summary>
		/// Create the <see cref="Textbox"/> with a certain <paramref name="resolution"/> and a <paramref name="font"/>.
		/// </summary>
      public Textbox(Vector2 resolution, Font font) => Init((uint)resolution.X, (uint)resolution.Y, font);
      ~Textbox() => text.Dispose();

      /// <summary>
		/// Draws the <see cref="Textbox"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/> and <see cref="Textbox"/> parameters.
		/// </summary>
      public override void Draw()
      {
         camera.renderTexture.Clear(Color.Red);
         Update();
         camera.renderTexture.Draw(text, new(BlendMode, Transform.Identity, Texture, Shader));
         camera.Display();
         base.Draw();
      }

      public List<Vector2> GetCharacterCorners(uint characterIndex)
      {
         var result = new List<Vector2>();
         Update();
         var prevIndex = (int)characterIndex;
         characterIndex = GetNextNonFormatChar(characterIndex);

         var resScale = Size / Resolution;
         var tl = (text.Position.ToSystem() + text.FindCharacterPos(characterIndex).ToSystem()) * resScale;
         var tr = (text.Position.ToSystem() + text.FindCharacterPos(characterIndex + 1).ToSystem()) * resScale;

         if (prevIndex < left.Length && left[prevIndex] == '\n') // end of line
            tr = new(Resolution.X, tl.Y);

         var y = tl.PointMoveAtAngle(90, CharacterSize * resScale.Y, false).Y;
         var boundTop = text.GetLocalBounds().Top;
         var bl = new Vector2(tl.X, y + boundTop);
         var br = new Vector2(tr.X, y + boundTop);

         tl.Y += boundTop;
         tr.Y += boundTop;

         result.Add(tl);
         result.Add(tr);
         result.Add(br);
         result.Add(bl);

         return result;
      }

      private void Init(uint resolutionX, uint resolutionY, Font font)
      {
         camera = new(resolutionX, resolutionY);
         Texture = camera.Texture;
         LineWidth = resolutionX;
         Alignment = Alignments.Center;
         left = "Hello, World!";
         Font = font;
      }
      private void Update()
      {
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

         void Top() => text.Position = new(-sz.X * 0.5f, -camera.renderTexture.Size.Y * 0.5f - l * 0.5f);
         void CenterX() => text.DisplayedString = center;
         void CenterY() => text.Position = new(-sz.X * 0.5f, -b.Height * 0.5f - (Alignment == Alignments.Left ? l : 0));
         void Bottom() => text.Position = new(-sz.X * 0.5f, camera.renderTexture.Size.Y * 0.5f - b.Height + l);
         void Left() => text.DisplayedString = left;
         void Right() => text.DisplayedString = right;
      }
      private uint GetNextNonFormatChar(uint charIndex)
      {
         if (charIndex > left.Length - 1)
            charIndex = (uint)left.Length - 1;

         if (Alignment == Alignments.TopRight || Alignment == Alignments.Right || Alignment == Alignments.BottomRight)
            return Execute(formatSpaceRangesRight, true);
         else if (Alignment == Alignments.Top || Alignment == Alignments.Center || Alignment == Alignments.Bottom)
            return Execute(formatSpaceRangesCenter);

         return charIndex;

         uint Execute(List<(int, int)> ranges, bool isRight = false)
         {
            var realIndex = (uint)0;
            for (uint i = 0; i < text.DisplayedString.Length; i++)
            {
               var lastRange = (0, 0);
               for (int j = 0; j < ranges.Count; j++)
                  if (((int)i).IsBetween(ranges[j].Item1, ranges[j].Item2, true, true))
                  {
                     i = (uint)ranges[j].Item2 + 1;
                     lastRange = ranges[j];
                     break;
                  }
               if (realIndex == charIndex)
                  return text.DisplayedString[(int)i] == '\n' && isRight == false ? (uint)lastRange.Item1 : i;

               realIndex++;
            }
            return charIndex;
         }
      }
   }
}
