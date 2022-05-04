using SMPL.Tools;
using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;
using System.Collections;
using System;

namespace SMPL.UI
{
	/// <summary>
	/// Inherit chain: <see cref="List"/> : <see cref="ScrollBar"/> : <see cref="Slider"/> : <see cref="ProgressBar"/> : <see cref="Sprite"/> :
	/// <see cref="Visual"/> : <see cref="Object"/> <br></br><br></br>
	/// This class takes control of multiple equally sized <see cref="Button"/>s by adding, removing, positioning and scrolling.
	/// </summary>
	public class List : ScrollBar
	{
		private int scrollIndex;

		public List<Button> Buttons { get; } = new();
		public int VisibleButtonCount { get; set; } = 5;
		public bool IsHovered
		{
			get
			{
				if (IsHidden || IsDisabled)
					return false;

				var first = Buttons[scrollIndex.Limit(0, Buttons.Count - 1)];
				var last = Buttons[(scrollIndex + VisibleButtonCount - 1).Limit(0, Buttons.Count - 1)];
				var hitbox = new Hitbox(
					first.CornerA,
					first.CornerB,
					ScrollUp.CornerA,
					ScrollDown.CornerB,
					ScrollDown.CornerC,
					last.CornerC,
					last.CornerD,
					first.CornerA);

				return hitbox.ConvexContains(Scene.MouseCursorPosition);
			}
		}
		public float ButtonWidth { get; set; } = 400;
		public float ButtonHeight => (LengthMax + ScrollDown.Size.X * 2) / Math.Max(VisibleButtonCount, 1);
		public int ScrollIndex => scrollIndex;

		public List()
		{
			Value = 0;
		}

		public override void Draw()
		{
			OriginUnit = new(0, 0.5f);

			if (IsHidden)
				return;

			VisibleButtonCount = Math.Max(VisibleButtonCount, 1);

			ScrollValue = 1;
			RangeA = 0;
			RangeB = (Buttons.Count - VisibleButtonCount).Limit(0, Buttons.Count - 1);

			scrollIndex = (int)Value;

			base.Draw();

			for (int i = scrollIndex; i < Buttons.Count; i++)
			{
				var btn = Buttons[i];
				btn.Parent = this;
				if (i >= scrollIndex && i < scrollIndex + VisibleButtonCount)
				{
					var x = ButtonHeight * 0.5f - ScrollUp.Size.X + (ButtonHeight * (i - scrollIndex));
					var y = -Size.Y * OriginUnit.Y + ButtonWidth * 0.5f + Size.Y;

					btn.Size = new(ButtonWidth, ButtonHeight);
					btn.OriginUnit = new(0.5f);
					btn.LocalPosition = new(x, y);
					btn.SetDefaultHitbox();
					btn.Hitbox.TransformLocalLines(btn);
					btn.Draw();
				}
				else
					btn.Hitbox.Lines.Clear();
			}
		}
	}
}
