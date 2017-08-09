using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceMapAnalyzer
{
	/// <summary>
	/// Reads the textures used in an MDL file.
	/// </summary>
	public class MDLFile
	{
		private string[] _textures;

		public string[] Textures => _textures;

		public MDLFile(string baseDir, string file)
		{
			using(var stream = File.Open(file, FileMode.Open))
			{
				ReadMaterials(baseDir, new BinaryReader(stream));
			}
		}

		private void ReadMaterials(string baseDir, BinaryReader reader)
		{
			reader.BaseStream.Seek(204, SeekOrigin.Begin);

			var textureCount = reader.ReadInt32();
			var textureOffset = reader.ReadInt32();

			var textureDirCount = reader.ReadInt32();
			var textureDirOffset = reader.ReadInt32();
			
			var textures = new List<string>();
			for(var i = 0; i < textureCount; i++)
			{
				reader.BaseStream.Seek(textureOffset + (i * 64), SeekOrigin.Begin);
				var offset = reader.ReadInt32();
				reader.BaseStream.Seek(offset - 4, SeekOrigin.Current);
				textures.Add(BSPFile.ReadNullTerminatedString(reader));
			}
			
			var textureDirs = new List<string>();
			for(var i = 0; i < textureDirCount; i++)
			{
				reader.BaseStream.Seek(textureDirOffset + i * 4, SeekOrigin.Begin);
				var offset = reader.ReadInt32();
				reader.BaseStream.Seek(offset, SeekOrigin.Begin);
				textureDirs.Add(BSPFile.ReadNullTerminatedString(reader));
			}

			// find each texture
			_textures = textures
				.SelectMany(t => textureDirs.Select(d => Path.Combine(baseDir, "materials", d, t)))
				.Where(t => File.Exists(t + ".vmt"))
				.Select(t => t.Replace(Path.GetFullPath(baseDir) + "\\", ""))
				.ToArray();
		}
	}
}
