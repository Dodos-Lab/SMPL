using Newtonsoft.Json;
using SMPL.Tools;
using System.IO;
using System.Numerics;

namespace Test
{
	[JsonObject(MemberSerialization.Fields)]
	public class Database
	{
		[JsonObject(MemberSerialization.Fields)]
		internal class Sheet<T>
		{
#pragma warning disable CS0649
			internal string name;
			internal T[] lines;
#pragma warning disable CS0649

			internal Sheet(T[] lines) => this.lines = lines;
		}
#pragma warning disable CS0649
		internal dynamic[] sheets;
#pragma warning disable CS0649

		public static Database Load(string cdbFilePath)
		{
			try
			{
				var menu = File.ReadAllText(cdbFilePath);
				return JsonConvert.DeserializeObject<Database>(menu.ToString());
			}
			catch { Console.LogError(1, $"Could not load {nameof(Database)} file with path '{cdbFilePath}'."); }
			return default;
		}
		private Database() { }

		public T GetLine<T>(string sheetName, int lineIndex)
		{
			try
			{
				for (int i = 0; i < sheets.Length; i++)
					if (sheets[i].name == sheetName)
					{
						var sheet = JsonConvert.DeserializeObject<Sheet<T>>(sheets[i].ToString());
						return sheet.lines[lineIndex];
					}
			}
			catch (System.Exception) { }
			Console.LogError(1, $"Could not find line with {nameof(lineIndex)} '{lineIndex}' in " +
				$"{nameof(Sheet<T>)}<{typeof(T)}> with {nameof(sheetName)} '{sheetName}' in this {nameof(Database)}.");
			return default;
		}
	}
}
