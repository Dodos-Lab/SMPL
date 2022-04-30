using SMPL.Core;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;
using System.Numerics;

namespace SMPL.UI
{
   /// <summary>
   /// Inherit chain: <see cref="ProgressBar"/> : <see cref="Sprite"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
   /// Simply a <see cref="Sprite"/> that uses its <see cref="Sprite.Size"/>.X to indicate some value in a range.
   /// </summary>
   public class ProgressBar : Sprite
   {
      private float val, max, rangeA, rangeB;

      /// <summary>
      /// The first bound of the range of the <see cref="Value"/>.
      /// </summary>
      public float RangeA
      {
         get => rangeA;
         set { rangeA = value; Value = val; Update(); }
      }
      /// <summary>
      /// The first bound of the range of the <see cref="Value"/>.
      /// </summary>
      public float RangeB
      {
         get => rangeB;
         set { rangeB = value; Value = val; Update(); }
      }
      /// <summary>
      /// The maximum <see cref="Sprite.Size"/>.X in the world. In other words: the maximum size (width) of the <see cref="ProgressBar"/>.
      /// </summary>
      public float LengthMax
      {
         get => max;
         set { max = value; Update(); }
      }

      /// <summary>
      /// The current progress of the <see cref="ProgressBar"/> in range [0 - 1].
      /// </summary>
      public float ProgressUnit
      {
         get => Value.Map(RangeA, RangeB, 0, 1);
         set => Value = value.Map(0, 1, RangeA, RangeB);
      }
      /// <summary>
      /// The current value of the <see cref="ProgressBar"/> in range [<see cref="RangeA"/> - <see cref="RangeB"/>]
      /// </summary>
      public float Value
      {
         get => val;
         set { val = value.Limit(RangeA, RangeB); Update(); }
      }

      public ProgressBar()
      {
         Size = new Vector2(LengthMax, 40) * Scale;
         RangeB = 1;
         OriginUnit = new(0, 0.5f);
         LengthMax = 400;
         ProgressUnit = 0.5f;
      }
      private void Update()
      {
         Size = new(Value.Map(RangeA, RangeB, 0, LengthMax) * Scale, Size.Y);
         TexCoordsUnitB = new(ProgressUnit, TexCoordsUnitB.Y);
      }
   }
}
