using SFML.Window;
using SMPL.Core;
using SMPL.Tools;

namespace SMPL.UI
{
	public class ListCarousel : List
	{
		public Button Previous { get; }
		public Button Next { get; }

		public new bool IsHovered
		{
			get
			{
				if (IsHidden || IsDisabled)
					return false;

				var hitbox = new Hitbox(
					Previous.CornerA,
					Next.CornerB,
					Next.CornerC,
					Previous.CornerD,
					Previous.CornerA);

				return hitbox.ConvexContains(Scene.MouseCursorPosition);
			}
		}
		public bool SelectionIsRepeating { get; set; }
		public int SelectionIndex
		{
			get => (int)Value;
			set => Value = value.Limit(0, Buttons.Count, SelectionIsRepeating ? Extensions.Limitation.Overflow : Extensions.Limitation.ClosestBound);
		}
		public Button Selection => Buttons.Count == 0 ? null : Buttons[SelectionIndex];

		public ListCarousel(Button previous = null, Button next = null)
		{
			SelectionIsRepeating = true;

			Angle = 0;
			previous ??= new();
			next ??= new();

			Previous = previous;
			Next = next;

			Previous.Clicked += OnPrevious;
			Next.Clicked += OnNext;

			Previous.Held += OnPreviousHold;
			Next.Held += OnNextHold;
		}

		private void OnPreviousHold() => OnScrollDown();
		private void OnNextHold() => OnScrollUp();
		private void OnPrevious() => OnScrollDown();
		private void OnNext() => OnScrollUp();

		protected override void OnScrollUp()
		{
			if (IsDisabled)
				return;

			SelectionIndex++;
		}
		protected override void OnScrollDown()
		{
			if (IsDisabled)
				return;

			SelectionIndex--;
		}

		public override void Draw()
		{
			ScrollValue = 1;
			RangeA = 0;
			RangeB = Buttons.Count - 1;

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
			Selection.SetDefaultHitbox();
			Selection.Hitbox.TransformLocalLines(Selection);

			Selection.Draw();
		}
	}
}
