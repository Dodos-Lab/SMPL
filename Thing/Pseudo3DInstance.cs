namespace SMPL
{
	internal class Pseudo3DInstance : VisualInstance
	{
		public string CameraUID { get; set; }
		public float Depth { get; set; } = 100;
		public float Tilt { get; set; } = 45f;
		public float Z { get; set; }

		public Vector2 Position2D { get; set; }

		public Vector2 LocalSize { get; set; } = new(100);
		[JsonIgnore]
		public Vector2 Size
		{
			get => LocalSize * Scale;
			set => LocalSize = value / Scale;
		}

		public Vector2 OriginUnit { get; set; } = new(0.5f);
		[JsonIgnore]
		public Vector2 Origin
		{
			get => OriginUnit * LocalSize;
			set => OriginUnit = value / LocalSize;
		}

		#region Backend
		protected float currDepth;
		private static readonly List<Pseudo3DInstance> pseudo3Ds = new();

		[JsonConstructor]
		internal Pseudo3DInstance() => Init();
		internal Pseudo3DInstance(string uid) : base(uid) => Init();

		private void Init()
		{
			pseudo3Ds.Add(this);
		}
		internal static void Update()
		{
			for(int i = 0; i < pseudo3Ds.Count; i++)
			{
				var instance = pseudo3Ds[i];

				if(instance.IsDisabled)
					continue;

				var z = GetUnitZ().X;
				var cam = GetCamera();
				instance.Position = instance.Z < -1000f || instance.Z > 1000f ? new Vector2().NaN() : GetPosZ();
				instance.Scale = 1f - z / 100f;
				instance.Tilt = cam.Position.AngleBetweenPoints(instance.Position);
				instance.currDepth = instance.Position.DistanceBetweenPoints(cam.Position) * (instance.Depth / 500f);

				Vector2 GetUnitZ()
				{
					var z = instance.Z.Map(-1000, 1000, 0, 1);
					var anim = z.Animate(Extensions.Animation.Circle, Extensions.AnimationWay.Forward);
					return new(anim.Map(0, 1, -646.41f, 100));
				}
				Vector2 GetPosZ()
				{
					var cam = GetCamera();
					return instance.Position2D.PointPercentTowardPoint(cam.Position, GetUnitZ());
				}
				CameraInstance GetCamera()
				{
					var cam = Get<CameraInstance>(instance.CameraUID);
					cam ??= Scene.MainCamera;
					return cam;
				}
			}
		}
		internal override void OnDraw(RenderTarget renderTarget) { }
		internal override void OnDestroy()
		{
			base.OnDestroy();
			pseudo3Ds.Remove(this);
		}

		#endregion
	}
}
