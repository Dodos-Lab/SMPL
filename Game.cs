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
global using SMPL.Graphics;
global using SMPL.Tools;
global using Console = SMPL.Tools.Console;
global using Time = SMPL.Tools.Time;

namespace SMPL
{
	public static class Game
	{
		public static Settings Settings { get; internal set; } = new();
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

			InitWindow(Settings.WindowStates.Windowed, Settings.ScreenResolution);

			Scene.CurrentScene = startingScene;
			Scene.LoadingScene = loadingScene;

			if(Thing.Exists(Scene.MAIN_CAMERA_UID) == false)
				Thing.CreateCamera(Scene.MAIN_CAMERA_UID, Settings.ScreenResolution);

			Scene.assetsLoading = new(Scene.ThreadLoadAssets) { IsBackground = true, Name = "AssetsLoading" };
			Scene.assetsLoading.Start();

			while(Window.IsOpen)
			{
				Window.DispatchEvents();
				Scene.MainCamera.RenderTexture.Clear(Color.Black);

				Time.Update();
				Scene.UpdateCurrentScene();
				Thing.DrawAllVisuals(Scene.MainCamera.RenderTexture);
				CameraInstance.DrawMainCameraToWindow();
			}
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
			var sz = Settings.ScreenResolution;
			Window.Size = new((uint)(sz.X / 1.5f), (uint)(sz.Y / 1.5f));
			Window.Position = new Vector2i((int)(sz.X / 2f), (int)(sz.Y / 2f)) - new Vector2i((int)(Window.Size.X / 2), (int)(Window.Size.Y / 2));
		}

		#region Backend
		internal static Styles currWindowStyle;
		private static void Main() { }
		private static void OnClose(object sender, EventArgs e) => Stop();
		internal static void InitWindow(Settings.WindowStates windowState, Vector2 resolution)
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

			if(windowState == Settings.WindowStates.Windowed)
				CenterWindow();
		}
		internal static Settings.WindowStates ToWindowStates(this Styles style)
		{
			return style switch
			{
				Styles.Fullscreen => Settings.WindowStates.Fullscreen,
				Styles.None => Settings.WindowStates.Borderless,
				_ => Settings.WindowStates.Windowed,
			};
		}
		internal static Styles ToWindowStyles(this Settings.WindowStates windowStates)
		{
			return windowStates switch
			{
				Settings.WindowStates.Borderless => Styles.None,
				Settings.WindowStates.Fullscreen => Styles.Fullscreen,
				_ => Styles.Default,
			};
		}
		#endregion
	}
}
