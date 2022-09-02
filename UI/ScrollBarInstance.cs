namespace SMPL.UI
{
	internal class ScrollBarInstance : SliderInstance
	{
		public bool IsFocused { get; set; }

		public float Step { get; set; }
		public float StepUnit
		{
			get => Step.Map(RangeA, RangeB, 0, 1);
			set => Step = value.Map(0, 1, RangeA, RangeB);
		}

		public string ButtonUpUID => scrollUpUID;
		public string ButtonDownUID => scrollDownUID;

		public void GoUp() => OnUp();
		public void GoDown() => OnDown();

		#region Backend
		[JsonProperty]
		private string scrollUpUID, scrollDownUID;

		[JsonConstructor]
		internal ScrollBarInstance() => Init();
		internal ScrollBarInstance(string uid, string buttonUpUID, string buttonDownUID) : base(uid)
		{
			scrollUpUID = buttonUpUID;
			scrollDownUID = buttonDownUID;

			var up = GetButtonUp();
			var down = GetButtonDown();
			up.ParentUID = UID;
			down.ParentUID = UID;

			Init();
		}

		private void Init()
		{
			Angle = 90;

			StepUnit = 0.1f;

			EmptyColor = new(255, 255, 255, 100);
			ProgressColor = new(255, 255, 255, 100);

			Event.ButtonClicked += OnButtonClick;
			Event.ButtonHeld += OnButtonHold;

			if(Game.Window != null)
				Game.Window.MouseWheelScrolled += OnScroll;

			TryUpdate();
		}
		private void TryUpdate()
		{
			if(IsDisabled)
				return;

			var up = GetButtonUp();
			var down = GetButtonDown();

			if(up != null)
			{
				up.LocalAngle = 0;
				up.OriginUnit = new(1, 0);
				up.LocalSize = new(LocalSize.Y);
				up.Position = BoundingBox.Lines[0].A;
				up.ParentUID = UID;
			}
			if(down != null)
			{
				down.LocalAngle = 0;
				down.OriginUnit = new(0);
				down.LocalSize = new(LocalSize.Y);
				down.Position = BoundingBox.Lines[1].A;
				down.ParentUID = UID;
			}
		}

		private void OnButtonClick(string thingUID)
		{
			TryScroll(thingUID);
		}
		private void OnButtonHold(string thingUID)
		{
			TryScroll(thingUID);
		}

		private void TryScroll(string thingUID)
		{
			if(thingUID == scrollUpUID)
				GoUp();
			else if(thingUID == scrollDownUID)
				GoDown();
		}
		protected virtual void OnUp()
		{
			if(IsDisabled)
				return;

			Value -= Step;
		}
		protected virtual void OnDown()
		{
			if(IsDisabled)
				return;

			Value += Step;
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

			TryUpdate(); // has to be after draw
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			if(Game.Window != null)
				Game.Window.MouseWheelScrolled -= OnScroll;

			Event.ButtonClicked -= OnButtonClick;
			Event.ButtonHeld -= OnButtonHold;
		}
		protected ButtonInstance GetButtonUp()
		{
			return Get<ButtonInstance>(ButtonUpUID);
		}
		protected ButtonInstance GetButtonDown()
		{
			return Get<ButtonInstance>(ButtonDownUID);
		}
		#endregion
	}
}
