using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeGameDataReader;

namespace SourceMapAnalyzer
{
	// allows entity definitions to be looked up from multiple FGD files
	public class FGDLookup
	{
		private Dictionary<string, Dictionary<string, FGDType>> _entityProperties =
			new Dictionary<string, Dictionary<string, FGDType>>(); 

		public FGDLookup(params ForgeGameData[] data)
		{
			foreach(var file in data)
			{
				foreach(var ent in file.EntityClasses)
				{
					var props = new Dictionary<string, FGDType>();

					AddBaseToProperties(ent, props);

					foreach(var prop in ent.Properties)
					{
						props[prop.Name] = TypeFromString(prop.DataType);
					}

					_entityProperties[ent.Name] = props;
				}
			}
		}

		public FGDType FindPropertyType(string name, string prop)
		{
			if(!_entityProperties.ContainsKey(name)) return FGDType.None;
			if(!_entityProperties[name].ContainsKey(prop)) return FGDType.None;
			return _entityProperties[name][prop];
		}

		private void AddBaseToProperties(ForgeGameData.EntityClass ent, Dictionary<string, FGDType> props)
		{
			foreach(var b in ent.BaseClasses)
			{
				AddBaseToProperties(b, props);

				foreach(var prop in b.Properties)
				{
					props[prop.Name] = TypeFromString(prop.DataType);
				}
			}
		}

		private FGDType TypeFromString(string str) => (FGDType)Array.IndexOf(_internalTypeNames, str);

		private readonly string[] _internalTypeNames = 
		{
			"string",
			"integer",
			"float",
			"axis",
			"angle",
			"color255",
			"color1",
			"filterclass",
			"material",
			"node_dest",
			"npcclass",
			"origin",
			"pointentityclass",
			"scene",
			"sidelist",
			"sound",
			"sprite",
			"studio",
			"target_destination",
			"target_name_or_class",
			"target_source",
			"vecline",
			"vector"
		};

		public enum FGDType
		{
			String,
			Integer,
			Float,
			Axis,
			Angle,
			Color255,
			Color1,
			FilterClass,
			Material,
			NodeDest,
			NPCClass,
			Origin,
			PointEntityClass,
			Scene,
			Sidelist,
			Sound,
			Sprite,
			Studio,
			TargetDestination,
			TargetNameOrClass,
			TargetSource,
			Vecline,
			Vector,
			None
		}
	}
}
