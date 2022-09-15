namespace SMPL.GUI
{
	internal class ListInstance : ScrollBarInstance
	{
		public new Hitbox BoundingBox
		{
			get
			{
				var baseBB = base.BoundingBox.Lines;

				if(baseBB.Count == 0)
					return bb;

				var tl = baseBB[0].A.MoveAtAngle(Angle + 90, Size.Y, false).MoveAtAngle(Angle + 180, Size.Y, false);
				var tr = baseBB[1].A.MoveAtAngle(Angle + 90, Size.Y, false).MoveAtAngle(Angle, Size.Y, false);
				var br = tr.MoveAtAngle(Angle + 90, ItemWidth * Scale, false);
				var bl = tl.MoveAtAngle(Angle + 90, ItemWidth * Scale, false);

				bb.Lines.Clear();
				bb.LocalLines.Clear();
				bb.Lines.Add(new(tl, tr));
				bb.Lines.Add(new(tr, br));
				bb.Lines.Add(new(br, bl));
				bb.Lines.Add(new(bl, tl));

				return bb;
			}
		}

		public List<Thing.GUI.ListItem> Items { get; } = new();

		public int VisibleItemCountMax { get; set; } = 5;
		public int VisibleItemCountCurrent => Math.Min(Items.Count, VisibleItemCountMax);

		public float ItemWidth { get; set; } = 300;
		public float ItemHeight
		{
			get
			{
				var max = Math.Max(VisibleItemCountMax, 1);
				var sp = ItemSpacing;
				return (MaxLength + LocalSize.Y * 2f) / max - (sp - (sp / max));
			}
		}
		[JsonIgnore]
		public float ItemSpacing
		{
			get => spacing;
			set => spacing = value.Limit(0, MaxLength);
		}

		[JsonIgnore]
		public int ScrollIndex => scrollIndex;

		#region Backend
		[JsonProperty]
		private float spacing = 5;
		[JsonProperty]
		protected int scrollIndex;

		[JsonConstructor]
		internal ListInstance() { }
		internal ListInstance(string uid) : base(uid)
		{
			Value = 0;
		}

		protected void UpdateItemBoundingBoxes()
		{
			for(int i = 0; i < Items.Count; i++)
			{
				var item = Items[i];
				var itemBB = item.ButtonDetails.boundingBox;
				var corners = GetItemCorners(i);

				itemBB.LocalLines.Clear();
				itemBB.Lines.Clear();
				itemBB.Lines.Add(new(corners[0], corners[1]));
				itemBB.Lines.Add(new(corners[1], corners[2]));
				itemBB.Lines.Add(new(corners[2], corners[3]));
				itemBB.Lines.Add(new(corners[3], corners[0]));

				var hidden = i.IsBetween(ScrollIndex, ScrollIndex + VisibleItemCountCurrent - 1, true, true) == false;
				item.ButtonDetails.IsDisabled = hidden;
				item.ButtonDetails.IsHidden = hidden;
				item.TextDetails.IsHidden = hidden;
			}
		}
		protected virtual void UpdateDefaultValues()
		{
			VisibleItemCountMax = Math.Max(VisibleItemCountMax, 1);
			IsFocused = BoundingBox.IsHovered || base.BoundingBox.IsHovered ||
				ButtonUp.boundingBox.IsHovered || ButtonDown.boundingBox.IsHovered;

			Step = 1;
			RangeA = 0;
			RangeB = MathF.Max(Items.Count - VisibleItemCountMax, 1);

			scrollIndex = (int)Value;

			Size = new(MaxLength * Scale, Size.Y);
		}
		protected void TryItemEvents()
		{
			for(int i = 0; i < Items.Count; i++)
			{
				var item = Items[i];
				if(item.ButtonDetails.IsDisabled)
					continue;

				var itemBB = item.ButtonDetails.boundingBox;
				var buttonResult = itemBB.TryButton(isHoldable: false);

				var events = new List<(bool, Action<string, int, Thing.GUI.ListItem>)>()
				{
					(buttonResult.IsHovered, Event.ListItemHover), (buttonResult.IsUnhovered, Event.ListItemUnhover),
					(buttonResult.IsPressed, Event.ListItemPress), (buttonResult.IsReleased, Event.ListItemRelease),
					(buttonResult.IsClicked, Event.ListItemClick), (buttonResult.IsHeld, Event.ListItemHold),
				};

				for(int j = 0; j < events.Count; j++)
					if(events[j].Item1)
						events[j].Item2.Invoke(UID, i, item);
			}
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			if(IsDisabled == false)
				TryItemEvents();

			UpdateDefaultValues();
			UpdateItemBoundingBoxes();

			if(IsHidden == false)
				for(int i = 0; i < Items.Count; i++)
				{
					var item = Items[i];
					item.ButtonDetails.Draw(renderTarget);

					var itemCorners = GetItemCorners(i);
					var itemCenter = itemCorners[0].PercentToTarget(itemCorners[2], new(50));
					item.TextDetails.UpdateGlobalText(itemCenter, Angle - 90, Scale);
					item.TextDetails.Draw(renderTarget);
				}
		}
		private List<Vector2> GetItemCorners(int index)
		{
			var bb = BoundingBox.Lines;
			var tl = bb[3].A.MoveAtAngle(Angle, ((ItemHeight + ItemSpacing) * Scale) * (index - ScrollIndex), false);
			var tr = bb[0].A.MoveAtAngle(Angle, ((ItemHeight + ItemSpacing) * Scale) * (index - ScrollIndex), false);
			var br = tr.MoveAtAngle(Angle, ItemHeight * Scale, false);
			var bl = tl.MoveAtAngle(Angle, ItemHeight * Scale, false);
			return new() { tl, tr, br, bl };
		}
		#endregion
	}
}
