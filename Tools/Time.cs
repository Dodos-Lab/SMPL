namespace SMPL.Tools
{
	/// <summary>
	/// A class that tracks time and calculates time related values.
	/// </summary>
	public static class Time
	{
		/// <summary>
		/// The type of time convertion going from one time unit to another. This is used by <see cref="ToTime"/>.
		/// </summary>
		public enum Convertion
		{
			MillisecondsToSeconds, MillisecondsToMinutes,
			SecondsToMilliseconds, SecondsToMinutes, SecondsToHours,
			MinutesToMilliseconds, MinutesToSeconds, MinutesToHours, MinutesToDays,
			HoursToSeconds, HoursToMinutes, HoursToDays, HoursToWeeks,
			DaysToMinutes, DaysToHours, DaysToWeeks,
			WeeksToHours, WeeksToDays
		}

		public static string ToTimer(this float seconds)
		{
			var ts = TimeSpan.FromSeconds(seconds);
			return $"{ts:%h}h {ts:%m}m {ts:%s}s";
		}
		/// <summary>
		/// Converts a <paramref name="time"/> from one time unit to another (chosen by <paramref name="convertType"/>). Then returns the result.
		/// </summary>
		public static float ToTime(this float time, Convertion convertType)
		{
			return convertType switch
			{
				Convertion.MillisecondsToSeconds => time / 1000,
				Convertion.MillisecondsToMinutes => time / 1000 / 60,
				Convertion.SecondsToMilliseconds => time * 1000,
				Convertion.SecondsToMinutes => time / 60,
				Convertion.SecondsToHours => time / 3600,
				Convertion.MinutesToMilliseconds => time * 60000,
				Convertion.MinutesToSeconds => time * 60,
				Convertion.MinutesToHours => time / 60,
				Convertion.MinutesToDays => time / 1440,
				Convertion.HoursToSeconds => time * 3600,
				Convertion.HoursToMinutes => time * 60,
				Convertion.HoursToDays => time / 24,
				Convertion.HoursToWeeks => time / 168,
				Convertion.DaysToMinutes => time * 1440,
				Convertion.DaysToHours => time * 24,
				Convertion.DaysToWeeks => time / 7,
				Convertion.WeeksToHours => time * 168,
				Convertion.WeeksToDays => time * 7,
				_ => 0,
			};
		}

		/// <summary>
		/// The real time clock taken from <see cref="DateTime.Now"/> in seconds ranged
		/// [0 - 86399]<br></br>or in clock hours ranged [12 AM, 00:00, 24:00 - 11:59:59 AM, 23:59:59].
		/// </summary>
		public static float Clock { get; private set; }
		/// <summary>
		/// The time in seconds since the last frame/tick/update. This is useful for multiplying a step value against it in continuous calculations
		/// so that the step value is consistent on all systems.<br></br><br></br>
		/// - Example: An <see cref="ThingInstance"/> moving with the speed of 1 pixel per frame/tick/update in a game running at 60 FPS will be moving with 60
		/// pixels per second.<br></br> But on a game running at 120 FPS - it will be moving with 120 pixels per second or twice as fast.<br></br>
		/// This also  means that some users with low-end hardware will appear to play the game in slow motion
		/// (when the FPS drops bellow 40, 30, 20).<br></br>
		/// The step value of that <see cref="ThingInstance"/> (in this case the speed of '1 pixel per frame/tick/update') should be multiplied with
		/// <see cref="Delta"/> to prevent it from messing with the gameplay.<br></br><br></br>
		/// - Note: The continuous movement methods in <see cref="Extensions"/> are already accounting the delta time
		/// in their calculations with an argument determining whether they are FPS dependent.
		/// </summary>
		public static float Delta { get; private set; }
		/// <summary>
		/// The frames/ticks/updates per second.
		/// </summary>
		public static float FPS { get; private set; }
		/// <summary>
		/// The average FPS since the start of the game. See <see cref="FPS"/> for more info.
		/// </summary>
		public static float AverageFPS { get; private set; }
		/// <summary>
		/// The seconds that have passed since the start of the game.
		/// </summary>
		public static float GameClock { get; private set; }
		/// <summary>
		/// The amount of rendered frames since the start of the game.
		/// </summary>
		public static uint FrameCount { get; private set; }

		public static void Update()
		{
			GameClock = (float)time.ElapsedTime.AsSeconds();
			Delta = (float)delta.ElapsedTime.AsSeconds();
			delta.Restart();
			Clock = (float)DateTime.Now.TimeOfDay.TotalSeconds;
			if((float)updateFPS.ElapsedTime.AsSeconds() > 0.1f)
			{
				updateFPS.Restart();
				FPS = 1f / Delta;
				AverageFPS = FrameCount / GameClock;
			}
			FrameCount++;
		}
		#region Backend
		private static readonly Clock time = new(), delta = new(), updateFPS = new();
		#endregion
	}
}
