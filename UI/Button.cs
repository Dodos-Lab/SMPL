using SFML.Window;
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
      private float holdDelayTimer, holdTriggerTimer;
      private bool isClicked;

      /// <summary>
      /// The collection of methods called by this <see cref="Button"/> upon certain events.
      /// </summary>
      public delegate void ButtonEventHandler();
      /// <summary>
      /// Raised upon left clicking the <see cref="Button"/>.
      /// </summary>
      public event ButtonEventHandler Clicked;
      /// <summary>
      /// Raised upon holiding left mouse button on this <see cref="Button"/>.
      /// </summary>
      public event ButtonEventHandler Held;

      /// <summary>
      /// Whether this UI element is currently interactive.
      /// </summary>
      public bool IsDisabled { get; set; }

      public float HoldDelay { get; set; } = 0.5f;
      public float HoldTriggerSpeed { get; set; } = 0.1f;

      /// <summary>
      /// A way for the child classes of <see cref="Button"/> to raise the <see cref="Held"/> event and handle the logic around it by overriding this.
      /// </summary>
      protected virtual void OnHold()
         => Held?.Invoke();
      /// <summary>
      /// A way for the child classes of <see cref="Button"/> to raise the <see cref="Clicked"/> event and handle the logic around it by overriding this.
      /// </summary>
      protected virtual void OnClick()
         => Clicked?.Invoke();
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
		public void Trigger() => OnClick();

      /// <summary>
		/// Draws the <see cref="Button"/> on the <paramref name="camera"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/> and <see cref="Button"/> parameters.
      /// The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if <see langword="null"/>.
		/// </summary>
      public override void Draw(Camera camera = null)
      {
         Update();
         base.Draw(camera);
      }

		protected override void OnDestroy()
		{
			base.OnDestroy();
         Clicked = null;
         Held = null;
		}

		/// <summary>
		/// A way to "click" the <see cref="Button"/> through code.
		/// </summary>
      private void Update()
      {
         if (IsDisabled)
            return;

         holdDelayTimer -= Time.Delta;
         holdTriggerTimer = holdTriggerTimer < 0 ? HoldTriggerSpeed : holdTriggerTimer - Time.Delta;

         var mousePos = Scene.MouseCursorPosition;
         var hovered = Hitbox.ConvexContains(mousePos);
         var leftClicked = Mouse.IsButtonPressed(Mouse.Button.Left);
         var id = GetHashCode();

         if (holdTriggerTimer < 0 && holdDelayTimer < 0 && hovered && isClicked)
            OnHold();

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
            holdDelayTimer = HoldDelay;
            OnPress();
         }
         if ((leftClicked == false).Once($"{id}-release"))
         {
            if (hovered)
            {
               if (isClicked && holdDelayTimer > 0) // initially clicked & not holding
                  OnClick();
               OnRelease();
            }
            isClicked = false;
         }
      }
      internal void Unhover() => OnUnhover();
   }
}
