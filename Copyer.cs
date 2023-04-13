using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapAnalyzer
{
	public class Copyer
	{
		/// <summary>
		/// Copies files from baseDir to newDir
		/// </summary>
		public static void CopyFiles(string baseDir, string newDir, IEnumerable<string> files)
		{
			var readDirs = new Dictionary<string, string[]>();
			foreach(var file in files)
			{
				var fileNoExt = WithoutAllExtensions(file).ToLower();
				var fileDir = Path.GetDirectoryName(file);
				var searchDir = Path.Combine(baseDir, fileDir);
				if (!Directory.Exists(searchDir))
				{
					continue;
				}

				if (!readDirs.ContainsKey(searchDir))
				{
					readDirs[searchDir] = Directory.GetFiles(searchDir);
				}

				var filesWithName = readDirs[searchDir].Where(f => WithoutAllExtensions(f).ToLower() == fileNoExt);
				CreateDirectoryStructure(newDir, fileDir);
				foreach(var f in filesWithName)
				{
					var destFile = Path.Combine(newDir, f.Replace(baseDir + "\\", ""));
					if (!File.Exists(destFile))
					{
						File.Copy(f, destFile);
					}
				}
			}
		}

		private static string WithoutAllExtensions(string fileName) => Path.GetFileName(fileName).Split('.')[0];

		private static void CreateDirectoryStructure(string baseDir, string dir)
		{
			var parts = dir.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var partsCreated = new List<string>() { baseDir };

			foreach(var part in parts)
			{
				partsCreated.Add(part);
				if(!Directory.Exists(Path.Combine(partsCreated.ToArray())))
				{
					Directory.CreateDirectory(Path.Combine(partsCreated.ToArray()));
				}
			}
		}
	}
}
