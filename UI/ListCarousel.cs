using SFML.Window;
using SMPL.Core;
using SMPL.Tools;

namespace SMPL.UI
{
	public class ListCarousel : List
	{
		private int selectionIndex;

		public Button Previous { get; }
		public Button Next { get; }

		public int SelectionIndex { get => selectionIndex; set => selectionIndex = value.Limit(0, Buttons.Count - 1); }
		public Button Selection { get => Buttons.Count == 0 ? null : Buttons[selectionIndex]; }

		public ListCarousel(Button previous = null, Button next = null)
		{
			Angle = 0;
			previous ??= new();
			next ??= new();

			Previous = previous;
			Next = next;

			Previous.Clicked += OnPrevious;
			Next.Clicked += OnNext;
		}

		private void OnPrevious() => SelectionIndex--;
		private void OnNext() => SelectionIndex++;

		public override void Draw()
		{
			Previous.SetDefaultHitbox();
			Previous.Hitbox.TransformLocalLines(Previous);

			Next.SetDefaultHitbox();
			Next.Hitbox.TransformLocalLines(Next);

			Previous.Size = new(ButtonHeight, ButtonHeight);
			Next.Size = new(ButtonHeight, ButtonHeight);

			Previous.Parent = this;
			Next.Parent = this;

			Previous.LocalPosition = new(-ButtonWidth * 0.5f - Previous.Size.X * 0.5f, 0);
			Next.LocalPosition = new(ButtonWidth * 0.5f + Next.Size.X * 0.5f, 0);

			Previous.Draw();
			Next.Draw();

			if (Selection == null)
				return;

			Selection.Size = new(ButtonWidth, ButtonHeight);
			Selection.Parent = this;
			Selection.LocalPosition = new();
			Selection.Draw();
		}
	}
}
