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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageDialog));
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.addButtonMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.addButtonFromKeePassMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.addButtonFromFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.keyInfoView = new dlech.SshAgentLib.WinForms.KeyInfoView();
      this.addButtonMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Value";
      resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      // 
      // addButtonMenuStrip
      // 
      this.addButtonMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addButtonFromKeePassMenuItem,
            this.addButtonFromFileMenuItem});
      this.addButtonMenuStrip.Name = "addButtonMenuStrip";
      this.addButtonMenuStrip.ShowImageMargin = false;
      resources.ApplyResources(this.addButtonMenuStrip, "addButtonMenuStrip");
      // 
      // addButtonFromKeePassMenuItem
      // 
      this.addButtonFromKeePassMenuItem.Name = "addButtonFromKeePassMenuItem";
      resources.ApplyResources(this.addButtonFromKeePassMenuItem, "addButtonFromKeePassMenuItem");
      this.addButtonFromKeePassMenuItem.Click += new System.EventHandler(this.addButtonFromKeePassMenuItem_Click);
      // 
      // addButtonFromFileMenuItem
      // 
      this.addButtonFromFileMenuItem.Name = "addButtonFromFileMenuItem";
      resources.ApplyResources(this.addButtonFromFileMenuItem, "addButtonFromFileMenuItem");
      this.addButtonFromFileMenuItem.Click += new System.EventHandler(this.addButtonFromFileMenuItem_Click);
      // 
      // keyInfoView
      // 
      this.keyInfoView.AddButtonSplitMenu = this.addButtonMenuStrip;
      resources.ApplyResources(this.keyInfoView, "keyInfoView");
      this.keyInfoView.Name = "keyInfoView";
      this.keyInfoView.AddFromFileHelpRequested += new System.EventHandler(this.keyInfoView_AddFromFileHelpRequested);
      // 
      // ManageDialog
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.keyInfoView);
      this.HelpButton = true;
      this.KeyPreview = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ManageDialog";
      this.ShowInTaskbar = false;
      this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ManageDialog_HelpButtonClicked);
      this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ManageDialog_HelpRequested);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ManageDialog_KeyDown);
      this.addButtonMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private dlech.SshAgentLib.WinForms.KeyInfoView keyInfoView;
    private System.Windows.Forms.ContextMenuStrip addButtonMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem addButtonFromKeePassMenuItem;
    private System.Windows.Forms.ToolStripMenuItem addButtonFromFileMenuItem;
  }
}