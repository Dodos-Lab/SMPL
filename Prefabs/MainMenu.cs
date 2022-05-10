using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		private Button settings, exit;
		private Sprite background = new(), panel = new();

		public string PanelTexturePath { get; set; }
		public string BackgroundTexturePath { get; set; }
		public List<Button> Buttons { get; private set; } = new();
		public Button Settings
		{
			get => settings;
			set
			{
				settings = value;
				if (settings == null)
					settings.Clicked -= OnSettingsClick;
				else
					settings.Clicked += OnSettingsClick;
			}
		}
		public Button Exit
		{
			get => exit;
			set
			{
				exit = value;
				if (exit == null)
					exit.Clicked -= OnExitClick;
				else
					exit.Clicked += OnExitClick;
			}
		}

		protected sealed override void OnUpdate()
		{
			MainCamera.Position = new();
			MainCamera.Angle = 0;
			MainCamera.Scale = 1;

			if (BackgroundTexturePath != null && CurrentScene.Textures.ContainsKey(BackgroundTexturePath))
			{
				background.TexturePath = BackgroundTexturePath;
				background.Size = new(MainCamera.Texture.Size.X, MainCamera.Texture.Size.Y);
				background.Draw();
			}

			if (PanelTexturePath != null && CurrentScene.Textures.ContainsKey(PanelTexturePath))
			{
				panel.TexturePath = PanelTexturePath;
				panel.Position = new(-MainCamera.Texture.Size.X * 0.15f, 0);
				panel.Size = new(MainCamera.Texture.Size.X * 0.5f, MainCamera.Texture.Size.Y * 0.85f);
				panel.Draw();
			}

			var h = GetButtonHeight() * 1.2f;
			for (int i = 0; i < Buttons.Count; i++)
			{
				var btn = Buttons[i];
				var y = i * h - (h * Buttons.Count * 0.5f) - (h * 0.52f);
				DrawButton(btn, y);
			}
			DrawButton(Settings, Buttons.Count > 0 ? Buttons[^1].Position.Y + h : 0);
			DrawButton(Exit, Buttons.Count > 0 ? Buttons[^1].Position.Y + h * 2 : h);
		}
		protected override void OnStop()
		{
			for (int i = 0; i < Buttons.Count; i++)
				Buttons[i].Destroy();

			background.Destroy();
			panel.Destroy();
			Settings.Destroy();
			Exit.Destroy();

			background = null;
			panel = null;
			Buttons = null;
			Settings = null;
			Exit = null;
		}

		private void OnSettingsClick()
		{

		}
		private void OnExitClick() => Game.Stop();

		private static float GetButtonHeight() => MainCamera.Texture.Size.Y * 0.08f;
		private static void DrawButton(Button button, float y)
		{
			if (button == null)
				return;

			button.Size = new(MainCamera.Texture.Size.X * 0.2f, MainCamera.Texture.Size.Y * 0.08f);
			button.Position = new(MainCamera.Texture.Size.X * 0.3f, y);
			button.SetDefaultHitbox();
			button.Hitbox.TransformLocalLines(button);
			button.Draw();
		}
	}
}
