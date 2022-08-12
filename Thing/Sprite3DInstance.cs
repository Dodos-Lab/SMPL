namespace SMPL
{
	internal class Sprite3DInstance : SpriteInstance
	{
		public List<string> TexturePaths { get; } = new();
		public float Height { get; set; } = 20;
		public float Angle3D { get; set; } = 270;
		public Hitbox BoundingBox3D
		{
			get
			{
				var baseBB = base.GetBoundingBox();
				baseBB.TransformLocalLines(UID);

				var h = Height * Scale;
				var tl = baseBB.Lines[0].A.PointMoveAtAngle(Angle3D, h, false);
				var tr = baseBB.Lines[1].A.PointMoveAtAngle(Angle3D, h, false);
				var br = baseBB.Lines[2].A.PointMoveAtAngle(Angle3D, h, false);
				var bl = baseBB.Lines[3].A.PointMoveAtAngle(Angle3D, h, false);

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

		[JsonConstructor]
		internal Sprite3DInstance() { }
		internal Sprite3DInstance(string uid, params string[] texturePaths) : base(uid)
		{
			TexturePaths = texturePaths?.ToList();
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden || TexturePaths.Count == 0)
				return;

			var h = Height * Scale / TexturePaths.Count;
			var baseBB = base.GetBoundingBox();
			baseBB.TransformLocalLines(UID);

			for(int i = 0; i < TexturePaths.Count; i++)
			{
				var tex = Scene.CurrentScene.Textures[TexturePaths[i]];
				var verts = new Vertex[]
				{
					new(baseBB.Lines[0].A.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(0, 0)),
					new(baseBB.Lines[1].A.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(tex.Size.X, 0)),
					new(baseBB.Lines[2].A.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(tex.Size.X, tex.Size.Y)),
					new(baseBB.Lines[3].A.PointMoveAtAngle(Angle3D, i * h, false).ToSFML(), Tint, new(0, tex.Size.Y)),
				};

				renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader(renderTarget)));
			}
		}
		#endregion
	}
}
