using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SourceMapAnalyzer
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			string map = null;
			string gameDir = null;
			bool makePackage = false;
			var baseFgds = new List<string>();
			var gameFgds = new List<string>();
			var vpks = new List<string>();

			var p = new OptionSet()
			{
				{ "m|map=", (m) => map = m },
				{ "b|base=", (b) => baseFgds.Add(b) },
				{ "g|game=", (g) => gameFgds.Add(g) },
				{ "v|vpk=", (v) => vpks.Add(v) },
				{ "d|dir=", (d) => gameDir = d },
				{ "package", (pack) => makePackage = pack != null }
			};

			if(map == null)
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new MainForm());
				return;
			}

			p.Parse(args);
			var analyzer = new MapAnalyzer(map, gameDir, baseFgds, gameFgds, vpks);
			analyzer.Output(makePackage, gameDir);
		}
	}
}
