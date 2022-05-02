using SMPL.Core;
using SMPL.Tools;
using SMPL.Graphics;
using SMPL.UI;
using System.Collections.Generic;
using System.Collections;

namespace SMPL.UI
{
	/// <summary>
	/// Inherit chain: <see cref="List"/> : <see cref="ScrollBar"/> : <see cref="Slider"/> : <see cref="ProgressBar"/> : <see cref="Sprite"/> :
	/// <see cref="Visual"/> : <see cref="Object"/> <br></br><br></br>
	/// This class takes control of multiple equally sized <see cref="Button"/>s by adding, removing, positioning and scrolling.
	/// </summary>
	public class List : ScrollBar
	{
		private int scrollIndex, visibleBtnCount = 6;

		public List<Button> Buttons { get; } = new();
		public int VisibleButtonCount
		{
			get => visibleBtnCount;
			set => visibleBtnCount = value.Limit(1, Buttons.Count);
		}
		public bool IsHovered
		{
			get
			{
				var first = Buttons[scrollIndex.Limit(0, Buttons.Count - 1)];
				var last = Buttons[(scrollIndex + VisibleButtonCount - 1).Limit(0, Buttons.Count - 1)];
				var hitbox = new Hitbox(
					first.CornerA,
					ScrollUp.CornerA,
					ScrollDown.CornerB,
					ScrollDown.CornerC,
					first.CornerB,
					last.CornerC,
					last.CornerD,
					first.CornerA);

				return hitbox.ConvexContains(Scene.MouseCursorPosition);
			}
		}
		public float ButtonWidth { get; set; } = 400;
		public int ScrollIndex => scrollIndex;

		public List()
		{
			Value = 0;
		}

		public override void Draw()
		{
			if (IsHidden)
				return;

			ScrollValue = 1;
			RangeA = 0;
			RangeB = (Buttons.Count - VisibleButtonCount).Limit(0, Buttons.Count - 1);

			scrollIndex = (int)Value;

			base.Draw();

			var h = LengthMax / (VisibleButtonCount - 1);
			for (int i = scrollIndex; i < Buttons.Count; i++)
			{
				var btn = Buttons[i];
				btn.Parent = this;
				if (i >= scrollIndex && i < scrollIndex + VisibleButtonCount)
				{
					btn.Size = new(ButtonWidth, h);
					btn.OriginUnit = new(0.5f);
					btn.LocalPosition = new(-LengthMax * OriginUnit.X + h * (i - scrollIndex), -Size.Y * OriginUnit.Y + ButtonWidth * 0.5f + Size.Y);
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
