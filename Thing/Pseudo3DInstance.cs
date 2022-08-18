namespace SMPL
{
	internal class Pseudo3DInstance : VisualInstance
	{
		public float Depth { get; set; } = 50;
		public float Tilt { get; set; } = 45f;

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
		internal Pseudo3DInstance() { }
		internal Pseudo3DInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget) { }
		#endregion
	}
}
