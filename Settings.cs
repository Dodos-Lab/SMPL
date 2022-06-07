using System.Windows.Forms;

namespace SMPL
{
	public class Settings
	{
		public enum WindowStates { Windowed, Borderless, Fullscreen }

		public static List<Vector2> SupportedMonitorResolutions
		{
			get
			{
				var result = new List<Vector2>();
				var m = new DEVMODE();
				int i = 0;
				while(EnumDisplaySettings(null, i, ref m))
				{
					var res = new Vector2(m.dmPelsWidth, m.dmPelsHeight);
					if(result.Contains(res) == false)
						result.Add(res);
					i++;
				}
				return result;
			}
		}

		public float VolumeUnitMaster { get; set; } = 1f;
		public float VolumeUnitSound { get; set; } = 1f;
		public float VolumeUnitMusic { get; set; } = 0.5f;

		public bool IsVSyncEnabled
		{
			get => vsync;
			set { vsync = value; Game.Window.SetVerticalSyncEnabled(value); }
		}
		public Vector2 Resolution
		{
			get => res;
			set
			{
				if(SupportedMonitorResolutions.Contains(value) == false)
					value = ScreenResolution;

				res = value;
			}
		}
		public float ScaleGUI { get; set; } = 1;
		public WindowStates WindowState { get; set; } = WindowStates.Borderless;

		[JsonIgnore]
		public static Vector2 ScreenResolution => new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

		#region Backend
		private bool vsync;
		private Vector2 res = ScreenResolution;

		internal const string DB_PATH = "settings.cdb";
		internal const string DB_SHEET_NAME = "settings";

		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

		[StructLayout(LayoutKind.Sequential)]
		private struct DEVMODE
		{

			private const int CCHDEVICENAME = 0x20;
			private const int CCHFORMNAME = 0x20;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string dmDeviceName;
			public short dmSpecVersion;
			public short dmDriverVersion;
			public short dmSize;
			public short dmDriverExtra;
			public int dmFields;
			public int dmPositionX;
			public int dmPositionY;
			public ScreenOrientation dmDisplayOrientation;
			public int dmDisplayFixedOutput;
			public short dmColor;
			public short dmDuplex;
			public short dmYResolution;
			public short dmTTOption;
			public short dmCollate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
			public string dmFormName;
			public short dmLogPixels;
			public int dmBitsPerPel;
			public int dmPelsWidth;
			public int dmPelsHeight;
			public int dmDisplayFlags;
			public int dmDisplayFrequency;
			public int dmICMMethod;
			public int dmICMIntent;
			public int dmMediaType;
			public int dmDitherType;
			public int dmReserved1;
			public int dmReserved2;
			public int dmPanningWidth;
			public int dmPanningHeight;

		}
		#endregion
	}
}
