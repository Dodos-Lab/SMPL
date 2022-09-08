namespace SMPL
{
	internal class SpriteInstance : VisualInstance
	{
		[JsonIgnore]
		public Vector2 TexCoordA
		{
			set
			{
				var tex = GetTexture();
				TexCoordUnitA = tex == null ? value : value / new Vector2(tex.Size.X, tex.Size.Y);
			}
			get
			{
				var tex = GetTexture();
				return tex == null ? TexCoordUnitA : new Vector2(tex.Size.X, tex.Size.Y) * TexCoordUnitA;
			}
		}
		[JsonIgnore]
		public Vector2 TexCoordB
		{
			set
			{
				var tex = GetTexture();
				TexCoordUnitB = tex == null ? value : value / new Vector2(tex.Size.X, tex.Size.Y);
			}
			get
			{
				var tex = GetTexture();
				return tex == null ? TexCoordUnitB : new Vector2(tex.Size.X, tex.Size.Y) * TexCoordUnitB;
			}
		}

		public Vector2 TexCoordUnitA { get; set; }
		public Vector2 TexCoordUnitB { get; set; } = new(1);

		public virtual Vector2 LocalSize { get; set; } = new(100);
		[JsonIgnore]
		public virtual Vector2 Size
		{
			get => LocalSize * Scale;
			set => LocalSize = value / Scale;
		}

		public Vector2 OriginUnit { get; set; } = new(0.5f);
		[JsonIgnore]
		public Vector2 Origin
		{
			get => OriginUnit * LocalSize;
			set => OriginUnit = value / LocalSize;
		}

		public override Hitbox BoundingBox
		{
			get
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
				bb.TransformLocalLines(UID);
				return bb;
			}
		}

		#region Backend
		[JsonConstructor]
		internal SpriteInstance() { }
		internal SpriteInstance(string uid) : base(uid) { }
		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			var tex = GetTexture();
			var w = tex == null ? 0 : tex.Size.X;
			var h = tex == null ? 0 : tex.Size.Y;
			var w0 = w * TexCoordUnitA.X;
			var ww = w * TexCoordUnitB.X;
			var h0 = h * TexCoordUnitA.Y;
			var hh = h * TexCoordUnitB.Y;

			var lines = BoundingBox.Lines;
			var verts = new Vertex[]
			{
				new(lines[0].A.ToSFML(), Tint, new(w0, h0)),
				new(lines[1].A.ToSFML(), Tint, new(ww, h0)),
				new(lines[2].A.ToSFML(), Tint, new(ww, hh)),
				new(lines[3].A.ToSFML(), Tint, new(w0, hh)),
			};

			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader(renderTarget)));
		}
		#endregion
	}
}
