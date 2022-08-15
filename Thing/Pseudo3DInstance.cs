namespace SMPL
{
	internal class Pseudo3DInstance : SpriteInstance
	{
		public float Depth { get; set; } = 50;
		public float Tilt { get; set; } = 45f;
		public float PerspectiveUnit { get; set; } = 1.2f;
		public Hitbox BoundingBox3D
		{
			get
			{
				var baseBB = base.GetBoundingBox();
				baseBB.TransformLocalLines(UID);

				var h = Depth * Scale;
				var tl = baseBB.Lines[0].A.PointMoveAtAngle(Tilt, h, false);
				var tr = baseBB.Lines[1].A.PointMoveAtAngle(Tilt, h, false);
				var br = baseBB.Lines[2].A.PointMoveAtAngle(Tilt, h, false);
				var bl = baseBB.Lines[3].A.PointMoveAtAngle(Tilt, h, false);
				var center = tr.PointPercentTowardPoint(bl, new(50));
				var percent = new Vector2(PerspectiveUnit.Map(1, 0, 0, 100));

				tl = tl.PointPercentTowardPoint(center, percent);
				tr = tr.PointPercentTowardPoint(center, percent);
				br = br.PointPercentTowardPoint(center, percent);
				bl = bl.PointPercentTowardPoint(center, percent);

				bb.Lines.Clear();
				bb.LocalLines.Clear();
				bb.Lines.Add(new(tl, tr));
				bb.Lines.Add(new(tr, br));
				bb.Lines.Add(new(br, bl));
				bb.Lines.Add(new(bl, tl));

				return bb;
			}
		}

		#region Backend
		private readonly new Hitbox bb = new();

		[JsonConstructor]
		internal Pseudo3DInstance() { }
		internal Pseudo3DInstance(string uid) : base(uid) { }
		#endregion
	}
}
