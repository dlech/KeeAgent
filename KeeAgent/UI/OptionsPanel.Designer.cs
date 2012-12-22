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
      this.customListViewEx1 = new KeePass.UI.CustomListViewEx();
      this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.SuspendLayout();
      // 
      // customListViewEx1
      // 
      resources.ApplyResources(this.customListViewEx1, "customListViewEx1");
      this.customListViewEx1.CheckBoxes = true;
      this.customListViewEx1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
      this.customListViewEx1.FullRowSelect = true;
      this.customListViewEx1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("customListViewEx1.Groups")))});
      this.customListViewEx1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.customListViewEx1.Name = "customListViewEx1";
      this.customListViewEx1.ShowItemToolTips = true;
      this.customListViewEx1.UseCompatibleStateImageBehavior = false;
      this.customListViewEx1.View = System.Windows.Forms.View.Details;
      // 
      // columnHeader1
      // 
      resources.ApplyResources(this.columnHeader1, "columnHeader1");
      // 
      // OptionsPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.customListViewEx1);
      this.Name = "OptionsPanel";
      this.ResumeLayout(false);

        }

        #endregion

        private KeePass.UI.CustomListViewEx customListViewEx1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}