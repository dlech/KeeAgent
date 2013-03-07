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
      // OptionsPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.modeLabel);
      this.Controls.Add(this.modeComboBox);
      this.Controls.Add(this.customListViewEx);
      this.Name = "OptionsPanel";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private KeePass.UI.CustomListViewEx customListViewEx;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.Label modeLabel;
    }
}