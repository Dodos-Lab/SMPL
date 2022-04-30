using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SMPL.Core;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;

namespace SMPL.UI
{
   /// <summary>
   /// Inherit chain: <see cref="Inputbox"/> : <see cref="Textbox"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// This class processes text input and displays it. It is limited to a <see cref="Textbox.Alignments.TopLeft"/> alignment to avoid weird results
   /// (scrolling the text is not possible because of that). Therefore the <see cref="Inputbox"/> may be best used for short inputs that fit in the initial box.
   /// <br></br><br></br>
   /// - Note: The <see cref="Inputbox"/> is an expensive class performance-wise and it shouldn't be recreated frequently if at all.
   /// </summary>
   public class Inputbox : Textbox
   {
      private Clock cursorBlinkTimer;
      private bool cursorIsVisible;
      private uint cursorIndex;

      /// <summary>
      /// The collection of methods called by <see cref="Submitted"/>.
      /// </summary>
      public delegate void SubmitEventHandler();
      /// <summary>
      /// Triggered whenever the player submits their input with <see cref="Keyboard.Key.Enter"/>.
      /// </summary>
      public event SubmitEventHandler Submitted;

      /// <summary>
      /// Whether an <see cref="Inputbox"/> is ready to receive input. This is also set automatically upon <see cref="Mouse.Button.Left"/> click on a <see cref="Textbox"/>.
      /// </summary>
      public static bool IsFocused { get; set; }
      /// <summary>
      /// Whether this UI element is currently interactive.
      /// </summary>
      public bool IsDisabled { get; set; }

      /// <summary>
      /// The color of the blinking text cursor.
      /// </summary>
      public Color CursorColor { get; set; }
      /// <summary>
      /// The blinking speed of the text cursor.
      /// </summary>
      public float CursorBlinksPerSecond { get; set; }
      /// <summary>
      /// The index position in the text of the blinking cursor. This is automatically incremented by 1 upon typing.
      /// </summary>
      public uint CursorPositionIndex { get => cursorIndex; set => cursorIndex = (uint)((int)value).Limit(0, Text.Length); }

      /// <summary>
      /// Create the <see cref="Inputbox"/> with a certain resolution size of [<paramref name="resolutionX"/>, <paramref name="resolutionY"/>].
      /// </summary>
      public Inputbox(Font font, uint resolutionX = 300, uint resolutionY = 40) : base(font, resolutionX, resolutionY) => Init();
      /// <summary>
		/// Draws the <see cref="Inputbox"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/>, <see cref="Textbox"/> and <see cref="Inputbox"/> parameters.
		/// </summary>
		public override void Draw()
      {
         Alignment = Alignments.TopLeft;

         SetDefaultHitbox();
         Hitbox.TransformLocalLines(this);

         if (IsDisabled == false)
         {
            if (Mouse.IsButtonPressed(Mouse.Button.Left).Once($"press-{GetHashCode()}"))
            {
               IsFocused = Hitbox.ConvexContains(Scene.MouseCursorPosition);
               ShowCursor();

               var index = GetCharacterIndex(Scene.MouseCursorPosition);
               CursorPositionIndex = (uint)(index == -1 ? Text.Length : index);
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left).Once($"left-{GetHashCode()}"))
               SetIndex((int)CursorPositionIndex - 1);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right).Once($"right-{GetHashCode()}"))
               SetIndex((int)CursorPositionIndex + 1);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up).Once($"up-{GetHashCode()}"))
               SetIndex(Text.Length);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Down).Once($"down-{GetHashCode()}"))
               SetIndex(0);
            if (Keyboard.IsKeyPressed(Keyboard.Key.Delete).Once($"delete-{GetHashCode()}") && CursorPositionIndex < Text.Length)
            {
               ShowCursor();
               Text = Text.Remove((int)CursorPositionIndex, 1);
            }
         }

         base.Draw();

         if (IsFocused == false || IsDisabled)
            return;

         if (cursorBlinkTimer.ElapsedTime.AsSeconds() >= CursorBlinksPerSecond)
         {
            cursorBlinkTimer.Restart();
            cursorIsVisible = !cursorIsVisible;
         }

         if (CursorBlinksPerSecond == 0)
            cursorIsVisible = true;
         else if (CursorBlinksPerSecond < 0)
            cursorIsVisible = false;

         if (cursorIsVisible == false)
            return;

         var addLetter = Text.Length == 0 || Text[^1] == '\n';
         var isLast = CursorPositionIndex == Text.Length;

         if (addLetter)
            Text += "|";
         var corners = GetCharacterCorners(CursorPositionIndex - (uint)(isLast && addLetter == false ? 1 : 0));
         if (addLetter)
            Text = Text.Remove(Text.Length - 1);

         if (corners.Count == 0)
            return;

         var tl = corners[isLast ? 1 : 0];
         var bl = corners[isLast ? 2 : 3];
         var sz = CharacterSize * Scale * 0.1f;
         var br = bl.PointMoveAtAngle(Angle, sz, false);
         var tr = tl.PointMoveAtAngle(Angle, sz, false);

         var verts = new Vertex[]
         {
            new(tl.ToSFML(), CursorColor),
            new(bl.ToSFML(), CursorColor),
            new(br.ToSFML(), CursorColor),
            new(tr.ToSFML(), CursorColor),
         };
         DrawTarget.renderTexture.Draw(verts, PrimitiveType.Quads);

         void SetIndex(int index)
         {
            CursorPositionIndex = (uint)index;
            ShowCursor();
         }
      }

      /// <summary>
      /// A way for the child classes of <see cref="Inputbox"/> to raise the event and handle the logic around it by overriding this.
      /// </summary>
      protected void OnSubmit()
      {
         Submitted?.Invoke();
      }

      private void Init()
      {
         Game.Window.TextEntered += OnInput;
         cursorBlinkTimer = new();
         CursorBlinksPerSecond = 0.5f;
         Text = "";
         CursorColor = Color.White;
         Size = new(300, 40);
      }
      private void OnInput(object sender, TextEventArgs e)
      {
         if (IsFocused == false || Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl))
            return;

         var keyStr = e.Unicode;
         keyStr = keyStr.Replace('\r', '\n');
         ShowCursor();

         if (keyStr == "\n")
         {
            IsFocused = false;
            OnSubmit();
            return;
         }

         if (keyStr == "\b") // is backspace
         {
            if (CursorPositionIndex == 0)
               return;

            Text = Text.Remove((int)CursorPositionIndex - 1, 1);
            CursorPositionIndex--;
         }
         else
         {
            Text = Text.Insert((int)CursorPositionIndex, keyStr);
            CursorPositionIndex++;
         }
      }
      private void ShowCursor()
      {
         cursorBlinkTimer.Restart();
         cursorIsVisible = true;
      }
   }
}
