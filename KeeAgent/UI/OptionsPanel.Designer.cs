namespace KeeAgent.UI
{
    partial class OptionsPanel
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
            if (disposing && (components != null)) {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsPanel));
      this.customListViewEx = new KeePass.UI.CustomListViewEx();
      this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.modeComboBox = new System.Windows.Forms.ComboBox();
      this.modeLabel = new System.Windows.Forms.Label();
      this.helpButton = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.msysPathBrowseButton = new System.Windows.Forms.Button();
      this.cygwinPathBrowseButton = new System.Windows.Forms.Button();
      this.msysSocketPathTextBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.cygwinSocketPathTextBox = new System.Windows.Forms.TextBox();
      this.useMsysSocketCheckBox = new System.Windows.Forms.CheckBox();
      this.label3 = new System.Windows.Forms.Label();
      this.useCygwinSocketCheckBox = new System.Windows.Forms.CheckBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // customListViewEx
      // 
      resources.ApplyResources(this.customListViewEx, "customListViewEx");
      this.customListViewEx.CheckBoxes = true;
      this.customListViewEx.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
      this.customListViewEx.FullRowSelect = true;
      this.customListViewEx.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.customListViewEx.Name = "customListViewEx";
      this.customListViewEx.ShowItemToolTips = true;
      this.customListViewEx.UseCompatibleStateImageBehavior = false;
      this.customListViewEx.View = System.Windows.Forms.View.Details;
      // 
      // columnHeader
      // 
      resources.ApplyResources(this.columnHeader, "columnHeader");
      // 
      // modeComboBox
      // 
      this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.modeComboBox.FormattingEnabled = true;
      resources.ApplyResources(this.modeComboBox, "modeComboBox");
      this.modeComboBox.Name = "modeComboBox";
      // 
      // modeLabel
      // 
      resources.ApplyResources(this.modeLabel, "modeLabel");
      this.modeLabel.Name = "modeLabel";
      // 
      // helpButton
      // 
      resources.ApplyResources(this.helpButton, "helpButton");
      this.helpButton.Image = global::KeeAgent.Properties.Resources.Help_png;
      this.helpButton.Name = "helpButton";
      this.helpButton.UseVisualStyleBackColor = true;
      this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
      // 
      // groupBox1
      // 
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.Controls.Add(this.msysPathBrowseButton);
      this.groupBox1.Controls.Add(this.cygwinPathBrowseButton);
      this.groupBox1.Controls.Add(this.msysSocketPathTextBox);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.cygwinSocketPathTextBox);
      this.groupBox1.Controls.Add(this.useMsysSocketCheckBox);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.useCygwinSocketCheckBox);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      // 
      // msysPathBrowseButton
      // 
      resources.ApplyResources(this.msysPathBrowseButton, "msysPathBrowseButton");
      this.msysPathBrowseButton.Name = "msysPathBrowseButton";
      this.msysPathBrowseButton.UseVisualStyleBackColor = true;
      this.msysPathBrowseButton.Click += new System.EventHandler(this.msysPathBrowseButton_Click);
      // 
      // cygwinPathBrowseButton
      // 
      resources.ApplyResources(this.cygwinPathBrowseButton, "cygwinPathBrowseButton");
      this.cygwinPathBrowseButton.Name = "cygwinPathBrowseButton";
      this.cygwinPathBrowseButton.UseVisualStyleBackColor = true;
      this.cygwinPathBrowseButton.Click += new System.EventHandler(this.cygwinPathBrowseButton_Click);
      // 
      // msysSocketPathTextBox
      // 
      resources.ApplyResources(this.msysSocketPathTextBox, "msysSocketPathTextBox");
      this.msysSocketPathTextBox.Name = "msysSocketPathTextBox";
      // 
      // label4
      // 
      resources.ApplyResources(this.label4, "label4");
      this.label4.Name = "label4";
      // 
      // cygwinSocketPathTextBox
      // 
      resources.ApplyResources(this.cygwinSocketPathTextBox, "cygwinSocketPathTextBox");
      this.cygwinSocketPathTextBox.Name = "cygwinSocketPathTextBox";
      // 
      // useMsysSocketCheckBox
      // 
      resources.ApplyResources(this.useMsysSocketCheckBox, "useMsysSocketCheckBox");
      this.useMsysSocketCheckBox.Name = "useMsysSocketCheckBox";
      this.useMsysSocketCheckBox.UseVisualStyleBackColor = true;
      this.useMsysSocketCheckBox.CheckedChanged += new System.EventHandler(this.useMsysSocketCheckBox_CheckedChanged);
      // 
      // label3
      // 
      resources.ApplyResources(this.label3, "label3");
      this.label3.Name = "label3";
      // 
      // useCygwinSocketCheckBox
      // 
      resources.ApplyResources(this.useCygwinSocketCheckBox, "useCygwinSocketCheckBox");
      this.useCygwinSocketCheckBox.Name = "useCygwinSocketCheckBox";
      this.useCygwinSocketCheckBox.UseVisualStyleBackColor = true;
      this.useCygwinSocketCheckBox.CheckedChanged += new System.EventHandler(this.useCygwinSocketCheckBox_CheckedChanged);
      // 
      // label1
      // 
      resources.ApplyResources(this.label1, "label1");
      this.label1.Name = "label1";
      // 
      // OptionsPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.helpButton);
      this.Controls.Add(this.modeLabel);
      this.Controls.Add(this.modeComboBox);
      this.Controls.Add(this.customListViewEx);
      this.Name = "OptionsPanel";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private KeePass.UI.CustomListViewEx customListViewEx;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox msysSocketPathTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox cygwinSocketPathTextBox;
        private System.Windows.Forms.CheckBox useMsysSocketCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox useCygwinSocketCheckBox;
        private System.Windows.Forms.Button msysPathBrowseButton;
        private System.Windows.Forms.Button cygwinPathBrowseButton;
    }
}