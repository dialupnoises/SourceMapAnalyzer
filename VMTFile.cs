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
		public static string[] ReadVmts(VPKSystem vpk, VirtualFileSystem vfs, IEnumerable<string> files)
		{
			var readVmts = new HashSet<string>(new StringIEqualityComparer());
			var vmtsToRead = new HashSet<string>(files, new StringIEqualityComparer());

			while(vmtsToRead.Any())
			{
				var newVmt = vmtsToRead.First();
				var newVmtFile = newVmt + ".vmt";
				// not in a vpk, so we need to check it
				if(!vpk.ContainsFile(newVmtFile))
				{
					var vfsVmt = vfs.GetFile(newVmtFile);
					if (vfsVmt != null)
					{
						var keys = ParseVmt(vfsVmt.ReadAllText());
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
				vmtsToRead.Add(Path.Combine("materials", tex));
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

		private class StringIEqualityComparer : IEqualityComparer<string>
		{
			public bool Equals(string x, string y)
			{
				return x.Equals(y, StringComparison.OrdinalIgnoreCase);
			}

			public int GetHashCode(string obj)
			{
				return obj.ToLower().GetHashCode();
			}
		}
	}
}
