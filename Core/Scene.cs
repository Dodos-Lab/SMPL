using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;

namespace SMPL
{
	/// <summary>
	/// Used to handle multiple stages of the game (like menus, levels, loading screens etc). This class needs to be
	/// inherited in order to catch all the events and access all the data (by overriding the 'OnX' methods).<br></br><br></br>
	/// Each <see cref="Scene"/> has a few states:<br></br>
	/// - Unloaded: The scene just exists as an instance (perhaps in a scene manager), ready to be started.<br></br>
	/// - When a scene becomes the <see cref="CurrentScene"/> it requires some optional assets to be loaded in <see cref="OnRequireAssets"/>
	/// in which an instance of <see cref="AssetQueue"/> should be returned with all the paths and details about the assets (if any).<br></br>
	/// - After that the <see cref="LoadingScene"/> takes over while the <see cref="CurrentScene"/>'s assets are being loaded in the background.
	/// The progress could be checked through <see cref="LoadingPercent"/>.<br></br>
	/// - Loading assets for the <see cref="LoadingScene"/> happens by either setting it as a <see cref="CurrentScene"/> and through
	/// <see cref="OnRequireAssets"/> or by loading them manually in <see cref="OnStart"/> (which would freeze the game for the duration
	/// if there are many/large assets).<br></br>
	/// - The <see cref="CurrentScene"/> starts once its assets are loaded: <see cref="OnStart"/>. Here everything in the scene
	/// can be instantiated and initialized so that everything has the same state
	/// when restarting the scene.<br></br>
	/// - The game loop for that scene proceeds after that with <see cref="OnUpdate"/>.<br></br>
	/// - Until <see cref="OnStop"/> where a different <see cref="CurrentScene"/> is chosen. Here all the scene ending game logic
	/// should be handled and all assets unloaded. Then the whole process repeats.<br></br>
	/// - Or until <see cref="OnGameStop"/>. This is only for game logic since all game resources are unloaded and released regardless.
	/// </summary>
	public class Scene
	{
		/// <summary>
		/// Some values describing a text.
		/// </summary>
		public class TextDetails
      {
			/// <summary>
			/// This determines the positional offset from <see cref="Object.Position"/> as a vector.<br></br>
			/// Note: [0, 0] is the top left and [1, 1] is the bottom right corner of the text.
			/// Values can also go bellow 0 and above 1.
			/// </summary>
			public Vector2 OriginUnit { get; set; }

			/// <summary>
			/// On which <see cref="Camera"/> this <see cref="Visual"/> should be drawn. This value is set to <see cref="MainCamera"/>
			/// if it is null upon drawing.
			/// </summary>
			public Camera DrawTarget { get; set; } = MainCamera;
			/// <summary>
			/// The font used to draw the text.
			/// </summary>
			public Font Font { get; set; }
			/// <summary>
			/// The <see cref="string"/> text itself used upon drawing.
			/// </summary>
			public string Text { get; set; } = "Hello, World!";
			/// <summary>
			/// The color of the text.
			/// </summary>
			public Color Color { get; set; } = Color.White;
			/// <summary>
			/// The character size as provided in the <see cref="Font"/>.
			/// </summary>
			public uint CharacterSize { get; set; } = 32;
			/// <summary>
			/// The spacing inbetween characters.
			/// </summary>
			public float CharacterSpace { get; set; } = 1;
			/// <summary>
			/// The spacing inbetween lines.
			/// </summary>
			public float LineSpace { get; set; } = 1;
			/// <summary>
			/// The color of the outline of the <see cref="Text"/>. Make sure to have non-zero <see cref="OutlineSize"/>.
			/// </summary>
			public Color OutlineColor { get; set; } = Color.Black;
			/// <summary>
			/// The size of the outline of the <see cref="Text"/>.
			/// </summary>
			public float OutlineSize { get; set; }
			/// <summary>
			/// The style of the <see cref="Text"/>. May also have multiple as so:
			/// <code>Style = Styles.Bold | Styles.Underlined;</code>
			/// </summary>
			public Text.Styles Style { get; set; }

			public TextDetails(Font font) => Font = font;
		}

		/// <summary>
		/// Used for loading 3D models as a set of layered textures (aka sprite stacking). An instance of this
		/// needs to be passed inside <see cref="AssetQueue"/> in <see cref="OnRequireAssets"/>.
		/// </summary>
		public struct TexturedModel3D
      {
			/// <summary>
			/// The path to the texture that this .obj file uses.
			/// </summary>
			public string TexturePath { get; set; }
			/// <summary>
			/// The path to the .obj file.
			/// </summary>
			public string ObjModelPath { get; set; }
			/// <summary>
			/// An optional uniqiue ID/Name ussed for an access key when stored in <see cref="Sprites3D"/>. The <see cref="ObjModelPath"/>
			/// is used if null.
			/// </summary>
			public string UniqueName { get; set; }
			/// <summary>
			/// The amount of detail along the depth of the <see cref="Sprite3D"/>.
			/// </summary>
			public uint TextureCount { get; set; }
			/// <summary>
			/// The amount of detail along the width and height of the <see cref="Sprite3D"/>
			/// </summary>
			public float TextureDetail { get; set; }
			/// <summary>
			/// The scaling applied to the .obj file when loading it. Useful for flipping the model with negative values as well.
			/// </summary>
			public Vector3 Scale { get; set; }

			/// <summary>
			/// Initializes the .obj path and the texture path. Optionally initializes the rest of the values.
			/// </summary>
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
		/// <summary>
		/// Used to pass the paths and details of the needed assets to a <see cref="Scene"/> through <see cref="Scene.OnRequireAssets"/> to be loaded.
		/// </summary>
		public struct AssetQueue
		{
			/// <summary>
			/// All the paths of the required textures in the <see cref="CurrentScene"/>.
			/// </summary>
			public List<string> Textures { get; set; }
			/// <summary>
			/// All the paths of the required music in the <see cref="CurrentScene"/>.
			/// </summary>
			public List<string> Music { get; set; }
			/// <summary>
			/// All the paths of the required sounds in the <see cref="CurrentScene"/>.
			/// </summary>
			public List<string> Sounds { get; set; }
			/// <summary>
			/// All the paths of the required fonts in the <see cref="CurrentScene"/>.
			/// </summary>
			public List<string> Fonts { get; set; }
			/// <summary>
			/// All the paths and details of the required <see cref="Sprite3D"/>s in the <see cref="CurrentScene"/>.
			/// </summary>
			public List<TexturedModel3D> TexturedModels3D { get; set; }
		}

		private static Scene scene, loadScene, unloadScene, startScene, stopScene;
		private static Thread assetsLoading;

		/// <summary>
		/// The initial <see cref="Camera"/> accessible from everywhere.
		/// </summary>
		public static Camera MainCamera { get; internal set; }
		/// <summary>
		/// A shortcut toward the <see cref="MainCamera"/>'s <see cref="Camera.MouseCursorPosition"/>.
		/// </summary>
		public static Vector2 MouseCursorPosition
		{
			get => MainCamera.MouseCursorPosition;
			set => MainCamera.MouseCursorPosition = value;
		}
		/// <summary>
		/// A scene in which all the game logic is executing currently (unless assets for this scene are loading). See <see cref="Scene"/> for more info.
		/// </summary>
		public static Scene CurrentScene
		{
			get => scene;
			set
			{
				unloadScene = scene;
				scene = value;
				LoadingScene?.OnStart();
				loadScene = value;
			}
		}
		/// <summary>
		/// A scene that takes over whenever <see cref="CurrentScene"/> is loading. See <see cref="Scene"/> for more info.
		/// </summary>
		public static Scene LoadingScene { get; set; }

		/// <summary>
		/// The percentage of loaded assets whenever this scene is the <see cref="CurrentScene"/>.
		/// </summary>
		public float LoadingPercent { get; private set; }

		/// <summary>
		/// The loaded textures whenever this scene is the <see cref="CurrentScene"/>. See <see cref="Scene"/> for more info.
		/// </summary>
		protected Dictionary<string, Texture> Textures { get; } = new();
		/// <summary>
		/// The loaded music whenever this scene is the <see cref="CurrentScene"/>. See <see cref="Scene"/> for more info.
		/// </summary>
		protected Dictionary<string, Music> Music { get; } = new();
		/// <summary>
		/// The loaded sounds whenever this scene is the <see cref="CurrentScene"/>. See <see cref="Scene"/> for more info.
		/// </summary>
		protected Dictionary<string, Sound> Sounds { get; } = new();
		/// <summary>
		/// The loaded fonts whenever this scene is the <see cref="CurrentScene"/>. See <see cref="Scene"/> for more info.
		/// </summary>
		protected Dictionary<string, Font> Fonts { get; } = new();
		/// <summary>
		/// The loaded 3D sprites whenever this scene is the <see cref="CurrentScene"/>. See <see cref="Scene"/> for more info.
		/// </summary>
		protected Dictionary<string, Sprite3D> Sprites3D { get; } = new();

		/// <summary>
		/// Returns the required asset paths and details whenever this scene becomes the <see cref="CurrentScene"/>.
		/// See <see cref="Scene"/> for more info.
		/// </summary>
		protected virtual AssetQueue OnRequireAssets() => default;
		/// <summary>
		/// Called when this scene is done loading its assets after becoming the <see cref="CurrentScene"/>.
		/// See <see cref="Scene"/> for more info.
		/// </summary>
		protected virtual void OnStart() { }
		/// <summary>
		/// This is the game loop of this scene. It is called continuously while this scene is the <see cref="CurrentScene"/>.
		/// See <see cref="Scene"/> for more info.
		/// </summary>
		protected virtual void OnUpdate() { }
		/// <summary>
		/// Called when this scene is no longer the <see cref="CurrentScene"/>.
		/// See <see cref="Scene"/> for more info.
		/// </summary>
		protected virtual void OnStop() { }
		/// <summary>
		/// Called before the game is closed. See <see cref="Scene"/> for more info.
		/// </summary>
		protected virtual void OnGameStop() { }

		/// <summary>
		/// A quick way to draw a text onto a <see cref="Camera"/> with no <see cref="Textbox"/>. The text may be customized through
		/// <paramref name="textDetails"/> and <paramref name="originUnit"/>. It may also take the transformations of an <paramref name="obj"/>.
		/// The <see cref="TextDetails.DrawTarget"/> is assumed to be the <see cref="MainCamera"/> if no
		/// <see cref="TextDetails.DrawTarget"/> is passed.
		/// </summary>
		public static void DrawText(TextDetails textDetails, Object obj = null, Vector2 originUnit = default)
      {
			if (textDetails.Font == null)
				return;

			textDetails.DrawTarget ??= MainCamera;

			obj ??= new();

			var text = Textbox.text;
			text.Font = textDetails.Font;
			text.CharacterSize = textDetails.CharacterSize;
			text.FillColor = textDetails.Color;
			text.LetterSpacing = textDetails.CharacterSpace;
			text.LineSpacing = textDetails.LineSpace;
			text.OutlineColor = textDetails.OutlineColor;
			text.OutlineThickness = textDetails.OutlineSize;
			text.Style = textDetails.Style;
			text.DisplayedString = textDetails.Text;
			text.Position = obj.Position.ToSFML();
			text.Rotation = obj.Angle;
			text.Scale = new(obj.Scale, obj.Scale);

			var local = text.GetLocalBounds(); // has to be after everything
			text.Origin = new(local.Width * originUnit.X, local.Height * originUnit.Y);
			text.Position = text.Position.ToSystem().PointMoveAtAngle(text.Rotation - 90, local.Top, false).ToSFML();

			textDetails.DrawTarget.renderTexture.Draw(text);
		}

      internal void LoadAssets()
		{
			var assets = OnRequireAssets();
			var loadedCount = 0;

			if (assets.Textures == null && assets.Sounds == null && assets.Music == null && assets.Fonts == null && assets.TexturedModels3D == null)
			{
				CurrentScene.LoadingPercent = 100;
				return;
			}

			try
			{
				for (int i = 0; i < assets.Textures?.Count; i++)
				{
					Textures[assets.Textures[i]] = new Texture(assets.Textures[i]);
					UpdateLoadingPercent();
				}
				for (int i = 0; i < assets.Sounds?.Count; i++)
				{
					Sounds[assets.Sounds[i]] = new Sound(new SoundBuffer(assets.Sounds[i]));
					UpdateLoadingPercent();
				}
				for (int i = 0; i < assets.Music?.Count; i++)
				{
					Music[assets.Music[i]] = new Music(assets.Music[i]);
					UpdateLoadingPercent();
				}
				for (int i = 0; i < assets.Fonts?.Count; i++)
				{
					Fonts[assets.Fonts[i]] = new Font(assets.Fonts[i]);
					UpdateLoadingPercent();
				}
				for (int i = 0; i < assets.TexturedModels3D?.Count; i++)
				{
					Sprites3D[assets.TexturedModels3D[i].UniqueName] = new Sprite3D(assets.TexturedModels3D[i]);
					UpdateLoadingPercent();
				}
			}
			catch (System.Exception) { Console.LogError(-1, $"Could not load assets."); }

			void UpdateLoadingPercent()
			{
				loadedCount++;

				var total = GetCount(assets.Fonts) + GetCount(assets.Music) + GetCount(assets.Sounds) + GetCount(assets.Textures) +
					GetCount(assets.TexturedModels3D);
				CurrentScene.LoadingPercent = loadedCount.Map(0, total, 0, 100);

				int GetCount<T>(List<T> list) => list?.Count ?? 0;
			}
		}
		internal void UnloadAssets()
		{
			foreach (var kvp in Textures)
				kvp.Value.Dispose();

			Textures.Clear();
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
			if (startScene != null)
			{
				LoadingScene?.OnStop();
				startScene = null;
				CurrentScene?.OnStart();
			}
			if (stopScene != null)
			{
				stopScene = null;
				CurrentScene?.OnStop();
			}
			if (CurrentScene?.LoadingPercent < 100)
				LoadingScene?.OnUpdate();
			else
				CurrentScene?.OnUpdate();
		}
		internal static void ThreadLoadAssets()
		{
			while (true)
			{
				Thread.Sleep(1);
				if (loadScene != null)
				{
					scene.LoadAssets();
					startScene = loadScene;
					loadScene = null;
				}
				if (unloadScene != null)
				{
					scene.UnloadAssets();
					stopScene = unloadScene;
					unloadScene = null;
				}
			}
		}
	}
}
