using SFML.Window;
using SMPL.Graphics;
using SMPL.Tools;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SMPL.UI
{
	public class ListMultiselect : List
	{
		public List<Tick> Ticks { get; private set; } = new();
		public List<int> SelectionIndexes { get; private set; } = new();

		public override void Draw(Camera camera = null)
		{
			base.Draw(camera);
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
				tick.Draw(camera);
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
		protected override void OnDestroy()
		{
			base.OnDestroy();

			for (int i = 0; i < Ticks.Count; i++)
				Ticks[i].Destroy();
			Ticks = null;
			SelectionIndexes = null;
		}
	}
}
