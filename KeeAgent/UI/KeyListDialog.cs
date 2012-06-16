using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using dlech.PageantSharp;
using System.IO;
using KeePassLib.Cryptography;
using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Parameters;

namespace KeeAgent.UI
{
	public partial class KeyListDialog : Form
	{
		public KeyListDialog(KeeAgentExt ext)
		{
			InitializeComponent();

			IEnumerable<KeeAgentKey> keyList = ext.GetKeeAgentKeyList();

			foreach (KeeAgentKey key in keyList) {
                PwEntry entry = null;
                List<PwDatabase> databases = ext.pluginHost.MainWindow.DocumentManager.GetOpenDatabases();
                foreach (PwDatabase database in databases) {
                    // make sure we are looking in the right database
                    if (database.IOConnectionInfo.Path == key.DbPath) {
                        entry = database.RootGroup.FindEntry(key.Uuid, true);
                        break;
                    }
                }
				if (entry != null) {

					/* get group path */
                    string groupPath = entry.ParentGroup.GetFullPath();					

                    /* get entry title */
                    string entryTitle = entry.Strings.Get(PwDefs.TitleField).ReadString();

					/* get fingerprint */
					string fingerprint;
					try {
						fingerprint = PSUtil.ToHex(key.GetFingerprint());
					} catch (Exception) {
						fingerprint = string.Empty;
					}

					string algorithm = null;
                    if (key.KeyParameters.Public is RsaKeyParameters) {
						algorithm = PpkFile.PublicKeyAlgorithms.ssh_rsa;
					}
					if (key.KeyParameters.Public is DsaPublicKeyParameters) {
						algorithm = PpkFile.PublicKeyAlgorithms.ssh_dss;
					}

					/* add info to data grid view */
					keyDataSet.Keys.AddKeysRow(
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

		private void PuttyKeyListDialog_Shown(object sender, EventArgs e)
		{
			// borrow icon from owner
			if (this.Owner != null) {
				this.Icon = this.Owner.Icon;
			}
		}

        private void keyDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
	}
}
