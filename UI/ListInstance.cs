namespace SMPL.UI
{
	internal class ListInstance : ScrollBarInstance
	{
		public new Hitbox BoundingBox
		{
			get
			{
				var u = GetButtonUp();
				var d = GetButtonDown();
				var baseBB = base.GetBoundingBox();
				baseBB.TransformLocalLines(UID);
				if(baseBB.Lines.Count == 0)
					return bb;
				var up = u == null ? baseBB.Lines : u.BoundingBox.Lines;
				var down = d == null ? baseBB.Lines : d.BoundingBox.Lines;

				var tl = up[0].A;
				var tr = down[1].A;
				var br = down[2].A.PointMoveAtAngle(Angle + 90, ButtonWidth * Scale, false);
				var bl = up[3].A.PointMoveAtAngle(Angle + 90, ButtonWidth * Scale, false);

				bb.Lines.Clear();
				bb.LocalLines.Clear();
				bb.Lines.Add(new(tl, tr));
				bb.Lines.Add(new(tr, br));
				bb.Lines.Add(new(br, bl));
				bb.Lines.Add(new(bl, tl));

				return bb;
			}
		}
		public string ButtonsTag { get; set; }

		public int VisibleButtonCountMax { get; set; } = 5;
		public int VisibleButtonCountCurrent => Math.Min(GetButtonUIDs().Count, VisibleButtonCountMax);

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

		public int ScrollIndex => scrollIndex;

		#region Backend
		private float spacing = 5;
		private int scrollIndex;
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

			IsFocused = BoundingBox.IsHovered;

			if(left.Once($"click-list-{GetHashCode()}"))
			{
				clickedUID = null;
				var btnUIDs = GetButtonUIDs();
				var first = ScrollIndex.Limit(0, btnUIDs.Count);
				var last = (ScrollIndex + VisibleButtonCountMax).Limit(0, btnUIDs.Count);
				for(int i = first; i < last; i++)
				{
					var btn = Get<ButtonInstance>(btnUIDs[i]);
					if(btn == null)
						continue;

					if(btn.BoundingBox.IsHovered)
						clickedUID = btnUIDs[i];
				}

				if(BoundingBox.IsHovered == false)
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

			var btnUIDs = GetButtonUIDs();
			VisibleButtonCountMax = Math.Max(VisibleButtonCountMax, 1);

			Step = 1;
			RangeA = 0;
			RangeB = MathF.Max((btnUIDs.Count - VisibleButtonCountMax).Limit(0, btnUIDs.Count - 1), RangeA);

			scrollIndex = (int)Value;

			var nothingToScroll = btnUIDs.Count != 0 && RangeA == RangeB;
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

			for(int i = 0; i < btnUIDs.Count; i++)
			{
				var btn = Get<ButtonInstance>(btnUIDs[i]);
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
				btn.LocalAngle = 270;
				btn.Scale = Scale;
				btn.OriginUnit = new(0.5f);
				btn.LocalPosition = new(x, y);
			}

			Update();
		}
		internal List<string> GetButtonUIDs()
		{
			return SMPL.Thing.GetUIDsByTag(ButtonsTag);
		}
		#endregion
	}
}
