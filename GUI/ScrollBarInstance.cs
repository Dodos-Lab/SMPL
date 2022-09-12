namespace SMPL.GUI
{
	internal class ScrollBarInstance : SliderInstance
	{
		public new Hitbox BoundingBox
		{
			get
			{
				var prev = Value;
				Value = RangeB;
				var baseBB = base.BoundingBox.LocalLines;

				var tl = baseBB[0].A;
				var tr = baseBB[1].A;
				var br = baseBB[2].A;
				var bl = baseBB[3].A;

				bb.Lines.Clear();
				bb.LocalLines.Clear();
				bb.LocalLines.Add(new(tl, tr));
				bb.LocalLines.Add(new(tr, br));
				bb.LocalLines.Add(new(br, bl));
				bb.LocalLines.Add(new(bl, tl));
				bb.TransformLocalLines(UID);

				Value = prev;
				return bb;
			}
		}

		public Thing.GUI.ButtonDetails ButtonUp { get; } = new();
		public Thing.GUI.ButtonDetails ButtonDown { get; } = new();

		public bool IsFocused { get; set; }

		public float Step { get; set; }
		public float StepUnit
		{
			get => Step.Map(RangeA, RangeB, 0, 1);
			set => Step = value.Map(0, 1, RangeA, RangeB);
		}

		public void GoUp() => OnUp();
		public void GoDown() => OnDown();

		#region Backend
		[JsonConstructor]
		internal ScrollBarInstance() => Init();
		internal ScrollBarInstance(string uid) : base(uid)
		{
			Angle = 90;
			StepUnit = 0.1f;

			EmptyColor = new(255, 255, 255, 100);
			ProgressColor = new(255, 255, 255, 100);

			Init();
		}

		private void Init()
		{
			Event.ButtonClicked += OnButtonClick;
			Event.ButtonHeld += OnButtonHold;

			if(Game.Window != null)
				Game.Window.MouseWheelScrolled += OnScroll;
		}

		private void OnButtonClick(string uid, Thing.GUI.ButtonDetails btn)
		{
			TryScroll(uid, btn);
		}
		private void OnButtonHold(string uid, Thing.GUI.ButtonDetails btn)
		{
			TryScroll(uid, btn);
		}

		private void TryScroll(string uid, Thing.GUI.ButtonDetails btn)
		{
			if(uid != UID)
				return;

			if(btn == ButtonUp)
				GoUp();
			else if(btn == ButtonDown)
				GoDown();
		}
		protected virtual void OnUp()
		{
			if(IsDisabled)
				return;

			Value -= Step;
			Event.ScrollBarMove(UID);
		}
		protected virtual void OnDown()
		{
			if(IsDisabled)
				return;

			Value += Step;
			Event.ScrollBarMove(UID);
		}

		internal void OnScroll(object sender, MouseWheelScrollEventArgs e)
		{
			if(IsFocused == false || IsDisabled)
				return;

			if(e.Delta < 0)
				GoDown();
			else
				GoUp();
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			UpdateButtonBoundingBoxes();
			if(IsDisabled == false)
				TryButtonEvents();

			if(IsHidden == false)
			{
				ButtonUp.Draw(renderTarget);
				ButtonDown.Draw(renderTarget);
			}
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			if(Game.Window != null)
				Game.Window.MouseWheelScrolled -= OnScroll;

			Event.ButtonClicked -= OnButtonClick;
			Event.ButtonHeld -= OnButtonHold;
		}

		protected void UpdateButtonBoundingBoxes()
		{
			var bbUp = ButtonUp.boundingBox;
			var bbD = ButtonDown.boundingBox;
			var local = BoundingBox.LocalLines;

			bbUp.Lines.Clear();
			bbUp.LocalLines.Clear();
			bbD.Lines.Clear();
			bbD.LocalLines.Clear();

			var sz = LocalSize.Y;
			var a = local[0].A;
			var b = local[1].A;
			var c = local[2].A;
			var d = local[3].A;
			bbUp.LocalLines.Add(new(a - new Vector2(sz, 0), a));
			bbUp.LocalLines.Add(new(a, d));
			bbUp.LocalLines.Add(new(d, d - new Vector2(sz, 0)));
			bbUp.LocalLines.Add(new(bbUp.LocalLines[2].B, bbUp.LocalLines[0].A));
			bbUp.TransformLocalLines(UID);

			bbD.LocalLines.Add(new(b, b + new Vector2(sz, 0)));
			bbD.LocalLines.Add(new(bbD.LocalLines[0].B, bbD.LocalLines[0].B + new Vector2(0, sz)));
			bbD.LocalLines.Add(new(bbD.LocalLines[1].B, c));
			bbD.LocalLines.Add(new(c, b));
			bbD.TransformLocalLines(UID);
		}
		protected virtual void TryButtonEvents()
		{
			TryProcessButton(ButtonUp);
			TryProcessButton(ButtonDown);

			void TryProcessButton(Thing.GUI.ButtonDetails btn)
			{
				if(btn.IsDisabled)
					return;

				var result = btn.boundingBox.TryButton();
				var events = new List<(bool, Action<string, Thing.GUI.ButtonDetails>)>()
				{
					(result.IsHovered, Event.ButtonHover), (result.IsUnhovered, Event.ButtonUnhover),
					(result.IsPressed, Event.ButtonPress), (result.IsReleased, Event.ButtonRelease),
					(result.IsClicked, Event.ButtonClick), (result.IsHeld, Event.ButtonHold),
				};

				for(int i = 0; i < events.Count; i++)
					if(events[i].Item1)
						events[i].Item2.Invoke(UID, btn);
			}
		}
		#endregion
	}
}
