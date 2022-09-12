namespace SMPL.GUI
{
	internal class CheckboxInstance : ButtonInstance
	{
		public bool IsChecked
		{
			get => isChecked;
			set
			{
				isChecked = value;
				Event.CheckboxCheck(UID);
			}
		}

		#region Backend
		private bool isChecked;

		[JsonConstructor]
		internal CheckboxInstance()
		{
			Init();
		}
		internal CheckboxInstance(string uid) : base(uid)
		{
			Init();
		}

		private void Init()
		{
			Size = new(50);
			Event.ButtonClicked += OnClick;
		}
		private void OnClick(string thingUID, Thing.GUI.ButtonDetails btn)
		{
			if(IsDisabled || thingUID != UID)
				return;

			IsChecked = !IsChecked;
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			Event.ButtonClicked -= OnClick;
		}
		#endregion
	}
}