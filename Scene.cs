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
		public bool IsLoaded { get; private set; }

		//public ThemeUI ThemeUI { get; set; }

		public Scene(params string[] initialAssetPaths)
		{
			Init(initialAssetPaths);
		}
		public Scene(List<string> initialAssetPaths)
		{
			Init(initialAssetPaths.ToArray());
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

		protected ReadOnlyCollection<string> GetAssetPaths()
		{
			var result = new List<string>();
			result.AddRange(Textures.Keys.ToList());
			result.AddRange(Fonts.Keys.ToList());
			result.AddRange(Music.Keys.ToList());
			result.AddRange(Sounds.Keys.ToList());
			result.AddRange(Files.Keys.ToList());
			return result.AsReadOnly();
		}
		protected bool AssetsAreLoaded(string path)
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
		protected void LoadAssets(params string[] paths)
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
					else if(extension == ".obj")
						Console.LogError(1, $"Work in progress...");
					else
						Files[path] = File.ReadAllText(path);

					assetQueue.TryAdd(path, path);
					loadedAssets.TryAdd(path, path);
				}
				catch(Exception) { Console.LogError(-1, $"Could not load asset at '{path}'."); }
			}
		}
		protected void UnloadAssets(params string[] paths)
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
			void TryRemove<T>(ConcurrentDictionary<string, T> assets, string key)
			{
				if(assets.ContainsKey(key) == false)
					return;

				assets.Remove(key, out _);
			}
			void Remove(string path)
			{
				TryDisposeAndRemove(Textures, path);
				TryDisposeAndRemove(Fonts, path);
				TryDisposeAndRemove(Music, path);
				TryDisposeAndRemove(Sounds, path);

				//TryRemove(Sprites3D, path);
				TryRemove(Files, path);

				loadedAssets.TryRemove(path, out _);
				assetQueue.TryRemove(path, out _);
			}
		}
		protected void UnloadAllAssets()
		{
			DisposeAndClear(Textures);
			DisposeAndClear(Fonts);
			DisposeAndClear(Music);
			DisposeAndClear(Sounds);

			//Sprites3D.Clear();
			Files.Clear();

			loadedAssets.Clear();
			assetQueue.Clear();

			void DisposeAndClear<T>(ConcurrentDictionary<string, T> assets) where T : IDisposable
			{
				foreach(var kvp in assets)
					kvp.Value.Dispose();

				assets.Clear();
			}
		}
		protected bool Save(string filePath)
		{
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

				var assets = GetAssetPaths();
				for(int i = 0; i < assets.Count; i++)
					assetQueue.TryAdd(assets[i], assets[i]);

				foreach(var kvp in objs)
				{
					TryAdd(cameras, kvp.Value);
					TryAdd(sprites, kvp.Value);
					TryAdd(lights, kvp.Value);
					TryAdd(texts, kvp.Value);
					TryAdd(npatches, kvp.Value);
					TryAdd(audio, kvp.Value);
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
					dict[t.UID] = t;
			}
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

		internal Dictionary<string, ThingInstance> objs = new();
		private ConcurrentDictionary<string, string> loadedAssets = new();
		internal ConcurrentDictionary<string, Texture> Textures { get; } = new();
		internal ConcurrentDictionary<string, Music> Music { get; } = new();
		internal ConcurrentDictionary<string, Sound> Sounds { get; } = new();
		internal ConcurrentDictionary<string, Font> Fonts { get; } = new();
		internal ConcurrentDictionary<string, string> Files { get; } = new();
		//internal Dictionary<string, Sprite3D> Sprites3D { get; } = new();

		[JsonConstructor]
		internal Scene() { }

		private void Init(string[] initAssetPaths)
		{
			if(initAssetPaths == null)
				return;

			for(int i = 0; i < initAssetPaths.Length; i++)
			{
				var a = initAssetPaths[i];
				if(a != null)
					assetQueue.TryAdd(a, a);
			}
		}

		internal void LoadInitialAssets()
		{
			foreach(var asset in assetQueue)
				LoadAssets(asset.Key);

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
				CurrentScene.hasStarted = true;
				CurrentScene?.OnStart();
			}
			if(CurrentScene.hasStarted == false)
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
						CurrentScene.LoadAssets(asset.Key);

				if(unloadScene != null)
				{
					if(scene.IsLoaded == false)
					{
						Thing.DestroyAll();
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
