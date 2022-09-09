namespace SMPL.GUI
{
	internal class ListDropdownInstance : ListInstance
	{
		public string ButtonShowUID { get; set; }
		[JsonIgnore]
		public Thing.GUI.ListItem SelectedItem => Items.Count == 0 ? null : Items[SelectionIndex];
		public int SelectionIndex
		{
			get => selectionIndex;
			set
			{
				UpdateDefaultValues();
				selectionIndex = value.Limit(0, Items.Count, Extensions.Limitation.ClosestBound);
				TryUpdateSelection();
			}
		}

		#region Backend
		private bool isOpen;
		private int selectionIndex;

		[JsonConstructor]
		internal ListDropdownInstance() => Init();
		internal ListDropdownInstance(string uid) : base(uid)
		{
			Init();
		}
		private void Init()
		{
			Event.ButtonClicked += OnButtonClick;
		}

		private void OnButtonClick(string thingUID)
		{
			//var btnUIDs = GetButtonUIDs();
			//if(thingUID == ButtonShowUID || thingUID == SelectionUID)
			//	Show(isOpen == false);
			//else if(btnUIDs.Contains(thingUID))
			//{
			//	SelectionIndex = btnUIDs.IndexOf(thingUID);
			//	Show(false);
			//	GetSelection()?.Unhover();
			//}
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			TryUpdate(); // has to be after draw
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();

			Event.ButtonClicked -= OnButtonClick;
		}
		protected void OnUnfocus()
		{
			var showBtn = GetShowButton();
			if(showBtn == null || showBtn.BoundingBox.IsHovered)
				return;

			Show(false);
		}

		private void TryUpdate()
		{
			//if(IsDisabled)
			//	return;
			//
			//UpdateDefaultValues();
			//
			//var btnUIDs = GetButtonUIDs();
			//var hidden = btnUIDs.Count == 0 || isOpen == false;
			//
			//TryUpdateSelection();
			//
			//var showBtn = GetShowButton();
			//if(showBtn != null)
			//{
			//	//showBtn.Position = GetButtonUp().BoundingBox.Lines[3].A;
			//	showBtn.OriginUnit = new(0, 1);
			//}
			//
			//var sel = GetSelection();
			//if(sel != null && showBtn != null)
			//	showBtn.LocalSize = new(sel.LocalSize.Y);
			//
			////TrySetButtonVisibility(GetButtonUp(), false);
			////TrySetButtonVisibility(GetButtonDown(), false);
			//
			//for(int i = 0; i < btnUIDs.Count; i++)
			//	TrySetButtonVisibility(Get<ButtonInstance>(btnUIDs[i]));
			//
			//if(Age < 2f)
			//	Show(false);
			//
			//void TrySetButtonVisibility(ButtonInstance btn, bool inList = true)
			//{
			//	if(btn == null || btn.UID == SelectionUID)
			//		return;
			//
			//	btn.IsHiddenSelf = hidden;
			//	btn.IsDisabledSelf = hidden;
			//}
		}
		private void Show(bool isOpen)
		{
			this.isOpen = isOpen;
			IsHiddenSelf = isOpen == false;
			IsDisabledSelf = isOpen == false;
		}
		private ButtonInstance GetShowButton()
		{
			return Get<ButtonInstance>(ButtonShowUID);
		}
		private void UpdateDefaultValues()
		{
			RangeA = 0;
			RangeB = Items.Count - 1;
			OriginUnit = new(0, 0.5f);
		}
		private void TryUpdateSelection()
		{
			//var sel = GetSelection();
			//if(sel == null)
			//	return;
			//
			//var index = GetButtonUIDs().IndexOf(sel.UID);
			//var hidden = index.IsBetween((int)Value, (int)Value + VisibleItemCountCurrent, true) == false && isOpen;
			//
			//sel.IsDisabledSelf = hidden;
			//sel.IsHiddenSelf = hidden;
			//
			//sel.ParentUID = UID;
			//sel.LocalSize = new(ItemWidth, ItemHeight);
			//sel.LocalPosition = new(-ButtonHeight * 0.5f - GetButtonUp().LocalSize.X, ButtonWidth * 0.5f + GetButtonUp().LocalSize.Y * 0.5f);
		}
		#endregion
	}
}
