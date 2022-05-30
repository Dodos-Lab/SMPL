using Newtonsoft.Json;
using SFML.Graphics;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;

namespace SMPL.UI
{
	/// <summary>
	/// Inherit chain: <see cref="ScrollBar"/> : <see cref="Slider"/> : <see cref="ProgressBar"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/>
	/// <br></br><br></br>
	/// This class simply extendeds <see cref="Slider"/> with handled mouse scrolling and buttons.
	/// </summary>
	public class ScrollBar : Slider
	{
		/// <summary>
		/// Whether the <see cref="ScrollBar"/> is accepting input from the mouse scroll.
		/// </summary>
		public bool IsReceivingInput { get; set; }

		/// <summary>
		/// The amount of change on <see cref="ProgressBar.Value"/> upon scrolling.
		/// </summary>
		public float ScrollValue { get; set; }
		/// <summary>
		/// The amount of change on <see cref="ProgressBar.Value"/> upon scrolling in the range [0 - 1] according to
		/// [<see cref="ProgressBar.RangeA"/> - <see cref="ProgressBar.RangeB"/>].
		/// </summary>
		public float ScrollValueUnit
		{
			get => ScrollValue.Map(RangeA, RangeB, 0, 1);
			set => ScrollValue = value.Map(0, 1, RangeA, RangeB);
		}

		/// <summary>
		/// A button used to bring the scroll up.
		/// </summary>
		[JsonIgnore]
		public Button ScrollUp { get; private set; }
		/// <summary>
		/// A button used to bring the scroll down.
		/// </summary>
		[JsonIgnore]
		public Button ScrollDown { get; private set; }

		public ScrollBar(Button scrollUp = null, Button scrollDown = null)
		{
			var theme = Scene.CurrentScene.ThemeUI;
			var noUp = scrollUp == null;
			var noDown = scrollDown == null;

			scrollUp ??= theme == null ? new Button() : (theme.ScrollUpTexturePath == null ? theme.CreateTextButton("^") : theme.CreateButton());
			scrollDown ??= theme == null ? new Button() : (theme.ScrollDownTexturePath == null ? theme.CreateTextButton("V") : theme.CreateButton());

			if (noUp && theme != null)
				scrollUp.TexturePath = theme.ScrollUpTexturePath ?? theme.ButtonTexturePath;
			if (noDown && theme != null)
				scrollDown.TexturePath = theme.ScrollDownTexturePath ?? theme.ButtonTexturePath;

			if (scrollUp is TextButton u)
				u.QuickText.OriginUnit = new(1.8f, 0);
			if (scrollDown is TextButton d)
				d.QuickText.OriginUnit = new(-0.5f, 0);

			ScrollUp = scrollUp;
			ScrollDown = scrollDown;

			Angle = 90;
			
			ScrollValueUnit = 0.1f;

			EmptyColor = new(0, 0, 0, 100);
			ProgressColor = new(0, 0, 0, 100);

			ScrollUp.Clicked += OnScrollUp;
			ScrollDown.Clicked += OnScrollDown;
			ScrollUp.Held += OnScrollUpHold;
			ScrollDown.Held += OnScrollDownHold;

			if (Game.Window == null)
				Console.LogError(1, $"The {nameof(Game.Window)} is not yet created but this {nameof(ScrollBar)} is trying to subscribe to its events.\n" +
					$"Consider creating it after the {nameof(Scene)} starts.");
			else
				Game.Window.MouseWheelScrolled += OnScroll;

			Update();
		}

		protected virtual void OnScrollUp()
		{
			if (IsDisabled)
				return;

			Value -= ScrollValue;
		}
		protected virtual void OnScrollDown()
		{
			if (IsDisabled)
				return;

			Value += ScrollValue;
		}
		protected virtual void OnScrollUpHold() => OnScrollUp();
		protected virtual void OnScrollDownHold() => OnScrollDown();

		private void OnScroll(object sender, SFML.Window.MouseWheelScrollEventArgs e)
		{
			if (IsReceivingInput == false || IsDisabled)
				return;

			if (e.Delta < 0)
				OnScrollDown();
			else
				OnScrollUp();
		}

		/// <summary>
		/// Draws the <see cref="ScrollBar"/> on the <paramref name="camera"/> according to all the required
		/// <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/>, <see cref="ProgressBar"/>, <see cref="Slider"/> and <see cref="ScrollBar"/> parameters.
		/// The <paramref name="camera"/> is assumed to be the <see cref="Scene.MainCamera"/> if <see langword="null"/>.
		/// </summary>
		public override void Draw(Camera camera = null)
		{
			base.Draw(camera);

			Update();

			ScrollUp.Draw(camera);
			ScrollDown.Draw(camera);
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();

			Game.Window.MouseWheelScrolled -= OnScroll;

			ScrollUp.Clicked -= OnScrollUp;
			ScrollDown.Clicked -= OnScrollDown;
			ScrollUp.Held -= OnScrollUpHold;
			ScrollDown.Held -= OnScrollDownHold;

			ScrollUp.Destroy();
			ScrollDown.Destroy();

			ScrollUp = null;
			ScrollDown = null;
		}

		private void Update()
		{
			ScrollUp.Angle = Angle;
			ScrollDown.Angle = Angle;

			ScrollUp.OriginUnit = new(1, 0);
			ScrollDown.OriginUnit = new(0);

			ScrollUp.Parent = this;
			ScrollDown.Parent = this;

			ScrollUp.Size = new(Size.Y);
			ScrollDown.Size = new(Size.Y);

			ScrollUp.Position = CornerA;
			ScrollDown.Position = CornerB;
		}
	}
}
