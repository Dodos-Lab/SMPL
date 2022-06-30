namespace SMPL.Graphics
{
	public class Property
	{
		#region Property Names
		public const string THING_TYPES = nameof(ThingInstance.Types);
		public const string THING_UID = nameof(ThingInstance.UID);
		public const string THING_OLD_UID = nameof(ThingInstance.OldUID);
		public const string THING_AGE = nameof(ThingInstance.Age);
		public const string THING_AGE_AS_TIMER = nameof(ThingInstance.AgeAsTimer);
		public const string THING_PARENT_UID = nameof(ThingInstance.ParentUID);
		public const string THING_PARENT_OLD_UID = nameof(ThingInstance.ParentOldUID);
		public const string THING_CHILDREN_UIDS = nameof(ThingInstance.ChildrenUIDs);
		public const string THING_LOCAL_POSITION = nameof(ThingInstance.LocalPosition);
		public const string THING_LOCAL_ANGLE = nameof(ThingInstance.LocalAngle);
		public const string THING_LOCAL_DIRECTION = nameof(ThingInstance.LocalDirection);
		public const string THING_LOCAL_SCALE = nameof(ThingInstance.LocalScale);
		public const string THING_POSITION = nameof(ThingInstance.Position);
		public const string THING_ANGLE = nameof(ThingInstance.Angle);
		public const string THING_DIRECTION = nameof(ThingInstance.Direction);
		public const string THING_SCALE = nameof(ThingInstance.Scale);
		public const string THING_HITBOX = nameof(ThingInstance.Hitbox);
		public const string THING_BOUNDING_BOX = nameof(ThingInstance.BoundingBox);

		public const string CAMERA_IS_SMOOTH = nameof(CameraInstance.IsSmooth);
		public const string CAMERA_MOUSE_CURSOR_POSITION = nameof(CameraInstance.MouseCursorPosition);
		public const string CAMERA_RENDER_TEXTURE = nameof(CameraInstance.RenderTexture);
		public const string CAMERA_RESOLUTION = nameof(CameraInstance.Resolution);

		public const string LIGHT_COLOR = nameof(LightInstance.Color);

		public const string VISUAL_TINT = nameof(VisualInstance.Tint);
		public const string VISUAL_IS_HIDDEN = nameof(VisualInstance.IsHidden);
		public const string VISUAL_IS_SMOOTH = nameof(VisualInstance.IsSmooth);
		public const string VISUAL_IS_REPEATED = nameof(VisualInstance.IsRepeated);
		public const string VISUAL_DEPTH = nameof(VisualInstance.Depth);
		public const string VISUAL_TEXTURE_PATH = nameof(VisualInstance.TexturePath);
		public const string VISUAL_EFFECT = nameof(VisualInstance.Effect);
		public const string VISUAL_CAMERA_UIDS = nameof(VisualInstance.CameraUIDs);
		public const string VISUAL_BLEND_MODE = nameof(VisualInstance.BlendMode);

		public const string SPRITE_TEX_COORD_UNIT_A = nameof(SpriteInstance.TexCoordUnitA);
		public const string SPRITE_TEX_COORD_UNIT_B = nameof(SpriteInstance.TexCoordUnitB);
		public const string SPRITE_TEX_COORD_A = nameof(SpriteInstance.TexCoordA);
		public const string SPRITE_TEX_COORD_B = nameof(SpriteInstance.TexCoordB);
		public const string SPRITE_SIZE = nameof(SpriteInstance.Size);
		public const string SPRITE_LOCAL_SIZE = nameof(SpriteInstance.LocalSize);
		public const string SPRITE_ORIGIN = nameof(SpriteInstance.Origin);
		public const string SPRITE_ORIGIN_UNIT = nameof(SpriteInstance.OriginUnit);

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
		public const string THING_GET_LOCAL_POSITION_FROM_PARENT = nameof(ThingInstance.GetLocalPositionFromParent);
		public const string THING_GET_POSITION_FROM_PARENT = nameof(ThingInstance.GetPositionFromParent);
		public const string THING_GET_LOCAL_POSITION_FROM_SELF = nameof(ThingInstance.GetLocalPositionFromSelf);
		public const string THING_GET_POSITION_FROM_SELF = nameof(ThingInstance.GetPositionFromSelf);
		public const string THING_GET_CORNER_CLOCKWISE = nameof(ThingInstance.GetCornerClockwise);
		public const string THING_DESTROY = nameof(ThingInstance.Destroy);

		public const string CAMERA_SNAP = nameof(CameraInstance.Snap);

		public const string VISUAL_GET_EFFECT_CODE = nameof(VisualInstance.GetEffectCode);
		public const string VISUAL_SET_CUSTOM_EFFECT = nameof(VisualInstance.SetCustomEffect);
		public const string VISUAL_SET_EFFECT_BOOL = nameof(VisualInstance.SetEffectBool);
		public const string VISUAL_SET_EFFECT_INT = nameof(VisualInstance.SetEffectInt);
		public const string VISUAL_SET_EFFECT_FLOAT = nameof(VisualInstance.SetEffectFloat);
		public const string VISUAL_SET_EFFECT_VECTOR2 = nameof(VisualInstance.SetEffectVector2);
		public const string VISUAL_SET_EFFECT_VECTOR3 = nameof(VisualInstance.SetEffectVector3);
		public const string VISUAL_SET_EFFECT_VECTOR4 = nameof(VisualInstance.SetEffectVector4);
		public const string VISUAL_SET_EFFECT_COLOR = nameof(VisualInstance.SetEffectColor);
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
	public static class ThingInfo
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
				Thing.MissingPropError(ThingInstance.GetTryError(uid), propertyName, false, true);
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
				Thing.MissingMethodError(ThingInstance.GetTryError(uid), methodName, false, true);
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
					Thing.MissingPropError(obj, propName, set);
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
