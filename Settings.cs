namespace SMPL
{
	public class Settings
	{
		internal const string DB_PATH = "settings.cdb";
		internal const string DB_SHEET_NAME = "settings";

		public float VolumeMaster { get; set; } = 1f;
		public float VolumeSound { get; set; } = 1f;
		public float VolumeMusic { get; set; } = 0.5f;

		public bool IsVSyncEnabled { get; set; }
		public float PixelSize { get; set; } = 1;
	}
}
