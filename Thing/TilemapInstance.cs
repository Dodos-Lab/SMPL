namespace SMPL
{
	public static partial class Thing
	{
		public struct Tile
		{
			public Vector2 IndeciesTexCoord { get; set; }
			public Vector2 IndeciesSize { get; set; }
			public Color Color { get; set; }
			public int Depth { get; set; }

			public Tile(Vector2 texCoordIndecies, int depth = 0, float indexSizeX = 1, float indexSizeY = 1,
				byte r = 255, byte g = 255, byte b = 255, byte a = 255)
			{
				IndeciesTexCoord = texCoordIndecies;
				IndeciesSize = new(indexSizeX, indexSizeY);
				Color = new(r, g, b, a);
				Depth = depth;
			}
		}
	}
	internal class TilemapInstance : VisualInstance
	{
		public Vector2 TileSize { get; set; } = new(32);
		public Vector2 TileGap { get; set; }

		public void SetTile(Vector2 tilePositionIndecies, Thing.Tile tile)
		{
			if(map.ContainsKey(tilePositionIndecies) == false)
				map[tilePositionIndecies] = new();

			map[tilePositionIndecies][tile.Depth] = tile;
		}
		public void SetTileSquare(Vector2 tilePositionIndeciesA, Vector2 tilePositionIndeciesB, Thing.Tile tile)
		{
			var a = tilePositionIndeciesA;
			var b = tilePositionIndeciesB;

			if(a.X > b.X)
				(a.X, b.X) = (b.X, a.X);
			if(a.Y > b.Y)
				(a.Y, b.Y) = (b.Y, a.Y);

			for(float x = a.X; x < b.X; x++)
				for(float y = a.Y; y < b.Y; y++)
					SetTile(new(x, y), tile);
		}

		public List<Thing.Tile> GetTilesFromPosition(Vector2 position)
		{
			var pos = GetTileIndecies(position);
			return map.ContainsKey(pos) == false ? new() : map[pos].Values.ToList();
		}
		public List<Thing.Tile> GetTilesFromIndecies(Vector2 tileIndecies)
		{
			return map.ContainsKey(tileIndecies) == false ? new() : map[tileIndecies].Values.ToList();
		}
		public Vector2 GetTileIndecies(Vector2 position)
		{
			return (GetLocalPositionFromSelf(position) / TileSize).PointToGrid(TileSize) / TileSize;
		}
		public Vector2 GetTilePosition(Vector2 tileIndecies)
		{
			return GetPositionFromSelf(tileIndecies * TileSize);
		}

		#region Backend
		private readonly VertexArray vertsArr = new(PrimitiveType.Quads);
		[JsonProperty]
		private readonly Dictionary<Vector2, SortedDictionary<float, Thing.Tile>> map = new();

		[JsonConstructor]
		internal TilemapInstance() { }
		internal TilemapInstance(string uid) : base(uid) { }
		internal override void OnDestroy()
		{
			base.OnDestroy();
			map.Clear();
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			vertsArr.Clear();

			foreach(var kvp in map)
				foreach(var kvp2 in kvp.Value.Reverse())
				{
					var localPos = kvp.Key * TileSize;
					var topLeft = GetPositionFromSelf(localPos).ToSFML();
					var topRight = GetPositionFromSelf(localPos + new Vector2(TileSize.X, 0)).ToSFML();
					var botRight = GetPositionFromSelf(localPos + TileSize).ToSFML();
					var botLeft = GetPositionFromSelf(localPos + new Vector2(0, TileSize.Y)).ToSFML();
					var txCrdsA = kvp2.Value.IndeciesTexCoord * TileSize + (TileGap * kvp2.Value.IndeciesTexCoord);
					var txCrdsB = txCrdsA + kvp2.Value.IndeciesSize * TileSize;
					var c = kvp2.Value.Color;

					vertsArr.Append(new(topLeft, c, new(txCrdsA.X, txCrdsA.Y)));
					vertsArr.Append(new(topRight, c, new(txCrdsB.X, txCrdsA.Y)));
					vertsArr.Append(new(botRight, c, new(txCrdsB.X, txCrdsB.Y)));
					vertsArr.Append(new(botLeft, c, new(txCrdsA.X, txCrdsB.Y)));
				}

			renderTarget.Draw(vertsArr, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));
		}
		internal override Hitbox GetBoundingBox()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
