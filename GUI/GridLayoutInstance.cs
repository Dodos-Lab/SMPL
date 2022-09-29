namespace SMPL.GUI
{
	internal class GridLayoutInstance : SpriteInstance
	{
		public override float LocalAngle
		{
			get => base.LocalAngle;
			set
			{
				if(base.LocalAngle == value)
					return;

				base.LocalAngle = value;
				RecalculateItemBoundingBoxes();
			}
		}
		public override Vector2 LocalPosition
		{
			get => base.LocalPosition;
			set
			{
				if(base.LocalPosition == value)
					return;

				base.LocalPosition = value;
				RecalculateItemBoundingBoxes();
			}
		}
		public override Vector2 LocalSize
		{
			get => base.LocalSize;
			set
			{
				if(base.LocalSize == value)
					return;

				base.LocalSize = value;
				RecalculateItemBoundingBoxes();
			}
		}
		public override float LocalScale
		{
			get => base.LocalScale;
			set
			{
				if(base.LocalScale == value)
					return;

				base.LocalScale = value;
				RecalculateItemBoundingBoxes();
			}
		}

		public ReadOnlyCollection<string> ItemUIDs
		{
			get
			{
				var list = items.Keys.ToList();
				list.Add(null);
				return list.AsReadOnly();
			}
		}
		public ReadOnlyCollection<Thing.GUI.ListItem> Items
		{
			get
			{
				var list = items.Values.ToList();
				list.Add(null);
				return list.AsReadOnly();
			}
		}
		public void SetItemRight(string uid, string targetItemUID, float localSize, float localOffset)
		{
			if(uid == null)
				return;

			cuts.Add(new()
			{
				uid = uid,
				targetUID = targetItemUID,
				localSize = localSize,
				localOffset = localOffset,
				method = CutRight
			});
			items[uid] = new Thing.GUI.ListItem();
			CutRight(uid, targetItemUID, localSize, localOffset);
			RecalculateItemBoundingBoxes();
		}
		public void SetItemLeft(string uid, string targetItemUID, float localSize, float localOffset)
		{
			if(uid == null)
				return;

			cuts.Add(new()
			{
				uid = uid,
				targetUID = targetItemUID,
				localSize = localSize,
				localOffset = localOffset,
				method = CutLeft
			});
			items.Add(uid, new Thing.GUI.ListItem());
			CutLeft(uid, targetItemUID, localSize, localOffset);
			RecalculateItemBoundingBoxes();
		}
		public void SetItemUp(string uid, string targetItemUID, float localSize, float localOffset)
		{
			if(uid == null)
				return;

			cuts.Add(new()
			{
				uid = uid,
				targetUID = targetItemUID,
				localSize = localSize,
				localOffset = localOffset,
				method = CutUp
			});
			items.Add(uid, new Thing.GUI.ListItem());
			CutUp(uid, targetItemUID, localSize, localOffset);
			RecalculateItemBoundingBoxes();
		}
		public void SetItemDown(string uid, string targetItemUID, float localSize, float localOffset)
		{
			if(uid == null)
				return;

			cuts.Add(new()
			{
				uid = uid,
				targetUID = targetItemUID,
				localSize = localSize,
				localOffset = localOffset,
				method = CutDown
			});
			items.Add(uid, new Thing.GUI.ListItem());
			CutDown(uid, targetItemUID, localSize, localOffset);
			RecalculateItemBoundingBoxes();
		}

		public Thing.GUI.ListItem Item(string uid)
		{
			return uid != null && items.ContainsKey(uid) ? items[uid] : rootItem;
		}
		public void Trigger(string itemUID) => Event.ButtonClick(UID, itemUID != null && items.ContainsKey(itemUID) ? items[itemUID].ButtonDetails : null);

		#region Backend
		internal struct Cut
		{
			public string uid;
			public string targetUID;
			public float localSize;
			public float localOffset;
			public Action<string, string, float, float> method;
		}
		private readonly List<Cut> cuts = new();
		private readonly Thing.GUI.ListItem rootItem = new();

		[JsonProperty]
		private Dictionary<string, Thing.GUI.ListItem> items = new();

		[JsonConstructor]
		internal GridLayoutInstance() { }
		internal GridLayoutInstance(string uid) : base(uid) { }

		private void RecalculateItemBoundingBoxes()
		{
			// this is called upon a new cut, a new local size/scale/angle/pos

			var rootBB = rootItem.ButtonDetails.boundingBox;
			rootBB.LocalLines.Clear();
			rootBB.Lines.Clear();

			var boundB = BoundingBox;
			for(int i = 0; i < boundB.Lines.Count; i++)
				rootItem.ButtonDetails.boundingBox.LocalLines.Add(new(LocalPositionFromSelf(boundB.Lines[i].A), LocalPositionFromSelf(boundB.Lines[i].B)));

			foreach(var kvp in items)
			{
				var itemBB = kvp.Value.ButtonDetails.boundingBox;
				itemBB.Lines.Clear();
				itemBB.LocalLines.Clear();
			}
			for(int i = 0; i < cuts.Count; i++)
			{
				var c = cuts[i];
				c.method?.Invoke(c.uid, c.targetUID, c.localSize, c.localOffset);
			}
			rootBB.TransformLocalLines(UID);
			foreach(var kvp in items)
			{
				var itemBB = kvp.Value.ButtonDetails.boundingBox;
				itemBB.TransformLocalLines(UID);
			}
		}
		private void CutDown(string uid, string targetItemUID, float localSize, float localOffset)
		{
			var bb = (targetItemUID != null && items.ContainsKey(targetItemUID) ?
				items[targetItemUID].ButtonDetails.boundingBox : rootItem.ButtonDetails.boundingBox).LocalLines;
			var item = items[uid];
			var itemBB = item.ButtonDetails.boundingBox;
			var local = itemBB.LocalLines;

			localSize = MathF.Max(1, localSize);
			localOffset = MathF.Max(0, localOffset);

			if(bb.Count == 0)
				bb = rootItem.ButtonDetails.boundingBox.LocalLines;

			local.Add(new(MovePoint(bb[3].A, 270, localSize), MovePoint(bb[2].A, 270, localSize)));
			local.Add(new(local[^1].B, bb[2].A));
			local.Add(new(local[^1].B, bb[3].A));
			local.Add(new(local[^1].B, local[0].A));

			bb[1] = new(bb[1].A, MovePoint(local[1].A, 270, localOffset));
			bb[2] = new(bb[1].B, MovePoint(local[0].A, 270, localOffset));
			bb[3] = new(bb[2].B, bb[0].A);

			itemBB.TransformLocalLines(UID);
		}
		private void CutUp(string uid, string targetItemUID, float localSize, float localOffset)
		{
			var bb = (targetItemUID != null && items.ContainsKey(targetItemUID) ?
				items[targetItemUID].ButtonDetails.boundingBox : rootItem.ButtonDetails.boundingBox).LocalLines;
			var item = items[uid];
			var itemBB = item.ButtonDetails.boundingBox;
			var local = itemBB.LocalLines;

			localSize = MathF.Max(1, localSize);
			localOffset = MathF.Max(0, localOffset);

			if(bb.Count == 0)
				bb = rootItem.ButtonDetails.boundingBox.LocalLines;

			local.Add(new(bb[0].A, bb[1].A));
			local.Add(new(local[^1].B, MovePoint(local[^1].B, 90, localSize)));
			local.Add(new(local[^1].B, MovePoint(local[0].A, 90, localSize)));
			local.Add(new(local[^1].B, local[0].A));

			bb[0] = new(MovePoint(local[3].A, 90, localOffset), MovePoint(local[2].A, 90, localOffset));
			bb[1] = new(bb[0].B, bb[2].A);
			bb[3] = new(bb[3].A, bb[0].A);

			itemBB.TransformLocalLines(UID);
		}
		private void CutLeft(string uid, string targetItemUID, float localSize, float localOffset)
		{
			var bb = (targetItemUID != null && items.ContainsKey(targetItemUID) ?
				items[targetItemUID].ButtonDetails.boundingBox : rootItem.ButtonDetails.boundingBox).LocalLines;
			var item = items[uid];
			var itemBB = item.ButtonDetails.boundingBox;
			var local = itemBB.LocalLines;

			localSize = MathF.Max(1, localSize);
			localOffset = MathF.Max(0, localOffset);

			if(bb.Count == 0)
				bb = rootItem.ButtonDetails.boundingBox.LocalLines;

			local.Add(new(bb[0].A, MovePoint(bb[0].A, 0, localSize)));
			local.Add(new(local[^1].B, MovePoint(bb[3].A, 0, localSize)));
			local.Add(new(local[^1].B, bb[3].A));
			local.Add(new(local[^1].B, local[0].A));

			bb[0] = new(MovePoint(local[1].A, 0, localOffset), bb[1].A);
			bb[2] = new(bb[2].A, MovePoint(local[2].A, 0, localOffset));
			bb[3] = new(bb[2].B, bb[0].A);

			itemBB.TransformLocalLines(UID);
		}
		private void CutRight(string uid, string targetItemUID, float localSize, float localOffset)
		{
			var bb = (targetItemUID != null && items.ContainsKey(targetItemUID) ?
				items[targetItemUID].ButtonDetails.boundingBox : rootItem.ButtonDetails.boundingBox).LocalLines;

			var item = items[uid];
			var itemBB = item.ButtonDetails.boundingBox;
			var local = itemBB.LocalLines;

			localSize = MathF.Max(1, localSize);
			localOffset = MathF.Max(0, localOffset);

			if(bb.Count == 0)
				bb = rootItem.ButtonDetails.boundingBox.LocalLines;

			local.Add(new(MovePoint(bb[1].A, 180, localSize), bb[1].A));
			local.Add(new(local[^1].B, bb[2].A));
			local.Add(new(local[^1].B, MovePoint(bb[2].A, 180, localSize)));
			local.Add(new(local[^1].B, local[0].A));

			bb[0] = new(bb[0].A, MovePoint(local[0].A, 180, localOffset));
			bb[1] = new(bb[0].B, MovePoint(local[3].A, 180, localOffset));
			bb[2] = new(bb[1].B, bb[2].B);

			itemBB.TransformLocalLines(UID);
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			if(IsDisabled == false)
				TryTriggerEvents(); // has to be after draw

			if(IsHidden == false)
			{
				Draw(rootItem);

				foreach(var kvp in items)
					Draw(kvp.Value);
			}

			void Draw(Thing.GUI.ListItem item)
			{
				item.ButtonDetails.Draw(renderTarget);

				var bb = item.ButtonDetails.boundingBox.Lines;
				var center = bb[0].A.PercentToTarget(bb[2].A, new(50));
				item.TextDetails.UpdateGlobalText(center, Angle, Scale);
				item.TextDetails.Draw(renderTarget);
			}
		}
		private void TryTriggerEvents()
		{
			if(rootItem.ButtonDetails.IsDisabled == false)
				TriggerEventsForItem(rootItem, null);

			foreach(var kvp in items)
			{
				var item = kvp.Value;
				if(item.ButtonDetails.IsDisabled)
					continue;

				TriggerEventsForItem(item, kvp.Key);
			}

			void TriggerEventsForItem(Thing.GUI.ListItem item, string uid)
			{
				var itemBB = item.ButtonDetails.boundingBox;
				var buttonResult = itemBB.TryButton(isHoldable: false);

				var events = new List<(bool, Action<string, string>)>()
				{
					(buttonResult.IsHovered, Event.GridLayoutItemHover), (buttonResult.IsUnhovered, Event.GridLayoutItemUnhover),
					(buttonResult.IsPressed, Event.GridLayoutItemPress), (buttonResult.IsReleased, Event.GridLayoutItemRelease),
					(buttonResult.IsClicked, Event.GridLayoutItemClick), (buttonResult.IsHeld, Event.GridLayoutItemHold),
				};

				for(int j = 0; j < events.Count; j++)
					if(events[j].Item1)
						events[j].Item2.Invoke(UID, uid);
			}
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			items = null;
		}

		private static Vector2 MovePoint(Vector2 vec, float ang, float sz) => vec.MoveAtAngle(ang, sz, false);
		#endregion
	}
}
