using SFML.Window;
using SMPL.Core;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;

namespace SMPL.UI
{
   /// <summary>
   /// Inherit chain: <see cref="Button"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// This class is a <see cref="Sprite"/> that handles all of the logic around animating and triggering a button. A 
   /// convex <see cref="Sprite.Hitbox"/> is required for the correct results (the default one would do for the most cases). Inherit
   /// it to handle the animations from the child class (might be used like a theme for multiple <see cref="Button"/> instances).
   /// </summary>
   public class Button : Sprite
   {
      private bool isClicked;

      /// <summary>
      /// The collection of methods called by <see cref="Clicked"/>.
      /// </summary>
      public delegate void ClickedEventHandler();
      /// <summary>
      /// Raised upon triggering the <see cref="Button"/>.
      /// </summary>
      public event ClickedEventHandler Clicked;

      /// <summary>
      /// Whether this UI element is currently interactive.
      /// </summary>
      public bool IsDisabled { get; set; }

      /// <summary>
      /// A way for the child classes of <see cref="Button"/> to raise the event and handle the logic around it by overriding this.
      /// </summary>
      protected virtual void OnClick()
      {
         Clicked?.Invoke();
      }
      /// <summary>
      /// Override this to handle the logic upon hovering the <see cref="Button"/>'s <see cref="Sprite.Hitbox"/> with the mouse cursor.
      /// </summary>
      protected virtual void OnHover() { }
      /// <summary>
      /// Override this to handle the logic upon unhovering the <see cref="Button"/>'s <see cref="Sprite.Hitbox"/> with the mouse cursor.
      /// </summary>
      protected virtual void OnUnhover() { }
      /// <summary>
      /// Override this to handle the logic upon pressing the <see cref="Mouse.Button.Left"/> over the <see cref="Button"/>.
      /// </summary>
      protected virtual void OnPress() { }
      /// <summary>
      /// Override this to handle the logic upon releasing the <see cref="Mouse.Button.Left"/> over the <see cref="Button"/>.
      /// </summary>
      protected virtual void OnRelease() { }

      /// <summary>
		/// Draws the <see cref="Button"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/> and <see cref="Button"/> parameters.
		/// </summary>
      public override void Draw()
      {
         Update();
         base.Draw();
      }
      private void Update()
      {
         if (IsDisabled)
            return;

         var mousePos = Scene.MouseCursorPosition;
         var hovered = Hitbox.ConvexContains(mousePos);
         var leftClicked = Mouse.IsButtonPressed(Mouse.Button.Left);
         var id = GetHashCode();

         if (hovered.Once($"{id}-hovered"))
         {
            if (isClicked)
               OnPress();
            
            OnHover();
         }
         if ((hovered == false).Once($"{id}-unhovered"))
            OnUnhover();

         if (leftClicked.Once($"{id}-press") && hovered)
         {
            isClicked = true;
            OnPress();
         }
         if ((leftClicked == false).Once($"{id}-release"))
         {
            if (hovered)
            {
               if (isClicked)
                  OnClick();
               OnRelease();
            }
            isClicked = false;
         }
      }

      internal void Unhover() => OnUnhover();
   }
}
