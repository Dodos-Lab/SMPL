namespace SMPL
{
	internal class ClothInstance : VisualInstance
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

		public void Lock(Vector2 indexes, bool isLocked)
		{
			var x = (int)indexes.X.Limit(0, segCount.X - 1);
			var y = (int)indexes.Y.Limit(0, segCount.Y - 1);
			rope.Points[GetIndex(x, y)].IsLocked = isLocked;
		}

		#region Backend
		private Vector2 size, segCount;
		private readonly VertexArray verts = new(PrimitiveType.Quads);

		[JsonProperty]
		private readonly Rope rope;

		[JsonConstructor]
		internal ClothInstance() { }
		internal ClothInstance(string uid, Vector2 size, Vector2 segmentCount) : base(uid)
		{
			segCount = new(MathF.Max(1, segmentCount.X + 1), MathF.Max(1, segmentCount.Y + 1));
			size = new(MathF.Max(2, size.X), MathF.Max(2, size.Y));
			this.size = size;
			var sz = size / segCount;

			rope = new(Position, (int)segmentCount.Y, sz.Y);

			for(int i = 1; i < rope.Points.Count; i++)
				rope.Untie(rope.Points[i - 1], rope.Points[i], true);
			rope.Points.Clear();

			for(int y = 0; y < segCount.Y; y++)
				for(int x = 0; x < segCount.X; x++)
				{
					var point = new Rope.Point(new Vector2(x, y) * sz);
					rope.Points.Add(point);

					if(y > 0)
					{
						var upPoint = rope.Points[GetIndex(x, y - 1)];
						rope.Tie(upPoint, point);
					}
					if(x > 0)
					{
						var prevPoint = rope.Points[GetIndex(x - 1, y)];
						rope.Tie(prevPoint, point);
					}
				}
			Lock(new(), true);
			Lock(new(segCount.X, 0), true);
			Lock(segCount, true);
			Lock(new(0, segCount.Y), true);
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			rope.Position = Position;
			rope.Update();

			verts.Clear();

			Position.DrawPoint();

			var tex = GetTexture();
			var w = tex == null ? 0 : tex.Size.X;
			var h = tex == null ? 0 : tex.Size.Y;
			var w0 = w * TexCoordUnitA.X;
			var ww = w * TexCoordUnitB.X;
			var h0 = h * TexCoordUnitA.Y;
			var hh = h * TexCoordUnitB.Y;

			for(int y = 0; y < segCount.Y - 1; y++)
				for(int x = 0; x < segCount.X - 1; x++)
				{
					var topL = rope.Points[GetIndex(x, y)];
					var topR = rope.Points[GetIndex(x + 1, y)];
					var botR = rope.Points[GetIndex(x + 1, y + 1)];
					var botL = rope.Points[GetIndex(x, y + 1)];
					var tl = GetTexCoord(x, y);
					var tr = GetTexCoord(x + 1, y);
					var br = GetTexCoord(x + 1, y + 1);
					var bl = GetTexCoord(x, y + 1);

					verts.Append(new(topL.Position.ToSFML(), Tint, tl));
					verts.Append(new(topR.Position.ToSFML(), Tint, tr));
					verts.Append(new(botR.Position.ToSFML(), Tint, br));
					verts.Append(new(botL.Position.ToSFML(), Tint, bl));
				}

			renderTarget.Draw(verts, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));

			BoundingBox.Draw();

			Vector2f GetTexCoord(int x, int y)
			{
				return new Vector2f(
					((float)x).Map(0, segCount.X - 1, w0, ww),
					((float)y).Map(0, segCount.Y - 1, h0, hh));
			}
		}
		internal override Hitbox GetBoundingBox()
		{
			var center = GetLocalPositionFromSelf(rope.Points[GetIndex((int)segCount.X / 2, (int)segCount.Y / 2)].Position);
			var sz = size * 0.5f;
			var hitbox = new Hitbox(
				center + new Vector2(-sz.X, -sz.Y),
				center + new Vector2(sz.X, -sz.Y),
				center + new Vector2(sz.X, sz.Y),
				center + new Vector2(-sz.X, sz.Y),
				center + new Vector2(-sz.X, -sz.Y));
			return hitbox;
		}

		private int GetIndex(int x, int y)
		{
			return y * (int)segCount.X + x;
		}
		private Vector2 GetIndexes(int i)
		{
			return new(i / segCount.X, i % segCount.X);
		}
		#endregion
	}
}
