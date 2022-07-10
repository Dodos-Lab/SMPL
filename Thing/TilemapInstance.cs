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
		public Dictionary<string, Thing.Tile> TilePalette { get; } = new();
		public Vector2 TileSize { get; set; } = new(32);
		public Vector2 TileGap { get; set; }
		public int TileCount => tileCount;

		public void SetTile(Vector2 tilePositionIndecies, string tilePaletteUID)
		{
			if(TryTilePaletteIndexError(tilePaletteUID))
				return;

			if(map.ContainsKey(tilePositionIndecies) == false)
				map[tilePositionIndecies] = new();

			tileCount++;

			UpdateCornerPoints(tilePositionIndecies);
			map[tilePositionIndecies][TilePalette[tilePaletteUID].Depth] = tilePaletteUID;
		}
		public void SetTileSquare(Vector2 tilePositionIndeciesA, Vector2 tilePositionIndeciesB, string tilePaletteUID)
		{
			if(TryTilePaletteIndexError(tilePaletteUID))
				return;

			var a = tilePositionIndeciesA;
			var b = tilePositionIndeciesB;

			SecureMinMax(ref a, ref b);

			for(float x = a.X; x < b.X; x++)
				for(float y = a.Y; y < b.Y; y++)
					SetTile(new(x, y), tilePaletteUID);
		}

		public bool HasTileAtDepth(Vector2 tilePositionIndecies, int depth)
		{
			return HasTile(tilePositionIndecies) && map[tilePositionIndecies].ContainsKey(depth);
		}
		public bool HasTile(Vector2 tilePositionIndecies)
		{
			return map.ContainsKey(tilePositionIndecies);
		}

		public void RemoveTileAtDepth(Vector2 tilePositionIndecies, int depth)
		{
			if(HasTileAtDepth(tilePositionIndecies, depth) == false)
				return;

			tileCount--;
			map[tilePositionIndecies].Remove(depth);

			RecalculateCornerPoints();
		}
		public void RemoveTiles(Vector2 tilePositionIndecies)
		{
			if(map.ContainsKey(tilePositionIndecies) == false)
				return;

			tileCount--;
			map.Remove(tilePositionIndecies);

			RecalculateCornerPoints();
		}
		public void RemoveTileSquareAtDepth(Vector2 tilePositionIndeciesA, Vector2 tilePositionIndeciesB, int depth)
		{
			var a = tilePositionIndeciesA;
			var b = tilePositionIndeciesB;

			SecureMinMax(ref a, ref b);

			for(float x = a.X; x < b.X; x++)
				for(float y = a.Y; y < b.Y; y++)
					RemoveTileAtDepth(new(x, y), depth);
		}
		public void RemoveTileSquare(Vector2 tilePositionIndeciesA, Vector2 tilePositionIndeciesB)
		{
			var a = tilePositionIndeciesA;
			var b = tilePositionIndeciesB;

			SecureMinMax(ref a, ref b);

			for(float x = a.X; x < b.X; x++)
				for(float y = a.Y; y < b.Y; y++)
					RemoveTiles(new(x, y));
		}

		public List<string> GetPaletteUIDsFromPosition(Vector2 position)
		{
			var pos = GetTileIndecies(position);
			return map.ContainsKey(pos) == false ? new() : map[pos].Values.ToList();
		}
		public List<string> GetPaletteUIDsFromTileIndecies(Vector2 tileIndecies)
		{
			return map.ContainsKey(tileIndecies) == false ? new() : map[tileIndecies].Values.ToList();
		}
		public Vector2 GetTileIndecies(Vector2 position)
		{
			return (GetLocalPositionFromSelf(position) / TileSize).PointToGrid(new(1));
		}
		public Vector2 GetTilePosition(Vector2 tileIndecies)
		{
			return GetPositionFromSelf(tileIndecies * TileSize + (TileSize * 0.5f));
		}

		#region Backend
		private int tileCount;
		private Vector2 topLeftIndecies = new(float.MaxValue, float.MaxValue), botRightIndecies = new(float.MinValue, float.MinValue);
		private readonly VertexArray vertsArr = new(PrimitiveType.Quads);

		[JsonProperty]
		private readonly Dictionary<Vector2, SortedDictionary<int, string>> map = new();

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
					var tile = TilePalette[kvp2.Value];
					var txCrdsA = tile.IndeciesTexCoord * TileSize + (TileGap * tile.IndeciesTexCoord) + TileGap;
					var txCrdsB = txCrdsA + tile.IndeciesSize * TileSize;
					var c = tile.Color;

					vertsArr.Append(new(topLeft, c, new(txCrdsA.X, txCrdsA.Y)));
					vertsArr.Append(new(topRight, c, new(txCrdsB.X, txCrdsA.Y)));
					vertsArr.Append(new(botRight, c, new(txCrdsB.X, txCrdsB.Y)));
					vertsArr.Append(new(botLeft, c, new(txCrdsA.X, txCrdsB.Y)));
				}

			renderTarget.Draw(vertsArr, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));
		}
		internal override Hitbox GetBoundingBox()
		{
			return tileCount == 0 ? base.GetBoundingBox() : new Hitbox(
				new Vector2(topLeftIndecies.X, topLeftIndecies.Y) * TileSize,
				new Vector2(botRightIndecies.X, topLeftIndecies.Y) * TileSize,
				new Vector2(botRightIndecies.X, botRightIndecies.Y) * TileSize,
				new Vector2(topLeftIndecies.X, botRightIndecies.Y) * TileSize,
				new Vector2(topLeftIndecies.X, topLeftIndecies.Y) * TileSize);
		}

		private bool TryTilePaletteIndexError(string tilePaletteUID)
		{
			if(TilePalette.Count == 0)
			{
				Console.LogError(1, $"There are no tiles in the {nameof(TilePalette)} of this Tilemap, add some before setting tiles.");
				return true;
			}
			if(TilePalette.ContainsKey(tilePaletteUID) == false)
			{
				Console.LogError(1, $"The {nameof(tilePaletteUID)} is not present in the {nameof(TilePalette)}.");
				return true;
			}
			return false;
		}
		private void RecalculateCornerPoints()
		{
			topLeftIndecies = new(float.MaxValue, float.MaxValue);
			botRightIndecies = new(float.MinValue, float.MinValue);
			foreach(var kvp in map)
				UpdateCornerPoints(kvp.Key);
		}
		private void UpdateCornerPoints(Vector2 tilePositionIndecies)
		{
			var p = tilePositionIndecies;

			if(topLeftIndecies.X > p.X)
				topLeftIndecies.X = p.X;
			if(topLeftIndecies.Y > p.Y)
				topLeftIndecies.Y = p.Y;
			if(botRightIndecies.X < p.X + 1)
				botRightIndecies.X = p.X + 1;
			if(botRightIndecies.Y < p.Y + 1)
				botRightIndecies.Y = p.Y + 1;
		}
		private static void SecureMinMax(ref Vector2 min, ref Vector2 max)
		{
			if(min.X > max.X)
				(min.X, max.X) = (max.X, min.X);
			if(min.Y > max.Y)
				(min.Y, max.Y) = (max.Y, min.Y);
		}
		#endregion
	}
}
