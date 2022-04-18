using System.Numerics;

namespace SMPL
{
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
