using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Text;

namespace SMPL
{
	/// <summary>
	/// Various methods that extend the primitive types and collections aiming to be a shortcut for longer calculations/systems.
	/// </summary>
	public static class Extensions
	{
		public enum Limits { ClosestBound, Overflow }
		public enum RoundWay { Closest, Up, Down }
		public enum RoundWhen5 { TowardEven, AwayFromZero, TowardZero, TowardNegativeInfinity, TowardPositiveInfinity }
		public enum SizeToSize
		{
			Bit_Byte, Bit_KB,
			Byte_Bit, Byte_KB, Byte_MB,
			KB_Bit, KB_Byte, KB_MB, KB_GB,
			MB_Byte, MB_KB, MB_GB, MB_TB,
			GB_KB, GB_MB, GB_TB,
			TB_MB, TB_GB
		}
		public enum Animations
		{
			BendWeak, // Sine
			Bend, // Cubic
			BendStrong, // Quint
			Circle, // Circ
			Elastic, // Elastic
			Swing, // Back
			Bounce // Bounce
		}
		public enum AnimationCurves { In, Out, InOut }

		private static readonly Dictionary<string, int> gateEntries = new();
		private static readonly Dictionary<string, bool> gates = new();

		/// <summary>
		/// Returns true only the first time <paramref name="condition"/> is true. This is reset whenever <paramref name="condition"/> becomes false.
		/// This process can be repeated <paramref name="max"/> amount of times, always returns false after that.
		/// Some <paramref name="uniqueness"/> can be included if the line at which this method is called is not unique enough
		/// (for example in a for loop or in cases where multiple instances can be checked in the same condition):<br></br>
		/// <code>
		/// if (unit1.Overlaps(unit2).Once(uniqueness: unit1.Name)))
		/// ...
		/// </code>
		/// </summary>
		public static bool Once(this bool condition, uint max = uint.MaxValue, string uniqueness = default)
		{
			var uniqueID = $"{Debug.GetFilePath(1)}-{Debug.GetLineNumber(1)}-{uniqueness}";
			if (gates.ContainsKey(uniqueID) == false && condition == false) return false;
			else if (gates.ContainsKey(uniqueID) == false && condition == true)
			{
				gates[uniqueID] = true;
				gateEntries[uniqueID] = 1;
				return true;
			}
			else
			{
				if (gates[uniqueID] == true && condition == true) return false;
				else if (gates[uniqueID] == false && condition == true)
				{
					gates[uniqueID] = true;
					gateEntries[uniqueID]++;
					return true;
				}
				else if (gateEntries[uniqueID] < max) gates[uniqueID] = false;
			}
			return false;
		}
		/// <summary>
		/// Switches the values of two variables.
		/// </summary>
		public static void Swap<T>(ref T A, ref T B)
      {
			var swap = A;
			A = B;
			B = swap;
      }

		/// <summary>
		/// Picks randomly a single <typeparamref name="T"/> value out of a <paramref name="list"/> and returns it.
		/// </summary>
		public static T Choose<T>(this List<T> list)
		{
			return list[Random(0, list.Count - 1)];
		}
		/// <summary>
		/// Randomly shuffles the contents of a <paramref name="list"/>.
		/// </summary>
		public static void Shuffle<T>(this List<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = new Random().Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
		/// <summary>
		/// Calculates the average <see cref="float"/> out of a <paramref name="list"/> of <see cref="float"/>s and returns it.
		/// </summary>
		public static float Average(this List<float> list)
		{
			var sum = 0f;
			for (int i = 0; i < list.Count; i++)
				sum += list[i];
			return sum / list.Count;
		}

		/// <summary>
		/// Returns whether <paramref name="text"/> can be cast to a <see cref="float"/>.
		/// </summary>
		public static bool IsNumber(this string text)
		{
			return float.IsNaN(ToNumber(text));
		}
		/// <summary>
		/// Returns whether <paramref name="text"/> contains only letters.
		/// </summary>
		public static bool IsLetters(this string text)
		{
			for (int i = 0; i < text.Length; i++)
			{
				var isLetter = (text[i] >= 'A' && text[i] <= 'Z') || (text[i] >= 'a' && text[i] <= 'z');
				if (isLetter == false)
					return false;
			}
			return true;
		}
		/// <summary>
		/// Puts <paramref name="text"/> to the right with a set amount of <paramref name="spaces"/>
		/// if they are more than the <paramref name="text"/>'s length.<br></br>
		/// </summary>
		public static string Align(this string text, int spaces)
		{
			return string.Format("{0," + spaces + "}", text);
		}
		/// <summary>
		/// Adds <paramref name="text"/> to itself a certain amount of <paramref name="times"/> and returns it.
		/// </summary>
		public static string Repeat(this string text, uint times)
		{
			var result = "";
			for (int i = 0; i < times; i++)
				result = $"{result}{text}";
			return result;
		}
		/// <summary>
		/// Encrypts and compresses a <paramref name="text"/> and returns the result.
		/// The <paramref name="text"/> can be retrieved back with <see cref="Decompress(string)"/>
		/// </summary>
		public static string Compress(this string text)
		{
			var buffer = Encoding.UTF8.GetBytes(text);
			var memoryStream = new MemoryStream();
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				gZipStream.Write(buffer, 0, buffer.Length);

			memoryStream.Position = 0;

			var compressedData = new byte[memoryStream.Length];
			memoryStream.Read(compressedData, 0, compressedData.Length);

			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}
		/// <summary>
		/// Decrypts and decompresses a <paramref name="compressedText"/> and returns it. See <see cref="Compress(string)"/>.
		/// </summary>
		public static string Decompress(this string compressedText)
		{
			var gZipBuffer = Convert.FromBase64String(compressedText);
			using var memoryStream = new MemoryStream();
			var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
			memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

			var buffer = new byte[dataLength];

			memoryStream.Position = 0;
			using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				gZipStream.Read(buffer, 0, buffer.Length);

			return Encoding.UTF8.GetString(buffer);
		}
		/// <summary>
		/// Tries to convert <paramref name="text"/> to a <see cref="float"/> and returns the result (<see cref="float.NaN"/> if unsuccessful).
		/// This also takes into account the system's default decimal symbol.
		/// </summary>
		public static float ToNumber(this string text)
		{
			var result = 0.0f;
			text = text.Replace(',', '.');
			var parsed = float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

			return parsed ? result : float.NaN;
		}

		/// <summary>
		/// Wraps a <paramref name="number"/> around the range 0-360 and returns it.
		/// </summary>
		public static float AngleTo360(this float number)
		{
			return ((number % 360) + 360) % 360;
		}
		/// <summary>
		/// Converts a unit <paramref name="progress"/> [0 - 1] to a value calculated according to
		/// <paramref name="animationType"/> and <paramref name="animationCurve"/> then returns it.
		/// </summary>
		public static float AnimateUnit(this float progress, Animations animationType, AnimationCurves animationCurve)
		{
			var result = 0f;
			var x = progress;
			switch (animationType)
			{
				case Animations.BendWeak:
					{
						result = animationCurve == AnimationCurves.In ? 1 - MathF.Cos(x * MathF.PI / 2) :
							animationCurve == AnimationCurves.Out ? 1 - MathF.Sin(x * MathF.PI / 2) :
							-(MathF.Cos(MathF.PI * x) - 1) / 2;
						break;
					}
				case Animations.Bend:
					{
						result = animationCurve == AnimationCurves.In ? x * x * x :
							animationCurve == AnimationCurves.Out ? 1 - MathF.Pow(1 - x, 3) :
							(x < 0.5 ? 4 * x * x * x : 1 - MathF.Pow(-2 * x + 2, 3) / 2);
						break;
					}
				case Animations.BendStrong:
					{
						result = animationCurve == AnimationCurves.In ? x * x * x * x :
							animationCurve == AnimationCurves.Out ? 1 - MathF.Pow(1 - x, 5) :
							(x < 0.5 ? 16 * x * x * x * x * x : 1 - MathF.Pow(-2 * x + 2, 5) / 2);
						break;
					}
				case Animations.Circle:
					{
						result = animationCurve == AnimationCurves.In ? 1 - MathF.Sqrt(1 - MathF.Pow(x, 2)) :
							animationCurve == AnimationCurves.Out ? MathF.Sqrt(1 - MathF.Pow(x - 1, 2)) :
							(x < 0.5 ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2 : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2);
						break;
					}
				case Animations.Elastic:
					{
						result = animationCurve == AnimationCurves.In ?
							(x == 0 ? 0 : x == 1 ? 1 : -MathF.Pow(2, 10 * x - 10) * MathF.Sin((x * 10 - 10.75f) * ((2 * MathF.PI) / 3))) :
							animationCurve == AnimationCurves.Out ?
							(x == 0 ? 0 : x == 1 ? 1 : MathF.Pow(2, -10 * x) * MathF.Sin((x * 10 - 0.75f) * (2 * MathF.PI) / 3) + 1) :
							(x == 0 ? 0 : x == 1 ? 1 : x < 0.5f ? -(MathF.Pow(2, 20 * x - 10) * MathF.Sin((20f * x - 11.125f) *
							(2 * MathF.PI) / 4.5f)) / 2 :
							(MathF.Pow(2, -20 * x + 10) * MathF.Sin((20 * x - 11.125f) * (2 * MathF.PI) / 4.5f)) / 2 + 1);
						break;
					}
				case Animations.Swing:
					{
						result = animationCurve == AnimationCurves.In ? 2.70158f * x * x * x - 1.70158f * x * x :
							animationCurve == AnimationCurves.Out ? 1 + 2.70158f * MathF.Pow(x - 1, 3) + 1.70158f * MathF.Pow(x - 1, 2) :
							(x < 0.5 ? (MathF.Pow(2 * x, 2) * ((2.59491f + 1) * 2 * x - 2.59491f)) / 2 :
							(MathF.Pow(2 * x - 2, 2) * ((2.59491f + 1) * (x * 2 - 2) + 2.59491f) + 2) / 2);
						break;
					}
				case Animations.Bounce:
					{
						result = animationCurve == AnimationCurves.In ? 1 - easeOutBounce(1 - x) :
							animationCurve == AnimationCurves.Out ? easeOutBounce(x) :
							(x < 0.5f ? (1 - easeOutBounce(1 - 2 * x)) / 2 : (1 + easeOutBounce(2 * x - 1)) / 2);
						break;
					}
			}
			return result * 100;

			float easeOutBounce(float x)
			{
				return x < 1 / 2.75f ? 7.5625f * x * x : x < 2 / 2.75f ? 7.5625f * (x -= 1.5f / 2.75f) * x + 0.75f :
					x < 2.5f / 2.75f ? 7.5625f * (x -= 2.25f / 2.75f) * x + 0.9375f : 7.5625f * (x -= 2.625f / 2.75f) * x + 0.984375f;
			}
		}
		/// <summary>
		/// Restricts a <paramref name="number"/> in the inclusive range [<paramref name="rangeA"/> - <paramref name="rangeB"/>] with a
		/// <paramref name="limitation"/> and returns it. Also known as Clamping.
		/// </summary>
		public static float Limit(this float number, float rangeA, float rangeB, Limits limitation = Limits.ClosestBound)
		{
			if (rangeA > rangeB)
				Swap(ref rangeA, ref rangeB);

			if (limitation == Limits.ClosestBound)
			{
				if (number < rangeA)
					return rangeA;
				else if (number > rangeB)
					return rangeB;
				return number;
			}
			else
			{
				rangeB += 1;
				var a = number;
				a = Map(a);
				while (a < rangeA) a = Map(a);
				return a;
				float Map(float b)
				{
					b = ((b % rangeB) + rangeB) % rangeB;
					if (b < rangeA) b = rangeB - (rangeA - b);
					return b;
				}
			}
		}
		/// <summary>
		/// Ensures a <paramref name="number"/> is <paramref name="signed"/> and returns the result.
		/// </summary>
		public static float Sign(this float number, bool signed)
		{
			return signed ? -MathF.Abs(number) : MathF.Abs(number);
		}
		/// <summary>
		/// Calculates <paramref name="number"/>'s precision (amount of digits after the decimal point) and returns it.
		/// </summary>
		public static int Precision(this float number)
		{
			var cultDecPoint = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			var split = number.ToString().Split(cultDecPoint);
			return split.Length > 1 ? split[1].Length : 0;
		}
		/// <summary>
		/// Rounds a <paramref name="number"/> <paramref name="toward"/> a chosen way and <paramref name="precision"/> then returns the result.
		/// May take into account a certain <paramref name="priority"/>.
		/// </summary>
		public static float Round(this float number, float precision = 0, RoundWay toward = RoundWay.Closest,
			RoundWhen5 priority = RoundWhen5.AwayFromZero)
		{
			precision = (int)precision.Limit(0, 5);

			if (toward == RoundWay.Down || toward == RoundWay.Up)
			{
				var numStr = number.ToString();
				var prec = Precision(number);
				if (prec > 0 && prec > precision)
				{
					var digit = toward == RoundWay.Down ? "1" : "9";
					numStr = numStr.Remove(numStr.Length - 1);
					numStr = $"{numStr}{digit}";
					number = float.Parse(numStr);
				}
			}

			return MathF.Round(number, (int)precision, (MidpointRounding)priority);
		}
		/// <summary>
		/// Converts a <paramref name="number"/> from <paramref name="sizeToSize"/> and returns it.
		/// </summary>
		public static float ToDataSize(this float number, SizeToSize sizeToSize)
		{
			return sizeToSize switch
			{
				SizeToSize.Bit_Byte => number / 8,
				SizeToSize.Bit_KB => number / 8000,
				SizeToSize.Byte_Bit => number * 8,
				SizeToSize.Byte_KB => number / 1024,
				SizeToSize.Byte_MB => number / 1_048_576,
				SizeToSize.KB_Bit => number * 8000,
				SizeToSize.KB_Byte => number * 1024,
				SizeToSize.KB_MB => number / 1024,
				SizeToSize.KB_GB => number / 1_048_576,
				SizeToSize.MB_Byte => number * 1_048_576,
				SizeToSize.MB_KB => number * 1024,
				SizeToSize.MB_GB => number / 1024,
				SizeToSize.MB_TB => number / 1_048_576,
				SizeToSize.GB_KB => number * 1_048_576,
				SizeToSize.GB_MB => number * 1024,
				SizeToSize.GB_TB => number / 1024,
				SizeToSize.TB_MB => number * 1_048_576,
				SizeToSize.TB_GB => number * 1024,
				_ => default,
			};
		}
		/// <summary>
		/// Returns whether <paramref name="number"/> is in range [<paramref name="rangeA"/> - <paramref name="rangeB"/>].
		/// The ranges may be <paramref name="inclusiveLower"/> or <paramref name="inclusiveUpper"/>.
		/// </summary>
		public static bool IsBetween(this float number, float rangeA, float rangeB, bool inclusiveLower = false, bool inclusiveUpper = false)
		{
			if (rangeA > rangeB)
				Swap(ref rangeA, ref rangeB);
			var l = inclusiveLower ? rangeA <= number : rangeA < number;
			var u = inclusiveUpper ? rangeB >= number : rangeB > number;
			return l && u;
		}
		/// <summary>
		/// Moves a <paramref name="number"/> in the direction of <paramref name="speed"/>. May be <paramref name="fpsDependent"/>
		/// (see <see cref="Time.Delta"/> for info). The result is then returned.
		/// </summary>
		public static float Move(this float number, float speed, bool fpsDependent = true)
		{
			if (fpsDependent)
				speed *= Time.Delta;
			return number + speed;
		}
		/// <summary>
		/// Moves a <paramref name="number"/> toward a <paramref name="targetNumber"/> with <paramref name="speed"/>. May be
		/// <paramref name="fpsDependent"/> (see <see cref="Time.Delta"/> for info).
		/// The calculation ensures not to pass the <paramref name="targetNumber"/>. The result is then returned.
		/// </summary>
		public static float MoveToward(this float number, float targetNumber, float speed, bool fpsDependent = true)
		{
			var goingPos = number < targetNumber;
			var result = Move(number, goingPos ? Sign(speed, false) : Sign(speed, true), fpsDependent);

			if (goingPos && result > targetNumber)
				return targetNumber;
			else if (goingPos == false && result < targetNumber)
				return targetNumber;
			return result;
		}
		/// Maps a <paramref name="number"/> from [<paramref name="A1"/> - <paramref name="B1"/>] to
		/// [<paramref name="B1"/> - <paramref name="B2"/>] and returns it. Similar to Lerping (linear interpolation).<br></br>
		/// Example: 50 mapped from [0 - 100] and [0 - 1] results to 0.5<br></br>
		/// Example: 25 mapped from [30 - 20] and [1 - 5] results to 3
		/// </summary>
		public static float Map(this float number, float A1, float A2, float B1, float B2)
		{
			return (number - A1) / (A2 - A1) * (B2 - B1) + B1;
		}
		/// <summary>
		/// Rotates a 360 degrees <paramref name="angle"/> toward a <paramref name="targetAngle"/> with <paramref name="speed"/>
		/// taking the closest direction. May be <paramref name="fpsDependent"/> (see <see cref="Time.Delta"/> for info).
		/// The calculation ensures not to pass the <paramref name="targetAngle"/>. The result is then returned.
		/// </summary>
		public static float MoveAngleTowardAngle(this float angle, float targetAngle, float speed, bool fpsDependent = true)
		{
			angle = AngleTo360(angle);
			targetAngle = AngleTo360(targetAngle);
			speed = Math.Abs(speed);
			var difference = angle - targetAngle;

			// stops the rotation with an else when close enough
			// prevents the rotation from staying behind after the stop
			var checkedSpeed = speed;
			if (fpsDependent) checkedSpeed *= Time.Delta;
			if (Math.Abs(difference) < checkedSpeed) angle = targetAngle;
			else if (difference >= 0 && difference < 180) angle = Move(angle, -speed, fpsDependent);
			else if (difference >= -180 && difference < 0) angle = Move(angle, speed, fpsDependent);
			else if (difference >= -360 && difference < -180) angle = Move(angle, -speed, fpsDependent);
			else if (difference >= 180 && difference < 360) angle = Move(angle, speed, fpsDependent);

			// detects speed greater than possible
			// prevents jiggle when passing 0-360 & 360-0 | simple to fix yet took me half a day
			if (Math.Abs(difference) > 360 - checkedSpeed) angle = targetAngle;

			return angle;
		}
		/// <summary>
		/// Generates a random <see cref="float"/> number in the inclusive range [<paramref name="rangeA"/> - <paramref name="rangeB"/>] with
		/// <paramref name="precision"/> and an optional <paramref name="seed"/>. Then returns the result.
		/// </summary>
		public static float Random(this float rangeA, float rangeB, float precision = 0, float seed = float.NaN)
		{
			if (rangeA > rangeB)
				Swap(ref rangeA, ref rangeB);

			precision = (int)precision.Limit(0, 5);
			precision = MathF.Pow(10, precision);

			rangeA *= precision;
			rangeB *= precision;

			var s = new Random(float.IsNaN(seed) ? Guid.NewGuid().GetHashCode() : (int)seed);
			var randInt = s.Next((int)rangeA, (int)rangeB + 1).Limit((int)rangeA, (int)rangeB);

			return randInt / (precision);
		}
		/// <summary>
		/// Returns true only <paramref name="percent"/>% / returns false (100 - <paramref name="percent"/>)% of the times.
		/// </summary>
		public static bool HasChance(this float percent)
		{
			percent = percent.Limit(0, 100);
			var n = Random(1f, 100f); // should not roll 0 so it doesn't return true with 0% (outside of roll)
			return n <= percent;
		}
		/// <summary>
		/// Converts a <paramref name="number"/> from one time unit to another (chosen by <paramref name="convertType"/>). Then returns the result.
		/// </summary>
		public static float ToTime(this float number, Time.ChoiceConvertion convertType)
		{
			return convertType switch
			{
				Time.ChoiceConvertion.MillisecondsToSeconds => number / 1000,
				Time.ChoiceConvertion.MillisecondsToMinutes => number / 1000 / 60,
				Time.ChoiceConvertion.SecondsToMilliseconds => number * 1000,
				Time.ChoiceConvertion.SecondsToMinutes => number / 60,
				Time.ChoiceConvertion.SecondsToHours => number / 3600,
				Time.ChoiceConvertion.MinutesToMilliseconds => number * 60000,
				Time.ChoiceConvertion.MinutesToSeconds => number * 60,
				Time.ChoiceConvertion.MinutesToHours => number / 60,
				Time.ChoiceConvertion.MinutesToDays => number / 1440,
				Time.ChoiceConvertion.HoursToSeconds => number * 3600,
				Time.ChoiceConvertion.HoursToMinutes => number * 60,
				Time.ChoiceConvertion.HoursToDays => number / 24,
				Time.ChoiceConvertion.HoursToWeeks => number / 168,
				Time.ChoiceConvertion.DaysToMinutes => number * 1440,
				Time.ChoiceConvertion.DaysToHours => number * 24,
				Time.ChoiceConvertion.DaysToWeeks => number / 7,
				Time.ChoiceConvertion.WeeksToHours => number * 168,
				Time.ChoiceConvertion.WeeksToDays => number * 7,
				_ => 0,
			};
		}
		/// <summary>
		/// Converts <paramref name="seconds"/> into a <see cref="string"/> following a <paramref name="format"/>. Then returns the result.
		/// </summary>
		public static string SecondsToTimeText(this float seconds, Time.Format format = new())
		{
			seconds = seconds.Sign(false);
			var secondsStr = $"{seconds:F0}";
			var ms = 0;
			if (secondsStr.Contains('.'))
			{
				var spl = secondsStr.Split('.');
				ms = int.Parse(spl[1]) * 100;
				seconds = seconds.Round(toward: RoundWay.Down);
			}
			var sec = seconds % 60;
			var min = ToTime(seconds, Time.ChoiceConvertion.SecondsToMinutes) % 60;
			var hr = ToTime(seconds, Time.ChoiceConvertion.SecondsToHours);
			var msShow = !format.Milliseconds.IsSkipped;
			var secShow = !format.Seconds.IsSkipped;
			var minShow = !format.Minutes.IsSkipped;
			var hrShow = !format.Hours.IsSkipped;

			var sep = format.Separator == null || format.Separator == "" ? ":" : format.Separator;
			var msStr = msShow ? $"{ms:D2}" : "";
			var secStr = secShow ? $"{(int)sec:D2}" : "";
			var minStr = minShow ? $"{(int)min:D2}" : "";
			var hrStr = hrShow ? $"{(int)hr:D2}" : "";
			var msF = msShow ? $"{format.Milliseconds.Display}" : "";
			var secF = secShow ? $"{format.Seconds.Display}" : "";
			var minF = minShow ? $"{format.Minutes.Display}" : "";
			var hrF = hrShow ? $"{format.Hours.Display}" : "";
			var secMsSep = msShow && (secShow || minShow || hrShow) ? $"{sep}" : "";
			var minSecSep = secShow && (minShow || hrShow) ? $"{sep}" : "";
			var hrMinSep = minShow && hrShow ? $"{sep}" : "";

			return $"{hrStr}{hrF}{hrMinSep}{minStr}{minF}{minSecSep}{secStr}{secF}{secMsSep}{msStr}{msF}";
		}
		/// <summary>
		/// Converts a 360 degrees <paramref name="angle"/> into a normalized direction <see cref="Vector2"/> then returns the result.
		/// </summary>
		public static Vector2 AngleToDirection(this float angle)
		{
			//Angle to Radians : (Math.PI / 180) * angle
			//Radians to Vector2 : Vector2.x = cos(angle) ; Vector2.y = sin(angle)

			var rad = MathF.PI / 180 * angle;
			var dir = new Vector2(MathF.Cos(rad), MathF.Sin(rad));

			return new(dir.X, dir.Y);
		}
		/// <summary>
		/// Converts <paramref name="radians"/> to a 360 degrees angle and returns the result.
		/// </summary>
		public static float RadiansToDegrees(this float radians)
		{
			return radians * (180f / MathF.PI);
		}
		/// <summary>
		/// Converts a 360 <paramref name="degrees"/> angle into radians and returns the result.
		/// </summary>
		public static float DegreesToRadians(this float degrees)
		{
			return (MathF.PI / 180f) * degrees;
		}

		/// <summary>
		/// Calculates a reflected normalized <paramref name="direction"/> <see cref="Vector2"/> as if it was to bounce off of a
		/// <paramref name="surfaceNormal"/> (the direction the surface is facing) and returns it.
		/// </summary>
		public static Vector2 ReflectDirection(this Vector2 direction, Vector2 surfaceNormal)
      {
			return Vector2.Reflect(direction, surfaceNormal);
      }
		/// <summary>
		/// Normalizes a <paramref name="direction"/> <see cref="Vector2"/>. Or in other words: sets the length (magnitude) of the
		/// <paramref name="direction"/> <see cref="Vector2"/> to 1. Then the result is returned.
		/// </summary>
		public static Vector2 NormalizeDirection(this Vector2 direction)
      {
			return Vector2.Normalize(direction);
      }
		/// <summary>
		/// Calculates the distance between a <paramref name="point"/> and a <paramref name="targetPoint"/> then returns it.
		/// </summary>
		public static float DistanceBetweenPoints(this Vector2 point, Vector2 targetPoint)
      {
			return Vector2.Distance(point, targetPoint);
      }
		/// <summary>
		/// Returns whether this <paramref name="vector"/> is invalid.
		/// </summary>
		public static bool IsNaN(this Vector2 vector)
		{
			return float.IsNaN(vector.X) || float.IsNaN(vector.Y);
		}
		/// <summary>
		/// Returns an invalid <see cref="Vector2"/>.
		/// </summary>
		public static Vector2 NaN(this Vector2 vector)
		{
			return new(float.NaN, float.NaN);
		}
		/// <summary>
		/// Converts a directional unit <see cref="Vector2"/> into a 360 degrees angle and returns the result.
		/// </summary>
		public static float DirectionToAngle(this Vector2 direction)
		{
			//Vector2 to Radians: atan2(Vector2.y, Vector2.x)
			//Radians to Angle: radians * (180 / Math.PI)

			var rad = MathF.Atan2(direction.Y, direction.X);
			var result = rad * (180f / MathF.PI);
			return result;
		}
		/// <summary>
		/// Calculates the 360 degrees angle between two <see cref="Vector2"/> points and returns it.
		/// </summary>
		public static float AngleBetweenPoints(this Vector2 point, Vector2 targetPoint)
		{
			return DirectionToAngle(targetPoint - point);
		}
		/// <summary>
		/// Snaps a <paramref name="point"/> to the closest grid cell according to <paramref name="gridSize"/> and returns the result.
		/// </summary>
		public static Vector2 PointToGrid(this Vector2 point, Vector2 gridSize)
		{
			// this prevents -0 cells
			point.X -= point.X < 0 ? gridSize.X : 0;
			point.Y -= point.Y < 0 ? gridSize.Y : 0;

			point.X -= point.X % gridSize.X;
			point.Y -= point.Y % gridSize.Y;
			return point;
		}
		/// <summary>
		/// Calculates the direction between <paramref name="point"/> and <paramref name="targetPoint"/>. The result may be
		/// <paramref name="normalized"/> (see <see cref="NormalizeDirection"/> for info). Then it is returned.
		/// </summary>
		public static Vector2 DirectionBetweenPoints(this Vector2 point, Vector2 targetPoint, bool normalized = true)
		{
			return normalized ? Vector2.Normalize(targetPoint - point) : targetPoint - point;
		}
		/// <summary>
		/// Moves a <paramref name="point"/> in <paramref name="direction"/> with <paramref name="speed"/>. May be <paramref name="fpsDependent"/>
		/// (see <see cref="Time.Delta"/> for info). The result is then returned.
		/// </summary>
		public static Vector2 MovePointInDirection(this Vector2 point, Vector2 direction, float speed, bool fpsDependent = true)
		{
			point.X += direction.X * speed * (fpsDependent ? Time.Delta : 1);
			point.Y += direction.Y * speed * (fpsDependent ? Time.Delta : 1);
			return new(point.X, point.Y);
		}
		/// <summary>
		/// Moves a <paramref name="point"/> at a 360 degrees <paramref name="angle"/> with <paramref name="speed"/>. May be
		/// <paramref name="fpsDependent"/> (see <see cref="Time.Delta"/> for info). The result is then returned.
		/// </summary>
		public static Vector2 MovePointAtAngle(this Vector2 point, float angle, float speed, bool fpsDependent = true)
		{
			var result = MovePointInDirection(point, Vector2.Normalize(angle.AngleToDirection()), speed, fpsDependent);
			return result;
		}
		/// <summary>
		/// Moves a <paramref name="point"/> toward <paramref name="targetPoint"/> with <paramref name="speed"/>. May be
		/// <paramref name="fpsDependent"/> (see <see cref="Time.Delta"/> for info). The calculation ensures not to pass the
		/// <paramref name="targetPoint"/>. The result is then returned.
		/// </summary>
		public static Vector2 MovePointTowardPoint(this Vector2 point, Vector2 targetPoint, float speed, bool fpsDependent = true)
		{
			var result = point.MovePointAtAngle(point.AngleBetweenPoints(targetPoint), speed, fpsDependent);

			speed *= fpsDependent ? Time.Delta : 1;
         return Vector2.Distance(result, targetPoint) < speed * 2 ? targetPoint : result;
      }
		/// <summary>
		/// Calculates the <see cref="Vector2"/> point that is a certain <paramref name="percent"/> between <paramref name="point"/> and
		/// <paramref name="targetPoint"/> then returns the result. Also known as Lerping (linear interpolation).
		/// </summary>
      public static Vector2 PercentTowardTarget(this Vector2 point, Vector2 targetPoint, Vector2 percent)
		{
			point.X = percent.X.Map(0, 100, point.X, targetPoint.X);
			point.Y = percent.Y.Map(0, 100, point.Y, targetPoint.Y);
			return point;
		}
		/// <summary>
		/// Draws <paramref name="point"/> to a <paramref name="renderTarget"/> with <paramref name="color"/> having some
		/// <paramref name="size"/>. The <paramref name="renderTarget"/> is assumed to be the <see cref="Game.Window"/> if no
		/// <paramref name="renderTarget"/> is passed. The default <paramref name="color"/> is assumed to be white if no
		/// <paramref name="color"/> is passed.
		/// </summary>
		public static void DrawPoint(this Vector2 point, RenderTarget renderTarget = default, Color color = default, float size = 4)
		{
			renderTarget ??= Game.Window;
			color = color == default ? Color.White : color;

			size /= 2;
			var tl = point.MovePointAtAngle(270, size, false).MovePointAtAngle(180, size, false);
			var tr = point.MovePointAtAngle(270, size, false).MovePointAtAngle(0, size, false);
			var br = point.MovePointAtAngle(90, size, false).MovePointAtAngle(0, size, false);
			var bl = point.MovePointAtAngle(90, size, false).MovePointAtAngle(180, size, false);

			var vert = new Vertex[]
			{
				new(new(tl.X, tl.Y), color),
				new(new(tr.X, tr.Y), color),
				new(new(br.X, br.Y), color),
				new(new(bl.X, bl.Y), color),
			};
			renderTarget.Draw(vert, PrimitiveType.Quads);
		}
		/// <summary>
		/// Converts a <see cref="Vector2f"/> into a <see cref="Vector2"/> and returns the result.
		/// </summary>
		public static Vector2 ToSystem(this Vector2f vector)
		{
			return new(vector.X, vector.Y);
		}
		/// <summary>
		/// Converts a <see cref="Vector2"/> into a <see cref="Vector2f"/> and returns the result.
		/// </summary>
		public static Vector2f ToSFML(this Vector2 vector)
		{
			return new(vector.X, vector.Y);
		}
		/// <summary>
		/// Picks the outside points in a collection of <paramref name="points"/> which when connected will form an outline of the initial collection.
		/// The picked points are then returned in a new list.
		/// </summary>
		public static List<Vector2> OutlinePoints(this ICollection<Vector2> points)
		{
			var result = new List<Vector2>();
			foreach (var p in points)
			{
				if (result.Count == 0)
					result.Add(p);
				else
				{
					if (result[0].X > p.X)
						result[0] = p;
					else if (result[0].X == p.X)
						if (result[0].Y > p.Y)
							result[0] = p;
				}
			}
			var counter = 0;
			while (counter < result.Count)
			{
				var q = Next(points, result[counter]);
				result.Add(q);
				if (q == result[0] || result.Count > points.Count)
					break;
				counter++;

				Vector2 Next(ICollection<Vector2> points, Vector2 p)
				{
					Vector2 q = p;
					int t;
					foreach (Vector2 r in points)
					{
						t = ((q.X - p.X) * (r.Y - p.Y) - (r.X - p.X) * (q.Y - p.Y)).CompareTo(0);
						if (t == -1 || t == 0 && Vector2.Distance(p, r) > Vector2.Distance(p, q))
							q = r;
					}
					return q;
				}
			}
			result.Add(result[0]);
			return result;
		}

		/// <summary>
		/// Generates a random <see cref="int"/> number in the inclusive range [<paramref name="rangeA"/> - <paramref name="rangeB"/>] with an
		/// optional <paramref name="seed"/>. Then returns the result.
		/// </summary>
		public static int Random(this int rangeA, int rangeB, float seed = float.NaN)
		{
			return (int)Random(rangeA, rangeB, 0, seed);
		}
		/// <summary>
		/// Returns true only <paramref name="percent"/>% / returns false (100 - <paramref name="percent"/>)% of the times.
		/// </summary>
		public static bool HasChance(this int percent)
		{
			return HasChance((float)percent);
		}
		/// <summary>
		/// Restricts a <paramref name="number"/> in the inclusive range [<paramref name="rangeA"/> - <paramref name="rangeB"/>] with a
		/// <paramref name="limitation"/> and returns it. Also known as Clamping.
		/// </summary>
		public static int Limit(this int number, int rangeA, int rangeB, Limits limitation = Limits.ClosestBound)
		{
			return (int)Limit((float)number, rangeA, rangeB, limitation);
		}
		/// <summary>
		/// Ensures a <paramref name="number"/> is <paramref name="signed"/> and returns the result.
		/// </summary>
		public static int Sign(this int number, bool signed) => (int)Sign((float)number, signed);
		/// <summary>
		/// Returns whether <paramref name="number"/> is in range [<paramref name="rangeA"/> - <paramref name="rangeB"/>].
		/// The ranges may be <paramref name="inclusiveLower"/> or <paramref name="inclusiveUpper"/>.
		/// </summary>
		public static bool IsBetween(this int number, int rangeA, int rangeB, bool inclusiveLower = false,
			bool inclusiveUpper = false) => IsBetween((float)number, rangeA, rangeB, inclusiveLower, inclusiveUpper);
		/// <summary>
		/// Maps a <paramref name="number"/> from [<paramref name="A1"/> - <paramref name="B1"/>] to
		/// [<paramref name="B1"/> - <paramref name="B2"/>] and returns it. Similar to Lerping (linear interpolation).<br></br>
		/// Example: 50 mapped from [0 - 100] and [0 - 1] results to 0.5<br></br>
		/// Example: 25 mapped from [30 - 20] and [1 - 5] results to 3
		/// </summary>
		public static int Map(this int number, int A1, int A2, int B1, int B2) =>
			(int)Map((float)number, A1, A2, B1, B2);
	}
}
