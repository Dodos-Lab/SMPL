using SFML.Graphics;
using System.Collections.Generic;
using System.Numerics;
using SFML.System;

namespace SMPL
{
	public abstract class Particles : Visual
	{
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

		private readonly List<Particle> particles = new();
		public int ParticleCount => particles == null ? 0 : particles.Count;

		public void Spawn(uint amount)
		{
			for (int i = 0; i < amount; i++)
				particles.Add(OnParticleSpawn());
		}

		protected abstract Particle OnParticleSpawn();
		protected abstract void OnParticleUpdate(Particle particle);
		public override void Draw()
		{
			if (particles == null || particles.Count == 0)
				return;

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

				var topLeft = pos.MovePointAtAngle(ang + 270, sz * 0.5f, false).MovePointAtAngle(ang + 180, sz * 0.5f, false);
				var topRight = topLeft.MovePointAtAngle(ang, sz, false);
				var botRight = topRight.MovePointAtAngle(ang + 90, sz, false);
				var botLeft = topLeft.MovePointAtAngle(ang + 90, sz, false);

				verts[i] = new(topLeft.ToSFML(), c, new(txA.X, txA.Y));
				verts[i + 1] = new(topRight.ToSFML(), c, new(txB.X, txA.Y));
				verts[i + 2] = new(botRight.ToSFML(), c, new(txB.X, txB.Y));
				verts[i + 3] = new(botLeft.ToSFML(), c, new(txA.X, txB.Y));
			}

			RenderTarget.Draw(verts, PrimitiveType.Quads, new(BlendMode, Transform.Identity, Texture, Shader));
		}
	}
}
