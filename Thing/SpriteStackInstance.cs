namespace SMPL
{
	internal class SpriteStackInstance : Pseudo3DInstance
	{
		public Hitbox BoundingBox3D
		{
			get
			{
				var baseBB = base.GetBoundingBox();
				baseBB.TransformLocalLines(UID);

				if(baseBB.Lines.Count != 4)
					return bb;

				var h = Depth * Scale;
				var tl = baseBB.Lines[0].A.PointMoveAtAngle(Tilt, h, false);
				var tr = baseBB.Lines[1].A.PointMoveAtAngle(Tilt, h, false);
				var br = baseBB.Lines[2].A.PointMoveAtAngle(Tilt, h, false);
				var bl = baseBB.Lines[3].A.PointMoveAtAngle(Tilt, h, false);

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
		internal SpriteStackInstance() { }
		internal SpriteStackInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden || TexturePath == null || Scene.CurrentScene.loadedTextureStacks.ContainsKey(TexturePath) == false)
				return;

			var texStack = Scene.CurrentScene.loadedTextureStacks[TexturePath];
			var h = Depth * Scale / texStack.Count;
			var txs = Scene.CurrentScene.Textures;
			var bb = BoundingBox.Lines;

			for(int i = 0; i < texStack.Count; i++)
			{
				var tex = txs.ContainsKey(texStack[i]) ? txs[texStack[i]] : default;
				var sz = tex == default ? new Vector2() : new(tex.Size.X, tex.Size.Y);

				var prevSmooth = false;
				if(tex != null)
				{
					prevSmooth = tex.Smooth;
					tex.Smooth = IsSmooth;
				}

				var verts = new Vertex[]
				{
					new(bb[0].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(0, 0)),
					new(bb[1].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(sz.X, 0)),
					new(bb[2].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(sz.X, sz.Y)),
					new(bb[3].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(0, sz.Y)),
				};

				renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader(renderTarget)));

				if(tex != null)
					tex.Smooth = prevSmooth;
			}
		}
		internal override Hitbox GetBoundingBox()
		{
			var tl = -Origin;
			var tr = new Vector2(LocalSize.X, 0) - Origin;
			var br = LocalSize - Origin;
			var bl = new Vector2(0, LocalSize.Y) - Origin;

			bb.Lines.Clear();
			bb.LocalLines.Clear();
			bb.LocalLines.Add(new(tl, tr));
			bb.LocalLines.Add(new(tr, br));
			bb.LocalLines.Add(new(br, bl));
			bb.LocalLines.Add(new(bl, tl));
			return bb;
		}
		#endregion
	}
}
