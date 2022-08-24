namespace SMPL.UI
{
	internal class ListDropdownInstance : ListInstance
	{
		public string ButtonShowUID { get; private set; }
		[JsonIgnore]
		public string SelectionUID => GetButtonUIDs().Count == 0 ? null : GetButtonUIDs()[SelectionIndex];
		public int SelectionIndex
		{
			get => selectionIndex;
			set
			{
				UpdateDefaultValues();
				selectionIndex = value.Limit(0, GetButtonUIDs().Count, Extensions.Limitation.ClosestBound);
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

			ButtonShowUID = btnShowUID;
			var btn = GetShowButton();
			if(btn == null)
				return;

			btn.ParentUID = UID;
			btn.OriginUnit = new(0, 1);
		}

		private void OnButtonClick(string thingUID)
		{
			var btnUIDs = GetButtonUIDs();
			if(thingUID == ButtonShowUID || thingUID == SelectionUID)
				isOpen = !isOpen;
			else if(btnUIDs.Contains(thingUID))
			{
				SelectionIndex = btnUIDs.IndexOf(thingUID);
				isOpen = false;
				GetSelection()?.Unhover();
			}
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			UpdateDefaultValues();

			var btnUIDs = GetButtonUIDs();
			var hidden = btnUIDs.Count == 0 || isOpen == false;

			TryUpdateSelection();

			var showBtn = GetShowButton();
			if(showBtn != null)
				showBtn.Position = GetButtonUp().BoundingBox.Lines[3].A;

			var sel = GetSelection();
			if(sel != null)
				showBtn.LocalSize = new(sel.LocalSize.Y);

			base.OnDraw(renderTarget);

			IsHidden = isOpen == false;
			IsDisabled = isOpen == false;

			TrySetButtonVisibility(GetButtonUp(), false);
			TrySetButtonVisibility(GetButtonDown(), false);

			for(int i = 0; i < btnUIDs.Count; i++)
				TrySetButtonVisibility(Get<ButtonInstance>(btnUIDs[i]));

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
			return Get<ButtonInstance>(ButtonShowUID);
		}
		private ButtonInstance GetSelection()
		{
			return Get<ButtonInstance>(SelectionUID);
		}
		private void UpdateDefaultValues()
		{
			RangeA = 0;
			RangeB = GetButtonUIDs().Count - 1;
			OriginUnit = new(0, 0.5f);
		}
		private void TryUpdateSelection()
		{
			var sel = GetSelection();
			if(sel == null)
				return;

			var index = GetButtonUIDs().IndexOf(sel.UID);
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
