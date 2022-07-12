global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Threading;
global using System.Windows.Forms;
global using Fasterflect;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Serialization;
global using SFML.Audio;
global using SFML.Graphics;
global using SFML.Graphics.Glsl;
global using SFML.System;
global using SFML.Window;
global using SMPL.Tools;
global using Console = SMPL.Tools.Console;
global using Time = SMPL.Tools.Time;

namespace SMPL
{
	public static class Game
	{
		public enum WindowState { Windowed, Borderless, Fullscreen }
		public static ReadOnlyCollection<Vector2> SupportedMonitorResolutions
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
				return result.AsReadOnly();
			}
		}
		public static Vector2 ScreenResolution => new(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
		public static string SettingsUID { get; set; }

		public static RenderWindow Window { get; internal set; }
		public static Vector2 MouseCursorPosition
		{
			get { var p = Mouse.GetPosition(Window); return new(p.X, p.Y); }
			set { Mouse.SetPosition(new((int)value.X, (int)value.Y), Window); }
		}

		public static void Start(Scene startingScene, Scene loadingScene = null)
		{
			if(startingScene == null || Window != null)
				return;

			InitWindow(WindowState.Windowed, Game.ScreenResolution);

			Scene.CurrentScene = startingScene;
			Scene.LoadingScene = loadingScene;

			if(Thing.Exists(Scene.MAIN_CAMERA_UID) == false)
				Thing.CreateCamera(Scene.MAIN_CAMERA_UID, Game.ScreenResolution);

			Scene.assetsLoading = new(Scene.ThreadLoadAssets) { IsBackground = true, Name = "AssetsLoading" };
			Scene.assetsLoading.Start();

			while(Window.IsOpen)
			{
				Window.DispatchEvents();
				Scene.MainCamera.RenderTexture.Clear(Color.Black);

				Time.Update();
				Scene.UpdateCurrentScene();
				UpdateEngine(Scene.MainCamera.RenderTexture);
				CameraInstance.DrawMainCameraToWindow();
			}
		}
		public static void UpdateEngine(RenderTarget renderTarget)
		{
			var visuals = VisualInstance.visuals.Reverse();
			var cameras = CameraInstance.cameras;

			for(int i = 0; i < cameras.Count; i++)
				if(cameras[i].UID != Scene.MAIN_CAMERA_UID)
					cameras[i].RenderTexture.Clear(Color.Transparent);

			foreach(var kvp in visuals)
				for(int i = 0; i < kvp.Value.Count; i++)
					for(int j = 0; j < kvp.Value[i].CameraUIDs.Count; j++)
					{
						var cam = ThingInstance.Get<CameraInstance>(kvp.Value[i].CameraUIDs[j]);
						if(cam != null)
							kvp.Value[i].Draw(cam.RenderTexture);
					}


			for(int i = 0; i < cameras.Count; i++)
				if(cameras[i].UID != Scene.MAIN_CAMERA_UID)
					cameras[i].RenderTexture.Display();

			foreach(var kvp in visuals)
				for(int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Draw(renderTarget);

			Scene.UpdateCurrentScene();
			AudioInstance.Update();
		}
		public static void Stop()
		{
			Scene.CurrentScene?.GameStop();
			Window.Close();
		}

		public static void OpenWebPage(string url)
		{
			try { Process.Start(url); }
			catch
			{
				if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					Process.Start("xdg-open", url);
				else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					Process.Start("open", url);
				else
					Console.LogError(1, $"Could not load URL '{url}'.");
			}
		}
		public static void CenterWindow()
		{
			var sz = Game.ScreenResolution;
			Window.Size = new((uint)(sz.X / 1.5f), (uint)(sz.Y / 1.5f));
			Window.Position = new Vector2i((int)(sz.X / 2f), (int)(sz.Y / 2f)) - new Vector2i((int)(Window.Size.X / 2), (int)(Window.Size.Y / 2));
		}

		#region Backend
		internal static Styles currWindowStyle;
		private static void Main() { }
		private static void OnClose(object sender, EventArgs e) => Stop();
		internal static void InitWindow(WindowState windowState, Vector2 resolution)
		{
			currWindowStyle = windowState.ToWindowStyles();
			Window = new(new((uint)resolution.X, (uint)resolution.Y), "SMPL Game", currWindowStyle) { Position = new() };
			Window.Clear();
			Window.Display();
			Window.Closed += OnClose;
			Window.SetFramerateLimit(120);

			var view = Window.GetView();
			view.Center = new();
			Window.SetView(view);

			if(windowState == WindowState.Windowed)
				CenterWindow();
		}
		internal static WindowState ToWindowStates(this Styles style)
		{
			return style switch
			{
				Styles.Fullscreen => WindowState.Fullscreen,
				Styles.None => WindowState.Borderless,
				_ => WindowState.Windowed,
			};
		}
		internal static Styles ToWindowStyles(this WindowState windowStates)
		{
			return windowStates switch
			{
				WindowState.Borderless => Styles.None,
				WindowState.Fullscreen => Styles.Fullscreen,
				_ => Styles.Default,
			};
		}

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
