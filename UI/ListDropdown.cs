using Newtonsoft.Json;
using SFML.Window;
using SMPL.Graphics;
using SMPL.Tools;

namespace SMPL.UI
{
	public class ListDropdown : List
	{
		private bool isOpen;
		private int selectionIndex;

		public delegate void SelectedEventHandler(Button button);
		public event SelectedEventHandler Selected;

		[JsonIgnore]
		public Button ShowList { get; private set; }
		[JsonIgnore]
		public Button Selection => Buttons.Count == 0 ? null : Buttons[SelectionIndex];
		public int SelectionIndex
		{
			get => selectionIndex;
			set
			{
				UpdateDefaultValues();
				selectionIndex = value.Limit(0, Buttons.Count, Extensions.Limitation.ClosestBound);
				UpdateSelection();
			}
		}

		public ListDropdown(Button showList = default)
		{
			showList ??= new();
			ShowList = showList;
			ShowList.Clicked += OnShow;
			ShowList.Parent = this;
			ShowList.OriginUnit = new(0, 1);
		}

		public override void Draw(Camera camera = null)
		{
			UpdateDefaultValues();

			if (Selection != null)
			{
				UpdateSelection();
				Selection.IsDisabled = isOpen == false;
				Selection.Draw(camera);
			}

			ShowList.Position = ScrollUp.CornerD;
			if (Selection != null)
				ShowList.LocalSize = new(Selection.LocalSize.Y);

			ShowList.Draw(camera);

			base.Draw(camera);

			IsHidden = isOpen == false;
			IsDisabled = isOpen == false;
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();

			ShowList.Clicked -= OnShow;
			ShowList.Destroy();
			ShowList = null;
			Selected = null;
		}

		protected virtual void OnSelect(Button button) => Selected?.Invoke(button);
		protected override void OnUnfocus()
		{
			if (ShowList == null || ShowList.Hitbox.IsHovered)
				return;
			isOpen = false;
		}
		protected override void OnButtonClick(Button button)
		{
			SelectionIndex = Buttons.IndexOf(button);
			isOpen = false;
			Selection.Unhover();
			OnSelect(button);
		}

		private void OnShow()
		{
			isOpen = !isOpen;
		}
		private void UpdateDefaultValues()
		{
			RangeA = 0;
			RangeB = Buttons.Count - 1;
			OriginUnit = new(0, 0.5f);
		}
		private void UpdateSelection()
		{
			Selection.Parent = this;
			Selection.LocalSize = new(ButtonWidth, ButtonHeight);
			Selection.LocalPosition = new(-ButtonHeight * 0.5f - ScrollUp.LocalSize.X, ButtonWidth * 0.5f + ScrollUp.LocalSize.Y * 0.5f);
		}
	}
}
