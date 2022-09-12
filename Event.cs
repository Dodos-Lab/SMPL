namespace SMPL
{
	public static class Event
	{
		public delegate void EventHandler();
		public delegate void ThingEventHandler(string thingUID);
		public delegate void GUIButtonEventHandler(string guiUID, Thing.GUI.ButtonDetails buttonDetails);
		public delegate void SceneEventHandler(string sceneName);
		public delegate void MultiplayerClientEventHandler(string clientUniqueID);
		public delegate void MultiplayerMessageEventHandler(LAN.Message message);
		public delegate void ParticleEventHandler(string particleManagerUID, Thing.Particle particle);
		public delegate void ListItemEventHandler(string listUID, int itemIndex, Thing.GUI.ListItem item);
		public delegate void InputboxEventHandler(string inputboxUID, string input);

		public static event MultiplayerClientEventHandler MultiplayerClientConnected;
		public static event MultiplayerClientEventHandler MultiplayerClientDisconnected;
		public static event MultiplayerClientEventHandler MultiplayerClientTakenUID;
		public static event EventHandler MultiplayerServerStarted;
		public static event EventHandler MultiplayerServerStopped;
		public static event MultiplayerMessageEventHandler MultiplayerMessageReceived;

		public static event EventHandler GameStopped;

		public static event SceneEventHandler SceneStarted;
		public static event SceneEventHandler SceneUpdated;
		public static event SceneEventHandler SceneStopped;

		public static event ThingEventHandler ThingCreated;

		public static event GUIButtonEventHandler ButtonClicked;
		public static event GUIButtonEventHandler ButtonHeld;
		public static event GUIButtonEventHandler ButtonHovered;
		public static event GUIButtonEventHandler ButtonUnhovered;
		public static event GUIButtonEventHandler ButtonPressed;
		public static event GUIButtonEventHandler ButtonReleased;
		public static event GUIButtonEventHandler ButtonDragged;
		public static event GUIButtonEventHandler ButtonDropped;

		public static event ListItemEventHandler ListItemClicked;
		public static event ListItemEventHandler ListItemHeld;
		public static event ListItemEventHandler ListItemHovered;
		public static event ListItemEventHandler ListItemUnhovered;
		public static event ListItemEventHandler ListItemPressed;
		public static event ListItemEventHandler ListItemReleased;
		public static event ListItemEventHandler ListItemSelected;
		public static event ListItemEventHandler ListItemDeselected;

		public static event ThingEventHandler ScrollBarMoved;
		public static event ThingEventHandler ListDropdownToggled;

		public static event ThingEventHandler CheckboxChecked;
		public static event ThingEventHandler InputboxSubmitted;
		public static event InputboxEventHandler InputboxTyped;

		public static event ParticleEventHandler ParticleUpdated;

		#region Backend
		internal static void GameStop() => GameStopped?.Invoke();

		internal static void SceneStart(string name) => SceneStarted?.Invoke(name);
		internal static void SceneUpdate(string name) => SceneUpdated?.Invoke(name);
		internal static void SceneStop(string name) => SceneStopped?.Invoke(name);

		internal static void ThingCreate(string uid) => ThingCreated?.Invoke(uid);

		internal static void ButtonClick(string uid, Thing.GUI.ButtonDetails btn) => ButtonClicked?.Invoke(uid, btn);
		internal static void ButtonHold(string uid, Thing.GUI.ButtonDetails btn) => ButtonHeld?.Invoke(uid, btn);
		internal static void ButtonHover(string uid, Thing.GUI.ButtonDetails btn) => ButtonHovered?.Invoke(uid, btn);
		internal static void ButtonUnhover(string uid, Thing.GUI.ButtonDetails btn) => ButtonUnhovered?.Invoke(uid, btn);
		internal static void ButtonPress(string uid, Thing.GUI.ButtonDetails btn) => ButtonPressed?.Invoke(uid, btn);
		internal static void ButtonRelease(string uid, Thing.GUI.ButtonDetails btn) => ButtonReleased?.Invoke(uid, btn);
		internal static void ButtonDrag(string uid, Thing.GUI.ButtonDetails btn) => ButtonDragged?.Invoke(uid, btn);
		internal static void ButtonDrop(string uid, Thing.GUI.ButtonDetails btn) => ButtonDropped?.Invoke(uid, btn);

		internal static void ListItemClick(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemClicked?.Invoke(listUID, itemIndex, item);
		internal static void ListItemHold(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemHeld?.Invoke(listUID, itemIndex, item);
		internal static void ListItemHover(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemHovered?.Invoke(listUID, itemIndex, item);
		internal static void ListItemUnhover(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemUnhovered?.Invoke(listUID, itemIndex, item);
		internal static void ListItemPress(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemPressed?.Invoke(listUID, itemIndex, item);
		internal static void ListItemRelease(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemReleased?.Invoke(listUID, itemIndex, item);
		internal static void ListItemSelect(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemSelected?.Invoke(listUID, itemIndex, item);
		internal static void ListItemDeselect(string listUID, int itemIndex, Thing.GUI.ListItem item) => ListItemDeselected?.Invoke(listUID, itemIndex, item);

		internal static void ListDropdownToggle(string uid) => ListDropdownToggled?.Invoke(uid);
		internal static void ScrollBarMove(string uid) => ScrollBarMoved?.Invoke(uid);
		internal static void CheckboxCheck(string uid) => CheckboxChecked?.Invoke(uid);
		internal static void InputboxSubmit(string uid) => InputboxSubmitted?.Invoke(uid);
		internal static void InputboxType(string uid, string input) => InputboxTyped?.Invoke(uid, input);

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
