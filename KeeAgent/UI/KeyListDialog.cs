using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using dlech.PageantSharp;
using KeePassLib;
using Org.BouncyCastle.Crypto.Parameters;
using System.ComponentModel;

namespace KeeAgent.UI
{
  public partial class KeyListDialog : Form
  {
    public KeyListDialog(KeeAgentExt aExt)
    {
      InitializeComponent();

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
            algorithm = OpenSsh.PublicKeyAlgorithms.ssh_rsa;
          } else if (key.CipherKeyPair.Public is DsaPublicKeyParameters) {
            algorithm = OpenSsh.PublicKeyAlgorithms.ssh_dss;
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

    private void PuttyKeyListDialog_Shown(object aSender, EventArgs aEvent)
    {
      // borrow icon from owner
      if (Owner != null) {
        Icon = Owner.Icon;
      }
    }
    
  }
}
