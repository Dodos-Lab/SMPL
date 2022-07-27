namespace SMPL
{
	public static partial class Thing
	{
		public class Particle
		{
			public Vector2 TexCoordUnitA { get; set; }
			public Vector2 TexCoordUnitB { get; set; } = new(1);
			public Color Color { get; set; } = Color.White;
			public float Lifetime { get; set; } = 2;
			public float Speed { get; set; }
			public float MoveAngle { get; set; }

			public Vector2 Position { get; set; }
			public float Angle { get; set; }
			public float Size { get; set; } = 8;
		}
	}
	internal class ParticleManagerInstance : VisualInstance
	{
		public int Count => particles == null ? 0 : particles.Count;

		public void Spawn(Thing.Particle particle)
		{
			particles.Add(particle);
		}
		public void Destroy(Thing.Particle particle)
		{
			particles.Remove(particle);
		}

		#region Backend
		private readonly List<Thing.Particle> particles = new();

		[JsonConstructor]
		internal ParticleManagerInstance() : base() { }
		internal ParticleManagerInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget)
		{
			if(particles.Count == 0)
				return;

			var ps = new List<Thing.Particle>(particles);
			var verts = new Vertex[ps.Count * 4];

			for(int i = 0; i < ps.Count * 4; i += 4)
			{
				var p = ps[i / 4];
				if(p == null)
					continue;

				if(p.Lifetime <= 0)
				{
					p.Lifetime = 0;
					particles.Remove(p);
					continue;
				}
				p.Lifetime -= Time.Delta;

				if(p.Speed != 0)
					p.Position = p.Position.PointMoveAtAngle(p.MoveAngle, p.Speed);

				Event.ParticleUpdate(UID, p);

				var texture = GetTexture();
				var txSz = texture == null ? new Vector2() : new Vector2(texture.Size.X, texture.Size.Y);
				var pos = p.Position;
				var txA = p.TexCoordUnitA * txSz;
				var txB = p.TexCoordUnitB * txSz;
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
		#endregion
	}
}
