namespace KeeAgent.UI
{
    partial class EntryPanel
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntryPanel));
      this.hasSshKeyCheckBox = new System.Windows.Forms.CheckBox();
      this.loadAtStartupCheckBox = new System.Windows.Forms.CheckBox();
      this.locationGroupBox = new KeeAgent.UI.GroupBoxEx();
      this.fileNameTextBox = new System.Windows.Forms.TextBox();
      this.entrySettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.browseButton = new System.Windows.Forms.Button();
      this.attachmentComboBox = new System.Windows.Forms.ComboBox();
      this.fileRadioButton = new System.Windows.Forms.RadioButton();
      this.attachmentRadioButton = new System.Windows.Forms.RadioButton();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.locationGroupBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // hasSshKeyCheckBox
      // 
      resources.ApplyResources(this.hasSshKeyCheckBox, "hasSshKeyCheckBox");
      this.hasSshKeyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "HasSshKey", true));
      this.hasSshKeyCheckBox.Name = "hasSshKeyCheckBox";
      this.hasSshKeyCheckBox.UseVisualStyleBackColor = true;
      this.hasSshKeyCheckBox.CheckedChanged += new System.EventHandler(this.hasSshKeyCheckBox_CheckedChanged);
      // 
      // loadAtStartupCheckBox
      // 
      resources.ApplyResources(this.loadAtStartupCheckBox, "loadAtStartupCheckBox");
      this.loadAtStartupCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "LoadAtStartup", true));
      this.loadAtStartupCheckBox.Name = "loadAtStartupCheckBox";
      this.loadAtStartupCheckBox.UseVisualStyleBackColor = true;
      // 
      // locationGroupBox
      // 
      resources.ApplyResources(this.locationGroupBox, "locationGroupBox");
      this.locationGroupBox.Controls.Add(this.fileNameTextBox);
      this.locationGroupBox.Controls.Add(this.browseButton);
      this.locationGroupBox.Controls.Add(this.attachmentComboBox);
      this.locationGroupBox.Controls.Add(this.fileRadioButton);
      this.locationGroupBox.Controls.Add(this.attachmentRadioButton);
      this.locationGroupBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedRadioButton", this.entrySettingsBindingSource, "Location.SelectedType", true));
      this.locationGroupBox.Name = "locationGroupBox";
      this.locationGroupBox.SelectedRadioButton = "";
      this.locationGroupBox.TabStop = false;
      // 
      // fileNameTextBox
      // 
      resources.ApplyResources(this.fileNameTextBox, "fileNameTextBox");
      this.fileNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.entrySettingsBindingSource, "Location.FileName", true));
      this.fileNameTextBox.Name = "fileNameTextBox";
      // 
      // entrySettingsBindingSource
      // 
      this.entrySettingsBindingSource.DataSource = typeof(KeeAgent.EntrySettings);
      // 
      // browseButton
      // 
      resources.ApplyResources(this.browseButton, "browseButton");
      this.browseButton.Name = "browseButton";
      this.browseButton.UseVisualStyleBackColor = true;
      this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
      // 
      // attachmentComboBox
      // 
      resources.ApplyResources(this.attachmentComboBox, "attachmentComboBox");
      this.attachmentComboBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.entrySettingsBindingSource, "Location.AttachmentName", true));
      this.attachmentComboBox.FormattingEnabled = true;
      this.attachmentComboBox.Name = "attachmentComboBox";
      this.attachmentComboBox.VisibleChanged += new System.EventHandler(this.attachmentComboBox_VisibleChanged);
      // 
      // fileRadioButton
      // 
      resources.ApplyResources(this.fileRadioButton, "fileRadioButton");
      this.fileRadioButton.Name = "fileRadioButton";
      this.fileRadioButton.TabStop = true;
      this.fileRadioButton.UseVisualStyleBackColor = true;
      this.fileRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // attachmentRadioButton
      // 
      resources.ApplyResources(this.attachmentRadioButton, "attachmentRadioButton");
      this.attachmentRadioButton.Name = "attachmentRadioButton";
      this.attachmentRadioButton.TabStop = true;
      this.attachmentRadioButton.UseVisualStyleBackColor = true;
      this.attachmentRadioButton.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
      // 
      // EntryPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.locationGroupBox);
      this.Controls.Add(this.loadAtStartupCheckBox);
      this.Controls.Add(this.hasSshKeyCheckBox);
      this.Name = "EntryPanel";
      this.locationGroupBox.ResumeLayout(false);
      this.locationGroupBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox hasSshKeyCheckBox;
        private System.Windows.Forms.CheckBox loadAtStartupCheckBox;
        private System.Windows.Forms.BindingSource entrySettingsBindingSource;
        private System.Windows.Forms.RadioButton attachmentRadioButton;
        private KeeAgent.UI.GroupBoxEx locationGroupBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.ComboBox attachmentComboBox;
        private System.Windows.Forms.RadioButton fileRadioButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

    }
}