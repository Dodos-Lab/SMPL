namespace SMPL.UI
{
	public static partial class Thing
	{
		public enum TextboxAlignment
		{
			TopLeft, Top, TopRight,
			Left, Center, Right,
			BottomLeft, Bottom, BottomRight
		}
		public enum TextboxSymbolCollection { Character, Word, Line }
	}

	internal class TextboxInstance : TextInstance
	{
		public Color BackgroundColor { get; set; } = new(100, 100, 100);

		public override string FontPath { get; set; }
		public override string Value { get; set; }

		public string CameraUID { get; set; }

		public Thing.TextboxAlignment Alignment { get; set; }

		public Vector2 ShadowOffset { get; set; }
		public Color ShadowColor { get; set; } = Color.Black;

		public int GetSymbolIndex(Vector2 worldPoint)
		{
			for(int i = 0; i < Value.Length; i++)
			{
				var corners = GetSymbolCorners(i).ToList();
				if(corners.Count == 4)
					corners.Add(corners[0]);
				var hitbox = new Hitbox(corners.ToArray());
				if(hitbox.Contains(worldPoint))
					return i;
			}
			return -1;
		}
		public ReadOnlyCollection<Vector2> GetSymbolCorners(int characterIndex)
		{
			var result = new List<Vector2>();

			if(characterIndex < 0 || characterIndex > Value.Length - 1)
				return result.AsReadOnly();

			Update();
			var prevIndex = characterIndex;
			characterIndex = (characterIndex + 1).Limit(0, Value.Length);

			var tl = (textInstance.FindCharacterPos((uint)characterIndex - 1)).ToSystem();
			var tr = (textInstance.FindCharacterPos((uint)characterIndex)).ToSystem();

			tl += (textInstance.Position - textInstance.Origin).ToSystem();
			tr += (textInstance.Position - textInstance.Origin).ToSystem();

			if(prevIndex < Value.Length && Value[prevIndex] == '\n') // end of line
				tr = new(GetRes().X, tl.Y);

			var sc = Scale;
			var y = new Vector2(0, SymbolSize);
			var bl = tl + y;
			var br = tr + y;

			if(tl.X == tr.X || tl.Y == bl.Y)
				return result.AsReadOnly();

			result.Add(GetPositionFromSelf(tl / sc));
			result.Add(GetPositionFromSelf(tr / sc));
			result.Add(GetPositionFromSelf(br / sc));
			result.Add(GetPositionFromSelf(bl / sc));

			return result.AsReadOnly();
		}
		public string GetSymbols(Vector2 worldPoint, Thing.TextboxSymbolCollection symbols)
		{
			for(int i = 0; i < Value.Length; i++)
			{
				var corners = GetSymbolCorners(i).ToList();
				if(corners.Count == 0)
					continue;

				corners.Add(corners[0]);
				var hitbox = new Hitbox(corners.ToArray());
				if(hitbox.Contains(worldPoint))
				{
					var left = "";
					var right = "";

					if(symbols == Thing.TextboxSymbolCollection.Character)
						return Value[(int)i].ToString();
					else if(symbols == Thing.TextboxSymbolCollection.Word)
					{
						if(Value[(int)i].ToString().IsLetters() == false)
							return null;

						for(int l = (int)i; l >= 0; l--)
						{
							var symbol = $"{Value[l]}";
							if(symbol.IsLetters())
								left = $"{symbol}{left}";
							else
								break;
							if(Value.Contains(left) == false)
								break;
						}
						for(int r = (int)i + 1; r < Value.Length; r++)
						{
							var symbol = $"{Value[r]}";
							if(symbol.IsLetters())
								right = $"{right}{symbol}";
							else
								break;
							if(Value.Contains(right) == false)
								break;
						}
					}
					else
					{
						if(Value[(int)i] == '\n')
							return null;

						for(int l = (int)i; l >= 0; l--)
						{
							var symbol = $"{Value[l]}";
							if(symbol != '\n'.ToString())
								left = $"{symbol}{left}";
							else
								break;
							if(Value.Contains(left) == false)
								break;
						}
						for(int r = (int)i + 1; r < Value.Length; r++)
						{
							var symbol = $"{Value[r]}";
							if(symbol != '\n'.ToString())
								right = $"{right}{symbol}";
							else
								break;
							if(Value.Contains(right) == false)
								break;
						}
					}
					return $"{left}{right}";
				}
			}
			return null;
		}

		#region Backend
		private List<(int, int)> formatSpaceRangesRight = new(), formatSpaceRangesCenter = new();

		protected bool skipParentRender;
		protected float textOffsetX;

		[JsonConstructor]
		internal TextboxInstance() { }
		internal TextboxInstance(string uid, string cameraUID) : base(uid)
		{
			Init(cameraUID);
		}

		private void Init(string cameraUID)
		{
			CameraUID = cameraUID;
			TexturePath = cameraUID;
			Alignment = Thing.TextboxAlignment.Center;
			Value = "Hello, World!";
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			Update();

			var camera = GetCamera();
			if(camera == null)
				return;

			if(skipParentRender == false)
				camera.RenderTexture.Clear(BackgroundColor);
			if(ShadowOffset != default || ShadowColor.A == 0)
			{
				var tr = Transform.Identity;
				var col = textInstance.FillColor;
				var outCol = textInstance.OutlineColor;
				tr.Translate(ShadowOffset.ToSFML());
				textInstance.FillColor = ShadowColor;
				textInstance.OutlineColor = ShadowColor;
				camera.RenderTexture.Draw(textInstance, new(tr));
				textInstance.FillColor = col;
				textInstance.OutlineColor = outCol;
			}
			if(string.IsNullOrWhiteSpace(textInstance.DisplayedString) == false)
				camera.RenderTexture.Draw(textInstance);

			if(skipParentRender == false)
				DrawTextbox(renderTarget);
		}
		internal override Hitbox GetBoundingBox()
		{
			bb.LocalLines.Clear();
			bb.Lines.Clear();
			bb.LocalLines.AddRange(GetCamera().BoundingBox.LocalLines);
			return bb;
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			GetCamera()?.Destroy(false);
			formatSpaceRangesCenter = null;
			formatSpaceRangesRight = null;
		}

		protected void Update()
		{
			var camera = GetCamera();
			if(camera == null)
				return;

			textInstance.Position = new(textOffsetX, 0);
			textInstance.Rotation = 0;
			textInstance.Scale = new(1, 1);
			textInstance.Font = GetFont();
			textInstance.CharacterSize = (uint)SymbolSize;
			textInstance.FillColor = Tint;
			textInstance.LetterSpacing = SymbolSpace;
			textInstance.LineSpacing = LineSpace;
			textInstance.OutlineColor = OutlineColor;
			textInstance.OutlineThickness = OutlineSize;
			textInstance.Style = Style;
			textInstance.DisplayedString = Value;

			var b = GetLocalBounds();
			textInstance.Origin = new(b.Width / 2f, b.Height / 2f);
			var sz = camera.RenderTexture.Size;
			var smallOffset = SymbolSize / 4f;
			var top = -sz.Y / 2f + b.Height / 2f - smallOffset / 2f;
			var left = -sz.X / 2f + b.Width / 2f + smallOffset;
			var right = sz.X / 2f - b.Width / 2f - smallOffset;
			var bot = sz.Y / 2f - b.Height / 2f - smallOffset * 1.5f;

			switch(Alignment)
			{
				case Thing.TextboxAlignment.TopLeft: textInstance.Position = new(textOffsetX + left, top); break;
				case Thing.TextboxAlignment.TopRight: textInstance.Position = new(textOffsetX + right, top); break;
				case Thing.TextboxAlignment.Top: textInstance.Position = new(textOffsetX, top); break;
				case Thing.TextboxAlignment.Left: textInstance.Position = new(textOffsetX + left, -smallOffset); break;
				case Thing.TextboxAlignment.Right: textInstance.Position = new(textOffsetX + right, -smallOffset); break;
				case Thing.TextboxAlignment.Center: textInstance.Position = new(textOffsetX, -smallOffset); break;
				case Thing.TextboxAlignment.BottomLeft: textInstance.Position = new(textOffsetX + left, bot); break;
				case Thing.TextboxAlignment.BottomRight: textInstance.Position = new(textOffsetX + right, bot); break;
				case Thing.TextboxAlignment.Bottom: textInstance.Position = new(textOffsetX, bot); break;
			}
		}
		protected virtual FloatRect GetLocalBounds()
		{
			return textInstance.GetLocalBounds();
		}
		protected void DrawTextbox(RenderTarget renderTarget)
		{
			var camera = GetCamera();
			if(camera == null)
				return;

			camera.RenderTexture.Display();

			var bb = BoundingBox;
			var res = GetRes();
			var verts = new Vertex[]
			{
				new(bb.Lines[0].A.ToSFML(), Color.White, new()),
				new(bb.Lines[1].A.ToSFML(), Color.White, new(res.X, 0)),
				new(bb.Lines[2].A.ToSFML(), Color.White, res.ToSFML()),
				new(bb.Lines[3].A.ToSFML(), Color.White, new(0, res.Y)),
			};
			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, Scene.CurrentScene.Textures[camera.UID], GetShader(renderTarget)));
		}
		protected CameraInstance GetCamera()
		{
			return Get<CameraInstance>(CameraUID);
		}
		private Font GetFont()
		{
			var fonts = Scene.CurrentScene.Fonts;
			var path = FontPath.ToBackslashPath();
			return path != null && fonts.ContainsKey(path) ? fonts[path] : null;
		}
		internal Vector2 GetRes()
		{
			var cam = Get<CameraInstance>(CameraUID);
			return cam == null ? default : cam.Resolution;
		}
		#endregion
	}
}
