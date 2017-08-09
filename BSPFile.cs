using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceMapAnalyzer
{
	/// <summary>
	/// Encapsulates a single BSP file.
	/// </summary>
	public class BSPFile
	{
		// constants used within the file
		private const uint MAGIC_NUMBER = 0x50534256;
		private const uint PAKFILE_EOCD_SIG = 0x06054b50;
		private const uint PAKFILE_CD_SIG = 0x02014b50;
		private const int HEADER_LUMPS = 64;
		private const int LUMP_ENTITIES = 0;
		private const int LUMP_TEXDATA_STRING_DATA = 43;
		private const int LUMP_TEXDATA_STRING_TABLE = 44;
		private const int LUMP_PAKFILE = 40;
		private const int LUMP_GAME = 35;

		private int _version;
		private int _mapVersion;
		private string[] _materials;
		private string[] _models;
		private string[] _sounds;
		private string[] _sprites;
		private string[] _pakfile;
		private Dictionary<string, string>[] _entities;
		private BSPLump[] _lumps;
		private FGDLookup _fgdLookup;
		private string _file;

		/// <summary>
		/// The path to the file.
		/// </summary>
		public string FilePath => _file;
		/// <summary>
		/// The version of the BSP format used.
		/// </summary>
		public int Version => _version;
		/// <summary>
		/// The version of the map.
		/// </summary>
		public int MapVersion => _mapVersion;
		/// <summary>
		/// A list of paths to materials.
		/// </summary>
		public string[] Materials => _materials;
		/// <summary>
		/// A list of paths to models.
		/// </summary>
		public string[] Models => _models;
		/// <summary>
		/// A list of paths to sounds.
		/// </summary>
		public string[] Sounds => _sounds;
		/// <summary>
		/// A list of paths to sprites.
		/// </summary>
		public string[] Sprites => _sprites;
		/// <summary>
		/// A list of resources contained within the pakfile attached to the BSP file.
		/// </summary>
		public string[] PakfileResources => _pakfile;
		/// <summary>
		/// A list of entities, represented as a dictionary of key-value pairs.
		/// </summary>
		public Dictionary<string, string>[] Entities => _entities;

		/// <summary>
		/// Create a BSP file.
		/// </summary>
		public BSPFile(FGDLookup lookup, string file)
		{
			_file = file;
			_fgdLookup = lookup;

			using(var f = File.Open(file, FileMode.Open))
			{
				ReadFile(f);
			}
		}

		// Reads info from the file.
		private void ReadFile(Stream inputStream)
		{
			var reader = new BinaryReader(inputStream);

			// sanity check
			if(reader.ReadUInt32() != MAGIC_NUMBER)
			{
				throw new Exception("Unknown file format.");
			}

			// read info from BSP header
			_version = reader.ReadInt32();
			_lumps = new BSPLump[HEADER_LUMPS];
			for(var i = 0; i < HEADER_LUMPS; i++)
			{
				var lump = new BSPLump();
				lump.Offset = reader.ReadInt32();
				lump.Length = reader.ReadInt32();
				lump.Version = reader.ReadInt32();
				lump.Code = Encoding.ASCII.GetString(reader.ReadBytes(4));
				_lumps[i] = lump;
			}

			_mapVersion = reader.ReadInt32();

			// read the data
			ReadTextures(reader);
			ReadEntities(reader);
			ReadPakfile(reader);
			ReadStaticProps(reader);
			
			reader.Close();
		}

		// static props are encoded differently from other props, and point to models
		private void ReadStaticProps(BinaryReader reader)
		{
			SeekToLump(reader, LUMP_GAME);
			var gameLumpCount = reader.ReadInt32();
			var gameLumps = new Dictionary<string, BSPLump>();

			// lumps inside lumps here
			for(var i = 0; i < gameLumpCount; i++)
			{
				var lump = new BSPLump();
				lump.Code = Encoding.ASCII.GetString(reader.ReadBytes(4));
				reader.ReadInt16();
				lump.Version = reader.ReadUInt16();
				lump.Offset = reader.ReadInt32();
				lump.Length = reader.ReadInt32();
				gameLumps[lump.Code] = lump;
			}

			if(!gameLumps.ContainsKey("prps"))
				return;

			reader.BaseStream.Seek(gameLumps["prps"].Offset, SeekOrigin.Begin);
			var entryCount = reader.ReadInt32();
			var models = new List<string>();
			for(var i = 0; i < entryCount; i++)
			{
				var str = Encoding.ASCII.GetString(reader.ReadBytes(128)).TrimEnd('\0');
				models.Add(ConformPath(str));
			}

			_models = _models.Concat(models).Distinct().ToArray();
		}

		// the pakfile is a zip file attached to the BSP that contains map-specific resources
		private void ReadPakfile(BinaryReader reader)
		{
			SeekToLump(reader, LUMP_PAKFILE);

			// attempt to find start of the EOCD record
			var start = _lumps[LUMP_PAKFILE].Offset;
			// 22 is the smallest possible size of the EOCD
			var offset = _lumps[LUMP_PAKFILE].Length - 22;
			var cdSize = 0;
			long cdOffset = -1;
			while(offset > 0)
			{
				reader.BaseStream.Seek(start + offset, SeekOrigin.Begin);
				if(reader.ReadUInt32() == PAKFILE_EOCD_SIG)
				{
					// read the size of the central directory
					reader.BaseStream.Seek(start + offset + 12, SeekOrigin.Begin);
					cdSize = reader.ReadInt32();
					// calculate the pos of the cd, seek there, and see if it's right
					var offsetTest = start + offset - cdSize;
					reader.BaseStream.Seek(offsetTest, SeekOrigin.Begin);
					if(reader.ReadUInt32() == PAKFILE_CD_SIG)
					{
						cdOffset = offsetTest;
						break;
					}
				}
				offset--;
			}

			if(cdOffset == -1)
			{
				throw new Exception("Can't find pakfile central directory!");
			}

			var files = new List<string>();
			reader.BaseStream.Seek(cdOffset, SeekOrigin.Begin);
			while(reader.ReadUInt32() == PAKFILE_CD_SIG)
			{
				// seek to the parts we care about
				reader.BaseStream.Seek(24, SeekOrigin.Current);
				var fileNameLength = reader.ReadUInt16();
				var extraFieldLength = reader.ReadUInt16();
				var commentLength = reader.ReadUInt16();
				reader.BaseStream.Seek(12, SeekOrigin.Current);

				var fileName = Encoding.ASCII.GetString(reader.ReadBytes(fileNameLength));
				files.Add(TrimExtension(fileName));
				reader.BaseStream.Seek(extraFieldLength + commentLength, SeekOrigin.Current);
			}

			_pakfile = files.ToArray();
		}

		// entities reference all sorts of resources
		private void ReadEntities(BinaryReader reader)
		{
			SeekToLump(reader, LUMP_ENTITIES);
			var entityData = Encoding.ASCII.GetString(reader.ReadBytes(_lumps[LUMP_ENTITIES].Length));
			var materials = new HashSet<string>();
			var models = new HashSet<string>();
			var sounds = new HashSet<string>();
			var sprites = new HashSet<string>();

			// they're encoded in the map in the valve kv format
			var ents = new List<Dictionary<string, string>>();
			Dictionary<string, string> ent = null;
			foreach(var line in entityData.Split('\n'))
			{
				var trimmedLine = line.Trim();
				if(trimmedLine == "{")
				{
					ent = new Dictionary<string, string>();
					continue;
				}
				else if(trimmedLine == "}")
				{
					ents.Add(ent);
					AddResourcesFromEnt(ent, materials, models, sounds, sprites);
					continue;
				}
				else if(trimmedLine == "\0")
				{
					continue;
				}

				var parts = trimmedLine.Split(' ');
				ent[parts[0].Trim('"')] = parts[1].Trim('"');
			}

			_entities = ents.ToArray();
			// add to the materials list what the materials list doesn't already contain
			_materials = materials.Concat(_materials).Distinct().ToArray();
			_models = models.ToArray();
			_sounds = sounds.ToArray();
			_sprites = sprites.ToArray();
		}

		// parse the entity and add resources
		private void AddResourcesFromEnt(
			Dictionary<string, string> ent, 
			HashSet<string> materials, 
			HashSet<string> models, 
			HashSet<string> sounds,
			HashSet<string> sprites)
		{
			// look up the entity in the FGD file to find out what resources are used in it
			var name = ent["classname"];
			foreach(var key in ent.Keys)
			{
				var type = _fgdLookup.FindPropertyType(name, key);
				if(type == FGDLookup.FGDType.Material)
				{
					materials.Add(ConformPath(ent[key]));
				}
				else if(type == FGDLookup.FGDType.Sprite)
				{
					sprites.Add(ConformPath(ent[key]));
				}
				else if(type == FGDLookup.FGDType.Studio) // studiomdl, go figure
				{
					models.Add(ConformPath(ent[key]));
				}
				else if(type == FGDLookup.FGDType.Sound)
				{
					if(ent[key].EndsWith("wav", StringComparison.CurrentCulture) || ent[key].EndsWith("mp3", StringComparison.CurrentCulture))
						sounds.Add(ConformPath(ent[key]));
					else
						sounds.Add(ent[key]);
				}
			}
		}

		// read textures used in the map itself (in brushes)
		private void ReadTextures(BinaryReader reader)
		{
			var numTextures = _lumps[LUMP_TEXDATA_STRING_TABLE].Length / 4;
			var textureOffsets = new int[numTextures];
			_materials = new string[numTextures];
			SeekToLump(reader, LUMP_TEXDATA_STRING_TABLE);

			for(var i = 0; i < numTextures; i++)
			{
				textureOffsets[i] = reader.ReadInt32();
			}

			var baseOffset = _lumps[LUMP_TEXDATA_STRING_DATA].Offset;
			for(var i = 0; i < numTextures; i++)
			{
				reader.BaseStream.Seek(baseOffset + textureOffsets[i], SeekOrigin.Begin);
				_materials[i] = ReadNullTerminatedString(reader).ToLower().Replace('/', '\\');
			}
		}

		// seek to the specific lump offset
		private void SeekToLump(BinaryReader reader, int lump) => reader.BaseStream.Seek(_lumps[lump].Offset, SeekOrigin.Begin);

		// maps contain all sorts of interesting paths
		private string ConformPath(string file)
		{
			var noExtPath = TrimExtension(file);
			if(noExtPath.StartsWith(".\\", StringComparison.CurrentCulture))
				return noExtPath.Substring(2);
			return noExtPath;
		}
		
		// sometimes files have extensions... and sometimes they don't
		private string TrimExtension(string file) => 
			Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));

		public static string ReadNullTerminatedString(BinaryReader reader)
		{
			var bytes = new List<byte>();
			byte readByte;
			while((readByte = reader.ReadByte()) != 0)
				bytes.Add(readByte);
			return Encoding.ASCII.GetString(bytes.ToArray());
		}

		private struct BSPLump
		{
			public int Offset;
			public int Length;
			public int Version;
			public string Code;
		}
	}
}
