namespace SMPL.Tools
{
	/// <summary>
	/// Accesses information about where the code is currently executing and where it was called from.<br></br><br></br>
	/// - Those are usually slow operations, avoid using frequently.
	/// </summary>
	public static class Debug
	{
		/// <summary>
		/// Returns whether this application is started from a standalone .exe or through Visual Studio.
		/// </summary>
		public static bool IsRunningInVisualStudio => Debugger.IsAttached;
		/// <summary>
		/// The offset on the stack call chain going from the current method (index 0) up to <c>Main()</c>.
		/// </summary>
		public static uint CallChainIndex { get; set; }
		/// <summary>
		/// Returns the current line number based on <see cref="CallChainIndex"/> and 0 if it fails to retrieve it.
		/// </summary>
		public static int LineNumber => new StackFrame((int)CallChainIndex + 1, true)?.GetFileLineNumber() ?? 0;
		/// <summary>
		/// Returns the current method name based on <see cref="CallChainIndex"/> and <see langword="null"/> if it fails to retrieve it.
		/// </summary>
		public static string MethodName
		{
			get
			{
				var method = new StackFrame((int)CallChainIndex + 1, true)?.GetMethod();
				var declStr = method == null || method.DeclaringType == null ? "Unknown" : method.DeclaringType.Name;
				return method == null || (method.IsSpecialName && method.Name.Contains(".ctor") == false)
					? default
					: (method?.Name.Replace(".ctor", $"new {declStr}"));
			}
		}
		/// <summary>
		/// Returns the current class name based on <see cref="CallChainIndex"/> and <see langword="null"/> if it fails to retrieve it.
		/// </summary>
		public static string ClassName => new StackFrame((int)CallChainIndex + 1, true)?.GetMethod()?.DeclaringType?.Name;
		/// <summary>
		/// Returns the current namespace based on <see cref="CallChainIndex"/> and <see langword="null"/> if it fails to retrieve it.
		/// </summary>
		public static string Namespace => new StackFrame((int)CallChainIndex + 1, true)?.GetMethod()?.DeclaringType?.Namespace;
		/// <summary>
		/// Returns the current file path based on <see cref="CallChainIndex"/> and <see langword="null"/> if it fails to retrieve it.
		/// </summary>
		public static string FilePath => new StackFrame((int)CallChainIndex + 1, true)?.GetFileName();
		/// <summary>
		/// Returns the current file name based on <see cref="CallChainIndex"/> and <see langword="null"/> if it fails to retrieve it.
		/// </summary>
		public static string FileName => Path.GetFileNameWithoutExtension(new StackFrame((int)CallChainIndex + 1, true)?.GetFileName());
		/// <summary>
		/// Returns the current file directory based on <see cref="CallChainIndex"/> and <see langword="null"/> if it fails to retrieve it.
		/// </summary>
		public static string FileDirectory => Path.GetDirectoryName(new StackFrame((int)CallChainIndex + 1, true)?.GetFileName());
	}
}
