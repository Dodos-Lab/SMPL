namespace SMPL
{
	/// <summary>
	/// A class for continuously going over set data overtime.
	/// </summary>
	public class Animation
	{
		private float rawIndex;
		private const float LOWER_BOUND = -0.49f;

		/// <summary>
		/// This is calculated by rounding <see cref="RawIndex"/> and is used to retrieve the <see cref="CurrentValue"/> from <see cref="Values"/>.
		/// </summary>
		public int Index => (int)RawIndex.Round();
		/// <summary>
		/// A value used for time incrementation. See <see cref="Advance"/> for more info.
		/// </summary>
		public float RawIndex
		{
			get => rawIndex;
			set { rawIndex = value.Limit(LOWER_BOUND, Values.Length); }
		}
		/// <summary>
		/// The current progress (from 0 to 1).
		/// </summary>
		public float ProgressUnit => RawIndex.Map(LOWER_BOUND, Values.Length, 0, 1);

		/// <summary>
		/// All possible "frames" that the <see cref="Animation"/> goes over. They can be any <see cref="object"/>.
		/// </summary>
		public object[] Values { get; set; }
		/// <summary>
		/// The current "frame" retrieved by <see cref="Index"/> from <see cref="Values"/>.
		/// </summary>
		public object CurrentValue { get; private set; }
		/// <summary>
		/// The speed (in frames per second).
		/// </summary>
		public float FPS { get; set; }
		/// <summary>
		/// Whether the <see cref="Animation"/> starts over when the end is reached.
		/// </summary>
		public bool IsRepeating { get; set; }

		public Animation(float fps, bool isRepeating, params object[] values)
		{
			Values = values;
			rawIndex = 0;
			FPS = fps;
			IsRepeating = isRepeating;
			RawIndex = LOWER_BOUND;
		}

		/// <summary>
		/// This updates the <see cref="Animation"/> by incrementing the <see cref="RawIndex"/> according to
		/// <see cref="Time.Delta"/> and <see cref="FPS"/> (so that the <see cref="Animation"/> runs consistently on all systems).
		/// Wraps back to 0 if <see cref="IsRepeating"/>. Needs to be called continuously in order to work (in <see cref="Scene.OnUpdate"/> for example).
		/// </summary>
		public void Advance()
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
