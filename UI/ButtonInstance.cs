namespace SMPL.UI
{
	internal class ButtonInstance : SpriteInstance
	{
		public bool IsDraggable { get; set; }
		public float HoldDelay { get; set; } = 0.5f;
		public float HoldTriggerSpeed { get; set; } = 0.1f;

		public void Trigger() => Event.ButtonClick(UID);

		#region Backend
		private Vector2 prevMousePos;
		private float holdDelayTimer, holdTriggerTimer;
		private bool isClicked;

		[JsonConstructor]
		internal ButtonInstance() { }
		internal ButtonInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			TryUpdate();

			if(IsHidden == false)
				base.OnDraw(renderTarget);
		}
		private void TryUpdate()
		{
			if(IsDisabled)
				return;

			holdDelayTimer -= Time.Delta;
			holdTriggerTimer = holdTriggerTimer < 0 ? HoldTriggerSpeed : holdTriggerTimer - Time.Delta;

			var hovered = BoundingBox.IsHovered;
			var leftClicked = Mouse.IsButtonPressed(Mouse.Button.Left);
			var id = GetHashCode();

			if(holdTriggerTimer < 0 && holdDelayTimer < 0 && hovered && isClicked)
				Event.ButtonHold(UID);
			if(hovered.Once($"{id}-hovered"))
			{
				if(isClicked)
					Event.ButtonPress(UID);

				Event.ButtonHover(UID);
			}
			if((hovered == false).Once($"{id}-unhovered"))
				Event.ButtonUnhover(UID);

			if(leftClicked.Once($"{id}-press") && hovered)
			{
				isClicked = true;
				holdDelayTimer = HoldDelay;
				Event.ButtonPress(UID);

				if(IsDraggable)
					Event.ButtonDrag(UID);
			}
			if((leftClicked == false).Once($"{id}-release"))
			{
				if(hovered)
				{
					if(isClicked)
						Event.ButtonClick(UID);

					if(isClicked && IsDraggable)
						Event.ButtonDrop(UID);

					Event.ButtonRelease(UID);
					Event.ButtonHover(UID);
				}
				isClicked = false;
			}

			var mousePos = Scene.MouseCursorPosition;
			if(isClicked && IsDraggable)
			{
				var delta = mousePos - prevMousePos;
				Position += delta;
			}
			prevMousePos = mousePos;
		}

		internal void Hover() => Event.ButtonHover(UID);
		internal void Unhover() => Event.ButtonUnhover(UID);
		#endregion
	}
}
