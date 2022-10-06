namespace SMPL
{
	public static partial class Thing
	{
		public sealed class Property
		{
			#region Property Names
			public const string IS_DISABLED = nameof(ThingInstance.IsDisabled);
			public const string IS_DISABLED_SELF = nameof(ThingInstance.IsDisabledSelf);
			public const string TYPES = nameof(ThingInstance.Types);
			public const string UID = nameof(ThingInstance.UID);
			public const string OLD_UID = nameof(ThingInstance.OldUID);
			public const string NUMERIC_UID = nameof(ThingInstance.NumericUID);
			public const string TAGS = nameof(ThingInstance.Tags);
			public const string AGE = nameof(ThingInstance.Age);
			public const string PARENT_UID = nameof(ThingInstance.ParentUID);
			public const string PARENT_OLD_UID = nameof(ThingInstance.ParentOldUID);
			public const string CHILDREN_UIDS = nameof(ThingInstance.ChildrenUIDs);
			public const string LOCAL_POSITION = nameof(ThingInstance.LocalPosition);
			public const string LOCAL_ANGLE = nameof(ThingInstance.LocalAngle);
			public const string LOCAL_DIRECTION = nameof(ThingInstance.LocalDirection);
			public const string LOCAL_SCALE = nameof(ThingInstance.LocalScale);
			public const string POSITION = nameof(ThingInstance.Position);
			public const string ANGLE = nameof(ThingInstance.Angle);
			public const string DIRECTION = nameof(ThingInstance.Direction);
			public const string SCALE = nameof(ThingInstance.Scale);
			public const string HITBOX = nameof(ThingInstance.Hitbox);
			public const string BOUNDING_BOX = nameof(ThingInstance.BoundingBox);

			public const string CAMERA_IS_SMOOTH = nameof(CameraInstance.IsSmooth);
			public const string CAMERA_MOUSE_CURSOR_POSITION = nameof(CameraInstance.MousePosition);
			public const string CAMERA_RENDER_TEXTURE = nameof(CameraInstance.RenderTexture);
			public const string CAMERA_RESOLUTION = nameof(CameraInstance.Resolution);

			public const string VISUAL_TINT = nameof(VisualInstance.Tint);
			public const string VISUAL_IS_HIDDEN = nameof(VisualInstance.IsHidden);
			public const string VISUAL_IS_HIDDEN_SELF = nameof(VisualInstance.IsHiddenSelf);
			public const string VISUAL_IS_SMOOTH = nameof(VisualInstance.IsSmooth);
			public const string VISUAL_IS_REPEATED = nameof(VisualInstance.IsRepeated);
			public const string VISUAL_ORDER = nameof(VisualInstance.Order);
			public const string VISUAL_TEXTURE_PATH = nameof(VisualInstance.TexturePath);
			public const string VISUAL_EFFECT = nameof(VisualInstance.Effect);
			public const string VISUAL_CAMERA_TAG = nameof(VisualInstance.CameraTag);
			public const string VISUAL_BLEND_MODE = nameof(VisualInstance.BlendMode);

			public const string SPRITE_TEX_COORD_UNIT_A = nameof(SpriteInstance.TexCoordUnitA);
			public const string SPRITE_TEX_COORD_UNIT_B = nameof(SpriteInstance.TexCoordUnitB);
			public const string SPRITE_TEX_COORD_A = nameof(SpriteInstance.TexCoordA);
			public const string SPRITE_TEX_COORD_B = nameof(SpriteInstance.TexCoordB);
			public const string SPRITE_SIZE = nameof(SpriteInstance.Size);
			public const string SPRITE_LOCAL_SIZE = nameof(SpriteInstance.LocalSize);
			public const string SPRITE_ORIGIN = nameof(SpriteInstance.Origin);
			public const string SPRITE_ORIGIN_UNIT = nameof(SpriteInstance.OriginUnit);

			public const string PSEUDO_3D_CAMERA_UID = nameof(Pseudo3DInstance.CameraUID);
			public const string PSEUDO_3D_Z = nameof(Pseudo3DInstance.Z);
			public const string PSEUDO_3D_POSITION_2D = nameof(Pseudo3DInstance.Position2D);
			public const string PSEUDO_3D_TILT = nameof(Pseudo3DInstance.Tilt);
			public const string PSEUDO_3D_DEPTH = nameof(Pseudo3DInstance.Depth);
			public const string PSEUDO_3D_LOCAL_SIZE = nameof(Pseudo3DInstance.LocalSize);
			public const string PSEUDO_3D_SIZE = nameof(Pseudo3DInstance.Size);
			public const string PSEUDO_3D_ORIGIN = nameof(Pseudo3DInstance.Origin);
			public const string PSEUDO_3D_ORIGIN_UNIT = nameof(Pseudo3DInstance.OriginUnit);

			public const string CUBE_PERSPECTIVE_UNIT = nameof(CubeInstance.PerspectiveUnit);
			public const string CUBE_SIDE_BOUNDING_BOX = nameof(CubeInstance.BoundingBox3D);
			public const string CUBE_SIDE_TOP = nameof(CubeInstance.SideTop);
			public const string CUBE_SIDE_BOTTOM = nameof(CubeInstance.SideBottom);
			public const string CUBE_SIDE_LEFT = nameof(CubeInstance.SideLeft);
			public const string CUBE_SIDE_RIGHT = nameof(CubeInstance.SideRight);
			public const string CUBE_SIDE_NEAR = nameof(CubeInstance.SideNear);
			public const string CUBE_SIDE_FAR = nameof(CubeInstance.SideFar);

			public const string SPRITE_STACK_BOUNDING_BOX = nameof(SpriteStackInstance.BoundingBox3D);
			public const string LIGHT_COLOR = nameof(LightInstance.Color);
			public const string NINE_PATCH_BORDER_SIZE = nameof(NinePatchInstance.BorderSize);
			public const string PARTICLE_MANAGER_COUNT = nameof(ParticleManagerInstance.Count);

			public const string CLOTH_HAS_THREADS = nameof(ClothInstance.HasThreads);
			public const string CLOTH_TEX_COORD_A = nameof(ClothInstance.TexCoordA);
			public const string CLOTH_TEX_COORD_B = nameof(ClothInstance.TexCoordB);
			public const string CLOTH_TEX_COORD_UNIT_A = nameof(ClothInstance.TexCoordUnitA);
			public const string CLOTH_TEX_COORD_UNIT_B = nameof(ClothInstance.TexCoordUnitB);
			public const string CLOTH_BREAK_THRESHOLD = nameof(ClothInstance.BreakThreshold);
			public const string CLOTH_FORCE = nameof(ClothInstance.Force);
			public const string CLOTH_GRAVITY = nameof(ClothInstance.Gravity);

			public const string TEXT_FONT_PATH = nameof(TextInstance.FontPath);
			public const string TEXT_VALUE = nameof(TextInstance.Value);
			public const string TEXT_COLOR = nameof(TextInstance.Color);
			public const string TEXT_SYMBOL_SIZE = nameof(TextInstance.SymbolSize);
			public const string TEXT_SYMBOL_SPACE = nameof(TextInstance.SymbolSpace);
			public const string TEXT_LINE_SPACE = nameof(TextInstance.LineSpace);
			public const string TEXT_OUTLINE_COLOR = nameof(TextInstance.OutlineColor);
			public const string TEXT_OUTLINE_SIZE = nameof(TextInstance.OutlineSize);
			public const string TEXT_STYLE = nameof(TextInstance.Style);
			public const string TEXT_ORIGIN_UNIT = nameof(TextInstance.OriginUnit);

			public const string AUDIO_PATH = nameof(AudioInstance.Path);
			public const string AUDIO_DURATION = nameof(AudioInstance.Duration);
			public const string AUDIO_STATUS = nameof(AudioInstance.Status);
			public const string AUDIO_IS_LOOPING = nameof(AudioInstance.IsLooping);
			public const string AUDIO_IS_GLOBAL = nameof(AudioInstance.IsGlobal);
			public const string AUDIO_VOLUME_UNIT = nameof(AudioInstance.VolumeUnit);
			public const string AUDIO_PITCH_UNIT = nameof(AudioInstance.PitchUnit);
			public const string AUDIO_PROGRESS_UNIT = nameof(AudioInstance.ProgressUnit);
			public const string AUDIO_PROGRESS = nameof(AudioInstance.Progress);
			public const string AUDIO_FADE = nameof(AudioInstance.DistanceFade);

			public const string TILEMAP_TILE_SIZE = nameof(TilemapInstance.TileSize);
			public const string TILEMAP_TILE_GAP = nameof(TilemapInstance.TileGap);
			public const string TILEMAP_TILE_COUNT = nameof(TilemapInstance.TileCount);
			public const string TILEMAP_TILE_PALETTE = nameof(TilemapInstance.TilePalette);

			public const string GUI_BUTTON_IS_DRAGGABLE = nameof(ButtonInstance.IsDraggable);
			public const string GUI_BUTTON_HOLD_DELAY = nameof(ButtonInstance.HoldDelay);
			public const string GUI_BUTTON_HOLD_TRIGGER_SPEED = nameof(ButtonInstance.HoldTriggerSpeed);

			public const string GUI_TEXT_BUTTON_IS_HYPERLINK = nameof(TextButtonInstance.IsHyperlink);

			public const string GUI_CHECKBOX_IS_CHECKED = nameof(CheckboxInstance.IsChecked);

			public const string GUI_PROGRESS_BAR_RANGE_A = nameof(ProgressBarInstance.RangeA);
			public const string GUI_PROGRESS_BAR_RANGE_B = nameof(ProgressBarInstance.RangeB);
			public const string GUI_PROGRESS_BAR_UNIT = nameof(ProgressBarInstance.ProgressUnit);
			public const string GUI_PROGRESS_BAR_VALUE = nameof(ProgressBarInstance.Value);
			public const string GUI_PROGRESS_BAR_MAX_LENGTH = nameof(ProgressBarInstance.MaxLength);

			public const string GUI_TEXTBOX_ALIGNMENT = nameof(TextboxInstance.Alignment);
			public const string GUI_TEXTBOX_BACKGROUND_COLOR = nameof(TextboxInstance.BackgroundColor);
			public const string GUI_TEXTBOX_SHADOW_OFFSET = nameof(TextboxInstance.ShadowOffset);
			public const string GUI_TEXTBOX_SHADOW_COLOR = nameof(TextboxInstance.ShadowColor);

			public const string GUI_INPUTBOX_IS_FOCUSED = nameof(InputboxInstance.IsFocused);
			public const string GUI_INPUTBOX_CURSOR_COLOR = nameof(InputboxInstance.CursorColor);
			public const string GUI_INPUTBOX_CURSOR_POSITION_INDEX = nameof(InputboxInstance.CursorPositionIndex);
			public const string GUI_INPUTBOX_PLACEHOLDER_VALUE = nameof(InputboxInstance.PlaceholderValue);
			public const string GUI_INPUTBOX_PLACEHOLDER_COLOR = nameof(InputboxInstance.PlaceholderColor);

			public const string GUI_SLIDER_LENGTH_UNIT = nameof(SliderInstance.LengthUnit);
			public const string GUI_SLIDER_LENGTH = nameof(SliderInstance.Length);
			public const string GUI_SLIDER_PROGRESS_COLOR = nameof(SliderInstance.ProgressColor);
			public const string GUI_SLIDER_EMPTY_COLOR = nameof(SliderInstance.EmptyColor);

			public const string GUI_SCROLL_BAR_IS_FOCUSED = nameof(ScrollBarInstance.IsFocused);
			public const string GUI_SCROLL_BAR_STEP = nameof(ScrollBarInstance.Step);
			public const string GUI_SCROLL_BAR_STEP_UNIT = nameof(ScrollBarInstance.StepUnit);
			public const string GUI_SCROLL_BAR_BUTTON_UP = nameof(ScrollBarInstance.ButtonUp);
			public const string GUI_SCROLL_BAR_BUTTON_DOWN = nameof(ScrollBarInstance.ButtonDown);

			public const string GUI_LIST_VISIBLE_ITEM_COUNT_MAX = nameof(ListInstance.VisibleItemCountMax);
			public const string GUI_LIST_VISIBLE_ITEM_COUNT_CURRENT = nameof(ListInstance.VisibleItemCountCurrent);
			public const string GUI_LIST_ITEM_WIDTH = nameof(ListInstance.ItemWidth);
			public const string GUI_LIST_ITEM_HEIGHT = nameof(ListInstance.ItemHeight);
			public const string GUI_LIST_ITEM_SPACING = nameof(ListInstance.ItemSpacing);
			public const string GUI_LIST_ITEMS = nameof(ListInstance.Items);
			public const string GUI_LIST_SCROLL_INDEX = nameof(ListInstance.ScrollIndex);

			public const string GUI_LIST_CAROUSEL_IS_REPEATING = nameof(ListCarouselInstance.IsRepeating);
			public const string GUI_LIST_CAROUSEL_SELECTION_INDEX = nameof(ListCarouselInstance.SelectionIndex);
			public const string GUI_LIST_CAROUSEL_SELECTED_ITEM = nameof(ListCarouselInstance.SelectedItem);

			public const string GUI_LIST_DROPDOWN_SELECTION_INDEX = nameof(ListDropdownInstance.SelectionIndex);
			public const string GUI_LIST_DROPDOWN_SELECTED_ITEM = nameof(ListDropdownInstance.SelectedItem);
			public const string GUI_LIST_DROPDOWN_BUTTON = nameof(ListDropdownInstance.Button);

			public const string GUI_LIST_MULTISELECT_SELECTION_INDEXES = nameof(ListMultiselectInstance.SelectionIndexes);
			public const string GUI_LIST_MULTISELECT_SELECTED_ITEMS = nameof(ListMultiselectInstance.SelectedItems);
			#endregion

			public string Name => name;
			public Type Type => type;
			public string OwnerType => ownerType;
			public bool HasSetter => hasSetter;
			public bool HasGetter => hasGetter;

			public override string ToString()
			{
				var getStr = HasGetter ? "get;" : "";
				var setStr = HasSetter ? "set;" : "";
				var sep = HasGetter && HasSetter ? " " : "";

				return $"{OwnerType} {{ {Type.GetPrettyName()} {Name} {{ {getStr}{sep}{setStr} }} }}";
			}

			public static void AddTo<T>(string uid, string propertyName, T value)
			{
				var instance = ThingInstance.GetTryError(uid);
				if(instance == null)
					return;

				if(string.IsNullOrWhiteSpace(propertyName))
				{
					Console.LogError(1, $"The '{nameof(propertyName)}' should not be empty or null.");
					return;
				}
				else if(IsOwnedBy(uid, propertyName))
				{
					Console.LogError(1, $"The {instance.GetType().GetPrettyName()} '{uid}' already contains a property with the name '{propertyName}'.");
					return;
				}

				var type = typeof(T);
				type.Whitelist();
				instance.customProperties[propertyName] = new()
				{
					Type = type,
					Value = value
				};
			}
			public static bool IsOwnedBy(string uid, string propertyName)
			{
				return GetPropInfo(uid, false, propertyName, false) != null;
			}
			public static Property GetFrom(string uid, string propertyName)
			{
				var obj = ThingInstance.GetTryError(uid);
				if(obj == null)
					return default;

				// don't even bother checking setters since all props have getters
				var get = GetPropInfo(uid, false, propertyName, false);
				if(get == null)
				{
					MissingPropError(ThingInstance.GetTryError(uid), propertyName, false);
					return default;
				}

				return get;
			}
			public static List<Property> GetAllFrom(string uid)
			{
				var names = new List<string>();
				var result = new List<Property>();
				var getters = GetPropsInfo(uid, false);
				var setters = GetPropsInfo(uid, true);

				ProcessList(getters);
				ProcessList(setters);

				return result;

				void ProcessList(List<Property> list)
				{
					for(int i = 0; i < list.Count; i++)
						if(names.Contains(list[i].ToString()) == false)
						{
							result.Add(list[i]);
							names.Add(list[i].ToString());
						}
				}
			}

			#region Backend
			internal string name, ownerType;
			internal Type type;
			internal bool hasSetter, hasGetter;

			internal Property() { }
			#endregion
		}
		public sealed class Method
		{
			#region Method Names
			public const string LOCAL_POSITION_FROM_PARENT = nameof(ThingInstance.LocalPositionFromParent);
			public const string POSITION_FROM_PARENT = nameof(ThingInstance.PositionFromParent);
			public const string LOCAL_POSITION_FROM_SELF = nameof(ThingInstance.LocalPositionFromSelf);
			public const string POSITION_FROM_SELF = nameof(ThingInstance.PositionFromSelf);
			public const string DESTROY = nameof(ThingInstance.Destroy);

			public const string CAMERA_SNAP = nameof(CameraInstance.Snap);
			public const string CAMERA_POINT_TO_CAMERA = nameof(CameraInstance.WorldToCamera);
			public const string CAMERA_POINT_TO_WORLD = nameof(CameraInstance.CameraToWorld);
			public const string CAMERA_POINT_TO_PARALLAX = nameof(CameraInstance.PointToParallax);

			public const string PARTICLE_MANAGER_SPAWN = nameof(ParticleManagerInstance.Spawn);
			public const string PARTICLE_MANAGER_DESTROY = nameof(ParticleManagerInstance.Destroy);

			public const string VISUAL_EFFECT_CODE = nameof(VisualInstance.EffectCode);
			public const string VISUAL_SET_CUSTOM_EFFECT = nameof(VisualInstance.SetCustomEffect);
			public const string VISUAL_SET_EFFECT_BOOL = nameof(VisualInstance.SetEffectBool);
			public const string VISUAL_SET_EFFECT_INT = nameof(VisualInstance.SetEffectInt);
			public const string VISUAL_SET_EFFECT_FLOAT = nameof(VisualInstance.SetEffectFloat);
			public const string VISUAL_SET_EFFECT_VECTOR2 = nameof(VisualInstance.SetEffectVector2);
			public const string VISUAL_SET_EFFECT_VECTOR3 = nameof(VisualInstance.SetEffectVector3);
			public const string VISUAL_SET_EFFECT_VECTOR4 = nameof(VisualInstance.SetEffectVector4);
			public const string VISUAL_SET_EFFECT_COLOR = nameof(VisualInstance.SetEffectColor);

			public const string TILEMAP_SET_TILE = nameof(TilemapInstance.SetTile);
			public const string TILEMAP_SET_TILE_SQUARE = nameof(TilemapInstance.SetTileSquare);
			public const string TILEMAP_HAS_TILE = nameof(TilemapInstance.HasTile);
			public const string TILEMAP_REMOVE_TILES = nameof(TilemapInstance.RemoveTiles);
			public const string TILEMAP_REMOVE_TILE_SQUARE = nameof(TilemapInstance.RemoveTileSquare);
			public const string TILEMAP_TILE_INDEXES = nameof(TilemapInstance.TileIndexes);
			public const string TILEMAP_TILE_POSITION = nameof(TilemapInstance.TilePosition);
			public const string TILEMAP_PALETTE_UIDS_FROM_POSITION = nameof(TilemapInstance.PaletteUIDsFromPosition);
			public const string TILEMAP_PALETTE_UIDS_FROM_TILE_INDEXES = nameof(TilemapInstance.PaletteUIDsFromTileIndexes);

			public const string CLOTH_CUT = nameof(ClothInstance.Cut);
			public const string CLOTH_PIN = nameof(ClothInstance.Pin);

			public const string UI_BUTTON_TRIGGER = nameof(ButtonInstance.Trigger);

			public const string UI_TEXTBOX_GET_SYMBOL_CORNERS = nameof(TextboxInstance.SymbolCorners);
			public const string UI_TEXTBOX_GET_SYMBOL_INDEX = nameof(TextboxInstance.SymbolIndex);
			public const string UI_TEXTBOX_GET_SYMBOLS = nameof(TextboxInstance.Symbols);

			public const string UI_INPUTBOX_SUBMIT = nameof(InputboxInstance.Submit);

			public const string UI_SCROLL_BAR_GO_UP = nameof(ScrollBarInstance.GoUp);
			public const string UI_SCROLL_BAR_GO_DOWN = nameof(ScrollBarInstance.GoDown);
			#endregion

			public sealed class Parameter
			{
				public string Name => name;
				public Type Type => type;
				public Method Owner => owner;

				public override string ToString()
				{
					return $"{Type.GetPrettyName()} {Name}";
				}

				#region Backend
				internal string name;
				internal Type type;
				internal Method owner;

				internal Parameter() { }
				#endregion
			}

			public string Name => name;
			public Type ReturnType => returnType;
			public string OwnerType => ownerType;
			public ReadOnlyCollection<Parameter> Parameters => parameters.AsReadOnly();

			public override string ToString()
			{
				var paramStr = "";
				var returnTypeStr = ReturnType == null ? "void" : ReturnType.GetPrettyName();
				for(int i = 0; i < parameters.Count; i++)
					paramStr += (i == 0 ? "" : ", ") + parameters[i].ToString();
				return $"{OwnerType} {{ {returnTypeStr} {Name}({paramStr}) }}";
			}

			public static bool IsOwnedBy(string uid, string methodName)
			{
				return GetFrom(uid, methodName) != null;
			}
			public static Method GetFrom(string uid, string methodName)
			{
				var rtrn = GetMethodInfo(uid, false, methodName, false);
				var vd = GetMethodInfo(uid, true, methodName, false);

				if(rtrn == null && vd == null)
				{
					MissingMethodError(ThingInstance.GetTryError(uid), methodName, false, true);
					return default;
				}

				return rtrn ?? vd;
			}
			public static List<Method> GetAllFrom(string uid)
			{
				var names = new List<string>();
				var result = new List<Method>();
				var voids = GetMethodsInfo(uid, true);
				var returns = GetMethodsInfo(uid, false);

				ProcessList(voids);
				ProcessList(returns);

				return result;

				void ProcessList(List<Method> list)
				{
					for(int i = 0; i < list.Count; i++)
						if(names.Contains(list[i].ToString()) == false)
						{
							result.Add(list[i]);
							names.Add(list[i].ToString());
						}
				}
			}

			#region Backend
			internal string name, ownerType;
			internal Type returnType;
			internal List<Parameter> parameters = new();

			internal Method() { }
			#endregion
		}
		public static class GUI
		{
			public class ListItem
			{
				public ButtonDetails ButtonDetails { get; } = new();
				public TextDetails TextDetails { get; } = new();
			}
			public class ButtonDetails
			{
				public bool IsDisabled { get; set; }
				public bool IsHidden { get; set; }
				public bool IsSmooth { get; set; }
				public bool IsRepeated { get; set; } = true;

				public virtual string TexturePath { get; set; }
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
				internal readonly Hitbox boundingBox = new();

				internal Texture GetTexture()
				{
					var textures = Scene.CurrentScene.Textures;
					var path = TexturePath.ToBackslashPath();
					return path != null && textures.ContainsKey(path) ? textures[path] : Game.defaultTexture;
				}
				internal void Draw(RenderTarget renderTarget)
				{
					if(IsHidden)
						return;

					var tex = GetTexture();
					var w = tex == null ? 0 : tex.Size.X;
					var h = tex == null ? 0 : tex.Size.Y;
					var w0 = w * TexCoordUnitA.X;
					var ww = w * TexCoordUnitB.X;
					var h0 = h * TexCoordUnitA.Y;
					var hh = h * TexCoordUnitB.Y;

					var lines = boundingBox.Lines;
					var verts = new Vertex[]
					{
						new(lines[0].A.ToSFML(), Tint, new(w0, h0)),
						new(lines[1].A.ToSFML(), Tint, new(ww, h0)),
						new(lines[2].A.ToSFML(), Tint, new(ww, hh)),
						new(lines[3].A.ToSFML(), Tint, new(w0, hh)),
					};

					renderTarget.Draw(verts, PrimitiveType.Quads, new(tex));
				}
				#endregion
			}
			public class TextDetails
			{
				public bool IsHidden { get; set; }
				public string FontPath { get; set; }
				public string Value { get; set; } = "Hello, World!";
				public Color Color { get; set; } = Color.White;
				public int SymbolSize
				{
					get => symbolSize;
					set => symbolSize = value.Limit(0, 2048, Extensions.Limitation.Overflow);
				}
				public float SymbolSpace { get; set; } = 1;
				public float LineSpace { get; set; } = 1;
				public Color OutlineColor { get; set; } = Color.Black;
				public float OutlineSize { get; set; } = 1;
				public Text.Styles Style { get; set; }
				public Vector2 OriginUnit { get; set; } = new(0.5f);
				public float Scale { get; set; } = 1;

				#region Backend
				private int symbolSize = 32;

				internal void Draw(RenderTarget renderTarget)
				{
					if(IsHidden || GetFont() == null)
						return;

					renderTarget.Draw(TextInstance.textInstance);
				}

				internal void UpdateGlobalText(Vector2 position, float angle, float scale)
				{
					var text = TextInstance.textInstance;
					text.Font = GetFont();
					text.CharacterSize = (uint)SymbolSize.Limit(0, int.MaxValue);
					text.FillColor = Color;
					text.LetterSpacing = SymbolSpace;
					text.LineSpacing = LineSpace;
					text.OutlineColor = OutlineColor;
					text.OutlineThickness = OutlineSize;
					text.Style = Style;
					text.DisplayedString = Value;
					text.Position = position.ToSFML();
					text.Rotation = angle;
					text.Scale = new(scale * Scale, scale * Scale);

					var local = text.GetLocalBounds(); // has to be after everything
					text.Origin = new(local.Width * OriginUnit.X, local.Height * OriginUnit.Y * 1.4f);
				}
				private Font GetFont()
				{
					var fonts = Scene.CurrentScene.Fonts;
					var path = FontPath.ToBackslashPath();
					return path != null && fonts.ContainsKey(path) ? fonts[path] : null;
				}
				#endregion
			}

			public enum TextboxAlignment
			{
				TopLeft, Top, TopRight,
				Left, Center, Right,
				BottomLeft, Bottom, BottomRight
			}
			public enum TextboxSymbolCollection { Character, Word, Line }

			public static string CreateButton(string uid, string texturePath)
			{
				var t = new ButtonInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateCheckbox(string uid, string texturePath)
			{
				var t = new CheckboxInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateProgressBar(string uid, string texturePath)
			{
				var t = new ProgressBarInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateTextButton(string uid, string texturePath, string fontPath, string value = "Hello, World!")
			{
				var t = new TextButtonInstance(uid) { TexturePath = texturePath };
				t.TextDetails.FontPath = fontPath;
				t.TextDetails.Value = value;
				return t.UID;
			}
			public static string CreateTextbox(string uid, string fontPath, string value = "Hello, World!",
				uint resolutionX = 200, uint resolutionY = 200)
			{
				var t = new TextboxInstance(uid, resolutionX, resolutionY) { FontPath = fontPath, Value = value };
				return t.UID;
			}
			public static string CreateInputbox(string uid, string fontPath,
				uint resolutionX = 300, uint resolutionY = 60)
			{
				var t = new InputboxInstance(uid, resolutionX, resolutionY) { FontPath = fontPath };
				return t.UID;
			}
			public static string CreateSlider(string uid, string texturePath)
			{
				var t = new SliderInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateScrollBar(string uid, string texturePath)
			{
				var t = new ScrollBarInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateList(string uid, string texturePath)
			{
				var t = new ListInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateListCarousel(string uid)
			{
				var t = new ListCarouselInstance(uid);
				return t.UID;
			}
			public static string CreateListDropdown(string uid, string texturePath)
			{
				var t = new ListDropdownInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateListMultiselect(string uid, string texturePath)
			{
				var t = new ListMultiselectInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
			public static string CreateListGrid(string uid, string texturePath)
			{
				var t = new GridLayoutInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
		}

		public static List<string> UIDs => Scene.CurrentScene.objs.Keys.ToList();
		public static List<string> Tags
		{
			get
			{
				var tags = new List<string>();
				var objs = Scene.CurrentScene.objs;
				foreach(var kvp in objs)
					for(int i = 0; i < kvp.Value.Tags.Count; i++)
					{
						var tag = kvp.Value.Tags[i];
						if(tags.Contains(tag))
							continue;

						tags.Add(tag);
					}
				return tags;
			}
		}

		public static List<string> GetUIDsByTag(string tag)
		{
			var uids = new List<string>();
			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
				if(kvp.Value.Tags.Contains(tag))
					uids.Add(kvp.Key);
			return uids;
		}
		public static string GetFreeUID(string uid)
		{
			if(Scene.CurrentScene == null)
				return uid;

			var i = 1;

			if(string.IsNullOrWhiteSpace(uid))
				uid = nameof(Thing);

			var freeUID = uid;

			var objs = Scene.CurrentScene.objs;
			while(objs.ContainsKey(freeUID))
			{
				var lastSymbol = freeUID[^1].ToString();

				if(lastSymbol.IsNumber())
				{
					freeUID = $"{freeUID[..^1]}{i}";
					i++;
					continue;
				}

				freeUID = $"{uid}{i}";
				i++;
			}
			return freeUID;
		}

		public static bool Exists(string uid)
		{
			return string.IsNullOrWhiteSpace(uid) == false && Scene.CurrentScene.objs.ContainsKey(uid);
		}
		public static string Duplicate(string uid, string newUID)
		{
			var thing = ThingInstance.Get_(uid);
			var prevUID = uid;
			var prevOldUID = thing.oldUID;
			var children = new List<string>(thing.childrenUIDs);

			thing.UID = GetFreeUID(newUID);

			var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
			var json = JsonConvert.SerializeObject(thing, settings);

			thing.UID = prevUID;
			thing.oldUID = prevOldUID;
			var newThing = JsonConvert.DeserializeObject<ThingInstance>(json, settings);
			newThing.numUID = ThingInstance.currNumUID;
			ThingInstance.currNumUID++;

			for(int i = 0; i < children.Count; i++)
			{
				var child = ThingInstance.Get_(children[i]);
				child.ParentUID = prevUID;
			}

			return newThing.UID;
		}
		public static void Destroy(string uid, bool destroyChildren)
		{
			if(uid == Scene.MAIN_CAMERA_UID)
				return;

			var obj = ThingInstance.Get_(uid);
			if(obj == null)
				return;

			obj.Destroy(destroyChildren);
		}

		public static string CreateAudio(string uid, string filePath)
		{
			var t = new AudioInstance(uid) { Path = filePath };
			return t.UID;
		}
		public static string CreateNinePatch(string uid, string texturePath)
		{
			var t = new NinePatchInstance(uid) { TexturePath = texturePath };
			return t.UID;
		}
		public static string CreateSprite(string uid, string texturePath)
		{
			var t = new SpriteInstance(uid) { TexturePath = texturePath };
			return t.UID;
		}
		public static string CreateSpriteStack(string uid, string textureStack)
		{
			var t = new SpriteStackInstance(uid) { TexturePath = textureStack };
			return t.UID;
		}
		public static string CreateCube(string uid, string texturePath)
		{
			var t = new CubeInstance(uid) { TexturePath = texturePath };
			return t.UID;
		}
		public static string CreateLight(string uid, Color color)
		{
			var t = new LightInstance(uid) { Color = color };
			return t.UID;
		}
		public static string CreateCamera(string uid, Vector2 resolution)
		{
			var t = new CameraInstance(uid, resolution);
			return t.UID;
		}
		public static string CreateText(string uid, string fontPath, string value = "Hello, World!")
		{
			var t = new TextInstance(uid) { FontPath = fontPath, Value = value };
			return t.UID;
		}
		public static string CreateTilemap(string uid, string texturePath, float tileWidth = 32, float tileHeight = 32)
		{
			var t = new TilemapInstance(uid) { TexturePath = texturePath, TileSize = new(tileWidth, tileHeight) };
			return t.UID;
		}
		public static string CreateCloth(string uid, string texturePath, float width = 300, float height = 300,
			int quadCountX = 5, int quadCountY = 5)
		{
			var t = new ClothInstance(uid, new(width, height), new(quadCountX, quadCountY)) { TexturePath = texturePath };
			return t.UID;
		}
		public static string CreateParticleManager(string uid, string texturePath)
		{
			var t = new ParticleManagerInstance(uid) { TexturePath = texturePath };
			return t.UID;
		}

		public static void Set(string uid, string setPropertyName, object value)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null || value == null)
				return;
			var type = obj.GetType();

			if(obj.customProperties.ContainsKey(setPropertyName))
			{
				obj.customProperties[setPropertyName].Value = value;
				return;
			}

			TryAddAllProps(type, true);

			var key = (type, setPropertyName);
			if(setters.ContainsKey(key) == false)
			{
				if(settersAllNames[type].Contains(setPropertyName))
					setters[key] = type.DelegateForSetPropertyValue(setPropertyName);
				else
				{
					MissingPropError(obj, setPropertyName, true);
					return;
				}
			}

			var valueType = value.GetType();
			var t = setterTypes[key];
			if(valueType != t)
			{
				PropTypeMismatchError(obj, setPropertyName, valueType, t);
				return;
			}

			if(setters.ContainsKey(key))
				setters[key].Invoke(obj, value);
		}
		public static T Get<T>(string uid, string getPropertyName)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;

			var type = obj.GetType();
			var key = (type, getPropertyName);

			TryAddAllProps(type, false);

			var result = GetResult(out var success);
			var prop = Property.GetFrom(uid, getPropertyName);

			if(success == false && prop != null)
			{
				ReturnTypeMismatchError(false, obj, getPropertyName, typeof(T), prop.type);
				return default;
			}
			return result;

			T GetResult(out bool success)
			{
				success = false;

				if(obj.customProperties.ContainsKey(getPropertyName))
				{
					var prop = obj.customProperties[getPropertyName];
					if(prop.Type == typeof(T))
					{
						success = true;
						return (T)prop.Value;
					}
				}
				else if(getters.ContainsKey(key) == false)
				{
					if(gettersAllNames[type].Contains(getPropertyName))
						getters[key] = type.DelegateForGetPropertyValue(getPropertyName);
					else
						return default;
				}

				if(getters.ContainsKey(key) == false)
					return default;

				var value = getters[key].Invoke(obj);
				var valueIsNull = value == null; // some properties can be null, null is not T
				if(valueIsNull || value.GetType() == typeof(T))
				{
					success = true;
					return (T)value;
				}
				return default;
			}
		}
		public static void Do(string uid, string voidMethodName, params object[] parameters)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return;
			var type = obj.GetType();
			var key = (type, voidMethodName);

			if(voidMethods.ContainsKey(key) == false)
			{
				TryAddAllMethods(type, true);

				if(voidMethodsAllNames[type].Contains(voidMethodName))
				{
					var p = type.GetMethod(voidMethodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					voidMethods[key] = type.DelegateForCallMethod(voidMethodName, paramTypes.ToArray());
					voidMethodParamTypes[key] = paramTypes;
				}
				else
					MissingMethodError(obj, voidMethodName, true);
			}

			if(voidMethods.ContainsKey(key) && TryTypeMismatchError(obj, key, voidMethodParamTypes[key], parameters.ToArray()) == false)
				voidMethods[key].Invoke(obj, parameters);
		}
		public static T DoGet<T>(string uid, string getMethodName, params object[] parameters)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();
			var key = (type, getMethodName);

			if(returnMethods.ContainsKey(key) == false)
			{
				TryAddAllMethods(type, false);

				if(returnMethodsAllNames[type].Contains(getMethodName))
				{
					var p = type.GetMethod(getMethodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					returnMethods[key] = type.DelegateForCallMethod(getMethodName, paramTypes.ToArray());
					returnMethodParamTypes[key] = paramTypes;
				}
				else
				{
					MissingMethodError(obj, getMethodName, false);
					return default;
				}
			}

			var paramLength = parameters == null ? 1 : parameters.Length;
			if(returnMethodParamTypes.ContainsKey(key) == false || // no params found on that method
				returnMethodParamTypes[key].Count != paramLength || // provided params are more or less than method params
				TryTypeMismatchError(obj, key, returnMethodParamTypes[key], parameters)) // provided params are different types from method params
				return default;

			var result = returnMethods[key].Invoke(obj, parameters ?? new object[1]); // possible null parameter, not null params array
			if(result is T t)
				return t;

			ReturnTypeMismatchError(true, obj, getMethodName, typeof(T), returnMethodTypes[key]);
			return default;
		}

		#region Backend
		internal class CustomProperty
		{
			public Type Type { get; set; }
			public object Value { get; set; }
		}

		internal readonly static Dictionary<Type, List<string>> settersAllNames = new(), gettersAllNames = new(),
			voidMethodsAllNames = new(), returnMethodsAllNames = new();

		internal readonly static Dictionary<(Type, string), List<string>> returnMethodParamNames = new(), voidMethodParamNames = new();
		internal readonly static Dictionary<(Type, string), Type> setterTypes = new(), getterTypes = new(), returnMethodTypes = new();
		internal readonly static Dictionary<(Type, string), List<Type>> returnMethodParamTypes = new(), voidMethodParamTypes = new();

		internal readonly static Dictionary<(Type, string), MemberSetter> setters = new();
		internal readonly static Dictionary<(Type, string), MemberGetter> getters = new();
		internal readonly static Dictionary<(Type, string), Fasterflect.MethodInvoker> voidMethods = new(), returnMethods = new();

		internal static void TryAddAllProps(Type type, bool setter)
		{
			var allNames = setter ? settersAllNames : gettersAllNames;
			var allTypes = setter ? setterTypes : getterTypes;

			var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			var propNames = new List<string>();
			for(int i = 0; i < props.Length; i++)
			{
				if((props[i].CanWrite == false && setter) || (props[i].CanRead == false && setter == false))
					continue;

				var name = props[i].Name;
				propNames.Add(name);
				allTypes[(type, name)] = props[i].PropertyType;
			}
			allNames[type] = propNames;
		}
		internal static void TryAddAllMethods(Type type, bool onlyVoid)
		{
			var allNames = onlyVoid ? voidMethodsAllNames : returnMethodsAllNames;
			var allParamTypes = onlyVoid ? voidMethodParamTypes : returnMethodParamTypes;
			var allParamNames = onlyVoid ? voidMethodParamNames : returnMethodParamNames;

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			var methodNames = new List<string>();
			for(int i = 0; i < methods.Length; i++)
			{
				if(methods[i].IsSpecialName || methods[i].DeclaringType == typeof(object) ||
					methods[i].Name == "ToString" ||
					(methods[i].ReturnType != typeof(void) && onlyVoid) ||
					(methods[i].ReturnType == typeof(void) && onlyVoid == false))
					continue;

				var name = methods[i].Name;
				methodNames.Add(name);

				if(onlyVoid == false)
					returnMethodTypes[(type, name)] = methods[i].ReturnType;

				var parameters = methods[i].GetParameters();
				allParamTypes[(type, name)] = new();
				allParamNames[(type, name)] = new();
				for(int j = 0; j < parameters.Length; j++)
				{
					allParamTypes[(type, name)].Add(parameters[j].ParameterType);
					allParamNames[(type, name)].Add(parameters[j].Name);
				}
			}
			allNames[type] = methodNames;
		}
		internal static string GetAllProps(string uid, bool setter)
		{
			var all = GetPropsInfo(uid, setter);
			var result = "";

			for(int i = 0; i < all.Count; i++)
			{
				var sep = i == 0 ? "" : "\n";
				result += $"{sep}{all[i]}";
			}

			return result;
		}
		internal static string GetAllMethods(string uid, bool onlyVoid)
		{
			var all = GetMethodsInfo(uid, onlyVoid);
			var result = "";

			for(int i = 0; i < all.Count; i++)
			{
				var sep = i == 0 ? "" : "\n";
				result += $"{sep}{all[i]}";
			}

			return result;
		}

		internal static List<Property> GetPropsInfo(string uid, bool set)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return new();

			var type = obj.GetType();

			TryAddAllProps(type, true);
			TryAddAllProps(type, false);

			var result = new List<Property>();
			var allNames = set ? settersAllNames[type] : gettersAllNames[type];

			for(int i = 0; i < allNames.Count; i++)
				result.Add(GetPropInfo(uid, set, allNames[i], false));
			foreach(var kvp in obj.customProperties)
				result.Add(GetPropInfo(uid, set, kvp.Key, false));

			return result;
		}
		internal static List<Method> GetMethodsInfo(string uid, bool isVoid)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return new();

			var type = obj.GetType();

			TryAddAllMethods(type, isVoid);

			var result = new List<Method>();
			var allNames = isVoid ? voidMethodsAllNames[type] : returnMethodsAllNames[type];

			for(int i = 0; i < allNames.Count; i++)
				result.Add(GetMethodInfo(uid, isVoid, allNames[i], false));

			return result;
		}
		internal static Property GetPropInfo(string uid, bool set, string propName, bool error = true)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();

			if(obj.customProperties.ContainsKey(propName))
				return new()
				{
					hasGetter = true,
					hasSetter = true,
					name = propName,
					ownerType = obj.GetType().GetPrettyName(),
					type = obj.customProperties[propName].Type
				};

			TryAddAllProps(type, true);
			TryAddAllProps(type, false);

			var allNames = set ? settersAllNames : gettersAllNames;
			if(allNames[type].Contains(propName) == false)
			{
				if(error)
					MissingPropError(obj, propName, set);
				return default;
			}

			var key = (type, propName);
			return new Property()
			{
				hasGetter = set == false || gettersAllNames[type].Contains(propName),
				hasSetter = set || settersAllNames[type].Contains(propName),
				name = propName,
				ownerType = type.GetPrettyName(),
				type = set ? setterTypes[key] : getterTypes[key]
			};
		}
		internal static Method GetMethodInfo(string uid, bool isVoid, string methodName, bool error = true)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();

			TryAddAllMethods(type, isVoid);

			var allNames = isVoid ? voidMethodsAllNames : returnMethodsAllNames;
			if(allNames[type].Contains(methodName) == false)
			{
				if(error)
					Thing.MissingMethodError(obj, methodName, isVoid);
				return default;
			}

			var key = (type, methodName);
			var paramTypes = isVoid ? voidMethodParamTypes[key] : returnMethodParamTypes[key];
			var paramNames = new List<string>(isVoid ? voidMethodParamNames[key] : returnMethodParamNames[key]);

			var result = new Method()
			{
				name = methodName,
				returnType = isVoid ? null : returnMethodTypes[key],
				ownerType = type.GetPrettyName(),
			};
			for(int j = 0; j < paramNames.Count; j++)
				result.parameters.Add(new() { name = paramNames[j], owner = result, type = paramTypes[j] });
			return result;
		}

		internal static void DestroyAll()
		{
			if(Scene.CurrentScene == null)
				return;

			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
				if(kvp.Value != Scene.MainCamera)
					kvp.Value.Destroy(true);
		}

		internal static bool TryTypeMismatchError(ThingInstance obj, (Type, string) key, List<Type> paramTypes, object[] parameters)
		{
			if(parameters == null || parameters.Length == 0)
				return false;

			var nth = new string[] { "st", "nd", "rd" };
			var method = Method.GetFrom(obj.UID, key.Item2).ToString().Replace("Instance", "");

			if(paramTypes.Count != parameters.Length)
			{
				Console.LogError(2, $"The method\n{method}\ncannot process its parameters.",
					$"It expects {paramTypes.Count} parameters, not {parameters.Length}.");
				return true;
			}

			for(int i = 0; i < paramTypes.Count; i++)
			{
				var p = parameters[i];
				var expectedType = paramTypes[i];
				var type = p.GetType();
				if((p == null && expectedType.IsClass == false) ||
					(p != null && type != expectedType))
				{
					var nthStr = i < 4 ? nth[i] : "th";
					var paramStr = p == null ? "null" : p.GetType().GetPrettyName(true);
					Console.LogError(2, $"The method\n{method}\ncannot process the value of its {i + 1}{nthStr} parameter.",
						$"It expects a value of type {paramTypes[i].GetPrettyName()}, not {paramStr}.");
					return true;
				}
			}
			return false;
		}
		internal static void MissingPropError(ThingInstance obj, string propertyName, bool set, bool skipGetSet = false)
		{
			var prop = set ? "setter property" : "getter property";
			var props = set ? "setter properties" : "getter properties";

			if(skipGetSet)
			{
				prop = "property";
				props = "properties";
			}

			Console.LogError(2, $"The {obj.GetType().GetPrettyName()} '{obj.UID}' does not have a {prop} '{propertyName}'.",
				$"It has the following {props}:\n{GetAllProps(obj.UID, set)}");
		}
		internal static void MissingMethodError(ThingInstance obj, string methodName, bool isVoid, bool skipVoidReturn = false)
		{
			var voidStr = isVoid ? "void " : "return ";

			if(skipVoidReturn)
				voidStr = "";

			Console.LogError(2, $"{obj} does not have a {voidStr}method '{methodName}'.",
				$"It has the following {voidStr}methods:\n{GetAllMethods(obj.UID, isVoid)}");
		}
		internal static void PropTypeMismatchError(ThingInstance obj, string propertyName, Type valueType, Type expectedValueType)
		{
			var prop = Property.GetFrom(obj.UID, propertyName);
			Console.LogError(1, $"The property\n{prop}\ncannot process the provided value.",
				$"It expects a value of type {expectedValueType.GetPrettyName(true)}, not {valueType.GetPrettyName(true)}.");
		}
		internal static void ReturnTypeMismatchError(bool isMethod, ThingInstance obj, string memberName, Type valueType, Type expectedValueType)
		{
			object member = isMethod ? Method.GetFrom(obj.UID, memberName) : Property.GetFrom(obj.UID, memberName);
			var memberStr = isMethod ? "method" : "property";
			Console.LogError(1, $"The {memberStr}\n{member}\ncannot process the provided return type.",
				$"It expects the type {expectedValueType.GetPrettyName(true)}, not {valueType.GetPrettyName(true)}.");
		}
		#endregion
	}
}
