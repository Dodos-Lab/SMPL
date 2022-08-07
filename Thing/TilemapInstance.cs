namespace SMPL
{
	public static partial class Thing
	{
		public struct Tile
		{
			public Vector2 Indexes { get; set; }
			public Color Color { get; set; }

			public Tile(Vector2 texCoordIndexes, byte r = 255, byte g = 255, byte b = 255, byte a = 255)
			{
				Indexes = texCoordIndexes;
				Color = new(r, g, b, a);
			}
		}
	}
	internal class TilemapInstance : VisualInstance
	{
		public Dictionary<string, Thing.Tile> TilePalette { get; } = new();
		public Vector2 TileSize { get; set; } = new(32);
		public Vector2 TileGap { get; set; }
		[JsonIgnore]
		public int TileCount => tileCount;

		public void SetTile(Vector2 tilePositionIndexes, string tilePaletteUID)
		{
			if(TryTilePaletteError(tilePaletteUID))
				return;

			if(map.ContainsKey(tilePositionIndexes) == false)
				tileCount++;

			UpdateCornerPoints(tilePositionIndexes);
			map[tilePositionIndexes] = tilePaletteUID;
		}
		public void SetTileSquare(Vector2 tilePositionIndexesA, Vector2 tilePositionIndexesB, string tilePaletteUID)
		{
			if(TryTilePaletteError(tilePaletteUID))
				return;

			var a = tilePositionIndexesA;
			var b = tilePositionIndexesB;

			SecureMinMax(ref a, ref b);

			for(float x = a.X; x < b.X; x++)
				for(float y = a.Y; y < b.Y; y++)
					SetTile(new(x, y), tilePaletteUID);
		}
		public void RemoveTiles(Vector2 tilePositionIndexes)
		{
			if(map.ContainsKey(tilePositionIndexes) == false)
				return;

			tileCount--;
			map.Remove(tilePositionIndexes);

			RecalculateCornerPoints();
		}
		public void RemoveTileSquare(Vector2 tilePositionIndexesA, Vector2 tilePositionIndexesB)
		{
			var a = tilePositionIndexesA;
			var b = tilePositionIndexesB;

			SecureMinMax(ref a, ref b);

			for(float x = a.X; x < b.X; x++)
				for(float y = a.Y; y < b.Y; y++)
					RemoveTiles(new(x, y));
		}
		public bool HasTile(Vector2 tilePositionIndexes)
		{
			return map.ContainsKey(tilePositionIndexes);
		}

		public string GetPaletteUIDsFromPosition(Vector2 position)
		{
			return GetPaletteUIDsFromTileIndexes(GetTileIndexes(position));
		}
		public string GetPaletteUIDsFromTileIndexes(Vector2 tileIndexes)
		{
			return map.ContainsKey(tileIndexes) == false ? default : map[tileIndexes];
		}
		public Vector2 GetTileIndexes(Vector2 position)
		{
			return (GetLocalPositionFromSelf(position) / (TileSize + TileGap)).PointToGrid(new(1));
		}
		public Vector2 GetTilePosition(Vector2 tileIndexes)
		{
			return GetPositionFromSelf(tileIndexes * TileSize + (TileSize * 0.5f));
		}

		#region Backend
		private int tileCount;
		private Vector2 topLeftIndexes = new(float.MaxValue, float.MaxValue), botRightIndexes = new(float.MinValue, float.MinValue);
		private readonly VertexArray vertsArr = new(PrimitiveType.Quads);
		private readonly Dictionary<Vector2, string> map = new();

		[JsonProperty]
		private readonly Dictionary<string, string> jsonMap = new();

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
			{
				var localPos = kvp.Key * TileSize;
				var topLeft = GetPositionFromSelf(localPos).ToSFML();
				var topRight = GetPositionFromSelf(localPos + new Vector2(TileSize.X, 0)).ToSFML();
				var botRight = GetPositionFromSelf(localPos + TileSize).ToSFML();
				var botLeft = GetPositionFromSelf(localPos + new Vector2(0, TileSize.Y)).ToSFML();
				var tile = TilePalette[kvp.Value];
				var txCrdsA = tile.Indexes * TileSize + (TileGap * tile.Indexes);
				var txCrdsB = txCrdsA + TileSize;
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
			if(tileCount == 0)
				return base.GetBoundingBox();

			var tl = new Vector2(topLeftIndexes.X, topLeftIndexes.Y) * TileSize;
			var tr = new Vector2(botRightIndexes.X, topLeftIndexes.Y) * TileSize;
			var br = new Vector2(botRightIndexes.X, botRightIndexes.Y) * TileSize;
			var bl = new Vector2(topLeftIndexes.X, botRightIndexes.Y) * TileSize;

			bb.Lines.Clear();
			bb.LocalLines.Clear();
			bb.LocalLines.Add(new(tl, tr));
			bb.LocalLines.Add(new(tr, br));
			bb.LocalLines.Add(new(br, bl));
			bb.LocalLines.Add(new(bl, tl));
			return bb;
		}

		internal void MapToJSON()
		{
			jsonMap.Clear();

			foreach(var kvp in map)
				jsonMap[JsonConvert.SerializeObject(kvp.Key)] = kvp.Value;
		}
		internal void MapFromJSON()
		{
			map.Clear();

			foreach(var kvp in jsonMap)
			{
				map[JsonConvert.DeserializeObject<Vector2>(kvp.Key)] = kvp.Value;
				tileCount++;
			}

			RecalculateCornerPoints();
		}

		private bool TryTilePaletteError(string tilePaletteUID)
		{
			if(TilePalette.Count == 0)
			{
				Console.LogError(1, $"There are no tiles in the {nameof(TilePalette)} of this Tilemap, add some before setting tiles.");
				return true;
			}
			if(TilePalette.ContainsKey(tilePaletteUID) == false)
			{
				Console.LogError(1, $"The {nameof(tilePaletteUID)} '{tilePaletteUID}' is not present in the {nameof(TilePalette)}.");
				return true;
			}
			return false;
		}
		private void RecalculateCornerPoints()
		{
			topLeftIndexes = new(float.MaxValue, float.MaxValue);
			botRightIndexes = new(float.MinValue, float.MinValue);
			foreach(var kvp in map)
				UpdateCornerPoints(kvp.Key);
		}
		private void UpdateCornerPoints(Vector2 tilePositionIndexes)
		{
			var p = tilePositionIndexes;

			if(topLeftIndexes.X > p.X)
				topLeftIndexes.X = p.X;
			if(topLeftIndexes.Y > p.Y)
				topLeftIndexes.Y = p.Y;
			if(botRightIndexes.X < p.X + 1)
				botRightIndexes.X = p.X + 1;
			if(botRightIndexes.Y < p.Y + 1)
				botRightIndexes.Y = p.Y + 1;
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
