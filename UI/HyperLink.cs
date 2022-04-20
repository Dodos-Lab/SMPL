using System.Numerics;

namespace SMPL
{
   /// <summary>
   /// Inherit chain: <see cref="HyperLink"/> : <see cref="Button"/> : <see cref="Sprite"/> : <see cref="Visual"/> :
   /// <see cref="Object"/><br></br><br></br>
   /// 
   /// This class may use the <see cref="Button"/> events and <see cref="Sprite.Hitbox"/> to act as a hyperlink. 
   /// It also adds a customizable text to achieve that.
   /// </summary>
   public class HyperLink : Button
   {
      /// <summary>
      /// The customizable settings of the <see cref="HyperLink"/> text.
      /// </summary>
      public Scene.TextDetails TextDetails { get; set; }

      /// <summary>
      /// Create the <see cref="HyperLink"/> with some <paramref name="textDetails"/>.
      /// </summary>
      public HyperLink(Scene.TextDetails textDetails)
      {
         TextDetails = textDetails;
         Color = SFML.Graphics.Color.Transparent;
      }

      /// <summary>
		/// Draws the <see cref="HyperLink"/> on the <see cref="Visual.DrawTarget"/> according to all the required
      /// <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/>, <see cref="Button"/> and <see cref="HyperLink"/> parameters.
		/// </summary>
      public override void Draw()
      {
         base.Draw();
         Scene.DrawText(TextDetails, this, OriginUnit);
         var b = Textbox.text.GetLocalBounds();
         Size = new(b.Width * Scale, b.Height * Scale);

         SetDefaultHitbox();
         Hitbox.TransformLocalLines(this);
      }
   }
}
