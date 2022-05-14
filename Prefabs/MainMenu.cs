using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		private readonly string fontPath, btnTexPath, logoTexPath, bgTexPath;

		public Sprite Background { get; private set; }
		public Sprite GameLogo { get; private set; }

		public List<TextButton> Buttons { get; } = new();

		public TextButton Settings { get; private set; }
		public TextButton Back { get; private set; }
		public TextButton Exit { get; private set; }

		public MainMenu(string fontPath = null, string buttonTexturePath = null, string logoTexturePath = null, string backgroundTexturePath = null)
		{
			bgTexPath = backgroundTexturePath;
			logoTexPath = logoTexturePath;
			btnTexPath = buttonTexturePath;
			this.fontPath = fontPath;
		}

		protected override AssetQueue OnRequireAssets()
		{
			return new() { Textures = new() { btnTexPath, bgTexPath, logoTexPath }, Fonts = new() { fontPath } };
		}
		protected override void OnStart()
		{
			var sz = MainCamera.Texture.Size;
			Background = new() { TexturePath = bgTexPath, Size = new(sz.X, sz.Y), Parent = MainCamera };
			GameLogo = new() { TexturePath = logoTexPath, Size = new(sz.Y * 0.8f, sz.Y * 0.8f), Position = new(-sz.X * 0.2f, 0) };

			Back = new(new(fontPath, "Back")) { TexturePath = btnTexPath, Parent = MainCamera };

			Settings = new(new(fontPath, "Settings")) { TexturePath = btnTexPath };
			Exit = new(new(fontPath, "Exit")) { TexturePath = btnTexPath };

			Settings.Clicked += OnSettingsClick;
			Exit.Clicked += OnExitClick;
		}

		protected virtual void OnSettingsClick(Button button)
		{
			MainCamera.Position = new(MainCamera.Texture.Size.X * 0.2f, 0);
		}
		protected virtual void OnExitClick(Button button) => Game.Stop();

		protected override void OnUpdate()
		{
			Background.Draw();
			GameLogo.Draw();

			UpdateBackButton();
			UpdateAndDrawMenu();

			void UpdateAndDrawMenu()
			{
				var sz = MainCamera.Texture.Size;
				var x = sz.X * 0.265f;
				var btnSz = Buttons.Count == 0 ? new(250, 60) : Buttons[0].LocalSize;
				var offset = 1.2f;

				for (int i = 0; i < Buttons.Count; i++)
				{
					Buttons[i].Parent = i == 0 ? null : Buttons[i - 1];
					Buttons[i].LocalSize = btnSz;
					Buttons[i].LocalPosition = i != 0 ? new(0, Buttons[i].LocalSize.Y * offset) : new(x, -(Buttons.Count + 1) * (Buttons[i].LocalSize.Y * offset) * 0.5f);
					SetButtonHitbox(Buttons[i]);
					Buttons[i].Draw();
				}
				Settings.Parent = Buttons.Count == 0 ? null : Buttons[^1];
				Settings.LocalSize = btnSz;
				Settings.LocalPosition = Buttons.Count == 0 ? new(x, 0) : new(0, Buttons[^1].LocalSize.Y * offset);
				Exit.Parent = Settings;
				Exit.LocalSize = btnSz;
				Exit.LocalPosition = new(0, Settings.LocalSize.Y * offset);

				SetButtonHitbox(Settings);
				SetButtonHitbox(Exit);
				Settings.Draw();
				Exit.Draw();
			}
			void UpdateBackButton()
			{
				Back.Draw();
			}
		}
		protected override void OnStop()
		{
			Background.Destroy(); Background = null;
			GameLogo.Destroy(); GameLogo = null;

			for (int i = 0; i < Buttons.Count; i++)
				Buttons[i].Destroy();
			Buttons.Clear();

			Settings.Destroy(); Settings = null;
			Exit.Destroy(); Exit = null;
		}

		private static void SetButtonHitbox(Button button)
		{
			button.SetDefaultHitbox();
			button.Hitbox.TransformLocalLines(button);
		}
	}
}
