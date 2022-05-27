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
		private float sc, guiSc;

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
		protected Slider ScaleGUI { get; private set; }
		protected Tick VSync { get; private set; }
		protected ListCarousel WindowState { get; private set; }
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
				Textures = new() { r.ButtonTexturePath, r.BackgroundTexturePath, r.LogoTexturePath, r.SliderTexturePath, r.TickOffTexturePath, r.TickOnTexturePath },
				Fonts = new() { r.FontPath },
				Databases = new() { File.Exists(SMPL.Settings.DB_PATH) ? SMPL.Settings.DB_PATH : null },
			};
		}
		protected override void OnStart()
		{
			var gfx = default(TextButton);
			var audio = default(TextButton);
			var sz = MainCamera.Resolution;
			var r = resources;

			TryLoadSettingsDatabase();
			InitMenu();
			InitSettings();
			SubscribeButtons();

			Background = new()
			{
				TexturePath = r.BackgroundTexturePath,
				Size = sz,
				Parent = MainCamera
			};

			UpdateScaleGUI();

			void InitMenu()
			{
				GameLogo = new() { TexturePath = r.LogoTexturePath, Size = new(sz.Y * 0.8f, sz.Y * 0.8f), Position = new(-sz.X * 0.2f, 0) };

				Back = new(new(r.FontPath, "Back"))
				{
					TexturePath = r.ButtonTexturePath,
					LocalPosition = new(-sz.X * 0.4f, sz.Y * 0.4f),
					LocalSize = new(sz.X * 0.08f, sz.Y * 0.07f),
					Parent = MainCamera,
				};

				if (r.PlayScene != null)
					Play = new(new(r.FontPath, "Play")) { TexturePath = r.ButtonTexturePath };

				Settings = new(new(r.FontPath, "Settings")) { TexturePath = r.ButtonTexturePath };
				Exit = new(new(r.FontPath, "Exit")) { TexturePath = r.ButtonTexturePath };

				SettingsMenu = new() { TexturePath = r.SliderTexturePath, Parent = Settings };
				SettingsMenu.ScrollUp.TexturePath = r.ButtonTexturePath;
				SettingsMenu.ScrollDown.TexturePath = r.ButtonTexturePath;

				gfx = new TextButton(new(r.FontPath, "Graphics")) { TexturePath = r.ButtonTexturePath };
				audio = new TextButton(new(r.FontPath, "Audio")) { TexturePath = r.ButtonTexturePath };
				SettingsMenu.Buttons.Add(gfx);
				SettingsMenu.Buttons.Add(audio);
			}
			void InitSettings()
			{
				var offset = 1.5f;
				VolumeMaster = new()
				{
					TexturePath = r.SliderTexturePath,
					LocalPosition = audioPos,
					Value = Game.Settings.VolumeUnitMaster,
				};
				VolumeSound = new()
				{
					TexturePath = r.SliderTexturePath,
					Parent = VolumeMaster,
					LocalPosition = new(0, VolumeMaster.Size.Y * offset),
					Value = Game.Settings.VolumeUnitSound
				};
				VolumeMusic = new()
				{
					TexturePath = r.SliderTexturePath,
					Parent = VolumeSound,
					LocalPosition = new(0, VolumeSound.Size.Y * offset),
					Value = Game.Settings.VolumeUnitMusic
				};
				PixelSize = new()
				{
					TexturePath = r.SliderTexturePath,
					LocalPosition = gfxPos,
					Value = Game.Settings.ResolutionScale,
					RangeA = 0.2f,
				};
				ScaleGUI = new()
				{
					TexturePath = r.SliderTexturePath,
					Parent = PixelSize,
					LocalPosition = new(0, PixelSize.Size.Y * offset),
					RangeA = 0.35f,
					RangeB = 2.5f,
					Value = Game.Settings.ScaleGUI,
				};
				VSync = new()
				{
					TexturePath = Game.Settings.IsVSyncEnabled ? r.TickOnTexturePath : r.TickOffTexturePath,
					Parent = ScaleGUI,
					LocalPosition = new(0, ScaleGUI.Size.Y * offset),
					IsActive = Game.Settings.IsVSyncEnabled
				};
				WindowState = new();

				SubscribeButton(VSync);
			}
			void SubscribeButtons()
			{
				Back.Clicked += OnBackClick;
				gfx.Clicked += OnGfxClick;
				audio.Clicked += OnAudioClick;
				VSync.Clicked += OnVSyncClick;

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
			}
		}
		protected override void OnUpdate()
		{
			var qText = new QuickText(resources.FontPath, "Master", VolumeMaster.Position) { OriginUnit = new(0.5f, 0.6f), Scale = 1.5f };

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
		protected virtual void OnTickClick(Tick tick) => tick.TexturePath = tick.IsActive ? resources.TickOnTexturePath : resources.TickOffTexturePath;

		private void OnVSyncClick(Button button)
		{
			var tick = (Tick)button;
			Game.Settings.IsVSyncEnabled = (tick).IsActive;
			OnTickClick(tick);
		}
		private void OnGfxClick(Button button) => MainCamera.Position = gfxPos;
		private void OnAudioClick(Button button) => MainCamera.Position = audioPos;
		private void OnPlayClick(Button button) => CurrentScene = resources.PlayScene ?? CurrentScene;
		private void OnBackClick(Button button)
		{
			sc = Game.Settings.ResolutionScale;
			guiSc = ScaleGUI.Value;

			MainCamera.Position = default;
			UpdateSettingsDatabase();
			TryLoadSettingsDatabase();

			UpdateScaleGUI();
		}
		private void OnSettingsClick(Button button) => SettingsMenuIsVisible = !SettingsMenuIsVisible;
		private void OnExitClick(Button button) => Game.Stop();

		private void UpdateScaleGUI()
		{
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
		private void SubscribeButton(Button button)
		{
			button.Hovered += OnButtonHover;
			button.Unhovered += OnButtonUnhover;
			button.Pressed += OnButtonPress;
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
