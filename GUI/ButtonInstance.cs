namespace SMPL.GUI
{
	internal class ButtonInstance : SpriteInstance
	{
		public bool IsDraggable { get; set; }
		public float HoldDelay { get; set; } = 0.5f;
		public float HoldTriggerSpeed { get; set; } = 0.1f;

		public void Trigger() => Event.ButtonClick(UID);

		#region Backend

		[JsonConstructor]
		internal ButtonInstance() { }
		internal ButtonInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			if(IsDisabled == false)
				TryTriggerEvents(); // has to be after draw
		}
		private void TryTriggerEvents()
		{
			var buttonResult = BoundingBox.TryButton(HoldDelay, HoldTriggerSpeed, IsDraggable);
			var events = new List<(bool, Action<string>)>()
			{
				(buttonResult.IsDragged, Event.ButtonDrag), (buttonResult.IsDropped, Event.ButtonDrop), (buttonResult.IsHeld, Event.ButtonHold),
				(buttonResult.IsHovered, Event.ButtonHover), (buttonResult.IsUnhovered, Event.ButtonUnhover),
				(buttonResult.IsPressed, Event.ButtonPress), (buttonResult.IsReleased, Event.ButtonRelease),
				(buttonResult.IsClicked, Event.ButtonClick),
			};

			for(int i = 0; i < events.Count; i++)
				if(events[i].Item1)
					events[i].Item2.Invoke(UID);
		}

		internal void Hover() => Event.ButtonHover(UID);
		internal void Unhover() => Event.ButtonUnhover(UID);
		#endregion
	}
}
