namespace SMPL.GUI
{
	internal class SliderInstance : ProgressBarInstance
	{
		public float LengthUnit { get; set; } = 0.2f;
		public float Length
		{
			get => LengthUnit.Map(0, 1, 0, MaxLength);
			set => LengthUnit = value.Map(0, MaxLength, 0, 1);
		}

		public Color ProgressColor { get; set; } = new(255, 255, 255, 100);
		public Color EmptyColor { get; set; } = new(0, 0, 0, 100);

		#region Backend
		private bool isClicked;

		[JsonConstructor]
		internal SliderInstance() { }
		internal SliderInstance(string uid) : base(uid)
		{
			OriginUnit = new(0.5f);
		}

		internal override void OnDraw(RenderTarget renderTarget)
		{
			TryUpdate();

			if(IsHidden)
				return;

			TexCoordUnitB = new(1);

			var tex = GetTexture();
			var w = tex == null ? 0 : tex.Size.X;
			var h = tex == null ? 0 : tex.Size.Y;
			var w0 = w * TexCoordUnitA.X;
			var ww = w * TexCoordUnitB.X;
			var h0 = h * TexCoordUnitA.Y;
			var hh = h * TexCoordUnitB.Y;
			var bb = base.BoundingBox.Lines;

			var tl = bb[0].A.PointPercentTowardPoint(bb[1].A, new(ProgressUnit * 100 - LengthUnit * 50));
			var tr = bb[0].A.PointPercentTowardPoint(bb[1].A, new(ProgressUnit * 100 + LengthUnit * 50));
			var bl = bb[3].A.PointPercentTowardPoint(bb[2].A, new(ProgressUnit * 100 - LengthUnit * 50));
			var br = bb[3].A.PointPercentTowardPoint(bb[2].A, new(ProgressUnit * 100 + LengthUnit * 50));

			if(ProgressUnit < LengthUnit * 0.5f)
			{
				tl = bb[0].A;
				bl = bb[3].A;
			}
			else if(ProgressUnit > 1 - LengthUnit * 0.5f)
			{
				tr = bb[1].A;
				br = bb[2].A;
			}

			var verts = new Vertex[]
			{
				new(tl.ToSFML(), Tint, new(w0, h0)),
				new(tr.ToSFML(), Tint, new(ww, h0)),
				new(br.ToSFML(), Tint, new(ww, hh)),
				new(bl.ToSFML(), Tint, new(w0, hh)),
			};
			var fill = new Vertex[]
			{
				new(bb[0].A.ToSFML(), ProgressColor),
				new(tl.ToSFML(), ProgressColor),
				new(bl.ToSFML(), ProgressColor),
				new(bb[3].A.ToSFML(), ProgressColor),

				new(tr.ToSFML(), EmptyColor),
				new(bb[1].A.ToSFML(), EmptyColor),
				new(bb[2].A.ToSFML(), EmptyColor),
				new(br.ToSFML(), EmptyColor),
			};

			renderTarget.Draw(fill, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, null, null));
			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, tex, GetShader(renderTarget)));
		}
		private void TryUpdate()
		{
			if(IsDisabled)
				return;

			Size = new(MaxLength * Scale, Size.Y);

			var left = Mouse.IsButtonPressed(Mouse.Button.Left);
			if(left.Once($"slider-click-{GetHashCode()}") && BoundingBox.IsHovered)
				isClicked = true;

			if(left == false)
				isClicked = false;

			if(isClicked)
			{
				var bb = BoundingBox.Lines;
				var a = bb[0].A;
				var b = bb[0].B;
				var closest = new Line(a, b).GetClosestPoint(Scene.MouseCursorPosition);
				var dist = a.DistanceBetweenPoints(b);
				var value = a.DistanceBetweenPoints(closest).Map(0, dist, RangeA, RangeB);
				var sz = Size;

				Value = value;
				Size = sz;
			}
		}
		#endregion
	}
}
