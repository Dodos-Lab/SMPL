using System.Numerics;

namespace SMPL
{
   /// <summary>
   /// Inherit chain: <see cref="Window"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// <br></br><br></br>
	/// - Note: The <see cref="Window"/> is an expensive class performance-wise and it shouldn't be recreated frequently if at all.
   /// </summary>
   public class Window : Sprite
   {
      public Camera Camera { get; private set; }

      public Window(Vector2 resolution) => Init((uint)resolution.X, (uint)resolution.Y);
      public Window(uint resolutionX, uint resolutionY) => Init(resolutionX, resolutionY);

      private void Init(uint resolutionX, uint resolutionY)
      {
         Camera = new(resolutionX, resolutionY);
         Texture = Camera.Texture;
      }
   }
}
