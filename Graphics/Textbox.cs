using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
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

      public Font Font { get; set; }
      public string Text { get; set; } = "Hello, World!";
      public uint CharacterSize { get; set; } = 64;
      public float CharacterSpace { get; set; } = 1;
      public float LineSpace { get; set; } = 1;
      public Color OutlineColor { get; set; } = Color.Black;
      public float OutlineSize { get; set; }
      public Alignments Alignment { get; set; }

      public Textbox(uint resolutionX, uint resolutionY) => Init(resolutionX, resolutionY);
      public Textbox(Vector2 resolution) => Init((uint)resolution.X, (uint)resolution.Y);
      ~Textbox() => text.Dispose();

      private void Init(uint resolutionX, uint resolutionY)
      {
         camera = new(resolutionX, resolutionY);
         Texture = camera.Texture;
      }
      public override void Draw()
      {
         camera.renderTexture.Clear(Color.Red);
         text.Font = Font;
         text.DisplayedString = Text;
         text.CharacterSize = CharacterSize;
         text.FillColor = Color;
         text.LetterSpacing = CharacterSpace;
         text.LineSpacing = LineSpace;
         text.OutlineColor = OutlineColor;
         text.OutlineThickness = OutlineSize;

         var b = text.GetGlobalBounds();
         var sz = camera.renderTexture.Size;

         switch (Alignment)
         {
            case Alignments.TopLeft: break;
            case Alignments.Top:
               {
                  text.Position = new(-sz.X * 0.5f, -camera.renderTexture.Size.Y * 0.5f);
                  break;
               }
            case Alignments.TopRight: break;
            case Alignments.Left: break;
            case Alignments.Center:
               {
                  text.Position = new(-sz.X * 0.5f, -b.Height * 0.5f);
                  break;
               }
            case Alignments.Right: break;
            case Alignments.BottomLeft: break;
            case Alignments.Bottom:
               {
                  text.Position = new(-sz.X * 0.5f, camera.renderTexture.Size.Y * 0.5f - b.Height - CharacterSpace * 10f);
                  break;
               }
            case Alignments.BottomRight: break;
         }

         camera.renderTexture.Draw(text, new(BlendMode, Transform.Identity, Texture, Shader));
         camera.Display();
         base.Draw();
      }
   }
}
