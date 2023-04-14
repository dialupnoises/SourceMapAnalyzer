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
		public static void CopyFiles(VirtualFileSystem vfs, string newDir, IEnumerable<string> files, ref Dictionary<string, HashSet<string>> foundFiles)
		{
			foreach(var file in files)
			{
				var fileNoExt = WithoutAllExtensions(file).ToLower();
				var fileDir = Path.GetDirectoryName(file);
				if (!vfs.Exists(fileDir))
				{
					continue;
				}

				if (!foundFiles.ContainsKey(file))
				{
					foundFiles[file] = new HashSet<string>();
				}

				var filesWithName = vfs.GetFiles(fileDir).Where(f => WithoutAllExtensions(f.FileName).ToLower() == fileNoExt);
				CreateDirectoryStructure(newDir, fileDir);
				foreach(var f in filesWithName)
				{
					var destFile = Path.Combine(newDir, f.FilePath);
					if (!File.Exists(destFile))
					{
						f.Copy(destFile);
					}

					foundFiles[file].Add(Path.GetExtension(f.FileName));
				}
			}
		}
		public static void CreateDirectoryStructure(string baseDir, string dir)
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
		private static string WithoutAllExtensions(string fileName) => Path.GetFileName(fileName).Split('.')[0];
	}
}
