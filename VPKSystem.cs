using SteamDatabase.ValvePak;
using System.Collections.Generic;
using System.Linq;

namespace SourceMapAnalyzer
{
	/// <summary>
	/// Allows files to be looked up from several VPK files as if it was one.
	/// </summary>
	public class VPKSystem
	{
		private Package[] _packages;

		public VPKSystem(IEnumerable<string> files)
		{
			_packages = files.Select(f => { var p = new Package(); p.Read(f); return p; }).ToArray();
		}

		public bool ContainsFile(string file)
		{
			foreach(var package in _packages)
			{
				foreach(var ext in package.Entries)
				{
					if(package.FindEntry(file + "." + ext.Key) != null)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
