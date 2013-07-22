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
      this.entrySettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.addKeyAtOpenCheckBox = new System.Windows.Forms.CheckBox();
      this.removeKeyAtCloseCheckBox = new System.Windows.Forms.CheckBox();
      this.keyLocationPanel = new KeeAgent.UI.KeyLocationPanel();
      this.helpButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // hasSshKeyCheckBox
      // 
      resources.ApplyResources(this.hasSshKeyCheckBox, "hasSshKeyCheckBox");
      this.hasSshKeyCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "AllowUseOfSshKey", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.hasSshKeyCheckBox.Name = "hasSshKeyCheckBox";
      this.hasSshKeyCheckBox.UseVisualStyleBackColor = true;
      this.hasSshKeyCheckBox.CheckedChanged += new System.EventHandler(this.hasSshKeyCheckBox_CheckedChanged);
      // 
      // entrySettingsBindingSource
      // 
      this.entrySettingsBindingSource.DataSource = typeof(KeeAgent.EntrySettings);
      // 
      // addKeyAtOpenCheckBox
      // 
      resources.ApplyResources(this.addKeyAtOpenCheckBox, "addKeyAtOpenCheckBox");
      this.addKeyAtOpenCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "AddAtDatabaseOpen", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.addKeyAtOpenCheckBox.Name = "addKeyAtOpenCheckBox";
      this.addKeyAtOpenCheckBox.UseVisualStyleBackColor = true;
      // 
      // removeKeyAtCloseCheckBox
      // 
      resources.ApplyResources(this.removeKeyAtCloseCheckBox, "removeKeyAtCloseCheckBox");
      this.removeKeyAtCloseCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "RemoveAtDatabaseClose", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.removeKeyAtCloseCheckBox.Name = "removeKeyAtCloseCheckBox";
      this.removeKeyAtCloseCheckBox.UseVisualStyleBackColor = true;
      // 
      // keyLocationPanel
      // 
      this.keyLocationPanel.BackColor = System.Drawing.Color.Transparent;
      this.keyLocationPanel.DataBindings.Add(new System.Windows.Forms.Binding("KeyLocation", this.entrySettingsBindingSource, "Location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.keyLocationPanel.KeyLocation = null;
      resources.ApplyResources(this.keyLocationPanel, "keyLocationPanel");
      this.keyLocationPanel.Name = "keyLocationPanel";
      // 
      // helpButton
      // 
      resources.ApplyResources(this.helpButton, "helpButton");
      this.helpButton.Image = global::KeeAgent.Properties.Resources.Help_png;
      this.helpButton.Name = "helpButton";
      this.helpButton.UseVisualStyleBackColor = true;
      this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
      // 
      // EntryPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.helpButton);
      this.Controls.Add(this.keyLocationPanel);
      this.Controls.Add(this.removeKeyAtCloseCheckBox);
      this.Controls.Add(this.addKeyAtOpenCheckBox);
      this.Controls.Add(this.hasSshKeyCheckBox);
      this.Name = "EntryPanel";
      ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox hasSshKeyCheckBox;
        private System.Windows.Forms.CheckBox addKeyAtOpenCheckBox;
        private System.Windows.Forms.CheckBox removeKeyAtCloseCheckBox;
        internal System.Windows.Forms.BindingSource entrySettingsBindingSource;
        private KeyLocationPanel keyLocationPanel;
        private System.Windows.Forms.Button helpButton;

    }
}