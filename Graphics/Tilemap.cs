namespace SMPL.Graphics
{
	public struct Tile
	{
		public Vector2 IndeciesTexCoords { get; set; }
		public Vector2 IndeciesSize { get; set; }
		public Color Color { get; set; }
		public float Layer { get; set; }

		public Tile(Vector2 texCoordsIndecies, float layer = 0, float indexSizeX = 1, float indexSizeY = 1,
			byte r = 255, byte g = 255, byte b = 255, byte a = 255)
		{
			IndeciesTexCoords = texCoordsIndecies;
			IndeciesSize = new(indexSizeX, indexSizeY);
			Color = new(r, g, b, a);
			Layer = layer;
		}
	}
	internal class Tilemap : Visual
	{
		public Vector2 CellSize { get; set; } = new(32);

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(IsHidden)
				return;

			var camera = Get<Camera>(CameraUID);
			if(camera != null)
				renderTarget = camera.renderTexture;

			var vertsArr = new VertexArray(PrimitiveType.Quads);

			foreach(var kvp in map)
				foreach(var kvp2 in kvp.Value)
				{
					var localPos = kvp.Key * CellSize;
					var topLeft = GetPositionFromSelf(localPos).ToSFML();
					var topRight = GetPositionFromSelf(localPos + new Vector2(CellSize.X, 0)).ToSFML();
					var botRight = GetPositionFromSelf(localPos + CellSize).ToSFML();
					var botLeft = GetPositionFromSelf(localPos + new Vector2(0, CellSize.Y)).ToSFML();
					var txCrdsA = kvp2.Value.IndeciesTexCoords * CellSize;
					var txCrdsB = txCrdsA + kvp2.Value.IndeciesSize * CellSize;
					var c = kvp2.Value.Color;

					vertsArr.Append(new(topLeft, c, new(txCrdsA.X, txCrdsA.Y)));
					vertsArr.Append(new(topRight, c, new(txCrdsB.X, txCrdsA.Y)));
					vertsArr.Append(new(botRight, c, new(txCrdsB.X, txCrdsB.Y)));
					vertsArr.Append(new(botLeft, c, new(txCrdsA.X, txCrdsB.Y)));
				}

			renderTarget.Draw(vertsArr, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader()));
			vertsArr.Dispose();
		}
		public void SetTile(Vector2 tilePositionIndecies, Tile tile)
		{
			if(map.ContainsKey(tilePositionIndecies) == false)
				map[tilePositionIndecies] = new();

			map[tilePositionIndecies][tile.Layer] = tile;
		}
		public void SetTileSquare(Vector2 tilePositionIndeciesA, Vector2 tilePositionIndeciesB, Tile tile)
		{
			if(tilePositionIndeciesA.X > tilePositionIndeciesB.X)
			{
				var swap = tilePositionIndeciesB.X;
				tilePositionIndeciesB.X = tilePositionIndeciesA.X;
				tilePositionIndeciesA.X = swap;
			}
			if(tilePositionIndeciesA.Y > tilePositionIndeciesB.Y)
			{
				var swap = tilePositionIndeciesB.Y;
				tilePositionIndeciesB.Y = tilePositionIndeciesA.Y;
				tilePositionIndeciesA.Y = swap;
			}

			for(float x = tilePositionIndeciesA.X; x < tilePositionIndeciesB.X; x++)
				for(float y = tilePositionIndeciesA.Y; y < tilePositionIndeciesB.Y; y++)
					SetTile(new(x, y), tile);
		}

		public List<Tile> GetTilesFromPosition(Vector2 position)
		{
			var pos = GetTileIndecies(position);
			return map.ContainsKey(pos) == false ? new() : map[pos].Values.ToList();
		}
		public List<Tile> GetTilesFromIndecies(Vector2 tileIndecies)
		{
			return map.ContainsKey(tileIndecies) == false ? new() : map[tileIndecies].Values.ToList();
		}
		public Vector2 GetTileIndecies(Vector2 position)
		{
			return (GetLocalPositionFromSelf(position) / CellSize).PointToGrid(CellSize) / CellSize;
		}
		public Vector2 GetTilePosition(Vector2 tileIndecies)
		{
			return GetPositionFromSelf(tileIndecies * CellSize);
		}

		internal override void OnDestroy()
		{
			base.OnDestroy();
			map.Clear();
		}
		internal Tilemap(string uid) : base(uid) { }

		#region Backend
		[JsonProperty]
		private readonly Dictionary<Vector2, SortedDictionary<float, Tile>> map = new();

		#endregion
	}
}
