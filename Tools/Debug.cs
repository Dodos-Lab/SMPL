using System.Diagnostics;
using System.IO;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;

namespace SMPL.Tools
{
	/// <summary>
	/// Accesses information about where the code is currently executing and where it was called from.
	/// </summary>
	public static class Debug
	{
		/// <summary>
		/// Returns whether this application is started from a standalone .exe or through Visual Studio.
		/// </summary>
		public static bool IsRunningInVisualStudio => Debugger.IsAttached;
		/// <summary>
		/// The offset on the stack call chain going from the current method (index 0) up to Main().
		/// </summary>
		public static uint CallChainIndex { get; set; }
		/// <summary>
		/// Returns the current line number based on <see cref="CallChainIndex"/> and 0 if it fails to retrieve it.
		/// </summary>
		public static int LineNumber => new StackFrame((int)CallChainIndex + 1, true)?.GetFileLineNumber() ?? 0;

		public static string MethodName => new StackFrame((int)CallChainIndex + 1, true)?.GetMethod()?.Name;
		public static string ClassName => new StackFrame((int)CallChainIndex + 1, true)?.GetMethod()?.DeclaringType?.Name;
		public static string Namespace => new StackFrame((int)CallChainIndex + 1, true)?.GetMethod()?.DeclaringType?.Namespace;
		public static string FilePath => new StackFrame((int)CallChainIndex + 1, true)?.GetFileName();
		public static string FileName => Path.GetFileNameWithoutExtension(new StackFrame((int)CallChainIndex + 1, true)?.GetFileName());
		public static string FileDirectory => Path.GetDirectoryName(new StackFrame((int)CallChainIndex + 1, true)?.GetFileName());
	}
}
