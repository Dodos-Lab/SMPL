namespace SMPL
{
	internal class SettingsInstance : ThingInstance
	{
		public Vector2 Resolution
		{
			get => res;
			set
			{
				if(Game.SupportedMonitorResolutions.Contains(value) == false)
					value = Game.ScreenResolution;

				res = value;
			}
		}
		public bool IsVSyncEnabled
		{
			get => vsync;
			set { vsync = value; Game.Window.SetVerticalSyncEnabled(value); }
		}
		public Game.WindowState WindowState { get; set; } = Game.WindowState.Borderless;

		public float ScaleGUI { get; set; } = 1;

		public float VolumeMaster { get; set; } = 1f;
		public float VolumeSound { get; set; } = 1f;
		public float VolumeMusic { get; set; } = 0.5f;

		#region Backend
		private bool vsync;
		private Vector2 res = Game.ScreenResolution;

		internal const string DB_PATH = "settings.cdb";
		internal const string DB_SHEET_NAME = "settings";
		#endregion
	}
}
