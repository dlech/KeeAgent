using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePassLib;
using System.ComponentModel;
using System.Drawing;
using KeeAgent.Properties;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace KeeAgent.UI
{
  public partial class ManageDialog : Form
  {
    private KeeAgentExt mExt;

    public ManageDialog(KeeAgentExt aExt)
    {
      InitializeComponent();

      // update title depending on Agent Mode
      mExt = aExt;
      if (mExt.mAgent is Agent) {
        Text += Translatable.TitleSuffixAgentMode;
      } else {
        Text += Translatable.TitleSuffixClientMode;
      }
      keyInfoView.SetAgent(mExt.mAgent);
    }

    private void addButtonFromFileMenuItem_Click(object sender, EventArgs e)
    {
      keyInfoView.ShowFileOpenDialog();
    }

    private void addButtonFromKeePassMenuItem_Click(object sender, EventArgs e)
    {
      var entryPicker = new EntryPickerDialog(mExt.mPluginHost);
      var result = entryPicker.ShowDialog();
      if (result == DialogResult.OK) {
        try {
          mExt.AddEntry(entryPicker.SelectedEntry);
        } catch (Exception) {
          // error message already shown
        }        
      }
      if (mExt.mAgent is AgentClient) {
        keyInfoView.ReloadKeyListView();
      }
    }
  }
}
