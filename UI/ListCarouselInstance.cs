namespace SMPL.UI
{
	internal class ListCarouselInstance : ListInstance
	{
		public override Hitbox BoundingBox => GetBoundingBox();

		public bool IsRepeating { get; set; } = true;
		public int SelectionIndex
		{
			get => (int)Value;
			set
			{
				UpdateDefaultValues();
				Value = value.Limit(0, GetButtonUIDs().Count, IsRepeating ? Extensions.Limitation.Overflow : Extensions.Limitation.ClosestBound);
			}
		}
		public string SelectionUID => GetButtonUIDs().Count == 0 ? null : GetButtonUIDs()[(int)Value];

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
			var btnUIDs = GetButtonUIDs();

			for(int i = 0; i < btnUIDs.Count; i++)
			{
				var btn = Get<ButtonInstance>(btnUIDs[i]);
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
			var baseBB = base.bb;
			var tl = prev == null ? baseBB.Lines[0].A : prev.BoundingBox.Lines[0].A;
			var tr = next == null ? baseBB.Lines[1].A : next.BoundingBox.Lines[1].A;
			var br = next == null ? baseBB.Lines[2].A : next.BoundingBox.Lines[2].A;
			var bl = prev == null ? baseBB.Lines[3].A : prev.BoundingBox.Lines[3].A;

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
			RangeB = GetButtonUIDs().Count - 1;
		}
		#endregion
	}
}
