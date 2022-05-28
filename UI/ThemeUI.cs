using Newtonsoft.Json;
using SFML.Graphics;
using SMPL.Graphics;
using System.Numerics;

namespace SMPL.UI
{
	public class ThemeUI
	{
		public string InputboxFontPath { get; set; }
		public string TextButtonFontPath { get; set; }
		public string TextBoxFontPath { get; set; }
		public string QuickTextFontPath { get; set; }

		public string ListNextTexturePath { get; set; }
		public string ListPreviousTexturePath { get; set; }
		public string ScrollDownTexturePath { get; set; }
		public string ScrollUpTexturePath { get; set; }
		public string ScrollBarTexturePath { get; set; }
		public string SliderTexturePath { get; set; }
		public string TickOffTexturePath { get; set; }
		public string TickOnTexturePath { get; set; }
		public string ButtonTexturePath { get; set; }
		public string ProgressBarTexturePath { get; set; }

		public void IncludeInAssetQueue(ref Scene.AssetQueue assetQueue)
		{
			if (assetQueue.Textures == null)
				assetQueue.Textures = new();
			if (assetQueue.Fonts == null)
				assetQueue.Fonts = new();

			assetQueue.Textures.Add(ListNextTexturePath);
			assetQueue.Textures.Add(ListPreviousTexturePath);
			assetQueue.Textures.Add(ScrollDownTexturePath);
			assetQueue.Textures.Add(ScrollUpTexturePath);
			assetQueue.Textures.Add(ScrollBarTexturePath);
			assetQueue.Textures.Add(SliderTexturePath);
			assetQueue.Textures.Add(TickOffTexturePath);
			assetQueue.Textures.Add(TickOnTexturePath);
			assetQueue.Textures.Add(ButtonTexturePath);
			assetQueue.Textures.Add(ProgressBarTexturePath);

			assetQueue.Fonts.Add(InputboxFontPath);
			assetQueue.Fonts.Add(TextButtonFontPath);
			assetQueue.Fonts.Add(TextBoxFontPath);
			assetQueue.Fonts.Add(QuickTextFontPath);
		}

		public QuickText CreateQuickText(string text = "Hello, World!", Vector2 position = default) => new(QuickTextFontPath, text, position);
		public Button CreateButton() => new() { Theme = this, TexturePath = ButtonTexturePath };
		public TextButton CreateTextButton(string text = "Click me!") => new(new(TextButtonFontPath, text)) { Theme = this, TexturePath = ButtonTexturePath };
		public Tick CreateTick() => new() { Theme = this, TexturePath = TickOffTexturePath };
		public Slider CreateSlider() => new() { TexturePath = SliderTexturePath };
		public ProgressBar CreateProgressBar() => new() { TexturePath = ProgressBarTexturePath };
		public ScrollBar CreateScrollBar()
		{
			var up = ScrollUpTexturePath == null ? CreateButton() : CreateTextButton();
			var down = ScrollDownTexturePath == null ? CreateButton() : CreateTextButton();

			if (up is TextButton u)
				u.QuickText.Text = "^";

			if (down is TextButton d)
				d.QuickText.Text = "V";

			return new(up, down) { TexturePath = ScrollBarTexturePath };
		}
		public Inputbox CreateInputbox() => new(InputboxFontPath);
		public Textbox CreateTextbox() => new(TextBoxFontPath);
		public ListCarousel CreateListCarousel()
		{
			var prev = ListPreviousTexturePath != null ? CreateButton() : CreateTextButton();
			var next = ListPreviousTexturePath != null ? CreateButton() : CreateTextButton();

			if (prev is TextButton p)
				p.QuickText.Text = "<";

			if (next is TextButton n)
				n.QuickText.Text = ">";

			return new ListCarousel(prev, next) { TexturePath = ScrollBarTexturePath };
		}
		public List CreateList() => new() { TexturePath = ScrollBarTexturePath };
		public ListDropdown CreateListDropdown() => new() { TexturePath = ScrollBarTexturePath };
		public ListMultiselect CreateListMultiselect() => new() { TexturePath = ScrollBarTexturePath };

		protected virtual void OnButtonClick(Button button) { }
		protected virtual void OnButtonHold(Button button) { }
		protected virtual void OnButtonRelease(Button button) { }
		protected virtual void OnButtonHover(Button button) => button.Tint = new(255, 255, 255);
		protected virtual void OnButtonUnhover(Button button) => button.Tint = new(220, 220, 220);
		protected virtual void OnButtonPress(Button button) => button.Tint = new(200, 200, 200);
		protected virtual void OnTickClick(Tick tick) => tick.TexturePath = tick.IsActive ? TickOnTexturePath : TickOffTexturePath;

		internal void ButtonClick(Button button) => OnButtonClick(button);
		internal void ButtonHold(Button button) => OnButtonHold(button);
		internal void ButtonHover(Button button) => OnButtonHover(button);
		internal void ButtonUnhover(Button button) => OnButtonUnhover(button);
		internal void ButtonPress(Button button) => OnButtonPress(button);
		internal void ButtonRelease(Button button) => OnButtonRelease(button);
		internal void TickClick(Tick tick) => OnTickClick(tick);
	}
}
