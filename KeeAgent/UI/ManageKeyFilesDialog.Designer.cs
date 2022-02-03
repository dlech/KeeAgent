namespace KeeAgent.UI
{
  partial class ManageKeyFilesDialog
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.publicKeyLocationPanel = new KeeAgent.UI.KeyLocationPanel();
            this.privateKeyLocationPanel = new KeeAgent.UI.KeyLocationPanel();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(456, 265);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(377, 265);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // publicKeyLocationPanel
            // 
            this.publicKeyLocationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.publicKeyLocationPanel.Attachments = null;
            this.publicKeyLocationPanel.BackColor = System.Drawing.Color.Transparent;
            this.publicKeyLocationPanel.ErrorMessage = "<key file error message>";
            this.publicKeyLocationPanel.KeyLocation = null;
            this.publicKeyLocationPanel.Location = new System.Drawing.Point(8, 8);
            this.publicKeyLocationPanel.Margin = new System.Windows.Forms.Padding(6);
            this.publicKeyLocationPanel.Name = "publicKeyLocationPanel";
            this.publicKeyLocationPanel.Size = new System.Drawing.Size(527, 118);
            this.publicKeyLocationPanel.TabIndex = 1;
            this.publicKeyLocationPanel.Title = "Public Key File Location";
            // 
            // privateKeyLocationPanel
            // 
            this.privateKeyLocationPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.privateKeyLocationPanel.Attachments = null;
            this.privateKeyLocationPanel.BackColor = System.Drawing.Color.Transparent;
            this.privateKeyLocationPanel.ErrorMessage = "<key file error message>";
            this.privateKeyLocationPanel.KeyLocation = null;
            this.privateKeyLocationPanel.Location = new System.Drawing.Point(8, 138);
            this.privateKeyLocationPanel.Margin = new System.Windows.Forms.Padding(6);
            this.privateKeyLocationPanel.Name = "privateKeyLocationPanel";
            this.privateKeyLocationPanel.Size = new System.Drawing.Size(527, 118);
            this.privateKeyLocationPanel.TabIndex = 0;
            this.privateKeyLocationPanel.Title = "Private Key File Location";
            // 
            // ManageKeyFilesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(542, 299);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.publicKeyLocationPanel);
            this.Controls.Add(this.privateKeyLocationPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageKeyFilesDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Key Files";
            this.ResumeLayout(false);

    }

    #endregion

    private KeyLocationPanel privateKeyLocationPanel;
    private KeyLocationPanel publicKeyLocationPanel;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button okButton;
  }
}
