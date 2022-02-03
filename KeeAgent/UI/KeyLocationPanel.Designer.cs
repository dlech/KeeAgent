namespace KeeAgent.UI
{
  partial class KeyLocationPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyLocationPanel));
            this.locationSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.locationGroupBox = new KeeAgent.UI.GroupBoxEx();
            this.errorMessage = new KeeAgent.UI.InPlaceMessage();
            this.saveKeyToTempFileCheckBox = new System.Windows.Forms.CheckBox();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.attachmentComboBox = new System.Windows.Forms.ComboBox();
            this.fileRadioButton = new System.Windows.Forms.RadioButton();
            this.attachmentRadioButton = new System.Windows.Forms.RadioButton();
            this.attachButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.locationSettingsBindingSource)).BeginInit();
            this.locationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // locationSettingsBindingSource
            // 
            this.locationSettingsBindingSource.DataSource = typeof(KeeAgent.EntrySettings.LocationData);
            this.locationSettingsBindingSource.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.locationSettingsBindingSource_BindingComplete);
            // 
            // locationGroupBox
            // 
            this.locationGroupBox.Controls.Add(this.attachButton);
            this.locationGroupBox.Controls.Add(this.errorMessage);
            this.locationGroupBox.Controls.Add(this.saveKeyToTempFileCheckBox);
            this.locationGroupBox.Controls.Add(this.fileNameTextBox);
            this.locationGroupBox.Controls.Add(this.browseButton);
            this.locationGroupBox.Controls.Add(this.attachmentComboBox);
            this.locationGroupBox.Controls.Add(this.fileRadioButton);
            this.locationGroupBox.Controls.Add(this.attachmentRadioButton);
            this.locationGroupBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedRadioButton", this.locationSettingsBindingSource, "SelectedType", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.locationGroupBox, "locationGroupBox");
            this.locationGroupBox.Name = "locationGroupBox";
            this.locationGroupBox.SelectedRadioButton = "";
            this.locationGroupBox.TabStop = false;
            // 
            // errorMessage
            // 
            this.errorMessage.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.errorMessage, "errorMessage");
            this.errorMessage.Name = "errorMessage";
            this.errorMessage.StockIcon = KeeAgent.UI.SystemIcon.StockIconId.Warning;
            // 
            // saveKeyToTempFileCheckBox
            // 
            resources.ApplyResources(this.saveKeyToTempFileCheckBox, "saveKeyToTempFileCheckBox");
            this.saveKeyToTempFileCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.locationSettingsBindingSource, "SaveAttachmentToTempFile", true));
            this.saveKeyToTempFileCheckBox.Name = "saveKeyToTempFileCheckBox";
            this.saveKeyToTempFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileNameTextBox
            // 
            resources.ApplyResources(this.fileNameTextBox, "fileNameTextBox");
            this.fileNameTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.locationSettingsBindingSource, "FileName", true));
            this.fileNameTextBox.Name = "fileNameTextBox";
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
            this.attachmentComboBox.DataBindings.Add(new System.Windows.Forms.Binding("SelectedValue", this.locationSettingsBindingSource, "AttachmentName", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.attachmentComboBox.DisplayMember = "Key";
            this.attachmentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.attachmentComboBox.FormattingEnabled = true;
            this.attachmentComboBox.Name = "attachmentComboBox";
            this.attachmentComboBox.ValueMember = "Key";
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
            // attachButton
            // 
            resources.ApplyResources(this.attachButton, "attachButton");
            this.attachButton.Name = "attachButton";
            this.attachButton.UseVisualStyleBackColor = true;
            this.attachButton.Click += new System.EventHandler(this.attachButton_Click);
            // 
            // KeyLocationPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.locationGroupBox);
            this.Name = "KeyLocationPanel";
            ((System.ComponentModel.ISupportInitialize)(this.locationSettingsBindingSource)).EndInit();
            this.locationGroupBox.ResumeLayout(false);
            this.locationGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton attachmentRadioButton;
        private KeeAgent.UI.GroupBoxEx locationGroupBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.ComboBox attachmentComboBox;
        private System.Windows.Forms.RadioButton fileRadioButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        internal System.Windows.Forms.BindingSource locationSettingsBindingSource;
        private System.Windows.Forms.CheckBox saveKeyToTempFileCheckBox;
    private InPlaceMessage errorMessage;
    private System.Windows.Forms.Button attachButton;
  }
}
