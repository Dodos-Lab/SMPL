namespace SMPL.GUI
{
	internal class ListCarouselInstance : ListInstance
	{
		public new Hitbox BoundingBox
		{
			get
			{
				UpdateButtonBoundingBoxes();
				var upBB = ButtonUp.boundingBox.Lines;
				var dBB = ButtonDown.boundingBox.Lines;
				bb.LocalLines.Clear();
				bb.Lines.Clear();
				bb.Lines.Add(new(upBB[0].A, dBB[1].A));
				bb.Lines.Add(new(dBB[1].A, dBB[2].A));
				bb.Lines.Add(new(dBB[2].A, upBB[3].A));
				bb.Lines.Add(new(upBB[3].A, upBB[0].A));
				return bb;
			}
		}

		public bool IsRepeating { get; set; } = true;
		public int SelectionIndex
		{
			get => (int)Value;
			set
			{
				UpdateDefaultValues();
				var prev = (int)Value;
				Value = value.Limit(0, Items.Count, IsRepeating ? Extensions.Limitation.Overflow : Extensions.Limitation.ClosestBound);
				Value = MathF.Max(Value, 0);

				if(Value != prev)
				{
					Event.ListItemDeselect(UID, prev, Items[prev]);
					Event.ListItemSelect(UID, SelectionIndex, SelectedItem);
				}
			}
		}
		public Thing.GUI.ListItem SelectedItem => Items.Count == 0 ? null : Items[(int)Value];

		#region Backend
		internal ListCarouselInstance() { }
		internal ListCarouselInstance(string uid) : base(uid)
		{
			Angle = 0;
			Size = new(Size.X, Size.Y * 2f);
		}

		protected override void OnUp()
		{
			if(IsDisabled || Items.Count < 2)
				return;

			SelectionIndex--;
		}
		protected override void OnDown()
		{
			if(IsDisabled || Items.Count < 2)
				return;

			SelectionIndex++;
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsDisabled)
			{
				IsFocused = false;
				return;
			}

			IsFocused = BoundingBox.IsHovered;

			UpdateButtonBoundingBoxes();
			TryButtonEvents();

			if(ButtonUp.IsHidden == false)
				ButtonUp.Draw(renderTarget);
			if(ButtonDown.IsHidden == false)
				ButtonDown.Draw(renderTarget);

			var upBB = ButtonUp.boundingBox.Lines;
			var downBB = ButtonDown.boundingBox.Lines;

			var sel = SelectedItem;
			if(sel == null)
				return;

			var selBB = sel.ButtonDetails.boundingBox;
			selBB.Lines.Clear();
			selBB.LocalLines.Clear();
			selBB.Lines.Add(new(upBB[1].A, downBB[0].A));
			selBB.Lines.Add(new(downBB[0].A, downBB[3].A));
			selBB.Lines.Add(new(downBB[3].A, upBB[2].A));
			selBB.Lines.Add(new(upBB[2].A, upBB[1].A));

			if(sel.ButtonDetails.IsHidden == false)
				sel.ButtonDetails.Draw(renderTarget);
			if(sel.TextDetails.IsHidden == false)
			{
				sel.TextDetails.UpdateGlobalText(selBB.Lines[0].A.PointPercentTowardPoint(selBB.Lines[2].A, new(50)).ToSFML(), Angle, Scale);
				sel.TextDetails.Draw(renderTarget);
			}

			TrySelectionEvents();
		}
		protected override void UpdateDefaultValues()
		{
			VisibleItemCountMax = Math.Max(VisibleItemCountMax, 1);
			IsFocused = BoundingBox.IsHovered;

			Step = 1;
			RangeA = 0;
			RangeB = Items.Count - 1;

			scrollIndex = (int)Value;
		}
		private void TrySelectionEvents()
		{
			if(SelectedItem.ButtonDetails.IsDisabled)
				return;

			var result = SelectedItem.ButtonDetails.boundingBox.TryButton();
			var events = new List<(bool, Action<string, int, Thing.GUI.ListItem>)>()
			{
				(result.IsHovered, Event.ListItemHover), (result.IsUnhovered, Event.ListItemUnhover),
				(result.IsPressed, Event.ListItemPress), (result.IsReleased, Event.ListItemRelease),
				(result.IsClicked, Event.ListItemClick), (result.IsHeld, Event.ListItemHold),
			};

			for(int j = 0; j < events.Count; j++)
				if(events[j].Item1)
					events[j].Item2.Invoke(UID, SelectionIndex, SelectedItem);
		}
		#endregion
	}
}
