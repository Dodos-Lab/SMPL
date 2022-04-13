using SFML.Graphics;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace SMPL
{
   /// <summary>
   /// A square fragment configuration used by a <see cref="Tilemap"/> for building a textured grid from a single <see cref="Texture"/>.
   /// </summary>
   public struct Tile
   {
      /// <summary>
      /// The position of this <see cref="Tile"/> in the <see cref="Visual.Texture"/> as indecies according to the <see cref="IndeciesSize"/>
      /// and <see cref="Tilemap.CellSize"/>.<br></br><br></br>
      /// - Example: [this = 0, 0] + [<see cref="IndeciesSize"/> = 1, 1] + [<see cref="Tilemap.CellSize"/> = 32, 32] =
      /// the very top left cell in a 32x32 cell tileset.<br></br>
      /// - Example: [this = 3, 1] + [<see cref="IndeciesSize"/> = 1, 2] + [<see cref="Tilemap.CellSize"/> = 32, 32] =
      /// this tile would be the both [3, 1] and [3, 2] in the tileset but it would be resized to fit into a 32x32 square in the world.<br></br>
      /// </summary>
      public Vector2 IndeciesTexCoords { get; set; }
      /// <summary>
      /// The size of this <see cref="Tile"/> in the <see cref="Visual.Texture"/> as indecies according to <see cref="Tilemap.CellSize"/>.
      /// See <see cref="IndeciesTexCoords"/> for more info.
      /// </summary>
      public Vector2 IndeciesSize { get; set; }
      /// <summary>
      /// The tint color of this <see cref="Tile"/> upon being drawn.
      /// </summary>
      public Color Color { get; set; }
      /// <summary>
      /// The "height" of the <see cref="Tile"/> upon being drawn. Useful for having multiple tiles on top of each other.<br></br><br></br>
      /// - Example:<br></br>
      /// 0 Flat background color<br></br>
      /// 1 Grass tile<br></br>
      /// 2 Tree tile<br></br>
      /// 3 Occasional cloud tile
      /// </summary>
      public float Layer { get; set; }

      /// <summary>
      /// Create the tile with some <paramref name="texCoordsIndecies"/> and the resut of the data being optional.
      /// </summary>
      public Tile(Vector2 texCoordsIndecies, float layer = 0, float indexSizeX = 1, float indexSizeY = 1,
         byte r = 255, byte g = 255, byte b = 255, byte a = 255)
      {
         IndeciesTexCoords = texCoordsIndecies;
         IndeciesSize = new(indexSizeX, indexSizeY);
         Color = new(r, g, b, a);
         Layer = layer;
      }
   }

   /// <summary>
   /// Inherit chain: <see cref="Object"/> : <see cref="Visual"/> : <see cref="Tilemap"/><br></br><br></br>
   /// A grid with potentially infinite amount of square texture fragments configured by <see cref="Tile"/>s. Useful for building maps with tilesets
   /// (a texture containing each square cell fragment in a grid). Huge <see cref="Tilemap"/>s may need to be separated into multiple ones and
   /// managed/drawn in portions/chunks to increase performance.
   /// </summary>
   public class Tilemap : Visual
   {
      private readonly Dictionary<Vector2, SortedDictionary<float, Tile>> map = new();

      /// <summary>
      /// The size of the cell grid in the <see cref="Visual.Texture"/> and in the world. The world size can also be <see cref="Object.Scale"/>d.
      /// </summary>
      public Vector2 CellSize { get; set; } = new(32);

      /// <summary>
		/// Draws all of the set <see cref="Tile"/>s on the <see cref="Visual.Camera"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/> and <see cref="Tilemap"/> parameters.
		/// </summary>
      public override void Draw()
      {
         if (IsHidden)
            return;

         Camera ??= Scene.MainCamera;

         var vertsArr = new VertexArray(PrimitiveType.Quads);

         foreach (var kvp in map)
            foreach (var kvp2 in kvp.Value)
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

         Camera.Draw(vertsArr, new(BlendMode, Transform.Identity, Texture, Shader));
         vertsArr.Dispose();
      }
      /// <summary>
      /// Creates a <paramref name="tile"/> at some <paramref name="tilePositionIndecies"/>. Any <see cref="Tile"/> that is already there and
      /// has the same <see cref="Tile.Layer"/> as <paramref name="tile"/> will be replaced.
      /// </summary>
      public void SetTile(Vector2 tilePositionIndecies, Tile tile)
      {
         if (map.ContainsKey(tilePositionIndecies) == false)
            map[tilePositionIndecies] = new();

         map[tilePositionIndecies][tile.Layer] = tile;
      }
      /// <summary>
      /// Creates a square of the same repeated <paramref name="tile"/> between <paramref name="tilePositionIndeciesA"/> and
      /// <paramref name="tilePositionIndeciesB"/>. Any <see cref="Tile"/> that is already there and
      /// has the same <see cref="Tile.Layer"/> as <paramref name="tile"/> will be replaced.
      /// </summary>
      public void SetTileSquare(Vector2 tilePositionIndeciesA, Vector2 tilePositionIndeciesB, Tile tile)
      {
         if (tilePositionIndeciesA.X > tilePositionIndeciesB.X)
         {
            var swap = tilePositionIndeciesB.X;
            tilePositionIndeciesB.X = tilePositionIndeciesA.X;
            tilePositionIndeciesA.X = swap;
         }
         if (tilePositionIndeciesA.Y > tilePositionIndeciesB.Y)
         {
            var swap = tilePositionIndeciesB.Y;
            tilePositionIndeciesB.Y = tilePositionIndeciesA.Y;
            tilePositionIndeciesA.Y = swap;
         }

         for (float x = tilePositionIndeciesA.X; x < tilePositionIndeciesB.X; x++)
            for (float y = tilePositionIndeciesA.Y; y < tilePositionIndeciesB.Y; y++)
               SetTile(new(x, y), tile);
      }

      /// <summary>
      /// Returns a <see cref="List{T}"/> of all the <see cref="Tile"/>s at a particular world <paramref name="position"/> sorted by their
      /// <see cref="Tile.Layer"/>.
      /// </summary>
      public List<Tile> GetTilesFromPosition(Vector2 position)
      {
         var pos = GetTileIndecies(position);
         return map.ContainsKey(pos) == false ? new() : map[pos].Values.ToList();
		}
      /// <summary>
      /// Returns a <see cref="List{T}"/> of all the <see cref="Tile"/>s at the particular <paramref name="tileIndecies"/> sorted by their
      /// <see cref="Tile.Layer"/>.
      /// </summary>
      public List<Tile> GetTilesFromIndecies(Vector2 tileIndecies)
      {
         return map.ContainsKey(tileIndecies) == false ? new() : map[tileIndecies].Values.ToList();
      }
      /// <summary>
      /// Returns the tile indecies of a world <paramref name="position"/>.
      /// </summary>
      public Vector2 GetTileIndecies(Vector2 position)
      {
         return (GetLocalPositionFromSelf(position) / CellSize).PointToGrid(CellSize) / CellSize;
      }
      /// <summary>
      /// Returns the world position of some <paramref name="tileIndecies"/>.
      /// </summary>
		public Vector2 GetTilePosition(Vector2 tileIndecies)
      {
         return tileIndecies / CellSize / Scale;
      }
   }
}
