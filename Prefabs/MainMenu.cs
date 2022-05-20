using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;
using System.Numerics;
using SMPL.Tools;
using SFML.Window;
using System;
using System.IO;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		public class Resources
		{
			public Scene PlayScene { get; set; }
			public string FontPath { get; set; }
			public string ButtonTexturePath { get; set; }
			public string LogoTexturePath { get; set; }
			public string BackgroundTexturePath { get; set; }
			public string SliderTexturePath { get; set; }
			public string TickOnTexturePath { get; set; }
			public string TickOffTexturePath { get; set; }
		}

		private bool settingsVisible;
		private readonly Resources resources;
		private static Vector2 audioPos = new(12775, 12805), gfxPos = new(7123, 45523);

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
		protected Slider VolumeMaster { get; private set; }
		protected Slider VolumeSound { get; private set; }
		protected Slider VolumeMusic { get; private set; }
		protected Slider PixelSize { get; private set; }
		protected Tick VSync { get; private set; }
		protected TextButton Settings { get; private set; }
		protected TextButton Back { get; private set; }
		protected TextButton Exit { get; private set; }

		public MainMenu(Resources resources)
		{
			if (resources == null)
				return;

			this.resources = resources;
		}

		protected override AssetQueue OnRequireAssets()
		{
			var r = resources;
			return new()
			{
				Textures = new() { r.ButtonTexturePath, r.BackgroundTexturePath, r.LogoTexturePath, r.SliderTexturePath, r.TickOffTexturePath, r.TickOnTexturePath},
				Fonts = new() { r.FontPath },
				Databases = new() { SMPL.Settings.DB_PATH },
			};
		}
		protected override void OnStart()
		{
			LoadSettingsDatabase();

			var sz = MainCamera.Texture.Size;
			var r = resources;
			Background = new() { TexturePath = r.BackgroundTexturePath, Size = new(sz.X, sz.Y), Parent = MainCamera };
			GameLogo = new() { TexturePath = r.LogoTexturePath, Size = new(sz.Y * 0.8f, sz.Y * 0.8f), Position = new(-sz.X * 0.2f, 0) };

			Back = new(new(r.FontPath, "Back"))
			{
				TexturePath = r.ButtonTexturePath,
				Parent = MainCamera,
				LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.4f),
				Size = new(250, 80)
			};
			
			if (r.PlayScene != null)
				Play = new(new(r.FontPath, "Play")) { TexturePath = r.ButtonTexturePath };

			Settings = new(new(r.FontPath, "Settings")) { TexturePath = r.ButtonTexturePath };
			Exit = new(new(r.FontPath, "Exit")) { TexturePath = r.ButtonTexturePath };

			SettingsMenu = new() { ButtonWidth = 250, TexturePath = r.SliderTexturePath, Parent = Settings };
			SettingsMenu.ScrollUp.TexturePath = r.ButtonTexturePath;
			SettingsMenu.ScrollDown.TexturePath = r.ButtonTexturePath;

			var gfx = new TextButton(new(r.FontPath, "Graphics")) { TexturePath = r.ButtonTexturePath };
			var audio = new TextButton(new(r.FontPath, "Audio")) { TexturePath = r.ButtonTexturePath };
			SettingsMenu.Buttons.Add(gfx);
			SettingsMenu.Buttons.Add(audio);

			VolumeMaster = new() { Scale = 1.4f, TexturePath = r.SliderTexturePath, Position = audioPos - new Vector2(0, 200), Value = Game.Settings.VolumeMaster };
			VolumeSound = new() { Scale = 1.4f, TexturePath = r.SliderTexturePath, Parent = VolumeMaster, LocalPosition = new(0, 150), Value = Game.Settings.VolumeSound };
			VolumeMusic = new() { Scale = 1.4f, TexturePath = r.SliderTexturePath, Parent = VolumeSound, LocalPosition = new(0, 150), Value = Game.Settings.VolumeMusic };

			PixelSize = new() { Scale = 1.4f, TexturePath = r.SliderTexturePath, Position = gfxPos - new Vector2(0, 100), Value = Game.Settings.PixelSize, RangeA = 0.1f };
			VSync = new() { TexturePath = r.TickOffTexturePath, Parent = PixelSize, LocalPosition = new(0, 150), IsActive = Game.Settings.IsVSyncEnabled };

			SubscribeButtons();

			void LoadSettingsDatabase()
			{
				var path = SMPL.Settings.DB_PATH;
				var sheetName = SMPL.Settings.DB_SHEET_NAME;

				if (File.Exists(path) == false)
					return;

				var sheet = Databases[path].GetSheet<Settings>(sheetName);
				if (sheet.Count > 0)
					Game.Settings = sheet[0];

				MainCamera.Destroy();
				var sz = VideoMode.DesktopMode;
				var pixelSize = Game.Settings.PixelSize;
				MainCamera = new((uint)(sz.Width * pixelSize), (uint)(sz.Height * pixelSize));

				Game.Window.SetVerticalSyncEnabled(Game.Settings.IsVSyncEnabled);
			}
			void SubscribeButtons()
			{
				Back.Clicked += OnBackClick;
				gfx.Clicked += OnGfxClick;
				audio.Clicked += OnAudioClick;

				Settings.Clicked += OnSettingsClick;
				Exit.Clicked += OnExitClick;

				if (Play != null)
				{
					SubscribeButton(Play);
					Play.Clicked += OnPlayClick;
				}

				for (int i = 0; i < SettingsMenu.Buttons.Count; i++)
					SubscribeButton(SettingsMenu.Buttons[i]);

				SubscribeButton(Back);
				SubscribeButton(Settings);
				SubscribeButton(Exit);

				void SubscribeButton(Button button)
				{
					button.Hovered += OnButtonHover;
					button.Unhovered += OnButtonUnhover;
					button.Pressed += OnButtonPress;
				}
			}
		}
		protected override void OnUpdate()
		{
			var qText = new QuickText(resources.FontPath, "Master", VolumeMaster.Position) { OriginUnit = new(0.5f, 0.6f), Scale = 1.5f };

			if (Mouse.IsButtonPressed(Mouse.Button.Left).Once("close-settings-menu-asgl2kj") &&
				SettingsMenu.IsHovered == false && Settings.Hitbox.IsHovered == false)
				SettingsMenuIsVisible = false;

			Background.Draw();
			GameLogo.Draw();

			if (MainCamera.Position != default)
				Back.Draw();

			UpdateAndDrawMenu();
			UpdateAndDrawAudio();
			UpdateAndDrawGfx();

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
			void UpdateAndDrawAudio()
			{
				VolumeMaster.Draw();
				VolumeSound.Draw();
				VolumeMusic.Draw();
				Game.Settings.VolumeMaster = VolumeMaster.Value;
				Game.Settings.VolumeSound = VolumeSound.Value;
				Game.Settings.VolumeMusic = VolumeMusic.Value;

				DrawText("Master", VolumeMaster.Position);
				DrawText("Sound", VolumeSound.Position);
				DrawText("Music", VolumeMusic.Position);
			}
			void UpdateAndDrawGfx()
			{
				PixelSize.Draw();
				VSync.Draw();

				Game.Settings.PixelSize = PixelSize.Value;
				Game.Settings.IsVSyncEnabled = VSync.IsActive;

				var value = (int)(PixelSize.Value * 30f) / 30f;
				DrawText($"{VideoMode.DesktopMode.Width * value}x{VideoMode.DesktopMode.Height * value}", PixelSize.Position);
				DrawText("VSync", VSync.CornerA + new Vector2(-VSync.Size.X * 0.8f, VSync.Size.Y * 0.5f));
			}
			void DrawText(string text, Vector2 position)
			{
				qText.Text = text;
				qText.Position = position;
				qText.Draw();
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

		protected virtual void OnButtonHover(Button button) => button.Tint = new(255, 255, 255);
		protected virtual void OnButtonUnhover(Button button) => button.Tint = new(220, 220, 220);
		protected virtual void OnButtonPress(Button button) => button.Tint = new(200, 200, 200);

		protected virtual void OnGfxClick(Button button) => MainCamera.Position = gfxPos;
		protected virtual void OnAudioClick(Button button) => MainCamera.Position = audioPos;
		protected virtual void OnPlayClick(Button button) => CurrentScene = resources.PlayScene ?? CurrentScene;
		protected virtual void OnBackClick(Button button)
		{
			MainCamera.Position = default;
			UpdateSettingsDatabase();
		}
		protected virtual void OnSettingsClick(Button button) => SettingsMenuIsVisible = !SettingsMenuIsVisible;
		protected virtual void OnExitClick(Button button) => Game.Stop();

		private static void UpdateSettingsDatabase()
		{
			var settingsDb = default(Database);
			var path = SMPL.Settings.DB_PATH;
			var sheetName = SMPL.Settings.DB_SHEET_NAME;

			if (File.Exists(path) == false)
			{
				settingsDb = new(path);
				var sh = settingsDb.AddSheet<Settings>(sheetName);
				sh.Add(Game.Settings);
			}
			else
			{
				settingsDb = Database.Load(path);
				var sheet = settingsDb.GetSheet<Settings>(sheetName);
				sheet.Clear();
				sheet.Add(Game.Settings);
			}
			settingsDb.SaveSheet<Settings>(sheetName);
		}
	}
}
