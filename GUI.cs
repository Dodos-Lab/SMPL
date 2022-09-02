namespace SMPL
{
	public static class GUI
	{
		public class Body
		{
			public string TexturePath { get; set; }
			public Vector2 SizeUnit { get; set; } = new(0.2f, 0.2f);
			public Color Tint { get; set; } = Color.White;
			[JsonIgnore]
			public Vector2 TexCoordA
			{
				set
				{
					var tex = GetTexture();
					TexCoordUnitA = tex == null ? value : value / new Vector2(tex.Size.X, tex.Size.Y);
				}
				get
				{
					var tex = GetTexture();
					return tex == null ? TexCoordUnitA : new Vector2(tex.Size.X, tex.Size.Y) * TexCoordUnitA;
				}
			}
			[JsonIgnore]
			public Vector2 TexCoordB
			{
				set
				{
					var tex = GetTexture();
					TexCoordUnitB = tex == null ? value : value / new Vector2(tex.Size.X, tex.Size.Y);
				}
				get
				{
					var tex = GetTexture();
					return tex == null ? TexCoordUnitB : new Vector2(tex.Size.X, tex.Size.Y) * TexCoordUnitB;
				}
			}

			public Vector2 TexCoordUnitA { get; set; }
			public Vector2 TexCoordUnitB { get; set; } = new(1);

			#region Backend
			internal Texture GetTexture()
			{
				var textures = Scene.CurrentScene.Textures;
				var path = TexturePath.ToBackslashPath();
				return path != null && textures.ContainsKey(path) ? textures[path] : Game.defaultTexture;
			}
			#endregion
		}

		public static void Image(Vector2 positionUnit = default, Body body = default, RenderTarget renderTarget = default)
		{
			body ??= new();
			var rend = renderTarget ?? Scene.MainCamera.RenderTexture;
			var view = rend.GetView();
			var pos = view.Center.ToSystem() + view.Size.ToSystem() * (positionUnit / 2f);
			var aspectRatio = new Vector2(1, view.Size.X / view.Size.Y);
			var halfSz = (view.Size.ToSystem() * body.SizeUnit * aspectRatio) / 2f;
			var tl = pos + new Vector2(-halfSz.X, -halfSz.Y);
			var tr = pos + new Vector2(halfSz.X, -halfSz.Y);
			var br = pos + new Vector2(halfSz.X, halfSz.Y);
			var bl = pos + new Vector2(-halfSz.X, halfSz.Y);
			var tex = body.GetTexture();
			var w = tex == null ? 0 : tex.Size.X;
			var h = tex == null ? 0 : tex.Size.Y;
			var w0 = w * body.TexCoordUnitA.X;
			var ww = w * body.TexCoordUnitB.X;
			var h0 = h * body.TexCoordUnitA.Y;
			var hh = h * body.TexCoordUnitB.Y;
			var verts = new Vertex[]
			{
				new(tl.ToSFML(), body.Tint, new(w0, h0)),
				new(tr.ToSFML(), body.Tint, new(ww, h0)),
				new(br.ToSFML(), body.Tint, new(ww, hh)),
				new(bl.ToSFML(), body.Tint, new(w0, hh)),
			};
			rend.Draw(verts, PrimitiveType.Quads, new(BlendMode.Alpha, Transform.Identity, tex, null));
		}
		public static bool Button(Vector2 positionUnit = default, Body body = default, RenderTarget renderTarget = default)
		{
			Image(positionUnit, body, renderTarget);
			return false;
		}
	}
}
