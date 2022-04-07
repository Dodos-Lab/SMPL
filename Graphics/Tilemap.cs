using SFML.Graphics;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace SMPL
{
   public class Tilemap : Visual
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

      private readonly Dictionary<Vector2, SortedDictionary<float, Tile>> map = new();

      public Vector2 CellSize { get; set; } = new(32);

      public override void Draw()
      {
         if (IsHidden)
            return;

         var vertsArr = new VertexArray(PrimitiveType.Quads);

         foreach (var kvp in map)
            foreach (var kvp2 in kvp.Value)
            {
               var worldPos = kvp.Key;
               var posA = worldPos * CellSize * Scale;
               var posB = posA + CellSize * Scale;
               var txCrdsA = kvp2.Value.IndeciesTexCoords * CellSize;
               var txCrdsB = txCrdsA + kvp2.Value.IndeciesSize * CellSize;
               var c = kvp2.Value.Color;

               vertsArr.Append(new(new(posA.X, posA.Y), c, new(txCrdsA.X, txCrdsA.Y)));
               vertsArr.Append(new(new(posB.X, posA.Y), c, new(txCrdsB.X, txCrdsA.Y)));
               vertsArr.Append(new(new(posB.X, posB.Y), c, new(txCrdsB.X, txCrdsB.Y)));
               vertsArr.Append(new(new(posA.X, posB.Y), c, new(txCrdsA.X, txCrdsB.Y)));
            }

         var transform = Transform.Identity;
         transform.Translate(new(0, 1));

         RenderTarget.Draw(vertsArr, new(BlendMode, transform, Texture, Shader));
         vertsArr.Dispose();
      }
      public void SetTile(Vector2 tilePositionIndecies, Tile tile)
      {
         if (map.ContainsKey(tilePositionIndecies) == false)
            map[tilePositionIndecies] = new();

         map[tilePositionIndecies][tile.Layer] = tile;
      }
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

      public List<Tile> GetTiles(Vector2 position)
      {
         var pos = position.PointToGrid(CellSize * Scale) / Scale / CellSize;
         return map.ContainsKey(pos) == false ? new() : map[pos].Values.ToList();
		}
		public Vector2 GetTilePosition(Vector2 tileIndecies)
      {
         return tileIndecies / CellSize / Scale;
      }
   }
}
