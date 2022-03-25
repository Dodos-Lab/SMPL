using SFML.Graphics;
using System.Collections.Generic;
using System.Numerics;
using SFML.System;

namespace SMPL
{
   public class VisualTilemap : Visual
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

      public Texture Texture { get; set; }
      public Vector2 CellSize { get; set; } = new(32);

      public override void Draw()
      {
         if (IsHidden)
            return;

         var vertsArr = new VertexArray(PrimitiveType.Triangles);

         foreach (var kvp in map)
            foreach (var kvp2 in kvp.Value)
            {
               var worldPos = kvp.Key;
               var posA = worldPos * CellSize * Scale;
               var posB = posA + CellSize * Scale;
               var txCrdsA = kvp2.Value.IndeciesTexCoords * CellSize;
               var txCrdsB = txCrdsA + kvp2.Value.IndeciesSize * CellSize;
               var c = kvp2.Value.Color;
               
               vertsArr.Append(new(new(0, 0), c, new(80, 96)));
               vertsArr.Append(new(new(96, 0), c, new(80 + 16, 96)));
               vertsArr.Append(new(new(0, 96), c, new(80, 96 + 16)));
               vertsArr.Append(new(new(96, 0), c, new(80 + 16, 96)));
               vertsArr.Append(new(new(96, 96), c, new(80 + 16, 96 + 16)));
               vertsArr.Append(new(new(0, 96), c, new(80, 96 + 16)));
            }

         RenderTarget.Draw(vertsArr, new(BlendMode, Transform.Identity, Texture, Shader));
      }
      public void SetTile(Vector2 tilePositionIndexes, Tile tile)
      {
         if (map.ContainsKey(tilePositionIndexes) == false)
            map[tilePositionIndexes] = new();

         map[tilePositionIndexes][tile.Layer] = tile;
      }
      public Vector2 GetTile(Vector2 position)
      {
         return position.ToGrid(CellSize * Scale) / Scale;
      }
   }
}
