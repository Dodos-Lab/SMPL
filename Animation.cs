namespace SMPL
{
	public struct Animation
	{
		private float rawIndex;

		public int Index => (int)RawIndex.Round();
		public float RawIndex { get => rawIndex; set { rawIndex = value.Limit(GetLowerBound(), Values.Length); } }
		public float Progress => RawIndex.Map(GetLowerBound(), Values.Length, 0, 1);

		public object[] Values { get; set; }
		public object CurrentValue { get; private set; }
		public float Speed { get; set; }
		public bool IsRepeating { get; set; }

		public Animation(float speed, bool isRepeating, params object[] values)
		{
			Values = values;
			rawIndex = 0;
			Speed = speed;
			IsRepeating = isRepeating;
			CurrentValue = default;
			RawIndex = GetLowerBound();
		}
		public void Update()
		{
			if (Values == null)
				return;

			RawIndex += Time.Delta * Speed;
			if (Index >= Values.Length)
				RawIndex = GetLowerBound();

			CurrentValue = Values[Index];
		}

		private static float GetLowerBound() => -0.49f;
	}
}
