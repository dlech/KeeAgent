namespace KeeAgent.UI
{
	partial class ManageDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageDialog));
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.keyInfoView1 = new dlech.SshAgentLib.WinForms.KeyInfoView();
      this.SuspendLayout();
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Value";
      resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      // 
      // keyInfoView1
      // 
      resources.ApplyResources(this.keyInfoView1, "keyInfoView1");
      this.keyInfoView1.Name = "keyInfoView1";
      // 
      // ManageDialog
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.keyInfoView1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ManageDialog";
      this.ShowInTaskbar = false;
      this.Load += new System.EventHandler(this.ManageDialog_Load);
      this.ResumeLayout(false);

		}

		#endregion

    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn algorithmDataGridViewTextBoxColumn;
        private dlech.SshAgentLib.WinForms.KeyInfoView keyInfoView1;
	}
}