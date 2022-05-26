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
            this.addKeyAtOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.removeKeyAtCloseCheckBox = new System.Windows.Forms.CheckBox();
            this.helpButton = new System.Windows.Forms.Button();
            this.keyInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.copyPublicKeyButton = new System.Windows.Forms.Button();
            this.publicKeyTextBox = new System.Windows.Forms.TextBox();
            this.commentTextBox = new System.Windows.Forms.TextBox();
            this.fingerprintTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.confirmConstraintCheckBox = new System.Windows.Forms.CheckBox();
            this.delayedUpdateKeyInfoTimer = new System.Windows.Forms.Timer(this.components);
            this.lifetimeConstraintCheckBox = new System.Windows.Forms.CheckBox();
            this.lifetimeConstraintNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lifetimeConstraintLabel = new System.Windows.Forms.Label();
            this.openManageFilesDialogButton = new System.Windows.Forms.Button();
            this.destinationConstraintCheckBox = new System.Windows.Forms.CheckBox();
            this.destinationConstraintDataGridView = new System.Windows.Forms.DataGridView();
            this.fromHostDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FromHostKeysColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.toUserDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toHostDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ToHostKeysColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.destinationConstraintContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteCurrentRowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destinationConstraintBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.entrySettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.invalidKeyWarningIcon = new KeeAgent.UI.SystemIcon();
            this.keyInfoGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lifetimeConstraintNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.destinationConstraintDataGridView)).BeginInit();
            this.destinationConstraintContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.destinationConstraintBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.invalidKeyWarningIcon)).BeginInit();
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
            this.keyInfoGroupBox.Controls.Add(this.copyPublicKeyButton);
            this.keyInfoGroupBox.Controls.Add(this.publicKeyTextBox);
            this.keyInfoGroupBox.Controls.Add(this.commentTextBox);
            this.keyInfoGroupBox.Controls.Add(this.fingerprintTextBox);
            this.keyInfoGroupBox.Controls.Add(this.label3);
            this.keyInfoGroupBox.Controls.Add(this.label2);
            this.keyInfoGroupBox.Controls.Add(this.label1);
            this.keyInfoGroupBox.Name = "keyInfoGroupBox";
            this.keyInfoGroupBox.TabStop = false;
            //
            // copyPublicKeyButton
            //
            resources.ApplyResources(this.copyPublicKeyButton, "copyPublicKeyButton");
            this.copyPublicKeyButton.Name = "copyPublicKeyButton";
            this.copyPublicKeyButton.UseVisualStyleBackColor = true;
            this.copyPublicKeyButton.Click += new System.EventHandler(this.copyPublicKeyButton_Click);
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
            // confirmConstraintCheckBox
            //
            resources.ApplyResources(this.confirmConstraintCheckBox, "confirmConstraintCheckBox");
            this.confirmConstraintCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "UseConfirmConstraintWhenAdding", true));
            this.confirmConstraintCheckBox.Name = "confirmConstraintCheckBox";
            this.confirmConstraintCheckBox.UseVisualStyleBackColor = true;
            //
            // delayedUpdateKeyInfoTimer
            //
            this.delayedUpdateKeyInfoTimer.Tick += new System.EventHandler(this.delayedUpdateKeyInfoTimer_Tick);
            //
            // lifetimeConstraintCheckBox
            //
            resources.ApplyResources(this.lifetimeConstraintCheckBox, "lifetimeConstraintCheckBox");
            this.lifetimeConstraintCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "UseLifetimeConstraintWhenAdding", true));
            this.lifetimeConstraintCheckBox.Name = "lifetimeConstraintCheckBox";
            this.lifetimeConstraintCheckBox.UseVisualStyleBackColor = true;
            this.lifetimeConstraintCheckBox.CheckedChanged += new System.EventHandler(this.lifetimeConstraintCheckBox_CheckedChanged);
            //
            // lifetimeConstraintNumericUpDown
            //
            this.lifetimeConstraintNumericUpDown.DataBindings.Add(new System.Windows.Forms.Binding("Value", this.entrySettingsBindingSource, "LifetimeConstraintDuration", true));
            resources.ApplyResources(this.lifetimeConstraintNumericUpDown, "lifetimeConstraintNumericUpDown");
            this.lifetimeConstraintNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.lifetimeConstraintNumericUpDown.Name = "lifetimeConstraintNumericUpDown";
            this.lifetimeConstraintNumericUpDown.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
            //
            // lifetimeConstraintLabel
            //
            resources.ApplyResources(this.lifetimeConstraintLabel, "lifetimeConstraintLabel");
            this.lifetimeConstraintLabel.Name = "lifetimeConstraintLabel";
            //
            // openManageFilesDialogButton
            //
            resources.ApplyResources(this.openManageFilesDialogButton, "openManageFilesDialogButton");
            this.openManageFilesDialogButton.Name = "openManageFilesDialogButton";
            this.openManageFilesDialogButton.UseVisualStyleBackColor = true;
            this.openManageFilesDialogButton.Click += new System.EventHandler(this.openManageFilesDialogButton_Click);
            //
            // destinationConstraintCheckBox
            //
            resources.ApplyResources(this.destinationConstraintCheckBox, "destinationConstraintCheckBox");
            this.destinationConstraintCheckBox.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.entrySettingsBindingSource, "UseDestinationConstraintWhenAdding", true));
            this.destinationConstraintCheckBox.Name = "destinationConstraintCheckBox";
            this.destinationConstraintCheckBox.UseVisualStyleBackColor = true;
            this.destinationConstraintCheckBox.CheckedChanged += new System.EventHandler(this.destinationConstraintCheckBox_CheckedChanged);
            //
            // destinationConstraintDataGridView
            //
            resources.ApplyResources(this.destinationConstraintDataGridView, "destinationConstraintDataGridView");
            this.destinationConstraintDataGridView.AutoGenerateColumns = false;
            this.destinationConstraintDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.destinationConstraintDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fromHostDataGridViewTextBoxColumn,
            this.FromHostKeysColumn,
            this.toUserDataGridViewTextBoxColumn,
            this.toHostDataGridViewTextBoxColumn,
            this.ToHostKeysColumn});
            this.destinationConstraintDataGridView.ContextMenuStrip = this.destinationConstraintContextMenuStrip;
            this.destinationConstraintDataGridView.DataSource = this.destinationConstraintBindingSource;
            this.destinationConstraintDataGridView.Name = "destinationConstraintDataGridView";
            this.destinationConstraintDataGridView.RowTemplate.Height = 33;
            this.destinationConstraintDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.destinationConstraintDataGridView_CellContentClick);
            this.destinationConstraintDataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.destinationConstraintDataGridView_CellPainting);
            this.destinationConstraintDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.destinationConstraintDataGridView_DataBindingComplete);
            this.destinationConstraintDataGridView.EnabledChanged += new System.EventHandler(this.destinationConstraintDataGridView_EnabledChanged);
            this.destinationConstraintDataGridView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.destinationConstraintDataGridView_MouseUp);
            //
            // fromHostDataGridViewTextBoxColumn
            //
            this.fromHostDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.fromHostDataGridViewTextBoxColumn.DataPropertyName = "FromHost";
            resources.ApplyResources(this.fromHostDataGridViewTextBoxColumn, "fromHostDataGridViewTextBoxColumn");
            this.fromHostDataGridViewTextBoxColumn.Name = "fromHostDataGridViewTextBoxColumn";
            //
            // FromHostKeysColumn
            //
            this.FromHostKeysColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.FromHostKeysColumn.DataPropertyName = "FromHostKeys";
            resources.ApplyResources(this.FromHostKeysColumn, "FromHostKeysColumn");
            this.FromHostKeysColumn.Name = "FromHostKeysColumn";
            this.FromHostKeysColumn.ReadOnly = true;
            this.FromHostKeysColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.FromHostKeysColumn.Text = "";
            //
            // toUserDataGridViewTextBoxColumn
            //
            this.toUserDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.toUserDataGridViewTextBoxColumn.DataPropertyName = "ToUser";
            resources.ApplyResources(this.toUserDataGridViewTextBoxColumn, "toUserDataGridViewTextBoxColumn");
            this.toUserDataGridViewTextBoxColumn.Name = "toUserDataGridViewTextBoxColumn";
            //
            // toHostDataGridViewTextBoxColumn
            //
            this.toHostDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.toHostDataGridViewTextBoxColumn.DataPropertyName = "ToHost";
            resources.ApplyResources(this.toHostDataGridViewTextBoxColumn, "toHostDataGridViewTextBoxColumn");
            this.toHostDataGridViewTextBoxColumn.Name = "toHostDataGridViewTextBoxColumn";
            //
            // ToHostKeysColumn
            //
            this.ToHostKeysColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ToHostKeysColumn.DataPropertyName = "ToHostKeys";
            resources.ApplyResources(this.ToHostKeysColumn, "ToHostKeysColumn");
            this.ToHostKeysColumn.Name = "ToHostKeysColumn";
            this.ToHostKeysColumn.ReadOnly = true;
            this.ToHostKeysColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ToHostKeysColumn.Text = "";
            //
            // destinationConstraintContextMenuStrip
            //
            this.destinationConstraintContextMenuStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.destinationConstraintContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteCurrentRowMenuItem});
            this.destinationConstraintContextMenuStrip.Name = "contextMenuStrip1";
            resources.ApplyResources(this.destinationConstraintContextMenuStrip, "destinationConstraintContextMenuStrip");
            //
            // deleteCurrentRowMenuItem
            //
            this.deleteCurrentRowMenuItem.Name = "deleteCurrentRowMenuItem";
            resources.ApplyResources(this.deleteCurrentRowMenuItem, "deleteCurrentRowMenuItem");
            this.deleteCurrentRowMenuItem.Click += new System.EventHandler(this.deleteCurrentRowMenuItem_Click);
            //
            // destinationConstraintBindingSource
            //
            this.destinationConstraintBindingSource.DataSource = typeof(KeeAgent.EntrySettings.DestinationConstraint);
            //
            // entrySettingsBindingSource
            //
            this.entrySettingsBindingSource.DataSource = typeof(KeeAgent.EntrySettings);
            //
            // invalidKeyWarningIcon
            //
            this.invalidKeyWarningIcon.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.invalidKeyWarningIcon, "invalidKeyWarningIcon");
            this.invalidKeyWarningIcon.Name = "invalidKeyWarningIcon";
            this.invalidKeyWarningIcon.StockIcon = KeeAgent.UI.SystemIcon.StockIconId.Warning;
            this.invalidKeyWarningIcon.TabStop = false;
            //
            // EntryPanel
            //
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.destinationConstraintDataGridView);
            this.Controls.Add(this.destinationConstraintCheckBox);
            this.Controls.Add(this.invalidKeyWarningIcon);
            this.Controls.Add(this.openManageFilesDialogButton);
            this.Controls.Add(this.lifetimeConstraintLabel);
            this.Controls.Add(this.lifetimeConstraintNumericUpDown);
            this.Controls.Add(this.lifetimeConstraintCheckBox);
            this.Controls.Add(this.confirmConstraintCheckBox);
            this.Controls.Add(this.keyInfoGroupBox);
            this.Controls.Add(this.helpButton);
            this.Controls.Add(this.removeKeyAtCloseCheckBox);
            this.Controls.Add(this.addKeyAtOpenCheckBox);
            this.Controls.Add(this.hasSshKeyCheckBox);
            this.Name = "EntryPanel";
            this.keyInfoGroupBox.ResumeLayout(false);
            this.keyInfoGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lifetimeConstraintNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.destinationConstraintDataGridView)).EndInit();
            this.destinationConstraintContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.destinationConstraintBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entrySettingsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.invalidKeyWarningIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox hasSshKeyCheckBox;
        private System.Windows.Forms.CheckBox addKeyAtOpenCheckBox;
        private System.Windows.Forms.CheckBox removeKeyAtCloseCheckBox;
        internal System.Windows.Forms.BindingSource entrySettingsBindingSource;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.GroupBox keyInfoGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox publicKeyTextBox;
        private System.Windows.Forms.TextBox commentTextBox;
        private System.Windows.Forms.TextBox fingerprintTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button copyPublicKeyButton;
        private System.Windows.Forms.CheckBox confirmConstraintCheckBox;
        private System.Windows.Forms.Timer delayedUpdateKeyInfoTimer;
        private System.Windows.Forms.CheckBox lifetimeConstraintCheckBox;
        private System.Windows.Forms.NumericUpDown lifetimeConstraintNumericUpDown;
        private System.Windows.Forms.Label lifetimeConstraintLabel;
    private System.Windows.Forms.Button openManageFilesDialogButton;
    private SystemIcon invalidKeyWarningIcon;
    private System.Windows.Forms.CheckBox destinationConstraintCheckBox;
    private System.Windows.Forms.DataGridView destinationConstraintDataGridView;
    private System.Windows.Forms.BindingSource destinationConstraintBindingSource;
    private System.Windows.Forms.ContextMenuStrip destinationConstraintContextMenuStrip;
    private System.Windows.Forms.ToolStripMenuItem deleteCurrentRowMenuItem;
    private System.Windows.Forms.DataGridViewTextBoxColumn fromHostDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewButtonColumn FromHostKeysColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn toUserDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn toHostDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewButtonColumn ToHostKeysColumn;
  }
}
