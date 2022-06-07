namespace SMPL.Tools
{
	[JsonObject(MemberSerialization.Fields)]
	public class Database
	{
		public static Database Load(string cdbFilePath)
		{
			var db = default(Database);
			try
			{
				db = JsonConvert.DeserializeObject<Database>(File.ReadAllText(cdbFilePath));
				db.path = cdbFilePath;
			}
			catch { Console.LogError(1, $"Could not load {nameof(Database)} file with path '{cdbFilePath}'."); }
			return db;
		}
		public Database(string cdbFilePath) => path = cdbFilePath;

		public List<T> GetSheet<T>(string sheetName)
		{
			var msg = "";
			try
			{
				if(cacheSheets == null)
					cacheSheets = new();

				if(cacheSheets.ContainsKey(sheetName))
					return ((Sheet<T>)cacheSheets[sheetName]).linesList;

				for(int i = 0; i < sheets.Length; i++)
					if(sheets[i].name == sheetName)
					{
						var json = sheets[i].ToString();
						var sheet = (Sheet<T>)JsonConvert.DeserializeObject<Sheet<T>>(json);
						var linesList = sheet.lines.ToList();
						cacheSheets[sheetName] = sheet;
						((Sheet<T>)cacheSheets[sheetName]).linesList = linesList;
						return linesList;
					}
			}
			catch(Exception ex) { msg = ex.Message; }
			Console.LogError(1, $"Could not load {nameof(Sheet<T>)}<{typeof(T)}> with {nameof(sheetName)} '{sheetName}' in this {nameof(Database)}. Info:\n{msg}");
			return default;
		}
		public List<T> AddSheet<T>(string sheetName)
		{
			cacheSheets[sheetName] = new Sheet<T>();
			return GetSheet<T>(sheetName);
		}
		public void SaveSheet<T>(string sheetName)
		{
			if(cacheSheets.ContainsKey(sheetName) == false)
			{
				Console.LogError(1, $"No {nameof(Sheet)} with {nameof(sheetName)} '{sheetName}' was found in this {nameof(Database)}. " +
					$"This may be because a {nameof(Sheet)}\nwith such name does not exist or it is not yet loaded into this {nameof(Database)} " +
					$"(by {nameof(GetSheet)}({nameof(sheetName)})).");
				return;
			}

			var isNew = sheets == null;
			if(isNew == false)
				for(int i = 0; i < sheets.Length; i++)
					if(sheets[i].name == sheetName)
					{
						sheets[i] = cacheSheets[sheetName];
						break;
					}

			if(isNew)
			{
				var list = new List<dynamic> { cacheSheets[sheetName] };
				sheets = list.ToArray();
			}

			var sheet = ((Sheet<T>)cacheSheets[sheetName]);
			sheet.name = sheetName;
			sheet.lines = sheet.linesList.ToArray();
			sheet.columns = GetColumns();
			var json = JsonConvert.SerializeObject(this, Formatting.Indented);
			if(isNew)
			{
				json = json.Replace("\"separators\": null,", "\"separators\": [],");
				json = json.Replace("\"props\": null", "\"props\": {}");
				json = json.Replace("\"customTypes\": null,", "\"customTypes\": [],");
			}

			var dir = Path.GetDirectoryName(path);
			if(string.IsNullOrWhiteSpace(dir) == false)
				Directory.CreateDirectory(dir);
			File.WriteAllText(path, json);

			dynamic[] GetColumns()
			{
				var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				var result = new List<Column>();
				for(int i = 0; i < fields.Length; i++)
					if(ShouldSkipFieldOrProperty(fields[i], fields[i].IsPrivate) == false)
						ProcessType(fields[i].FieldType, fields[i].Name);

				for(int i = 0; i < props.Length; i++)
					if(ShouldSkipFieldOrProperty(props[i], false) == false)
						ProcessType(props[i].PropertyType, props[i].Name);

				return result.ToArray();

				void ProcessType(Type type, string name)
				{
					if(type == typeof(string)) SetColumn("1");
					else if(type == typeof(bool)) SetColumn("2");
					else if(type == typeof(int)) SetColumn("3");
					else if(type == typeof(float)) SetColumn("4");
					else if(type.IsEnum) SetColumn(type.IsDefined(typeof(FlagsAttribute), false) ? "10" : "5");
					else SetColumn("16");

					void SetColumn(string typeStr)
					{
						var enumNames = "";
						if(typeStr == "5" || typeStr == "10")
						{
							var e = Enum.GetNames(type);
							typeStr = $"{typeStr}:";
							for(int j = 0; j < e.Length; j++)
							{
								var comma = j == 0 ? "" : ",";
								enumNames = $"{enumNames}{comma}{e[j]}";
							}
						}
						result.Add(new() { typeStr = $"{typeStr}{enumNames}", name = name });
					}
				}
				bool ShouldSkipFieldOrProperty(MemberInfo memberInfo, bool isPrivate)
				{
					var ignore = memberInfo.GetCustomAttribute<JsonIgnoreAttribute>() != null ||
						memberInfo.GetCustomAttribute<CompilerGeneratedAttribute>() != null ||
						memberInfo.GetCustomAttribute<NonSerializedAttribute>() != null ||
						isPrivate;
					var force = memberInfo.GetCustomAttribute<JsonPropertyAttribute>() != null ||
						memberInfo.GetCustomAttribute<DataMemberAttribute>() != null;
					return ignore && force == false;
				}
			}
		}

		#region Backend
		private class Sheet { }
		[JsonObject(MemberSerialization.Fields)]
		private class Sheet<T> : Sheet
		{
#pragma warning disable CS0649
			internal string name;
			internal T[] lines;
			internal dynamic[] columns;
			internal dynamic[] separators;
			internal dynamic props;

			[JsonIgnore]
			internal List<T> linesList = new();
#pragma warning disable CS0649
		}
		private class Column
		{
			public string typeStr;
			public string name;
		}

#pragma warning disable CS0649
		internal dynamic[] sheets;
		internal dynamic[] customTypes;
		internal bool compress;
#pragma warning disable CS0649
		[JsonIgnore]
		private Dictionary<string, Sheet> cacheSheets = new();
		[JsonIgnore]
		private string path;
		#endregion
	}
}
