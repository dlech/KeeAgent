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
			this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.encryptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.commentDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.publicKeyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.privateKeyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.privateMACDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.puttyKeyBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.puttyKeyDataSet = new KeeAgent.PuttyKeyDataSet();
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
            this.typeDataGridViewTextBoxColumn,
            this.encryptionDataGridViewTextBoxColumn,
            this.commentDataGridViewTextBoxColumn,
            this.publicKeyDataGridViewTextBoxColumn,
            this.privateKeyDataGridViewTextBoxColumn,
            this.privateMACDataGridViewTextBoxColumn});
			this.keyDataGridView.DataSource = this.puttyKeyBindingSource;
			this.keyDataGridView.Name = "keyDataGridView";
			this.keyDataGridView.ReadOnly = true;
			this.keyDataGridView.RowHeadersVisible = false;
			// 
			// typeDataGridViewTextBoxColumn
			// 
			this.typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
			resources.ApplyResources(this.typeDataGridViewTextBoxColumn, "typeDataGridViewTextBoxColumn");
			this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
			this.typeDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// encryptionDataGridViewTextBoxColumn
			// 
			this.encryptionDataGridViewTextBoxColumn.DataPropertyName = "Encryption";
			resources.ApplyResources(this.encryptionDataGridViewTextBoxColumn, "encryptionDataGridViewTextBoxColumn");
			this.encryptionDataGridViewTextBoxColumn.Name = "encryptionDataGridViewTextBoxColumn";
			this.encryptionDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// commentDataGridViewTextBoxColumn
			// 
			this.commentDataGridViewTextBoxColumn.DataPropertyName = "Comment";
			resources.ApplyResources(this.commentDataGridViewTextBoxColumn, "commentDataGridViewTextBoxColumn");
			this.commentDataGridViewTextBoxColumn.Name = "commentDataGridViewTextBoxColumn";
			this.commentDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// publicKeyDataGridViewTextBoxColumn
			// 
			this.publicKeyDataGridViewTextBoxColumn.DataPropertyName = "PublicKey";
			resources.ApplyResources(this.publicKeyDataGridViewTextBoxColumn, "publicKeyDataGridViewTextBoxColumn");
			this.publicKeyDataGridViewTextBoxColumn.Name = "publicKeyDataGridViewTextBoxColumn";
			this.publicKeyDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// privateKeyDataGridViewTextBoxColumn
			// 
			this.privateKeyDataGridViewTextBoxColumn.DataPropertyName = "PrivateKey";
			resources.ApplyResources(this.privateKeyDataGridViewTextBoxColumn, "privateKeyDataGridViewTextBoxColumn");
			this.privateKeyDataGridViewTextBoxColumn.Name = "privateKeyDataGridViewTextBoxColumn";
			this.privateKeyDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// privateMACDataGridViewTextBoxColumn
			// 
			this.privateMACDataGridViewTextBoxColumn.DataPropertyName = "PrivateMAC";
			resources.ApplyResources(this.privateMACDataGridViewTextBoxColumn, "privateMACDataGridViewTextBoxColumn");
			this.privateMACDataGridViewTextBoxColumn.Name = "privateMACDataGridViewTextBoxColumn";
			this.privateMACDataGridViewTextBoxColumn.ReadOnly = true;
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
		private System.Windows.Forms.DataGridViewTextBoxColumn commentDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn publicKeyDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn privateKeyDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn privateMACDataGridViewTextBoxColumn;
	}
}