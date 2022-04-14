using SFML.Graphics;

namespace SMPL
{
	/// <summary>
	/// Inherit chain: <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
	/// A base class for most textured graphics.
	/// </summary>
	public abstract class Visual : Object
	{
		/// <summary>
		/// The color of this <see cref="Visual"/>.
		/// </summary>
		public Color Color { get; set; } = Color.White;
		/// <summary>
		/// At which <see cref="DrawTarget"/> this <see cref="Visual"/> should be drawn. This value is set to <see cref="Scene.MainCamera"/>
		/// if it is null upon drawing.
		/// </summary>
		public Camera DrawTarget { get; set; } = Scene.MainCamera;
		/// <summary>
		/// The type of drawing as to whether it should include the <see cref="BlendMode.Alpha"/> channel, whether it should
		/// <see cref="BlendMode.Add"/>/<see cref="BlendMode.Multiply"/> the color
		/// to the already present background color etc. The default expected behavior is <see cref="BlendMode.Alpha"/>. <see cref="BlendMode.Multiply"/>
		/// is good for simulating light since (putting half transparent yellow for example) it will enhance the colors beneath it making them "lit"
		/// in yellow "light".
		/// </summary>
		public BlendMode BlendMode { get; set; } = BlendMode.Alpha;
		/// <summary>
		/// Whether this <see cref="Visual"/> skips its <see cref="Draw"/>.
		/// </summary>
		public bool IsHidden { get; set; }
		/// <summary>
		/// The texture is used in many ways by each <see cref="Visual"/> (if at all). Check them for info as to how they use it.
		/// </summary>
		public Texture Texture { get; set; }
		/// <summary>
		/// The shader applied to this <see cref="Visual"/> upon drawing. This is used for per pixel/per corner effects that run on the GPU
		/// (and are extremely fast because of that). Shaders are written in the GLSL language as <see cref="string"/> and are given to the
		/// <see cref="SFML.Graphics.Shader"/> class to be compiled and used. Check <see cref="Effect"/> for premade ones.
		/// </summary>
		public Shader Shader { get; set; }

		// no summary since it's covered by the classes that inherit this class & it is not visible to the user
		public abstract void Draw();
	}
}
