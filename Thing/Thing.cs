namespace SMPL
{
	public static partial class Thing
	{
		public static class UI
		{
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
			public static string CreateTextButton(string uid, string textUID, string texturePath, string fontPath, string value = "Hello, World!")
			{
				var tt = new TextInstance(textUID) { FontPath = fontPath, Value = value };
				var t = new TextButtonInstance(uid, textUID) { TexturePath = texturePath, TextUID = tt.UID };
				return t.UID;
			}
			public static string CreateTextbox(string uid, string cameraUID, string fontPath, string value = "Hello, World!",
				uint resolutionX = 200, uint resolutionY = 200)
			{
				var t = new TextboxInstance(uid, cameraUID, resolutionX, resolutionY) { FontPath = fontPath, Value = value };
				return t.UID;
			}
			public static string CreateInputbox(string uid, string cameraUID, string fontPath, string value = "Hello, World!",
				uint resolutionX = 300, uint resolutionY = 40)
			{
				var t = new InputboxInstance(uid, cameraUID, resolutionX, resolutionY) { FontPath = fontPath, Value = value };
				return t.UID;
			}
			public static string CreateSlider(string uid, string texturePath)
			{
				var t = new SliderInstance(uid) { TexturePath = texturePath };
				return t.UID;
			}
		}

		public static List<string> GetUIDs()
		{
			return Scene.CurrentScene.objs.Keys.ToList();
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
		public static List<string> GetTags()
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
		public static string GetFreeUID(string uid)
		{
			var i = 1;

			if(string.IsNullOrWhiteSpace(uid))
				uid = nameof(Thing);

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
			return string.IsNullOrWhiteSpace(uid) == false && Scene.CurrentScene.objs.ContainsKey(uid);
		}
		public static void Destroy(string uid, bool destroyChildren)
		{
			if(uid == Scene.MAIN_CAMERA_UID)
				return;

			var obj = ThingInstance.Get(uid);
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
		public static string CreateCloth(string uid, string texturePath, float width = 100, float height = 100, int quadCountX = 5, int quadCountY = 5)
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
			if(obj == null)
				return;
			var type = obj.GetType();
			var key = (type, setPropertyName);
			Info.TryAddAllProps(type, true);

			if(Info.setters.ContainsKey(key) == false)
			{
				if(Info.settersAllNames[type].Contains(setPropertyName))
					Info.setters[key] = type.DelegateForSetPropertyValue(setPropertyName);
				else
				{
					MissingPropError(obj, setPropertyName, true);
					return;
				}
			}

			var valueType = value.GetType();
			if(valueType != Info.setterTypes[key])
			{
				PropTypeMismatchError(obj, setPropertyName, valueType, Info.setterTypes[key], true);
				return;
			}

			if(Info.setters.ContainsKey(key))
				Info.setters[key].Invoke(obj, value);
		}
		public static object Get(string uid, string getPropertyName)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();
			var key = (type, getPropertyName);

			if(Info.getters.ContainsKey(key) == false)
			{
				Info.TryAddAllProps(type, false);

				if(Info.gettersAllNames[type].Contains(getPropertyName))
					Info.getters[key] = type.DelegateForGetPropertyValue(getPropertyName);
				else
				{
					MissingPropError(obj, getPropertyName, false);
					return default;
				}
			}

			return Info.getters.ContainsKey(key) ? Info.getters[key].Invoke(obj) : default;
		}
		public static void CallVoid(string uid, string voidMethodName, params object[] parameters)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return;
			var type = obj.GetType();
			var key = (type, voidMethodName);

			if(Info.voidMethods.ContainsKey(key) == false)
			{
				Info.TryAddAllMethods(type, true);

				if(Info.voidMethodsAllNames[type].Contains(voidMethodName))
				{
					var p = type.GetMethod(voidMethodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					Info.voidMethods[key] = type.DelegateForCallMethod(voidMethodName, paramTypes.ToArray());
					Info.voidMethodParamTypes[key] = paramTypes;
				}
				else
					MissingMethodError(obj, voidMethodName, true);
			}

			if(Info.voidMethods.ContainsKey(key) && TryTypeMismatchError(obj, key, Info.voidMethodParamTypes[key], parameters.ToArray(), true) == false)
				Info.voidMethods[key].Invoke(obj, parameters);
		}
		public static object CallGet(string uid, string getMethodName, params object[] parameters)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();
			var key = (type, getMethodName);

			if(Info.returnMethods.ContainsKey(key) == false)
			{
				Info.TryAddAllMethods(type, false);

				if(Info.returnMethodsAllNames[type].Contains(getMethodName))
				{
					var p = type.GetMethod(getMethodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					Info.returnMethods[key] = type.DelegateForCallMethod(getMethodName, paramTypes.ToArray());
					Info.returnMethodParamTypes[key] = paramTypes;
				}
				else
					MissingMethodError(obj, getMethodName, false);
			}

			return Info.returnMethodParamTypes.ContainsKey(key) == false ||
				TryTypeMismatchError(obj, key, Info.returnMethodParamTypes[key], parameters, false) ?
				default : Info.returnMethods[key].Invoke(obj, parameters);
		}

		#region Backend
		internal static void DestroyAll()
		{
			if(Scene.CurrentScene == null)
				return;

			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
				if(kvp.Value != Scene.MainCamera)
					kvp.Value.Destroy(true);
		}

		internal static bool TryTypeMismatchError(ThingInstance obj, (Type, string) key, List<Type> paramTypes, object[] parameters, bool isVoid)
		{
			if(parameters == null || parameters.Length == 0)
				return false;

			var nth = new string[] { "st", "nd", "rd" };
			var method = Info.GetMethod(obj.UID, key.Item2);

			if(paramTypes.Count != parameters.Length)
			{
				Console.LogError(2, $"The method\n{method}\ncannot process its parameters.",
					$"It expects {paramTypes.Count} parameters, not {parameters.Length}.");
				return true;
			}

			for(int i = 0; i < paramTypes.Count; i++)
				if(parameters[i].GetType() != paramTypes[i])
				{
					var nthStr = i < 4 ? nth[i] : "th";
					Console.LogError(2, $"The method\n{method}\ncannot process the value of its {i + 1}{nthStr} parameter.",
						$"It expects a value of type {paramTypes[i].FullName}, not {parameters[i].GetType().FullName}.");
					return true;
				}
			return false;
		}
		internal static void MissingPropError(ThingInstance obj, string propertyName, bool set, bool skipGetSet = false)
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
				$"It has the following {final}:\n{Info.GetAllProps(obj.UID, set)}");
		}
		internal static void MissingMethodError(ThingInstance obj, string methodName, bool isVoid, bool skipVoidReturn = false)
		{
			var voidStr = isVoid ? "void " : "return ";

			if(skipVoidReturn)
				voidStr = "";

			Console.LogError(2, $"{obj} does not have a {voidStr}method <{methodName}>.",
				$"It has the following {voidStr}methods:\n{Info.GetAllMethods(obj.UID, isVoid)}");
		}
		internal static void PropTypeMismatchError(ThingInstance obj, string propertyName, Type valueType, Type expectedValueType, bool set)
		{
			var prop = Info.GetProperty(obj.UID, propertyName);
			Console.LogError(1, $"The property\n{prop}\ncannot process the provided value.",
				$"It expects a value of type `{expectedValueType.GetPrettyName(true)}`, not `{valueType.GetPrettyName(true)}`.");
		}
		#endregion
	}
}
