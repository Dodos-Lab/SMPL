namespace SMPL.UI
{
	internal class ListMultiselectInstance : ListInstance
	{
		[JsonIgnore]
		public ReadOnlyCollection<string> SelectionUIDs
		{
			get
			{
				var result = new List<string>();
				for(int i = 0; i < SelectionIndexes.Count; i++)
				{
					if(i >= ButtonUIDs.Count)
						break;

					result.Add(ButtonUIDs[SelectionIndexes[i]]);
				}
				return result.AsReadOnly();
			}
		}
		public ReadOnlyCollection<int> SelectionIndexes => selectionIndexes.AsReadOnly();

		#region Backend
		private readonly List<int> selectionIndexes = new();

		[JsonConstructor]
		internal ListMultiselectInstance() { }
		internal ListMultiselectInstance(string uid, string btnUpUID, string btnDownUID) : base(uid, btnUpUID, btnDownUID)
		{
			Event.ButtonClicked += OnButtonClick;
		}

		private void OnButtonClick(string thingUID)
		{
			if(ButtonUIDs.Contains(thingUID) == false)
				return;

			var index = ButtonUIDs.IndexOf(thingUID);
			var contains = selectionIndexes.Contains(index);

			if(contains)
				selectionIndexes.Remove(index);
			else
				selectionIndexes.Add(index);

			Event.ListMultiselectionChanged(thingUID, contains == false);
		}
		#endregion
	}
}
