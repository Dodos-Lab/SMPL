namespace SMPL.Graphics
{
	internal class Particle
	{
		internal Vector2 TexCoordsUnitA { get; set; }
		internal Vector2 TexCoordsUnitB { get; set; } = new(1);
		internal Color Color { get; set; } = Color.White;
		internal float Lifetime { get; set; } = 2;
		internal float Speed { get; set; } = 100;
		internal float MoveAngle { get; set; }

		internal Vector2 Position { get; set; }
		internal float Angle { get; set; }
		internal float Size { get; set; } = 8;

		internal Particle()
		{
			TexCoordsUnitA = default;
			TexCoordsUnitB = new(1);
			Color = Color.White;
			Lifetime = 2;
			Speed = 100;
			MoveAngle = 0;

			Position = default;
			Angle = 0;
			Size = 8;
		}
	}

	internal abstract class ParticleManager : Visual
	{
		private readonly List<Particle> particles = new();

		internal int ParticleCount => particles == null ? 0 : particles.Count;

		internal ParticleManager(string uid) : base(uid) { }
		internal void Spawn(uint amount)
		{
			for(int i = 0; i < amount; i++)
				particles.Add(OnParticleSpawn());
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(particles == null || particles.Count == 0)
				return;

			var camera = Get<Camera>(CameraUID);
			if(camera != null)
				renderTarget = camera.GetRenderTexture();

			var ps = new List<Particle>(particles);
			var verts = new Vertex[ps.Count * 4];

			for(int i = 0; i < ps.Count * 4; i += 4)
			{
				var p = ps[i / 4];
				if(p.Lifetime <= 0)
				{
					p.Lifetime = 0;
					particles.Remove(p);
					continue;
				}
				p.Lifetime -= Time.Delta;

				OnParticleUpdate(p);

				var Texture = GetTexture();
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

			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, GetTexture(), GetShader(renderTarget)));
		}
		internal override Hitbox GetBoundingBox()
		{
			throw new NotImplementedException();
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			particles.Clear();
		}

		internal void DestroyParticle(Particle particle) => particles.Remove(particle);
		internal abstract Particle OnParticleSpawn();
		internal abstract void OnParticleUpdate(Particle particle);
	}
}
