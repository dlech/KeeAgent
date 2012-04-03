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
using System.IO;

namespace KeeAgent.UI
{
	public partial class PuttyKeyListDialog : Form
	{
		private const string macSeparator = ":";

		public PuttyKeyListDialog(PwDatabase database)
		{
			InitializeComponent();

			foreach (PwEntry entry in database.RootGroup.GetEntries(true)) {
				foreach (KeyValuePair<string, ProtectedBinary> bin in entry.Binaries) {
					if (bin.Key.EndsWith(".ppk")) {
						try {
							string path = entry.Strings.Get(PwDefs.TitleField).ReadString();
							PwGroup parentGroup = entry.ParentGroup;
							while (parentGroup != null) {
								path = Path.Combine(parentGroup.Name, path);
								parentGroup = parentGroup.ParentGroup;
							}
							PpkFile file = new PpkFile(bin.Value.ReadData());
							puttyKeyDataSet.PuttyKeys.AddPuttyKeysRow(
								file.PublicKeyAlgorithm,
								file.PrivateKeyAlgorithm,
								file.Comment,
								file.PublicKey,
								file.PrivateKey,
								file.PrivateMAC,
								path,
								bin.Key
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

		private void keyDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
		{
			if (e.ColumnIndex == PrivateMAC.Index) {
				if (typeof(string).IsInstanceOfType(e.Value)) {
					string valueString = (string)e.Value;
					e.Value = KeeAgentUtil.FormatMAC(valueString);
				}
			}
			if (e.ColumnIndex == Size.Index) {
				if (typeof(string).IsInstanceOfType(e.Value)) {
					string valueString = (string)e.Value;
					e.Value = valueString.Length;
				}
			}
		}

		
	}
}
