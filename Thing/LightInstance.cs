namespace SMPL
{
	public static partial class Thing
	{
		public static Color ShadowColor { get; set; } = new(50, 50, 50, 150);
		public static int ShadowOrderZ { get; set; }
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

		private static void CalculateShadowVerts(Hitbox hitbox, Vector2 lightPos, float lightSc, VertexArray shadowMapVerts)
		{
			var lines = hitbox.Lines;
			if(lines.Count == 0)
				return;

			var shadowSize = DEFAULT_BB_SIZE * lightSc * 1.05f;

			for(int i = 0; i < lines.Count; i++)
			{
				var a = lines[i].A;
				var b = lines[i].B;
				var distA = lightPos.Distance(a);
				var distB = lightPos.Distance(b);
				var c = b.MoveToTarget(lightPos, -MathF.Max(0, shadowSize - distB), false);
				var d = a.MoveToTarget(lightPos, -MathF.Max(0, shadowSize - distA), false);

				//var center = a.PercentToTarget(b, new(50));
				//var ray = new Hitbox(center, lightPos);
				//var rayHits = hitbox.CrossPoints(ray);
				//if(rayHits.Count == 1)
				//	continue;

				shadowMapVerts.Append(new(a.ToSFML(), Thing.ShadowColor));
				shadowMapVerts.Append(new(b.ToSFML(), Thing.ShadowColor));
				shadowMapVerts.Append(new(c.ToSFML(), Color.Transparent));
				shadowMapVerts.Append(new(d.ToSFML(), Color.Transparent));

			}
		}
		internal static void UpdateGlobalArrays(RenderTarget renderTarget)
		{
			for(int i = 0; i < lights.Count; i++)
			{
				var light = lights[i];
				colors[i] = light.Color;
				scales[i] = light.Scale;

				var p = renderTarget.MapCoordsToPixel(
					light.Position.ToSFML(), renderTarget.GetView()) - new Vector2i(0, (int)renderTarget.Size.Y);
				positions[i] = new Vector2(p.X, p.Y);
			}
		}
		internal static void Update(VisualInstance visual, VertexArray shadowMapVerts)
		{
			for(int j = 0; j < lights.Count; j++)
			{
				var light = lights[j];
				var pos = light.Position;
				var sc = light.Scale;

				CalculateShadowVerts(visual.BoundingBox, pos, sc, shadowMapVerts);
			}

			var amb = Thing.ShadowColor;
			var ambientVec = new Vector3(amb.R, amb.G, amb.B);
			var lerp = Vector3.Lerp(new(), ambientVec, amb.A / 255f);
			var resultCol = new Color((byte)lerp.X, (byte)lerp.Y, (byte)lerp.Z);
			visual.SetEffectColor("AmbientColor", resultCol);
			visual.SetShaderColorArray("Colors", colors);
			visual.SetShaderVector2Array("Positions", positions);
			visual.SetShaderFloatArray("Scales", scales);
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
