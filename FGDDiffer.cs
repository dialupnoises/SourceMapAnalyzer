using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeGameDataReader;
using EntityClass = ForgeGameDataReader.ForgeGameData.EntityClass;

namespace SourceMapAnalyzer
{
	/// <summary>
	/// Returns the entities unique in the second set of FGDs from the first.
	/// </summary>
	public static class FGDDiffer
	{
		public static FGDDiff Diff(IEnumerable<ForgeGameData> sourceData, IEnumerable<ForgeGameData> newData)
		{
			return new FGDDiff(DoDiff(sourceData, newData));
		}

		private static IEnumerable<Tuple<EntityClass, EntityClass>> DoDiff(IEnumerable<ForgeGameData> sourceData, IEnumerable<ForgeGameData> newData)
		{
			var sourceEntries = new Dictionary<string, EntityClass>();
			foreach(var data in sourceData)
			{
				foreach(var c in data.EntityClasses)
				{
					if(c.ClassType == ForgeGameData.ClassTypes.BaseClass) continue;
					sourceEntries[c.Name] = c;
				}
			}
			
			foreach(var data in newData)
			{
				foreach(var c in data.EntityClasses)
				{
					if(c.ClassType == ForgeGameData.ClassTypes.BaseClass) continue;

					if(!sourceEntries.ContainsKey(c.Name))
						yield return new Tuple<EntityClass, EntityClass>(null, c);
					else if(!CompareEntities(sourceEntries[c.Name], c))
						yield return new Tuple<EntityClass, EntityClass>(sourceEntries[c.Name], c);
				}
			}
		}

		// check to see if the two entities are equivalent to each other
		private static bool CompareEntities(EntityClass a, EntityClass b)
		{
			return
				a.ClassType == b.ClassType &&
				a.BaseClasses.All(c => b.BaseClasses.Any(bc => bc.Name == c.Name)) &&
				a.Inputs.All(i => ContainsInput(b, i)) &&
				a.Outputs.All(o => ContainsOutput(b, o)) &&
				a.Properties.All(p => ContainsProperty(b, p)) &&
				a.SpecialProperties.All(p => ContainsSpecialProperty(b, p));
		}

		private static bool ContainsOutput(EntityClass ent, EntityClass.Output output) =>
			ent.Outputs.Any(o => o.DataType == output.DataType && o.Name == output.Name);

		private static bool ContainsInput(EntityClass ent, EntityClass.Input input) =>
			ent.Inputs.Any(i => i.Name == input.Name && i.DataType == input.DataType);

		private static bool ContainsSpecialProperty(EntityClass ent, EntityClass.SpecialProperty prop) =>
			ent.SpecialProperties.Any(p =>
				p.Name == prop.Name &&
				p.Parameters == prop.Parameters);

		private static bool ContainsProperty(EntityClass ent, EntityClass.Property prop) =>
			ent.Properties.Any(p =>
				p.Name == prop.Name &&
				p.DefaultValue == prop.DefaultValue &&
				p.DataType == prop.DataType &&
				p.ShortDescription == prop.ShortDescription &&
				p.LongDescription == prop.LongDescription &&
				p.Flags.All(f => ContainsFlag(prop, f)) &&
				p.Options.All(o => ContainsOption(prop, o)));

		private static bool ContainsFlag(EntityClass.Property prop, EntityClass.Property.Flag flag) =>
			prop.Flags.Any(f =>
				f.MaskValue == flag.MaskValue &&
				f.DefaultValue == flag.DefaultValue &&
				f.Description == flag.Description);

		private static bool ContainsOption(EntityClass.Property prop, EntityClass.Property.Option option) =>
			prop.Options.Any(o =>
				o.Description == option.Description &&
				o.Value == option.Value);
	}

	public class FGDDiff
	{
		private EntityClass[] _uniqueEntities;
		private EntityClass[] _differentEntities;

		public EntityClass[] UniqueEntities => _uniqueEntities;
		public EntityClass[] DifferentEntities => _differentEntities;

		public FGDDiff(IEnumerable<Tuple<EntityClass, EntityClass>> diffs)
		{
			_uniqueEntities = diffs.Where(d => d.Item1 == null).Select(d => d.Item2).ToArray();
			_differentEntities = diffs.Where(d => d.Item1 != null).Select(d => d.Item2).ToArray();
		}

		public DiffType FindEntType(string name)
		{
			if(UniqueEntities.Any(e => e.Name == name))
				return DiffType.Unique;
			if(DifferentEntities.Any(e => e.Name == name))
				return DiffType.Different;
			return DiffType.Shared;
		}

		public enum DiffType
		{
			Different,
			Unique,
			Shared
		}
	}
}
