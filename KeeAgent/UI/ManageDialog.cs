using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using dlech.PageantSharp;
using KeePassLib;
using Org.BouncyCastle.Crypto.Parameters;
using System.ComponentModel;
using System.Drawing;
using KeeAgent.Properties;
using System.Text;

namespace KeeAgent.UI
{
  public partial class ManageDialog : Form
  {
    private KeeAgentExt mExt;

    public ManageDialog(KeeAgentExt aExt)
    {
      mExt = aExt;

      InitializeComponent();

      UpdateLockedStatus(this, new Agent.LockEventArgs(mExt.mPageant.IsLocked));
      UpdateLoadedKeys();

      inMemoryKeysDataGridView.AutoGenerateColumns = false;
      inMemoryKeysDataGridView.DataSource = aExt.mInMemoryKeys;

      IEnumerable<KeeAgentKey> inDatabaseKeyList = aExt.GetKeeAgentKeyList();

      foreach (KeeAgentKey key in inDatabaseKeyList) {
        PwEntry entry = null;
        List<PwDatabase> databases = aExt.mPluginHost.MainWindow.DocumentManager.GetOpenDatabases();
        foreach (PwDatabase database in databases) {
          // make sure we are looking in the right database
          if (database.IOConnectionInfo.Path == key.DbPath) {
            entry = database.RootGroup.FindEntry(key.Uuid, true);
            break;
          }
        }
        if (entry != null) {

          /* get group path */
          string groupPath = entry.ParentGroup.GetFullPath(Path.DirectorySeparatorChar.ToString(), false);

          /* get entry title */
          string entryTitle = entry.Strings.Get(PwDefs.TitleField).ReadString();

          /* get fingerprint */
          string fingerprint = key.Fingerprint;

          string algorithm = null;
          if (key.CipherKeyPair.Public is RsaKeyParameters) {
            algorithm = OpenSsh.PublicKeyAlgorithm.SSH_RSA.GetName();
          } else if (key.CipherKeyPair.Public is DsaPublicKeyParameters) {
            algorithm = OpenSsh.PublicKeyAlgorithm.SSH_DSS.GetName();
          } else {
            algorithm = Translatable.UnknownAlgorithm;
          }

          /* add info to data grid view */
          inFileKeyDataSet.KeeAgentKeys.AddKeeAgentKeysRow(
            algorithm,
            key.Size,
            fingerprint,
            key.Comment,
            key.DbPath,
            groupPath,
            entryTitle,
            key.KeyFileName
          );
        }

        key.Dispose();
      }
    }

    private void UpdateLockedStatus(object aSender, Agent.LockEventArgs aEventArgs)
    {
      mExt.mPluginHost.MainWindow.Invoke((MethodInvoker)delegate()
      {
        if (aEventArgs.IsLocked) {
          lockedStatusIconLabel.Image = Resources.Locked;
          lockedStatusTextLabel.Text = Translatable.StatusLocked;
          lockedStatusButton.Text = Translatable.ButtonUnlock;

        } else {
          lockedStatusIconLabel.Image = Resources.Unlocked;
          lockedStatusTextLabel.Text = Translatable.StatusUnlocked;
          lockedStatusButton.Text = Translatable.ButtonLock;
        }
      });
    }

    private void PuttyKeyListDialog_Shown(object aSender, EventArgs aEvent)
    {
      // borrow icon from owner
      if (Owner != null) {
        Icon = Owner.Icon;
      }
    }

    private void KeyListDialog_Load(object sender, EventArgs e)
    {
      mExt.mPageant.Locked += UpdateLockedStatus;
    }

    private void KeyListDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      mExt.mPageant.Locked -= UpdateLockedStatus;
    }

    private void lockedStatusButton_Click(object sender, EventArgs e)
    {
      using (PasswordDialog dialog = new PasswordDialog()) {
        bool success = false;
        while (!success) {
          DialogResult result = dialog.ShowDialog();
          if (result != DialogResult.OK) {
            return;
          }
          PinnedByteArray password = dialog.GetPassword();
          if (mExt.mPageant.IsLocked) {
            success = mExt.mPageant.Unlock(password.Data);
          } else {
            success = mExt.mPageant.Lock(password.Data);
          }
          password.Clear();
        }
      }
    }

    private void inMemoryKeysDataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      UpdateLoadedKeys();
    }

    private void inMemoryKeysDataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
    {
      UpdateLoadedKeys();
    }

    private void UpdateLoadedKeys()
    {
      bool noKeys = (inMemoryKeysDataGridView.RowCount == 0);
      noLoadedKeysLabel.Visible = noKeys;
      inMemoryKeysDataGridView.Visible = !noKeys;
    }

  }
}
