namespace SMPL
{
	public class Scene
	{
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
				if(value == null)
				{
					Game.Stop();
					return;
				}
				unloadScene = scene;
				scene = value;
				LoadingScene?.OnStart();
				loadScene = value;
			}
		}
		public static Scene LoadingScene { get; set; }

		public const string MAIN_CAMERA_UID = "MainCamera";
		public string SavePath { get; protected set; }
		public string AssetsDirectory { get; protected set; } = "Assets";
		public bool IsLoaded { get; private set; }

		//public ThemeUI ThemeUI { get; set; }

		[JsonConstructor]
		internal Scene() { }
		public Scene(params string[] initialAssetPaths)
		{
			assetQueue = new(initialAssetPaths);
		}
		public Scene(List<string> initialAssetPaths)
		{
			assetQueue = new(initialAssetPaths);
		}

		public static T Load<T>(string path) where T : Scene
		{
			if(path == null)
				return default;

			try
			{
				var def = new Scene();
				var prevScene = scene;
				scene = def;
				var json = File.ReadAllText(path).Decompress();
				var loadedScene = JsonConvert.DeserializeObject<T>(json);

				loadedScene.objs = def.objs;
				loadedScene.IsLoaded = true;
				scene = prevScene;

				return loadedScene;
			}
			catch(Exception) { Console.LogError(1, $"Could not load {nameof(Scene)} from '{path}'."); }
			return default;
		}

		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnStop() { }
		protected virtual void OnGameStop() { }

		protected void LoadAssets(params string[] paths)
		{
			if(string.IsNullOrWhiteSpace(AssetsDirectory) == false)
				Directory.CreateDirectory(AssetsDirectory);

			for(int i = 0; i < paths?.Length; i++)
			{
				var path = paths[i];
				if(loadedAssets.Contains(path))
					return;

				if(path == null)
				{
					Console.LogError(-1, "Could not load asset with its path being 'null'.");
					return;
				}
				var isFolder = Path.HasExtension(path) == false;

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

				var extension = Path.GetExtension(path);
				var filePath = $"{AssetsDirectory}\\{path}";

				try
				{
					if(extension == ".png" || extension == ".jpg" || extension == ".bmp")
						Textures[path] = new(filePath);
					else if(extension == ".ogg" || extension == ".wav" || extension == ".flac")
					{
						var music = new Music(filePath);
						if(music.Duration.AsSeconds() > 15)
							Music[path] = music;
						else
						{
							Sounds[path] = new(new SoundBuffer(filePath));
							music.Dispose();
						}
					}
					else if(extension == ".ttf" || extension == ".otf")
						Fonts[path] = new(filePath);
					else if(extension == ".vert")
						Shaders[path] = new(filePath, null, Visual.DEFAULT_FRAG);
					else if(extension == ".frag")
						Shaders[path] = new(Visual.DEFAULT_VERT, null, filePath);
					else if(extension == ".obj")
						Console.LogError(1, $"Work in progress...");
					else
						Files[path] = File.ReadAllText(filePath);

					assetQueue.Add(path);
					loadedAssets.Add(path);
				}
				catch(Exception) { Console.LogError(-1, $"Could not load asset at '{filePath}'."); }
			}
		}
		protected void UnloadAssets(params string[] paths)
		{
			for(int i = 0; i < paths?.Length; i++)
			{
				var path = paths[i];
				TryDisposeAndRemove(Textures, path);
				TryDisposeAndRemove(Fonts, path);
				TryDisposeAndRemove(Music, path);
				TryDisposeAndRemove(Sounds, path);
				TryDisposeAndRemove(Shaders, path);

				//TryRemove(Sprites3D, path);
				TryRemove(Files, path);
			}

			void TryDisposeAndRemove<T>(ConcurrentDictionary<string, T> assets, string key) where T : IDisposable
			{
				if(assets.ContainsKey(key) == false)
					return;

				assets[key].Dispose();
				assets.Remove(key, out _);
			}
			void TryRemove<T>(ConcurrentDictionary<string, T> assets, string key)
			{
				if(assets.ContainsKey(key) == false)
					return;

				assets.Remove(key, out _);
			}
		}
		protected void UnloadAllAssets()
		{
			DisposeAndClear(Textures);
			DisposeAndClear(Fonts);
			DisposeAndClear(Music);
			DisposeAndClear(Sounds);
			DisposeAndClear(Shaders);

			//Sprites3D.Clear();
			Files.Clear();

			void DisposeAndClear<T>(ConcurrentDictionary<string, T> assets) where T : IDisposable
			{
				foreach(var kvp in assets)
					kvp.Value.Dispose();

				assets.Clear();
			}
		}
		protected void Save()
		{
			if(SavePath == null)
				return;

			try
			{
				foreach(var kvp in objs)
				{
					TryAdd(cameras, kvp.Value);
					TryAdd(sprites, kvp.Value);
				}

				var json = JsonConvert.SerializeObject(this);
				File.WriteAllText(SavePath, json.Compress());
			}
			catch(Exception) { Console.LogError(1, $"Could not save {nameof(Scene)} at '{SavePath}'."); }

			void TryAdd<T>(Dictionary<string, T> dict, Thing thing) where T : Thing
			{
				if(thing is T t && dict.ContainsKey(t.UID) == false)
					dict[t.UID] = t;
			}
		}

		public override string ToString()
		{
			return $"{nameof(Scene)} '{SavePath}'";
		}

		#region Backend
		internal struct TexturedModel3D
		{
			public string TexturePath { get; set; }
			public string ObjModelPath { get; set; }
			public string UniqueName { get; set; }
			public uint TextureCount { get; set; }
			public float TextureDetail { get; set; }
			public Vector3 Scale { get; set; }

			public TexturedModel3D(string objPath, string texturePath, string uniqueName = null, uint textureCount = 20, float textureDetail = 20,
				float scaleX = 1, float scaleY = 1, float scaleZ = 1)
			{
				ObjModelPath = objPath;
				TexturePath = texturePath;
				UniqueName = uniqueName ?? objPath;
				TextureCount = textureCount;
				TextureDetail = textureDetail;
				Scale = new(scaleX, scaleY, scaleZ);
			}
		}

		private static Scene scene, loadScene, unloadScene, startScene, stopScene;
		internal static Thread assetsLoading;
		internal static Camera MainCamera => Thing.Get<Camera>(MAIN_CAMERA_UID);

		[JsonProperty]
		private ConcurrentBag<string> assetQueue = new();
		[JsonProperty]
		private Dictionary<string, Camera> cameras = new();
		[JsonProperty]
		private Dictionary<string, Sprite> sprites = new();

		internal Dictionary<string, Thing> objs = new();
		private ConcurrentBag<string> loadedAssets = new();
		internal ConcurrentDictionary<string, Texture> Textures { get; } = new();
		internal ConcurrentDictionary<string, Music> Music { get; } = new();
		internal ConcurrentDictionary<string, Sound> Sounds { get; } = new();
		internal ConcurrentDictionary<string, Font> Fonts { get; } = new();
		internal ConcurrentDictionary<string, Shader> Shaders { get; } = new();
		internal ConcurrentDictionary<string, string> Files { get; } = new();
		//internal Dictionary<string, Sprite3D> Sprites3D { get; } = new();

		internal void LoadInitialAssets()
		{
			foreach(var asset in assetQueue)
				LoadAssets(asset);

			loadedAssets = assetQueue;
		}
		internal void GameStop() => OnGameStop();

		internal static void UpdateCurrentScene()
		{
			if(stopScene != null)
			{
				stopScene = null;
				CurrentScene?.OnStop();
			}
			if(startScene != null)
			{
				LoadingScene?.OnStop();
				startScene = null;
				CurrentScene?.OnStart();
			}
			if(CurrentScene.assetQueue.Count < CurrentScene.loadedAssets.Count)
				LoadingScene?.OnUpdate();
			else
				CurrentScene?.OnUpdate();
		}
		internal static void ThreadLoadAssets()
		{
			while(true)
			{
				Thread.Sleep(1);

				if(CurrentScene != null && CurrentScene.loadedAssets.Count < CurrentScene.assetQueue.Count)
					foreach(var asset in CurrentScene.assetQueue)
						CurrentScene.LoadAssets(asset);

				if(unloadScene != null)
				{
					if(scene.IsLoaded == false)
					{
						ThingManager.DestroyAll();
						scene.UnloadAllAssets();
					}

					stopScene = unloadScene;
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
