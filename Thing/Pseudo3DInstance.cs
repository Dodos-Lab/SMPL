namespace SMPL
{
	internal class Pseudo3DInstance : VisualInstance
	{
		public float Depth { get; set; } = 50;
		public float Tilt { get; set; } = 45f;

		#region Backend
		[JsonConstructor]
		internal Pseudo3DInstance() { }
		internal Pseudo3DInstance(string uid) : base(uid) { }

		internal override void OnDraw(RenderTarget renderTarget) { }
		#endregion
	}
}
