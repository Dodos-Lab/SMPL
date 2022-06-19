using Fasterflect;

namespace SMPL
{
	public static class ThingManager
	{
		public class PropertyInfo
		{
			public string Name { get; internal set; }
			public string Type { get; internal set; }
			public string OwnerType { get; internal set; }
			public bool IsSetter { get; internal set; }
			public bool IsGetter { get; internal set; }

			public override string ToString()
			{
				var getStr = IsGetter ? "get;" : "";
				var setStr = IsSetter ? "set;" : "";
				var sep = IsGetter && IsSetter ? " " : "";

				return $"{OwnerType}.{Type} {Name} {{ {getStr}{sep}{setStr} }}";
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
				return $"{returnTypeStr} {OwnerType}.{Name}({paramStr})";
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

		public static void UpdateAllThings()
		{
			var objs = Thing.objsOrder;
			foreach(var kvp in objs)
				for(int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Update();
		}
		public static void DrawAllVisuals(RenderTarget renderTarget)
		{
			var objs = Visual.visuals;
			foreach(var kvp in objs)
				for(int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Draw(renderTarget);
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
		public static string Create(string uid)
		{
			var spr = new Thing(GetFreeUID(uid));
			return spr.UID;
		}
		public static string CreateSprite(string uid)
		{
			var spr = new Sprite(GetFreeUID(uid));
			return spr.UID;
		}
		public static string CreateCamera(string uid, Vector2 resolution)
		{
			var cam = new Camera(uid, (uint)(resolution.X), (uint)(resolution.Y));
			return cam.UID;
		}

		public static bool HasSetter(string uid, string setPropertyName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
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
				ThingNotFoundError(uid);
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
				ThingNotFoundError(uid);
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
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllMethods(type, true);

			return voidMethodsAllNames.ContainsKey(type) && voidMethodsAllNames[type].Contains(voidMethodName);
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
					MissingPropError(type, obj, setPropertyName, true);
					return;
				}
			}

			var valueType = setterTypes[(type, setPropertyName)];
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
					MissingPropError(type, obj, getPropertyName, false);
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
					MissingMethodError(type, obj, voidMethodName, true);
			}

			if(TryTypeMismatchError(obj, voidMethodParamTypes, parameters.ToArray(), true) == false && voidMethods.ContainsKey(key))
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
					MissingMethodError(type, obj, getMethodName, false);
			}

			return TryTypeMismatchError(obj, returnMethodParamTypes, parameters, false) || returnMethodParamTypes.ContainsKey(key) == false ?
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

		internal static void ThingNotFoundError(string uid)
		{
			Console.LogError(0, $"{{{uid}}} does not exist.");
		}
		private static bool TryTypeMismatchError(Thing obj, Dictionary<(Type, string), List<Type>> paramTypes, object[] parameters, bool isVoid)
		{
			if(parameters == null || parameters.Length == 0)
				return false;

			var nth = new string[] { "st", "nd", "rd" };
			var vStr = isVoid ? "Void" : "Return";

			foreach(var kvp in paramTypes)
				for(int i = 0; i < kvp.Value.Count; i++)
					if(parameters[i].GetType() != kvp.Value[i])
					{
						var nthStr = i < 4 ? nth[i] : "th";
						Console.LogError(2, $"The {vStr}<{kvp.Key.Item2}> of {obj.GetType().Name}{{{obj.UID}}} cannot process its {i + 1}{nthStr} " +
							$"parameter's value of type `{parameters[i].GetType().Name}`.",
							$"It expects a value of type `{kvp.Value[i]}`.");
						return true;
					}
			return false;
		}

		private static void MissingPropError(Type type, Thing obj, string propertyName, bool set)
		{
			var setStr = set ? "setter" : "getter";
			var objStr = $"{obj.GetType().Name}{{{obj.UID}}}";
			Console.LogError(2, $"{objStr} does not have the {setStr} [{propertyName}].",
				$"{objStr} has the following {setStr}s:\n{GetAllProps(obj.UID, set)}");
		}
		private static void MissingMethodError(Type type, Thing obj, string methodName, bool isVoid)
		{
			var voidStr = isVoid ? "void" : "return";
			var objStr = $"{obj.GetType().Name}{{{obj.UID}}}";
			Console.LogError(2, $"{objStr} does not have a {voidStr} method <{methodName}>.",
				$"{objStr} has the following {voidStr} methods:\n{GetAllMethods(obj.UID, isVoid)}");
		}
		private static void PropTypeMismatchError(Thing obj, string propertyName, Type valueType, Type expectedValueType, bool set)
		{
			var sStr = set ? "Set" : "Get";
			Console.LogError(1, $"The {sStr}[{propertyName}] of {obj.GetType().Name}{{{obj.UID}}} cannot process a value of Type`{valueType.Name}`.",
				$"It expects a value of Type`{expectedValueType.Name}`.");
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
				ThingNotFoundError(uid);
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

			TryAddAllProps(type, true);
			TryAddAllProps(type, false);

			var result = new List<PropertyInfo>();
			var allNames = set ? settersAllNames[type] : gettersAllNames[type];
			for(int i = 0; i < allNames.Count; i++)
			{
				var name = allNames[i];
				var key = (type, name);
				result.Add(new()
				{
					IsGetter = set == false || gettersAllNames[type].Contains(name),
					IsSetter = set || settersAllNames[type].Contains(name),
					Name = name,
					OwnerType = type.GetPrettyName(),
					Type = (set ? setterTypes[key] : getterTypes[key]).GetPrettyName()
				});
			}
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
			{
				var name = allNames[i];
				var key = (type, name);
				var paramTypes = isVoid ? voidMethodParamTypes[key] : returnMethodParamTypes[key];
				var paramNames = new List<string>(isVoid ? voidMethodParamNames[key] : returnMethodParamNames[key]);

				var method = new MethodInfo()
				{
					ReturnType = isVoid ? null : returnMethodTypes[key].GetPrettyName(),
					Name = name,
					OwnerType = type.GetPrettyName(),
				};
				for(int j = 0; j < paramNames.Count; j++)
					method.parameters.Add(new() { Name = paramNames[j], Owner = method, Type = paramTypes[j].GetPrettyName() });
				result.Add(method);
			}
			return result;
		}
		#endregion
	}
}
