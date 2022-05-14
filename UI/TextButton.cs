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
      
      public QuickText QuickText { get; set; }

      /// <summary>
      /// Create the <see cref="TextButton"/> with some <paramref name="quickText"/>.
      /// </summary>
      public TextButton(QuickText quickText)
      {
         Size = new(250, 60);
         QuickText = quickText;
         QuickText.Parent = this;
      }

      /// <summary>
		/// Draws the <see cref="TextButton"/> on the <paramref name="camera"/> according to all the required
      /// <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/>, <see cref="Button"/> and <see cref="TextButton"/> parameters.
      /// The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if <see langword="null"/>.
		/// </summary>
      public override void Draw(Camera camera = null)
      {
         if (QuickText != null)
            QuickText?.UpdateGlobalText();
         if (IsHyperlink)
         {
            var b = QuickText.textInstance.GetLocalBounds();
            Size = new(b.Width * Scale, b.Height * Scale);

            SetDefaultHitbox();
            Hitbox.TransformLocalLines(this);
         }
         base.Draw(camera);

         if (IsHidden == false)
            QuickText.Draw(camera);
      }

		protected override void OnDestroy()
		{
			base.OnDestroy();
         QuickText.Destroy();
         QuickText = null;
		}
	}
}
