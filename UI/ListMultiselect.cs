using SFML.Window;
using SMPL.Core;
using SMPL.Tools;
using System.Collections.Generic;

namespace SMPL.UI
{
	public class ListMultiselect : List
	{
		private List<Button> selected = new();
		private Button clicked;

		public delegate void MultiselectEventHandler(Button button);
		public event MultiselectEventHandler Selected;
		public event MultiselectEventHandler Unselected;

		public ListMultiselect()
		{

		}

		public override void Draw()
		{
			OriginUnit = new(0, 0.5f);

			base.Draw();

			var left = Mouse.IsButtonPressed(Mouse.Button.Left);

			if (left.Once($"click-dropdown-{GetHashCode()}"))
			{
				var first = ScrollIndex.Limit(0, Buttons.Count);
				var last = (ScrollIndex + VisibleButtonCount).Limit(0, Buttons.Count);
				for (int i = first; i < last; i++)
					if (Buttons[i].Hitbox.ConvexContains(Scene.MouseCursorPosition))
						clicked = Buttons[i];

				if (clicked == null && IsHovered == false)
					clicked = null;
			}
			if ((left == false).Once($"release-dropdown-{GetHashCode()}") && clicked != null)
			{
				if (clicked.Hitbox.ConvexContains(Scene.MouseCursorPosition))
				{
					if (selected.Contains(clicked))
					{
						selected.Remove(clicked);
						OnUnselect(clicked);
					}
					else
					{
						selected.Add(clicked);
						OnSelect(clicked);
					}
				}
				else
					clicked = null;
			}
		}

		protected virtual void OnSelect(Button button) => Selected?.Invoke(button);
		protected virtual void OnUnselect(Button button) => Unselected?.Invoke(button);
	}
}
