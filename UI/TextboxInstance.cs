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
		public Vector2 Resolution => res;
		public Color BackgroundColor { get; set; } = new(100, 100, 100);

		public new string FontPath
		{
			get => font;
			set
			{
				font = value;
				Value = left; // recalculate alignments
			}
		}
		public new string Value
		{
			get => left;
			set
			{
				left = value;

				textInstance.Font = GetFont();
				textInstance.CharacterSize = (uint)SymbolSize;
				textInstance.LetterSpacing = SymbolSpace;
				textInstance.LineSpacing = LineSpace;
				textInstance.Style = Style;
				textInstance.Position = new();
				textInstance.Rotation = 0;
				textInstance.Scale = new(1, 1);

				textInstance.DisplayedString = " ";
				var spaceWidth = textInstance.GetGlobalBounds().Width;

				formatSpaceRangesCenter.Clear();
				formatSpaceRangesRight.Clear();
				right = "";
				center = "";

				var lines = left.Split('\n');
				lineCount = (uint)lines.Length;
				for(int i = 0; i < lines.Length; i++)
				{
					var line = lines[i];
					textInstance.DisplayedString = line;
					var width = LineWidth - textInstance.GetGlobalBounds().Width;
					var spaces = spaceWidth == 0 ? 0 : (int)(width / spaceWidth).Limit(0, 9999).Round() - 1;
					var centerLeft = (int)(((float)spaces) / 2).Round();
					var centerRight = (int)(((float)spaces) / 2).Round();

					spaces = Math.Max(spaces, 1);
					centerLeft = Math.Max(centerLeft, 1);
					centerRight = Math.Max(centerRight, 1);

					formatSpaceRangesRight.Add((right.Length, right.Length - 1 + spaces));
					right += $"{" ".Repeat(spaces)}{line}\n";

					formatSpaceRangesCenter.Add((center.Length, center.Length - 1 + centerLeft));
					center += $"{" ".Repeat(centerLeft)}{line}";
					formatSpaceRangesCenter.Add((center.Length, center.Length - 1 + centerRight));
					center += $"{" ".Repeat(centerRight)}\n";
				}
			}
		}

		public uint LineWidth { get; set; }
		public uint LineCount => lineCount;

		public Thing.TextboxAlignment Alignment { get; set; }

		public Vector2 ShadowOffset { get; set; }
		public Color ShadowColor { get; set; } = Color.Black;

		public int GetSymbolIndex(Vector2 worldPoint)
		{
			for(int i = 0; i < Value.Length; i++)
			{
				var corners = GetSymbolCorners((uint)i).ToList();
				if(corners.Count == 4)
					corners.Add(corners[0]);
				var hitbox = new Hitbox(corners.ToArray());
				if(hitbox.ConvexContains(worldPoint))
					return i;
			}
			return -1;
		}
		public ReadOnlyCollection<Vector2> GetSymbolCorners(uint characterIndex)
		{
			var result = new List<Vector2>();

			if(characterIndex > left.Length - 1)
				return result.AsReadOnly();

			Update();
			var prevIndex = (int)characterIndex;
			characterIndex = GetNextNonFormatChar(characterIndex);

			var tl = (textInstance.Position + textInstance.FindCharacterPos(characterIndex)).ToSystem();
			var tr = (textInstance.Position + textInstance.FindCharacterPos(characterIndex + 1)).ToSystem();

			if(prevIndex < left.Length && left[prevIndex] == '\n') // end of line
				tr = new(Resolution.X, tl.Y);

			var sc = Scale;
			var boundTop = textInstance.GetLocalBounds().Top;
			var y = new Vector2(0, SymbolSize + boundTop);
			var bl = tl + y;
			var br = tr + y;

			tl.Y += boundTop;
			tr.Y += boundTop;

			var view = camera.RenderTexture.GetView();
			var camA = view.Center - view.Size * 0.5f;
			var camB = view.Center + view.Size * 0.5f;

			Limit(ref tl);
			Limit(ref tr);
			Limit(ref br);
			Limit(ref bl);

			if(tl.X == tr.X || tl.Y == bl.Y)
				return result.AsReadOnly();

			result.Add(GetPositionFromSelf(tl / sc));
			result.Add(GetPositionFromSelf(tr / sc));
			result.Add(GetPositionFromSelf(br / sc));
			result.Add(GetPositionFromSelf(bl / sc));

			return result.AsReadOnly();

			void Limit(ref Vector2 corner) => corner = new(corner.X.Limit(camA.X, camB.X), corner.Y.Limit(camA.Y, camB.Y));
		}
		public string GetSymbols(Vector2 worldPoint, Thing.TextboxSymbolCollection symbols)
		{
			for(uint i = 0; i < Value.Length; i++)
			{
				var corners = GetSymbolCorners(i).ToList();
				if(corners.Count == 0)
					continue;

				corners.Add(corners[0]);
				var hitbox = new Hitbox(corners.ToArray());
				if(hitbox.ConvexContains(worldPoint))
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
		private Vector2 res;
		private uint lineCount;
		private List<(int, int)> formatSpaceRangesRight = new(), formatSpaceRangesCenter = new();
		private CameraInstance camera;
		private string left, center, right, font;

		[JsonConstructor]
		internal TextboxInstance() { }
		internal TextboxInstance(string uid, string cameraUID, uint resolutionX = 200, uint resolutionY = 200) : base(uid)
		{
			Init(cameraUID, resolutionX, resolutionY);
		}

		private void Init(string cameraUID, uint resolutionX, uint resolutionY)
		{
			camera = new(cameraUID, new(resolutionX, resolutionY));
			res = new(resolutionX, resolutionY);
			TexturePath = camera.UID;
			LineWidth = resolutionX;
			Alignment = Thing.TextboxAlignment.Center;
			left = "Hello, World!";
		}
		internal override void OnDraw(RenderTarget renderTarget)
		{
			Update();

			camera.RenderTexture.Clear(BackgroundColor);
			if(ShadowOffset != default)
			{
				var tr = Transform.Identity;
				var col = textInstance.FillColor;
				tr.Translate(ShadowOffset.ToSFML());
				textInstance.FillColor = ShadowColor;
				camera.RenderTexture.Draw(textInstance, new(SFML.Graphics.BlendMode.Alpha, tr, null, null));
				textInstance.FillColor = col;
			}
			camera.RenderTexture.Draw(textInstance);
			camera.RenderTexture.Display();

			var bb = BoundingBox;
			var verts = new Vertex[]
			{
				new(bb.Lines[0].A.ToSFML(), Color.White, new()),
				new(bb.Lines[1].A.ToSFML(), Color.White, new(Resolution.X, 0)),
				new(bb.Lines[2].A.ToSFML(), Color.White, Resolution.ToSFML()),
				new(bb.Lines[3].A.ToSFML(), Color.White, new(0, Resolution.Y)),
			};
			renderTarget.Draw(verts, PrimitiveType.Quads, new(GetBlendMode(), Transform.Identity, Scene.CurrentScene.Textures[camera.UID], GetShader(renderTarget)));
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			camera.Destroy(false);
			camera = null;
			formatSpaceRangesCenter = null;
			formatSpaceRangesRight = null;
		}
		internal override Hitbox GetBoundingBox()
		{
			var res = Resolution * new Vector2(0.5f);
			var or = res * OriginUnit;
			return new Hitbox(
				new Vector2(0) - or,
				new Vector2(res.X, 0) - or,
				new Vector2(res.X, res.Y) - or,
				new Vector2(0, res.Y) - or,
				new Vector2(0) - or);
		}

		private void Update()
		{
			textInstance.Position = new();
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

			switch(Alignment)
			{
				case Thing.TextboxAlignment.TopLeft: Left(); break;
				case Thing.TextboxAlignment.TopRight: Right(); break;
				case Thing.TextboxAlignment.Top: CenterX(); break;
				case Thing.TextboxAlignment.Left: Left(); break;
				case Thing.TextboxAlignment.Right: Right(); break;
				case Thing.TextboxAlignment.Center: CenterX(); break;
				case Thing.TextboxAlignment.BottomLeft: Left(); break;
				case Thing.TextboxAlignment.BottomRight: Right(); break;
				case Thing.TextboxAlignment.Bottom: CenterX(); break;
			}
			var b = textInstance.GetLocalBounds();
			var sz = camera.RenderTexture.Size;
			switch(Alignment)
			{
				case Thing.TextboxAlignment.TopLeft: Top(); break;
				case Thing.TextboxAlignment.TopRight: Top(); break;
				case Thing.TextboxAlignment.Top: Top(); break;
				case Thing.TextboxAlignment.Left: CenterY(); break;
				case Thing.TextboxAlignment.Right: CenterY(); break;
				case Thing.TextboxAlignment.Center: CenterY(); break;
				case Thing.TextboxAlignment.BottomLeft: Bottom(); break;
				case Thing.TextboxAlignment.BottomRight: Bottom(); break;
				case Thing.TextboxAlignment.Bottom: Bottom(); break;
			}

			void Top() => textInstance.Position = new(-sz.X * 0.5f, -camera.RenderTexture.Size.Y * 0.5f - b.Top * 0.5f);
			void CenterX() => textInstance.DisplayedString = center;
			void CenterY() => textInstance.Position = new(-sz.X * 0.5f, -b.Height * 0.5f - (Alignment == Thing.TextboxAlignment.Left ? b.Top : 0));
			void Bottom() => textInstance.Position = new(-sz.X * 0.5f, camera.RenderTexture.Size.Y * 0.5f - b.Height + b.Top);
			void Left() => textInstance.DisplayedString = left;
			void Right() => textInstance.DisplayedString = right;
		}
		private Font GetFont()
		{
			var fonts = Scene.CurrentScene.Fonts;
			var path = FontPath.ToBackslashPath();
			return path != null && fonts.ContainsKey(path) ? fonts[path] : null;
		}
		private uint GetNextNonFormatChar(uint charIndex)
		{
			if(Alignment == Thing.TextboxAlignment.TopRight || Alignment == Thing.TextboxAlignment.Right || Alignment == Thing.TextboxAlignment.BottomRight)
				return Execute(formatSpaceRangesRight, true);
			else if(Alignment == Thing.TextboxAlignment.Top || Alignment == Thing.TextboxAlignment.Center || Alignment == Thing.TextboxAlignment.Bottom)
				return Execute(formatSpaceRangesCenter);

			return charIndex;

			uint Execute(List<(int, int)> ranges, bool isRight = false)
			{
				var realIndex = (uint)0;
				for(uint i = 0; i < textInstance.DisplayedString.Length; i++)
				{
					var lastRange = (0, 0);
					for(int j = 0; j < ranges.Count; j++)
						if(((int)i).IsBetween(ranges[j].Item1, ranges[j].Item2, true, true))
						{
							i = (uint)ranges[j].Item2 + 1;
							lastRange = ranges[j];
							break;
						}
					if(realIndex == charIndex)
						return textInstance.DisplayedString[(int)i] == '\n' && isRight == false ? (uint)lastRange.Item1 : i;

					realIndex++;
				}
				return charIndex;
			}
		}
		#endregion
	}
}
