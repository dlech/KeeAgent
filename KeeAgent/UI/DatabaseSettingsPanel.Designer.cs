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
      this.mUnlockOnAgentActivityCheckBox = new System.Windows.Forms.CheckBox();
      this.mDatabaseSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.mOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.mAgentModeGroupBox = new KeeAgent.UI.GroupBoxEx();
      ((System.ComponentModel.ISupportInitialize)(this.mDatabaseSettingsBindingSource)).BeginInit();
      this.mAgentModeGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // mUnlockOnAgentActivityCheckBox
      // 
      resources.ApplyResources(this.mUnlockOnAgentActivityCheckBox, "mUnlockOnAgentActivityCheckBox");
      this.mUnlockOnAgentActivityCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.mDatabaseSettingsBindingSource, "UnlockOnActivity", true));
      this.mUnlockOnAgentActivityCheckBox.Name = "mUnlockOnAgentActivityCheckBox";
      this.mUnlockOnAgentActivityCheckBox.UseVisualStyleBackColor = true;
      // 
      // mDatabaseSettingsBindingSource
      // 
      this.mDatabaseSettingsBindingSource.DataSource = typeof(KeeAgent.DatabaseSettings);
      // 
      // mAgentModeGroupBox
      // 
      resources.ApplyResources(this.mAgentModeGroupBox, "mAgentModeGroupBox");
      this.mAgentModeGroupBox.Controls.Add(this.mUnlockOnAgentActivityCheckBox);
      this.mAgentModeGroupBox.Name = "mAgentModeGroupBox";
      this.mAgentModeGroupBox.SelectedRadioButton = "";
      this.mAgentModeGroupBox.TabStop = false;
      // 
      // DatabaseSettingsPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.mAgentModeGroupBox);
      this.Name = "DatabaseSettingsPanel";
      ((System.ComponentModel.ISupportInitialize)(this.mDatabaseSettingsBindingSource)).EndInit();
      this.mAgentModeGroupBox.ResumeLayout(false);
      this.mAgentModeGroupBox.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox mUnlockOnAgentActivityCheckBox;
        private System.Windows.Forms.BindingSource mDatabaseSettingsBindingSource;
        private System.Windows.Forms.OpenFileDialog mOpenFileDialog;
        private GroupBoxEx mAgentModeGroupBox;

    }
}