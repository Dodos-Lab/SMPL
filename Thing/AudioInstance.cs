namespace SMPL
{
	public static partial class Thing
	{
		public enum AudioStatus { Playing, Paused, Stopped }

		public static string ListenerUID { get; set; } = Scene.MAIN_CAMERA_UID;
	}
	internal class AudioInstance : ThingInstance
	{
		[JsonIgnore]
		public float Duration => duration;
		public Thing.AudioStatus Status
		{
			get => status;
			set
			{
				status = value;

				var sound = GetSound();
				var music = GetMusic();

				if(sound != null)
					switch(status)
					{
						case Thing.AudioStatus.Playing: if(sound.Status != SoundStatus.Playing) sound.Play(); break;
						case Thing.AudioStatus.Paused: sound.Pause(); break;
						case Thing.AudioStatus.Stopped:
							{
								Progress = 0;
								sound.Stop();
								break;
							}
					}
				else if(music != null)
					switch(status)
					{
						case Thing.AudioStatus.Playing: if(music.Status != SoundStatus.Playing) music.Play(); break;
						case Thing.AudioStatus.Paused: music.Pause(); break;
						case Thing.AudioStatus.Stopped:
							{
								Progress = 0;
								music.Stop();
								break;
							}
					}
			}
		}

		public string Path
		{
			get => audioPath;
			set
			{
				if(created == false)
					Status = Thing.AudioStatus.Stopped;
				audioPath = value;
			}
		}
		public bool IsLooping { get; set; }
		public bool IsGlobal { get; set; } = true;
		public float VolumeUnit { get; set; } = 1f;
		public float PitchUnit { get; set; } = 1f;
		[JsonIgnore]
		public float Progress
		{
			get
			{
				var sound = GetSound();
				var music = GetMusic();

				if(sound != null)
					return sound.PlayingOffset.AsSeconds();
				else if(music != null)
					return music.PlayingOffset.AsSeconds();
				return default;
			}
			set
			{
				var sound = GetSound();
				var music = GetMusic();

				if(sound != null)
					sound.PlayingOffset = SFML.System.Time.FromSeconds(value);
				else if(music != null)
					music.PlayingOffset = SFML.System.Time.FromSeconds(value);
			}
		}
		public float ProgressUnit
		{
			get => Duration == 0 ? default : Progress / Duration;
			set => Progress = value * Duration;
		}
		public float DistanceFade { get; set; } = 2f;

		#region Backend
		private Sound sound;
		private Music music;
		private bool created;
		private static readonly List<AudioInstance> audios = new();
		[JsonProperty]
		private Thing.AudioStatus status;
		private float duration, prevProg;
		private string audioPath;

		[JsonConstructor]
		internal AudioInstance()
		{
			Init();
		}
		internal AudioInstance(string uid) : base(uid)
		{
			Init();
		}

		private void Init()
		{
			audios.Add(this);
			created = true;
		}
		private Sound GetSound()
		{
			if(sound != null)
				return sound;

			var sfx = Scene.CurrentScene.Sounds;
			if(audioPath != null && sfx.ContainsKey(audioPath))
				sound = new Sound(sfx[audioPath]);

			return sound;
		}
		private Music GetMusic()
		{
			if(music != null)
				return music;

			var m = Scene.CurrentScene.Music;
			if(audioPath != null && m.ContainsKey(audioPath))
				music = new Music(audioPath); // that's fine, it's just a stream, not holding any data (shouldn't be heavy)

			return music;
		}

		private void SyncAudioProps()
		{
			var sound = GetSound();
			var music = GetMusic();
			var vol = VolumeUnit * Game.Settings.VolumeMaster * 100f;
			var pos = new Vector3f(Position.X, Position.Y, 0f);

			if(sound != null)
			{
				duration = sound.SoundBuffer.Duration.AsSeconds();
				sound.Loop = IsLooping;
				sound.Pitch = PitchUnit;
				sound.Volume = vol * Game.settings.VolumeSound;
				sound.Attenuation = DistanceFade;
				sound.MinDistance = 100f;
				sound.Position = IsGlobal ? default : pos;
			}
			else if(music != null)
			{
				duration = music.Duration.AsSeconds();
				music.Loop = IsLooping;
				music.Pitch = PitchUnit;
				music.Volume = vol * Game.settings.VolumeMusic;
				music.Attenuation = DistanceFade;
				music.MinDistance = 100f;
				music.Position = IsGlobal ? default : pos;
			}
		}
		internal static void Update()
		{
			var thing = Get(Thing.ListenerUID);
			Listener.Position = thing == null ? default : new Vector3f(thing.Position.X, thing.Position.Y, 0f);

			for(int i = 0; i < audios.Count; i++)
			{
				var a = audios[i];
				a.SyncAudioProps();
				a.Status = a.status;

				if(a.ProgressUnit == 0 && a.prevProg != 0 && a.status == Thing.AudioStatus.Playing)
					a.Status = Thing.AudioStatus.Stopped;
				a.prevProg = a.ProgressUnit;
			}
		}
		internal override void OnDestroy()
		{
			base.OnDestroy();
			var sound = GetSound();
			var music = GetMusic();

			sound?.Stop();
			music?.Stop();

			sound?.Dispose();
			music?.Dispose();

			audios.Remove(this);
		}
		#endregion
	}
}
