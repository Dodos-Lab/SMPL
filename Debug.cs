using System.Diagnostics;

namespace SMPL
{
	public static class Debug
	{
		/// <summary>
		/// Returns whether this application is started from a standalone .exe or through Visual Studio.
		/// </summary>
		public static bool IsRunningInVisualStudio => Debugger.IsAttached;

		public static uint GetLineNumber(int depth = 0)
		{
			var info = new StackFrame(depth + 1, true);
			return (uint)info.GetFileLineNumber();
		}
		public static string GetMethodName(int depth = 0)
		{
			var info = new StackFrame(depth + 1, true);
			return info?.GetMethod()?.Name;
		}
		public static string GetFileName(int depth = 0)
		{
			var pathRaw = GetFilePath(depth + 1);
			if (pathRaw == null) return null;
			var path = pathRaw.Split('\\');
			var name = path[^1].Split('.');
			return name[0];
		}
		public static string GetFilePath(int depth = 0)
		{
			var info = new StackFrame(depth + 1, true);
			return info.GetFileName();
		}
		public static string GetFileDirectory(int depth = 0)
		{
			var fileName = new StackFrame(depth + 1, true).GetFileName();
			if (fileName == default)
				return default;
			var path = fileName.Split('\\');
			var dir = "";
			for (int i = 0; i < path.Length - 1; i++)
			{
				dir += path[i];
				if (i == path.Length - 2)
					continue;
				dir += "\\";
			}
			return dir;
		}
	}
}
