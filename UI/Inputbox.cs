using SFML.Graphics;
using System.Numerics;

namespace SMPL
{
   public class Inputbox : Textbox
   {
      /// <summary>
      /// Create the <see cref="Inputbox"/> with a certain resolution size of [<paramref name="resolutionX"/>, <paramref name="resolutionY"/>].
      /// </summary>
      public Inputbox(uint resolutionX, uint resolutionY, Font font) : base(resolutionX, resolutionY, font) => Init();
      /// <summary>
		/// Create the <see cref="Inputbox"/> with a certain <paramref name="resolution"/>.
		/// </summary>
      public Inputbox(Vector2 resolution, Font font) : base((uint)resolution.X, (uint)resolution.Y, font) => Init();

      private void Init()
      {
         Game.Window.TextEntered += OnInput;
      }

      private void OnInput(object sender, SFML.Window.TextEventArgs e)
      {

      }
   }
}
