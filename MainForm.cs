using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs.WinForms;
using Newtonsoft.Json;
using System.IO;
using System.Security.Policy;

namespace SourceMapAnalyzer
{
	public partial class MainForm : Form
	{
		private HashSet<string> portedVpks = new HashSet<string>();

		public MainForm()
		{
			InitializeComponent();
			RegisterMultiSelectEvents(baseFgdAddButton, baseFgdListBox, "Forge Game Data (*.fgd)|*.fgd");
			RegisterMultiSelectEvents(vpkAddButton, vpkListBox, "Valve Pack File (*.vpk)|*.vpk");
			RegisterMultiSelectEvents(mapAddButton, mapBox, "BSP Map (*.bsp)|*.bsp");

			gameFgdButton.Click += (sender, e) =>
			{
				var dialog = new OpenFileDialog();
				dialog.Filter = "Forge Game Data (*.fgd)|*.fgd|All Files (*.*)|*.*";
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					gameFgdBox.Text = dialog.FileName;
				}

				UpdateButtonConditions();
			};

			gameDirButton.Click += (sender, e) =>
			{
				var dialog = new VistaFolderBrowserDialog();
				if(dialog.ShowDialog() == DialogResult.OK)
				{
					gameDirBox.Text = dialog.SelectedPath;
				}

				UpdateButtonConditions();
			};

			portedVpksList.ItemCheck += (sender, e) =>
			{
				portedVpks.Clear();
				foreach(var item in portedVpksList.CheckedItems)
				{
					portedVpks.Add(item.ToString());
				}
			};

			// load cache
			if(File.Exists(".cache.json"))
			{
				var cachedData = JsonConvert.DeserializeObject<CachedValues>(File.ReadAllText(".cache.json"));
				baseFgdListBox.Items.AddRange(cachedData.FgdFiles);
				vpkListBox.Items.AddRange(cachedData.VpkFiles);
				mapBox.Items.AddRange(cachedData.Maps);
				gameDirBox.Text = cachedData.GameDir;
				gameFgdBox.Text = cachedData.GameFgd;
				packageModeDropdown.SelectedIndex = packageModeDropdown.Items.IndexOf(cachedData.PackageMode);
				portedVpks = new HashSet<string>(cachedData.PortedVpks ?? new string[] { });

				UpdateButtonConditions();
				UpdatePortedVpkList();
			}
		}

		private void UpdateButtonConditions()
		{
			processButton.Enabled = (
				baseFgdListBox.Items.Count > 0 &&
				vpkListBox.Items.Count > 0 &&
				mapBox.Items.Count > 0 &&
				!string.IsNullOrWhiteSpace(gameFgdBox.Text) &&
				!string.IsNullOrWhiteSpace(gameDirBox.Text)
			);
		}

		private void UpdatePortedVpkList()
		{
			portedVpksList.Items.Clear();

			var dirs = new HashSet<string>(vpkListBox.Items.Cast<string>().Select(p => Path.GetFileName(Path.GetDirectoryName(p))));
			foreach(var dir in dirs)
			{
				portedVpksList.Items.Add(dir, portedVpks.Contains(dir));
			}
		}

		private void RegisterMultiSelectEvents(Button button, ListBox box, string type)
		{
			button.Click += (sender, e) =>
			{
				var dialog = new OpenFileDialog();
				dialog.Multiselect = true;
				dialog.Filter = type + "|All Files (*.*)|*.*";
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					box.Items.AddRange(dialog.FileNames);
				}

				UpdateButtonConditions();
				UpdatePortedVpkList();
			};

			box.KeyUp += (sender, e) =>
			{
				if (e.KeyCode != Keys.Delete) return;
				if (box.SelectedItems.Count == 0) return;

				var newItems = new List<string>();
				for (var i = 0; i < box.Items.Count; i++)
				{
					if (!box.SelectedIndices.Contains(i))
						newItems.Add(box.Items[i].ToString());
				}

				box.Items.Clear();
				box.Items.AddRange(newItems.ToArray());

				UpdateButtonConditions();
				UpdatePortedVpkList();
			};
		}

		private void processButton_Click(object sender, System.EventArgs e)
		{
			var packageModeText = packageModeDropdown.SelectedItem.ToString();
			portedVpks.Clear();
			foreach(var item in portedVpksList.CheckedItems)
			{
				portedVpks.Add(item.ToString());
			}

			// cache inputs
			var cache = new CachedValues()
			{
				FgdFiles = baseFgdListBox.Items.Cast<string>().ToArray(),
				VpkFiles = vpkListBox.Items.Cast<string>().ToArray(),
				Maps = mapBox.Items.Cast<string>().ToArray(),
				GameDir = gameDirBox.Text,
				GameFgd = gameFgdBox.Text,
				PackageMode = packageModeText,
				PortedVpks = portedVpks.ToArray()
			};
			File.WriteAllText(".cache.json", JsonConvert.SerializeObject(cache));

			var sourceVpks = new HashSet<string>();
			var portVpks = new HashSet<string>();

			foreach(var vpk in vpkListBox.Items.Cast<string>())
			{
				var dir = Path.GetFileName(Path.GetDirectoryName(vpk));
				if(portedVpks.Contains(dir))
				{
					portVpks.Add(vpk);
				}
				else
				{
					sourceVpks.Add(vpk);
				}
			}

			var vfs = new VirtualFileSystem(portVpks.ToArray(), new string[] { gameDirBox.Text });

			var fgdFiles = baseFgdListBox.Items.Cast<string>();
			var analyzers = new Dictionary<string, MapAnalyzer>();
			foreach(var map in mapBox.Items)
			{
				var analyzer = new MapAnalyzer(map.ToString(), vfs, fgdFiles, new string[] { gameFgdBox.Text }, sourceVpks.ToArray());
				analyzer.Output();
				analyzers[map.ToString()] = analyzer;
			}

			var packageMode = PackageMode.None;
			switch(packageModeText)
			{
				case "None":
				default:
					packageMode = PackageMode.None;
					break;
				case "Folder Per Map":
					packageMode = PackageMode.FolderPerMap;
					break;
				case "Combined Folder":
					packageMode = PackageMode.CombinedFolder;
					break;
				case "Combined Addon":
					packageMode = PackageMode.CombinedAddon;
					break;
			}

			Packager.Package(analyzers, packageMode, vfs);

			MessageBox.Show("Completed!");
		}

		private class CachedValues
		{
			public string[] FgdFiles;
			public string[] VpkFiles;
			public string[] Maps;
			public string GameDir;
			public string GameFgd;
			public string PackageMode;
			public string[] PortedVpks;
		}
	}
}
