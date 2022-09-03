namespace SMPL
{
	public static class GUI
	{
		public class Body
		{
			public RenderTarget RenderTarget { get; set; }

			public string Name { get; set; }
			public string TexturePath { get; set; }

			public Vector2 PositionUnit { get; set; }
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
			internal bool isChecked;

			internal (Vector2, Vector2) Corners
			{
				get
				{
					var rend = RenderTarget ?? Scene.MainCamera.RenderTexture;
					var view = rend.GetView();
					var pos = view.Center.ToSystem() + view.Size.ToSystem() * (PositionUnit / 2f);
					var aspectRatio = new Vector2(1, view.Size.X / view.Size.Y);
					var halfSz = (view.Size.ToSystem() * SizeUnit * aspectRatio) / 2f;
					var tl = pos + new Vector2(-halfSz.X, -halfSz.Y);
					var br = pos + new Vector2(halfSz.X, halfSz.Y);
					return (tl, br);
				}
			}
			internal bool IsHovered
			{
				get
				{
					var pos = Scene.MouseCursorPosition;
					var corners = Corners;
					return pos.X.IsBetween(corners.Item1.X, corners.Item2.X) &&
						pos.Y.IsBetween(corners.Item1.Y, corners.Item2.Y);
				}
			}
			internal bool Intersects(Body body)
			{
				var corners = Corners;
				var targetCor = body.Corners;

				return corners.Item1.X < targetCor.Item2.X &&
					corners.Item2.X > targetCor.Item1.X &&
					corners.Item1.Y < targetCor.Item2.Y &&
					corners.Item2.Y > targetCor.Item1.Y;
			}
			internal Texture GetTexture()
			{
				var textures = Scene.CurrentScene.Textures;
				var path = TexturePath.ToBackslashPath();
				return path != null && textures.ContainsKey(path) ? textures[path] : Game.defaultTexture;
			}
			#endregion
		}

		public static void Image(Body body = default)
		{
			body ??= new();
			var rend = body.RenderTarget ?? Scene.MainCamera.RenderTexture;
			var corners = body.Corners;
			var tl = corners.Item1;
			var br = corners.Item2;
			var tr = new Vector2(br.X, tl.Y);
			var bl = new Vector2(tl.X, br.Y);
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
		public static bool Button(Body body = default, float holdDelay = 0.5f, float holdTriggerTimeOffset = 0.1f)
		{
			holdDelayTimer -= Time.Delta;
			holdTriggerTimer = holdTriggerTimer < 0 ? holdTriggerTimeOffset : holdTriggerTimer - Time.Delta;

			var hovered = body.IsHovered;
			var leftClicked = Mouse.IsButtonPressed(Mouse.Button.Left);
			var id = body.GetHashCode();
			var result = false;

			Console.Log(id);

			if(holdTriggerTimer < 0 && holdDelayTimer < 0 && hovered && isClicked)
			{
				Event._ButtonHold(body);
				result = true;
			}

			if(hovered.Once($"{id}-hovered-a;sglk"))
			{
				if(isClicked)
					Event._ButtonPress(body);

				Event._ButtonHover(body);
			}

			if((hovered == false).Once($"{id}-unhovered-a;sglk"))
				Event._ButtonUnhover(body);

			if(leftClicked.Once($"{id}-press-a;sglk") && hovered)
			{
				isClicked = true;
				holdDelayTimer = holdDelay;
				Event._ButtonPress(body);
			}

			if((leftClicked == false).Once($"{id}-release-a;sglk"))
			{
				if(hovered)
				{
					if(isClicked)
					{
						Event._ButtonClick(body);
						result = true;
					}

					Event._ButtonRelease(body);
					Event._ButtonHover(body);
				}
				isClicked = false;
			}

			Image(body);
			return result;
		}
		public static bool Checkbox(Body body = default)
		{
			var result = false;
			if(Button(body, int.MaxValue, int.MaxValue))
			{
				body.isChecked = body.isChecked == false;

				if(body.isChecked)
				{
					Event._CheckboxCheck(body);
					result = true;
				}
				else
					Event._CheckboxUncheck(body);
			}

			Image(body);
			return result;
		}

		#region Backend
		private static float holdDelayTimer, holdTriggerTimer;
		private static bool isClicked;
		#endregion
	}
}
