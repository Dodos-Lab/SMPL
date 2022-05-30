using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SMPL.Graphics;
using SMPL.Tools;
using Console = SMPL.Tools.Console;
using Time = SMPL.Tools.Time;

namespace SMPL
{
	/// <summary>
	/// All the core systems are handled here (the <see cref="Game"/> states, the <see cref="Window"/>, the game loop etc).
	/// </summary>
	public static class Game
	{
		internal static Styles currWindowStyle;

		public static Settings Settings { get; internal set; } = new();

		/// <summary>
		/// The raw <see cref="RenderWindow"/> instance. Useful for input events, drawing, ingame settings etc.
		/// </summary>
		public static RenderWindow Window { get; internal set; }

		/// <summary>
		/// The cursor's position relative to the <see cref="Window"/>.<br></br>
		/// - Example: [0, 0] is the top left corner of the <see cref="Window"/> and [<see cref="Window.Size"/>] is the bottom right corner
		/// </summary>
		public static Vector2 MouseCursorPosition
		{
			get { var p = Mouse.GetPosition(Window); return new(p.X, p.Y); }
			set { Mouse.SetPosition(new((int)value.X, (int)value.Y), Window); }
		}

		/// <summary>
		/// Boots up the <see cref="Game"/> systems that initialize the <see cref="Window"/> alongside other things.
		/// The <paramref name="startingScene"/> is set as <see cref="Scene.CurrentScene"/> and the optional <paramref name="loadingScene"/> as
		/// <see cref="Scene.LoadingScene"/>.
		/// </summary>
		public static void Start(Scene startingScene, Scene loadingScene = null)
		{
			if (startingScene == null || Window != null)
				return;

			InitWindow(Settings.WindowStates.Borderless, Settings.ScreenResolution);

			Scene.Init(startingScene, loadingScene);
			var sz = Settings.ScreenResolution;
			Scene.MainCamera = new((uint)(sz.X), (uint)(sz.Y));

			while (Window.IsOpen)
			{
				Window.DispatchEvents();
				Window.Clear();
				Scene.MainCamera.Fill(Color.Black);

				Time.Update();
				Scene.UpdateCurrentScene();
				Camera.DrawMainCameraToWindow();
				Window.Display();
			}
		}
		/// <summary>
		/// Notifies the <see cref="Scene.CurrentScene"/> with <see cref="Scene.OnGameStop"/> then closes out the <see cref="Window"/>
		/// and shuts down everything.
		/// </summary>
		public static void Stop()
		{
			Scene.CurrentScene?.GameStop();
			Window.Close();
		}

		/// <summary>
		/// Tries to open a web page in the browser through some <paramref name="url"/>.
		/// </summary>
		public static void OpenWebPage(string url)
		{
			try { Process.Start(url); }
			catch
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					Process.Start("xdg-open", url);
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
					Process.Start("open", url);
				else
					Console.LogError(1, $"Could not load URL '{url}'.");
			}
		}
		/// <summary>
		/// Centers the <see cref="Window"/> on  the user's screen.
		/// </summary>
		public static void CenterWindow()
		{
			var sz = Settings.ScreenResolution;
			Window.Size = new((uint)(sz.X / 1.5f), (uint)(sz.Y / 1.5f));
			Window.Position = new Vector2i((int)(sz.X / 2f), (int)(sz.Y / 2f)) - new Vector2i((int)(Window.Size.X / 2), (int)(Window.Size.Y / 2));
		}

		private static void Main() { }
		private static void OnClose(object sender, EventArgs e) => Stop();
		internal static void InitWindow(Settings.WindowStates windowState, Vector2 resolution)
		{
			currWindowStyle = windowState.ToWindowStyles();
			Window = new(new((uint)resolution.X, (uint)resolution.Y), "SMPL Game", currWindowStyle);
			Window.Position = new();
			Window.Clear();
			Window.Display();
			Window.Closed += OnClose;
			Window.SetFramerateLimit(120);

			var view = Window.GetView();
			view.Center = new();
			Window.SetView(view);

			if (windowState == Settings.WindowStates.Windowed)
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
	}
}
