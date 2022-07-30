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

		public void GoUp()
		{
			if(IsDisabled)
				return;

			Value -= Step;
		}
		public void GoDown()
		{
			if(IsDisabled)
				return;

			Value += Step;
		}

		#region Backend
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

			EmptyColor = new(0, 0, 0, 100);
			ProgressColor = new(0, 0, 0, 100);

			Event.ButtonClicked += OnButtonClick;
			Event.ButtonHeld += OnButtonHold;

			if(Game.Window == null)
				Console.LogError(1, $"The {nameof(Game.Window)} is not yet created but this {nameof(ScrollBarInstance)} is trying to subscribe to its events.\n" +
					$"Consider creating it after the {nameof(Scene)} starts.");
			else
				Game.Window.MouseWheelScrolled += OnScroll;

			Update();
		}
		private void Update()
		{
			var up = GetButtonUp();
			var down = GetButtonDown();
			var bb = BoundingBox;

			if(up != null)
			{
				up.LocalAngle = 0;
				up.OriginUnit = new(1, 0);
				up.Size = new(Size.Y);
				up.Position = bb.Lines[0].A;
				up.ParentUID = UID;
			}
			if(down != null)
			{
				down.LocalAngle = 0;
				down.OriginUnit = new(0);
				down.Size = new(Size.Y);
				down.Position = bb.Lines[1].A;
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
			base.OnDraw(renderTarget);

			Update();

			GetButtonUp()?.Draw(renderTarget);
			GetButtonDown()?.Draw(renderTarget);
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			Game.Window.MouseWheelScrolled -= OnScroll;
			Event.ButtonClicked -= OnButtonClick;
			Event.ButtonHeld -= OnButtonHold;
		}
		internal ButtonInstance GetButtonUp()
		{
			return Get<ButtonInstance>(ButtonUpUID);
		}
		internal ButtonInstance GetButtonDown()
		{
			return Get<ButtonInstance>(ButtonDownUID);
		}
		#endregion
	}
}
