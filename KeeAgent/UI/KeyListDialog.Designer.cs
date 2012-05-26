namespace KeeAgent.UI
{
	partial class KeyListDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyListDialog));
			this.closeButton = new System.Windows.Forms.Button();
			this.keyDataGridView = new System.Windows.Forms.DataGridView();
			this.keyTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.keySizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fingerprintDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dbPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fileNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.keysBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.keyDataSet = new KeeAgent.KeyDataSet();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.keyDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.keysBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.keyDataSet)).BeginInit();
			this.SuspendLayout();
			// 
			// closeButton
			// 
			resources.ApplyResources(this.closeButton, "closeButton");
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Name = "closeButton";
			this.closeButton.UseVisualStyleBackColor = true;
			// 
			// keyDataGridView
			// 
			this.keyDataGridView.AllowUserToAddRows = false;
			this.keyDataGridView.AllowUserToDeleteRows = false;
			resources.ApplyResources(this.keyDataGridView, "keyDataGridView");
			this.keyDataGridView.AutoGenerateColumns = false;
			this.keyDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.keyDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.keyTypeDataGridViewTextBoxColumn,
            this.keySizeDataGridViewTextBoxColumn,
            this.fingerprintDataGridViewTextBoxColumn,
            this.commentDataGridViewTextBoxColumn,
            this.dbPathDataGridViewTextBoxColumn,
            this.fileNameDataGridViewTextBoxColumn});
			this.keyDataGridView.DataSource = this.keysBindingSource;
			this.keyDataGridView.Name = "keyDataGridView";
			this.keyDataGridView.ReadOnly = true;
			this.keyDataGridView.RowHeadersVisible = false;
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
			// dbPathDataGridViewTextBoxColumn
			// 
			this.dbPathDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.dbPathDataGridViewTextBoxColumn.DataPropertyName = "DbPath";
			resources.ApplyResources(this.dbPathDataGridViewTextBoxColumn, "dbPathDataGridViewTextBoxColumn");
			this.dbPathDataGridViewTextBoxColumn.Name = "dbPathDataGridViewTextBoxColumn";
			this.dbPathDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// fileNameDataGridViewTextBoxColumn
			// 
			this.fileNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.fileNameDataGridViewTextBoxColumn.DataPropertyName = "FileName";
			resources.ApplyResources(this.fileNameDataGridViewTextBoxColumn, "fileNameDataGridViewTextBoxColumn");
			this.fileNameDataGridViewTextBoxColumn.Name = "fileNameDataGridViewTextBoxColumn";
			this.fileNameDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// keysBindingSource
			// 
			this.keysBindingSource.DataMember = "Keys";
			this.keysBindingSource.DataSource = this.keyDataSet;
			// 
			// keyDataSet
			// 
			this.keyDataSet.DataSetName = "DataSet";
			this.keyDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.DataPropertyName = "Value";
			resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			// 
			// KeeAgentKeyListDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.keyDataGridView);
			this.Controls.Add(this.closeButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "KeeAgentKeyListDialog";
			this.ShowInTaskbar = false;
			this.Shown += new System.EventHandler(this.PuttyKeyListDialog_Shown);
			((System.ComponentModel.ISupportInitialize)(this.keyDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.keysBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.keyDataSet)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button closeButton;
		private KeyDataSet keyDataSet;
		private System.Windows.Forms.DataGridView keyDataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn keyTypeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn keySizeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn fingerprintDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn dbPathDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.BindingSource keysBindingSource;
	}
}