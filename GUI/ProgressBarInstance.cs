namespace SMPL.GUI
{
	internal class ProgressBarInstance : SpriteInstance
	{
		[JsonIgnore]
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
		[JsonIgnore]
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
		[JsonIgnore]
		public float MaxLength
		{
			get => max;
			set { max = value; Update(); }
		}

		[JsonIgnore]
		public float ProgressUnit
		{
			get => Value.Map(RangeA, RangeB, 0, 1);
			set => Value = value.Map(0, 1, RangeA, RangeB);
		}
		[JsonIgnore]
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
		[JsonProperty]
		private float val, max, rangeA, rangeB;

		[JsonConstructor]
		internal ProgressBarInstance() { }
		internal ProgressBarInstance(string uid) : base(uid)
		{
			Size = new Vector2(MaxLength, 40);
			RangeB = 1;
			OriginUnit = new(0, 0.5f);
			MaxLength = 300;
			ProgressUnit = 0.5f;
		}

		private void Update()
		{
			if(IsDisabled)
				return;

			var value = Value.Map(RangeA, RangeB, 0, MaxLength);
			Size = new(value * Scale, Size.Y);
			TexCoordUnitB = new(ProgressUnit, TexCoordUnitB.Y);
		}
		#endregion
	}
}
