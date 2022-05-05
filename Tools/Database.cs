using Newtonsoft.Json;
using SMPL.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Console = SMPL.Tools.Console;

namespace Test
{
	[JsonObject(MemberSerialization.Fields)]
	public class Database
	{
		public class Sheet { }

		[JsonObject(MemberSerialization.Fields)]
		internal class Sheet<T> : Sheet
		{
#pragma warning disable CS0649
			internal string name;
			internal T[] lines;
			internal dynamic[] columns;
			internal dynamic[] separators;
			internal dynamic props;

			[JsonIgnore]
			internal List<T> linesList;
#pragma warning disable CS0649
		}
#pragma warning disable CS0649
		internal dynamic[] sheets;
		internal dynamic[] customTypes;
		internal bool compress;
#pragma warning disable CS0649
		[JsonIgnore]
		private Dictionary<string, Sheet> cacheSheets;
		[JsonIgnore]
		private string path;

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

		public List<T> GetSheet<T>(string sheetName)
		{
			try
			{
				if (cacheSheets == null)
					cacheSheets = new();

				if (cacheSheets.ContainsKey(sheetName))
					return ((Sheet<T>)cacheSheets[sheetName]).linesList;

				for (int i = 0; i < sheets.Length; i++)
					if (sheets[i].name == sheetName)
					{
						var json = sheets[i].ToString();
						var sheet = (Sheet<T>)JsonConvert.DeserializeObject<Sheet<T>>(json);
						var linesList = sheet.lines.ToList();
						cacheSheets[sheetName] = sheet;
						((Sheet<T>)cacheSheets[sheetName]).linesList = linesList;
						return linesList;
					}
			}
			catch (System.Exception) { }
			Console.LogError(1, $"Could not load {nameof(Sheet<T>)}<{typeof(T)}> with {nameof(sheetName)} '{sheetName}' in this {nameof(Database)}.");
			return default;
		}
		public void AddSheet<T>(string sheetName) => cacheSheets[sheetName] = new Sheet<T>();
		public void SaveSheet<T>(string sheetName)
		{
			if (cacheSheets.ContainsKey(sheetName) == false)
			{
				Console.LogError(1, $"No {nameof(Sheet)} with {nameof(sheetName)} '{sheetName}' was found in this {nameof(Database)}. " +
					$"This may be because a {nameof(Sheet)}\nwith such name does not exist or it is not yet loaded into this {nameof(Database)} " +
					$"(by {nameof(GetSheet)}({nameof(sheetName)})).");
				return;
			}

			var isNew = true;
			for (int i = 0; i < sheets.Length; i++)
				if (sheets[i].name == sheetName)
				{
					sheets[i] = cacheSheets[sheetName];
					isNew = false;
					break;
				}

			if (isNew)
			{
				var list = sheets.ToList();
				list.Add(cacheSheets[sheetName]);
				sheets = list.ToArray();
			}
			var sheet = ((Sheet<T>)cacheSheets[sheetName]);
			sheet.lines = sheet.linesList.ToArray();
			var json = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(path, json);
		}
	}
}
