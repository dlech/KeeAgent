namespace KeeAgent.UI
{
    partial class OptionsDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.button3 = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.notifyOptionsGroupBox = new System.Windows.Forms.GroupBox();
      this.radioButton4 = new System.Windows.Forms.RadioButton();
      this.radioButton3 = new System.Windows.Forms.RadioButton();
      this.radioButton2 = new System.Windows.Forms.RadioButton();
      this.radioButton1 = new System.Windows.Forms.RadioButton();
      this.tableLayoutPanel1.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.notifyOptionsGroupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
      this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
      this.tableLayoutPanel1.Controls.Add(this.notifyOptionsGroupBox, 0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      // 
      // flowLayoutPanel1
      // 
      resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
      this.flowLayoutPanel1.Controls.Add(this.button1);
      this.flowLayoutPanel1.Controls.Add(this.button2);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      // 
      // button1
      // 
      resources.ApplyResources(this.button1, "button1");
      this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button1.Name = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      resources.ApplyResources(this.button2, "button2");
      this.button2.Name = "button2";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.checkBox1);
      this.groupBox1.Controls.Add(this.button3);
      this.groupBox1.Controls.Add(this.textBox1);
      this.groupBox1.Controls.Add(this.label1);
      resources.ApplyResources(this.groupBox1, "groupBox1");
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.TabStop = false;
      // 
      // checkBox1
      // 
      resources.ApplyResources(this.checkBox1, "checkBox1");
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.UseVisualStyleBackColor = true;
      this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
      // 
      // button3
      // 
      resources.ApplyResources(this.button3, "button3");
      this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.button3.Name = "button3";
      this.button3.UseVisualStyleBackColor = true;
      this.button3.Click += new System.EventHandler(this.button3_Click);
      // 
      // textBox1
      // 
      resources.ApplyResources(this.textBox1, "textBox1");
      this.textBox1.Name = "textBox1";
      // 
      // label1
      // 
      resources.ApplyResources(this.label1, "label1");
      this.label1.Name = "label1";
      // 
      // notifyOptionsGroupBox
      // 
      this.notifyOptionsGroupBox.Controls.Add(this.radioButton4);
      this.notifyOptionsGroupBox.Controls.Add(this.radioButton3);
      this.notifyOptionsGroupBox.Controls.Add(this.radioButton2);
      this.notifyOptionsGroupBox.Controls.Add(this.radioButton1);
      resources.ApplyResources(this.notifyOptionsGroupBox, "notifyOptionsGroupBox");
      this.notifyOptionsGroupBox.Name = "notifyOptionsGroupBox";
      this.notifyOptionsGroupBox.TabStop = false;
      // 
      // radioButton4
      // 
      resources.ApplyResources(this.radioButton4, "radioButton4");
      this.radioButton4.Name = "radioButton4";
      this.radioButton4.TabStop = true;
      this.radioButton4.UseVisualStyleBackColor = true;
      this.radioButton4.CheckedChanged += new System.EventHandler(this.radioButton4_CheckedChanged);
      // 
      // radioButton3
      // 
      resources.ApplyResources(this.radioButton3, "radioButton3");
      this.radioButton3.Name = "radioButton3";
      this.radioButton3.TabStop = true;
      this.radioButton3.UseVisualStyleBackColor = true;
      this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
      // 
      // radioButton2
      // 
      resources.ApplyResources(this.radioButton2, "radioButton2");
      this.radioButton2.Name = "radioButton2";
      this.radioButton2.TabStop = true;
      this.radioButton2.UseVisualStyleBackColor = true;
      this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
      // 
      // radioButton1
      // 
      resources.ApplyResources(this.radioButton1, "radioButton1");
      this.radioButton1.Name = "radioButton1";
      this.radioButton1.TabStop = true;
      this.radioButton1.UseVisualStyleBackColor = true;
      this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
      // 
      // OptionsDialog
      // 
      this.AcceptButton = this.button1;
      resources.ApplyResources(this, "$this");
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.button2;
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "OptionsDialog";
      this.ShowInTaskbar = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.notifyOptionsGroupBox.ResumeLayout(false);
      this.notifyOptionsGroupBox.PerformLayout();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox notifyOptionsGroupBox;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
    }
}