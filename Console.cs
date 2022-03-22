using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SMPL
{
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
		#endregion
		private static bool consoleShown;

		public static string Input
		{
			get
			{
				Show();
				SelectionEnable(true);
				var result = System.Console.ReadLine();
				SelectionEnable(false);

				return result;
			}
		}

		public static void Log(object message, bool newLine = true)
		{
			Show();
			System.Console.Write(message + (newLine ? "\n" : ""));
		}
		public static void LogError(int depth, string description)
		{
			if (Debug.IsRunningInVisualStudio == false)
				return;

			var methods = new List<string>();
			var actions = new List<string>();

			if (depth >= 0)
				for (int i = 0; i < 50; i++)
					Add(depth + i + 1);

			Log($"[!] Error at");
			for (int i = methods.Count - 1; i >= 0; i--)
				Log($"[!] - {methods[i]}{actions[i]}");
			Log($"[!] {description}");

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
		public static void Clear()
		{
			Show();
			System.Console.Clear();
		}

		private static void Show()
		{
			if (consoleShown) return;

			AllocConsole();

			SelectionEnable(false);
			System.Console.Title = "Console";
			consoleShown = true;
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
