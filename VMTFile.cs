using ForgeGameDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SourceMapAnalyzer
{
	public static class VMTReader
	{
		public static string[] ReadVmts(VPKSystem vpk, string baseDir, IEnumerable<string> files)
		{
			var readVmts = new HashSet<string>();
			var vmtsToRead = new HashSet<string>(files);

			while(vmtsToRead.Any())
			{
				var newVmt = vmtsToRead.First();
				var newVmtFile = newVmt + ".vmt";
				// not in a vpk, so we need to check it
				if(!vpk.ContainsFile(newVmtFile))
				{
					var path = Path.Combine(baseDir, newVmtFile);
					if(File.Exists(path))
					{
						var keys = ParseVmt(File.ReadAllText(path));
						AddKeyIfExists("$basetexture", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$bumpmap", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$detail", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$texture2", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$normalmap", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$reflecttexture", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$refracttexture", ref keys, ref vmtsToRead, ref readVmts);
						AddKeyIfExists("$decaltexture", ref keys, ref vmtsToRead, ref readVmts);
					}
				}

				vmtsToRead.Remove(newVmt);
				readVmts.Add(newVmt);
			}

			return readVmts.ToArray();
		}

		private static void AddKeyIfExists(
			string key, 
			ref Dictionary<string, string> dict, 
			ref HashSet<string> vmtsToRead, 
			ref HashSet<string> readVmts)
		{
			if (
				dict.TryGetValue(key, out var tex) &&
				!vmtsToRead.Contains(tex) &&
				!readVmts.Contains(tex))
			{
				if(tex == "_rt_WaterReflection" || tex == "_rt_WaterRefraction")
				{
					return;
				}
				vmtsToRead.Add(tex);
			}
		}

		private static Dictionary<string, string> ParseVmt(string text)
		{
			var keys = new Dictionary<string, string>();

			int index = 0;
			var depth = 0;
			do
			{
				if (TokenReader.TryNextChar(text, ref index, '}', true))
				{
					depth--;
					continue;
				}

				var key = TokenReader.GetNextToken(text, ref index, out var newLine).ToLower();
				if (TokenReader.TryNextChar(text, ref index, '{', true))
				{
					depth++;
				}
				else
				{
					var value = TokenReader.GetNextToken(text, ref index, out newLine).ToLower();
					value = value.Replace('/', '\\');
					// don't replace values with sub-texture values
					if(!(keys.ContainsKey(key) && depth > 1))
					{
						keys[key] = value;
					}
				}
			} while (depth > 0);

			return keys;
		}
	}
}
