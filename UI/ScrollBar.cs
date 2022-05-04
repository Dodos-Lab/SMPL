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
		public Button ScrollUp { get; }
		/// <summary>
		/// A button used to bring the scroll down.
		/// </summary>
		public Button ScrollDown { get; }

		public ScrollBar()
		{
			Angle = 90;
			Game.Window.MouseWheelScrolled += OnScroll;
			ScrollValueUnit = 0.1f;

			ScrollUp = new();
			ScrollDown = new();

			ScrollUp.Clicked += OnScrollUp;
			ScrollDown.Clicked += OnScrollDown;

			ScrollUp.Held += OnScrollUpHold;
			ScrollDown.Held += OnScrollDownHold;

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
		/// Draws the <see cref="ScrollBar"/> on the <see cref="Visual.DrawTarget"/> according to all the required
		/// <see cref="Object"/>, <see cref="Visual"/>, <see cref="Sprite"/>, <see cref="ProgressBar"/>, <see cref="Slider"/> and <see cref="ScrollBar"/> parameters.
		/// </summary>
		public override void Draw()
		{
			FillColor = Color.Transparent;

			base.Draw();

			Update();

			ScrollUp.Draw();
			ScrollDown.Draw();
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

			ScrollUp.SetDefaultHitbox();
			ScrollUp.Hitbox.TransformLocalLines(ScrollUp);

			ScrollDown.SetDefaultHitbox();
			ScrollDown.Hitbox.TransformLocalLines(ScrollDown);
		}
	}
}
