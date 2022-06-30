namespace SMPL.Graphics
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
		public Vector2 TexCoordUnitB { get; set; } = new(1, 1);

		public Vector2 LocalSize { get; set; } = new(100, 100);
		[JsonIgnore]
		public Vector2 Size
		{
			get => LocalSize * Scale;
			set => LocalSize = value / Scale;
		}

		public Vector2 OriginUnit { get; set; } = new(0.5f, 0.5f);
		[JsonIgnore]
		public Vector2 Origin
		{
			get => OriginUnit * LocalSize;
			set => OriginUnit = value / LocalSize;
		}

		public override Vector2 GetCornerClockwise(int index)
		{
			index = index.Limit(0, 4, Extensions.Limitation.Overflow);
			return index switch
			{
				0 => GetPositionFromSelf(-Origin),
				1 => GetPositionFromSelf(new Vector2(LocalSize.X, 0) - Origin),
				2 => GetPositionFromSelf(LocalSize - Origin),
				3 => GetPositionFromSelf(new Vector2(0, LocalSize.Y) - Origin),
				_ => default,
			};
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

			var verts = new Vertex[]
			{
				new(GetCornerClockwise(0).ToSFML(), Tint, new(w0, h0)),
				new(GetCornerClockwise(1).ToSFML(), Tint, new(ww, h0)),
				new(GetCornerClockwise(2).ToSFML(), Tint, new(ww, hh)),
				new(GetCornerClockwise(3).ToSFML(), Tint, new(w0, hh)),
			};

			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader(renderTarget)));
		}

		internal override Hitbox GetBoundingBox()
		{
			var hitbox = new Hitbox(
				-Origin,
				new Vector2(LocalSize.X, 0) - Origin,
				LocalSize - Origin,
				new Vector2(0, LocalSize.Y) - Origin,
				-Origin);
			hitbox.TransformLocalLines(UID);
			return hitbox;
		}
		#endregion
	}
}
