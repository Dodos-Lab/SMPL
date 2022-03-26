namespace SMPL
{
	public class Animation
	{
		private float rawIndex;
		private const float LOWER_BOUND = -0.49f;

		public int Index => (int)RawIndex.Round();
		public float RawIndex
		{
			get => rawIndex;
			set { rawIndex = value.Limit(LOWER_BOUND, Values.Length); }
		}
		public float Progress => RawIndex.Map(LOWER_BOUND, Values.Length, 0, 1);

		public object[] Values { get; set; }
		public object CurrentValue { get; private set; }
		public float FPS { get; set; }
		public bool IsRepeating { get; set; }

		public Animation(float fps, bool isRepeating, params object[] values)
		{
			Values = values;
			rawIndex = 0;
			FPS = fps;
			IsRepeating = isRepeating;
			RawIndex = LOWER_BOUND;
		}
		public void Update()
		{
			if (Values == default)
			{
				CurrentValue = default;
				return;
			}

			RawIndex += Time.Delta * FPS;
			if (Index >= Values.Length)
				RawIndex = IsRepeating ? LOWER_BOUND : Values.Length - 1;

			CurrentValue = Values[Index];
		}
	}
}
