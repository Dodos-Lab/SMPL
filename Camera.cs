using SFML.Graphics;
using System.Numerics;
using System;
using SFML.System;
using SFML.Window;

namespace SMPL
{
	/// <summary>
	/// Inherit chain: <see cref="RenderTexture"/> : <see cref="RenderTarget"/> : <see cref="SFML.ObjectBase"/> : <see cref="IDisposable"/><br></br><br></br>
	/// A camera under the hood is just a <see cref="Texture"/> that can be drawn onto. The drawn things will appear as if they were "viewed" from the
	/// <see cref="Camera"/>. The resulting <see cref="Texture"/> may be drawn onto another <see cref="Camera"/> by most of the <see cref="Visual"/>s
	/// (usually and by default being the <see cref="Scene.MainCamera"/>).
	/// </summary>
	public class Camera : RenderTexture
	{
		/// <summary>
		/// Position in the world.
		/// </summary>
		public Vector2 ViewPosition
		{
			get => GetView().Center.ToSystem();
			set
			{
				var view = GetView();
				view.Center = value.ToSFML();
				SetView(view);
			}
		}
		/// <summary>
		/// Angle in the world.
		/// </summary>
		public float ViewAngle
		{
			get => GetView().Rotation;
			set
			{
				var view = GetView();
				view.Rotation = value;
				SetView(view);
			}
		}
		/// <summary>
		/// Size in the world.
		/// </summary>
		public Vector2 ViewSize
		{
			get => GetView().Size.ToSystem();
			set
			{
				var view = GetView();
				view.Size = value.ToSFML();
				SetView(view);
			}
		}

		/// <summary>
		/// The mouse cursor's position in the world (<see cref="CurrentScene"/>) relative to this <see cref="Camera"/>.
		/// </summary>
		public Vector2 MouseCursorPosition
		{
			get { var p = Game.Window.MapPixelToCoords(Mouse.GetPosition(Game.Window), GetView()); return new(p.X, p.Y); }
			set => Mouse.SetPosition(Game.Window.MapCoordsToPixel(value.ToSFML(), GetView()), Game.Window);
		}

		/// <summary>
		/// The top left corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public Vector2 TopLeft
		{
			get => MapPixelToCoords(new()).ToSystem();
		}
		/// <summary>
		/// The top right corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public Vector2 TopRight
		{
			get => MapPixelToCoords(new((int)Game.Window.Size.X, 0)).ToSystem();
		}
		/// <summary>
		/// The bottom right corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public Vector2 BottomRight
		{
			get => MapPixelToCoords(new((int)Game.Window.Size.X, (int)Game.Window.Size.Y)).ToSystem();
		}
		/// <summary>
		/// The bottom left corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public Vector2 BottomLeft
		{
			get => MapPixelToCoords(new(0, (int)Game.Window.Size.Y)).ToSystem();
		}

		/// <summary>
		/// Create the <see cref="Camera"/> with a certain <paramref name="resolution"/>.
		/// </summary>
		public Camera(Vector2 resolution) : base((uint)resolution.X.Limit(0, Texture.MaximumSize), (uint)resolution.Y.Limit(0, Texture.MaximumSize)) { }
		/// <summary>
		/// Create the <see cref="Camera"/> with a certain size of [<paramref name="resolutionX"/>, <paramref name="resolutionY"/>].
		/// </summary>
		public Camera(uint resolutionX, uint resolutionY) :
			base((uint)((int)resolutionX).Limit(0, (int)Texture.MaximumSize), (uint)((int)resolutionY).Limit(0, (int)Texture.MaximumSize)) { }

		/// <summary>
		/// Returns whether the <see cref="Camera"/> can "see" <paramref name="hitbox"/>. This uses <see cref="Hitbox.ConvexContains(Hitbox)"/>
		/// so concave <see cref="Hitbox"/>es may give wrong results.
		/// </summary>
		public bool Captures(Hitbox hitbox)
		{
			var screen = new Hitbox(TopLeft, TopRight, BottomRight, BottomLeft, TopLeft);
			return screen.ConvexContains(hitbox);
		}
		/// <summary>
		/// Returns whether the <see cref="Camera"/> can "see" <paramref name="point"/>.
		/// </summary>
		public bool Captures(Vector2 point)
		{
			var screen = new Hitbox(TopLeft, TopRight, BottomRight, BottomLeft, TopLeft);
			return screen.ConvexContains(point);
		}
		/// <summary>
		/// Tries to save this <see cref="Camera"/>'s <see cref="Texture"/> into an image file at some <paramref name="imagePath"/>. This is a slow
		/// operation (especially with bigger a <see cref="Texture.Size"/>) and it is better to avoid frequent calls to not hurt
		/// performance. Then returns whether the saving was successful.
		/// </summary>
		public void Snap(string imagePath)
      {
			var img = Texture.CopyToImage();
			var result = img.SaveToFile(imagePath);
			img.Dispose();

			if (result == false)
				Console.LogError(1, $"Could not save the image at '{imagePath}'.");
      }

		internal static void DrawMainCameraToWindow()
      {
			Scene.MainCamera.Display();
			var texSz = Scene.MainCamera.Size;
			var viewSz = Game.Window.GetView().Size;
			var verts = new Vertex[]
			{
				new(-viewSz * 0.5f, new Vector2f()),
				new(new Vector2f(viewSz.X * 0.5f, -viewSz.Y * 0.5f), new Vector2f(texSz.X, 0)),
				new(viewSz * 0.5f, new Vector2f(texSz.X, texSz.Y)),
				new(new Vector2f(-viewSz.X * 0.5f, viewSz.Y * 0.5f), new Vector2f(0, texSz.Y))
			};

			Game.Window.Draw(verts, PrimitiveType.Quads, new(Scene.MainCamera.Texture));
      }
	}
}
