namespace SMPL
{
	internal class SpriteStackInstance : Pseudo3DInstance
	{
		public List<string> TexturePaths { get; } = new();

		#region Backend
		[JsonConstructor]
		internal SpriteStackInstance() { }
		internal SpriteStackInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden || TexturePaths.Count == 0)
				return;

			var h = Depth * Scale / TexturePaths.Count;
			var txs = Scene.CurrentScene.Textures;
			var bb = BoundingBox.Lines;

			for(int i = 0; i < TexturePaths.Count; i++)
			{
				var tex = txs.ContainsKey(TexturePaths[i]) ? txs[TexturePaths[i]] : default;
				var sz = tex == default ? new Vector2() : new(tex.Size.X, tex.Size.Y);

				var verts = new Vertex[]
				{
					new(bb[0].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(0, 0)),
					new(bb[1].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(sz.X, 0)),
					new(bb[2].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(sz.X, sz.Y)),
					new(bb[3].A.PointMoveAtAngle(Tilt, i * h, false).ToSFML(), Tint, new(0, sz.Y)),
				};

				renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader(renderTarget)));
			}
		}
		#endregion
	}
}
