using System;
using SFML.Graphics;

namespace SMPL
{
	public static class Game
	{
		public static RenderWindow Window { get; private set; }

		public static void Start(Scene startingScene, Scene loadingScene = null)
		{
			InitWindow();
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
				Window = new(new(800, 600), "SMPL Game");
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
