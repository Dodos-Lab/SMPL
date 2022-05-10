using Newtonsoft.Json;
using SFML.Graphics;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;

namespace SMPL.Graphics
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
		public Color Tint { get; set; } = Color.White;
		/// <summary>
		/// The type of drawing as to whether it should include the <see cref="BlendMode.Alpha"/> channel, whether it should
		/// <see cref="BlendMode.Add"/>/<see cref="BlendMode.Multiply"/> the color
		/// to the already present background color etc. The default expected behavior is <see cref="BlendMode.Alpha"/>. <see cref="BlendMode.Multiply"/>
		/// is good for simulating light since putting half transparent yellow for example, will enhance the colors beneath it, making them "lit"
		/// in yellow "light".
		/// </summary>
		public BlendMode BlendMode { get; set; } = BlendMode.Alpha;
		/// <summary>
		/// Whether this <see cref="Visual"/> skips its <see cref="Draw"/>.
		/// </summary>
		public bool IsHidden { get; set; }
		/// <summary>
		/// See <see cref="Texture"/> for info.
		/// </summary>
		public string TexturePath { get; set; }
		/// <summary>
		/// See <see cref="ShaderPath"/> for info.
		/// </summary>
		public string ShaderPath { get; set; }
		/// <summary>
		/// The <see cref="SFML.Graphics.Texture"/> is retrieved by the <see cref="TexturePath"/> from the <see cref="Scene.CurrentScene"/>'s loaded textures and is
		/// used in many ways by each <see cref="Visual"/> (if at all).
		/// </summary>
		[JsonIgnore]
		public Texture Texture => TexturePath != null && Scene.CurrentScene.Textures.ContainsKey(TexturePath)? Scene.CurrentScene.Textures[TexturePath] : null;
		/// <summary>
		/// The <see cref="SFML.Graphics.Shader"/> is retrieved by the <see cref="ShaderPath"/> from the <see cref="Scene.CurrentScene"/>'s loaded shaders and is
		/// used by <see cref="Visual"/>s (if at all). Shaders are used for per pixel/corner effects that run on the GPU
		/// (and are extremely fast because of that). Shaders are written in the GLSL language as <see cref="string"/> and are given to the
		/// <see cref="SFML.Graphics.Shader"/> class to be compiled and used.
		/// </summary>
		[JsonIgnore]
		public Shader Shader => ShaderPath != null && Scene.CurrentScene.Shaders.ContainsKey(ShaderPath) ? Scene.CurrentScene.Shaders[ShaderPath] : null;

		// no summary since it's covered by the classes that inherit this class & it is not visible to the user
		public abstract void Draw(Camera camera = null);
	}
}
