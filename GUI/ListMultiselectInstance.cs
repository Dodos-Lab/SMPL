namespace SMPL.GUI
{
	internal class ListMultiselectInstance : ListInstance
	{
		[JsonIgnore]
		public ReadOnlyCollection<Thing.GUI.ListItem> SelectedItems
		{
			get
			{
				var result = new List<Thing.GUI.ListItem>();
				for(int i = 0; i < SelectionIndexes.Count; i++)
					result.Add(Items[SelectionIndexes[i]]);

				return result.AsReadOnly();
			}
		}
		public ReadOnlyCollection<int> SelectionIndexes => selectionIndexes.AsReadOnly();

		#region Backend
		private readonly List<int> selectionIndexes = new();

		[JsonConstructor]
		internal ListMultiselectInstance() { }
		internal ListMultiselectInstance(string uid) : base(uid)
		{
			Event.ListItemClicked += OnItemClick;
		}

		private void OnItemClick(string listUID, int itemIndex, Thing.GUI.ListItem item)
		{
			if(IsDisabled || listUID != UID)
				return;

			var contains = selectionIndexes.Contains(itemIndex);

			if(contains)
			{
				selectionIndexes.Remove(itemIndex);
				Event.ListItemDeselect(listUID, itemIndex, item);
			}
			else
			{
				selectionIndexes.Add(itemIndex);
				Event.ListItemSelect(listUID, itemIndex, item);
			}

		}

		#endregion
	}
}
