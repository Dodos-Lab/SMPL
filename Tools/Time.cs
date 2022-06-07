namespace SMPL.Tools
{
	/// <summary>
	/// A class that tracks time and calculates time related values.
	/// </summary>
	public static class Time
	{
		/// <summary>
		/// A set of values used by <see cref="SecondsToText(float, Format)"/> and <see cref="SecondsToText(int, Format)"/>
		/// that specify the way time is represented as a <see cref="string"/>.
		/// </summary>
		public struct Format
		{
			/// <summary>
			/// A set of values that represent a single unit of time (seconds, minutes, hours etc).
			/// </summary>
			public struct Unit
			{
				/// <summary>
				/// Whether this time <see cref="Unit"/> is skipped in the final <see cref="Format"/>.
				/// </summary>
				public bool IsSkipped { get; set; }
				/// <summary>
				/// The suffix that accompanies the <see cref="Unit"/>'s value in the final <see cref="Format"/> ('23s', '4 min', '10 hours' for example).
				/// </summary>
				public string Suffix { get; set; }
			}

			/// <summary>
			/// The <see cref="string"/> (usually ':') between <see cref="Unit"/> values ('0:22:10', '12h 20s' for example). For better
			/// default readability, this is assumed to be a space (' ') if null.
			/// </summary>
			public string Separator { get; set; }
			/// <summary>
			/// The settings for the hours unit.
			/// </summary>
			public Unit Hours { get; set; }
			/// <summary>
			/// The settings for the minutes unit.
			/// </summary>
			public Unit Minutes { get; set; }
			/// <summary>
			/// The settings for the seconds <see cref="Unit"/>.
			/// </summary>
			public Unit Seconds { get; set; }
			/// <summary>
			/// The settings for the milliseconds <see cref="Unit"/>.
			/// </summary>
			public Unit Milliseconds { get; set; }
		}
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

		/// <summary>
		/// Converts a <paramref name="number"/> from one time unit to another (chosen by <paramref name="convertType"/>). Then returns the result.
		/// </summary>
		public static float ToTime(this float number, Convertion convertType)
		{
			return convertType switch
			{
				Convertion.MillisecondsToSeconds => number / 1000,
				Convertion.MillisecondsToMinutes => number / 1000 / 60,
				Convertion.SecondsToMilliseconds => number * 1000,
				Convertion.SecondsToMinutes => number / 60,
				Convertion.SecondsToHours => number / 3600,
				Convertion.MinutesToMilliseconds => number * 60000,
				Convertion.MinutesToSeconds => number * 60,
				Convertion.MinutesToHours => number / 60,
				Convertion.MinutesToDays => number / 1440,
				Convertion.HoursToSeconds => number * 3600,
				Convertion.HoursToMinutes => number * 60,
				Convertion.HoursToDays => number / 24,
				Convertion.HoursToWeeks => number / 168,
				Convertion.DaysToMinutes => number * 1440,
				Convertion.DaysToHours => number * 24,
				Convertion.DaysToWeeks => number / 7,
				Convertion.WeeksToHours => number * 168,
				Convertion.WeeksToDays => number * 7,
				_ => 0,
			};
		}
		/// <summary>
		/// Converts <paramref name="seconds"/> into a <see cref="string"/> following a <paramref name="format"/>. Then returns the result.
		/// </summary>
		public static string SecondsToText(this float seconds, Format format = new())
		{
			if(float.IsNaN(seconds) || float.IsInfinity(seconds))
				return null;

			seconds = seconds.Sign(false);
			var ms = 0;
			if(seconds.ToString().Contains('.'))
			{
				var spl = seconds.ToString().Split('.');
				ms = int.Parse(spl[1]) * 100;
				seconds = seconds.Round(toward: Extensions.RoundWay.Down);
			}
			var sec = seconds % 60;
			var min = ToTime(seconds, Convertion.SecondsToMinutes) % 60;
			var hr = ToTime(seconds, Convertion.SecondsToHours);
			var msShow = !format.Milliseconds.IsSkipped;
			var secShow = !format.Seconds.IsSkipped;
			var minShow = !format.Minutes.IsSkipped;
			var hrShow = !format.Hours.IsSkipped;

			var sep = format.Separator ?? " ";
			var msStr = msShow ? $"{ms:D2}" : "";
			var secStr = secShow ? $"{(int)sec:D2}" : "";
			var minStr = minShow ? $"{(int)min:D2}" : "";
			var hrStr = hrShow ? $"{(int)hr:D2}" : "";
			var msF = msShow ? $"{format.Milliseconds.Suffix}" : "";
			var secF = secShow ? $"{format.Seconds.Suffix}" : "";
			var minF = minShow ? $"{format.Minutes.Suffix}" : "";
			var hrF = hrShow ? $"{format.Hours.Suffix}" : "";
			var secMsSep = msShow && (secShow || minShow || hrShow) ? $"{sep}" : "";
			var minSecSep = secShow && (minShow || hrShow) ? $"{sep}" : "";
			var hrMinSep = minShow && hrShow ? $"{sep}" : "";

			return $"{hrStr}{hrF}{hrMinSep}{minStr}{minF}{minSecSep}{secStr}{secF}{secMsSep}{msStr}{msF}";
		}
		/// <summary>
		/// Converts <paramref name="seconds"/> into a <see cref="string"/> following a <paramref name="format"/>. Then returns the result.
		/// </summary>
		public static string SecondsToText(this int seconds, Format format = new())
		{
			return SecondsToText((float)seconds, format);
		}

		/// <summary>
		/// The real time clock taken from <see cref="DateTime.Now"/> in seconds ranged
		/// [0 - 86399]<br></br>or in clock hours ranged [12 AM, 00:00, 24:00 - 11:59:59 AM, 23:59:59].
		/// </summary>
		public static float Clock { get; private set; }
		/// <summary>
		/// The time in seconds since the last frame/tick/update. This is useful for multiplying a step value against it in continuous calculations
		/// so that the step value is consistent on all systems.<br></br><br></br>
		/// - Example: An <see cref="Thing"/> moving with the speed of 1 pixel per frame/tick/update in a game running at 60 FPS will be moving with 60
		/// pixels per second.<br></br> But on a game running at 120 FPS - it will be moving with 120 pixels per second or twice as fast.<br></br>
		/// This also  means that some users with low-end hardware will appear to play the game in slow motion
		/// (when the FPS drops bellow 40, 30, 20).<br></br>
		/// The step value of that <see cref="Thing"/> (in this case the speed of '1 pixel per frame/tick/update') should be multiplied with
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

		#region Backend
		private static readonly Clock time = new(), delta = new(), updateFPS = new();

		internal static void Update()
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
		#endregion
	}
}
