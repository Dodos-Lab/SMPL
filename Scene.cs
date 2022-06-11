namespace SMPL
{
	public class Scene
	{
		public struct TexturedModel3D
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
		public struct AssetQueue
		{
			public List<string> Textures { get; set; }
			public List<string> Music { get; set; }
			public List<string> Sounds { get; set; }
			public List<string> Fonts { get; set; }
			public List<string> Shaders { get; set; }
			public List<string> Databases { get; set; }
			public List<TexturedModel3D> TexturedModels3D { get; set; }
		}

		public static string MainCameraUID { get; set; }
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

		public float LoadingPercent { get; private set; }
		//public ThemeUI ThemeUI { get; set; }

		public Scene(params string[] initialAssetPaths)
		{
			initialAssetPaths?.CopyTo(assets, 0);
		}

		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnStop() { }
		protected virtual void OnGameStop() { }

		protected void LoadAssets(string path)
		{
			if(path == null || IsValidFilename(path))
			{
				Console.LogError(1, $"The path '{path}' is invalid.");
				return;
			}
			var isFolder = Path.HasExtension(path) == false;

			if(isFolder)
			{
				var folderFiles = Directory.GetFiles(path);
				var subFolders = Directory.GetDirectories(path);

				for(int i = 0; i < subFolders.Length; i++)
					LoadAssets(subFolders[i]);

				for(int i = 0; i < folderFiles.Length; i++)
					LoadAssets(folderFiles[i]);
				return;
			}

			var extension = Path.GetExtension(path);

			try
			{
				if(extension == ".png" || extension == ".jpg" || extension == ".bmp")
					Textures[path] = new Texture(path);
				else if(extension == ".ogg" || extension == ".wav" || extension == ".flac")
				{
					var music = new Music(path);
					if(music.Duration.AsSeconds() > 15)
						Music[path] = music;
					else
					{
						Sounds[path] = new(new SoundBuffer(path));
						music.Dispose();
					}
				}
				else if(extension == ".ttf" || extension == ".otf")
					Fonts[path] = new(path);
				else if(extension == ".vert")
					Shaders[path] = new(path, null, Visual.DEFAULT_FRAG);
				else if(extension == ".frag")
					Shaders[path] = new(Visual.DEFAULT_VERT, null, path);
				else if(extension == ".cdb")
					Databases[path] = Database.Load(path);
				else if(extension == ".obj")
					Console.LogError(1, $"Work in progress...");
				else
					Files[path] = File.ReadAllText(path);
			}
			catch(Exception) { Console.LogError(1, $"Could not load asset at path '{path}'."); }

			bool IsValidFilename(string fileName)
			{
				var invalidChars = Path.GetInvalidFileNameChars();
				for(int i = 0; i < invalidChars.Length; i++)
					if(fileName.Contains(invalidChars[i]))
						return true;
				return false;
			}
		}
		protected void UnloadAssets(string path)
		{
			TryDisposeAndRemove(Textures, path);
			TryDisposeAndRemove(Fonts, path);
			TryDisposeAndRemove(Music, path);
			TryDisposeAndRemove(Sounds, path);
			TryDisposeAndRemove(Shaders, path);

			//TryRemove(Sprites3D, path);
			TryRemove(Databases, path);
			TryRemove(Files, path);

			void TryDisposeAndRemove<T>(Dictionary<string, T> assets, string key) where T : IDisposable
			{
				if(assets.ContainsKey(key) == false)
					return;

				assets[key].Dispose();
				assets.Remove(key);
			}
			void TryRemove<T>(Dictionary<string, T> assets, string key)
			{
				if(assets.ContainsKey(key) == false)
					return;

				assets.Remove(key);
			}
		}
		protected void UnloadAssets()
		{
			DisposeAndClear(Textures);
			DisposeAndClear(Fonts);
			DisposeAndClear(Music);
			DisposeAndClear(Sounds);
			DisposeAndClear(Shaders);

			//Sprites3D.Clear();
			Databases.Clear();
			Files.Clear();

			void DisposeAndClear<T>(Dictionary<string, T> assets) where T : IDisposable
			{
				foreach(var kvp in assets)
					kvp.Value.Dispose();

				assets.Clear();
			}
		}

		#region Backend
		private static Scene scene, loadScene, unloadScene, startScene, stopScene;
		private static Thread assetsLoading;
		private readonly string[] assets;

		internal static Camera MainCamera => Thing.Get<Camera>(MainCameraUID);
		internal Dictionary<string, Texture> Textures { get; } = new();
		internal Dictionary<string, Music> Music { get; } = new();
		internal Dictionary<string, Sound> Sounds { get; } = new();
		internal Dictionary<string, Font> Fonts { get; } = new();
		internal Dictionary<string, Shader> Shaders { get; } = new();
		internal Dictionary<string, Database> Databases { get; } = new();
		internal Dictionary<string, string> Files { get; } = new();
		//internal Dictionary<string, Sprite3D> Sprites3D { get; } = new();

		internal void LoadPreparedAssets()
		{
			for(int i = 0; i < assets.Length; i++)
				LoadAssets(assets[i]);
		}
		internal void GameStop() => OnGameStop();

		internal static void Init(Scene startingScene, Scene loadingScene)
		{
			LoadingScene = loadingScene;
			CurrentScene = startingScene;

			assetsLoading = new(ThreadLoadAssets) { IsBackground = true, Name = "AssetsLoading" };
			assetsLoading.Start();
		}
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
			if(CurrentScene?.LoadingPercent < 100)
				LoadingScene?.OnUpdate();
			else
				CurrentScene?.OnUpdate();
		}
		internal static void ThreadLoadAssets()
		{
			while(true)
			{
				Thread.Sleep(1);
				if(unloadScene != null)
				{
					scene.UnloadAssets();
					stopScene = unloadScene;
					unloadScene = null;
				}
				if(loadScene != null)
				{
					scene.LoadPreparedAssets();
					startScene = loadScene;
					loadScene = null;
				}
			}
		}
		#endregion
	}
}
