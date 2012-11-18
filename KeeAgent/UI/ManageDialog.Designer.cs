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
      this.closeButton = new System.Windows.Forms.Button();
      this.inFileKeysDataGridView = new System.Windows.Forms.DataGridView();
      this.keyTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.keySizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fingerprintDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.fileNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Group = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Entry = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.DbName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.inFileKeysBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.inFileKeyDataSet = new KeeAgent.KeyDataSet();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.inMemoryKeysDataGridView = new System.Windows.Forms.DataGridView();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.noLoadedKeysLabel = new System.Windows.Forms.Label();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.lockedStatusIconLabel = new System.Windows.Forms.Label();
      this.lockedStatusTextLabel = new System.Windows.Forms.Label();
      this.lockedStatusButton = new System.Windows.Forms.Button();
      this.ConfirmConstraintColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.LifetimeConstraintColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.inMemroyKeyTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.inMemoryKeySizeDataGridViewColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.inMemoryKeyFingerprintDataGridViewColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.inMemoryKeyCommentDataGridViewColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.inFileKeysDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.inFileKeysBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.inFileKeyDataSet)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.inMemoryKeysDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // closeButton
      // 
      resources.ApplyResources(this.closeButton, "closeButton");
      this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.closeButton.Name = "closeButton";
      this.closeButton.UseVisualStyleBackColor = true;
      // 
      // inFileKeysDataGridView
      // 
      this.inFileKeysDataGridView.AllowUserToAddRows = false;
      this.inFileKeysDataGridView.AllowUserToDeleteRows = false;
      this.inFileKeysDataGridView.AllowUserToResizeRows = false;
      resources.ApplyResources(this.inFileKeysDataGridView, "inFileKeysDataGridView");
      this.inFileKeysDataGridView.AutoGenerateColumns = false;
      this.inFileKeysDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.inFileKeysDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.keyTypeDataGridViewTextBoxColumn,
            this.keySizeDataGridViewTextBoxColumn,
            this.fingerprintDataGridViewTextBoxColumn,
            this.commentDataGridViewTextBoxColumn,
            this.fileNameDataGridViewTextBoxColumn,
            this.Group,
            this.Entry,
            this.DbName});
      this.inFileKeysDataGridView.DataSource = this.inFileKeysBindingSource;
      this.inFileKeysDataGridView.Name = "inFileKeysDataGridView";
      this.inFileKeysDataGridView.ReadOnly = true;
      this.inFileKeysDataGridView.RowHeadersVisible = false;
      // 
      // keyTypeDataGridViewTextBoxColumn
      // 
      this.keyTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.keyTypeDataGridViewTextBoxColumn.DataPropertyName = "KeyType";
      resources.ApplyResources(this.keyTypeDataGridViewTextBoxColumn, "keyTypeDataGridViewTextBoxColumn");
      this.keyTypeDataGridViewTextBoxColumn.Name = "keyTypeDataGridViewTextBoxColumn";
      this.keyTypeDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // keySizeDataGridViewTextBoxColumn
      // 
      this.keySizeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.keySizeDataGridViewTextBoxColumn.DataPropertyName = "KeySize";
      resources.ApplyResources(this.keySizeDataGridViewTextBoxColumn, "keySizeDataGridViewTextBoxColumn");
      this.keySizeDataGridViewTextBoxColumn.Name = "keySizeDataGridViewTextBoxColumn";
      this.keySizeDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // fingerprintDataGridViewTextBoxColumn
      // 
      this.fingerprintDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.fingerprintDataGridViewTextBoxColumn.DataPropertyName = "Fingerprint";
      resources.ApplyResources(this.fingerprintDataGridViewTextBoxColumn, "fingerprintDataGridViewTextBoxColumn");
      this.fingerprintDataGridViewTextBoxColumn.Name = "fingerprintDataGridViewTextBoxColumn";
      this.fingerprintDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // commentDataGridViewTextBoxColumn
      // 
      this.commentDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.commentDataGridViewTextBoxColumn.DataPropertyName = "Comment";
      resources.ApplyResources(this.commentDataGridViewTextBoxColumn, "commentDataGridViewTextBoxColumn");
      this.commentDataGridViewTextBoxColumn.Name = "commentDataGridViewTextBoxColumn";
      this.commentDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // fileNameDataGridViewTextBoxColumn
      // 
      this.fileNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.fileNameDataGridViewTextBoxColumn.DataPropertyName = "FileName";
      resources.ApplyResources(this.fileNameDataGridViewTextBoxColumn, "fileNameDataGridViewTextBoxColumn");
      this.fileNameDataGridViewTextBoxColumn.Name = "fileNameDataGridViewTextBoxColumn";
      this.fileNameDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // Group
      // 
      this.Group.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.Group.DataPropertyName = "Group";
      resources.ApplyResources(this.Group, "Group");
      this.Group.Name = "Group";
      this.Group.ReadOnly = true;
      // 
      // Entry
      // 
      this.Entry.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.Entry.DataPropertyName = "Entry";
      resources.ApplyResources(this.Entry, "Entry");
      this.Entry.Name = "Entry";
      this.Entry.ReadOnly = true;
      // 
      // DbName
      // 
      this.DbName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.DbName.DataPropertyName = "DbName";
      resources.ApplyResources(this.DbName, "DbName");
      this.DbName.Name = "DbName";
      this.DbName.ReadOnly = true;
      // 
      // inFileKeysBindingSource
      // 
      this.inFileKeysBindingSource.DataMember = "KeeAgentKeys";
      this.inFileKeysBindingSource.DataSource = this.inFileKeyDataSet;
      // 
      // inFileKeyDataSet
      // 
      this.inFileKeyDataSet.DataSetName = "DataSet";
      this.inFileKeyDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Value";
      resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.ReadOnly = true;
      // 
      // inMemoryKeysDataGridView
      // 
      this.inMemoryKeysDataGridView.AllowUserToAddRows = false;
      this.inMemoryKeysDataGridView.AllowUserToDeleteRows = false;
      this.inMemoryKeysDataGridView.AllowUserToResizeRows = false;
      resources.ApplyResources(this.inMemoryKeysDataGridView, "inMemoryKeysDataGridView");
      this.inMemoryKeysDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.inMemoryKeysDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ConfirmConstraintColumn,
            this.LifetimeConstraintColumn,
            this.inMemroyKeyTypeDataGridViewTextBoxColumn,
            this.inMemoryKeySizeDataGridViewColumn,
            this.inMemoryKeyFingerprintDataGridViewColumn,
            this.inMemoryKeyCommentDataGridViewColumn});
      this.inMemoryKeysDataGridView.Name = "inMemoryKeysDataGridView";
      this.inMemoryKeysDataGridView.ReadOnly = true;
      this.inMemoryKeysDataGridView.RowHeadersVisible = false;
      this.inMemoryKeysDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.inMemoryKeysDataGridView_CellFormatting);
      // 
      // label1
      // 
      resources.ApplyResources(this.label1, "label1");
      this.label1.Name = "label1";
      // 
      // label2
      // 
      resources.ApplyResources(this.label2, "label2");
      this.label2.Name = "label2";
      // 
      // splitContainer1
      // 
      resources.ApplyResources(this.splitContainer1, "splitContainer1");
      this.splitContainer1.Name = "splitContainer1";
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.inMemoryKeysDataGridView);
      this.splitContainer1.Panel1.Controls.Add(this.label1);
      this.splitContainer1.Panel1.Controls.Add(this.noLoadedKeysLabel);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.inFileKeysDataGridView);
      this.splitContainer1.Panel2.Controls.Add(this.label2);
      this.splitContainer1.Panel2.Controls.Add(this.closeButton);
      // 
      // noLoadedKeysLabel
      // 
      resources.ApplyResources(this.noLoadedKeysLabel, "noLoadedKeysLabel");
      this.noLoadedKeysLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.noLoadedKeysLabel.Name = "noLoadedKeysLabel";
      // 
      // tableLayoutPanel1
      // 
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.lockedStatusIconLabel);
      this.flowLayoutPanel1.Controls.Add(this.lockedStatusTextLabel);
      this.flowLayoutPanel1.Controls.Add(this.lockedStatusButton);
      resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      // 
      // lockedStatusIconLabel
      // 
      resources.ApplyResources(this.lockedStatusIconLabel, "lockedStatusIconLabel");
      this.lockedStatusIconLabel.Image = global::KeeAgent.Properties.Resources.Locked;
      this.lockedStatusIconLabel.Name = "lockedStatusIconLabel";
      // 
      // lockedStatusTextLabel
      // 
      resources.ApplyResources(this.lockedStatusTextLabel, "lockedStatusTextLabel");
      this.lockedStatusTextLabel.Name = "lockedStatusTextLabel";
      // 
      // lockedStatusButton
      // 
      resources.ApplyResources(this.lockedStatusButton, "lockedStatusButton");
      this.lockedStatusButton.Name = "lockedStatusButton";
      this.lockedStatusButton.UseVisualStyleBackColor = true;
      this.lockedStatusButton.Click += new System.EventHandler(this.lockedStatusButton_Click);
      // 
      // ConfirmConstraintColumn
      // 
      this.ConfirmConstraintColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.ConfirmConstraintColumn.DataPropertyName = "Constraints";
      resources.ApplyResources(this.ConfirmConstraintColumn, "ConfirmConstraintColumn");
      this.ConfirmConstraintColumn.Name = "ConfirmConstraintColumn";
      this.ConfirmConstraintColumn.ReadOnly = true;
      // 
      // LifetimeConstraintColumn
      // 
      this.LifetimeConstraintColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.LifetimeConstraintColumn.DataPropertyName = "Constraints";
      resources.ApplyResources(this.LifetimeConstraintColumn, "LifetimeConstraintColumn");
      this.LifetimeConstraintColumn.Name = "LifetimeConstraintColumn";
      this.LifetimeConstraintColumn.ReadOnly = true;
      // 
      // inMemroyKeyTypeDataGridViewTextBoxColumn
      // 
      this.inMemroyKeyTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.inMemroyKeyTypeDataGridViewTextBoxColumn.DataPropertyName = "Algorithm";
      resources.ApplyResources(this.inMemroyKeyTypeDataGridViewTextBoxColumn, "inMemroyKeyTypeDataGridViewTextBoxColumn");
      this.inMemroyKeyTypeDataGridViewTextBoxColumn.Name = "inMemroyKeyTypeDataGridViewTextBoxColumn";
      this.inMemroyKeyTypeDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // inMemoryKeySizeDataGridViewColumn
      // 
      this.inMemoryKeySizeDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.inMemoryKeySizeDataGridViewColumn.DataPropertyName = "Size";
      resources.ApplyResources(this.inMemoryKeySizeDataGridViewColumn, "inMemoryKeySizeDataGridViewColumn");
      this.inMemoryKeySizeDataGridViewColumn.Name = "inMemoryKeySizeDataGridViewColumn";
      this.inMemoryKeySizeDataGridViewColumn.ReadOnly = true;
      // 
      // inMemoryKeyFingerprintDataGridViewColumn
      // 
      this.inMemoryKeyFingerprintDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.inMemoryKeyFingerprintDataGridViewColumn.DataPropertyName = "MD5Fingerprint";
      resources.ApplyResources(this.inMemoryKeyFingerprintDataGridViewColumn, "inMemoryKeyFingerprintDataGridViewColumn");
      this.inMemoryKeyFingerprintDataGridViewColumn.Name = "inMemoryKeyFingerprintDataGridViewColumn";
      this.inMemoryKeyFingerprintDataGridViewColumn.ReadOnly = true;
      // 
      // inMemoryKeyCommentDataGridViewColumn
      // 
      this.inMemoryKeyCommentDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.inMemoryKeyCommentDataGridViewColumn.DataPropertyName = "Comment";
      resources.ApplyResources(this.inMemoryKeyCommentDataGridViewColumn, "inMemoryKeyCommentDataGridViewColumn");
      this.inMemoryKeyCommentDataGridViewColumn.Name = "inMemoryKeyCommentDataGridViewColumn";
      this.inMemoryKeyCommentDataGridViewColumn.ReadOnly = true;
      // 
      // ManageDialog
      // 
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tableLayoutPanel1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ManageDialog";
      this.ShowInTaskbar = false;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyListDialog_FormClosing);
      this.Load += new System.EventHandler(this.KeyListDialog_Load);
      this.Shown += new System.EventHandler(this.PuttyKeyListDialog_Shown);
      ((System.ComponentModel.ISupportInitialize)(this.inFileKeysDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.inFileKeysBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.inFileKeyDataSet)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.inMemoryKeysDataGridView)).EndInit();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel1.PerformLayout();
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button closeButton;
		private KeyDataSet inFileKeyDataSet;
		private System.Windows.Forms.DataGridView inFileKeysDataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.BindingSource inFileKeysBindingSource;
        private System.Windows.Forms.DataGridView inMemoryKeysDataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn algorithmDataGridViewTextBoxColumn;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lockedStatusIconLabel;
        private System.Windows.Forms.Label lockedStatusTextLabel;
        private System.Windows.Forms.Button lockedStatusButton;
        private System.Windows.Forms.Label noLoadedKeysLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn keySizeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fingerprintDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Group;
        private System.Windows.Forms.DataGridViewTextBoxColumn Entry;
        private System.Windows.Forms.DataGridViewTextBoxColumn DbName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ConfirmConstraintColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LifetimeConstraintColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn inMemroyKeyTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn inMemoryKeySizeDataGridViewColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn inMemoryKeyFingerprintDataGridViewColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn inMemoryKeyCommentDataGridViewColumn;
	}
}