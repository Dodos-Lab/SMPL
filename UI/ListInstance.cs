namespace SMPL.UI
{
	internal class ListInstance : ScrollBarInstance
	{
		public List<string> ButtonUIDs { get; } = new();

		public int VisibleButtonCountMax { get; set; } = 5;
		public int VisibleButtonCountCurrent => Math.Min(ButtonUIDs.Count, VisibleButtonCountMax);

		public float ButtonWidth { get; set; } = 400;
		public float ButtonHeight
		{
			get
			{
				var max = Math.Max(VisibleButtonCountMax, 1);
				var down = GetButtonDown();
				var sz = down == null ? 0 : down.LocalSize.X * 2;
				return (MaxLength + sz) / max - (ButtonSpacing - (ButtonSpacing / max));
			}
		}
		public float ButtonSpacing
		{
			get => spacing;
			set => spacing = value.Limit(0, MaxLength);
		}

		public bool IsHovered
		{
			get
			{
				if(IsHidden || IsDisabled || ButtonUIDs.Count == 0)
					return false;

				var up = GetButtonUp();
				var down = GetButtonDown();
				var tr = up.BoundingBox.Lines[0].A;
				var br = down.BoundingBox.Lines[1].A;

				var topLeft = tr.PointMoveAtAngle(180, (up.LocalSize.X + ButtonWidth) * Scale, false);
				var bottomLeft = br.PointMoveAtAngle(180, (down.LocalSize.X + ButtonWidth) * Scale, false);

				hitbox.Lines.Clear();
				hitbox.Lines.Add(new(topLeft, tr));
				hitbox.Lines.Add(new(tr, br));
				hitbox.Lines.Add(new(br, bottomLeft));
				hitbox.Lines.Add(new(bottomLeft, topLeft));

				return hitbox.ConvexContains(Scene.MouseCursorPosition);
			}
		}
		public int ScrollIndex => scrollIndex;

		#region Backend
		private float spacing = 5;
		private int scrollIndex;
		private readonly Hitbox hitbox = new();
		private string clickedUID;

		[JsonConstructor]
		internal ListInstance() => Init();
		internal ListInstance(string uid, string btnUpUID, string btnDownUID) : base(uid, btnUpUID, btnDownUID)
		{
			Init();
		}
		private void Init()
		{
			Value = 0;
		}
		internal void Update()
		{
			var left = Mouse.IsButtonPressed(Mouse.Button.Left);

			IsFocused = IsHovered;

			if(left.Once($"click-list-{GetHashCode()}"))
			{
				clickedUID = null;
				var first = ScrollIndex.Limit(0, ButtonUIDs.Count);
				var last = (ScrollIndex + VisibleButtonCountMax).Limit(0, ButtonUIDs.Count);
				for(int i = first; i < last; i++)
				{
					var btn = Get<ButtonInstance>(ButtonUIDs[i]);
					if(btn == null)
						continue;

					if(btn.BoundingBox.IsHovered)
						clickedUID = ButtonUIDs[i];
				}

				if(IsHovered == false)
					OnUnfocus();
			}

			var clicked = Get<ButtonInstance>(clickedUID);
			if(clicked == null)
				return;

			if((left == false).Once($"release-list-{GetHashCode()}") && clickedUID != null && clicked.BoundingBox.IsHovered)
			{
				clicked.Hover();
				Event.ListButtonClick(clickedUID);
			}
		}

		protected virtual void OnUnfocus() { }
		internal override void OnDraw(RenderTarget renderTarget)
		{
			OriginUnit = new(0, 0.5f);

			if(IsHidden)
				return;

			VisibleButtonCountMax = Math.Max(VisibleButtonCountMax, 1);

			Step = 1;
			RangeA = 0;
			RangeB = MathF.Max((ButtonUIDs.Count - VisibleButtonCountMax).Limit(0, ButtonUIDs.Count - 1), RangeA);

			scrollIndex = (int)Value;

			var nothingToScroll = ButtonUIDs.Count != 0 && RangeA == RangeB;
			var up = GetButtonUp();
			var down = GetButtonDown();
			var sz = up == null ? 0 : up.LocalSize.X;
			if(up != null)
			{
				up.IsHidden = nothingToScroll;
				up.IsDisabled = nothingToScroll;
			}
			if(down != null)
			{
				down.IsHidden = nothingToScroll;
				down.IsDisabled = nothingToScroll;
			}

			base.OnDraw(renderTarget);

			for(int i = scrollIndex; i < ButtonUIDs.Count; i++)
			{
				var btn = Get<ButtonInstance>(ButtonUIDs[i]);
				if(btn == null)
					continue;

				var isVisible = i >= scrollIndex && i < scrollIndex + VisibleButtonCountMax;
				btn.ParentUID = UID;
				if(isVisible == false)
				{
					btn.Position = new Vector2().NaN();
					continue;
				}

				var x = ButtonHeight * 0.5f - sz + ((ButtonHeight + ButtonSpacing) * (i - scrollIndex));
				var y = -LocalSize.Y * OriginUnit.Y + ButtonWidth * 0.5f + LocalSize.Y;

				btn.LocalSize = new(ButtonWidth, ButtonHeight);
				btn.Scale = Scale;
				btn.OriginUnit = new(0.5f);
				btn.LocalPosition = new(x, y);
			}

			Update();
		}
		#endregion
	}
}
