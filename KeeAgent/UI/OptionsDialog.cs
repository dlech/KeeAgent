using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace KeeAgent.UI
{
    public partial class OptionsDialog : Form
    {
        private KeeAgentExt ext;
        private Options selectedOptions;

        public OptionsDialog(KeeAgentExt ext)
        {
            this.ext = ext;
            this.selectedOptions = (Options)this.ext.options.Clone();

            InitializeComponent();

            switch (this.selectedOptions.Notification) {
                case NotificationOptions.AlwaysAsk:
                    radioButton1.Checked = true;
                    break;
                case NotificationOptions.AskOnce:
                    radioButton2.Checked = true;
                    break;
                case NotificationOptions.Balloon:
                    radioButton3.Checked = true;
                    break;
                case NotificationOptions.Never:
                    radioButton4.Checked = true;
                    break;
                default:
                    Debug.Fail("Unsupported option");
                    break;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.selectedOptions.Notification = NotificationOptions.AlwaysAsk;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.selectedOptions.Notification = NotificationOptions.AskOnce;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.selectedOptions.Notification = NotificationOptions.Balloon;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.selectedOptions.Notification = NotificationOptions.Never;
        }

        // OK button
        private void button1_Click(object sender, EventArgs e)
        {
            this.ext.options = selectedOptions;
            this.ext.saveOptions();
            Close();
        }
    }
}
