using SMPL.Core;
using SMPL.Tools;
using System.Numerics;
using SFML.Graphics;

namespace SMPL.Graphics
{
	public class SpriteCamera : Sprite
	{
		public Camera Camera { get; private set; }

		/// <summary>
		/// Create the <see cref="Camera"/> with a certain <paramref name="resolution"/>.
		/// </summary>
		public SpriteCamera(Vector2 resolution) => Init((uint)resolution.X.Limit(0, Texture.MaximumSize), (uint)resolution.Y.Limit(0, Texture.MaximumSize));
		/// <summary>
		/// Create the <see cref="Camera"/> with a certain resolution size of [<paramref name="resolutionX"/>, <paramref name="resolutionY"/>].
		/// </summary>
		public SpriteCamera(uint resolutionX, uint resolutionY) => Init(resolutionX, resolutionY);

		private void Init(uint resolutionX, uint resolutionY)
		{
			resolutionX = (uint)((int)resolutionX).Limit(0, (int)Texture.MaximumSize);
			resolutionY = (uint)((int)resolutionY).Limit(0, (int)Texture.MaximumSize);
			Camera = new(resolutionX, resolutionY);
		}

		public Vector2 PointToWorld(Vector2 cameraWorldPoint)
		{
			var sc = Size / Camera.Resolution;
			var m = Matrix3x2.Identity;

			m *= Matrix3x2.CreateScale(sc);
			m *= Matrix3x2.CreateRotation(Angle);
			m *= Matrix3x2.CreateTranslation(Position);

			m *= Matrix3x2.CreateTranslation(-Camera.Resolution * 0.5f);
			m *= Matrix3x2.CreateTranslation(Camera.PointToWorld(cameraWorldPoint));

			return GetPosition(m * GetParentMatrix());
		}
		public Vector2 PointToCamera(Vector2 point)
		{
			return default;
		}

		public override void Draw()
		{
			Texture = Camera.Texture;
			Camera.Display();
			base.Draw();
		}
	}
}
