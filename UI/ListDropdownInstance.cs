namespace SMPL.UI
{
	internal class ListDropdownInstance : ListInstance
	{
		public string ShowButtonUID { get; private set; }
		[JsonIgnore]
		public string SelectionUID => ButtonUIDs.Count == 0 ? null : ButtonUIDs[SelectionIndex];
		public int SelectionIndex
		{
			get => selectionIndex;
			set
			{
				UpdateDefaultValues();
				selectionIndex = value.Limit(0, ButtonUIDs.Count, Extensions.Limitation.ClosestBound);
				TryUpdateSelection();
			}
		}

		#region Backend
		private bool isOpen;
		private int selectionIndex;

		[JsonConstructor]
		internal ListDropdownInstance() { }
		internal ListDropdownInstance(string uid, string btnUpUID, string btnDownUID, string btnShowUID) : base(uid, btnUpUID, btnDownUID)
		{
			Event.ButtonClicked += OnButtonClick;

			ShowButtonUID = btnShowUID;
			var btn = GetShowButton();
			if(btn == null)
				return;

			btn.ParentUID = UID;
			btn.OriginUnit = new(0, 1);
		}

		private void OnButtonClick(string thingUID)
		{
			if(thingUID == ShowButtonUID || thingUID == SelectionUID)
				isOpen = !isOpen;
			else if(ButtonUIDs.Contains(thingUID))
			{
				SelectionIndex = ButtonUIDs.IndexOf(thingUID);
				isOpen = false;
				GetSelection()?.Unhover();
			}
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			UpdateDefaultValues();

			var hidden = ButtonUIDs.Count == 0 || isOpen == false;

			TryUpdateSelection();

			var showBtn = GetShowButton();
			showBtn.Position = GetButtonUp().BoundingBox.Lines[3].A;

			var sel = GetSelection();
			if(sel != null)
				showBtn.LocalSize = new(sel.LocalSize.Y);

			base.OnDraw(renderTarget);

			IsHidden = isOpen == false;
			IsDisabled = isOpen == false;

			TrySetButtonVisibility(GetButtonUp(), false);
			TrySetButtonVisibility(GetButtonDown(), false);

			for(int i = 0; i < ButtonUIDs.Count; i++)
				TrySetButtonVisibility(Get<ButtonInstance>(ButtonUIDs[i]));

			void TrySetButtonVisibility(ButtonInstance btn, bool inList = true)
			{
				if(btn == null || btn.UID == SelectionUID)
					return;

				btn.IsHidden = hidden;
				btn.IsDisabled = hidden;
				if(hidden && inList)
					btn.Position = new Vector2().NaN();
			}
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			Event.ButtonClicked -= OnButtonClick;
		}
		protected override void OnUnfocus()
		{
			var showBtn = GetShowButton();
			if(showBtn == null || showBtn.BoundingBox.IsHovered)
				return;

			isOpen = false;
		}

		private ButtonInstance GetShowButton()
		{
			return Get<ButtonInstance>(ShowButtonUID);
		}
		private ButtonInstance GetSelection()
		{
			return Get<ButtonInstance>(SelectionUID);
		}
		private void UpdateDefaultValues()
		{
			RangeA = 0;
			RangeB = ButtonUIDs.Count - 1;
			OriginUnit = new(0, 0.5f);
		}
		private void TryUpdateSelection()
		{
			var sel = GetSelection();
			if(sel == null)
				return;

			var index = ButtonUIDs.IndexOf(sel.UID);
			var hidden = index.IsBetween((int)Value, (int)Value + VisibleButtonCountCurrent, true) == false && isOpen;

			sel.IsDisabled = hidden;
			sel.IsHidden = hidden;

			sel.ParentUID = UID;
			sel.LocalSize = new(ButtonWidth, ButtonHeight);
			sel.LocalPosition = new(-ButtonHeight * 0.5f - GetButtonUp().LocalSize.X, ButtonWidth * 0.5f + GetButtonUp().LocalSize.Y * 0.5f);

			if(hidden)
				sel.Position = new Vector2().NaN();
		}
		#endregion
	}
}
