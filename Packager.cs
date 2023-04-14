using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapAnalyzer
{
	public static class Packager
	{
		private static readonly string[] ResourceFileExtensions = new string[] { ".vmt", ".mdl", ".jpg", ".png", ".svg", ".ogg", ".wav", ".mp3", "pcf", "otf", "ttf" };

		public static void Package(Dictionary<string, MapAnalyzer> analyzers, PackageMode mode, string gameDir)
		{
			if(mode == PackageMode.None)
			{
				return;
			}

			if(mode == PackageMode.CombinedFolder || mode == PackageMode.CombinedAddon)
			{
				if(!Directory.Exists("output"))
				{
					Directory.CreateDirectory("output");
				}
			}

			var foundFiles = new Dictionary<string, HashSet<string>>();

			foreach(var map in analyzers)
			{
				var analyzer = map.Value;
				var targetPath = "";
				if(mode == PackageMode.FolderPerMap)
				{
					targetPath = Path.GetFileNameWithoutExtension(analyzer.BspFile.FilePath);
					if (!Directory.Exists(targetPath))
					{
						Directory.CreateDirectory(targetPath);
					}
				}
				else if(mode == PackageMode.CombinedFolder || mode == PackageMode.CombinedAddon)
				{
					targetPath = "output";
				}

				Copyer.CopyFiles(gameDir, Path.GetFullPath(targetPath), analyzer.UsedResources, ref foundFiles);
			}

			if(mode == PackageMode.CombinedAddon)
			{
				var addonJson = JsonConvert.SerializeObject(new
				{
					title = "SourceMapAnalyzer Output",
					type = "ServerContent",
					tags = new string[] { }
				});

				File.WriteAllText("output/addon.json", addonJson);

				// write lua file adding all relevant files to download if the map is selected
				Copyer.CreateDirectoryStructure("output", "lua/autorun/server");
				var output = new StringBuilder();
				var randomHookName = "Initialize_" + new Random().Next();
				output.AppendLine($"hook.Add('Initialize', '{randomHookName}', function()");
				foreach(var map in analyzers)
				{
					output.AppendLine($"\tif game.GetMap() == '{Path.GetFileNameWithoutExtension(map.Key)}' then");
					foreach(var f in map.Value.UsedResources)
					{
						if(foundFiles.TryGetValue(f, out var exts))
						{
							foreach(var ext in ResourceFileExtensions)
							{
								if(exts.Contains(ext))
								{
									output.AppendLine($"\t\tresource.AddFile('{f.Replace('\\', '/')}{ext}')");
								}
							}
						}
					}
					output.AppendLine("\tend");
				}
				output.AppendLine("end)");
				File.WriteAllText("output/lua/autorun/server/resource.lua", output.ToString());
			}
		}
	}

	public enum PackageMode
	{
		None,
		FolderPerMap,
		CombinedFolder,
		CombinedAddon
	}
}
