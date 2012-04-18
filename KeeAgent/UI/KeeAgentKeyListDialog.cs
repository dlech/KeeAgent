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

namespace KeeAgent.UI
{
	public partial class KeeAgentKeyListDialog : Form
	{
		public KeeAgentKeyListDialog(KeeAgentExt ext)
		{
			InitializeComponent();

			IEnumerable<KeeAgentKey> keyList = ext.GetKeeAgentKeyList();

			foreach (KeeAgentKey key in keyList) {
				PwEntry entry = ext.pluginHost.Database.RootGroup.FindEntry(key.Uuid, true);
				if (entry != null) {

					/* build group path string */
					string path = entry.Strings.Get(PwDefs.TitleField).ReadString();
					PwGroup parentGroup = entry.ParentGroup;
					while (parentGroup != null) {
						path = Path.Combine(parentGroup.Name, path);
						parentGroup = parentGroup.ParentGroup;
					}

					/* get fingerprint */
					string fingerprint;
					try {
						fingerprint = PSUtil.ToHex(key.GetFingerprint());
					} catch (Exception) {
						fingerprint = string.Empty;
					}

					/* add info to data grid view */
					keyDataSet.Keys.AddKeysRow(
						key.Algorithm.KeyExchangeAlgorithm,
						key.Algorithm.KeySize,
						fingerprint,
						key.Comment,
						path,
						key.Filename
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
	}
}
