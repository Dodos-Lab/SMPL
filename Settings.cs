namespace SMPL
{
	public class Settings
	{
		internal const string DB_PATH = "settings.cdb";
		internal const string DB_SHEET_NAME = "settings";

		public float VolumeUnitMaster { get; set; } = 1f;
		public float VolumeUnitSound { get; set; } = 1f;
		public float VolumeUnitMusic { get; set; } = 0.5f;

		public bool IsVSyncEnabled { get; set; }
		public float ResolutionScale { get; set; } = 1;
		public float ScaleGUI { get; set; } = 1;
	}
}
