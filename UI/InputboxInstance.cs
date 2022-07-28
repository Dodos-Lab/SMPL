namespace SMPL.UI
{
	internal class InputboxInstance : TextboxInstance
	{
		public static bool IsFocused { get; set; }
		public bool IsDisabled { get; set; }

		public Color CursorColor { get; set; }
		public float CursorBlinkSpeed { get; set; }
		public int CursorPositionIndex
		{
			get => cursorIndex;
			set => cursorIndex = value.Limit(0, Value.Length);
		}

		public string PlaceholderValue { get; set; } = "Type...";
		public Color PlaceholderColor { get; set; } = new(255, 255, 255, 70);

		public void Submit()
		{
			Event.InputboxSubmit(UID);
		}

		#region Backend
		private Clock cursorBlinkTimer;
		private bool cursorIsVisible;
		private int cursorIndex;
		private float cursorPosX;

		[JsonConstructor]
		internal InputboxInstance() => Init();
		internal InputboxInstance(string uid, string fontPath, uint resolutionX = 300, uint resolutionY = 40) : base(uid, fontPath, resolutionX, resolutionY)
		{
			Init();
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			Alignment = Thing.TextboxAlignment.TopLeft;

			camera.RenderTexture.Clear(BackgroundColor);

			TryInput();
			TryDrawPlaceholder();
			base.OnDraw(renderTarget);
			TryDrawCursor();

			TryMoveTextWhenCursorOut();

			DrawTextbox(renderTarget);
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			cursorBlinkTimer.Dispose();
			cursorBlinkTimer = null;
			Game.Window.TextEntered -= OnInput;
		}

		private void Init()
		{
			Game.Window.TextEntered += OnInput;
			cursorBlinkTimer = new();
			CursorBlinkSpeed = 0.5f;
			Value = "";
			CursorColor = Color.White;
			skipParentRender = true;
		}
		private void OnInput(object sender, TextEventArgs e)
		{
			if(IsFocused == false || Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl))
				return;

			var keyStr = e.Unicode;
			keyStr = keyStr.Replace('\r', '\n');
			ShowCursor();

			if(keyStr == "\n")
			{
				IsFocused = false;
				Submit();
				return;
			}

			if(keyStr == "\b") // is backspace
			{
				if(CursorPositionIndex == 0)
					return;

				Value = Value.Remove((int)CursorPositionIndex - 1, 1);
				CursorPositionIndex--;
			}
			else
			{
				Value = Value.Insert((int)CursorPositionIndex, keyStr);
				CursorPositionIndex++;
			}
		}
		private void ShowCursor()
		{
			cursorBlinkTimer.Restart();
			cursorIsVisible = true;
		}

		private void TryDrawPlaceholder()
		{
			if(Value.Length != 0)
				return;

			Update();
			textInstance.OutlineThickness = 0;
			textInstance.FillColor = PlaceholderColor;
			textInstance.DisplayedString = "  " + PlaceholderValue;

			camera.RenderTexture.Draw(textInstance, new(SFML.Graphics.BlendMode.Alpha));
		}
		private void TryInput()
		{
			if(IsDisabled || Game.Window.HasFocus() == false)
				return;

			if(Mouse.IsButtonPressed(Mouse.Button.Left).Once($"press-{GetHashCode()}"))
			{
				IsFocused = BoundingBox.IsHovered;
				ShowCursor();

				var index = GetSymbolIndex(Scene.MouseCursorPosition);
				CursorPositionIndex = index == -1 ? Value.Length : index;
			}
			if(Keyboard.IsKeyPressed(Keyboard.Key.Left).Once($"left-{GetHashCode()}"))
				SetIndex((int)CursorPositionIndex - 1);
			if(Keyboard.IsKeyPressed(Keyboard.Key.Right).Once($"right-{GetHashCode()}"))
				SetIndex((int)CursorPositionIndex + 1);
			if(Keyboard.IsKeyPressed(Keyboard.Key.Up).Once($"up-{GetHashCode()}"))
				SetIndex(Value.Length);
			if(Keyboard.IsKeyPressed(Keyboard.Key.Down).Once($"down-{GetHashCode()}"))
				SetIndex(0);
			if(Keyboard.IsKeyPressed(Keyboard.Key.Delete).Once($"delete-{GetHashCode()}") && CursorPositionIndex < Value.Length)
			{
				ShowCursor();
				Value = Value.Remove((int)CursorPositionIndex, 1);
			}

			void SetIndex(int index)
			{
				CursorPositionIndex = index;
				ShowCursor();
			}
		}
		private void TryDrawCursor()
		{
			if(IsFocused == false || IsDisabled)
				return;

			if(cursorBlinkTimer.ElapsedTime.AsSeconds() >= CursorBlinkSpeed)
			{
				cursorBlinkTimer.Restart();
				cursorIsVisible = !cursorIsVisible;
			}

			if(CursorBlinkSpeed == 0)
				cursorIsVisible = true;
			else if(CursorBlinkSpeed < 0)
				cursorIsVisible = false;

			if(cursorIsVisible == false)
				return;

			var addLetter = Value.Length == 0 || Value[^1] == '\n';
			var isLast = CursorPositionIndex == Value.Length;

			if(addLetter)
				Value += "|";

			var corners = GetSymbolCorners(CursorPositionIndex - (isLast && addLetter == false ? 1 : 0));
			if(corners.Count == 0)
				return;

			if(addLetter)
				Value = Value.Remove(Value.Length - 1);

			var tl = corners[isLast ? 1 : 0];
			var bl = corners[isLast ? 2 : 3];
			var sz = SymbolSize * Scale * 0.05f;
			var br = bl.PointMoveAtAngle(Angle, sz, false);
			var tr = tl.PointMoveAtAngle(Angle, sz, false);

			cursorPosX = tl.X * Scale;

			var verts = new Vertex[]
			{
				new(tl.ToSFML(), CursorColor),
				new(bl.ToSFML(), CursorColor),
				new(br.ToSFML(), CursorColor),
				new(tr.ToSFML(), CursorColor),
			};
			camera.RenderTexture.Draw(verts, PrimitiveType.Quads);
		}
		private void TryMoveTextWhenCursorOut()
		{
			if(cursorPosX >= Resolution.X)
				textOffsetX -= SymbolSize * 0.3f;
			else if(cursorPosX <= -Resolution.X)
				textOffsetX += SymbolSize * 0.3f;
		}
		#endregion
	}
}
