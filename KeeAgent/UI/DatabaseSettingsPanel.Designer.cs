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
      this.unlockOnAgentActivityCheckBox = new System.Windows.Forms.CheckBox();
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.databaseSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.databaseSettingsBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // unlockOnAgentActivityCheckBox
      // 
      resources.ApplyResources(this.unlockOnAgentActivityCheckBox, "unlockOnAgentActivityCheckBox");
      this.unlockOnAgentActivityCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.databaseSettingsBindingSource, "UnlockOnActivity", true));
      this.unlockOnAgentActivityCheckBox.Name = "unlockOnAgentActivityCheckBox";
      this.unlockOnAgentActivityCheckBox.UseVisualStyleBackColor = true;
      // 
      // databaseSettingsBindingSource
      // 
      this.databaseSettingsBindingSource.DataSource = typeof(KeeAgent.DatabaseSettings);
      // 
      // DatabaseSettingsPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.unlockOnAgentActivityCheckBox);
      this.Name = "DatabaseSettingsPanel";
      ((System.ComponentModel.ISupportInitialize)(this.databaseSettingsBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox unlockOnAgentActivityCheckBox;
        private System.Windows.Forms.BindingSource databaseSettingsBindingSource;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

    }
}