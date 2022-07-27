namespace SMPL
{
	public static class Event
	{
		public delegate void EventHandler();
		public delegate void ThingEventHandler(string thingUID);
		public delegate void SceneEventHandler(string sceneName);
		public delegate void MultiplayerClientEventHandler(string clientUniqueID);
		public delegate void MultiplayerServerEventHandler();
		public delegate void MultiplayerMessageEventHandler(LAN.Message message);
		public delegate void ParticleEventHandler(string thingUID, Thing.Particle particle);

		public static event MultiplayerClientEventHandler MultiplayerClientConnected;
		public static event MultiplayerClientEventHandler MultiplayerClientDisconnected;
		public static event MultiplayerClientEventHandler MultiplayerClientTakenUID;
		public static event MultiplayerServerEventHandler MultiplayerServerStarted;
		public static event MultiplayerServerEventHandler MultiplayerServerStopped;
		public static event MultiplayerMessageEventHandler MultiplayerMessageReceived;

		public static event EventHandler GameStopped;

		public static event SceneEventHandler SceneStarted;
		public static event SceneEventHandler SceneUpdated;
		public static event SceneEventHandler SceneStopped;

		public static event ThingEventHandler ButtonClicked;
		public static event ThingEventHandler ButtonHeld;
		public static event ThingEventHandler ButtonHovered;
		public static event ThingEventHandler ButtonUnhovered;
		public static event ThingEventHandler ButtonPressed;
		public static event ThingEventHandler ButtonReleased;

		public static event ThingEventHandler CheckboxChecked;

		public static event ParticleEventHandler ParticleUpdated;

		#region Backend
		internal static void GameStop() => GameStopped?.Invoke();

		internal static void SceneStart(string name) => SceneStarted?.Invoke(name);
		internal static void SceneUpdate(string name) => SceneUpdated?.Invoke(name);
		internal static void SceneStop(string name) => SceneStopped?.Invoke(name);

		internal static void ButtonClick(string uid) => ButtonClicked?.Invoke(uid);
		internal static void ButtonHold(string uid) => ButtonHeld?.Invoke(uid);
		internal static void ButtonHover(string uid) => ButtonHovered?.Invoke(uid);
		internal static void ButtonUnhover(string uid) => ButtonUnhovered?.Invoke(uid);
		internal static void ButtonPress(string uid) => ButtonPressed?.Invoke(uid);
		internal static void ButtonRelease(string uid) => ButtonReleased?.Invoke(uid);

		internal static void CheckboxCheck(string uid) => CheckboxChecked?.Invoke(uid);

		internal static void ParticleUpdate(string uid, Thing.Particle particle) => ParticleUpdated?.Invoke(uid, particle);

		internal static void MultiplayerClientConnect(string clientUID)
		{
			MultiplayerClientConnected?.Invoke(clientUID);
		}
		internal static void MultiplayerClientDisconnect(string clientUID)
		{
			MultiplayerClientDisconnected?.Invoke(clientUID);
		}
		internal static void MultiplayerClientTakeUID(string clientUID)
		{
			MultiplayerClientTakenUID?.Invoke(clientUID);
		}
		internal static void MultiplayerServerStart()
		{
			MultiplayerServerStarted?.Invoke();
		}
		internal static void MultiplayerServerStop()
		{
			MultiplayerServerStopped?.Invoke();
		}
		internal static void MultiplayerMessageReceive(LAN.Message message)
		{
			MultiplayerMessageReceived?.Invoke(message);
		}
		#endregion
	}
}
