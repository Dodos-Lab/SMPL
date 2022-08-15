namespace SMPL
{
	public static partial class Thing
	{
		public struct CubeSide
		{
			public bool IsHidden { get; set; }
			public string TexturePath { get; set; }

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
			public Vector2 TexCoordUnitB { get; set; }

			public CubeSide(string texturePath)
			{
				IsHidden = false;
				TexturePath = texturePath;
				TexCoordUnitA = new();
				TexCoordUnitB = new(1);
			}

			internal Vector2 GetTexSize()
			{
				var tex = GetTexture();
				return tex == default ? new Vector2() : new(tex.Size.X, tex.Size.Y);
			}
			internal Texture GetTexture()
			{
				var path = TexturePath.ToBackslashPath();
				return HasTexture() ? Scene.CurrentScene.Textures[path] : null;
			}
			internal bool HasTexture(string trySetTexture = null)
			{
				if(trySetTexture != null && TexturePath != trySetTexture)
					TexturePath = trySetTexture;

				var path = TexturePath.ToBackslashPath();
				return path != null && Scene.CurrentScene.Textures.ContainsKey(path);
			}
		}
	}

	internal class CubeInstance : Pseudo3DInstance
	{
		public Thing.CubeSide SideNear { get; set; } = defCubeSide;
		public Thing.CubeSide SideLeft { get; set; } = defCubeSide;
		public Thing.CubeSide SideRight { get; set; } = defCubeSide;
		public Thing.CubeSide SideTop { get; set; } = defCubeSide;
		public Thing.CubeSide SideBottom { get; set; } = defCubeSide;

		#region Backend
		private static Thing.CubeSide defCubeSide = new() { TexCoordUnitB = new(1) };
		private readonly SortedDictionary<float, List<Action>> sortedSides = new();

		[JsonConstructor]
		internal CubeInstance() { }
		internal CubeInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden == false)
				base.OnDraw(renderTarget);

			if(PerspectiveUnit == 1f)
				PerspectiveUnit = 1.01f;

			var farBB = BoundingBox;
			var nearBB = BoundingBox3D;

			var farTl = farBB.Lines[0].A;
			var farTr = farBB.Lines[1].A;
			var farBr = farBB.Lines[2].A;
			var farBl = farBB.Lines[3].A;
			var nearTl = nearBB.Lines[0].A;
			var nearTr = nearBB.Lines[1].A;
			var nearBr = nearBB.Lines[2].A;
			var nearBl = nearBB.Lines[3].A;
			var tilt = Tilt.AngleTo360();

			var l = farTl.DistanceBetweenPoints(nearTl) + farBl.DistanceBetweenPoints(nearBl);
			var r = farTr.DistanceBetweenPoints(nearTr) + farBr.DistanceBetweenPoints(nearBr);
			var t = farTl.DistanceBetweenPoints(nearTl) + farTr.DistanceBetweenPoints(nearTr);
			var b = farBl.DistanceBetweenPoints(nearBl) + farBr.DistanceBetweenPoints(nearBr);

			sortedSides.Clear();
			sortedSides[r] = new();
			sortedSides[l] = new();
			sortedSides[t] = new();
			sortedSides[b] = new();
			sortedSides[r].Add(TryDrawRight);
			sortedSides[l].Add(TryDrawLeft);
			sortedSides[t].Add(TryDrawTop);
			sortedSides[b].Add(TryDrawBottom);

			var enumerable = PerspectiveUnit < 1 ? sortedSides : sortedSides.Reverse();

			foreach(var side in enumerable)
				for(int i = 0; i < side.Value.Count; i++)
					side.Value[i].Invoke();

			TryDrawSide(SideNear, nearTl, nearTr, nearBr, nearBl);

			void TryDrawRight() => TryDrawSide(SideRight, nearBr, nearTr, farTr, farBr);
			void TryDrawLeft() => TryDrawSide(SideLeft, nearTl, nearBl, farBl, farTl);
			void TryDrawTop() => TryDrawSide(SideTop, nearTr, nearTl, farTl, farTr);
			void TryDrawBottom() => TryDrawSide(SideBottom, nearBl, nearBr, farBr, farBl);
			void TryDrawSide(Thing.CubeSide cubeSide, Vector2 tl, Vector2 tr, Vector2 br, Vector2 bl)
			{
				if(cubeSide.IsHidden || cubeSide.HasTexture(TexturePath) == false)
					return;

				var sz = cubeSide.GetTexSize();
				var tex = cubeSide.GetTexture();
				var w = tex == null ? 0 : sz.X;
				var h = tex == null ? 0 : sz.Y;
				var w0 = w * cubeSide.TexCoordUnitA.X;
				var ww = w * cubeSide.TexCoordUnitB.X;
				var h0 = h * cubeSide.TexCoordUnitA.Y;
				var hh = h * cubeSide.TexCoordUnitB.Y;

				var verts = new Vertex[]
				{
					new(tl.ToSFML(), Tint, new(w0, h0)),
					new(tr.ToSFML(), Tint, new(ww, h0)),
					new(br.ToSFML(), Tint, new(ww, hh)),
					new(bl.ToSFML(), Tint, new(w0, hh))
				};

				renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, cubeSide.GetTexture(), GetShader(renderTarget)));
			}
		}
		#endregion
	}
}
