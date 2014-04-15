namespace KeeAgent.UI
{
    partial class DatabaseSettingsPanel
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabaseSettingsPanel));
      this.mHelpButton = new System.Windows.Forms.Button();
      this.allowAutoLoadExpiredCheckBox = new System.Windows.Forms.CheckBox();
      this.mDatabaseSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.mDatabaseSettingsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // mHelpButton
      // 
      resources.ApplyResources(this.mHelpButton, "mHelpButton");
      this.mHelpButton.Image = global::KeeAgent.Properties.Resources.Help_png;
      this.mHelpButton.Name = "mHelpButton";
      this.mHelpButton.UseVisualStyleBackColor = true;
      this.mHelpButton.Click += new System.EventHandler(this.mHelpButton_Click);
      // 
      // allowAutoLoadExpiredCheckBox
      // 
      resources.ApplyResources(this.allowAutoLoadExpiredCheckBox, "allowAutoLoadExpiredCheckBox");
      this.allowAutoLoadExpiredCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mDatabaseSettingsBindingSource, "AllowAutoLoadExpiredEntryKey", true));
      this.allowAutoLoadExpiredCheckBox.Name = "allowAutoLoadExpiredCheckBox";
      this.allowAutoLoadExpiredCheckBox.UseVisualStyleBackColor = true;
      // 
      // mDatabaseSettingsBindingSource
      // 
      this.mDatabaseSettingsBindingSource.DataSource = typeof(KeeAgent.DatabaseSettings);
      // 
      // DatabaseSettingsPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.allowAutoLoadExpiredCheckBox);
      this.Controls.Add(this.mHelpButton);
      this.Name = "DatabaseSettingsPanel";
      ((System.ComponentModel.ISupportInitialize)(this.mDatabaseSettingsBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource mDatabaseSettingsBindingSource;
        private System.Windows.Forms.Button mHelpButton;
        private System.Windows.Forms.CheckBox allowAutoLoadExpiredCheckBox;

    }
}