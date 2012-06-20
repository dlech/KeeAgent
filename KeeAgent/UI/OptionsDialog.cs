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

        public OptionsDialog(KeeAgentExt ext)
        {
            this.ext = ext;

            InitializeComponent();
            switch (this.ext.options.Notification) {
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
            this.ext.options.Notification = NotificationOptions.AlwaysAsk;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            this.ext.options.Notification = NotificationOptions.AskOnce;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.ext.options.Notification = NotificationOptions.Balloon;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.ext.options.Notification = NotificationOptions.Never;
        }       
    }
}
