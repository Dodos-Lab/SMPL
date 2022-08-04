namespace SMPL.UI
{
	internal class ProgressBarInstance : SpriteInstance
	{
		public float RangeA
		{
			get => rangeA;
			set
			{
				rangeA = value;
				Value = val;
				Update();
			}
		}
		public float RangeB
		{
			get => rangeB;
			set
			{
				rangeB = value;
				Value = val;
				Update();
			}
		}
		public float MaxLength
		{
			get => max;
			set { max = value; Update(); }
		}

		public float ProgressUnit
		{
			get => Value.Map(RangeA, RangeB, 0, 1);
			set => Value = value.Map(0, 1, RangeA, RangeB);
		}
		public float Value
		{
			get => val;
			set
			{
				val = value.Limit(RangeA, RangeB);
				Update();
			}
		}

		#region Backend
		private float val, max, rangeA, rangeB;
		[JsonConstructor]
		internal ProgressBarInstance()
		{
			Init();
		}
		internal ProgressBarInstance(string uid) : base(uid)
		{
			Init();
		}

		private void Init()
		{
			Size = new Vector2(MaxLength, 40);
			RangeB = 1;
			OriginUnit = new(0, 0.5f);
			MaxLength = 400;
			ProgressUnit = 0.5f;
		}
		private void Update()
		{
			Size = new(Value.Map(RangeA, RangeB, 0, MaxLength) * Scale, Size.Y);
			TexCoordUnitB = new(ProgressUnit, TexCoordUnitB.Y);
		}
		#endregion
	}
}
