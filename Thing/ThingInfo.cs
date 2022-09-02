namespace SMPL
{
	public static partial class Thing
	{
		public class Property
		{
			#region Property Names
			public const string IS_DISABLED = nameof(ThingInstance.IsDisabled);
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
			public const string CAMERA_MOUSE_CURSOR_POSITION = nameof(CameraInstance.MouseCursorPosition);
			public const string CAMERA_RENDER_TEXTURE = nameof(CameraInstance.RenderTexture);
			public const string CAMERA_RESOLUTION = nameof(CameraInstance.Resolution);

			public const string VISUAL_TINT = nameof(VisualInstance.Tint);
			public const string VISUAL_IS_HIDDEN = nameof(VisualInstance.IsHidden);
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

			public const string UI_BUTTON_IS_DRAGGABLE = nameof(ButtonInstance.IsDraggable);
			public const string UI_BUTTON_HOLD_DELAY = nameof(ButtonInstance.HoldDelay);
			public const string UI_BUTTON_HOLD_TRIGGER_SPEED = nameof(ButtonInstance.HoldTriggerSpeed);

			public const string UI_TEXT_BUTTON_IS_HYPERLINK = nameof(TextButtonInstance.IsHyperlink);
			public const string UI_TEXT_BUTTON_TEXT_UID = nameof(TextButtonInstance.TextUID);

			public const string UI_CHECKBOX_IS_CHECKED = nameof(CheckboxInstance.IsChecked);

			public const string UI_PROGRESS_BAR_RANGE_A = nameof(ProgressBarInstance.RangeA);
			public const string UI_PROGRESS_BAR_RANGE_B = nameof(ProgressBarInstance.RangeB);
			public const string UI_PROGRESS_BAR_UNIT = nameof(ProgressBarInstance.ProgressUnit);
			public const string UI_PROGRESS_BAR_VALUE = nameof(ProgressBarInstance.Value);
			public const string UI_PROGRESS_BAR_MAX_LENGTH = nameof(ProgressBarInstance.MaxLength);

			public const string UI_TEXTBOX_LINE_SPACE = nameof(TextboxInstance.LineSpace);
			public const string UI_TEXTBOX_ALIGNMENT = nameof(TextboxInstance.Alignment);
			public const string UI_TEXTBOX_BACKGROUND_COLOR = nameof(TextboxInstance.BackgroundColor);
			public const string UI_TEXTBOX_SHADOW_OFFSET = nameof(TextboxInstance.ShadowOffset);
			public const string UI_TEXTBOX_SHADOW_COLOR = nameof(TextboxInstance.ShadowColor);
			public const string UI_TEXTBOX_CAMERA_UID = nameof(TextboxInstance.CameraUID);

			public const string UI_INPUTBOX_IS_FOCUSED = nameof(InputboxInstance.IsFocused);
			public const string UI_INPUTBOX_CURSOR_COLOR = nameof(InputboxInstance.CursorColor);
			public const string UI_INPUTBOX_CURSOR_POSITION_INDEX = nameof(InputboxInstance.CursorPositionIndex);
			public const string UI_INPUTBOX_PLACEHOLDER_VALUE = nameof(InputboxInstance.PlaceholderValue);
			public const string UI_INPUTBOX_PLACEHOLDER_COLOR = nameof(InputboxInstance.PlaceholderColor);

			public const string UI_SLIDER_LENGTH_UNIT = nameof(SliderInstance.LengthUnit);
			public const string UI_SLIDER_LENGTH = nameof(SliderInstance.Length);
			public const string UI_SLIDER_PROGRESS_COLOR = nameof(SliderInstance.ProgressColor);
			public const string UI_SLIDER_EMPTY_COLOR = nameof(SliderInstance.EmptyColor);

			public const string UI_SCROLL_BAR_IS_FOCUSED = nameof(ScrollBarInstance.IsFocused);
			public const string UI_SCROLL_BAR_STEP = nameof(ScrollBarInstance.Step);
			public const string UI_SCROLL_BAR_STEP_UNIT = nameof(ScrollBarInstance.StepUnit);
			public const string UI_SCROLL_BAR_BUTTON_UP_UID = nameof(ScrollBarInstance.ButtonUpUID);
			public const string UI_SCROLL_BAR_BUTTON_DOWN_UID = nameof(ScrollBarInstance.ButtonDownUID);

			public const string UI_LIST_BUTTONS_TAG = nameof(ListInstance.ButtonsTag);
			public const string UI_LIST_VISIBLE_BUTTON_COUNT_MAX = nameof(ListInstance.VisibleButtonCountMax);
			public const string UI_LIST_VISIBLE_BUTTON_COUNT_CURRENT = nameof(ListInstance.VisibleButtonCountCurrent);
			public const string UI_LIST_BUTTON_WIDTH = nameof(ListInstance.ButtonWidth);
			public const string UI_LIST_BUTTON_HEIGHT = nameof(ListInstance.ButtonHeight);
			public const string UI_LIST_BUTTON_SPACING = nameof(ListInstance.ButtonSpacing);
			public const string UI_LIST_SCROLL_INDEX = nameof(ListInstance.ScrollIndex);

			public const string UI_LIST_CAROUSEL_IS_REPEATING = nameof(ListCarouselInstance.IsRepeating);
			public const string UI_LIST_CAROUSEL_SELECTION_INDEX = nameof(ListCarouselInstance.SelectionIndex);
			public const string UI_LIST_CAROUSEL_SELECTION_UID = nameof(ListCarouselInstance.SelectionUID);

			public const string UI_LIST_DROPDOWN_BUTTON_SHOW_UID = nameof(ListDropdownInstance.ButtonShowUID);
			public const string UI_LIST_DROPDOWN_SELECTION_UID = nameof(ListDropdownInstance.SelectionUID);
			public const string UI_LIST_DROPDOWN_SELECTION_INDEX = nameof(ListDropdownInstance.SelectionIndex);

			public const string UI_LIST_MULTISELECT_SELECTION_INDEXES = nameof(ListMultiselectInstance.SelectionIndexes);
			public const string UI_LIST_MULTISELECT_SELECTION_UIDS = nameof(ListMultiselectInstance.SelectionUIDs);
			#endregion

			public string Name { get; internal set; }
			public string Type { get; internal set; }
			public string OwnerType { get; internal set; }
			public bool HasSetter { get; internal set; }
			public bool HasGetter { get; internal set; }

			public override string ToString()
			{
				var getStr = HasGetter ? "get;" : "";
				var setStr = HasSetter ? "set;" : "";
				var sep = HasGetter && HasSetter ? " " : "";

				return $"{OwnerType} {{ {Type} {Name} {{ {getStr}{sep}{setStr} }} }}";
			}
		}
		public class Method
		{
			#region Method Names
			public const string GET_LOCAL_POSITION_FROM_PARENT = nameof(ThingInstance.GetLocalPositionFromParent);
			public const string GET_POSITION_FROM_PARENT = nameof(ThingInstance.GetPositionFromParent);
			public const string GET_LOCAL_POSITION_FROM_SELF = nameof(ThingInstance.GetLocalPositionFromSelf);
			public const string GET_POSITION_FROM_SELF = nameof(ThingInstance.GetPositionFromSelf);
			public const string DESTROY = nameof(ThingInstance.Destroy);

			public const string CAMERA_SNAP = nameof(CameraInstance.Snap);
			public const string CAMERA_POINT_TO_CAMERA = nameof(CameraInstance.PointToCamera);
			public const string CAMERA_POINT_TO_WORLD = nameof(CameraInstance.PointToWorld);
			public const string CAMERA_POINT_TO_PARALLAX = nameof(CameraInstance.PointToParallax);

			public const string PARTICLE_MANAGER_SPAWN = nameof(ParticleManagerInstance.Spawn);
			public const string PARTICLE_MANAGER_DESTROY = nameof(ParticleManagerInstance.Destroy);

			public const string VISUAL_GET_EFFECT_CODE = nameof(VisualInstance.GetEffectCode);
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
			public const string TILEMAP_GET_TILE_INDEXES = nameof(TilemapInstance.GetTileIndexes);
			public const string TILEMAP_GET_TILE_POSITION = nameof(TilemapInstance.GetTilePosition);
			public const string TILEMAP_GET_PALETTE_UIDS_FROM_POSITION = nameof(TilemapInstance.GetPaletteUIDsFromPosition);
			public const string TILEMAP_GET_PALETTE_UIDS_FROM_TILE_INDEXES = nameof(TilemapInstance.GetPaletteUIDsFromTileIndexes);

			public const string CLOTH_CUT = nameof(ClothInstance.Cut);
			public const string CLOTH_PIN = nameof(ClothInstance.Pin);

			public const string UI_BUTTON_TRIGGER = nameof(ButtonInstance.Trigger);

			public const string UI_TEXTBOX_GET_SYMBOL_CORNERS = nameof(TextboxInstance.GetSymbolCorners);
			public const string UI_TEXTBOX_GET_SYMBOL_INDEX = nameof(TextboxInstance.GetSymbolIndex);
			public const string UI_TEXTBOX_GET_SYMBOLS = nameof(TextboxInstance.GetSymbols);

			public const string UI_INPUTBOX_SUBMIT = nameof(InputboxInstance.Submit);

			public const string UI_SCROLL_BAR_GO_UP = nameof(ScrollBarInstance.GoUp);
			public const string UI_SCROLL_BAR_GO_DOWN = nameof(ScrollBarInstance.GoDown);
			#endregion

			public class Parameter
			{
				public string Name { get; internal set; }
				public string Type { get; internal set; }
				public Method Owner { get; internal set; }

				public override string ToString()
				{
					return $"{Type} {Name}";
				}
			}
			internal List<Parameter> parameters = new();

			public string Name { get; internal set; }
			public string ReturnType { get; internal set; }
			public string OwnerType { get; internal set; }
			public List<Parameter> Parameters => new(parameters);

			public override string ToString()
			{
				var paramStr = "";
				var returnTypeStr = ReturnType ?? "void";
				for(int i = 0; i < parameters.Count; i++)
					paramStr += (i == 0 ? "" : ", ") + parameters[i].ToString();
				return $"{OwnerType} {{ {returnTypeStr} {Name}({paramStr}) }}";
			}
		}
		public static class Info
		{
			public static bool HasSetter(string uid, string setPropertyName)
			{
				var obj = ThingInstance.Get(uid);
				if(obj == null)
				{
					ThingInstance.MissingError(uid);
					return default;
				}
				var type = obj.GetType();
				TryAddAllProps(type, true);

				return settersAllNames.ContainsKey(type) && settersAllNames[type].Contains(setPropertyName);
			}
			public static bool HasGetter(string uid, string getPropertyName)
			{
				var obj = ThingInstance.Get(uid);
				if(obj == null)
				{
					ThingInstance.MissingError(uid);
					return default;
				}
				var type = obj.GetType();
				TryAddAllProps(type, false);

				return gettersAllNames.ContainsKey(type) && gettersAllNames[type].Contains(getPropertyName);
			}
			public static bool HasReturnMethod(string uid, string returnMethodName)
			{
				var obj = ThingInstance.Get(uid);
				if(obj == null)
				{
					ThingInstance.MissingError(uid);
					return default;
				}
				var type = obj.GetType();
				TryAddAllMethods(type, false);

				return returnMethodsAllNames.ContainsKey(type) && returnMethodsAllNames[type].Contains(returnMethodName);
			}
			public static bool HasVoidMethod(string uid, string voidMethodName)
			{
				var obj = ThingInstance.Get(uid);
				if(obj == null)
				{
					ThingInstance.MissingError(uid);
					return default;
				}
				var type = obj.GetType();
				TryAddAllMethods(type, true);

				return voidMethodsAllNames.ContainsKey(type) && voidMethodsAllNames[type].Contains(voidMethodName);
			}

			public static Property GetProperty(string uid, string propertyName)
			{
				// don't even bother checking setters since all props have getters
				var get = GetPropInfo(uid, false, propertyName, false);
				if(get == null)
				{
					MissingPropError(ThingInstance.GetTryError(uid), propertyName, false, true);
					return default;
				}

				return get;
			}
			public static Method GetMethod(string uid, string methodName)
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
			public static List<Property> GetProperties(string uid)
			{
				var names = new List<string>();
				var result = new List<Property>();
				var getters = GetGetters(uid);
				var setters = GetSetters(uid);

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
			public static List<Method> GetMethods(string uid)
			{
				var names = new List<string>();
				var result = new List<Method>();
				var voids = GetVoidMethods(uid);
				var returns = GetReturnMethods(uid);

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
			public static List<Property> GetGetters(string uid)
			{
				return GetPropsInfo(uid, false);
			}
			public static List<Property> GetSetters(string uid)
			{
				return GetPropsInfo(uid, true);
			}
			public static List<Method> GetReturnMethods(string uid)
			{
				return GetMethodsInfo(uid, false);
			}
			public static List<Method> GetVoidMethods(string uid)
			{
				return GetMethodsInfo(uid, true);
			}

			#region Backend
			internal readonly static Dictionary<Type, List<string>>
				settersAllNames = new(), gettersAllNames = new(), voidMethodsAllNames = new(), returnMethodsAllNames = new();

			internal readonly static Dictionary<(Type, string), List<string>>
				returnMethodParamNames = new(), voidMethodParamNames = new();
			internal readonly static Dictionary<(Type, string), Type>
				setterTypes = new(), getterTypes = new(), returnMethodTypes = new();
			internal readonly static Dictionary<(Type, string), List<Type>>
				returnMethodParamTypes = new(), voidMethodParamTypes = new();

			internal readonly static Dictionary<(Type, string), MemberSetter> setters = new();
			internal readonly static Dictionary<(Type, string), MemberGetter> getters = new();
			internal readonly static Dictionary<(Type, string), Fasterflect.MethodInvoker>
				voidMethods = new(), returnMethods = new();

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
				var all = setter ? GetSetters(uid) : GetGetters(uid);
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
				var all = onlyVoid ? GetVoidMethods(uid) : GetReturnMethods(uid);
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
					result.Add(GetPropInfo(uid, set, allNames[i]));
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
					result.Add(GetMethod(uid, allNames[i]));
				return result;
			}
			internal static Property GetPropInfo(string uid, bool set, string propName, bool error = true)
			{
				var obj = ThingInstance.GetTryError(uid);
				if(obj == null)
					return default;
				var type = obj.GetType();

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
					HasGetter = set == false || gettersAllNames[type].Contains(propName),
					HasSetter = set || settersAllNames[type].Contains(propName),
					Name = propName,
					OwnerType = type.GetPrettyName(),
					Type = (set ? setterTypes[key] : getterTypes[key]).GetPrettyName()
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
					ReturnType = isVoid ? null : returnMethodTypes[key].GetPrettyName(),
					Name = methodName,
					OwnerType = type.GetPrettyName(),
				};
				for(int j = 0; j < paramNames.Count; j++)
					result.parameters.Add(new() { Name = paramNames[j], Owner = result, Type = paramTypes[j].GetPrettyName() });
				return result;
			}
			#endregion
		}
	}
}
