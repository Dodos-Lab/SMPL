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
		private static Vector2 audioPos = new(12775, 12805), gfxPos = new(7123, 45523);

		private readonly string logoTexPath, bgTexPath;
		private float guiSc;
		private bool settingsVisible, reload;
		private readonly Scene scene;
		private TextButton play;
		private Slider volumeMaster, volumeSound, volumeMusic, scaleGUI;
		private ListDropdown resolution;
		private Tick vSync;
		private ListCarousel windowState;
		private TextButton settings, back, exit;

		private bool SettingsMenuIsVisible
		{
			get => settingsVisible;
			set
			{
				settingsVisible = value;
				SettingsMenu.IsDisabled = value == false;
				SettingsMenu.IsHidden = value == false;
			}
		}

		protected Sprite Background { get; set; }
		protected Sprite GameLogo { get; set; }

		protected List SettingsMenu { get; private set; }
		protected List<TextButton> Buttons { get; } = new();

		public MainMenu(ThemeUI themeUI, Scene playScene = null, string logoTexturePath = null, string backgroundTexturePath = null)
		{
			if (themeUI == null)
				Console.LogError(1, $"The {nameof(themeUI)} for this {nameof(MainMenu)} is required and cannot be 'null'.");

			scene = playScene;
			ThemeUI = themeUI;
			logoTexPath = logoTexturePath;
			bgTexPath = backgroundTexturePath;
		}

		protected override AssetQueue OnRequireAssets()
		{
			var assets = new AssetQueue { Databases = new() { File.Exists(SMPL.Settings.DB_PATH) ? SMPL.Settings.DB_PATH : null } };
			assets.Textures = new() { bgTexPath, logoTexPath };
			ThemeUI?.IncludeInAssetQueue(ref assets);
			return assets;
		}
		protected override void OnStart()
		{
			var gfx = default(TextButton);
			var audio = default(TextButton);
			var sz = MainCamera.Resolution;
			var r = ThemeUI;

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
				if (ThemeUI == null)
					return;

				GameLogo = new() { TexturePath = logoTexPath, Size = new(sz.Y * 0.8f, sz.Y * 0.8f), Position = new(-sz.X * 0.2f, 0) };

				back = ThemeUI.CreateTextButton("Back");
				back.LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.4f);
				back.LocalSize = new(sz.X * 0.08f, sz.Y * 0.07f);
				back.Parent = MainCamera;

				if (scene != null)
					play = ThemeUI.CreateTextButton("Play");

				settings = ThemeUI.CreateTextButton("Settings");
				exit = ThemeUI.CreateTextButton("Exit");

				SettingsMenu = ThemeUI.CreateList();
				SettingsMenu.Parent = settings;

				gfx = ThemeUI.CreateTextButton("Graphics");
				audio = ThemeUI.CreateTextButton("Audio");
				SettingsMenu.Buttons.Add(gfx);
				SettingsMenu.Buttons.Add(audio);
			}
			void InitSettings()
			{
				if (ThemeUI == null)
					return;

				var offset = 2f;
				volumeMaster = ThemeUI.CreateSlider();
				volumeMaster.LocalPosition = audioPos;
				volumeMaster.Value = Game.Settings.VolumeUnitMaster;

				volumeSound = ThemeUI.CreateSlider();
				volumeSound.Parent = volumeMaster;
				volumeSound.LocalPosition = new(0, volumeMaster.Size.Y * offset);
				volumeSound.Value = Game.Settings.VolumeUnitSound;

				volumeMusic = ThemeUI.CreateSlider();
				volumeMusic.Parent = volumeSound;
				volumeMusic.LocalPosition = new(0, volumeSound.Size.Y * offset);
				volumeMusic.Value = Game.Settings.VolumeUnitMusic;

				var resolutions = Settings.SupportedMonitorResolutions;
				resolution = ThemeUI.CreateListDropdown();
				resolution.LocalPosition = gfxPos + new Vector2(MainCamera.Resolution.X * 0.4f, -MainCamera.Resolution.Y * 0.15f);
				for (int i = 0; i < resolutions.Count; i++)
					resolution.Buttons.Add(ThemeUI.CreateTextButton($"{resolutions[i].X}x{resolutions[i].Y}"));

				scaleGUI = ThemeUI.CreateSlider();
				scaleGUI.LocalPosition = gfxPos + new Vector2(-MainCamera.Resolution.X * 0.15f, -MainCamera.Resolution.Y * 0.15f);
				scaleGUI.RangeA = 0.35f;
				scaleGUI.RangeB = 2.5f;
				scaleGUI.Value = Game.Settings.ScaleGUI;

				vSync = ThemeUI.CreateTick();
				vSync.Parent = scaleGUI;
				vSync.LocalPosition = new(0, scaleGUI.Size.Y * offset);
				vSync.IsActive = Game.Settings.IsVSyncEnabled;
				vSync.TexturePath = vSync.IsActive ? ThemeUI.TickOnTexturePath : ThemeUI.TickOffTexturePath;

				windowState = ThemeUI.CreateListCarousel();
				windowState.Parent = vSync;
				windowState.LocalPosition = new(0, vSync.Size.Y * offset);
				windowState.ButtonWidth = 200;
				windowState.Buttons.Add(ThemeUI.CreateTextButton("Windowed"));
				windowState.Buttons.Add(ThemeUI.CreateTextButton("Borderless"));
				windowState.Buttons.Add(ThemeUI.CreateTextButton("Fullscreen"));
				var index = (int)Game.Settings.WindowState;
				windowState.SelectionIndex = index;
			}
			void SubscribeButtons()
			{
				if (ThemeUI == null)
					return;

				back.Clicked += OnBackClick;
				gfx.Clicked += OnGfxClick;
				audio.Clicked += OnAudioClick;
				vSync.Clicked += OnVSyncClick;

				settings.Clicked += OnSettingsClick;
				exit.Clicked += OnExitClick;

				if (play != null)
					play.Clicked += OnPlayClick;
			}
		}
		protected override void OnUpdate()
		{
			if (ThemeUI == null)
				return;

			var qText = ThemeUI.CreateQuickText("Master", volumeMaster.Position);
			qText.OriginUnit = new(0.5f, 0.6f);
			qText.Scale = 1.5f;

			if (Mouse.IsButtonPressed(Mouse.Button.Left).Once("close-settings-menu-asgl2kj") &&
				SettingsMenu.IsHovered == false && settings.Hitbox.IsHovered == false)
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
				if (play != null)
				{
					play.Parent = Buttons.Count == 0 ? null : Buttons[0];
					play.LocalSize = btnSz;
					play.LocalPosition = new(Buttons.Count == 0 ? x : 0, -btnSz.Y * offset);
				}

				Background.Size = sz;

				GameLogo.Position = new(-sz.X * 0.2f, 0);
				GameLogo.Size = new(sz.Y * 0.8f, sz.Y * 0.8f);

				back.LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.4f);

				SettingsMenu.ButtonWidth = 150;
				SettingsMenu.MaxLength = 300;
				SettingsMenu.LocalPosition =
					new(settings.LocalSize.X * 0.65f + SettingsMenu.ButtonWidth,
					-MathF.Max(SettingsMenu.VisibleButtonCountCurrent - 1, 0) * (SettingsMenu.ButtonHeight + SettingsMenu.ButtonSpacing));

				settings.Parent = Buttons.Count == 0 ? play : Buttons[^1];
				settings.LocalSize = btnSz;
				settings.LocalPosition = Buttons.Count == 0 && play == null ? new(x, 0) : new(0, btnSz.Y * offset);

				exit.Parent = settings;
				exit.LocalSize = btnSz;
				exit.LocalPosition = new(0, settings.LocalSize.Y * offset);

				Background.Draw();
				GameLogo.Draw();
				settings.Draw();
				exit.Draw();

				if (SettingsMenuIsVisible)
					SettingsMenu.Draw();

				if (MainCamera.Position != default)
				{
					back.Parent = MainCamera;
					back.Draw();
				}

				if (play != null)
					play.Draw();
			}
			void UpdateAndDrawAudio()
			{
				volumeMaster.Draw();
				volumeSound.Draw();
				volumeMusic.Draw();
				Game.Settings.VolumeUnitMaster = volumeMaster.Value;
				Game.Settings.VolumeUnitSound = volumeSound.Value;
				Game.Settings.VolumeUnitMusic = volumeMusic.Value;

				DrawText("Master", volumeMaster.Position);
				DrawText("Sound", volumeSound.Position);
				DrawText("Music", volumeMusic.Position);
			}
			void UpdateAndDrawGfx()
			{
				resolution.Draw();
				scaleGUI.Draw();
				vSync.Draw();
				windowState.Draw();

				//Game.Settings.ResolutionScale = pixelSize.Value;
				Game.Settings.ScaleGUI = scaleGUI.Value;
				Game.Settings.WindowState = (Settings.WindowStates)windowState.SelectionIndex;

				DrawText($"GUI Scale [x{scaleGUI.Value:F2}]", scaleGUI.Position);
				DrawText("VSync", vSync.CornerA + new Vector2(-vSync.Size.X * 1.1f, vSync.Size.Y * 0.5f));
			}
			void DrawText(string text, Vector2 position)
			{
				qText.Scale = guiSc;
				qText.Text = text;
				qText.Position = position;
				qText.Draw();
			}
		}
		protected override void OnStop()
		{
			if (ThemeUI == null)
				return;

			Background.Destroy(); Background = null;
			GameLogo.Destroy(); GameLogo = null;

			for (int i = 0; i < Buttons.Count; i++)
				Buttons[i].Destroy();
			Buttons.Clear();

			settings.Destroy(); settings = null;
			exit.Destroy(); exit = null;

			if (reload)
			{
				reload = false;
				CurrentScene = this;
			}
		}

		private void OnVSyncClick() => Game.Settings.IsVSyncEnabled = vSync.IsActive;
		private void OnGfxClick() => MainCamera.Position = gfxPos;
		private void OnAudioClick() => MainCamera.Position = audioPos;
		private void OnPlayClick() => CurrentScene = scene ?? CurrentScene;
		private void OnBackClick()
		{
			guiSc = scaleGUI.Value;

			MainCamera.Position = default;
			UpdateSettingsDatabase();
			TryLoadSettingsDatabase();

			UpdateScaleGUI();
		}
		private void OnSettingsClick() => SettingsMenuIsVisible = !SettingsMenuIsVisible;
		private void OnExitClick() => Game.Stop();

		private void UpdateScaleGUI()
		{
			if (ThemeUI == null)
				return;

			var s = guiSc;

			if (play != null)
				play.Scale = s;

			volumeMaster.Scale = s;
			volumeSound.Scale = s;
			volumeMusic.Scale = s;
			resolution.Scale = s;
			back.Scale = s;
			settings.Scale = s;
			exit.Scale = s;
			scaleGUI.Scale = s;
			//vSync.Scale = s;
			//windowState.Scale = s;
		}
		private void TryLoadSettingsDatabase()
		{
			var path = Settings.DB_PATH;
			var sheetName = Settings.DB_SHEET_NAME;

			if (File.Exists(path) == false || CurrentScene.Databases.ContainsKey(path) == false)
				return;

			var sheet = CurrentScene.Databases[path].GetSheet<Settings>(sheetName);
			if (sheet.Count > 0)
				Game.Settings = sheet[0];

			UpdateWindowState();

			MainCamera.Destroy();
			var sz = Game.Settings.Resolution;
			guiSc = Game.Settings.ScaleGUI;
			MainCamera = new((uint)(sz.X), (uint)(sz.Y));

			if (Background != null)
			{
				Background.Parent = MainCamera;
				Background.LocalPosition = new();
			}
		}

		private static void UpdateWindowState()
		{
			if (Game.currWindowStyle.ToWindowStates() == Game.Settings.WindowState)
				return;

			Game.Window.Dispose();
			Game.InitWindow(Game.Settings.WindowState);
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
