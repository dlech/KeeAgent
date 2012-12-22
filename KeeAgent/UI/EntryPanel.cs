using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using KeePass.UI;
using KeePassLib;

namespace KeeAgent.UI
{
  public partial class EntryPanel : UserControl
  {

    private PwEntry mPwEntry;

    public EntryPanel(PwEntry aPwEntry)
    {
      mPwEntry = aPwEntry;

      InitializeComponent();

      // make transparent so tab styling shows
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;

      entrySettingsBindingSource.DataSource = aPwEntry.GetKeeAgentEntrySettings();
      
      UpdateControlStates();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (ParentForm != null) {
        ParentForm.FormClosing +=
          delegate(object aSender, FormClosingEventArgs aEventArgs)
          {
            if (ParentForm.DialogResult == DialogResult.OK) {
              var settings = entrySettingsBindingSource.DataSource as EntrySettings;
              mPwEntry.SetKeeAgentEntrySettings(settings);
            }
          };
      }
    }

    private void UpdateControlStates()
    {
      loadAtStartupCheckBox.Enabled = hasSshKeyCheckBox.Checked;
    }

    private void hasSshKeyCheckBox_CheckedChanged(object sender, EventArgs e)
    {
      if (!hasSshKeyCheckBox.Checked) {
        loadAtStartupCheckBox.Checked = false;
      }
      UpdateControlStates();
    }
  }
}
