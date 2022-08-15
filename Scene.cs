namespace SMPL
{
	public sealed class Scene
	{
		public class ObjModel
		{
			public struct TexturedModelInfo
			{
				public string ObjModelPath { get; set; }
				public string TexturePath { get; set; }
				public int TextureCount { get; set; }
				public float TextureDetail { get; set; }
				public Vector3 Scale { get; set; }

				public TexturedModelInfo(string objPath, string texturePath, int textureCount = 20, float textureDetail = 20,
					float scaleX = 1, float scaleY = 1, float scaleZ = 1)
				{
					ObjModelPath = objPath;
					TexturePath = texturePath;
					TextureCount = textureCount;
					TextureDetail = textureDetail;
					Scale = new(scaleX, scaleY, scaleZ);
				}
			}
		}

		public static Vector2 MouseCursorPosition
		{
			get => MainCamera.MouseCursorPosition;
			set => MainCamera.MouseCursorPosition = value;
		}
		public static Scene CurrentScene
		{
			get => scene;
			set
			{
				if(value == scene)
					return;

				if(value == null)
				{
					Game.Stop();
					return;
				}

				Thing.DestroyAll();
				unloadScene = scene;
				scene = value;

				if(LoadingScene != null)
					Event.SceneStart(LoadingScene.Name);

				loadScene = value;
			}
		}
		public static Scene LoadingScene { get; set; }
		public const string MAIN_CAMERA_UID = "MainCamera";

		public bool IsLoaded { get; private set; }
		public string Name { get; set; }

		public Scene(string name)
		{
			Name = name;
		}
		public static void Load(string filePath)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				return;

			try
			{
				var def = new Scene();
				var prevScene = scene;
				scene = def;
				var json = File.ReadAllText(filePath).Decompress();
				var loadedScene = JsonConvert.DeserializeObject<Scene>(json);

				loadedScene.objs = def.objs;
				loadedScene.IsLoaded = true;
				scene = prevScene;

				foreach(var kvp in loadedScene.tilemaps)
					kvp.Value.MapFromJSON();

				CurrentScene = loadedScene;
			}
			catch(Exception) { Console.LogError(1, $"Could not load {nameof(Scene)} from '{filePath}'."); }
		}
		public bool Save(string directoryPath)
		{
			var filePath = Path.Join(directoryPath, $"{Name}.scene");
			try
			{
				// clear these in case of previous save/load
				assetQueue.Clear();
				cameras.Clear();
				sprites.Clear();
				lights.Clear();
				texts.Clear();
				npatches.Clear();
				audio.Clear();
				tilemaps.Clear();

				var assets = GetAssetPaths();
				for(int i = 0; i < assets.Count; i++)
					if(File.Exists(assets[i]))
						assetQueue.TryAdd(assets[i], assets[i]);

				foreach(var kvp in objs)
				{
					TryAdd(cameras, kvp.Value);
					TryAdd(sprites, kvp.Value);
					TryAdd(lights, kvp.Value);
					TryAdd(texts, kvp.Value);
					TryAdd(npatches, kvp.Value);
					TryAdd(audio, kvp.Value);
					TryAdd(tilemaps, kvp.Value);
				}

				var json = JsonConvert.SerializeObject(this);
				File.WriteAllText(filePath, json.Compress());
				return true;
			}
			catch(Exception)
			{
				Console.LogError(1, $"Could not save {nameof(Scene)} at '{filePath}'.");
				return false;
			}

			void TryAdd<T>(Dictionary<string, T> dict, ThingInstance thing) where T : ThingInstance
			{
				if(thing is T t && dict.ContainsKey(t.UID) == false)
				{
					dict[t.UID] = t;

					if(thing is TilemapInstance tilemap)
						tilemap.MapToJSON();
				}
			}
		}

		public ReadOnlyCollection<string> GetAssetPaths()
		{
			var result = new List<string>();
			result.AddRange(Textures.Keys.ToList());
			result.AddRange(Fonts.Keys.ToList());
			result.AddRange(Music.Keys.ToList());
			result.AddRange(Sounds.Keys.ToList());
			return result.AsReadOnly();
		}
		public bool AssetsAreLoaded(string path)
		{
			var assets = GetAssetPaths();
			return Directory.Exists(path) ? Dir(path) : File(path);

			bool Dir(string path)
			{
				var dirs = Directory.GetDirectories(path);
				var files = Directory.GetFiles(path);

				for(int i = 0; i < dirs.Length; i++)
					if(Dir(dirs[i]) == false)
						return false;

				for(int i = 0; i < files.Length; i++)
					if(File(files[i]) == false)
						return false;

				return true;
			}
			bool File(string path)
			{
				return assets.Contains(path);
			}
		}
		public void LoadAssets(params string[] assetPaths)
		{
			for(int i = 0; i < assetPaths?.Length; i++)
				assetQueue.TryAdd(assetPaths[i], assetPaths[i]);
		}
		public string[] LoadSpriteStack(string objModelPath, string texturePath, Vector3 modelScale, Vector3 modelRotation = default,
			int stackCount = 20, float stackDetail = 20)
		{
			if(objModelPath == null || texturePath == null || File.Exists(objModelPath) == false || File.Exists(texturePath) == false ||
				stackCount <= 1 || stackDetail <= 0)
				return default;

			var spriteStackInfo = new SpriteStackInfo(objModelPath, texturePath, modelScale, modelRotation, stackCount, stackDetail);
			var name = spriteStackInfo.Name;
			spriteStackQueue.TryAdd(name, spriteStackInfo);

			var result = new string[stackCount];
			for(int i = 0; i < result.Length; i++)
				result[i] = $"{name}-{i}";

			return result;
		}
		public void UnloadAssets(params string[] paths)
		{
			for(int i = 0; i < paths?.Length; i++)
			{
				var path = paths[i];
				if(Path.HasExtension(path))
				{
					Remove(path);
					continue;
				}

				var dirs = Directory.GetDirectories(path);
				var files = Directory.GetFiles(path);

				for(int j = 0; j < dirs.Length; j++)
					UnloadAssets(dirs[j]);
				for(int j = 0; j < files.Length; j++)
					Remove(files[j]);
			}

			void TryDisposeAndRemove<T>(ConcurrentDictionary<string, T> assets, string key) where T : IDisposable
			{
				if(assets.ContainsKey(key) == false)
					return;

				assets[key].Dispose();
				assets.Remove(key, out _);
			}
			void Remove(string path)
			{
				TryDisposeAndRemove(Textures, path);
				TryDisposeAndRemove(Fonts, path);
				TryDisposeAndRemove(Music, path);
				TryDisposeAndRemove(Sounds, path);

				loadedAssets.TryRemove(path, out _);
				assetQueue.TryRemove(path, out _);
			}
		}
		public void UnloadAllAssets()
		{
			DisposeAndClear(Textures);
			DisposeAndClear(Fonts);
			DisposeAndClear(Music);
			DisposeAndClear(Sounds);

			loadedAssets.Clear();
			assetQueue.Clear();

			void DisposeAndClear<T>(ConcurrentDictionary<string, T> assets) where T : IDisposable
			{
				foreach(var kvp in assets)
					kvp.Value.Dispose();

				assets.Clear();
			}
		}

		#region Backend
		internal struct SpriteStackInfo
		{
			public string Name => Path.GetFileNameWithoutExtension(ObjModelPath);
			public string ObjModelPath { get; set; }
			public string TexturePath { get; set; }
			public Vector3 Scale { get; set; }
			public Vector3 Rotation { get; set; }
			public int TextureCount { get; set; }
			public float TextureDetail { get; set; }

			public SpriteStackInfo(string objModelPath, string texturePath, Vector3 scale, Vector3 rot = default, int textureCount = 20, float textureDetail = 20)
			{
				ObjModelPath = objModelPath;
				TexturePath = texturePath;
				Scale = scale;
				Rotation = rot;
				TextureCount = textureCount.Limit(2, 1000);
				TextureDetail = textureDetail.Limit(1, 1000);
			}
		}

		private bool hasStarted;
		private static Scene scene, loadScene, unloadScene, startScene, stopScene;
		internal static Thread assetsLoading;
		internal static CameraInstance MainCamera => ThingInstance.Get<CameraInstance>(MAIN_CAMERA_UID);

		internal Dictionary<string, ThingInstance> objs = new();
		internal readonly ConcurrentDictionary<string, string> loadedAssets = new();
		internal ConcurrentDictionary<string, Texture> Textures { get; } = new();
		internal ConcurrentDictionary<string, Music> Music { get; } = new();
		internal ConcurrentDictionary<string, Sound> Sounds { get; } = new();
		internal ConcurrentDictionary<string, Font> Fonts { get; } = new();

		[JsonProperty]
		private readonly ConcurrentDictionary<string, SpriteStackInfo> spriteStackQueue = new();
		[JsonProperty]
		private readonly ConcurrentDictionary<string, string> assetQueue = new();
		[JsonProperty]
		private readonly Dictionary<string, CameraInstance> cameras = new();
		[JsonProperty]
		private readonly Dictionary<string, SpriteInstance> sprites = new();
		[JsonProperty]
		private readonly Dictionary<string, LightInstance> lights = new();
		[JsonProperty]
		private readonly Dictionary<string, TextInstance> texts = new();
		[JsonProperty]
		private readonly Dictionary<string, NinePatchInstance> npatches = new();
		[JsonProperty]
		private readonly Dictionary<string, AudioInstance> audio = new();
		[JsonProperty]
		private readonly Dictionary<string, TilemapInstance> tilemaps = new();

		static Scene()
		{
			assetsLoading = new(ThreadLoadAssets) { IsBackground = true, Name = "AssetsLoading" };
			assetsLoading.Start();
		}

		[JsonConstructor]
		internal Scene() { }

		internal void LoadSpriteStackBackground(SpriteStackInfo spriteStackInfo)
		{
			var texturePath = spriteStackInfo.TexturePath;
			var objModelPath = spriteStackInfo.ObjModelPath;
			var scale = spriteStackInfo.Scale;
			var textureDetail = spriteStackInfo.TextureDetail;
			var textureCount = spriteStackInfo.TextureCount;
			var rot = spriteStackInfo.Rotation;
			rot.X = rot.X.DegreesToRadians();
			rot.Y = rot.Y.DegreesToRadians();
			rot.Z = rot.Z.DegreesToRadians();

			try
			{
				var img = default(Image);
				if(File.Exists(texturePath))
					img = new Image(texturePath);

				var content = File.ReadAllText(objModelPath).Replace('\r', ' ').Split('\n');
				var layeredImages = new List<Image>();
				var indexTexCoords = new List<int>();
				var indexVert = new List<int>();
				var texCoords = new List<Vector3>();
				var verts = new List<Vector3>();
				var boundingBoxA = new Vector3();
				var boundingBoxB = new Vector3();

				for(int i = 0; i < content.Length; i++)
				{
					var split = content[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
					if(split.Length == 0)
						continue;

					switch(split[0])
					{
						case "v":
							{
								var v = new Vector3(N(1), N(2), N(3)) * scale;
								var matrix = Matrix4x4.CreateTranslation(v);
								matrix *= Matrix4x4.CreateFromYawPitchRoll(rot.X, rot.Y, rot.Z);
								v = matrix.Translation;

								verts.Add(v);

								if(v.X < boundingBoxA.X)
									boundingBoxA.X = v.X;
								if(v.Y < boundingBoxA.Y)
									boundingBoxA.Y = v.Y;
								if(v.Z < boundingBoxA.Z)
									boundingBoxA.Z = v.Z;

								if(v.X > boundingBoxB.X)
									boundingBoxB.X = v.X;
								if(v.Y > boundingBoxB.Y)
									boundingBoxB.Y = v.Y;
								if(v.Z > boundingBoxB.Z)
									boundingBoxB.Z = v.Z;

								break;
							}
						case "vt": texCoords.Add(new(N(1), 1 - N(2), 1)); break;
						case "f":
							{
								for(int j = 1; j < split.Length; j++)
								{
									var face = split[j].Split('/');
									indexVert.Add(int.Parse(face[0]) - 1);

									if(face.Length > 1 && face[1].Length != 0)
										indexTexCoords.Add(int.Parse(face[1]) - 1);
								}
								break;
							}
					}
					float N(int i) => float.Parse(split[i]);
				}

				var vertsOffset = boundingBoxA;
				var detail = textureDetail;
				boundingBoxB -= vertsOffset;
				var textureSize = boundingBoxB * detail;

				for(int i = 0; i < textureCount; i++)
					layeredImages.Add(new Image((uint)textureSize.X, (uint)textureSize.Z, Color.Transparent));

				for(int i = 0; i < indexVert.Count; i++)
				{
					var p = verts[indexVert[i]];
					var tx = i < indexTexCoords.Count ? texCoords[indexTexCoords[i]] : default;
					var resultTexCoordsX = (uint)(p.X - vertsOffset.X).Map(0, boundingBoxB.X, 0, (uint)textureSize.X - 1);
					var resultTexCoordsY = (uint)(p.Z - vertsOffset.Z).Map(0, boundingBoxB.Z, 0, (uint)textureSize.Z - 1);
					var textureIndex = (int)(p.Y - vertsOffset.Y).Map(0, boundingBoxB.Y, 0, textureCount - 1);
					var color = img == null ? Color.White : img.GetPixel((uint)(tx.X * img.Size.X), (uint)(tx.Y * img.Size.Y));

					layeredImages[textureIndex].SetPixel(resultTexCoordsX, resultTexCoordsY, color);
				}

				for(int i = 0; i < layeredImages.Count; i++)
				{
					var id = $"{spriteStackInfo.Name}-{i}";
					Textures[id] = new Texture(layeredImages[i]);
					assetQueue.TryAdd(id, id);
					loadedAssets.TryAdd(id, id);

					layeredImages[i].Dispose();
				}

				img?.Dispose();
			}
			catch(Exception) { Console.LogError(-1, $"Could not load textured model at '{objModelPath}' / '{texturePath}'."); }
		}
		internal void LoadAssetsBackground(params string[] paths)
		{
			for(int i = 0; i < paths?.Length; i++)
			{
				var path = paths[i];
				if(loadedAssets.ContainsKey(path))
					return;

				if(string.IsNullOrWhiteSpace(path) || (Directory.Exists(path) == false && File.Exists(path) == false))
				{
					Console.LogError(-1, $"Could not load asset at '{path}'. The path is invalid or the file/directory is missing.");
					return;
				}

				var isFolder = Path.HasExtension(path) == false;
				var extension = Path.GetExtension(path);
				var key = path.Replace(AppContext.BaseDirectory, "");

				if(isFolder)
				{
					var folderFiles = Directory.GetFiles(path);
					var subFolders = Directory.GetDirectories(path);

					for(int j = 0; j < subFolders.Length; j++)
						LoadAssetsBackground(subFolders[j]);

					for(int j = 0; j < folderFiles.Length; j++)
						LoadAssetsBackground(folderFiles[j]);
					return;
				}
				try
				{
					if(extension == ".png" || extension == ".jpg" || extension == ".bmp")
						Textures[key] = new(path) { Repeated = true };
					else if(extension == ".ogg" || extension == ".wav" || extension == ".flac")
					{
						var music = new Music(path);
						if(music.Duration.AsSeconds() > 15)
							Music[key] = music;
						else
						{
							Sounds[key] = new(new SoundBuffer(path));
							music.Dispose();
						}
					}
					else if(extension == ".ttf" || extension == ".otf")
						Fonts[key] = new(path);

					assetQueue.TryAdd(path, path);
					loadedAssets.TryAdd(path, path);
				}
				catch(Exception) { Console.LogError(-1, $"Could not load asset at '{path}'."); }
			}
		}

		internal static void UpdateCurrentScene()
		{
			if(stopScene != null)
			{
				stopScene = null;
				if(CurrentScene != null)
					Event.SceneStop(CurrentScene.Name);
			}
			if(startScene != null)
			{
				if(LoadingScene != null)
					Event.SceneStop(LoadingScene.Name);
				startScene = null;
				CurrentScene.hasStarted = true;
				if(CurrentScene != null)
					Event.SceneStart(CurrentScene.Name);
			}
			if(CurrentScene.hasStarted == false && LoadingScene != null)
				Event.SceneUpdate(LoadingScene.Name);
			else if(CurrentScene.hasStarted && CurrentScene != null)
				Event.SceneUpdate(CurrentScene.Name);
		}
		internal static void ThreadLoadAssets()
		{
			while(true)
			{
				Thread.Sleep(1);

				if(CurrentScene != null)
				{
					if(CurrentScene.loadedAssets.Count < CurrentScene.assetQueue.Count)
						foreach(var asset in CurrentScene.assetQueue)
							CurrentScene.LoadAssetsBackground(asset.Key);

					if(CurrentScene.spriteStackQueue.IsEmpty == false)
						foreach(var spriteStack in CurrentScene.spriteStackQueue)
							CurrentScene.LoadSpriteStackBackground(spriteStack.Value);
				}

				if(unloadScene != null)
				{
					stopScene = unloadScene;
					unloadScene.UnloadAllAssets();
					unloadScene = null;
				}
				if(loadScene != null)
				{
					startScene = loadScene;
					loadScene = null;
				}
			}
		}
		#endregion
	}
}
