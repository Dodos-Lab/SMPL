namespace SMPL.Prefabs
{
	/// <summary>
	/// A class for continuously iterating a set of data.
	/// </summary>
	public class Animation<T>
	{
		/// <summary>
		/// This is calculated by rounding <see cref="RawIndex"/> and is used to retrieve the <see cref="CurrentValue"/> from <see cref="Values"/>.
		/// </summary>
		public int Index
		{
			get
			{
				TryAdvance();
				return (int)RawIndex.Round();
			}
		}
		/// <summary>
		/// A value used for time incrementation. See <see cref="Advance"/> for more info.
		/// </summary>
		public float RawIndex
		{
			get
			{
				TryAdvance();
				return rawIndex;
			}
			set
			{
				TryAdvance();
				rawIndex = value.Limit(LOWER_BOUND, Values.Count);
			}
		}
		/// <summary>
		/// The current progress ranged [0-1].
		/// </summary>
		public float ProgressUnit
		{
			get
			{
				TryAdvance();
				return RawIndex.Map(LOWER_BOUND, Values.Count, 0, 1);
			}
		}

		/// <summary>
		/// All "frames" that the <see cref="Animation{T}"/> goes over.
		/// </summary>
		public List<T> Values
		{
			get
			{
				TryAdvance();
				return values;
			}
		}
		/// <summary>
		/// The current "frame" retrieved by <see cref="Index"/> from <see cref="Values"/>.
		/// </summary>
		public T CurrentValue
		{
			get
			{
				TryAdvance();
				return currValue;
			}
		}
		/// <summary>
		/// The speed (in frames per second).
		/// </summary>
		public float FPS
		{
			get
			{
				TryAdvance();
				return fps;
			}
			set
			{
				TryAdvance();
				fps = value;
			}
		}
		/// <summary>
		/// Whether the <see cref="Animation"/> starts over when the end is reached.
		/// </summary>
		public bool IsRepeating
		{
			get
			{
				TryAdvance();
				return isRepeating;
			}
			set
			{
				TryAdvance();
				isRepeating = value;
			}
		}

		public Animation(float fps, bool isRepeating, params T[] values)
		{
			for(int i = 0; i < values?.Length; i++)
				Values.Add(values[i]);

			rawIndex = 0;
			FPS = fps;
			IsRepeating = isRepeating;
			RawIndex = LOWER_BOUND;
		}

		#region Backend
		private readonly List<T> values = new();
		private bool isRepeating;
		private T currValue;
		private uint lastAdvanceFrame;
		private float rawIndex, fps;
		private const float LOWER_BOUND = -0.49f;

		private void TryAdvance()
		{
			if(lastAdvanceFrame == Time.FrameCount)
				return;

			lastAdvanceFrame = Time.FrameCount;

			if(Values == default)
			{
				currValue = default;
				return;
			}

			RawIndex += Time.Delta * FPS;
			if(Index >= Values.Count)
				RawIndex = IsRepeating ? LOWER_BOUND : Values.Count - 1;

			currValue = Values[Index];
		}
		#endregion
	}
}
