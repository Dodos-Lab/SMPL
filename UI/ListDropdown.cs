using SFML.Window;
using SMPL.Core;
using SMPL.Tools;

namespace SMPL.UI
{
	public class ListDropdown : List
	{
		private bool isOpen;
		private int selectionIndex;
		private Button clicked;

		public Tick ShowList { get; } = new();
		public Button Selection { get => Buttons.Count == 0 ? null : Buttons[selectionIndex]; }

		public ListDropdown()
		{
			OriginUnit = new(0, 0.5f);

			ShowList.Clicked += OnShow;
			ShowList.Parent = this;
			ShowList.OriginUnit = new(0.5f, 1);
			ShowList.Size = new(80, 80);
		}

		private void OnShow()
		{
			isOpen = !isOpen;
		}

		public override void Draw()
		{
			ShowList.Position = ScrollUp.CornerA;
			ShowList.SetDefaultHitbox();
			ShowList.Hitbox.TransformLocalLines(ShowList);
			ShowList.Draw();

			if (Selection != null)
			{
				Selection.Parent = this;
				Selection.LocalPosition = new(-Selection.Size.Y, Selection.Size.X * 0.5f + Size.Y * 0.5f);
				Selection.IsDisabled = isOpen == false;
				Selection.Draw();
			}

			base.Draw();

			var left = Mouse.IsButtonPressed(Mouse.Button.Left);
			if (left.Once($"click-dropdown-{GetHashCode()}") && isOpen)
			{
				var first = ScrollIndex.Limit(0, Buttons.Count);
				var last = (ScrollIndex + VisibleButtonCount).Limit(0, Buttons.Count);
				for (int i = first; i < last; i++)
					if (Buttons[i].Hitbox.ConvexContains(Scene.MouseCursorPosition))
						clicked = Buttons[i];
			}
			if ((left == false).Once($"release-dropdown-{GetHashCode()}") && clicked != null)
			{
				if (clicked.Hitbox.ConvexContains(Scene.MouseCursorPosition))
				{
					selectionIndex = Buttons.IndexOf(clicked);
					isOpen = false;
				}
				clicked = null;
			}

			IsHidden = isOpen == false;
			IsDisabled = isOpen == false;
		}
	}
}
