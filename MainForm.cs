using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace SourceMapAnalyzer
{
	public partial class MainForm : Form
	{
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
			};
		}

		private void processButton_Click(object sender, System.EventArgs e)
		{
			var fgdFiles = baseFgdListBox.Items.Cast<string>();
			var vpkFiles = vpkListBox.Items.Cast<string>();
			foreach(var map in mapBox.Items)
			{
				var analyzer = new MapAnalyzer(map.ToString(), gameDirBox.Text, fgdFiles, new string[] { gameFgdBox.Text }, vpkFiles);
				analyzer.Output(packageCheckbox.Checked, gameDirBox.Text);
			}

			MessageBox.Show("Completed!");
		}
	}
}
