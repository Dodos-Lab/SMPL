namespace SMPL.Graphics
{
	public struct Light
	{
		internal static readonly Vector2[] positions = new Vector2[50];
		internal static readonly Color[] colors = new Color[50];
		internal static readonly float[] radiuses = new float[50];
		internal static readonly float[] intensities = new float[50];

		public Vector2 Position { get; set; }
		public float Radius { get; set; }
		public float Intensity { get; set; }
		public Color Color { get; set; }

		public Light(Vector2 position, Color color, float radius = 0.5f, float intensity = 1f)
		{
			Position = position;
			Radius = radius;
			Intensity = intensity;
			Color = color;
		}

		public static Color AmbientColor { get; set; } = new Color(50, 50, 50);
		public static void Set(int index, Light light)
		{
			if(TryIndexError(index))
				return;

			positions[index] = light.Position;
			colors[index] = light.Color;
			intensities[index] = light.Intensity;
			radiuses[index] = light.Radius;
		}
		public static Light Get(int index)
		{
			return TryIndexError(index) ? default : new Light(positions[index], colors[index], radiuses[index], intensities[index]);
		}
		internal static void Update(Visual visual)
		{
			var camera = Thing.Get<Camera>(visual.CameraUID);
			if(camera == null)
				camera = Thing.Get<Camera>(Scene.MAIN_CAMERA_UID);

			var rend = camera.renderTexture;
			var positionsInShader = new Vector2[positions.Length];
			for(int j = 0; j < positionsInShader.Length; j++)
			{
				var pos = rend.MapCoordsToPixel(positions[j].ToSFML(), rend.GetView()) - new Vector2i(0, (int)(rend.Texture.Size.Y));
				positionsInShader[j] = new(pos.X, pos.Y);
			}

			visual.SetShaderColor("AmbientColor", AmbientColor);
			visual.SetShaderColorArray("Colors", colors);
			visual.SetShaderVector2Array("Positions", positionsInShader);
			visual.SetShaderFloatArray("Intensities", intensities);
			visual.SetShaderFloatArray("Radiuses", radiuses);
		}
		private static bool TryIndexError(int index)
		{
			if(index.IsBetween(0, 49, true, true) == false)
			{
				Console.LogError(1, $"The index {index} is out of range. Up to 50 lights are allowed (indexes 0 to 49 inclusively).");
				return true;
			}
			return false;
		}
	}
}
