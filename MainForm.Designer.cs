namespace SourceMapAnalyzer
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.vpkTabPage = new System.Windows.Forms.TabPage();
			this.vpkContainer = new System.Windows.Forms.SplitContainer();
			this.vpkListBox = new System.Windows.Forms.ListBox();
			this.vpkAddButton = new System.Windows.Forms.Button();
			this.baseTabPage = new System.Windows.Forms.TabPage();
			this.baseFgdContainer = new System.Windows.Forms.SplitContainer();
			this.baseFgdListBox = new System.Windows.Forms.ListBox();
			this.baseFgdAddButton = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.mapTabPage = new System.Windows.Forms.TabPage();
			this.mapContainer = new System.Windows.Forms.SplitContainer();
			this.mapBox = new System.Windows.Forms.ListBox();
			this.mapAddButton = new System.Windows.Forms.Button();
			this.processTab = new System.Windows.Forms.TabPage();
			this.processButton = new System.Windows.Forms.Button();
			this.gameDirBox = new System.Windows.Forms.TextBox();
			this.gameDirButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.gameFgdBox = new System.Windows.Forms.TextBox();
			this.gameFgdButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.packageModeDropdown = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.portedVpksList = new System.Windows.Forms.CheckedListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.vpkTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.vpkContainer)).BeginInit();
			this.vpkContainer.Panel1.SuspendLayout();
			this.vpkContainer.Panel2.SuspendLayout();
			this.vpkContainer.SuspendLayout();
			this.baseTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.baseFgdContainer)).BeginInit();
			this.baseFgdContainer.Panel1.SuspendLayout();
			this.baseFgdContainer.Panel2.SuspendLayout();
			this.baseFgdContainer.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.mapTabPage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mapContainer)).BeginInit();
			this.mapContainer.Panel1.SuspendLayout();
			this.mapContainer.Panel2.SuspendLayout();
			this.mapContainer.SuspendLayout();
			this.processTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// vpkTabPage
			// 
			this.vpkTabPage.Controls.Add(this.vpkContainer);
			this.vpkTabPage.Location = new System.Drawing.Point(4, 22);
			this.vpkTabPage.Name = "vpkTabPage";
			this.vpkTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.vpkTabPage.Size = new System.Drawing.Size(354, 332);
			this.vpkTabPage.TabIndex = 1;
			this.vpkTabPage.Text = "VPKs";
			this.vpkTabPage.UseVisualStyleBackColor = true;
			// 
			// vpkContainer
			// 
			this.vpkContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.vpkContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.vpkContainer.IsSplitterFixed = true;
			this.vpkContainer.Location = new System.Drawing.Point(3, 3);
			this.vpkContainer.Name = "vpkContainer";
			this.vpkContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// vpkContainer.Panel1
			// 
			this.vpkContainer.Panel1.Controls.Add(this.vpkListBox);
			// 
			// vpkContainer.Panel2
			// 
			this.vpkContainer.Panel2.Controls.Add(this.vpkAddButton);
			this.vpkContainer.Size = new System.Drawing.Size(348, 326);
			this.vpkContainer.SplitterDistance = 297;
			this.vpkContainer.TabIndex = 1;
			// 
			// vpkListBox
			// 
			this.vpkListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.vpkListBox.FormattingEnabled = true;
			this.vpkListBox.Location = new System.Drawing.Point(0, 0);
			this.vpkListBox.Name = "vpkListBox";
			this.vpkListBox.Size = new System.Drawing.Size(348, 297);
			this.vpkListBox.TabIndex = 0;
			// 
			// vpkAddButton
			// 
			this.vpkAddButton.Location = new System.Drawing.Point(268, 3);
			this.vpkAddButton.Name = "vpkAddButton";
			this.vpkAddButton.Size = new System.Drawing.Size(75, 23);
			this.vpkAddButton.TabIndex = 0;
			this.vpkAddButton.Text = "Add";
			this.vpkAddButton.UseVisualStyleBackColor = true;
			// 
			// baseTabPage
			// 
			this.baseTabPage.Controls.Add(this.baseFgdContainer);
			this.baseTabPage.Location = new System.Drawing.Point(4, 22);
			this.baseTabPage.Name = "baseTabPage";
			this.baseTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.baseTabPage.Size = new System.Drawing.Size(354, 332);
			this.baseTabPage.TabIndex = 0;
			this.baseTabPage.Text = "Base FGDs";
			this.baseTabPage.UseVisualStyleBackColor = true;
			// 
			// baseFgdContainer
			// 
			this.baseFgdContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.baseFgdContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.baseFgdContainer.IsSplitterFixed = true;
			this.baseFgdContainer.Location = new System.Drawing.Point(3, 3);
			this.baseFgdContainer.Name = "baseFgdContainer";
			this.baseFgdContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// baseFgdContainer.Panel1
			// 
			this.baseFgdContainer.Panel1.Controls.Add(this.baseFgdListBox);
			// 
			// baseFgdContainer.Panel2
			// 
			this.baseFgdContainer.Panel2.Controls.Add(this.baseFgdAddButton);
			this.baseFgdContainer.Size = new System.Drawing.Size(348, 326);
			this.baseFgdContainer.SplitterDistance = 297;
			this.baseFgdContainer.TabIndex = 0;
			// 
			// baseFgdListBox
			// 
			this.baseFgdListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.baseFgdListBox.FormattingEnabled = true;
			this.baseFgdListBox.Location = new System.Drawing.Point(0, 0);
			this.baseFgdListBox.Name = "baseFgdListBox";
			this.baseFgdListBox.Size = new System.Drawing.Size(348, 297);
			this.baseFgdListBox.TabIndex = 0;
			// 
			// baseFgdAddButton
			// 
			this.baseFgdAddButton.Location = new System.Drawing.Point(268, 3);
			this.baseFgdAddButton.Name = "baseFgdAddButton";
			this.baseFgdAddButton.Size = new System.Drawing.Size(75, 23);
			this.baseFgdAddButton.TabIndex = 0;
			this.baseFgdAddButton.Text = "Add";
			this.baseFgdAddButton.UseVisualStyleBackColor = true;
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.baseTabPage);
			this.tabControl.Controls.Add(this.vpkTabPage);
			this.tabControl.Controls.Add(this.mapTabPage);
			this.tabControl.Controls.Add(this.processTab);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(362, 358);
			this.tabControl.TabIndex = 0;
			// 
			// mapTabPage
			// 
			this.mapTabPage.Controls.Add(this.mapContainer);
			this.mapTabPage.Location = new System.Drawing.Point(4, 22);
			this.mapTabPage.Name = "mapTabPage";
			this.mapTabPage.Padding = new System.Windows.Forms.Padding(3);
			this.mapTabPage.Size = new System.Drawing.Size(354, 332);
			this.mapTabPage.TabIndex = 2;
			this.mapTabPage.Text = "Maps";
			this.mapTabPage.UseVisualStyleBackColor = true;
			// 
			// mapContainer
			// 
			this.mapContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.mapContainer.IsSplitterFixed = true;
			this.mapContainer.Location = new System.Drawing.Point(3, 3);
			this.mapContainer.Name = "mapContainer";
			this.mapContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// mapContainer.Panel1
			// 
			this.mapContainer.Panel1.Controls.Add(this.mapBox);
			// 
			// mapContainer.Panel2
			// 
			this.mapContainer.Panel2.Controls.Add(this.mapAddButton);
			this.mapContainer.Size = new System.Drawing.Size(348, 326);
			this.mapContainer.SplitterDistance = 297;
			this.mapContainer.TabIndex = 2;
			// 
			// mapBox
			// 
			this.mapBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapBox.FormattingEnabled = true;
			this.mapBox.Location = new System.Drawing.Point(0, 0);
			this.mapBox.Name = "mapBox";
			this.mapBox.Size = new System.Drawing.Size(348, 297);
			this.mapBox.TabIndex = 0;
			// 
			// mapAddButton
			// 
			this.mapAddButton.Location = new System.Drawing.Point(268, 3);
			this.mapAddButton.Name = "mapAddButton";
			this.mapAddButton.Size = new System.Drawing.Size(75, 23);
			this.mapAddButton.TabIndex = 0;
			this.mapAddButton.Text = "Add";
			this.mapAddButton.UseVisualStyleBackColor = true;
			// 
			// processTab
			// 
			this.processTab.Controls.Add(this.label4);
			this.processTab.Controls.Add(this.portedVpksList);
			this.processTab.Controls.Add(this.label3);
			this.processTab.Controls.Add(this.packageModeDropdown);
			this.processTab.Controls.Add(this.processButton);
			this.processTab.Controls.Add(this.gameDirBox);
			this.processTab.Controls.Add(this.gameDirButton);
			this.processTab.Controls.Add(this.label2);
			this.processTab.Controls.Add(this.gameFgdBox);
			this.processTab.Controls.Add(this.gameFgdButton);
			this.processTab.Controls.Add(this.label1);
			this.processTab.Location = new System.Drawing.Point(4, 22);
			this.processTab.Name = "processTab";
			this.processTab.Padding = new System.Windows.Forms.Padding(3);
			this.processTab.Size = new System.Drawing.Size(354, 332);
			this.processTab.TabIndex = 3;
			this.processTab.Text = "Process";
			this.processTab.UseVisualStyleBackColor = true;
			// 
			// processButton
			// 
			this.processButton.Enabled = false;
			this.processButton.Location = new System.Drawing.Point(271, 301);
			this.processButton.Name = "processButton";
			this.processButton.Size = new System.Drawing.Size(75, 23);
			this.processButton.TabIndex = 7;
			this.processButton.Text = "Process";
			this.processButton.UseVisualStyleBackColor = true;
			this.processButton.Click += new System.EventHandler(this.processButton_Click);
			// 
			// gameDirBox
			// 
			this.gameDirBox.Location = new System.Drawing.Point(72, 32);
			this.gameDirBox.Name = "gameDirBox";
			this.gameDirBox.Size = new System.Drawing.Size(243, 20);
			this.gameDirBox.TabIndex = 5;
			// 
			// gameDirButton
			// 
			this.gameDirButton.Location = new System.Drawing.Point(321, 32);
			this.gameDirButton.Name = "gameDirButton";
			this.gameDirButton.Size = new System.Drawing.Size(25, 20);
			this.gameDirButton.TabIndex = 4;
			this.gameDirButton.Text = "...";
			this.gameDirButton.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Game Dir.";
			// 
			// gameFgdBox
			// 
			this.gameFgdBox.Location = new System.Drawing.Point(72, 6);
			this.gameFgdBox.Name = "gameFgdBox";
			this.gameFgdBox.Size = new System.Drawing.Size(243, 20);
			this.gameFgdBox.TabIndex = 2;
			// 
			// gameFgdButton
			// 
			this.gameFgdButton.Location = new System.Drawing.Point(321, 6);
			this.gameFgdButton.Name = "gameFgdButton";
			this.gameFgdButton.Size = new System.Drawing.Size(25, 20);
			this.gameFgdButton.TabIndex = 1;
			this.gameFgdButton.Text = "...";
			this.gameFgdButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Game FGD";
			// 
			// packageModeDropdown
			// 
			this.packageModeDropdown.FormattingEnabled = true;
			this.packageModeDropdown.Items.AddRange(new object[] {
            "None",
            "Folder Per Map",
            "Combined Folder",
            "Combined Addon"});
			this.packageModeDropdown.Location = new System.Drawing.Point(92, 58);
			this.packageModeDropdown.Name = "packageModeDropdown";
			this.packageModeDropdown.Size = new System.Drawing.Size(254, 21);
			this.packageModeDropdown.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Package Mode";
			// 
			// portedVpksList
			// 
			this.portedVpksList.FormattingEnabled = true;
			this.portedVpksList.Location = new System.Drawing.Point(92, 85);
			this.portedVpksList.Name = "portedVpksList";
			this.portedVpksList.Size = new System.Drawing.Size(254, 94);
			this.portedVpksList.TabIndex = 10;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 85);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(66, 26);
			this.label4.TabIndex = 11;
			this.label4.Text = "VPKs to port\r\ncontent from";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(362, 358);
			this.Controls.Add(this.tabControl);
			this.Name = "MainForm";
			this.Text = "Source Map Analyzer";
			this.vpkTabPage.ResumeLayout(false);
			this.vpkContainer.Panel1.ResumeLayout(false);
			this.vpkContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.vpkContainer)).EndInit();
			this.vpkContainer.ResumeLayout(false);
			this.baseTabPage.ResumeLayout(false);
			this.baseFgdContainer.Panel1.ResumeLayout(false);
			this.baseFgdContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.baseFgdContainer)).EndInit();
			this.baseFgdContainer.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.mapTabPage.ResumeLayout(false);
			this.mapContainer.Panel1.ResumeLayout(false);
			this.mapContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mapContainer)).EndInit();
			this.mapContainer.ResumeLayout(false);
			this.processTab.ResumeLayout(false);
			this.processTab.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabPage vpkTabPage;
		private System.Windows.Forms.TabPage baseTabPage;
		private System.Windows.Forms.SplitContainer baseFgdContainer;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.Button baseFgdAddButton;
		private System.Windows.Forms.ListBox baseFgdListBox;
		private System.Windows.Forms.SplitContainer vpkContainer;
		private System.Windows.Forms.ListBox vpkListBox;
		private System.Windows.Forms.Button vpkAddButton;
		private System.Windows.Forms.TabPage mapTabPage;
		private System.Windows.Forms.SplitContainer mapContainer;
		private System.Windows.Forms.ListBox mapBox;
		private System.Windows.Forms.Button mapAddButton;
		private System.Windows.Forms.TabPage processTab;
		private System.Windows.Forms.TextBox gameFgdBox;
		private System.Windows.Forms.Button gameFgdButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox gameDirBox;
		private System.Windows.Forms.Button gameDirButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button processButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox packageModeDropdown;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckedListBox portedVpksList;
	}
}