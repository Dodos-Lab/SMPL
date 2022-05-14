using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;

namespace SMPL.UI
{
   /// <summary>
   /// Inherit chain: <see cref="Tick"/> : <see cref="Button"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// The tick is just a <see cref="Button"/> that holds a <see cref="bool"/> value for whether it is ticked.
   /// </summary>
   public class Tick : Button
   {
      /// <summary>
      /// Returns whether the <see cref="Tick"/> is "ticked". This value is updated before the <see cref="Button.Clicked"/> event
      /// so that it can be used in the event logic.
      /// </summary>
      public bool IsActive { get; private set; }
      /// <summary>
      /// A way for the child classes of <see cref="Tick"/> to raise the event and handle the logic around it by overriding this.
      /// </summary>
      protected override void OnClick(Button button)
      {
         IsActive = !IsActive;
         base.OnClick(button);
      }
   }
}
