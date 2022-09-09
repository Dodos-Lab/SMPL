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
				{
					if(i >= Items.Count)
						break;

					result.Add(Items[SelectionIndexes[i]]);
				}
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
			Event.ButtonClicked += OnButtonClick;
		}

		private void OnButtonClick(string thingUID)
		{
			//if(IsDisabled)
			//	return;
			//
			//var btnUIDs = GetButtonUIDs();
			//if(btnUIDs.Contains(thingUID) == false)
			//	return;
			//
			//var index = btnUIDs.IndexOf(thingUID);
			//var contains = selectionIndexes.Contains(index);
			//
			//if(contains)
			//	selectionIndexes.Remove(index);
			//else
			//	selectionIndexes.Add(index);
			//
			//Event.ListMultiselectionChanged(thingUID, contains == false);
		}
		#endregion
	}
}
