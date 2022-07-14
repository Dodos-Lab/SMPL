namespace SMPL
{
	public static class Event
	{
		public delegate void EventHandler();
		public delegate void ThingEventHandler(string thingUID);
		public delegate void SceneEventHandler(string sceneName);

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

		#endregion
	}
}
