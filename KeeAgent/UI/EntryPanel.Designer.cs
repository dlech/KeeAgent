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
      this.helpButton = new System.Windows.Forms.Button();
      this.keyInfoGroupBox = new System.Windows.Forms.GroupBox();
      this.copyPublicKeybutton = new System.Windows.Forms.Button();
      this.publicKeyTextBox = new System.Windows.Forms.TextBox();
      this.commentTextBox = new System.Windows.Forms.TextBox();
      this.fingerprintTextBox = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.keyLocationPanel = new KeeAgent.UI.KeyLocationPanel();
      ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).BeginInit();
      this.keyInfoGroupBox.SuspendLayout();
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
      // helpButton
      // 
      resources.ApplyResources(this.helpButton, "helpButton");
      this.helpButton.Image = global::KeeAgent.Properties.Resources.Help_png;
      this.helpButton.Name = "helpButton";
      this.helpButton.UseVisualStyleBackColor = true;
      this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
      // 
      // keyInfoGroupBox
      // 
      resources.ApplyResources(this.keyInfoGroupBox, "keyInfoGroupBox");
      this.keyInfoGroupBox.Controls.Add(this.copyPublicKeybutton);
      this.keyInfoGroupBox.Controls.Add(this.publicKeyTextBox);
      this.keyInfoGroupBox.Controls.Add(this.commentTextBox);
      this.keyInfoGroupBox.Controls.Add(this.fingerprintTextBox);
      this.keyInfoGroupBox.Controls.Add(this.label3);
      this.keyInfoGroupBox.Controls.Add(this.label2);
      this.keyInfoGroupBox.Controls.Add(this.label1);
      this.keyInfoGroupBox.Name = "keyInfoGroupBox";
      this.keyInfoGroupBox.TabStop = false;
      // 
      // copyPublicKeybutton
      // 
      resources.ApplyResources(this.copyPublicKeybutton, "copyPublicKeybutton");
      this.copyPublicKeybutton.Name = "copyPublicKeybutton";
      this.copyPublicKeybutton.UseVisualStyleBackColor = true;
      this.copyPublicKeybutton.Click += new System.EventHandler(this.copyPublicKeybutton_Click);
      // 
      // publicKeyTextBox
      // 
      resources.ApplyResources(this.publicKeyTextBox, "publicKeyTextBox");
      this.publicKeyTextBox.Name = "publicKeyTextBox";
      this.publicKeyTextBox.ReadOnly = true;
      // 
      // commentTextBox
      // 
      resources.ApplyResources(this.commentTextBox, "commentTextBox");
      this.commentTextBox.Name = "commentTextBox";
      this.commentTextBox.ReadOnly = true;
      // 
      // fingerprintTextBox
      // 
      resources.ApplyResources(this.fingerprintTextBox, "fingerprintTextBox");
      this.fingerprintTextBox.Name = "fingerprintTextBox";
      this.fingerprintTextBox.ReadOnly = true;
      // 
      // label3
      // 
      resources.ApplyResources(this.label3, "label3");
      this.label3.Name = "label3";
      // 
      // label2
      // 
      resources.ApplyResources(this.label2, "label2");
      this.label2.Name = "label2";
      // 
      // label1
      // 
      resources.ApplyResources(this.label1, "label1");
      this.label1.Name = "label1";
      // 
      // keyLocationPanel
      // 
      this.keyLocationPanel.BackColor = System.Drawing.Color.Transparent;
      this.keyLocationPanel.DataBindings.Add(new System.Windows.Forms.Binding("KeyLocation", this.entrySettingsBindingSource, "Location", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.keyLocationPanel.KeyLocation = null;
      resources.ApplyResources(this.keyLocationPanel, "keyLocationPanel");
      this.keyLocationPanel.Name = "keyLocationPanel";
      // 
      // EntryPanel
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.keyInfoGroupBox);
      this.Controls.Add(this.helpButton);
      this.Controls.Add(this.keyLocationPanel);
      this.Controls.Add(this.removeKeyAtCloseCheckBox);
      this.Controls.Add(this.addKeyAtOpenCheckBox);
      this.Controls.Add(this.hasSshKeyCheckBox);
      this.Name = "EntryPanel";
      ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).EndInit();
      this.keyInfoGroupBox.ResumeLayout(false);
      this.keyInfoGroupBox.PerformLayout();
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
        private System.Windows.Forms.GroupBox keyInfoGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox publicKeyTextBox;
        private System.Windows.Forms.TextBox commentTextBox;
        private System.Windows.Forms.TextBox fingerprintTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button copyPublicKeybutton;

    }
}