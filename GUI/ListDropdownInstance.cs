namespace SMPL.GUI
{
	internal class ListDropdownInstance : ListInstance
	{
		[JsonIgnore]
		public Thing.GUI.ListItem SelectedItem => Items.Count == 0 ? null : Items[SelectionIndex];
		public int SelectionIndex
		{
			get => selectionIndex;
			set
			{
				UpdateDefaultValues();
				var prev = (int)selectionIndex;
				selectionIndex = value.Limit(0, Items.Count, Extensions.Limitation.ClosestBound);
				selectionIndex = Math.Max(selectionIndex, 0);

				if(selectionIndex != prev)
				{
					Event.ListItemDeselect(UID, prev, Items[prev]);
					Event.ListItemSelect(UID, SelectionIndex, SelectedItem);
				}
			}
		}

		public Thing.GUI.ButtonDetails Button { get; } = new();

		#region Backend
		private int selectionIndex;
		private readonly Hitbox sel = new();
		private bool isOpen;

		[JsonConstructor]
		internal ListDropdownInstance() => Init();
		internal ListDropdownInstance(string uid) : base(uid)
		{
			Init();
		}
		private void Init()
		{
			Event.ButtonClicked += OnShowButtonClick;
			Event.ListItemClicked += OnItemClick;
		}

		private void OnItemClick(string listUID, int itemIndex, Thing.GUI.ListItem item)
		{
			if(Items.Count == 0 || Items.Contains(item) == false) // not from this list
				return;

			if(itemIndex != SelectionIndex || sel.IsHovered == false)
				Event.ListItemUnhover(UID, itemIndex, item);

			Show(false);
			SelectionIndex = itemIndex;
		}

		private void OnShowButtonClick(string guiUID, Thing.GUI.ButtonDetails buttonDetails)
		{
			if(Items.Count > 0 && buttonDetails == Button)
				Show(isOpen == false);
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			if(isOpen && Items.Count > 0)
				base.OnDraw(renderTarget);


			UpdateDefaultValues();
			UpdateButtonBoundingBoxes();
			UpdateButtonBoundingBox();
			UpdateSelectionBoundingBox();

			TryButtonEvents();

			if(isOpen == false)
				DrawSelection(renderTarget);
			Button.Draw(renderTarget);

			TryClickOutside();
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			Event.ButtonClicked -= OnShowButtonClick;
			Event.ListItemClicked -= OnItemClick;
		}

		private void TryClickOutside()
		{
			if(Mouse.IsButtonPressed(Mouse.Button.Left).Once($"lg;akg{GetHashCode()}-lmb"))
			{
				var btnUp = ButtonUp.boundingBox.Lines;
				var btnDown = ButtonDown.boundingBox.Lines;
				var scrollBB = new Hitbox(btnUp[2].A, btnUp[1].A, btnDown[0].A, btnDown[3].A, btnUp[2].A);

				if(scrollBB.IsHovered || BoundingBox.IsHovered || ButtonUp.boundingBox.IsHovered ||
					ButtonDown.boundingBox.IsHovered || Button.boundingBox.IsHovered ||
					sel.IsHovered)
					return;

				Show(false);
			}
		}
		private void TrySelectionEvents()
		{
			var selB = SelectedItem.ButtonDetails;
			if(selB.IsDisabled)
				return;

			var result = sel.TryButton(isHoldable: false);
			var events = new List<(bool, Action<string, int, Thing.GUI.ListItem>)>()
			{
				(result.IsHovered, Event.ListItemHover), (result.IsUnhovered, Event.ListItemUnhover),
				(result.IsPressed, Event.ListItemPress), (result.IsReleased, Event.ListItemRelease),
				(result.IsClicked, Event.ListItemClick),
			};

			for(int i = 0; i < events.Count; i++)
				if(events[i].Item1)
					events[i].Item2.Invoke(UID, SelectionIndex, SelectedItem);
		}
		protected override void TryButtonEvents()
		{
			base.TryButtonEvents();

			if(Button.IsDisabled)
				return;

			var result = Button.boundingBox.TryButton(isHoldable: false);
			var events = new List<(bool, Action<string, Thing.GUI.ButtonDetails>)>()
			{
				(result.IsHovered, Event.ButtonHover), (result.IsUnhovered, Event.ButtonUnhover),
				(result.IsPressed, Event.ButtonPress), (result.IsReleased, Event.ButtonRelease),
				(result.IsClicked, Event.ButtonClick),
			};

			for(int i = 0; i < events.Count; i++)
				if(events[i].Item1)
					events[i].Item2.Invoke(UID, Button);
		}

		private void UpdateSelectionBoundingBox()
		{
			var bb = BoundingBox.Lines;
			var btnBB = Button.boundingBox.Lines;
			var tl = bb[3].A.MoveAtAngle(Angle + 180, ItemHeight * Scale, false);
			var br = btnBB[3].A;
			var tr = br.MoveAtAngle(Angle + 180, ItemHeight * Scale, false);
			var bl = bb[3].A;

			sel.LocalLines.Clear();
			sel.Lines.Clear();
			sel.Lines.Add(new(tl, tr));
			sel.Lines.Add(new(tr, br));
			sel.Lines.Add(new(br, bl));
			sel.Lines.Add(new(bl, tl));
		}
		private void UpdateButtonBoundingBox()
		{
			var upBB = ButtonUp.boundingBox.Lines;
			var sz = Size.Y * 1.5f;
			var btnBB = Button.boundingBox;
			var bl = upBB[3].A;
			var tl = bl.MoveAtAngle(Angle + 180, sz, false);
			var br = bl.MoveAtAngle(Angle - 90, sz, false);
			var tr = br.MoveAtAngle(Angle + 180, sz, false);

			btnBB.LocalLines.Clear();
			btnBB.Lines.Clear();
			btnBB.Lines.Add(new(tl, tr));
			btnBB.Lines.Add(new(tr, br));
			btnBB.Lines.Add(new(br, bl));
			btnBB.Lines.Add(new(bl, tl));
		}

		private void DrawSelection(RenderTarget renderTarget)
		{
			if(Items.Count == 0)
				return;

			var selB = SelectedItem.ButtonDetails;
			var selT = SelectedItem.TextDetails;
			var selBB = selB.boundingBox;
			var prev = new List<Line>(selBB.Lines);
			var prevDisabledBtn = selB.IsDisabled;
			var prevHiddenBtn = selB.IsHidden;
			var prevHiddenTxt = selT.IsHidden;
			var center = sel.Lines[0].A.PercentToTarget(sel.Lines[2].A, new(50));

			selBB.Lines.Clear();
			for(int i = 0; i < sel.Lines.Count; i++)
				selBB.Lines.Add(sel.Lines[i]);

			selB.IsDisabled = false;
			selB.IsHidden = false;
			selT.IsHidden = false;

			TrySelectionEvents();
			selB.Draw(renderTarget);
			selT.UpdateGlobalText(center, Angle - 90, Scale);
			selT.Draw(renderTarget);

			selB.IsDisabled = prevDisabledBtn;
			selB.IsHidden = prevHiddenBtn;
			selT.IsHidden = prevHiddenTxt;

			selBB.Lines.Clear();
			for(int i = 0; i < prev.Count; i++)
				selBB.Lines.Add(prev[i]);
		}

		private void Show(bool isOpen)
		{
			this.isOpen = isOpen;
			IsDisabledSelf = isOpen == false;

			Event.ListDropdownToggle(UID);
		}
		#endregion
	}
}
