using SMPL.Core;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;
using System.Numerics;

namespace SMPL.UI
{
   /// <summary>
   /// Inherit chain: <see cref="TextButton"/> : <see cref="Button"/> : <see cref="Sprite"/> : <see cref="Visual"/> :
   /// <see cref="Object"/><br></br><br></br>
   /// 
   /// This class adds a customizable text and may use the <see cref="Button"/> events and <see cref="Sprite.Hitbox"/> to act as a labeled button
   /// or some text being a button.
   /// </summary>
   public class TextButton : Button
   {
      /// <summary>
      /// Whether this <see cref="TextButton"/> is treated as some text being a button (hyperlink) or a labeled button.
      /// The <see cref="Button"/> is resized to fit the text and <see cref="Sprite.SetDefaultHitbox"/> + <see cref="Hitbox.TransformLocalLines"/>
      /// are called upon drawing if this is <see langword="true"/> (meaning the entire text becomes the button).
      /// </summary>
      public bool IsHyperlink { get; set; }
      /// <summary>
		/// This determines the positional offset from <see cref="Object.Position"/> as a vector. [0, 0] is the top left and [1, 1]
      /// is the bottom right corner of the text. Values can also go bellow 0 and above 1.
		/// </summary>
      public Vector2 TextOriginUnit { get; set; } = new(0.5f);
      /// <summary>
      /// The customizable settings of the <see cref="TextButton"/> text.
      /// </summary>
      public Scene.TextDetails TextDetails { get; set; }

      /// <summary>
      /// Create the <see cref="TextButton"/> with some <paramref name="textDetails"/>.
      /// </summary>
      public TextButton(Scene.TextDetails textDetails)
      {
         TextDetails = textDetails;
      }

      /// <summary>
		/// Draws the <see cref="TextButton"/> on the <see cref="Visual.DrawTarget"/> according to all the required
      /// <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/>, <see cref="Button"/> and <see cref="TextButton"/> parameters.
		/// </summary>
      public override void Draw()
      {
         if (IsHyperlink)
         {
            TextDetails.UpdateGlobalText(this, TextOriginUnit);
            var b = Textbox.text.GetLocalBounds();
            Size = new(b.Width * Scale, b.Height * Scale);

            SetDefaultHitbox();
            Hitbox.TransformLocalLines(this);
         }
         base.Draw();
         Scene.DrawText(TextDetails, this, TextOriginUnit);
      }
   }
}
