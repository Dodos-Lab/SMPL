using System;
using System.Numerics;
using SFML.Graphics;
using SFML.Window;

namespace SMPL
{
	public static class Game
	{
		public static RenderWindow Window { get; private set; }

		public static Vector2 MousePosition
		{
			get
			{
				var p = Mouse.GetPosition(Window);
				return new(p.X, p.Y);
			}
		}

		public static void Start(Scene startingScene, Scene loadingScene = null)
		{
			InitWindow();
			Camera.Position = new();
			Scene.Init(startingScene, loadingScene);

			while (Window.IsOpen)
			{
				Window.DispatchEvents();
				Window.Clear(GetActiveSceneBgColor());

				Time.Update();
				Scene.UpdateCurrentScene();

				Window.Display();
			}

			void InitWindow()
			{
				Window = new(new(1280, 720), "SMPL Game");
				Window.Clear();
				Window.Display();
				Window.Closed += OnClose;
				Window.SetFramerateLimit(120);
			}
			Color GetActiveSceneBgColor()
			{
				var col = (Scene.CurrentScene?.LoadingPercent < 100 ? Scene.LoadingScene?.BackgroundColor : Scene.CurrentScene?.BackgroundColor);
				return col ?? Color.Black;
			}
		}
		public static void Stop()
		{
			Scene.CurrentScene?.GameStop();
			Window.Close();
		}

		private static void Main() { }
		private static void OnClose(object sender, EventArgs e) => Stop();
	}
}
