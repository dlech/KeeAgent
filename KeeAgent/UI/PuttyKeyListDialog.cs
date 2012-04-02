using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeePassLib;
using KeePassLib.Collections;
using KeePassLib.Security;
using dlech.PageantSharp;

namespace KeeAgent.UI
{
	public partial class PuttyKeyListDialog : Form
	{
		public PuttyKeyListDialog(PwDatabase database)
		{
			InitializeComponent();

			foreach (PwEntry entry in database.RootGroup.GetEntries(true)) {
				foreach (KeyValuePair<string, ProtectedBinary> bin in entry.Binaries) {
					if (bin.Key.EndsWith(".ppk")) {
						try {
							PpkFile file = new PpkFile(bin.Value.ReadData());
							puttyKeyDataSet.PuttyKeys.AddPuttyKeysRow(
								file.KeyType,
								file.Encryption,
								file.Comment,
								file.PublicKey,
								file.PrivateKey,
								file.PrivateMAC
							);
						} catch (Exception) { }
					}
				}
			}
		}

		private void PuttyKeyListDialog_Shown(object sender, EventArgs e)
		{
			// borrow icon from owner
			if (this.Owner != null) {
				this.Icon = this.Owner.Icon;
			}
		}
	}
}
