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
using KeePassLib.Cryptography;
using System.Diagnostics;
using System.Security;

namespace KeeAgent.UI
{
	public partial class PuttyKeyListDialog : Form
	{
		private PpkKeyMap keyMap;

		public PuttyKeyListDialog(PwDatabase database)
		{
			InitializeComponent();

			keyMap = new PpkKeyMap();

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

							SecureString passphrase = null;
							CryptoRandomStream crsRandomSource;
							ProtectedString passphraseFromKeepass = entry.Strings.Get(PwDefs.PasswordField);
							if (passphraseFromKeepass != null) {
								byte[] randomBytes = CryptoRandom.Instance.GetRandomBytes(256);
								crsRandomSource = new CryptoRandomStream(CrsAlgorithm.Salsa20, randomBytes);
								byte[] passphraseBytes = passphraseFromKeepass.ReadXorredString(crsRandomSource);
								crsRandomSource = new CryptoRandomStream(CrsAlgorithm.Salsa20, randomBytes);
								randomBytes = crsRandomSource.GetRandomBytes((uint)passphraseBytes.Length);
								passphrase = new SecureString();
								for (int i = 0; i < passphraseBytes.Length; i++) {
									passphrase.AppendChar((char)(passphraseBytes[i] ^ randomBytes[i]));
								}
								Array.Clear(passphraseBytes, 0, passphraseBytes.Length);
							}

							PpkFile.GetPassphraseCallback getPassphrase = delegate()
							{
								return passphrase;
							};

							PpkFile.WarnOldFileFormatCallback warnUser = delegate()
							{
								// TODO actually warn user
							};



							PpkKey key  = PpkFile.ParseData(bin.Value.ReadData(), getPassphrase, warnUser);

							keyMap.Add(PSUtil.ToHex(key.GetFingerprint()), entry.Uuid);

							puttyKeyDataSet.PuttyKeys.AddPuttyKeysRow(
								key.Algorithm.KeyExchangeAlgorithm,
								key.Algorithm.KeySize,
								PSUtil.ToHex(key.GetFingerprint()),
								key.Comment,
								path,
								bin.Key
							);
						} catch (Exception ex) {
							Debug.Fail(ex.ToString());
						}
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
