using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
   /// <summary>
   /// Inherit chain: <see cref="Textbox"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// <br></br><br></br>
   /// - Note: The <see cref="Textbox"/> is an expensive class performance-wise and it shouldn't be recreated frequently if at all.
   /// </summary>
   public class Textbox : Sprite
   {
      public enum Alignments
      {
         TopLeft, Top, TopRight,
         Left, Center, Right,
         BottomLeft, Bottom, BottomRight
      }

      private static readonly Text text = new();
      private Camera camera;
      private string left = "Hello, World!", center = "", right = "";

      public Font Font { get; set; }
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
      public uint CharacterSize { get; set; } = 32;
      public float CharacterSpace { get; set; } = 1;
      public uint LineWidth { get; set; }
      public float LineSpace { get; set; } = 1;
      public Color OutlineColor { get; set; } = Color.Black;
      public float OutlineSize { get; set; }
      public Alignments Alignment { get; set; }
      public Text.Styles Style { get; set; }
      public uint LineCount { get; private set; }

      public Textbox(uint resolutionX, uint resolutionY) => Init(resolutionX, resolutionY);
      public Textbox(Vector2 resolution) => Init((uint)resolution.X, (uint)resolution.Y);
      ~Textbox() => text.Dispose();

      private void Init(uint resolutionX, uint resolutionY)
      {
         camera = new(resolutionX, resolutionY);
         Texture = camera.Texture;
         LineWidth = resolutionX;
      }
      public override void Draw()
      {
         camera.renderTexture.Clear(Color.Red);
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
         void Bottom() => text.Position = new(-sz.X * 0.5f, camera.renderTexture.Size.Y * 0.5f - b.Height);
         void Left() => text.DisplayedString = left;
         void Right() => text.DisplayedString = right;
      }
   }
}
