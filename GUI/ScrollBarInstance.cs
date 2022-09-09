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
			Event.ScrollBarButtonClicked += OnButtonClick;
			Event.ScrollBarButtonHeld += OnButtonHold;

			if(Game.Window != null)
				Game.Window.MouseWheelScrolled += OnScroll;
		}

		private void OnButtonClick(string uid, Thing.GUI.ScrollDirection dir)
		{
			TryScroll(uid, dir);
		}
		private void OnButtonHold(string uid, Thing.GUI.ScrollDirection dir)
		{
			TryScroll(uid, dir);
		}

		private void TryScroll(string uid, Thing.GUI.ScrollDirection dir)
		{
			if(uid != UID)
				return;

			if(dir == Thing.GUI.ScrollDirection.Up)
				GoUp();
			else if(dir == Thing.GUI.ScrollDirection.Down)
				GoDown();
		}
		protected virtual void OnUp()
		{
			if(IsDisabled)
				return;

			Value -= Step;
			Event.ScrollBarScroll(UID, Thing.GUI.ScrollDirection.Up);
		}
		protected virtual void OnDown()
		{
			if(IsDisabled)
				return;

			Value += Step;
			Event.ScrollBarScroll(UID, Thing.GUI.ScrollDirection.Down);
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

			Event.ScrollBarButtonClicked -= OnButtonClick;
			Event.ScrollBarButtonHeld -= OnButtonHold;
		}

		private void UpdateButtonBoundingBoxes()
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
		private void TryButtonEvents()
		{
			var resultUp = ButtonUp.boundingBox.TryButton();
			var resultD = ButtonDown.boundingBox.TryButton();

			var events = new List<(bool, bool, Action<string, Thing.GUI.ScrollDirection>)>()
			{
				(resultUp.IsHovered, true, Event.ScrollBarButtonHover), (resultUp.IsUnhovered, true, Event.ScrollBarButtonUnhover),
				(resultUp.IsPressed, true, Event.ScrollBarButtonPress), (resultUp.IsReleased, true, Event.ScrollBarButtonRelease),
				(resultUp.IsClicked, true, Event.ScrollBarButtonClick), (resultUp.IsHeld, true, Event.ScrollBarButtonHold),

				(resultD.IsHovered, false, Event.ScrollBarButtonHover), (resultD.IsUnhovered, false, Event.ScrollBarButtonUnhover),
				(resultD.IsPressed, false, Event.ScrollBarButtonPress), (resultD.IsReleased, false, Event.ScrollBarButtonRelease),
				(resultD.IsClicked, false, Event.ScrollBarButtonClick), (resultD.IsHeld, false, Event.ScrollBarButtonHold),
			};

			for(int i = 0; i < events.Count; i++)
				if(events[i].Item1)
					events[i].Item3.Invoke(UID, events[i].Item2 ? Thing.GUI.ScrollDirection.Up : Thing.GUI.ScrollDirection.Down);
		}
		#endregion
	}
}
