using Newtonsoft.Json;
using SFML.Window;
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
		public Tick ShowList { get; }
		[JsonIgnore]
		public Button Selection => Buttons.Count == 0 ? null : Buttons[selectionIndex];

		public ListDropdown(Tick showList = default)
		{
			showList ??= new();
			ShowList = showList;
			ShowList.Clicked += OnShow;
			ShowList.Parent = this;
			ShowList.OriginUnit = new(0, 1);
		}

		public override void Draw()
		{
			OriginUnit = new(0, 0.5f);

			if (Selection != null)
			{
				Selection.Parent = this;
				Selection.Size = new(ButtonWidth, ButtonHeight);
				Selection.LocalPosition = new(-ButtonHeight * 0.5f - ScrollUp.Size.X, ButtonWidth * 0.5f + ScrollUp.Size.Y * 0.5f);
				Selection.IsDisabled = isOpen == false;
				Selection.Draw();
			}

			ShowList.Position = ScrollUp.CornerD;
			ShowList.Size = new(ShowList.Size.X, Selection.Size.Y);
			ShowList.SetDefaultHitbox();
			ShowList.Hitbox.TransformLocalLines(ShowList);

			if (Selection != null)
			{
				var top = ShowList.Hitbox.Lines[0];
				var bot = ShowList.Hitbox.Lines[2];
				top.A = Selection.CornerA;
				bot.B = Selection.CornerD;
				ShowList.Hitbox.Lines[0] = top;
				ShowList.Hitbox.Lines[2] = bot;
				ShowList.Hitbox.Lines[3] = new(bot.B, top.A);
			}

			ShowList.Draw();

			base.Draw();

			IsHidden = isOpen == false;
			IsDisabled = isOpen == false;
		}

		protected virtual void OnSelect(Button button) => Selected?.Invoke(button);
		protected override void OnUnfocus()
		{
			if (ShowList.Hitbox.ConvexContains(Scene.MouseCursorPosition))
				return;
			isOpen = false;
		}
		protected override void OnButtonClick(Button button)
		{
			selectionIndex = Buttons.IndexOf(button);
			isOpen = false;
			OnSelect(button);
		}

		private void OnShow()
		{
			isOpen = !isOpen;
		}
	}
}
