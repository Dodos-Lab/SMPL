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
			//var texture = new Texture("tileset.png");

			while (Window.IsOpen)
			{
				Window.DispatchEvents();
				Window.Clear(GetActiveSceneBgColor());

				Time.Update();
				Scene.UpdateCurrentScene();

				//var vertsArr = new Vertex[4];
				//var c = Color.White;
				//
				//vertsArr[0] = (new(new(0, 0), c, new(0, 0)));
				//vertsArr[1] = (new(new(96, 0), c, new(16, 0)));
				//vertsArr[2] = (new(new(96, 96), c, new(16, 16)));
				//vertsArr[3] = (new(new(0, 96), c, new(0, 16)));
				//
				//Window.Draw(vertsArr, PrimitiveType.Quads, new(BlendMode.Alpha, Transform.Identity, texture, null));

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
