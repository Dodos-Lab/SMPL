using Fasterflect;

namespace SMPL
{
	public static class ThingManager
	{
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

		public static bool Exists(string uid)
		{
			return uid != null && Thing.objs.ContainsKey(uid);
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

		public static bool HasSet(string uid, string propertyName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllProps(type, true);

			return settersAllNames.ContainsKey(type) && settersAllNames[type].Contains(propertyName);
		}
		public static bool HasGet(string uid, string propertyName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllProps(type, false);

			return gettersAllNames.ContainsKey(type) && gettersAllNames[type].Contains(propertyName);
		}
		public static bool HasDoGet(string uid, string methodName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllMethods(type, false);

			return returnMethodsAllNames.ContainsKey(type) && returnMethodsAllNames[type].Contains(methodName);
		}
		public static bool HasDo(string uid, string methodName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			TryAddAllMethods(type, true);

			return voidMethodsAllNames.ContainsKey(type) && voidMethodsAllNames[type].Contains(methodName);
		}

		public static void Set(string uid, string propertyName, object value)
		{
			var obj = Thing.Get(uid);
			if(obj == null || value == null)
			{
				ThingNotFoundError(uid);
				return;
			}
			var type = obj.GetType();
			var key = (type, propertyName);
			var valueType = value.GetType();

			if(setters.ContainsKey(key) == false)
			{
				TryAddAllProps(type, true);

				if(settersAllNames[type].Contains(propertyName))
					setters[key] = type.DelegateForSetPropertyValue(propertyName);
				else
				{
					MissingPropError(type, obj, propertyName, true);
					return;
				}
			}

			if(valueType != setterTypes[key])
			{
				PropTypeMismatchError(obj, propertyName, valueType, setterTypes[key], true);
				return;
			}

			if(setters.ContainsKey(key))
				setters[key].Invoke(obj, value);
		}
		public static object Get(string uid, string propertyName)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			var key = (type, propertyName);

			if(getters.ContainsKey(key) == false)
			{
				TryAddAllProps(type, false);

				if(gettersAllNames[type].Contains(propertyName))
					getters[key] = type.DelegateForGetPropertyValue(propertyName);
				else
				{
					MissingPropError(type, obj, propertyName, false);
					return default;
				}
			}

			return getters.ContainsKey(key) ? getters[key].Invoke(obj) : default;
		}
		public static void Do(string uid, string methodName, params object[] parameters)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return;
			}
			var type = obj.GetType();
			var key = (type, methodName);

			if(voidMethods.ContainsKey(key) == false)
			{
				TryAddAllMethods(type, true);

				if(voidMethodsAllNames[type].Contains(methodName))
					voidMethods[key] = type.DelegateForCallMethod(methodName);
				else
					MissingMethodError(type, obj, methodName, true);
			}

			if(TryTypeMismatchError(obj, voidMethodParamTypes, parameters.ToArray(), true) == false && voidMethods.ContainsKey(key))
				voidMethods[key].Invoke(obj, parameters);
		}
		public static object DoGet(string uid, string methodName, params object[] parameters)
		{
			var obj = Thing.Get(uid);
			if(obj == null)
			{
				ThingNotFoundError(uid);
				return default;
			}
			var type = obj.GetType();
			var key = (type, methodName);

			if(returnMethods.ContainsKey(key) == false)
			{
				TryAddAllMethods(type, false);

				if(returnMethodsAllNames[type].Contains(methodName))
				{
					var p = type.GetMethod(methodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					returnMethods[key] = type.DelegateForCallMethod(methodName, paramTypes.ToArray());
					returnMethodParamTypes[key] = paramTypes;
				}
				else
					MissingMethodError(type, obj, methodName, false);
			}

			return TryTypeMismatchError(obj, returnMethodParamTypes, parameters, false) || returnMethodParamTypes.ContainsKey(key) == false ?
				default : returnMethods[key].Invoke(obj, parameters);
		}
		public static List<string> GetUIDs()
		{
			return Thing.objs.Keys.ToList();
		}

		public static string GetFreeUID(string uid)
		{
			var i = 1;
			var freeUID = uid;

			while(Thing.objs.ContainsKey(freeUID))
			{
				freeUID = $"{uid}{i}";
				i++;
			}
			return freeUID;
		}

		#region Backend
		private readonly static Dictionary<Type, List<string>>
			settersAllNames = new(), gettersAllNames = new(), voidMethodsAllNames = new(), returnMethodsAllNames = new();

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

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
			var methodNames = new List<string>();
			for(int i = 0; i < methods.Length; i++)
			{
				if(methods[i].IsSpecialName || methods[i].DeclaringType == typeof(object) ||
					methods[i].Name == "ToString" ||
					(methods[i].ReturnType != typeof(void) && onlyVoid) ||
					(methods[i].ReturnType == typeof(void) && onlyVoid == false))
					continue;

				methodNames.Add(methods[i].Name);
			}
			allNames[type] = methodNames;
		}

		internal static void ThingNotFoundError(string uid)
		{
			Console.LogError(2, $"{{{uid}}} does not exist.");
		}
		private static bool TryTypeMismatchError(Thing obj, Dictionary<(Type, string), List<Type>> paramTypes, object[] parameters, bool isVoid)
		{
			var nth = new string[] { "st", "nd", "rd" };
			var vStr = isVoid ? "Do" : "DoGet";

			foreach(var kvp in paramTypes)
				for(int i = 0; i < kvp.Value.Count; i++)
					if(parameters == null || parameters.Length <= i || parameters[i].GetType() != kvp.Value[i])
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
			var setStr = set ? "Set" : "Get";
			var objStr = $"{obj.GetType().Name}{{{obj.UID}}}";
			Console.LogError(2, $"{objStr} does not have {setStr}[{propertyName}].", $"For {setStr}, {objStr} has:\n{GetAllProps(type, set)}");
		}
		private static void MissingMethodError(Type type, Thing obj, string methodName, bool isVoid)
		{
			var voidStr = isVoid ? "Do" : "DoGet";
			var objStr = $"{obj.GetType().Name}{{{obj.UID}}}";
			Console.LogError(2, $"{objStr} does not have {voidStr}<{methodName}>.", $"For {voidStr}, {objStr} has:\n{GetAllMethods(type, isVoid)}");
		}
		private static void PropTypeMismatchError(Thing obj, string propertyName, Type valueType, Type expectedValueType, bool set)
		{
			var sStr = set ? "Set" : "Get";
			Console.LogError(1, $"The {sStr}[{propertyName}] of {obj.GetType().Name}{{{obj.UID}}} cannot process a value of Type`{valueType.Name}`.",
				$"It expects a value of Type`{expectedValueType.Name}`.");
		}

		private static string GetAllProps(Type type, bool setter)
		{
			var all = setter ? settersAllNames[type] : gettersAllNames[type];
			var result = "";

			for(int i = 0; i < all.Count; i++)
			{
				var sep = i == 0 ? "" : " ";
				result += $"{sep}[{all[i]}]";
			}

			return result;
		}
		private static string GetAllMethods(Type type, bool onlyVoid)
		{
			var all = onlyVoid ? voidMethodsAllNames[type] : returnMethodsAllNames[type];
			var result = "";

			for(int i = 0; i < all.Count; i++)
			{
				var sep = i == 0 ? "" : " ";
				result += $"{sep}<{all[i]}>";
			}

			return result;
		}
		#endregion
	}
}
