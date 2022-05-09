using SFML.Window;
using SMPL.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SMPL.UI
{
	public class ListMultiselect : List
	{
		public List<Tick> Ticks { get; } = new();
		public List<int> SelectionIndexes { get; } = new();

		public override void Draw()
		{
			base.Draw();
			for (int i = ScrollIndex; i < ScrollIndex + VisibleButtonCount; i++)
			{
				if (Ticks.Count <= i)
					continue;

				var tick = Ticks[i];

				var btn = Buttons[i];
				tick.OriginUnit = new(1, 0);
				tick.Parent = this;
				tick.Position = btn.CornerA;
				tick.Size = new(ButtonHeight);
				tick.SetDefaultHitbox();
				tick.Hitbox.TransformLocalLines(tick);
				tick.Draw();
			}

			SelectionIndexes.Clear();
			for (int i = 0; i < Ticks.Count; i++)
				if (Ticks[i] != null && Ticks[i].IsActive)
					SelectionIndexes.Add(i);
		}

		protected override void OnButtonClick(Button button)
		{
			var index = Buttons.IndexOf(button);
			if (Ticks.Count <= index)
				return;

			Ticks[index].Trigger();
		}
	}
}
