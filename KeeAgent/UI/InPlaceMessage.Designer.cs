namespace KeeAgent.UI
{
  partial class InPlaceMessage
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InPlaceMessage));
            this.systemIcon1 = new KeeAgent.UI.SystemIcon();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.systemIcon1)).BeginInit();
            this.SuspendLayout();
            // 
            // systemIcon1
            // 
            this.systemIcon1.BackColor = System.Drawing.Color.Transparent;
            this.systemIcon1.Image = ((System.Drawing.Image)(resources.GetObject("systemIcon1.Image")));
            this.systemIcon1.Location = new System.Drawing.Point(3, 3);
            this.systemIcon1.Name = "systemIcon1";
            this.systemIcon1.Size = new System.Drawing.Size(16, 16);
            this.systemIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.systemIcon1.TabIndex = 0;
            this.systemIcon1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "<message>";
            // 
            // InPlaceMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.systemIcon1);
            this.Name = "InPlaceMessage";
            this.Size = new System.Drawing.Size(406, 26);
            ((System.ComponentModel.ISupportInitialize)(this.systemIcon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private SystemIcon systemIcon1;
    private System.Windows.Forms.Label label1;
  }
}
