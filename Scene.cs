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

		public Scene(string name, params string[] initialAssetPaths)
		{
			Name = name;

			for(int i = 0; i < initialAssetPaths?.Length; i++)
			{
				var a = initialAssetPaths[i];
				if(a != null)
					assetQueue.TryAdd(a, a);
			}
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
		public void LoadAssets(params string[] paths)
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
						LoadAssets(subFolders[j]);

					for(int j = 0; j < folderFiles.Length; j++)
						LoadAssets(folderFiles[j]);
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
		public void LoadTexturedModel(string objModelPath, string texturePath, Vector3 scale, int textureCount = 20, float textureDetail = 20)
		{
			if(objModelPath == null || texturePath == null || textureCount == 0 || File.Exists(objModelPath) == false || textureDetail <= 0)
				return;

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
				var objName = Path.GetFileNameWithoutExtension(objModelPath);

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
					var id = $"{objName}-{i}";
					Textures[id] = new Texture(layeredImages[i]);
					layeredImages[i].Dispose();
				}
				img?.Dispose();
			}
			catch(Exception) { Console.LogError(-1, $"Could not load textured model at '{objModelPath}' / '{texturePath}'."); }
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
		private bool hasStarted;
		private static Scene scene, loadScene, unloadScene, startScene, stopScene;
		internal static Thread assetsLoading;
		internal static CameraInstance MainCamera => ThingInstance.Get<CameraInstance>(MAIN_CAMERA_UID);

		[JsonProperty]
		private ConcurrentDictionary<string, string> assetQueue = new();
		[JsonProperty]
		private Dictionary<string, CameraInstance> cameras = new();
		[JsonProperty]
		private Dictionary<string, SpriteInstance> sprites = new();
		[JsonProperty]
		private Dictionary<string, LightInstance> lights = new();
		[JsonProperty]
		private Dictionary<string, TextInstance> texts = new();
		[JsonProperty]
		private Dictionary<string, NinePatchInstance> npatches = new();
		[JsonProperty]
		private Dictionary<string, AudioInstance> audio = new();
		[JsonProperty]
		private Dictionary<string, TilemapInstance> tilemaps = new();

		internal Dictionary<string, ThingInstance> objs = new();

		private ConcurrentDictionary<string, string> loadedAssets = new();
		internal ConcurrentDictionary<string, Texture> Textures { get; } = new();
		internal ConcurrentDictionary<string, Music> Music { get; } = new();
		internal ConcurrentDictionary<string, Sound> Sounds { get; } = new();
		internal ConcurrentDictionary<string, Font> Fonts { get; } = new();

		static Scene()
		{
			assetsLoading = new(ThreadLoadAssets) { IsBackground = true, Name = "AssetsLoading" };
			assetsLoading.Start();
		}

		[JsonConstructor]
		internal Scene() { }

		internal void LoadInitialAssets()
		{
			foreach(var asset in assetQueue)
				LoadAssets(asset.Key);

			loadedAssets = assetQueue;
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

				if(CurrentScene != null && CurrentScene.loadedAssets.Count < CurrentScene.assetQueue.Count)
					foreach(var asset in CurrentScene.assetQueue)
						CurrentScene.LoadAssets(asset.Key);

				if(unloadScene != null)
				{
					stopScene = unloadScene;
					unloadScene.UnloadAllAssets();
					unloadScene = null;
				}
				if(loadScene != null)
				{
					scene.LoadInitialAssets();
					startScene = loadScene;
					loadScene = null;
				}
			}
		}
		#endregion
	}
}
