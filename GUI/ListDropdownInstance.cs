namespace SMPL.GUI
{
	internal class ListDropdownInstance : ListInstance
	{
		[JsonIgnore]
		public Thing.GUI.ListItem SelectedItem => Items.Count == 0 ? null : Items[SelectionIndex];
		public int SelectionIndex
		{
			get => (int)Value;
			set
			{
				UpdateDefaultValues();
				var prev = (int)Value;
				Value = value.Limit(0, Items.Count, Extensions.Limitation.ClosestBound);
				Value = MathF.Max(Value, 0);

				if(Value != prev)
				{
					Event.ListItemDeselect(UID, prev, Items[prev]);
					Event.ListItemSelect(UID, SelectionIndex, SelectedItem);
				}
			}
		}

		public Thing.GUI.ButtonDetails Button { get; } = new();

		#region Backend
		private bool isOpen;

		[JsonConstructor]
		internal ListDropdownInstance() => Init();
		internal ListDropdownInstance(string uid) : base(uid)
		{
			Init();
		}
		private void Init()
		{
			Event.DropdownListButtonClicked += OnShowButtonClick;
			Event.ListItemClicked += OnItemClick;

			Show(false);
		}

		private void OnItemClick(string listUID, int itemIndex, Thing.GUI.ListItem item)
		{
			if(Items.Contains(item) == false) // not from this list
				return;

			SelectionIndex = itemIndex;
			Show(false);
			Event.ListItemUnhover(UID, SelectionIndex, SelectedItem);
		}

		private void OnShowButtonClick(string guiUID, Thing.GUI.ButtonDetails buttonDetails)
		{
			if(buttonDetails == Button)
				Show(isOpen == false);
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

			Event.DropdownListButtonClicked -= OnShowButtonClick;
			Event.ListItemClicked -= OnItemClick;
		}

		private void TryUpdate()
		{
			if(IsDisabled)
				return;

			UpdateDefaultValues();

			var hidden = Items.Count == 0 || isOpen == false;
		}
		private void Show(bool isOpen)
		{
			this.isOpen = isOpen;
			IsHiddenSelf = isOpen == false;
			IsDisabledSelf = isOpen == false;

			Event.DropdownListToggle(UID);
		}
		#endregion
	}
}
