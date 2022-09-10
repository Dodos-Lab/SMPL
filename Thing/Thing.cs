namespace SMPL
{
	public static partial class Thing
	{
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
					text.Scale = new(scale, scale);

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
			public static string CreateInputbox(string uid, string fontPath, string value = "",
				uint resolutionX = 300, uint resolutionY = 60)
			{
				var t = new InputboxInstance(uid, resolutionX, resolutionY) { FontPath = fontPath, Value = value };
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
			if(Scene.CurrentScene == null)
				return uid;

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
		public static string Duplicate(string uid, string newUID)
		{
			var thing = ThingInstance.Get(uid);
			var prevUID = uid;
			var prevOldUID = thing.oldUID;
			var children = new List<string>(thing.childrenUIDs);

			thing.UID = GetFreeUID(newUID);

			var settings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
			var json = JsonConvert.SerializeObject(thing, settings);

			thing.UID = prevUID;
			thing.oldUID = prevOldUID;
			var newThing = JsonConvert.DeserializeObject<ThingInstance>(json, settings);

			for(int i = 0; i < children.Count; i++)
			{
				var child = ThingInstance.Get(children[i]);
				child.ParentUID = prevUID;
			}

			return newThing.UID;
		}
		public static void Destroy(string uid, bool destroyChildren)
		{
			if(uid == Scene.MAIN_CAMERA_UID)
				return;

			var obj = ThingInstance.Get(uid);
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
		public static string CreateCloth(string uid, string texturePath, float width = 300, float height = 300, int quadCountX = 5, int quadCountY = 5)
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
			var t = Info.setterTypes[key];
			if(valueType != t && valueType.Inherits(t) == false)
			{
				PropTypeMismatchError(obj, setPropertyName, valueType, t);
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

			if(Info.voidMethods.ContainsKey(key) && TryTypeMismatchError(obj, key, Info.voidMethodParamTypes[key], parameters.ToArray()) == false)
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

			return Info.returnMethodParamTypes.ContainsKey(key) == false || // no params found on that method
				Info.returnMethodParamTypes[key].Count != parameters.Length || // provided params are different than method params
				TryTypeMismatchError(obj, key, Info.returnMethodParamTypes[key], parameters) ? // provided param types are different than desired
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

		internal static bool TryTypeMismatchError(ThingInstance obj, (Type, string) key, List<Type> paramTypes, object[] parameters)
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
		internal static void PropTypeMismatchError(ThingInstance obj, string propertyName, Type valueType, Type expectedValueType)
		{
			var prop = Info.GetProperty(obj.UID, propertyName);
			Console.LogError(1, $"The property\n{prop}\ncannot process the provided value.",
				$"It expects a value of type `{expectedValueType.GetPrettyName(true)}`, not `{valueType.GetPrettyName(true)}`.");
		}
		#endregion
	}
}
