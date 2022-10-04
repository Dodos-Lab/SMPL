namespace SMPL.Tools
{
	public static class Storage
	{
		internal class JsonBinder : ISerializationBinder
		{
			internal static JsonBinder Instance = new();
			public List<Type> KnownTypes { get; set; } = new();
			public JsonBinder()
			{
				KnownTypes.Add(typeof(object));
				KnownTypes.Add(typeof(bool));
				KnownTypes.Add(typeof(byte));
				KnownTypes.Add(typeof(int));
				KnownTypes.Add(typeof(float));
				KnownTypes.Add(typeof(string));

				KnownTypes.Add(typeof(Vector2));
				KnownTypes.Add(typeof(Vector3));
				KnownTypes.Add(typeof(Vec2));
				KnownTypes.Add(typeof(Vec3));
				KnownTypes.Add(typeof(Vec4));
				KnownTypes.Add(typeof(Color));
				KnownTypes.Add(typeof(Line));
				KnownTypes.Add(typeof(Thing.CodeGLSL));
				KnownTypes.Add(typeof(Thing.CubeSide));
				KnownTypes.Add(typeof(Thing.Tile));
				KnownTypes.Add(typeof(Scene.TextureStackInfo));
				KnownTypes.Add(typeof(Thing.AudioStatus));
				KnownTypes.Add(typeof(Thing.BlendMode));
				KnownTypes.Add(typeof(Thing.Effect));
				KnownTypes.Add(typeof(Text.Styles));
				KnownTypes.Add(typeof(Thing.GUI.TextboxAlignment));
				KnownTypes.Add(typeof(Thing.GUI.TextboxSymbolCollection));
				KnownTypes.Add(typeof(Thing.GUI.ListItem));
				KnownTypes.Add(typeof(Thing.GUI.ButtonDetails));
				KnownTypes.Add(typeof(Thing.GUI.TextDetails));
				KnownTypes.Add(typeof(Hitbox));
				KnownTypes.Add(typeof(Database));
				KnownTypes.Add(typeof(Scene));
				KnownTypes.Add(typeof(Settings));

				var types = new List<Type>(KnownTypes);
				for(int i = 0; i < types.Count; i++)
				{
					var list = typeof(List<>).MakeGenericType(types[i]);
					var readOnlyType = typeof(ReadOnlyCollection<>).MakeGenericType(types[i]);
					var animType = typeof(Animation<>).MakeGenericType(types[i]);
					var arrType = types[i].MakeArrayType();
					KnownTypes.Add(arrType);
					KnownTypes.Add(list);
					KnownTypes.Add(readOnlyType);
					KnownTypes.Add(animType);

					for(int j = 0; j < types.Count; j++)
					{
						var dict = typeof(Dictionary<,>).MakeGenericType(types[i], types[j]);
						KnownTypes.Add(dict);
					}
				}

				KnownTypes.Add(typeof(ConcurrentDictionary<string, Scene.TextureStackInfo>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, string>));
				KnownTypes.Add(typeof(Dictionary<string, ThingInstance>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, bool>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, int>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, float>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, Vec2>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, Vec3>));
				KnownTypes.Add(typeof(ConcurrentDictionary<string, Vec4>));

				KnownTypes.Add(typeof(Thing.CustomProperty));
				KnownTypes.Add(typeof(Dictionary<string, Thing.CustomProperty>));

				KnownTypes.Add(typeof(AudioInstance));
				KnownTypes.Add(typeof(CameraInstance));
				KnownTypes.Add(typeof(ClothInstance));
				KnownTypes.Add(typeof(CubeInstance));
				KnownTypes.Add(typeof(LightInstance));
				KnownTypes.Add(typeof(NinePatchInstance));
				KnownTypes.Add(typeof(ParticleManagerInstance));
				KnownTypes.Add(typeof(Pseudo3DInstance));
				KnownTypes.Add(typeof(SpriteInstance));
				KnownTypes.Add(typeof(SpriteStackInstance));
				KnownTypes.Add(typeof(TextInstance));
				KnownTypes.Add(typeof(TilemapInstance));

				KnownTypes.Add(typeof(ButtonInstance));
				KnownTypes.Add(typeof(CheckboxInstance));
				KnownTypes.Add(typeof(InputboxInstance));
				KnownTypes.Add(typeof(ListCarouselInstance));
				KnownTypes.Add(typeof(ListDropdownInstance));
				KnownTypes.Add(typeof(ListInstance));
				KnownTypes.Add(typeof(ListMultiselectInstance));
				KnownTypes.Add(typeof(ProgressBarInstance));
				KnownTypes.Add(typeof(ScrollBarInstance));
				KnownTypes.Add(typeof(SliderInstance));
				KnownTypes.Add(typeof(TextboxInstance));
				KnownTypes.Add(typeof(TextButtonInstance));
			}
			public Type BindToType(string assemblyName, string typeName)
			{
				for(int i = 0; i < KnownTypes.Count; i++)
					if(KnownTypes[i].ToString() == typeName)
						return KnownTypes[i];
				return default;
			}
			public void BindToName(Type serializedType, out string assemblyName, out string typeName)
			{
				assemblyName = null;
				typeName = serializedType.ToString();
			}
		}

		public static void Whitelist(this Type type)
		{
			if(JsonBinder.Instance.KnownTypes.Contains(type))
				return;

			JsonBinder.Instance.KnownTypes.Add(type);
		}
		public static T Duplicate<T>(this T obj) => FromJSON<T>(ToJSON(obj));
		/// <summary>
		/// Tries to convert a <paramref name="JSON"/> <see cref="string"/> into <typeparamref name="T"/> 
		/// <paramref name="instance"/> and returns it if successful. Otherwise returns 
		/// <paramref name="default"/>(<typeparamref name="T"/>).
		/// </summary>
		public static T FromJSON<T>(this string JSON)
		{
			try
			{
				var settings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.All,
					SerializationBinder = JsonBinder.Instance,
				};
				return JsonConvert.DeserializeObject<T>(JSON, settings);
			}
			catch(Exception ex)
			{
				var whitelist = JsonBinder.Instance.KnownTypes;
				var wt = "";

				for(int i = 0; i < whitelist.Count; i++)
				{
					var name = whitelist[i].GetPrettyName();
					if(name.Contains('[') || name.Contains('<'))
						break;

					wt += i == 0 ? "" : ", ";
					wt += i != 0 && i % 5 == 0 ? "\n" : "";
					wt += name;
				}
				wt += $".\nAs well as any generic Array, List, ReadOnlyCollection and Dictionary of those.\n" +
					$"Also internal {nameof(SMPL)} stuff.";

				var msg = ex.Message;
				if(msg.Contains("was not resolved."))
				{
					msg = msg.Split("was not resolved.")[0];
					msg += "is not whitelisted.";
				}

				Console.LogError(1, $"Could not turn {nameof(JSON)} into {typeof(T).Name}.\n{msg}",
					$"It may be an invalid JSON or contain/be a non-whitelisted {nameof(Type)}.\n\n" +
					$"The whitelisted {nameof(Type)}s are:\n{wt}\n\n" +
					$"Make sure to whitelist any required {nameof(Type)} by 'typeof(MyType).Whitelist()'.\n" +
					$"But be careful not to grant a player the ability to do so.\n" +
					$"That might to lead all sorts of vulnerabilities and code injections which might affect other players upon distribution.");
				return default;
			}
		}
		/// <summary>
		/// Tries to convert <paramref name="instance"/> into a <paramref name="JSON"/> <see cref="string"/> 
		/// and returns it if successful.
		/// </summary>
		public static string ToJSON(this object instance)
		{
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				SerializationBinder = JsonBinder.Instance,
			};
			return JsonConvert.SerializeObject(instance, settings);
		}
		public static T Load<T>(this string filePath)
		{
			byte[] decompressedBytes;
			var compressedStream = new MemoryStream(File.ReadAllBytes(filePath));

			using(var decompressorStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
			{
				using var decompressedStream = new MemoryStream();
				decompressorStream.CopyTo(decompressedStream);

				decompressedBytes = decompressedStream.ToArray();
			}

			var json = Encoding.UTF8.GetString(decompressedBytes);
			return json.FromJSON<T>();
		}
		public static void Save(this object instance, string filePath)
		{
			byte[] compressedBytes;
			using(var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(instance.ToJSON())))
			{
				using var compressedStream = new MemoryStream();
				using(var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Fastest, true))
				{
					uncompressedStream.CopyTo(compressorStream);
				}

				compressedBytes = compressedStream.ToArray();
			}

			File.WriteAllBytes(filePath, compressedBytes);
		}
	}
}
