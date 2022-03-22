using SFML.System;
using System;

namespace SMPL
{
	public static class Time
	{
		public struct Format
		{
			public struct Unit
			{
				public bool IsSkipped { get; set; }
				public string Display { get; set; }

				public Unit(bool isSkipped = false, string display = "")
				{
					Display = display;
					IsSkipped = isSkipped;
				}
			}

			public string Separator { get; set; }
			public Unit Hours { get; set; }
			public Unit Minutes { get; set; }
			public Unit Seconds { get; set; }
			public Unit Milliseconds { get; set; }

			public Format(Unit hours = new(), Unit minutes = new(), Unit seconds = new(),
				Unit milliseconds = new(), string separator = ":")
			{
				Hours = hours;
				Minutes = minutes;
				Seconds = seconds;
				Milliseconds = milliseconds;
				Separator = separator;
			}
		}
		public enum ChoiceConvertion
		{
			MillisecondsToSeconds, MillisecondsToMinutes,
			SecondsToMilliseconds, SecondsToMinutes, SecondsToHours,
			MinutesToMilliseconds, MinutesToSeconds, MinutesToHours, MinutesToDays,
			HoursToSeconds, HoursToMinutes, HoursToDays, HoursToWeeks,
			DaysToMinutes, DaysToHours, DaysToWeeks,
			WeeksToHours, WeeksToDays
		}

		private static readonly Clock time = new(), delta = new(), updateFPS = new();
		public static float Clock { get; private set; }
		public static float Delta { get; private set; }
		public static float FPS { get; private set; }
		public static float FPSAverage { get; private set; }
		public static float GameClock { get; private set; }
		public static uint TickCount { get; private set; }

		internal static void Update()
		{
			GameClock = (float)time.ElapsedTime.AsSeconds();
			Delta = (float)delta.ElapsedTime.AsSeconds();
			delta.Restart();
			Clock = (float)DateTime.Now.TimeOfDay.TotalSeconds;
			if ((float)updateFPS.ElapsedTime.AsSeconds() > 0.1f)
			{
				updateFPS.Restart();
				FPS = 1f / Delta;
				FPSAverage = TickCount / GameClock;
			}
			TickCount++;
		}
	}
}
