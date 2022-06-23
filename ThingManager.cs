namespace SMPL
{
	public static class ThingManager
	{
		public enum BlendModes { None, Alpha, Add, Multiply }
		public enum Effects
		{
			None,
			Custom,
			ColorFill,
			ColorAdjust,
			ColorReplaceLight,
			ColorsSwap,
			ColorsReplace,
			ColorsTint,
			Blink,
			Blur,
			Earthquake,
			Edge,
			Lights,
			Grid,
			Outline,
			Pixelate,
			Screen,
			Water
		}
		public struct CodeGLSL
		{
			private const string FRAG_UNI = @"uniform sampler2D Texture;
uniform vec2 TextureSize;
uniform bool HasTexture;
uniform float Time;
uniform vec2 CameraSize;
uniform vec2 CameraResolution;
";
			private const string FRAG_PRE_MAIN = @"
vec4 GetPixelColor(sampler2D texture, vec2 coords);
bool ColorEqualsColor(vec4 a, vec4 b, float margin);
float Map(float value, float min1, float max1, float min2, float max2);

float Map(float value, float min1, float max1, float min2, float max2)
{
  return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
}
vec4 GetPixelColor(sampler2D texture, vec2 coords)
{
	return texture2D(texture, coords);
}
bool ColorEqualsColor(vec4 a, vec4 b, float margin)
{
   margin += 0.001;
	return
		a.x > b.x - margin && a.x < b.x + margin &&
		a.y > b.y - margin && a.y < b.y + margin &&
		a.z > b.z - margin && a.z < b.z + margin &&
		a.w > b.w - margin && a.w < b.w + margin;
}

void main()
{
	vec4 Tint = gl_Color;
	vec2 TextureCoords = gl_TexCoord[0].xy;
	vec2 CameraCoords = gl_FragCoord / CameraSize;
	vec4 FinalColor = HasTexture ? GetPixelColor(Texture, TextureCoords) : vec4(1.0);
";
			private const string FRAG_POST_MAIN = @"
	gl_FragColor = FinalColor * Tint;
}";
			private const string VERT_UNI = @"uniform float Time;
uniform vec2 TextureSize;
";
			private const string VERT_PRE_MAIN = @"
void main()
{
	vec4 Corner = gl_Vertex;
";
			private const string VERT_POST_MAIN = @"
	gl_Position = gl_ModelViewProjectionMatrix * Corner;
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}";

			public string FragmentUniforms { get; set; }
			public string FragmentCode { get; set; }
			public string VertexUniforms { get; set; }
			public string VertexCode { get; set; }

			public string GetFragment()
			{
				return $"{FRAG_UNI}{FragmentUniforms}{FRAG_PRE_MAIN}{FragmentCode}{FRAG_POST_MAIN}";
			}
			public string GetVertex()
			{
				return $"{VERT_UNI}{VertexUniforms}{VERT_PRE_MAIN}{VertexCode}{VERT_POST_MAIN}";
			}

			public static CodeGLSL GetEffectCode(Effects effect)
			{
				if(effect == Effects.Custom)
					return default;

				return Visual.shaders[effect];
			}
		}

		public class PropertyInfo
		{
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
		public class MethodInfo
		{
			internal List<ParameterInfo> parameters = new();

			public string Name { get; internal set; }
			public string ReturnType { get; internal set; }
			public string OwnerType { get; internal set; }
			public List<ParameterInfo> Parameters => new(parameters);

			public override string ToString()
			{
				var paramStr = "";
				var returnTypeStr = ReturnType ?? "void";
				for(int i = 0; i < parameters.Count; i++)
					paramStr += (i == 0 ? "" : ", ") + parameters[i].ToString();
				return $"{OwnerType} {{ {returnTypeStr} {Name}({paramStr}) }}";
			}
		}
		public class ParameterInfo
		{
			public string Name { get; internal set; }
			public string Type { get; internal set; }
			public MethodInfo Owner { get; internal set; }

			public override string ToString()
			{
				return $"{Type} {Name}";
			}
		}

		public static Color AmbientColor { get; set; } = new Color(50, 50, 50);

		public static void UpdateAllThings()
		{
			var objs = Thing.objsOrder;
			foreach(var kvp in objs)
				for(int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Update();
		}
		public static void DrawAllVisuals(RenderTarget renderTarget)
		{
			var visuals = Visual.visuals.Reverse();
			foreach(var kvp in visuals)
				for(int i = 0; i < kvp.Value.Count; i++)
				{
					var visual = kvp.Value[i];

					if(visual.Effect == Effects.Lights)
						Light.Update(visual, renderTarget);

					visual.Draw(renderTarget);
				}
		}

		public static List<string> GetUIDs()
		{
			return Scene.CurrentScene.objs.Keys.ToList();
		}
		public static string GetFreeUID(string uid)
		{
			var i = 1;
			var freeUID = uid;

			var objs = Scene.CurrentScene.objs;
			while(objs.ContainsKey(freeUID))
			{
				freeUID = $"{uid}{i}";
				i++;
			}
			return freeUID;
		}

		public static bool Exists(string uid)
		{
			return uid != null && Scene.CurrentScene.objs.ContainsKey(uid);
		}
		public static void Destroy(string uid, bool destroyChildren)
		{
			if(uid == Scene.MAIN_CAMERA_UID)
				return;

			var obj = Thing.Get(uid);
			obj.Destroy(destroyChildren);
		}
		public static void DestroyAll()
		{
			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
				if(kvp.Value != Scene.MainCamera)
					kvp.Value.Destroy(true);
		}

		public static string CreateSprite(string uid)
		{
			var t = new Sprite(GetFreeUID(uid));
			return t.UID;
		}
		public static string CreateLight(string uid)
		{
			var t = new Light(GetFreeUID(uid));
			return t.UID;
		}
		public static string CreateCamera(string uid, Vector2 resolution)
		{
			var t = new Camera(uid, resolution);
			return t.UID;
		}

		public static bool HasSetter(string uid, string setPropertyName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				MissingThingError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllProps(type, true);

			return settersAllNames.ContainsKey(type) && settersAllNames[type].Contains(setPropertyName);
		}
		public static bool HasGetter(string uid, string getPropertyName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				MissingThingError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllProps(type, false);

			return gettersAllNames.ContainsKey(type) && gettersAllNames[type].Contains(getPropertyName);
		}
		public static bool HasReturnMethod(string uid, string returnMethodName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				MissingThingError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllMethods(type, false);

			return returnMethodsAllNames.ContainsKey(type) && returnMethodsAllNames[type].Contains(returnMethodName);
		}
		public static bool HasVoidMethod(string uid, string voidMethodName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				MissingThingError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllMethods(type, true);

			return voidMethodsAllNames.ContainsKey(type) && voidMethodsAllNames[type].Contains(voidMethodName);
		}

		public static PropertyInfo GetPropertyInfo(string uid, string propertyName)
		{
			// don't even bother checking setters since all props have getters
			var get = GetPropInfo(uid, false, propertyName, false);
			if(get == null)
			{
				MissingPropError(Get(uid), propertyName, false, true);
				return default;
			}

			return get;
		}
		public static MethodInfo GetMethodInfo(string uid, string methodName)
		{
			var rtrn = GetMethodInfo(uid, false, methodName, false);
			var vd = GetMethodInfo(uid, true, methodName, false);

			if(rtrn == null && vd == null)
			{
				MissingMethodError(Get(uid), methodName, false, true);
				return default;
			}

			return rtrn ?? vd;
		}
		public static List<PropertyInfo> GetPropertiesInfo(string uid)
		{
			var names = new List<string>();
			var result = new List<PropertyInfo>();
			var getters = GetGettersInfo(uid);
			var setters = GetSettersInfo(uid);

			ProcessList(getters);
			ProcessList(setters);

			return result;

			void ProcessList(List<PropertyInfo> list)
			{
				for(int i = 0; i < list.Count; i++)
					if(names.Contains(list[i].ToString()) == false)
					{
						result.Add(list[i]);
						names.Add(list[i].ToString());
					}
			}
		}
		public static List<MethodInfo> GetMethodsInfo(string uid)
		{
			var names = new List<string>();
			var result = new List<MethodInfo>();
			var voids = GetVoidMethodsInfo(uid);
			var returns = GetReturnMethodsInfo(uid);

			ProcessList(voids);
			ProcessList(returns);

			return result;

			void ProcessList(List<MethodInfo> list)
			{
				for(int i = 0; i < list.Count; i++)
					if(names.Contains(list[i].ToString()) == false)
					{
						result.Add(list[i]);
						names.Add(list[i].ToString());
					}
			}
		}
		public static List<PropertyInfo> GetGettersInfo(string uid)
		{
			return GetPropsInfo(uid, false);
		}
		public static List<PropertyInfo> GetSettersInfo(string uid)
		{
			return GetPropsInfo(uid, true);
		}
		public static List<MethodInfo> GetReturnMethodsInfo(string uid)
		{
			return GetMethodsInfo(uid, false);
		}
		public static List<MethodInfo> GetVoidMethodsInfo(string uid)
		{
			return GetMethodsInfo(uid, true);
		}

		public static void Set(string uid, string setPropertyName, object value)
		{
			var obj = Get(uid);
			if(obj == null)
				return;
			var type = obj.GetType();
			var key = (type, setPropertyName);
			TryAddAllProps(type, true);

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
			if(valueType != setterTypes[key])
			{
				PropTypeMismatchError(obj, setPropertyName, valueType, setterTypes[key], true);
				return;
			}

			if(setters.ContainsKey(key))
				setters[key].Invoke(obj, value);
		}
		public static object Get(string uid, string getPropertyName)
		{
			var obj = Get(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();
			var key = (type, getPropertyName);

			if(getters.ContainsKey(key) == false)
			{
				TryAddAllProps(type, false);

				if(gettersAllNames[type].Contains(getPropertyName))
					getters[key] = type.DelegateForGetPropertyValue(getPropertyName);
				else
				{
					MissingPropError(obj, getPropertyName, false);
					return default;
				}
			}

			return getters.ContainsKey(key) ? getters[key].Invoke(obj) : default;
		}
		public static void CallVoid(string uid, string voidMethodName, params object[] parameters)
		{
			var obj = Get(uid);
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

			if(voidMethods.ContainsKey(key) && TryTypeMismatchError(obj, key, voidMethodParamTypes[key], parameters.ToArray(), true) == false)
				voidMethods[key].Invoke(obj, parameters);
		}
		public static object CallGet(string uid, string getMethodName, params object[] parameters)
		{
			var obj = Get(uid);
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
					MissingMethodError(obj, getMethodName, false);
			}

			return returnMethodParamTypes.ContainsKey(key) == false || TryTypeMismatchError(obj, key, returnMethodParamTypes[key], parameters, false) ?
				default : returnMethods[key].Invoke(obj, parameters);
		}

		#region Backend
		private readonly static Dictionary<Type, List<string>>
			settersAllNames = new(), gettersAllNames = new(), voidMethodsAllNames = new(), returnMethodsAllNames = new();

		private readonly static Dictionary<(Type, string), List<string>>
			returnMethodParamNames = new(), voidMethodParamNames = new();
		private readonly static Dictionary<(Type, string), Type>
			setterTypes = new(), getterTypes = new(), returnMethodTypes = new();
		private readonly static Dictionary<(Type, string), List<Type>>
			returnMethodParamTypes = new(), voidMethodParamTypes = new();

		private readonly static Dictionary<(Type, string), MemberSetter> setters = new();
		private readonly static Dictionary<(Type, string), MemberGetter> getters = new();
		private readonly static Dictionary<(Type, string), Fasterflect.MethodInvoker>
			voidMethods = new(), returnMethods = new();

		private static void TryAddAllProps(Type type, bool setter)
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
		private static void TryAddAllMethods(Type type, bool onlyVoid)
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

		private static bool TryTypeMismatchError(Thing obj, (Type, string) key, List<Type> paramTypes, object[] parameters, bool isVoid)
		{
			if(parameters == null || parameters.Length == 0)
				return false;

			var nth = new string[] { "st", "nd", "rd" };
			var method = GetMethodInfo(obj.UID, key.Item2);

			for(int i = 0; i < paramTypes.Count; i++)
				if(parameters[i].GetType() != paramTypes[i])
				{
					var nthStr = i < 4 ? nth[i] : "th";
					Console.LogError(2, $"The method\n{method}\ncannot process the value of its {i + 1}{nthStr} parameter.",
						$"It expects a value of type `{paramTypes[i].FullName}`, not `{parameters[i].GetType().FullName}`.");
					return true;
				}
			return false;
		}
		internal static void MissingThingError(string uid)
		{
			Console.LogError(0, $"Thing{{{uid}}} does not exist.");
		}
		private static void MissingPropError(Thing obj, string propertyName, bool set, bool skipGetSet = false)
		{
			var setStr = set ? "setter" : "getter";
			var objStr = $"{obj.GetType().Name}{{{obj.UID}}}";
			var final = setStr + "s";

			if(skipGetSet)
			{
				setStr = "property";
				final = "properties";
			}

			Console.LogError(2, $"{objStr} does not have the {setStr} [{propertyName}].",
				$"It has the following {final}:\n{GetAllProps(obj.UID, set)}");
		}
		private static void MissingMethodError(Thing obj, string methodName, bool isVoid, bool skipVoidReturn = false)
		{
			var voidStr = isVoid ? "void " : "return ";

			if(skipVoidReturn)
				voidStr = "";

			Console.LogError(2, $"{obj} does not have a {voidStr}method <{methodName}>.",
				$"It has the following {voidStr}methods:\n{GetAllMethods(obj.UID, isVoid)}");
		}
		private static void PropTypeMismatchError(Thing obj, string propertyName, Type valueType, Type expectedValueType, bool set)
		{
			var prop = GetPropertyInfo(obj.UID, propertyName);
			Console.LogError(1, $"The property\n{prop}\ncannot process the provided value.",
				$"It expects a value of type `{expectedValueType.GetPrettyName(true)}`, not `{valueType.GetPrettyName(true)}`.");
		}

		private static string GetAllProps(string uid, bool setter)
		{
			var all = setter ? GetSettersInfo(uid) : GetGettersInfo(uid);
			var result = "";

			for(int i = 0; i < all.Count; i++)
			{
				var sep = i == 0 ? "" : "\n";
				result += $"{sep}{all[i]}";
			}

			return result;
		}
		private static string GetAllMethods(string uid, bool onlyVoid)
		{
			var all = onlyVoid ? GetVoidMethodsInfo(uid) : GetReturnMethodsInfo(uid);
			var result = "";

			for(int i = 0; i < all.Count; i++)
			{
				var sep = i == 0 ? "" : "\n";
				result += $"{sep}{all[i]}";
			}

			return result;
		}

		private static Thing Get(string uid)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				MissingThingError(uid);
				return default;
			}
			return obj;
		}
		private static List<PropertyInfo> GetPropsInfo(string uid, bool set)
		{
			var obj = Get(uid);
			if(obj == null)
				return new();
			var type = obj.GetType();

			var result = new List<PropertyInfo>();
			var allNames = set ? settersAllNames[type] : gettersAllNames[type];
			for(int i = 0; i < allNames.Count; i++)
				result.Add(GetPropInfo(uid, set, allNames[i]));
			return result;
		}
		private static List<MethodInfo> GetMethodsInfo(string uid, bool isVoid)
		{
			var obj = Get(uid);
			if(obj == null)
				return new();
			var type = obj.GetType();

			TryAddAllMethods(type, isVoid);
			var result = new List<MethodInfo>();
			var allNames = isVoid ? voidMethodsAllNames[type] : returnMethodsAllNames[type];
			for(int i = 0; i < allNames.Count; i++)
				result.Add(GetMethodInfo(uid, allNames[i]));
			return result;
		}
		private static PropertyInfo GetPropInfo(string uid, bool set, string propName, bool error = true)
		{
			var obj = Get(uid);
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
			return new PropertyInfo()
			{
				HasGetter = set == false || gettersAllNames[type].Contains(propName),
				HasSetter = set || settersAllNames[type].Contains(propName),
				Name = propName,
				OwnerType = type.GetPrettyName(),
				Type = (set ? setterTypes[key] : getterTypes[key]).GetPrettyName()
			};
		}
		private static MethodInfo GetMethodInfo(string uid, bool isVoid, string methodName, bool error = true)
		{
			var obj = Get(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();

			TryAddAllMethods(type, isVoid);

			var allNames = isVoid ? voidMethodsAllNames : returnMethodsAllNames;
			if(allNames[type].Contains(methodName) == false)
			{
				if(error)
					MissingMethodError(obj, methodName, isVoid);
				return default;
			}

			var key = (type, methodName);
			var paramTypes = isVoid ? voidMethodParamTypes[key] : returnMethodParamTypes[key];
			var paramNames = new List<string>(isVoid ? voidMethodParamNames[key] : returnMethodParamNames[key]);

			var result = new MethodInfo()
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
