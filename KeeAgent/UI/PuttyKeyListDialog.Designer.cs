namespace KeeAgent.UI
{
	partial class PuttyKeyListDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PuttyKeyListDialog));
			this.closeButton = new System.Windows.Forms.Button();
			this.keyDataGridView = new System.Windows.Forms.DataGridView();
			this.ppkKeyMapBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ppkKeyMapBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
			this.puttyKeyBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.puttyKeyDataSet = new KeeAgent.PuttyKeyDataSet();
			this.puttyKeysBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.keyTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.keySizeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fingerprintDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dbPathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fileNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.keyDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ppkKeyMapBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ppkKeyMapBindingSource1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyDataSet)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeysBindingSource)).BeginInit();
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
			this.keyDataGridView.DataSource = this.puttyKeysBindingSource;
			this.keyDataGridView.Name = "keyDataGridView";
			this.keyDataGridView.ReadOnly = true;
			this.keyDataGridView.RowHeadersVisible = false;
			// 
			// ppkKeyMapBindingSource
			// 
			this.ppkKeyMapBindingSource.DataSource = typeof(System.Collections.Generic.KeyValuePair<string, dlech.PageantSharp.PpkKey>);
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.DataPropertyName = "Value";
			resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			// 
			// ppkKeyMapBindingSource1
			// 
			this.ppkKeyMapBindingSource1.DataSource = typeof(System.Collections.Generic.KeyValuePair<string, KeePassLib.PwUuid>);
			// 
			// puttyKeyBindingSource
			// 
			this.puttyKeyBindingSource.DataSource = this.puttyKeyDataSet;
			this.puttyKeyBindingSource.Position = 0;
			// 
			// puttyKeyDataSet
			// 
			this.puttyKeyDataSet.DataSetName = "PuttyKeyDataSet";
			this.puttyKeyDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// puttyKeysBindingSource
			// 
			this.puttyKeysBindingSource.DataMember = "PuttyKeys";
			this.puttyKeysBindingSource.DataSource = this.puttyKeyDataSet;
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
			// PuttyKeyListDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.keyDataGridView);
			this.Controls.Add(this.closeButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PuttyKeyListDialog";
			this.ShowInTaskbar = false;
			this.Shown += new System.EventHandler(this.PuttyKeyListDialog_Shown);
			((System.ComponentModel.ISupportInitialize)(this.keyDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ppkKeyMapBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ppkKeyMapBindingSource1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyDataSet)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeysBindingSource)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button closeButton;
		private PuttyKeyDataSet puttyKeyDataSet;
		private System.Windows.Forms.BindingSource puttyKeyBindingSource;
		private System.Windows.Forms.DataGridView keyDataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn encryptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.BindingSource ppkKeyMapBindingSource;
		private System.Windows.Forms.BindingSource ppkKeyMapBindingSource1;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn keyTypeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn keySizeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn fingerprintDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn dbPathDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.BindingSource puttyKeysBindingSource;
	}
}