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
				SMPL.Event.CheckboxCheck(UID);
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
			SMPL.Event.ButtonClicked += OnClick;
		}
		private void OnClick(string thingUID)
		{
			if(IsDisabled || thingUID != UID)
				return;

			IsChecked = !IsChecked;
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			SMPL.Event.ButtonClicked -= OnClick;
		}
		#endregion
	}
}