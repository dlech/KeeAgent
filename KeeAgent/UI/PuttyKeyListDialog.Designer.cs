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
			this.puttyKeyBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.puttyKeyDataSet = new KeeAgent.PuttyKeyDataSet();
			this.DbPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.KeyType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EncryptionType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Size = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PrivateMAC = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.keyDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyDataSet)).BeginInit();
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
            this.DbPath,
            this.FileName,
            this.commentDataGridViewTextBoxColumn,
            this.KeyType,
            this.EncryptionType,
            this.Size,
            this.PrivateMAC});
			this.keyDataGridView.DataSource = this.puttyKeyBindingSource;
			this.keyDataGridView.Name = "keyDataGridView";
			this.keyDataGridView.ReadOnly = true;
			this.keyDataGridView.RowHeadersVisible = false;
			this.keyDataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.keyDataGridView_CellFormatting);
			// 
			// puttyKeyBindingSource
			// 
			this.puttyKeyBindingSource.DataMember = "PuttyKeys";
			this.puttyKeyBindingSource.DataSource = this.puttyKeyDataSet;
			// 
			// puttyKeyDataSet
			// 
			this.puttyKeyDataSet.DataSetName = "PuttyKeyDataSet";
			this.puttyKeyDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// DbPath
			// 
			this.DbPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.DbPath.DataPropertyName = "DbPath";
			resources.ApplyResources(this.DbPath, "DbPath");
			this.DbPath.Name = "DbPath";
			this.DbPath.ReadOnly = true;
			// 
			// FileName
			// 
			this.FileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.FileName.DataPropertyName = "FileName";
			resources.ApplyResources(this.FileName, "FileName");
			this.FileName.Name = "FileName";
			this.FileName.ReadOnly = true;
			// 
			// commentDataGridViewTextBoxColumn
			// 
			this.commentDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.commentDataGridViewTextBoxColumn.DataPropertyName = "Comment";
			resources.ApplyResources(this.commentDataGridViewTextBoxColumn, "commentDataGridViewTextBoxColumn");
			this.commentDataGridViewTextBoxColumn.Name = "commentDataGridViewTextBoxColumn";
			this.commentDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// KeyType
			// 
			this.KeyType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.KeyType.DataPropertyName = "KeyType";
			resources.ApplyResources(this.KeyType, "KeyType");
			this.KeyType.Name = "KeyType";
			this.KeyType.ReadOnly = true;
			// 
			// EncryptionType
			// 
			this.EncryptionType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.EncryptionType.DataPropertyName = "EncryptionType";
			resources.ApplyResources(this.EncryptionType, "EncryptionType");
			this.EncryptionType.Name = "EncryptionType";
			this.EncryptionType.ReadOnly = true;
			// 
			// Size
			// 
			this.Size.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Size.DataPropertyName = "PrivateKey";
			resources.ApplyResources(this.Size, "Size");
			this.Size.Name = "Size";
			this.Size.ReadOnly = true;
			// 
			// PrivateMAC
			// 
			this.PrivateMAC.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.PrivateMAC.DataPropertyName = "PrivateMAC";
			resources.ApplyResources(this.PrivateMAC, "PrivateMAC");
			this.PrivateMAC.Name = "PrivateMAC";
			this.PrivateMAC.ReadOnly = true;
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
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.puttyKeyDataSet)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button closeButton;
		private PuttyKeyDataSet puttyKeyDataSet;
		private System.Windows.Forms.BindingSource puttyKeyBindingSource;
		private System.Windows.Forms.DataGridView keyDataGridView;
		private System.Windows.Forms.DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn encryptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn DbPath;
		private System.Windows.Forms.DataGridViewTextBoxColumn FileName;
		private System.Windows.Forms.DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn KeyType;
		private System.Windows.Forms.DataGridViewTextBoxColumn EncryptionType;
		private System.Windows.Forms.DataGridViewTextBoxColumn Size;
		private System.Windows.Forms.DataGridViewTextBoxColumn PrivateMAC;
	}
}