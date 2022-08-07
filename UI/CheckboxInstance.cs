namespace SMPL.UI
{
	internal class CheckboxInstance : ButtonInstance
	{
		public bool IsActive
		{
			get => active;
			set
			{
				active = value;
				SMPL.Event.CheckboxCheck(UID);
			}
		}

		#region Backend
		private bool active;

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
			if(thingUID != UID)
				return;

			IsActive = !IsActive;
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			SMPL.Event.ButtonClicked -= OnClick;
		}
		#endregion
	}
}