using System;
using System.Windows.Forms;
using dlech.SshAgentLib;
using KeePassLib.Utility;

namespace KeeAgent.UI
{
  public partial class ManageDialog : Form
  {
    private KeeAgentExt mExt;

    public ManageDialog(KeeAgentExt aExt)
    {
      InitializeComponent();

#if __MonoCS__
      Icon = Properties.Resources.KeeAgent_icon_mono;
#else
      Icon = Properties.Resources.KeeAgent_ico;
#endif

      // update title depending on Agent Mode
      mExt = aExt;
      if (mExt.mAgent is Agent) {
        Text += Translatable.TitleSuffixAgentMode;
      } else {
        Text += Translatable.TitleSuffixClientMode;
      }
      keyInfoView.SetAgent(mExt.mAgent);
    }

    protected override void OnShown (EventArgs e)
    {
      base.OnShown (e);
      keyInfoView.dataGridView.ClearSelection();
    }

    private void addButtonFromFileMenuItem_Click(object sender, EventArgs e)
    {
      keyInfoView.ShowFileOpenDialog();
    }

    private void addButtonFromKeePassMenuItem_Click(object sender, EventArgs e)
    {
      var openDatabaseCount =
        mExt.mPluginHost.MainWindow.DocumentManager.GetOpenDatabases().Count;
      if (openDatabaseCount == 0) {
        MessageService.ShowWarning("No open databases found.",
          "Please open or unlock a database and then try again.");
        return;
      }

      var showConstraintControls = !(mExt.mAgent is PageantClient);
      var entryPicker =
        new EntryPickerDialog(mExt.mPluginHost, showConstraintControls);
      var result = entryPicker.ShowDialog();
      if (result == DialogResult.OK) {
        try {
          mExt.AddEntry(entryPicker.SelectedEntry, entryPicker.Constraints);
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
