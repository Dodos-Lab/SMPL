namespace SMPL.Tools
{
	public class Storage<UniqueKeyT, ValueT>
	{
		public int Count => indexes == null ? 0 : indexes.Count;
		public int[] Indexes => indexes == null ? Array.Empty<int>() : indexes.ToArray();
		public UniqueKeyT[] UniqueKeys
		{
			get
			{
				return indexes == null ? Array.Empty<UniqueKeyT>() : dict.Keys.ToArray();
			}
		}
		public ValueT[] Values
		{
			get
			{
				return indexes == null ? Array.Empty<ValueT>() : dict.Values.ToArray();
			}
		}

		public ValueT this[int index]
		{
			get => TryIndexNotFound(index) ? default : values[index];
			set
			{
				if(TryIndexNotFound(index))
					return;
				values[index] = value;
				dict[keys[index]] = value;
			}
		}
		public ValueT this[UniqueKeyT uniqueKey]
		{
			get => TryUniqueKeyNotFound(uniqueKey) ? default : dict[uniqueKey];
			set
			{
				if(TryUniqueKeyNotFound(uniqueKey))
					return;
				dict[uniqueKey] = value;
				values[keys.IndexOf(uniqueKey)] = value;
			}
		}

		public void Expand(int index, UniqueKeyT uniqueKey, ValueT value)
		{
			if(keys == null) keys = new List<UniqueKeyT>();
			if(indexes == null) indexes = new List<int>();
			if(dict == null) dict = new Dictionary<UniqueKeyT, ValueT>();
			if(values == null) values = new List<ValueT>();

			if(index >= values.Count)
			{
				var oldListK = new List<UniqueKeyT>(keys);
				var oldListV = new List<ValueT>(values);
				values = new List<ValueT>();
				keys = new List<UniqueKeyT>();
				for(int i = 0; i < index; i++)
				{
					values.Add(default);
					keys.Add(default);
				}
				for(int i = 0; i < oldListV.Count; i++)
				{
					values[i] = oldListV[i];
					keys[i] = oldListK[i];
				}
			}

			dict.Add(uniqueKey, value);
			values.Insert(index, value);
			keys.Insert(index, uniqueKey);
			indexes.Add(index);

			var sameIndexMet = false;
			for(int i = 0; i < indexes.Count; i++)
			{
				if(sameIndexMet && indexes[i] == index)
				{
					indexes[i]++;
					continue;
				}
				if(indexes[i] > index) indexes[i]++;
				else if(sameIndexMet == false && indexes[i] == index) sameIndexMet = true;
			}
		}
		public void ShrinkAt(int index)
		{
			if(TryIndexNotFound(index)) return;
			indexes.Remove(index);
			values.RemoveAt(index);
			dict.Remove(keys[index]);
			keys.RemoveAt(index);
		}
		public void ShrinkAt(UniqueKeyT uniqueKey)
		{
			if(TryUniqueKeyNotFound(uniqueKey)) return;
			indexes.Remove(keys.IndexOf(uniqueKey));
			values.RemoveAt(keys.IndexOf(uniqueKey));
			dict.Remove(uniqueKey);
			keys.Remove(uniqueKey);
		}

		public void Clear()
		{
			if(isLocked)
				return;

			indexes?.Clear();
			keys?.Clear();
			values?.Clear();
			dict?.Clear();
		}
		public void Lock()
		{
			isLocked = true;
		}

		public UniqueKeyT UniqueKeyAt(int index)
		{
			return TryIndexNotFound(index) ? default : keys[index];
		}
		public int IndexAt(UniqueKeyT uniqueKey)
		{
			return TryUniqueKeyNotFound(uniqueKey) ? default : keys.IndexOf(uniqueKey);
		}

		public bool HasIndex(int index)
		{
			return indexes != null && indexes.Contains(index);
		}
		public bool HasUniqueKey(UniqueKeyT uniqueKey)
		{
			return keys != null && keys.Contains(uniqueKey);
		}
		public bool HasValue(ValueT value)
		{
			return values != null && values.Contains(value);
		}

		#region Backend
		private bool isLocked;
		private List<int> indexes;
		private List<UniqueKeyT> keys;
		private List<ValueT> values;
		private Dictionary<UniqueKeyT, ValueT> dict;

		internal bool TryUniqueKeyNotFound(UniqueKeyT uniqueKey)
		{
			if(HasUniqueKey(uniqueKey))
				return false;
			Console.LogError(1, $"The unique key '{uniqueKey}' was not found.");
			return true;
		}
		internal bool TryIndexNotFound(int index)
		{
			if(HasIndex(index))
				return false;
			Console.LogError(1, $"The index '{index}' was not found.");
			return true;
		}
		#endregion
	}
}
