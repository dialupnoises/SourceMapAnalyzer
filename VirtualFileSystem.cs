using SteamDatabase.ValvePak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapAnalyzer
{
	public class VirtualFileSystem
	{
		private readonly string[] RootDirs = new string[] { "maps", "materials", "models", "particles", "resource", "sound" };

		private VfsNode _root;

		public VirtualFileSystem(string[] vpkFiles, string[] dirs)
		{
			_root = new VfsNode("", null);

			foreach(var vpk in vpkFiles)
			{
				AddVpkToTree(vpk);
			}

			foreach(var dir in dirs)
			{
				AddDirToTree(dir);
			}
		}

		public bool Exists(string file) => GetNode(file) != null;

		public IVfsFile GetFile(string file) => GetNode(file)?.File;

		public IEnumerable<IVfsFile> GetFiles(string dir)
		{
			var dirNode = GetNode(dir);
			return dirNode.Children.Where(c => !c.Children.Any()).Select(c => c.File);
		}

		private VfsNode GetNode(string path)
		{
			var parts = path.ToLower().Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			var node = _root;
			foreach (var part in parts)
			{
				if (!node.TryGetChild(part, out var newNode))
				{
					return null;
				}

				node = newNode;
			}

			return node;
		}

		private void AddVpkToTree(string vpk)
		{
			var package = new Package();
			package.Read(vpk);

			foreach(var ext in package.Entries)
			{
				foreach(var entry in ext.Value)
				{
					var parts = entry.GetFullPath().Split('/');
					var node = _root;
					foreach(var part in parts)
					{
						VfsNode newNode;
						if(!node.TryGetChild(part, out newNode))
						{
							newNode = new VfsNode(part, new VpkFile(package, entry));
							node.AddChild(newNode);
						}

						node = newNode;
					}
				}
			}
		}

		private void AddDirToTree(string dir)
		{
			foreach(var d in RootDirs)
			{
				AddDirToTree(dir, Path.Combine(dir, d), new string[] { }, _root);
			}
		}

		private void AddDirToTree(string baseDir, string dir, string[] parts, VfsNode parentNode)
		{
			var dirName = Path.GetFileName(dir);
			VfsNode thisNode;
			if(!parentNode.TryGetChild(dirName, out thisNode))
			{
				thisNode = new VfsNode(dirName, new DirFile(baseDir, dir));
				parentNode.AddChild(thisNode);
			}

			foreach(var f in Directory.EnumerateFiles(dir))
			{
				var name = Path.GetFileName(f);
				thisNode.AddChild(new VfsNode(name, new DirFile(baseDir, f)));
			}

			foreach(var d in Directory.EnumerateDirectories(dir))
			{
				var name = Path.GetFileName(d);
				AddDirToTree(baseDir, d, parts.Concat(new string[] { name }).ToArray(), thisNode);
			}
		}

		public interface IVfsFile
		{
			Stream OpenRead();
			string ReadAllText();
			void Copy(string dest);
			string FileName { get; }
			string FilePath { get; }
		}

		private class VpkFile : IVfsFile
		{
			private PackageEntry _entry;
			private Package _package;

			public string FileName => _entry.FileName;
			public string FilePath => _entry.GetFullPath();

			public VpkFile(Package package, PackageEntry entry)
			{
				_package = package;
				_entry = entry;
			}

			public Stream OpenRead()
			{
				_package.ReadEntry(_entry, out var bytes);
				return new MemoryStream(bytes);
			}

			public string ReadAllText()
			{
				_package.ReadEntry(_entry, out var bytes);
				return Encoding.UTF8.GetString(bytes);
			}

			public void Copy(string dest)
			{
				_package.ReadEntry(_entry, out var bytes);
				File.WriteAllBytes(dest, bytes);
			}
		}

		private class DirFile : IVfsFile
		{
			private string _path;
			private string _baseDir;

			public string FileName => Path.GetFileName(_path);
			public string FilePath => Path.GetFullPath(_path).Replace(Path.GetFullPath(_baseDir) + "\\", "");

			public DirFile(string baseDir, string path)
			{
				_path = path;
				_baseDir = baseDir;
			}

			public Stream OpenRead() => File.OpenRead(_path);

			public string ReadAllText() => File.ReadAllText(_path);

			public void Copy(string dest) => File.Copy(_path, dest);
		}

		private class VfsNode
		{
			public string Name;

			public IVfsFile File => _file;

			public IEnumerable<VfsNode> Children => _children.Values;

			private IVfsFile _file;
			private Dictionary<string, VfsNode> _children;

			public VfsNode(string name, IVfsFile file)
			{
				Name = name.ToLower();
				_file = file;
				_children = new Dictionary<string, VfsNode>();
			}

			public void AddChild(VfsNode node)
			{
				_children[node.Name] = node;
			}

			public bool TryGetChild(string name, out VfsNode node)
			{
				return _children.TryGetValue(name.ToLower(), out node);
			}
		}
	}
}
