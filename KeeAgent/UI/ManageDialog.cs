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
      mExt = aExt;

      InitializeComponent();

      inMemoryKeysDataGridView.AutoGenerateColumns = false;

      UpdateLockedStatus(this, new Agent.LockEventArgs(mExt.mPageant.IsLocked));
      UpdateLoadedKeys();      

      IEnumerable<KeeAgentKey> inDatabaseKeyList = mExt.GetKeeAgentKeyList();

      foreach (KeeAgentKey key in inDatabaseKeyList) {
        PwEntry entry = null;
        List<PwDatabase> databases = mExt.mPluginHost.MainWindow.DocumentManager.GetOpenDatabases();
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
          string fingerprint = key.MD5Fingerprint.ToHexString();

          string algorithm = key.Algorithm.GetIdentifierString();          

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
      mExt.mPageant.KeyListChanged += Agent_KeyListChanged;
    }

    private void KeyListDialog_FormClosing(object sender, FormClosingEventArgs e)
    {
      mExt.mPageant.Locked -= UpdateLockedStatus;
      mExt.mPageant.KeyListChanged -= Agent_KeyListChanged;
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
        
    private void UpdateLoadedKeys()
    {
      mExt.mPluginHost.MainWindow.Invoke((MethodInvoker)delegate()
      {
        // TODO fix cross-thread issues - don't really want to copy keys here
        inMemoryKeysDataGridView.DataSource = null;
        ISshKey[] keyList = new ISshKey[mExt.mPageant.KeyCount];
        mExt.mPageant.GetAllKeys().CopyTo(keyList, 0);
        inMemoryKeysDataGridView.DataSource = keyList;
        bool noKeys = (inMemoryKeysDataGridView.RowCount == 0);
        noLoadedKeysLabel.Visible = noKeys;
        inMemoryKeysDataGridView.Visible = !noKeys;
      }); ;
    }

    private void Agent_KeyListChanged(object aSender,
      Agent.KeyListChangeEventArgs aEventArgs)
    {
      UpdateLoadedKeys();
    }

    private void inMemoryKeysDataGridView_CellFormatting(object sender,
      DataGridViewCellFormattingEventArgs e)
    {
      if (e.Value is ICollection<Agent.KeyConstraint>) {
        ICollection<Agent.KeyConstraint> constraints =
          (ICollection<Agent.KeyConstraint>)e.Value;
        e.Value = string.Empty;
        if (e.ColumnIndex == ConfirmConstraintColumn.Index) {          
          foreach (Agent.KeyConstraint constraint in constraints) {
            if (constraint.Type ==
              Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_CONFIRM) {
                e.Value = "*";
                break;
            }
          }          
        } else if (e.ColumnIndex == LifetimeConstraintColumn.Index) {
          foreach (Agent.KeyConstraint constraint in constraints) {
            if (constraint.Type ==
              Agent.KeyConstraintType.SSH_AGENT_CONSTRAIN_LIFETIME) {
              e.Value = "*";
              break;
            }
          }          
        }
        e.FormattingApplied = true;        
      } else if (e.Value is PublicKeyAlgorithm) {
        PublicKeyAlgorithm algorithm = (PublicKeyAlgorithm)e.Value;
        e.Value = algorithm.GetIdentifierString();
        e.FormattingApplied = true;
      } else if (e.Value is byte[]) {
        byte[] bytes = (byte[])e.Value;
        e.Value = bytes.ToHexString();
        e.FormattingApplied = true;
      }
    }


  }
}
