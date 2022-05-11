using SFML.Graphics;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;
using System.Collections.Generic;
using System.Numerics;
using Sprite = SMPL.Graphics.Sprite;
using SFML.Window;

namespace SMPL.Prefabs
{
	public class MainMenu : Scene
	{
		private Button settings, exit, audio, graphics;
		private Sprite background = new(), panel = new();
		private bool audioVisible, graphicsVisible;
		private Object volumeSound = new() { Scale = 1.82f }, volumeMusic = new() { Scale = 1.82f }, pixelSize = new() { Scale = 1.75f },
			resolution = new() { Scale = 1.5f };

		public bool PanelIsShown { get; set; }
		public string LogoTexturePath { get; set; }
		public string PanelTexturePath { get; set; }
		public string BackgroundTexturePath { get; set; }

		public string FontPath { get; set; }
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
		public List SettingsButtons { get; private set; }
		public Slider VolumeSound { get; set; }
		public Slider VolumeMusic { get; set; }
		public Slider Resolution { get; set; }
		public Tick VSync { get; set; }
		public Button Audio
		{
			get => audio;
			set
			{
				audio = value;
				if (audio == null)
					audio.Clicked -= OnAudioClick;
				else
				{
					audio.IsHidden = true;
					audio.IsDisabled = true;
					audio.Clicked += OnAudioClick;
				}
			}
		}
		public Button Graphics
		{
			get => graphics;
			set
			{
				graphics = value;
				if (graphics == null)
					graphics.Clicked -= OnGraphicsClick;
				else
				{
					graphics.IsHidden = true;
					graphics.IsDisabled = true;
					graphics.Clicked += OnGraphicsClick;
				}
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

		protected override void OnStart()
		{
			SettingsButtons = new();
		}
		protected override void OnUpdate()
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
				panel.TexturePath = PanelIsShown ? PanelTexturePath : LogoTexturePath;
				panel.Position = new(-MainCamera.Texture.Size.X * 0.15f, 0);
				panel.Size = new(MainCamera.Texture.Size.X * 0.5f, MainCamera.Texture.Size.Y * 0.85f);
				panel.Draw();
			}

			var h = GetMainMenuButtonHeight() * 1.2f;
			for (int i = 0; i < Buttons.Count; i++)
			{
				var btn = Buttons[i];
				var y = i * h - (h * Buttons.Count * 0.5f) - (h * 0.52f);
				TryDrawMainMenuButton(btn, y);
			}
			TryDrawMainMenuButton(Settings, Buttons.Count > 0 ? Buttons[^1].Position.Y + h : 0);
			TryDrawMainMenuButton(Exit, Buttons.Count > 0 ? Buttons[^1].Position.Y + h * 2 : h);

			var hs = GetSettingsButtonHeight() * 1.2f;
			var startingY = -MainCamera.Texture.Size.Y * 0.38f;
			TryDrawSettingsButton(Graphics, startingY);
			TryDrawSettingsButton(Audio, startingY + hs);

			TryDrawAudioSlider(VolumeMusic, -hs * 0.75f);
			TryDrawAudioSlider(VolumeSound, hs * 0.75f);
			TryShowSlider(VolumeMusic, audioVisible);
			TryShowSlider(VolumeSound, audioVisible);
			if (VolumeSound != null && audioVisible)
			{
				volumeSound.Position = VolumeSound.CornerA;
				DrawText(new(FontPath) { Text = "Sound" }, volumeSound, new(1.1f, 0));
			}
			if (VolumeMusic != null && audioVisible)
			{
				volumeMusic.Position = VolumeMusic.CornerA;
				DrawText(new(FontPath) { Text = "Music" }, volumeMusic, new(1.1f, 0));
			}

			TryDrawGraphicSlider(Resolution, -hs * 2);
			TryShowSlider(Resolution, graphicsVisible);
			if (Resolution != null && graphicsVisible)
			{
				pixelSize.Position = Resolution.CornerA;
				resolution.Position = Resolution.CornerA.PointPercentTowardPoint(Resolution.CornerB, new(50));
				Resolution.RangeA = 0.1f;
				var v = (int)(Resolution.Value * 30) / 30f;
				var resW = VideoMode.DesktopMode.Width * v;
				var resH = VideoMode.DesktopMode.Height * v;
				DrawText(new(FontPath) { Text = "Resolution" }, pixelSize, new(1.1f, 0));
				DrawText(new(FontPath) { OutlineSize = 2, Text = $"{resW}x{resH}" }, resolution, new(0.5f, -0.2f));
			}
			if (VSync != null && graphicsVisible)
			{

			}
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

		public void HideEverythingInSettings()
		{
			audioVisible = false;
			graphicsVisible = false;

			OnHideEverythingInSettings();
		}
		public void HideEverytingInPanel()
		{
			HideEverythingInSettings();

			TryShowSettingsButton(Audio, false);
			TryShowSettingsButton(Graphics, false);

			OnHideEverythingInPanel();
		}

		protected virtual void OnHideEverythingInPanel() { }
		protected virtual void OnHideEverythingInSettings() { }

		private void OnAudioClick()
		{
			HideEverythingInSettings();
			audioVisible = true;
		}
		private void OnGraphicsClick()
		{
			HideEverythingInSettings();
			graphicsVisible = true;
		}
		private void OnSettingsClick()
		{
			PanelIsShown = true;

			TryShowSettingsButton(Audio);
			TryShowSettingsButton(Graphics);
		}
		private void OnExitClick() => Game.Stop();
		private static void TryShowSlider(Slider slider, bool show = true)
		{
			if (slider == null)
				return;

			slider.IsHidden = show == false;
			slider.IsDisabled = show == false;
		}
		private static void TryShowSettingsButton(Button button, bool show = true)
		{
			if (button == null)
				return;

			button.IsHidden = show == false;
			button.IsDisabled = show == false;
		}
		private static void TryDrawMainMenuButton(Button button, float y)
		{
			if (button == null)
				return;

			button.Size = new(MainCamera.Texture.Size.X * 0.2f, GetMainMenuButtonHeight());
			button.Position = new(MainCamera.Texture.Size.X * 0.3f, y);
			button.SetDefaultHitbox();
			button.Hitbox.TransformLocalLines(button);
			button.Draw();
		}
		private static void TryDrawSettingsButton(Button button, float y)
		{
			if (button == null)
				return;

			button.Size = new(MainCamera.Texture.Size.X * 0.11f, MainCamera.Texture.Size.Y * 0.06f);
			button.Position = new(MainCamera.Texture.Size.X * 0.033f, y);
			button.SetDefaultHitbox();
			button.Hitbox.TransformLocalLines(button);
			button.Draw();
		}
		private void TryDrawAudioSlider(Slider slider, float y)
		{
			if (slider == null)
				return;

			slider.LengthUnit = 0.1f;
			slider.Size = new(0, MainCamera.Texture.Size.Y * 0.06f);
			slider.LengthMax = MainCamera.Texture.Size.X * 0.25f;
			slider.Position = panel.Position + new Vector2(-MainCamera.Texture.Size.X * 0.02f, y);
			slider.Draw();
		}
		private void TryDrawGraphicSlider(Slider slider, float y)
		{
			if (slider == null)
				return;

			slider.LengthUnit = 0.1f;
			slider.Size = new(0, MainCamera.Texture.Size.Y * 0.06f);
			slider.LengthMax = MainCamera.Texture.Size.X * 0.2f;
			slider.Position = panel.Position + new Vector2(MainCamera.Texture.Size.X * 0.005f, y);
			slider.Draw();
		}

		private static float GetMainMenuButtonHeight() => MainCamera.Texture.Size.Y * 0.08f;
		private static float GetSettingsButtonHeight() => MainCamera.Texture.Size.Y * 0.06f;
	}
}
