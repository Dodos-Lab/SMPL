namespace SMPL
{
	public static class ThingManager
	{
		public static string Create(this string thingUID)
		{
			var thing = new Thing(thingUID);
			return thing.UID;
		}
		public static void Create(this List<string> thingUIDs)
		{
			for(int i = 0; i < thingUIDs.Count; i++)
				thingUIDs[i] = thingUIDs[i].Create();
		}

		public static void SetParentUID(this string thingUID, string uid)
		{
			var thing = Thing.Get(thingUID);

			if(thing != null)
				thing.ParentUID = uid;
		}
		public static void SetParentUIDs(this List<string> thingUIDs, string uid)
		{
			for(int i = 0; i < thingUIDs.Count; i++)
				thingUIDs[i].SetParentUID(uid);
		}
		public static string GetParentUID(this string thingUID)
		{
			var thing = Thing.Get(thingUID);
			return thing == null ? default : thing.ParentUID;
		}
		public static List<string> GetParentUIDs(this List<string> thingUIDs)
		{
			var result = new List<string>(thingUIDs.Count);
			for(int i = 0; i < thingUIDs.Count; i++)
				result.Add(thingUIDs[i].GetParentUID());
			return result;
		}

		public static string GetTypeName(this string thingUID)
		{
			var thing = Thing.Get(thingUID);
			return thing == null ? default : thing.GetType().Name;
		}
		public static List<string> GetTypeNames(this List<string> thingUIDs)
		{
			var result = new List<string>(thingUIDs.Count);
			for(int i = 0; i < thingUIDs.Count; i++)
				result.Add(thingUIDs[i].GetTypeName());
			return result;
		}
	}
}
