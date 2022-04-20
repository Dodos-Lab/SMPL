using System.Numerics;

namespace SMPL
{
   public class HyperLink : Button
   {
      public Scene.DrawTextDetails DrawTextDetails { get; set; }

      public HyperLink(Scene.DrawTextDetails drawTextDetails)
      {
         DrawTextDetails = drawTextDetails;
         Color = SFML.Graphics.Color.Transparent;
      }

      public override void Draw()
      {
         Scene.DrawText(DrawTextDetails, this, OriginUnit);
         var b = Textbox.text.GetLocalBounds();
         var offset = new Vector2(0, b.Top);
         Size = new(b.Left + b.Width * Scale, b.Height * Scale);

         SetDefaultHitbox();
         for (int i = 0; i < Hitbox.LocalLines.Count; i++)
         {
            var line = Hitbox.LocalLines[i];
            Hitbox.LocalLines[i] = new(line.A + offset, line.B + offset);
         }

         Hitbox.TransformLocalLines(this);
         base.Draw();
      }
   }
}
