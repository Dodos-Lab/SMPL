namespace SMPL
{
	internal class Pseudo3DInstance : VisualInstance
	{
		public string CameraUID { get; set; }
		public float Depth { get; set; } = 50;
		public float Tilt { get; set; } = 45f;
		public float Z { get; set; } = 800f;

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

		[JsonConstructor]
		internal Pseudo3DInstance() => Init();
		internal Pseudo3DInstance(string uid) : base(uid) => Init();

		private void Init()
		{
			Event.SceneUpdated += OnSceneUpdate;
		}

		private void OnSceneUpdate(string sceneName)
		{
			var z = GetUnitZ().X;
			Position = Z < -1000f || Z > 1000f ? new Vector2().NaN() : GetPosZ();
			Scale = 1f - z / 100f;
		}

		private Vector2 GetUnitZ()
		{
			var z = Z.Map(-1000, 1000, 0, 1);
			var anim = z.Animate(Extensions.Animation.Circle, Extensions.AnimationWay.Forward);
			return new(anim.Map(0, 1, -646.41f, 100));
		}
		private Vector2 GetPosZ()
		{
			var cam = GetCamera();
			return Position2D.PointPercentTowardPoint(cam.Position, GetUnitZ());
		}
		private Vector2 GetRealPos()
		{
			var cam = GetCamera();
			return Position2D.PointPercentTowardPoint(cam.Position, GetUnitZ());
		}
		private CameraInstance GetCamera()
		{
			var cam = Get<CameraInstance>(CameraUID);
			cam ??= Scene.MainCamera;
			return cam;
		}
		internal override void OnDraw(RenderTarget renderTarget) { }
		#endregion
	}
}
