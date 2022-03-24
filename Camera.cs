using System.Numerics;

namespace SMPL
{
	public static class Camera
	{
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

		public static Vector2 TopLeft
		{
			get => Game.Window.MapPixelToCoords(new()).ToSystem();
		}
		public static Vector2 TopRight
		{
			get => Game.Window.MapPixelToCoords(new((int)Game.Window.Size.X, 0)).ToSystem();
		}
		public static Vector2 BottomRight
		{
			get => Game.Window.MapPixelToCoords(new((int)Game.Window.Size.X, (int)Game.Window.Size.Y)).ToSystem();
		}
		public static Vector2 BottomLeft
		{
			get => Game.Window.MapPixelToCoords(new(0, (int)Game.Window.Size.Y)).ToSystem();
		}

		public static bool Captures(Vector2 point)
		{
			var hitbox = new Hitbox(TopLeft, TopRight, BottomRight, BottomLeft, TopLeft);
			return hitbox.ConvexContains(point);
		}
	}
}
