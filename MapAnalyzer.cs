﻿using ForgeGameDataReader;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceMapAnalyzer
{
	/// <summary>
	/// Actually performs the analysis.
	/// </summary>
	public class MapAnalyzer
	{
		private BSPFile _bsp;
		private FGDDiff _diff;
		private VPKSystem _packageSystem;
		private VirtualFileSystem _vfs;

		private string[] _usedResources;
		private string[] _uniqueEntities;
		private string[] _differentEntities;

		public string[] UsedResources => _usedResources;
		public string[] UniqueEntities => _uniqueEntities;
		public string[] DifferentEntities => _differentEntities;

		public BSPFile BspFile => _bsp;

		public MapAnalyzer(string bspFile, VirtualFileSystem vfs, IEnumerable<string> fgdFiles, IEnumerable<string> gameFgds, IEnumerable<string> vpks)
		{
			var sourceFgds = fgdFiles.Select(f => ForgeGameData.LoadFGD(f));
			var newFgds = gameFgds.Select(f => ForgeGameData.LoadFGD(f));
			var fgdLookup = new FGDLookup(sourceFgds.Concat(newFgds).ToArray());

			_vfs = vfs;
			_bsp = new BSPFile(vfs, fgdLookup, bspFile);
			_diff = FGDDiffer.Diff(sourceFgds, newFgds);
			_packageSystem = new VPKSystem(vpks);

			FindUniqueResources();
			FindUniqueEntities();
		}

		/// <summary>
		/// Copies data, if makePackage is true, and outputs report.
		/// </summary>
		public void Output()
		{
			using (var writer = new StreamWriter(File.Open(Path.GetFileNameWithoutExtension(_bsp.FilePath) + ".txt", FileMode.Create)))
			{
				writer.WriteLine("-- RESOURCES USED --");
				foreach (var resource in UsedResources.OrderBy(a => a))
				{
					writer.WriteLine("\t" + resource.Replace('\\', '/'));
				}

				writer.WriteLine("-- UNIQUE ENTITIES --");
				foreach (var ent in UniqueEntities.OrderBy(a => a))
				{
					writer.WriteLine("\t" + ent);
				}

				writer.WriteLine("-- DIFFERENT ENTITIES --");
				foreach (var ent in DifferentEntities.OrderBy(a => a))
				{
					writer.WriteLine("\t" + ent);
				}
			}
		}

		// finds entities in the game FGD that aren't in the base FGDs
		private void FindUniqueEntities()
		{
			var classNames = _bsp.Entities.Select(e => e["classname"]).Distinct();
			var different = new List<string>();
			var unique = new List<string>();

			foreach(var className in classNames)
			{
				var diffType = _diff.FindEntType(className);
				if(diffType == FGDDiff.DiffType.Different)
				{
					different.Add(className);
				}
				else if(diffType == FGDDiff.DiffType.Unique)
				{
					unique.Add(className);
				}
			}

			_differentEntities = different.ToArray();
			_uniqueEntities = unique.ToArray();
		}

		// finds unique resources used in the map
		private void FindUniqueResources()
		{
			var usedMaterials =
				_bsp.Materials
					.Concat(_bsp.Sprites)
					.Select(m => "materials\\" + m)
					.Where(m => IsResourceUnique(m));

			var usedModels =
				_bsp.Models
					.Select(m => m.ToLower())
					.Where(m => IsResourceUnique(m));

			var modelMatsUsed = 
				usedModels
				.Where(m => _vfs.Exists(m + ".mdl"))
				.SelectMany(m => new MDLFile(_vfs, _vfs.GetFile(m + ".mdl")).Textures)
				.Distinct()
				.Where(m => IsResourceUnique(m + ".vmt"))
				.ToArray();

			var usedSounds =
				_bsp.Sounds
					.Select(s => "sound\\" + s)
					.Where(s => IsResourceUnique(s));

			var foundMats = VMTReader.ReadVmts(_packageSystem, _vfs, usedMaterials.Concat(modelMatsUsed));

			_usedResources = foundMats.Concat(usedModels).Concat(usedSounds).ToArray();
		}
		
		private bool IsResourceUnique(string p)
		{
			return !_bsp.PakfileResources.Contains(p) && !_packageSystem.ContainsFile(p);
		}
	}
}
