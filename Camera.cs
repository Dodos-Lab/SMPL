using System.Numerics;

namespace SMPL
{
	public static class Camera
	{
		/// <summary>
		/// Position in the world.
		/// </summary>
		public static Vector2 Position
		{
			get => Game.Window.GetView().Center.ToSystem();
			set
			{
				var view = Game.Window.GetView();
				view.Center = value.ToSFML();
				Game.Window.SetView(view);
			}
		}
		/// <summary>
		/// Angle in the world.
		/// </summary>
		public static float Angle
		{
			get => Game.Window.GetView().Rotation;
			set
			{
				var view = Game.Window.GetView();
				view.Rotation = value;
				Game.Window.SetView(view);
			}
		}
		/// <summary>
		/// Size in the world.
		/// </summary>
		public static Vector2 Size
		{
			get => Game.Window.GetView().Size.ToSystem();
			set
			{
				var view = Game.Window.GetView();
				view.Size = value.ToSFML();
				Game.Window.SetView(view);
			}
		}
		

		/// <summary>
		/// The top left corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public static Vector2 TopLeft
		{
			get => Game.Window.MapPixelToCoords(new()).ToSystem();
		}
		/// <summary>
		/// The top right corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public static Vector2 TopRight
		{
			get => Game.Window.MapPixelToCoords(new((int)Game.Window.Size.X, 0)).ToSystem();
		}
		/// <summary>
		/// The bottom right corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public static Vector2 BottomRight
		{
			get => Game.Window.MapPixelToCoords(new((int)Game.Window.Size.X, (int)Game.Window.Size.Y)).ToSystem();
		}
		/// <summary>
		/// The bottom left corner of the <see cref="Camera"/> in the world.
		/// </summary>
		public static Vector2 BottomLeft
		{
			get => Game.Window.MapPixelToCoords(new(0, (int)Game.Window.Size.Y)).ToSystem();
		}

		/// <summary>
		/// Whether the <see cref="Camera"/> can "see" <paramref name="hitbox"/>.<br></br>
		/// - Note: This uses <see cref="Hitbox.ConvexContains(Hitbox)"/> so concave <see cref="Hitbox"/>es will give wrong results.
		/// </summary>
		public static bool Captures(Hitbox hitbox)
		{
			var screen = new Hitbox(TopLeft, TopRight, BottomRight, BottomLeft, TopLeft);
			return screen.ConvexContains(hitbox);
		}
		/// <summary>
		/// Whether the <see cref="Camera"/> can "see" <paramref name="point"/>.
		/// </summary>
		public static bool Captures(Vector2 point)
		{
			var screen = new Hitbox(TopLeft, TopRight, BottomRight, BottomLeft, TopLeft);
			return screen.ConvexContains(point);
		}
	}
}
