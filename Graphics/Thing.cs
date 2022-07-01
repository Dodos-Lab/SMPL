namespace SMPL.Graphics
{
	public static class Thing
	{
		public static Color AmbientColor { get; set; } = new Color(50, 50, 50);

		public static void DrawAllVisuals(RenderTarget renderTarget)
		{
			var visuals = VisualInstance.visuals.Reverse();
			var cameras = CameraInstance.cameras;

			for(int i = 0; i < cameras.Count; i++)
				if(cameras[i].UID != Scene.MAIN_CAMERA_UID)
					cameras[i].RenderTexture.Clear(Color.Transparent);

			foreach(var kvp in visuals)
				for(int i = 0; i < kvp.Value.Count; i++)
					for(int j = 0; j < kvp.Value[i].CameraUIDs.Count; j++)
					{
						var cam = ThingInstance.Get<CameraInstance>(kvp.Value[i].CameraUIDs[j]);
						if(cam != null)
							kvp.Value[i].Draw(cam.RenderTexture);
					}


			for(int i = 0; i < cameras.Count; i++)
				if(cameras[i].UID != Scene.MAIN_CAMERA_UID)
					cameras[i].RenderTexture.Display();

			foreach(var kvp in visuals)
				for(int i = 0; i < kvp.Value.Count; i++)
					kvp.Value[i].Draw(renderTarget);

			Scene.UpdateCurrentScene();
		}

		public static List<string> GetUIDs()
		{
			return Scene.CurrentScene.objs.Keys.ToList();
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
		public static void DestroyAll()
		{
			var objs = Scene.CurrentScene.objs;
			foreach(var kvp in objs)
				if(kvp.Value != Scene.MainCamera)
					kvp.Value.Destroy(true);
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
		public static string CreateText(string uid, string fontPath)
		{
			var t = new TextInstance(uid) { FontPath = fontPath };
			return t.UID;
		}

		public static void Set(string uid, string setPropertyName, object value)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return;
			var type = obj.GetType();
			var key = (type, setPropertyName);
			ThingInfo.TryAddAllProps(type, true);

			if(ThingInfo.setters.ContainsKey(key) == false)
			{
				if(ThingInfo.settersAllNames[type].Contains(setPropertyName))
					ThingInfo.setters[key] = type.DelegateForSetPropertyValue(setPropertyName);
				else
				{
					MissingPropError(obj, setPropertyName, true);
					return;
				}
			}

			var valueType = value.GetType();
			if(valueType != ThingInfo.setterTypes[key])
			{
				PropTypeMismatchError(obj, setPropertyName, valueType, ThingInfo.setterTypes[key], true);
				return;
			}

			if(ThingInfo.setters.ContainsKey(key))
				ThingInfo.setters[key].Invoke(obj, value);
		}
		public static object Get(string uid, string getPropertyName)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();
			var key = (type, getPropertyName);

			if(ThingInfo.getters.ContainsKey(key) == false)
			{
				ThingInfo.TryAddAllProps(type, false);

				if(ThingInfo.gettersAllNames[type].Contains(getPropertyName))
					ThingInfo.getters[key] = type.DelegateForGetPropertyValue(getPropertyName);
				else
				{
					MissingPropError(obj, getPropertyName, false);
					return default;
				}
			}

			return ThingInfo.getters.ContainsKey(key) ? ThingInfo.getters[key].Invoke(obj) : default;
		}
		public static void CallVoid(string uid, string voidMethodName, params object[] parameters)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return;
			var type = obj.GetType();
			var key = (type, voidMethodName);

			if(ThingInfo.voidMethods.ContainsKey(key) == false)
			{
				ThingInfo.TryAddAllMethods(type, true);

				if(ThingInfo.voidMethodsAllNames[type].Contains(voidMethodName))
				{
					var p = type.GetMethod(voidMethodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					ThingInfo.voidMethods[key] = type.DelegateForCallMethod(voidMethodName, paramTypes.ToArray());
					ThingInfo.voidMethodParamTypes[key] = paramTypes;
				}
				else
					MissingMethodError(obj, voidMethodName, true);
			}

			if(ThingInfo.voidMethods.ContainsKey(key) && TryTypeMismatchError(obj, key, ThingInfo.voidMethodParamTypes[key], parameters.ToArray(), true) == false)
				ThingInfo.voidMethods[key].Invoke(obj, parameters);
		}
		public static object CallGet(string uid, string getMethodName, params object[] parameters)
		{
			var obj = ThingInstance.GetTryError(uid);
			if(obj == null)
				return default;
			var type = obj.GetType();
			var key = (type, getMethodName);

			if(ThingInfo.returnMethods.ContainsKey(key) == false)
			{
				ThingInfo.TryAddAllMethods(type, false);

				if(ThingInfo.returnMethodsAllNames[type].Contains(getMethodName))
				{
					var p = type.GetMethod(getMethodName).GetParameters();
					var paramTypes = new List<Type>();
					for(int i = 0; i < p.Length; i++)
						paramTypes.Add(p[i].ParameterType);

					ThingInfo.returnMethods[key] = type.DelegateForCallMethod(getMethodName, paramTypes.ToArray());
					ThingInfo.returnMethodParamTypes[key] = paramTypes;
				}
				else
					MissingMethodError(obj, getMethodName, false);
			}

			return ThingInfo.returnMethodParamTypes.ContainsKey(key) == false ||
				TryTypeMismatchError(obj, key, ThingInfo.returnMethodParamTypes[key], parameters, false) ?
				default : ThingInfo.returnMethods[key].Invoke(obj, parameters);
		}

		#region Backend
		internal static bool TryTypeMismatchError(ThingInstance obj, (Type, string) key, List<Type> paramTypes, object[] parameters, bool isVoid)
		{
			if(parameters == null || parameters.Length == 0)
				return false;

			var nth = new string[] { "st", "nd", "rd" };
			var method = ThingInfo.GetMethod(obj.UID, key.Item2);

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
				$"It has the following {final}:\n{ThingInfo.GetAllProps(obj.UID, set)}");
		}
		internal static void MissingMethodError(ThingInstance obj, string methodName, bool isVoid, bool skipVoidReturn = false)
		{
			var voidStr = isVoid ? "void " : "return ";

			if(skipVoidReturn)
				voidStr = "";

			Console.LogError(2, $"{obj} does not have a {voidStr}method <{methodName}>.",
				$"It has the following {voidStr}methods:\n{ThingInfo.GetAllMethods(obj.UID, isVoid)}");
		}
		internal static void PropTypeMismatchError(ThingInstance obj, string propertyName, Type valueType, Type expectedValueType, bool set)
		{
			var prop = ThingInfo.GetProperty(obj.UID, propertyName);
			Console.LogError(1, $"The property\n{prop}\ncannot process the provided value.",
				$"It expects a value of type `{expectedValueType.GetPrettyName(true)}`, not `{valueType.GetPrettyName(true)}`.");
		}
		#endregion
	}
}
