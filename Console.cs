using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace SMPL
{
	/// <summary>
	/// A wrapper class for the <see cref="System.Console"/>. Mainly used for debugging by logging messages during runtime. This class
	/// makes sure to instantiate the <see cref="Console"/> once it is needed.
	/// </summary>
	public static class Console
	{
		#region smol brain
		private static void Form1_Load(object sender, EventArgs e) => AllocConsole();
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();
		private enum StdHandle : int
		{
			STD_INPUT_HANDLE = -10,
			STD_OUTPUT_HANDLE = -11,
			STD_ERROR_HANDLE = -12,
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle); //returns Handle
		internal enum ConsoleMode : uint
		{
			ENABLE_ECHO_INPUT = 0x0004,
			ENABLE_EXTENDED_FLAGS = 0x0080,
			ENABLE_INSERT_MODE = 0x0020,
			ENABLE_LINE_INPUT = 0x0002,
			ENABLE_MOUSE_INPUT = 0x0010,
			ENABLE_PROCESSED_INPUT = 0x0001,
			ENABLE_QUICK_EDIT_MODE = 0x0040,
			ENABLE_WINDOW_INPUT = 0x0008,
			ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,

			//screen buffer handle
			ENABLE_PROCESSED_OUTPUT = 0x0001,
			ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
			ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
			DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
			ENABLE_LVB_GRID_WORLDWIDE = 0x0010
		}
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr handle, int cmd);

		#endregion
		private static string input;
		private static bool isCreated, isVisible, inputRequired;
		private static Thread thread;

		public static string RequiredInput
		{
			get => isCreated ? input : null;
			private set => input = value;
		}
		public static string Title
		{
#pragma warning disable CA1416
			get => isCreated ? System.Console.Title : null;
#pragma warning restore CA1416
			set { TryCreate(); System.Console.Title = value; }
		}
		public static bool IsVisible
		{
			get => isCreated && isVisible;
			set
			{
				isVisible = value;
            if (isVisible)
					TryCreate();
				ShowWindow(GetConsoleWindow(), value ? 5 : 0);
			}
		}

		public static void RequireInput()
		{
			inputRequired = true;
			TryCreate();
		}
		/// <summary>
		/// Display <paramref name="message"/> on the <see cref="Console"/>. May be followed by <paramref name="newLine"/>.
		/// </summary>
		public static void Log(object message, bool newLine = true)
		{
			TryCreate();
			System.Console.Write(message + (newLine ? "\n" : ""));
		}
		/// <summary>
		/// Display an error on the <see cref="Console"/> with <paramref name="description"/>.
		/// Some information about where the error has occurred is also included through <paramref name="depth"/> and <see cref="Debug"/> methods.
		/// The <paramref name="description"/> is skipped if <paramref name="depth"/> is -1.
		/// </summary>
		public static void LogError(int depth, string description)
		{
			if (Debug.IsRunningInVisualStudio == false || depth < -1)
				return;

			var methods = new List<string>();
			var actions = new List<string>();

			if (depth >= 0)
				for (int i = 0; i < 50; i++)
					Add(depth + i + 1);

			Log($"[!] Error: {description}");
         if (depth > -1)
				Log($"[!] Method chain call:");
			for (int i = methods.Count - 1; i >= 0; i--)
				Log($"[!] - {methods[i]}{actions[i]}");
			Log("");

			void Add(int depth)
			{
				if (depth < 0)
					return;
				var action = Debug.GetMethodName(depth);
				if (string.IsNullOrEmpty(action))
					return;

				var file = $"{Debug.GetFileName(depth + 1)}.cs/";
				var method = $"{Debug.GetMethodName(depth + 1)}()";
				var line = $"{Debug.GetLineNumber(depth + 1)}";
				var methodName = file == ".cs/" ? "" : $"{file}{method}";

				if (methodName != "")
				{
					methods.Add(methodName);
					actions.Add($" {{ [{line}] {action}(); }}");
				}
			}
		}
		/// <summary>
		/// Remove all logs on the <see cref="Console"/>.
		/// </summary>
		public static void Clear()
		{
			TryCreate();
			System.Console.Clear();
		}

		private static void TryCreate()
		{
			if (isCreated)
				return;

			AllocConsole();

			SelectionEnable(false);
			System.Console.Title = "Console";
			isCreated = true;
			IsVisible = true;

			thread = new(UpdateThread) { IsBackground = true, Name = "Console" };
			thread.Start();
		}
		private static void UpdateThread()
      {
         while (true)
         {
				Thread.Sleep(1);

				if (inputRequired == false)
					continue;

				input = System.Console.ReadLine();
				inputRequired = false;
         }
      }
		private static void SelectionEnable(bool enabled)
		{
			var consoleHandle = GetStdHandle((int)StdHandle.STD_INPUT_HANDLE);
			GetConsoleMode(consoleHandle, out uint consoleMode);
			if (enabled) consoleMode |= ((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);
			else consoleMode &= ~((uint)ConsoleMode.ENABLE_QUICK_EDIT_MODE);

			consoleMode |= ((uint)ConsoleMode.ENABLE_EXTENDED_FLAGS);

			SetConsoleMode(consoleHandle, consoleMode);
		}
	}
}
