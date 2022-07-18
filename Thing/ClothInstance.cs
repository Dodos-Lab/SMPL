namespace SMPL
{
	internal class ClothInstance : VisualInstance
	{
		public float OriginUnit { get; set; }

		#region Backend
		private Vector2 size, quadCount;
		private readonly List<Rope> ropes = new();

		[JsonConstructor]
		internal ClothInstance() { }
		internal ClothInstance(string uid, Vector2 size, Vector2 quadCount) : base(uid)
		{
			this.quadCount = new(MathF.Max(1, quadCount.X), MathF.Max(1, quadCount.Y));
			this.size = new(MathF.Max(1, size.X), MathF.Max(1, size.Y));

			for(int i = 0; i < quadCount.X; i++)
			{

			}
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{

		}
		internal override Hitbox GetBoundingBox()
		{
			var origin = new Vector2(OriginUnit, 0);
			var hitbox = new Hitbox(
				-origin,
				new Vector2(size.X, 0) - origin,
				size - origin,
				new Vector2(0, size.Y) - origin,
				-origin);
			return hitbox;
		}

		#endregion
	}
}
