using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;
using System.Numerics;
using SMPL.Tools;
using SFML.Window;
using System;
using System.IO;
using Console = SMPL.Tools.Console;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		private readonly string logoTexPath, bgTexPath;
		private readonly Scene scene;
		private readonly ThemeUI theme;
		private float sc, guiSc;
		private bool settingsVisible;
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
		protected Slider ScaleGUI { get; private set; }
		protected Tick VSync { get; private set; }
		protected ListCarousel WindowState { get; private set; }
		protected TextButton Settings { get; private set; }
		protected TextButton Back { get; private set; }
		protected TextButton Exit { get; private set; }

		public MainMenu(ThemeUI themeUI, Scene playScene = null, string logoTexturePath = null, string backgroundTexturePath = null)
		{
			if (themeUI == null)
				Console.LogError(1, $"The {nameof(themeUI)} for this {nameof(MainMenu)} is required and cannot be 'null'.");

			scene = playScene;
			theme = themeUI;
			logoTexPath = logoTexturePath;
			bgTexPath = backgroundTexturePath;
		}

		protected override AssetQueue OnRequireAssets()
		{
			var assets = new AssetQueue { Databases = new() { File.Exists(SMPL.Settings.DB_PATH) ? SMPL.Settings.DB_PATH : null } };
			assets.Textures = new() { bgTexPath, logoTexPath };
			theme?.IncludeInAssetQueue(ref assets);
			return assets;
		}
		protected override void OnStart()
		{
			var gfx = default(TextButton);
			var audio = default(TextButton);
			var sz = MainCamera.Resolution;
			var r = theme;

			TryLoadSettingsDatabase();
			InitMenu();
			InitSettings();
			SubscribeButtons();

			Background = new()
			{
				TexturePath = bgTexPath,
				Size = sz,
				Parent = MainCamera
			};

			UpdateScaleGUI();

			void InitMenu()
			{
				if (theme == null)
					return;

				GameLogo = new() { TexturePath = logoTexPath, Size = new(sz.Y * 0.8f, sz.Y * 0.8f), Position = new(-sz.X * 0.2f, 0) };

				Back = theme.CreateTextButton("Back");
				Back.LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.4f);
				Back.LocalSize = new(sz.X * 0.08f, sz.Y * 0.07f);
				Back.Parent = MainCamera;

				if (scene != null)
					Play = theme.CreateTextButton("Play");

				Settings = theme.CreateTextButton("Settings");
				Exit = theme.CreateTextButton("Exit");

				SettingsMenu = theme.CreateList();
				SettingsMenu.Parent = Settings;

				gfx = theme.CreateTextButton("Graphics");
				audio = theme.CreateTextButton("Audio");
				SettingsMenu.Buttons.Add(gfx);
				SettingsMenu.Buttons.Add(audio);
			}
			void InitSettings()
			{
				if (theme == null)
					return;

				var offset = 1.5f;
				VolumeMaster = theme.CreateSlider();
				VolumeMaster.LocalPosition = audioPos;
				VolumeMaster.Value = Game.Settings.VolumeUnitMaster;

				VolumeSound = theme.CreateSlider();
				VolumeSound.Parent = VolumeMaster;
				VolumeSound.LocalPosition = new(0, VolumeMaster.Size.Y * offset);
				VolumeSound.Value = Game.Settings.VolumeUnitSound;

				VolumeMusic = theme.CreateSlider();
				VolumeMusic.Parent = VolumeSound;
				VolumeMusic.LocalPosition = new(0, VolumeSound.Size.Y * offset);
				VolumeMusic.Value = Game.Settings.VolumeUnitMusic;

				PixelSize = theme.CreateSlider();
				PixelSize.LocalPosition = gfxPos;
				PixelSize.Value = Game.Settings.ResolutionScale;
				PixelSize.RangeA = 0.2f;

				ScaleGUI = theme.CreateSlider();
				ScaleGUI.Parent = PixelSize;
				ScaleGUI.LocalPosition = new(0, PixelSize.Size.Y * offset);
				ScaleGUI.RangeA = 0.35f;
				ScaleGUI.RangeB = 2.5f;
				ScaleGUI.Value = Game.Settings.ScaleGUI;

				VSync = theme.CreateTick();
				VSync.Parent = ScaleGUI;
				VSync.LocalPosition = new(0, ScaleGUI.Size.Y * offset);
				VSync.IsActive = Game.Settings.IsVSyncEnabled;
				VSync.TexturePath = VSync.IsActive ? theme.TickOnTexturePath : theme.TickOffTexturePath;
			}
			void SubscribeButtons()
			{
				if (theme == null)
					return;

				Back.Clicked += OnBackClick;
				gfx.Clicked += OnGfxClick;
				audio.Clicked += OnAudioClick;
				VSync.Clicked += OnVSyncClick;

				Settings.Clicked += OnSettingsClick;
				Exit.Clicked += OnExitClick;

				if (Play != null)
					Play.Clicked += OnPlayClick;
			}
		}
		protected override void OnUpdate()
		{
			if (theme == null)
				return;

			var qText = theme.CreateQuickText("Master", VolumeMaster.Position);
			qText.OriginUnit = new(0.5f, 0.6f);
			qText.Scale = 1.5f;

			if (Mouse.IsButtonPressed(Mouse.Button.Left).Once("close-settings-menu-asgl2kj") &&
				SettingsMenu.IsHovered == false && Settings.Hitbox.IsHovered == false)
				SettingsMenuIsVisible = false;

			UpdateAndDrawMenu();
			UpdateAndDrawAudio();
			UpdateAndDrawGfx();

			void UpdateAndDrawMenu()
			{
				var sz = MainCamera.Resolution;
				var x = sz.X * 0.24f;
				var btnSz = Buttons.Count == 0 ? new(200, 80) : Buttons[0].LocalSize;
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
				}

				Background.Size = sz;

				GameLogo.Position = new(-sz.X * 0.2f, 0);
				GameLogo.Size = new(sz.Y * 0.8f, sz.Y * 0.8f);

				Back.LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.4f);

				SettingsMenu.ButtonWidth = 150;
				SettingsMenu.MaxLength = 300;
				SettingsMenu.LocalPosition =
					new(Settings.LocalSize.X * 0.65f + SettingsMenu.ButtonWidth,
					-MathF.Max(SettingsMenu.VisibleButtonCountCurrent - 1, 0) * (SettingsMenu.ButtonHeight + SettingsMenu.ButtonSpacing));

				Settings.Parent = Buttons.Count == 0 ? Play : Buttons[^1];
				Settings.LocalSize = btnSz;
				Settings.LocalPosition = Buttons.Count == 0 && Play == null ? new(x, 0) : new(0, btnSz.Y * offset);

				Exit.Parent = Settings;
				Exit.LocalSize = btnSz;
				Exit.LocalPosition = new(0, Settings.LocalSize.Y * offset);

				Background.Draw();
				GameLogo.Draw();
				Settings.Draw();
				Exit.Draw();

				if (SettingsMenuIsVisible)
					SettingsMenu.Draw();

				if (MainCamera.Position != default)
				{
					Back.Parent = MainCamera;
					Back.Draw();
				}

				if (Play != null)
					Play.Draw();
			}
			void UpdateAndDrawAudio()
			{
				VolumeMaster.Draw();
				VolumeSound.Draw();
				VolumeMusic.Draw();
				Game.Settings.VolumeUnitMaster = VolumeMaster.Value;
				Game.Settings.VolumeUnitSound = VolumeSound.Value;
				Game.Settings.VolumeUnitMusic = VolumeMusic.Value;

				DrawText("Master", VolumeMaster.Position);
				DrawText("Sound", VolumeSound.Position);
				DrawText("Music", VolumeMusic.Position);
			}
			void UpdateAndDrawGfx()
			{
				PixelSize.Draw();
				ScaleGUI.Draw();
				VSync.Draw();

				Game.Settings.ResolutionScale = PixelSize.Value;
				Game.Settings.ScaleGUI = ScaleGUI.Value;

				var value = (int)(PixelSize.Value * 30f) / 30f;
				DrawText($"Resolution [{VideoMode.DesktopMode.Width * value}x{VideoMode.DesktopMode.Height * value}]", PixelSize.Position);
				DrawText($"GUI Scale [x{ScaleGUI.Value:F2}]", ScaleGUI.Position);
				DrawText("VSync", VSync.CornerA + new Vector2(-VSync.Size.X * 1.1f, VSync.Size.Y * 0.5f));
			}
			void DrawText(string text, Vector2 position)
			{
				qText.Scale = sc * guiSc;
				qText.Text = text;
				qText.Position = position;
				qText.Draw();
			}
		}
		protected override void OnStop()
		{
			if (theme == null)
				return;

			Background.Destroy(); Background = null;
			GameLogo.Destroy(); GameLogo = null;

			for (int i = 0; i < Buttons.Count; i++)
				Buttons[i].Destroy();
			Buttons.Clear();

			Settings.Destroy(); Settings = null;
			Exit.Destroy(); Exit = null;
		}

		private void OnVSyncClick() => Game.Settings.IsVSyncEnabled = VSync.IsActive;
		private void OnGfxClick() => MainCamera.Position = gfxPos;
		private void OnAudioClick() => MainCamera.Position = audioPos;
		private void OnPlayClick() => CurrentScene = scene ?? CurrentScene;
		private void OnBackClick()
		{
			sc = Game.Settings.ResolutionScale;
			guiSc = ScaleGUI.Value;

			MainCamera.Position = default;
			UpdateSettingsDatabase();
			TryLoadSettingsDatabase();

			UpdateScaleGUI();
		}
		private void OnSettingsClick() => SettingsMenuIsVisible = !SettingsMenuIsVisible;
		private void OnExitClick() => Game.Stop();

		private void UpdateScaleGUI()
		{
			if (theme == null)
				return;

			var s = sc * guiSc;

			if (Play != null)
				Play.Scale = s;

			VolumeMaster.Scale = s;
			VolumeSound.Scale = s;
			VolumeMusic.Scale = s;
			PixelSize.Scale = s;
			VSync.Scale = s;
			Back.Scale = s;
			Settings.Scale = s;
			Exit.Scale = s;
		}
		private void TryLoadSettingsDatabase()
		{
			var path = SMPL.Settings.DB_PATH;
			var sheetName = SMPL.Settings.DB_SHEET_NAME;

			if (File.Exists(path) == false || CurrentScene.Databases.ContainsKey(path) == false)
				return;

			var sheet = CurrentScene.Databases[path].GetSheet<Settings>(sheetName);
			if (sheet.Count > 0)
				Game.Settings = sheet[0];

			MainCamera.Destroy();
			var sz = VideoMode.DesktopMode;
			sc = Game.Settings.ResolutionScale;
			guiSc = Game.Settings.ScaleGUI;
			MainCamera = new((uint)(sz.Width * sc), (uint)(sz.Height * sc));

			if (Background != null)
			{
				Background.Parent = MainCamera;
				Background.LocalPosition = new();
			}
		}

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
