namespace SMPL
{
	internal class ClothInstance : VisualInstance
	{
		public bool HasThreads { get; set; } = true;

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

		public float BreakThreshold { get; set; } = 30f;
		public Vector2 Force
		{
			get => rope.Force;
			set => rope.Force = value;
		}
		public Vector2 Gravity
		{
			get => rope.Gravity;
			set => rope.Gravity = value;
		}

		public void Pin(Vector2 indexes, bool isPinned)
		{
			if(indexes == default)
				return;

			rope.Points[GetIndex(indexes)].IsPinned = isPinned;
		}
		public void Cut(Vector2 indexes)
		{
			var i1 = GetIndex(indexes);
			var i2 = GetIndex(indexes + new Vector2(1, 0));
			var i3 = GetIndex(indexes + new Vector2(0, 1));
			rope.Untie(rope.Points[i1], rope.Points[i2], true);
			rope.Untie(rope.Points[i1], rope.Points[i3], true);
		}

		#region Backend
		private bool prevDisabled;
		private Vector2 prevPos;
		private float prevAng, prevSc;
		private Vector2 segCount, segSize;
		private readonly VertexArray verts = new(PrimitiveType.Quads);

		[JsonProperty]
		private readonly Rope rope;

		[JsonConstructor]
		internal ClothInstance() { }
		internal ClothInstance(string uid, Vector2 size, Vector2 segmentCount) : base(uid)
		{
			segCount = new((segmentCount.X + 1).Limit(1, 10), (segmentCount.Y + 1).Limit(1, 10));
			size = new(MathF.Max(2, size.X), MathF.Max(2, size.Y));
			var sz = size / segCount;
			segSize = sz;

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
						var upPoint = rope.Points[GetIndex(new(x, y - 1))];
						rope.Tie(upPoint, point);
					}
					if(x > 0)
					{
						var prevPoint = rope.Points[GetIndex(new(x - 1, y))];
						rope.Tie(prevPoint, point);
					}
				}

			Pin(new(), true);
			Pin(new(segCount.X, 0), true);
			Pin(segCount, true);
			Pin(new(0, segCount.Y), true);
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			rope.Position = Position;

			if(IsDisabled == false)
			{
				rope.Update();
				TryTear();
			}
			else if((IsDisabled && prevDisabled == false) || prevPos != Position || prevAng != Angle || prevSc != Scale)
				NonSimulationUpdate();

			prevPos = Position;
			prevAng = Angle;
			prevSc = Scale;
			prevDisabled = IsDisabled;

			if(IsHidden)
				return;

			var tex = GetTexture();
			var w = tex == null ? 0 : tex.Size.X;
			var h = tex == null ? 0 : tex.Size.Y;
			var w0 = w * TexCoordUnitA.X;
			var ww = w * TexCoordUnitB.X;
			var h0 = h * TexCoordUnitA.Y;
			var hh = h * TexCoordUnitB.Y;

			verts.Clear();
			for(int y = 0; y < segCount.Y - 1; y++)
				for(int x = 0; x < segCount.X - 1; x++)
				{
					var topL = rope.Points[GetIndex(new(x, y))];
					var topR = rope.Points[GetIndex(new(x + 1, y))];
					var botR = rope.Points[GetIndex(new(x + 1, y + 1))];
					var botL = rope.Points[GetIndex(new(x, y + 1))];

					if(topL.skip || topR.skip || botR.skip || botR.skip)
						continue;

					var tl = GetTexCoord(x, y);
					var tr = GetTexCoord(x + 1, y);
					var br = GetTexCoord(x + 1, y + 1);
					var bl = GetTexCoord(x, y + 1);

					verts.Append(new(topL.Position.ToSFML(), Tint, tl));
					verts.Append(new(topR.Position.ToSFML(), Tint, tr));
					verts.Append(new(botR.Position.ToSFML(), Tint, br));
					verts.Append(new(botL.Position.ToSFML(), Tint, bl));
				}

			var rendState = new RenderStates(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget));
			renderTarget.Draw(verts, rendState);

			if(HasThreads)
			{
				var lineVerts = rope.Lines.ToVertices(Tint, 1);
				renderTarget.Draw(lineVerts, PrimitiveType.Quads, rendState);
			}

			Vector2f GetTexCoord(int x, int y)
			{
				return new Vector2f(
					((float)x).Map(0, segCount.X - 1, w0, ww),
					((float)y).Map(0, segCount.Y - 1, h0, hh));
			}
		}
		internal override Hitbox GetBoundingBox()
		{
			var tl = rope.Points[0].Position;
			var tr = rope.Points[GetIndex(new((int)segCount.X - 1, 0))].Position;
			var br = rope.Points[^1].Position;
			var bl = rope.Points[GetIndex(new(0, (int)segCount.Y - 1))].Position;

			bb.LocalLines.Clear();
			bb.Lines.Clear();
			bb.Lines.Add(new(tl, tr));
			bb.Lines.Add(new(tr, br));
			bb.Lines.Add(new(br, bl));
			bb.Lines.Add(new(bl, tl));
			return bb;
		}
		internal override void OnDestroy()
		{
			verts.Dispose();
			base.OnDestroy();
		}

		private int GetIndex(Vector2 indexes)
		{
			indexes.X = indexes.X.Limit(0, segCount.X - 1);
			indexes.Y = indexes.Y.Limit(0, segCount.Y - 1);
			return (int)indexes.Y * (int)segCount.X + (int)indexes.X;
		}
		private void TryTear()
		{
			if(BreakThreshold <= 0)
				return;

			var tl = rope.Points[0].Position;
			var tr = rope.Points[GetIndex(new((int)segCount.X - 1, 0))].Position;
			var br = rope.Points[^1].Position;
			var bl = rope.Points[GetIndex(new(0, (int)segCount.Y - 1))].Position;

			var tltr = tl.DistanceBetweenPoints(tr);
			var blbr = tl.DistanceBetweenPoints(bl);
			var trbr = tr.DistanceBetweenPoints(br);
			var tlbl = tl.DistanceBetweenPoints(bl);
			var sz = segCount.X * segSize.X;
			var tearStep = segCount / BreakThreshold;

			for(int i = 0; i < segCount.Y; i++)
				if(ShouldBreak(tltr, tearStep.Y, i))
					Cut(new(segCount.X / 2, i));
			for(int i = 0; i < segCount.Y; i++)
				if(ShouldBreak(blbr, tearStep.Y, i))
					Cut(new(segCount.X / 2, i));

			for(int i = 0; i < segCount.X; i++)
				if(ShouldBreak(tlbl, tearStep.X, i))
					Cut(new(i, segCount.Y / 2));
			for(int i = 0; i < segCount.X; i++)
				if(ShouldBreak(trbr, tearStep.X, i))
					Cut(new(i, segCount.Y / 2));

			bool ShouldBreak(float dist, float tearStep, int i)
			{
				return dist > sz * (1 + (tearStep * (i + 1)));
			}
		}
		private void NonSimulationUpdate()
		{
			for(int y = 0; y < segCount.Y; y++)
				for(int x = 0; x < segCount.X; x++)
				{
					var indexes = new Vector2(x, y);
					var pos = GetPositionFromSelf(indexes * segSize);
					var point = rope.Points[GetIndex(indexes)];
					point.Position = pos;
					point.prevPosition = pos;
				}
		}
		#endregion
	}
}
