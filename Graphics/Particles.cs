using SFML.Graphics;
using SMPL.Graphics;
using SMPL.Tools;
using SMPL.UI;
using System.Collections.Generic;
using System.Numerics;

namespace SMPL.Graphics
{
	/// <summary>
	/// A particle controlled by the <see cref="ParticleManager"/> (check it for more info). No <see cref="Particle"/> references should be
	/// saved outside the scopes of <see cref="ParticleManager.OnParticleSpawn"/> and <see cref="ParticleManager.OnParticleUpdate"/>
	/// because it would be impossible for the <see cref="ParticleManager"/> to destroy said <see cref="Particle"/>s. This class may be inherited
	/// to further extend the data it holds but it should be cast to the child class in order to be used:<code>
	/// protected override void OnParticleUpdate(Particle particle)
	/// {
	/// var myBloodParticle = (BloodParticle)particle;
	/// // some blood movement code...
	/// return myBloodParticle;
	/// }</code>
	/// </summary>
	public class Particle
	{
		public Vector2 TexCoordsUnitA { get; set; }
		public Vector2 TexCoordsUnitB { get; set; } = new(1);
		public Color Color { get; set; } = Color.White;
		public float Lifetime { get; set; } = 2;
		public float Speed { get; set; } = 100;
		public float MoveAngle { get; set; }

		public Vector2 Position { get; set; }
		public float Angle { get; set; }
		public float Size { get; set; } = 8;

		public Particle() { }
		public Particle(Vector2 texCoordsA, Vector2 texCoordsB)
		{
			TexCoordsUnitA = texCoordsA;
			TexCoordsUnitB = texCoordsB;
		}
	}

	/// <summary>
	/// Inherit chain: <see cref="ParticleManager"/> : <see cref="Visual"/> : <see cref="Object"/><br></br><br></br>
	/// A class that spawns, updates and destroys <see cref="Particle"/>s. Here is how it is used:<br></br>
	/// - A child class that inherits <see cref="ParticleManager"/> is created.<br></br>
	/// - A certain amount of <see cref="Particle"/>s are spawned from the game logic by calling <see cref="Spawn"/>.<br></br>
	/// - <see cref="OnParticleSpawn"/> is overriden in the child class to initialize each setting of each <see cref="Particle"/>.<br></br>
	/// - <see cref="OnParticleUpdate(Particle)"/> is overriden in the child class to continuously update each
	/// <see cref="Particle"/> however needed.<br></br>Some logic in here might lead up to <see cref="DestroyParticle(Particle)"/>.
	/// </summary>
	public abstract class ParticleManager : Visual
	{
		private readonly List<Particle> particles = new();

		/// <summary>
		/// The amount of currently alive <see cref="Particle"/>s tracked by the <see cref="ParticleManager"/> manager.
		/// </summary>
		public int ParticleCount => particles == null ? 0 : particles.Count;

		/// <summary>
		/// Spawn a certain <paramref name="amount"/> of <see cref="Particle"/>s.
		/// </summary>
		public void Spawn(uint amount)
		{
			for (int i = 0; i < amount; i++)
				particles.Add(OnParticleSpawn());
		}

		/// <summary>
		/// End the life of a <paramref name="particle"/> immediately.
		/// </summary>
		protected void DestroyParticle(Particle particle) => particles.Remove(particle);
		/// <summary>
		/// This is called the very moment a <see cref="Particle"/> is spawned. Useful for configuring the initial settings for
		/// each <see cref="Particle"/>. See <see cref="ParticleManager"/> for more info.
		/// </summary>
		protected abstract Particle OnParticleSpawn();
		/// <summary>
		/// This is called for each <see cref="Particle"/> when calling <see cref="Draw"/>. Useful for continuously updating
		/// each <paramref name="particle"/> according to some logic. See <see cref="ParticleManager"/> for more info.
		/// </summary>
		protected abstract void OnParticleUpdate(Particle particle);

		/// <summary>
		/// Draws each <see cref="Particle"/> tracked by this <see cref="ParticleManager"/> on the <see cref="Visual.DrawTarget"/> according
		/// to all the required <see cref="Object"/>, <see cref="Visual"/> and <see cref="Particle"/> parameters.
		/// </summary>
		public override void Draw()
		{
			if (particles == null || particles.Count == 0)
				return;

			DrawTarget ??= Scene.MainCamera;

			var ps = new List<Particle>(particles);
			var verts = new Vertex[ps.Count * 4];

			for (int i = 0; i < ps.Count * 4; i += 4)
			{
				var p = ps[i / 4];
				if (p.Lifetime <= 0)
				{
					p.Lifetime = 0;
					particles.Remove(p);
					continue;
				}
				p.Lifetime -= Time.Delta;

				OnParticleUpdate(p);

				var txSz = Texture == null ? new Vector2() : new Vector2(Texture.Size.X, Texture.Size.Y);
				var pos = p.Position;
				var txA = p.TexCoordsUnitA * txSz;
				var txB = p.TexCoordsUnitB * txSz;
				var c = p.Color;
				var sz = p.Size;
				var ang = p.Angle;

				var topLeft = pos.PointMoveAtAngle(ang + 270, sz * 0.5f, false).PointMoveAtAngle(ang + 180, sz * 0.5f, false);
				var topRight = topLeft.PointMoveAtAngle(ang, sz, false);
				var botRight = topRight.PointMoveAtAngle(ang + 90, sz, false);
				var botLeft = topLeft.PointMoveAtAngle(ang + 90, sz, false);

				verts[i] = new(topLeft.ToSFML(), c, new(txA.X, txA.Y));
				verts[i + 1] = new(topRight.ToSFML(), c, new(txB.X, txA.Y));
				verts[i + 2] = new(botRight.ToSFML(), c, new(txB.X, txB.Y));
				verts[i + 3] = new(botLeft.ToSFML(), c, new(txA.X, txB.Y));
			}

			DrawTarget.renderTexture.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
	}
}
