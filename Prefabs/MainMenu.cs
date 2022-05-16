using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;
using System.Numerics;
using SMPL.Tools;
using SFML.Window;
using System;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		private bool settingsVisible;
		private readonly string fontPath, btnTexPath, logoTexPath, bgTexPath, sliderTexPath;
		private static Vector2 audioPos = new(12775, 12805), gfxPos = new(7123, 45523);
		private readonly Scene playScene;

		protected Sprite Background { get; private set; }
		protected Sprite GameLogo { get; private set; }

		protected List SettingsMenu { get; private set; }
		protected List<TextButton> Buttons { get; } = new();

		protected bool SettingsMenuIsVisible
		{
			get => settingsVisible;
			set
			{
				settingsVisible = value;
				SettingsMenu.IsDisabled = value == false;
				SettingsMenu.IsHidden = value == false;
			}
		}
		protected TextButton Play { get; private set; }
		protected Slider VolumeAudio { get; private set; }
		protected Slider VolumeMusic { get; private set; }
		protected TextButton Settings { get; private set; }
		protected TextButton Back { get; private set; }
		protected TextButton Exit { get; private set; }

		public MainMenu(string fontPath = null, string buttonTexPath = null, string logoTexPath = null, string bgTexPath = null, string sliderTexPath = null,
			Scene playScene = null)
		{
			this.bgTexPath = bgTexPath;
			this.logoTexPath = logoTexPath;
			this.sliderTexPath = sliderTexPath;
			btnTexPath = buttonTexPath;
			this.fontPath = fontPath;
			this.playScene = playScene;
		}

		protected override AssetQueue OnRequireAssets() => new() { Textures = new() { btnTexPath, bgTexPath, logoTexPath }, Fonts = new() { fontPath } };
		protected override void OnStart()
		{
			var sz = MainCamera.Texture.Size;
			Background = new() { TexturePath = bgTexPath, Size = new(sz.X, sz.Y), Parent = MainCamera };
			GameLogo = new() { TexturePath = logoTexPath, Size = new(sz.Y * 0.8f, sz.Y * 0.8f), Position = new(-sz.X * 0.2f, 0) };

			Back = new(new(fontPath, "Back")) { TexturePath = btnTexPath, Parent = MainCamera, LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.45f) };
			Back.Clicked += OnBackClick;

			if (playScene != null)
			{
				Play = new(new(fontPath, "Play")) { TexturePath = btnTexPath };
				Play.Clicked += OnPlayClick;
			}

			Settings = new(new(fontPath, "Settings")) { TexturePath = btnTexPath };
			Exit = new(new(fontPath, "Exit")) { TexturePath = btnTexPath };

			SettingsMenu = new() { ButtonWidth = 250, TexturePath = sliderTexPath, Parent = Settings };
			SettingsMenu.ScrollUp.TexturePath = btnTexPath;
			SettingsMenu.ScrollDown.TexturePath = btnTexPath;

			var gfx = new TextButton(new(fontPath, "Graphics")) { TexturePath = btnTexPath };
			var audio = new TextButton(new(fontPath, "Audio")) { TexturePath = btnTexPath };
			gfx.Clicked += OnGfxClick;
			audio.Clicked += OnAudioClick;
			SettingsMenu.Buttons.Add(gfx);
			SettingsMenu.Buttons.Add(audio);

			Settings.Clicked += OnSettingsClick;
			Exit.Clicked += OnExitClick;
		}

		protected virtual void OnGfxClick(Button button) => MainCamera.Position = gfxPos;
		protected virtual void OnAudioClick(Button button) => MainCamera.Position = audioPos;
		protected virtual void OnPlayClick(Button button) => CurrentScene = playScene ?? CurrentScene;
		protected virtual void OnBackClick(Button button) => MainCamera.Position = default;
		protected virtual void OnSettingsClick(Button button) => SettingsMenuIsVisible = !SettingsMenuIsVisible;
		protected virtual void OnExitClick(Button button) => Game.Stop();

		protected override void OnUpdate()
		{
			if (Mouse.IsButtonPressed(Mouse.Button.Left).Once("close-settings-menu-asgl2kj") &&
				SettingsMenu.IsHovered == false && Settings.Hitbox.IsHovered == false)
				SettingsMenuIsVisible = false;

			Background.Draw();
			GameLogo.Draw();

			if (MainCamera.Position != default)
				Back.Draw();

			UpdateAndDrawMenu();

			void UpdateAndDrawMenu()
			{
				var sz = MainCamera.Texture.Size;
				var x = sz.X * 0.24f;
				var btnSz = Buttons.Count == 0 ? new(300, 80) : Buttons[0].LocalSize;
				var offset = 1.2f;

				for (int i = 0; i < Buttons.Count; i++)
				{
					Buttons[i].Parent = i == 0 ? null : Buttons[i - 1];
					Buttons[i].LocalSize = btnSz;
					Buttons[i].LocalPosition = i != 0 ? new(0, btnSz.Y * offset) : new(x, -(Buttons.Count + 1) * (btnSz.Y * offset) * 0.5f);
					Buttons[i].Draw();
				}
				if (Play != null)
				{
					Play.Parent = Buttons.Count == 0 ? null : Buttons[0];
					Play.LocalSize = btnSz;
					Play.LocalPosition = new(Buttons.Count == 0 ? x : 0, -btnSz.Y * offset);
					Play.Draw();
				}
				if (SettingsMenuIsVisible)
					SettingsMenu.Draw();

				SettingsMenu.LocalPosition = new(450, -MathF.Max(SettingsMenu.VisibleButtonCountCurrent - 1, 0) * (SettingsMenu.ButtonHeight + SettingsMenu.ButtonSpacing));

				Settings.Parent = Buttons.Count == 0 ? Play : Buttons[^1];
				Settings.LocalSize = btnSz;
				Settings.LocalPosition = Buttons.Count == 0 && Play == null ? new(x, 0) : new(0, btnSz.Y * offset);
				Exit.Parent = Settings;
				Exit.LocalSize = btnSz;
				Exit.LocalPosition = new(0, Settings.LocalSize.Y * offset);

				Settings.Draw();
				Exit.Draw();
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
	}
}
