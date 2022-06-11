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

		protected virtual void OnStart() { }
		protected virtual void OnUpdate() { }
		protected virtual void OnStop() { }
		protected virtual void OnGameStop() { }

		#region Backend
		private static Scene scene, loadScene, unloadScene, startScene, stopScene;
		private static Thread assetsLoading;

		internal static Camera MainCamera => Thing.Get<Camera>(MainCameraUID);
		internal Dictionary<string, Texture> Textures { get; } = new();
		internal Dictionary<string, Music> Music { get; } = new();
		internal Dictionary<string, Sound> Sounds { get; } = new();
		internal Dictionary<string, Font> Fonts { get; } = new();
		internal Dictionary<string, Shader> Shaders { get; } = new();
		internal Dictionary<string, Database> Databases { get; } = new();
		//internal Dictionary<string, Sprite3D> Sprites3D { get; } = new();

		internal void LoadAssets()
		{
			var assets = new AssetQueue();
			var loadedCount = 0;

			if(assets.Textures == null && assets.Sounds == null && assets.Music == null && assets.Fonts == null && assets.TexturedModels3D == null)
			{
				CurrentScene.LoadingPercent = 100;
				return;
			}

			for(int i = 0; i < assets.Textures?.Count; i++)
			{
				if(assets.Textures[i] != null)
				{
					try
					{
						var t = new Texture(assets.Textures[i]);
						Textures[assets.Textures[i]] = t;
						t.Repeated = true;
					}
					catch(Exception) { Textures[assets.Textures[i]] = null; Console.LogError(-1, $"Could not load texture '{assets.Textures[i]}'."); }
				}
				UpdateLoadingPercent();
			}
			for(int i = 0; i < assets.Sounds?.Count; i++)
			{
				if(assets.Sounds[i] != null)
				{
					try { Sounds[assets.Sounds[i]] = new Sound(new SoundBuffer(assets.Sounds[i])); }
					catch(Exception) { Sounds[assets.Sounds[i]] = null; Console.LogError(-1, $"Could not load sound '{assets.Sounds[i]}'."); }
				}
				UpdateLoadingPercent();
			}
			for(int i = 0; i < assets.Music?.Count; i++)
			{
				if(assets.Music[i] != null)
				{
					try { Music[assets.Music[i]] = new Music(assets.Music[i]); }
					catch(Exception) { Music[assets.Music[i]] = null; Console.LogError(-1, $"Could not load music '{assets.Music[i]}'."); }
				}
				UpdateLoadingPercent();
			}
			for(int i = 0; i < assets.Fonts?.Count; i++)
			{
				if(assets.Fonts[i] != null)
				{
					try { Fonts[assets.Fonts[i]] = new Font(assets.Fonts[i]); }
					catch(Exception) { Fonts[assets.Fonts[i]] = null; Console.LogError(-1, $"Could not load font '{assets.Fonts[i]}'."); }
				}
				UpdateLoadingPercent();
			}
			//for(int i = 0; i < assets.TexturedModels3D?.Count; i++)
			//{
			//	try { Sprites3D[assets.TexturedModels3D[i].UniqueName] = new Sprite3D(assets.TexturedModels3D[i]); }
			//	catch(Exception)
			//	{ Sprites3D[assets.TexturedModels3D[i].UniqueName] = null; Console.LogError(-1, $"Could not load textured 3D model '{assets.TexturedModels3D[i]}'."); }
			//	UpdateLoadingPercent();
			//}
			for(int i = 0; i < assets.Databases?.Count; i++)
			{
				if(assets.Databases[i] != null)
					Databases[assets.Databases[i]] = Database.Load(assets.Databases[i]);
				UpdateLoadingPercent();
			}

			void UpdateLoadingPercent()
			{
				loadedCount++;

				var total = GetCount(assets.Fonts) + GetCount(assets.Music) + GetCount(assets.Sounds) + GetCount(assets.Textures) +
					GetCount(assets.TexturedModels3D) + GetCount(assets.Databases);
				CurrentScene.LoadingPercent = loadedCount.Map(0, total, 0, 100);

				int GetCount<T>(List<T> list) => list?.Count ?? 0;
			}
		}
		internal void UnloadAssets()
		{
			DisposeAndClear(Textures);
			DisposeAndClear(Fonts);
			DisposeAndClear(Music);
			DisposeAndClear(Sounds);
			DisposeAndClear(Shaders);

			//Sprites3D.Clear();

			void DisposeAndClear<T>(Dictionary<string, T> assets) where T : IDisposable
			{
				foreach(var kvp in assets)
					kvp.Value.Dispose();

				assets.Clear();
			}
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
					scene.LoadAssets();
					startScene = loadScene;
					loadScene = null;
				}
			}
		}
		#endregion
	}
}
