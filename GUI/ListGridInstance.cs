namespace SMPL.GUI
{
	internal class ListGridInstance : SpriteInstance
	{
		public Hitbox RootBoundingBox { get; } = new();

		public void Cut(Thing.GUI.ListGridCutDetails cutDetails)
		{
			cutDetails ??= new();

			if(cutDetails.NewItemUID == null)
				return;

			var tUID = cutDetails.TargetItemUID;
			var bb = (tUID != null && items.ContainsKey(tUID) ? items[tUID].ButtonDetails.boundingBox : rootBB).Lines;
			var sz = cutDetails.Size * Scale;
			var off = cutDetails.Offset * Scale;
			var w = bb[0].A.Distance(bb[1].A);
			var h = bb[1].A.Distance(bb[2].A);

			if(cutDetails.Direction == Extensions.Direction4.Right)
			{
				bb[0] = new(bb[0].A, Move(bb[0].B));
				bb[1] = new(Move(bb[1].A), Move(bb[1].B));
				bb[2] = new(Move(bb[2].A), bb[2].B);

				Vector2 Move(Vector2 vec) => vec.MoveAtAngle(Angle - 180, sz + off, false);
			}
			else if(cutDetails.Direction == Extensions.Direction4.Left)
			{

			}
			else if(cutDetails.Direction == Extensions.Direction4.Down)
			{

			}
			else if(cutDetails.Direction == Extensions.Direction4.Up)
			{

			}
		}
		public Thing.GUI.ListItem Get(string uid)
		{
			return uid != null && items.ContainsKey(uid) ? items[uid] : default;
		}
		public void Trigger(string itemUID) => Event.ButtonClick(UID, itemUID != null && items.ContainsKey(itemUID) ? items[itemUID].ButtonDetails : null);

		#region Backend
		private readonly Hitbox rootBB = new();

		[JsonProperty]
		private Dictionary<string, Thing.GUI.ListItem> items = new();

		[JsonConstructor]
		internal ListGridInstance() { }
		internal ListGridInstance(string uid) : base(uid) { UpdateRootBB(); }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			if(IsDisabled == false)
				TryTriggerEvents(); // has to be after draw

			rootBB.Draw();
		}
		private void TryTriggerEvents()
		{

		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			items = null;
		}
		private void UpdateRootBB()
		{
			rootBB.Lines.Clear();
			var bb = BoundingBox.Lines;
			for(int i = 0; i < bb.Count; i++)
				rootBB.Lines.Add(bb[i]);
		}
		#endregion
	}
}
