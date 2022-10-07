namespace SMPL
{
	public static partial class Thing
	{
		public static Color AmbientColor { get; set; } = new(50, 50, 50);
		public static Color SunColor { get; set; } = Color.White;
		public static float SunAngle { get; set; } = 45f;
	}

	internal class LightInstance : ThingInstance
	{
		public Color Color { get; set; } = new Color(255, 255, 255, 50);
		#region Backend
		internal static readonly Vector2[] positions = new Vector2[50];
		internal static readonly Color[] colors = new Color[50];
		internal static readonly float[] scales = new float[50];

		internal static List<LightInstance> lights = new();

		[JsonConstructor]
		internal LightInstance()
		{
			TryInit();
		}
		internal LightInstance(string uid) : base(uid)
		{
			TryInit();
		}
		private void TryInit()
		{
			if(TryCreationError() == false)
				lights.Add(this);
		}
		private bool TryCreationError()
		{
			if(lights.Count == 50)
			{
				Console.LogError(1, $"Cannot create {nameof(LightInstance)}{{{UID}}}. Only up to 50 lights are allowed.");
				return true;
			}
			return false;
		}

		internal static void UpdateGlobalArrays()
		{
			for(int i = 0; i < lights.Count; i++)
			{
				var light = lights[i];
				positions[i] = light.Position;
				colors[i] = light.Color;
				scales[i] = light.Scale;
			}
		}
		internal static void Update(VisualInstance visual, RenderTarget renderTarget)
		{
			var positionsInShader = new Vector2[positions.Length];
			for(int j = 0; j < lights.Count; j++)
			{
				var light = lights[j];
				var pos = light.Position;
				var sc = light.Scale;
				var dist = pos.Distance(visual.Position);

				var p = renderTarget.MapCoordsToPixel(pos.ToSFML(), renderTarget.GetView()) - new Vector2i(0, (int)renderTarget.Size.Y);
				positionsInShader[j] = new Vector2(p.X, p.Y);

				var verts = GetShadowVerts(visual.BoundingBox, pos, sc);
				renderTarget.Draw(verts, PrimitiveType.Quads);
			}

			var amb = Thing.AmbientColor;
			var ambientVec = new Vector3(amb.R, amb.G, amb.B);
			var lerp = Vector3.Lerp(new(), ambientVec, amb.A / 255f);
			var resultCol = new Color((byte)lerp.X, (byte)lerp.Y, (byte)lerp.Z);
			visual.SetEffectColor("AmbientColor", resultCol);
			visual.SetShaderColorArray("Colors", colors);
			visual.SetShaderVector2Array("Positions", positionsInShader);
			visual.SetShaderFloatArray("Scales", scales);
		}
		private static Vertex[] GetShadowVerts(Hitbox hitbox, Vector2 lightPos, float lightSc)
		{
			var lines = hitbox.Lines;
			if(lines.Count == 0)
				return Array.Empty<Vertex>();

			var verts = new Vertex[lines.Count * 4];
			var shadowSize = DEFAULT_BB_SIZE * lightSc * 0.75f;

			for(int i = 0; i < lines.Count; i++)
			{
				var a = lines[i].A;
				var b = lines[i].B;
				var c = b.MoveToTarget(lightPos, -shadowSize, false);
				var d = a.MoveToTarget(lightPos, -shadowSize, false);
				var center = a.PercentToTarget(b, new(50));
				var index = i * 4;
				var ray = new Hitbox(center, lightPos);
				var rayHits = hitbox.CrossPoints(ray);
				//var distA = a.Distance(lightPos);
				//var distB = b.Distance(lightPos);

				if(rayHits.Count == 1)
					continue;

				verts[index + 0] = new Vertex(a.ToSFML(), Color.Black);
				verts[index + 1] = new Vertex(b.ToSFML(), Color.Black);
				verts[index + 2] = new Vertex(c.ToSFML(), Color.Black);
				verts[index + 3] = new Vertex(d.ToSFML(), Color.Black);
			}

			return verts;

			Color GetColor(float dist)
			{
				var c = dist.Map(shadowSize, 0, 0, 255);
				c = Math.Clamp(c, 0, 255);
				return new(0, 0, 0, (byte)c);
			}
		}

		internal override void OnDestroy()
		{
			base.OnDestroy();

			var i = lights.IndexOf(this);
			positions[i] = default;
			colors[i] = default;
			scales[i] = default;

			lights.Remove(this);
		}
		#endregion
	}
}
