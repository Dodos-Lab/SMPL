namespace SMPL
{
	public class Settings
	{
		public enum WindowStates { Windowed, Fullscreen, Borderless }

		private bool vsync;

		internal const string DB_PATH = "settings.cdb";
		internal const string DB_SHEET_NAME = "settings";

		public float VolumeUnitMaster { get; set; } = 1f;
		public float VolumeUnitSound { get; set; } = 1f;
		public float VolumeUnitMusic { get; set; } = 0.5f;

		public bool IsVSyncEnabled
		{
			get => vsync;
			set { vsync = value; Game.Window.SetVerticalSyncEnabled(value); }
		}
		public float ResolutionScale { get; set; } = 1;
		public float ScaleGUI { get; set; } = 1;
		public WindowStates WindowState { get; set; }
	}
}
