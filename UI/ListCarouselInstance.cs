namespace SMPL.UI
{
	internal class ListCarouselInstance : ListInstance
	{
		public new Hitbox BoundingBox => GetBoundingBox();

		public bool SelectionIsRepeating { get; set; } = true;
		public int SelectionIndex
		{
			get => (int)Value;
			set
			{
				UpdateDefaultValues();
				Value = value.Limit(0, ButtonUIDs.Count, SelectionIsRepeating ? Extensions.Limitation.Overflow : Extensions.Limitation.ClosestBound);
			}
		}
		public string SelectionUID => ButtonUIDs.Count == 0 ? null : ButtonUIDs[(int)Value];

		#region Backend
		internal ListCarouselInstance() => Init();
		internal ListCarouselInstance(string uid, string buttonPreviousUID, string buttonNextUID) : base(uid, buttonPreviousUID, buttonNextUID)
		{
			Init();
		}
		private void Init()
		{
			Angle = 0;
		}

		protected override void OnUp()
		{
			if(IsDisabled)
				return;

			SelectionIndex++;
		}
		protected override void OnDown()
		{
			if(IsDisabled)
				return;

			SelectionIndex--;
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			IsFocused = BoundingBox.IsHovered;
			UpdateDefaultValues();

			var prev = GetButtonDown();
			var next = GetButtonUp();
			var selection = Get<ButtonInstance>(SelectionUID);

			for(int i = 0; i < ButtonUIDs.Count; i++)
			{
				var btn = Get<ButtonInstance>(ButtonUIDs[i]);
				btn.Position = new Vector2().NaN();
			}

			if(prev != null)
			{
				prev.ParentUID = UID;
				prev.OriginUnit = new(0.5f);
				prev.Size = new Vector2(ButtonHeight, ButtonHeight) * Scale;
				prev.LocalPosition = new(-ButtonWidth * 0.5f - ButtonHeight * 0.5f, 0);
			}
			if(next != null)
			{
				next.ParentUID = UID;
				next.OriginUnit = new(0.5f);
				next.Size = new Vector2(ButtonHeight, ButtonHeight) * Scale;
				next.LocalPosition = new(ButtonWidth * 0.5f + ButtonHeight * 0.5f, 0);
			}
			if(selection != null)
			{
				selection.ParentUID = UID;
				selection.Size = new Vector2(ButtonWidth, ButtonHeight) * Scale;
				selection.LocalPosition = new();
				selection.Angle = Angle;
			}
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
		}
		internal override Hitbox GetBoundingBox()
		{
			var prev = GetButtonDown();
			var next = GetButtonUp();
			var tl = prev.BoundingBox.Lines[0].A;
			var tr = next.BoundingBox.Lines[1].A;
			var br = next.BoundingBox.Lines[2].A;
			var bl = prev.BoundingBox.Lines[3].A;

			bb.Lines.Clear();
			bb.Lines.Add(new(tl, tr));
			bb.Lines.Add(new(tr, br));
			bb.Lines.Add(new(br, bl));
			bb.Lines.Add(new(bl, tl));
			return bb;
		}

		private void UpdateDefaultValues()
		{
			Step = 1;
			RangeA = 0;
			RangeB = ButtonUIDs.Count - 1;
		}
		#endregion
	}
}
