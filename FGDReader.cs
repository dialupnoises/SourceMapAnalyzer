// from http://www.interlopers.net/forum/viewtopic.php?f=25&t=32165

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ForgeGameDataReader
{
	class TokenReader
	{
		public static bool BeginningOfLineComment(String text, int index)
		{
			return text[index] == '/' && index + 1 < text.Length && text[index + 1] == '/';
		}

		public static void SkipWhiteSpace(String text, ref int index, out bool newLineEncountered)
		{
			newLineEncountered = false;
			while (index < text.Length && (Char.IsWhiteSpace(text[index]) || BeginningOfLineComment(text, index)))
			{
				if (BeginningOfLineComment(text, index))
				{
					SkipLineComment(text, ref index); newLineEncountered = true;
				}
				else
				{
					if (text[index] == '\n') newLineEncountered = true;
					index++;
				}
			}
		}

		public static String GetNextToken(String text, ref int index, out bool newLineEncountered)
		{
			const string wordSTOP = "{}\"()";
			String result = "";
			SkipWhiteSpace(text, ref index, out newLineEncountered);
			bool inQuote = false;
			if (index == text.Length) return null;
			if (text[index] == '"')
			{
				inQuote = true;
				index++; //leading quote
			}
			while (index < text.Length && ((inQuote && text[index] != '\r' && text[index] != '\n' && text[index] != '"') || (!wordSTOP.Contains(text[index]) && !Char.IsWhiteSpace(text[index]))))
			{
				if (BeginningOfLineComment(text, index))
				{
					//just hit a comment
					break;
				}
				result += text[index++];
			}
			if (index < text.Length && text[index] == '"') index++; //advance past trailing quote
			return result;
		}

		public static String GetNextToken(String text, ref int index, char delimiterCharacter)
		{
			const string wordSTOP = "{}\"()";
			String result = "";
			bool newLineEncountered;
			int previousIndex = index;
			SkipWhiteSpace(text, ref index, out newLineEncountered);
			if (newLineEncountered)
			{
				index = previousIndex;
				return null;
			}
			if (text[index] != delimiterCharacter)
			{
				return null;
			}
			index++;
			SkipWhiteSpace(text, ref index, out newLineEncountered);
			bool inQuote = false;
			if (index == text.Length) return null;
			if (text[index] == '"')
			{
				inQuote = true;
				index++; //leading quote
			}
			while (index < text.Length && ((inQuote && text[index] != '\r' && text[index] != '\n' && text[index] != '"') || (!wordSTOP.Contains(text[index]) && !Char.IsWhiteSpace(text[index]))))
			{
				if (!inQuote && BeginningOfLineComment(text, index))
				{
					//just hit a comment
					break;
				}
				if (!inQuote && text[index] == delimiterCharacter)
					break;
				result += text[index++];
			}
			if (text[index] == '"')
			{
				index++; //advance past trailing quote
				bool rewind;
				int savedIndex = index;
				SkipWhiteSpace(text, ref index, out rewind);
				if (rewind) index = savedIndex;
			}
			return result.Trim();
		}

		public static String GetFirstToken(String text, ref int index, char delimiterCharacter)
		{
			const string wordSTOP = "{}\"()";
			String result = "";
			bool newLineEncountered;
			int previousIndex = index;
			SkipWhiteSpace(text, ref index, out newLineEncountered);
			//             if (newLineEncountered)
			//             {
			//                 index = previousIndex;
			//                 return null;
			//             }
			//             if (text[index] != delimiterCharacter)
			//             {
			//                 return null;
			//             }
			bool inQuote = false;
			if (index == text.Length) return null;
			if (text[index] == '"')
			{
				inQuote = true;
				index++; //leading quote
			}
			while (index < text.Length && ((inQuote && text[index] != '\r' && text[index] != '\n' && text[index] != '"') || (!wordSTOP.Contains(text[index]) && !Char.IsWhiteSpace(text[index]))))
			{
				if (!inQuote && BeginningOfLineComment(text, index))
				{
					//just hit a comment
					break;
				}
				if (!inQuote && text[index] == delimiterCharacter)
					break;
				result += text[index++];
			}
			if (text[index] == '"')
			{
				index++; //advance past trailing quote
				bool rewind;
				int savedIndex = index;
				SkipWhiteSpace(text, ref index, out rewind);
				if (rewind) index = savedIndex;
			}
			return result.Trim();
		}

		public static String PeekNextToken(String text, int index, out bool newLineEncountered)
		{
			int copy = index;
			return GetNextToken(text, ref copy, out newLineEncountered);
		}

		public static String PeekNextToken(String text, int index, bool allowNewLine)
		{
			bool newLine;
			String result = PeekNextToken(text, index, out newLine);
			if (!allowNewLine && newLine) return null;
			return result;
		}

		public static bool TryNextChar(String text, ref int index, char nextChar, bool allowNewLine)
		{
			int savedIndex = index;
			bool newLine;
			SkipWhiteSpace(text, ref index, out newLine);
			if (!allowNewLine && newLine)
			{
				index = savedIndex;
				return false;
			}
			if (index < text.Length && text[index] == nextChar)
			{
				index++;
				return true;
			}
			else
			{
				index = savedIndex;
				return false;
			}
		}

		public static bool TryNextToken(String text, ref int index, String nextToken, bool allowNewLine)
		{
			int savedIndex = index;
			bool newLine;
			String foundToken = GetNextToken(text, ref index, out newLine);
			if (!allowNewLine && newLine)
			{
				index = savedIndex;
				return false;
			}
			if (foundToken.ToLower() == nextToken.ToLower())
			{
				return true;
			}
			else
			{
				index = savedIndex;
				return false;
			}
		}

		public static void SkipLineComment(String text, ref int index)
		{
			int newLineLength = Environment.NewLine.Length;
			int newLineIndex = text.IndexOf(Environment.NewLine, index);
			if (newLineIndex == -1)
				index = text.Length;
			else
				index = newLineIndex + newLineLength; //go past it
		}

		public static String ReadToNext(String text, ref int index, String stopString)
		{
			int stopPosition = text.IndexOf(stopString, index);
			int lineCommentPosition = text.IndexOf("//", index);
			String result = "";
			while (true)
			{
				if (stopPosition == -1)
				{
					throw new FormatException(stopString + " not found searching from starting character position: " + index);
				}
				if (lineCommentPosition == -1 || lineCommentPosition > stopPosition)
				{
					//there are no comments where we currently care about. Do this fast.
					result += text.Substring(index, stopPosition - index);
					index = stopPosition;
					return result;
				}
				else
				{
					//there's a comment before our end point. Do this the hard way
					stopPosition = lineCommentPosition;
					result += text.Substring(index, stopPosition - index);
					index = stopPosition;
					SkipLineComment(text, ref index);
					stopPosition = text.IndexOf(stopString, index);
					lineCommentPosition = text.IndexOf("//", index);
				}
			}
		}

		public static String[] SplitToNext(String text, ref int index, String stopString, String splitString)
		{
			String textTo = ReadToNext(text, ref index, stopString);
			String[] tokens = textTo.Split(new string[] { splitString }, StringSplitOptions.None);
			for (int tIndex = 0; tIndex < tokens.Length; tIndex++)
			{
				tokens[tIndex] = tokens[tIndex].Trim();
			}
			return tokens;
		}

		public static void ReadExpected(String text, ref int index, String expectedString)
		{
			int startIndex = index;
			bool dontCare;
			SkipWhiteSpace(text, ref index, out dontCare);
			if (text.Length - index < expectedString.Length || text.Substring(index, expectedString.Length).ToLower() != expectedString.ToLower()) throw new FormatException(expectedString + " expected at character position: " + startIndex);
			index += expectedString.Length;
		}
	}

	public class ForgeGameData
	{
		public static String ReadFGDDescription(String text, ref int index)
		{
			String result = "";
			bool dontCare;
			while (true)
			{
				result += TokenReader.GetNextToken(text, ref index, out dontCare);
				bool newLine;
				TokenReader.SkipWhiteSpace(text, ref index, out newLine);
				if (newLine || text[index] != '+') break;
				index++;
			}
			return result;
		}

		public enum ClassTypes
		{
			BaseClass,
			PointClass,
			NPCClass,
			SolidClass,
			KeyFrameClass,
			MoveClass,
			FilterClass
		}

		public class EntityClass
		{
			public ClassTypes ClassType;
			public List<EntityClass> BaseClasses = new List<EntityClass>();
			public String Name;
			public String Description;

			public class Property
			{
				public String Name;
				public String DataType;
				public Boolean ReadOnly;
				public String ShortDescription;
				public String DefaultValue;
				public String LongDescription;

				public class Option
				{
					public String Value;
					public String Description;

					public static Option ParseFromString(String text, ref int index)
					{
						Option temp = new Option();
						temp.Value = TokenReader.GetFirstToken(text, ref index, ':');
						if (TokenReader.TryNextChar(text, ref index, ':', false))
							temp.Description = ForgeGameData.ReadFGDDescription(text, ref index);
						return temp;
					}
				}
				public List<Option> Options = new List<Option>();

				public class Flag
				{
					public UInt32 MaskValue;
					public String Description;
					public Boolean DefaultValue;

					public static Flag ParseFromString(String text, ref int index)
					{
						Flag temp = new Flag();
						temp.MaskValue = Convert.ToUInt32(TokenReader.GetFirstToken(text, ref index, ':'));
						if (TokenReader.TryNextChar(text, ref index, ':', false))
							temp.Description = ForgeGameData.ReadFGDDescription(text, ref index);
						int zeroOrOneExpectedIndex = index;
						String zeroOrOne = TokenReader.GetNextToken(text, ref index, ':');
						temp.DefaultValue = zeroOrOne != "0";
						return temp;
					}
				}
				public List<Flag> Flags = new List<Flag>();

				public static Property ParseFromString(String text, ref int index)
				{
					Property temp = new Property();
					bool dontCare;
					temp.Name = TokenReader.ReadToNext(text, ref index, "(").Trim();
					index++; //skip (
					temp.DataType = TokenReader.ReadToNext(text, ref index, ")").Trim();
					index++; //skip )
					temp.ReadOnly = TokenReader.TryNextToken(text, ref index, "readonly", false);
					temp.ShortDescription = TokenReader.GetNextToken(text, ref index, ':');
					temp.DefaultValue = TokenReader.GetNextToken(text, ref index, ':');
					if (TokenReader.TryNextChar(text, ref index, ':', false)) temp.LongDescription = ForgeGameData.ReadFGDDescription(text, ref index);
					if (temp.DataType.ToLower() == "choices")
					{
						TokenReader.ReadExpected(text, ref index, "=");
						TokenReader.ReadExpected(text, ref index, "[");
						while (true)
						{
							TokenReader.SkipWhiteSpace(text, ref index, out dontCare);
							if (index >= text.Length) throw new FormatException("End of file encountered while looking for a ] character.");
							if (text[index] == ']')
							{
								index++;
								break;
							}
							temp.Options.Add(Option.ParseFromString(text, ref index));
						}
					}
					else if (temp.DataType.ToLower() == "flags")
					{
						TokenReader.ReadExpected(text, ref index, "=");
						TokenReader.ReadExpected(text, ref index, "[");
						while (true)
						{
							TokenReader.SkipWhiteSpace(text, ref index, out dontCare);
							if (index >= text.Length) throw new FormatException("End of file encountered while looking for a ] character.");
							if (text[index] == ']')
							{
								index++;
								break;
							}
							temp.Flags.Add(Flag.ParseFromString(text, ref index));
						}
					}
					return temp;
				}
			}
			public List<Property> Properties = new List<Property>();

			public class Output
			{
				public String Name;
				public String DataType;
				public String Description;

				public static Output ParseFromString(String text, ref int index)
				{
					Output temp = new Output();
					temp.Name = TokenReader.ReadToNext(text, ref index, "(").Trim();
					index++; //skip (
					temp.DataType = TokenReader.ReadToNext(text, ref index, ")").Trim();
					index++; //skip )
					if (TokenReader.TryNextChar(text, ref index, ':', false)) temp.Description = ForgeGameData.ReadFGDDescription(text, ref index).Trim();
					return temp;
				}
			}
			public List<Output> Outputs = new List<Output>();

			public class Input
			{
				public String Name;
				public String DataType;
				public String Description;

				public static Input ParseFromString(String text, ref int index)
				{
					Input temp = new Input();
					temp.Name = TokenReader.ReadToNext(text, ref index, "(").Trim();
					index++; //skip (
					temp.DataType = TokenReader.ReadToNext(text, ref index, ")").Trim();
					index++; //skip )
					if (TokenReader.TryNextChar(text, ref index, ':', false)) temp.Description = ForgeGameData.ReadFGDDescription(text, ref index).Trim();
					return temp;
				}
			}
			public List<Input> Inputs = new List<Input>();

			public class SpecialProperty
			{
				public String Name;
				public String Parameters;
			}
			public List<SpecialProperty> SpecialProperties = new List<SpecialProperty>();

			public static EntityClass ParseFromString(String text, ref int index, List<EntityClass> existingClasses)
			{
				EntityClass temp = new EntityClass();
				Boolean dontCare;
				int startIndex = index;
				String rootToken = TokenReader.GetNextToken(text, ref index, out dontCare);
				if (rootToken.StartsWith("@"))
				{
					temp.ClassType = (ClassTypes)Enum.Parse(typeof(ClassTypes), rootToken.Substring(1));
					int baseExpectedIndex = index;
					String baseKeyWord = TokenReader.GetNextToken(text, ref index, out dontCare);
					if (baseKeyWord.ToLower() == "base")
					{
						TokenReader.ReadExpected(text, ref index, "(");
						String[] baseClasses = TokenReader.SplitToNext(text, ref index, ")", ",");
						TokenReader.ReadExpected(text, ref index, ")");
						//we have base class names, but we want the actual EntityClass for each:
						foreach (String baseClassName in baseClasses)
						{
							bool found = false;
							foreach (EntityClass searchBaseClass in existingClasses)
							{
								if (searchBaseClass.Name.ToLower() == baseClassName.ToLower())
								{
									temp.BaseClasses.Add(searchBaseClass);
									found = true;
									break;
								}
							}
							if (!found) throw new FormatException("Baseclass '" + baseClassName + "' not found. Check the base class list starting at character position: " + baseExpectedIndex);
						}
					}
					else
					{
						index = baseExpectedIndex;
					}
					while (!TokenReader.TryNextChar(text, ref index, '=', false))
					{
						//not an equal sign here, so must be a special purpose property type, like axis, angle, color, etc.
						SpecialProperty temp2 = new SpecialProperty();
						temp2.Name = TokenReader.ReadToNext(text, ref index, "(");
						index++;
						temp2.Parameters = TokenReader.ReadToNext(text, ref index, ")");
						index++;
						temp.SpecialProperties.Add(temp2);
					}

					int classNameExpectedIndex = index;
					temp.Name = TokenReader.GetFirstToken(text, ref index, ':');
					if (temp.Name == null) throw new FormatException("End of file reached while looking for a class definitions name.");
					if (temp.Name == "") throw new FormatException("Class name expected at character position: " + classNameExpectedIndex);
					if (TokenReader.TryNextChar(text, ref index, ':', false)) temp.Description = ForgeGameData.ReadFGDDescription(text, ref index);
					TokenReader.ReadExpected(text, ref index, "[");

					while (true)
					{
						int propertyStartIndex = index;
						String propertyStart = TokenReader.GetNextToken(text, ref index, out dontCare);
						if (propertyStart == "]") break;
						if (propertyStart.ToLower() == "output") temp.Outputs.Add(Output.ParseFromString(text, ref index));
						else if (propertyStart.ToLower() == "input") temp.Inputs.Add(Input.ParseFromString(text, ref index));
						else
						{
							index = propertyStartIndex;
							temp.Properties.Add(Property.ParseFromString(text, ref index));
						}
					}
					return temp;
				}
				else
				{
					throw new FormatException("Entity class definition must begin with the @ at character position: " + startIndex);
				}
			}

		}
		public List<EntityClass> EntityClasses = new List<EntityClass>();

		public void Merge(ForgeGameData include)
		{
			EntityClasses.AddRange(include.EntityClasses);
		}

		public static ForgeGameData ParseFromString(String text, String workingDirectory)
		{
			int index = 0;
			int startIndex = 0;
			Boolean dontCare;
			ForgeGameData temp = new ForgeGameData();
			string rootToken;
			while ((rootToken = TokenReader.GetNextToken(text, ref index, out dontCare)) != null)
			{
				if (rootToken.ToLower() == "@include")
				{
					bool newLine;
					String fileName = TokenReader.GetNextToken(text, ref index, out newLine);
					if (newLine) throw new FormatException("File name should follow '@include' at character position: " + startIndex);
					temp.Merge(LoadFGD(System.IO.Path.Combine(workingDirectory, fileName)));
				}
				else if (rootToken.ToLower() == "@mapsize")
				{
					TokenReader.ReadToNext(text, ref index, ")");
					index++;
					//dont care
				}
				else if (rootToken.StartsWith("//"))
				{
					index = startIndex;
					TokenReader.SkipLineComment(text, ref index);
				}
				else
				{
					index = startIndex;
					temp.EntityClasses.Add(EntityClass.ParseFromString(text, ref index, temp.EntityClasses));
				}

				startIndex = index;
			}
			return temp;
		}

		public static ForgeGameData LoadFGD(String pathName)
		{
			StreamReader sr = new StreamReader(pathName);
			String text = sr.ReadToEnd();
			sr.Close();
			return ParseFromString(text, System.IO.Path.GetDirectoryName(pathName));
		}
	}
}