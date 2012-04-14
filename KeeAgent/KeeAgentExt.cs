using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using KeePass.Plugins;
using dlech.PageantSharp;
using KeeAgent.UI;
using KeePassLib;
using KeePassLib.Security;
using System.IO;
using System.Security;
using KeePassLib.Cryptography;
using System.Diagnostics;

namespace KeeAgent
{
	public sealed class KeeAgentExt : Plugin
	{
		internal IPluginHost pluginHost;
		private WinPageant pageant;
		private ToolStripMenuItem keeAgentMenuItem;

		public override bool Initialize(IPluginHost host)
		{
			bool result;

			this.pluginHost = host;

			try {
				this.pageant = new WinPageant(GetKeyList, GetSSH2Key);
				result = true;
			} catch (Exception) {
				InvokeMainWindow(new MethodInvoker(ShowPageantRunningErrorMessage));
				result = false;
			}

			InvokeMainWindow(new MethodInvoker(AddMenuItems));

			return result;
		}

		public override void Terminate()
		{			
			if (this.pageant != null) {
				this.pageant.Dispose();
			}
			RemoveMenuItems();
		}

		private void ShowPageantRunningErrorMessage()
		{
			MessageBox.Show(Translatable.ErrPageantRunning, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void AddMenuItems()
		{
			/* get Tools menu */
			ToolStripMenuItem toolsMenu = this.pluginHost.MainWindow.ToolsMenu;

			/* create parent menu item */
			keeAgentMenuItem = new ToolStripMenuItem();
			keeAgentMenuItem.Text = Translatable.KeeAgentMenuItem;

			if (pageant != null) {
				/* create childern menu items */
				ToolStripMenuItem keeAgentListPuttyKeysMenuItem = new ToolStripMenuItem();
				keeAgentListPuttyKeysMenuItem.Text = Translatable.ShowPuttyKeysMenuItem;
				keeAgentListPuttyKeysMenuItem.ToolTipText = Translatable.ShowPuttyKeysMenuItemToolTip;
				keeAgentListPuttyKeysMenuItem.Click += new EventHandler(keeAgentListPuttyKeysMenuItem_Click);

				/* add children to parent */
				keeAgentMenuItem.DropDownItems.Add(keeAgentListPuttyKeysMenuItem);
			} else {
				keeAgentMenuItem.Enabled = false;
			}

			/* add new items to tools menu */
			toolsMenu.DropDownItems.Add(keeAgentMenuItem);

		}

		private void RemoveMenuItems()
		{
			if (this.pluginHost != null && this.pluginHost.MainWindow != null && 
				this.keeAgentMenuItem != null) {

				/* get Tools menu */
				ToolStripMenuItem toolsMenu = this.pluginHost.MainWindow.ToolsMenu;
				/* remove items from tools menu */
				toolsMenu.DropDownItems.Remove(keeAgentMenuItem);
			}
		}

		private void keeAgentListPuttyKeysMenuItem_Click(object source, EventArgs e)
		{
			KeeAgentKeyListDialog dialog = new KeeAgentKeyListDialog(this);
			DialogResult result = dialog.ShowDialog(pluginHost.MainWindow);
			dialog.Dispose();
		}

		private void InvokeMainWindow(MethodInvoker methodInvoker)
		{
			Form mainWindow = this.pluginHost.MainWindow;

			if (mainWindow.InvokeRequired) {
				mainWindow.Invoke(methodInvoker);
			} else {
				methodInvoker.Invoke();
			}
		}

		internal IEnumerable<KeeAgentKey> GetKeyList()
		{
			List<KeeAgentKey> keyList = new List<KeeAgentKey>();

			if (this.pluginHost != null && this.pluginHost.Database != null) {

				foreach (PwEntry entry in this.pluginHost.Database.RootGroup.GetEntries(true)) {
					foreach (KeyValuePair<string, ProtectedBinary> bin in entry.Binaries) {

						/* handle PuTTY Private Key files */

						if (bin.Key.EndsWith(".ppk")) {
							try {
								SecureString passphrase = null;
								CryptoRandomStream crsRandomSource;
								ProtectedString passphraseFromKeepass = entry.Strings.Get(PwDefs.PasswordField);
								if (passphraseFromKeepass != null) {
									/* use random bytes to protect passphrase in memory */
									byte[] randomBytes = CryptoRandom.Instance.GetRandomBytes(256);
									crsRandomSource = new CryptoRandomStream(CrsAlgorithm.Salsa20, randomBytes);
									byte[] passphraseBytes = passphraseFromKeepass.ReadXorredString(crsRandomSource);
									crsRandomSource = new CryptoRandomStream(CrsAlgorithm.Salsa20, randomBytes);
									randomBytes = crsRandomSource.GetRandomBytes((uint)passphraseBytes.Length);
									passphrase = new SecureString();
									/* convert passphrase from KeePass protected format to .NET protected format */
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
									// we will warn user a different way... won't we???
								};

								PpkKey ppkKey  = PpkFile.ParseData(bin.Value.ReadData(), getPassphrase, warnUser);
								KeeAgentKey key = new KeeAgentKey(ppkKey, entry.Uuid, bin.Key);
								keyList.Add(key);
							} catch (Exception ex) {
								Debug.Fail(ex.ToString());
								// ignore errors - for now
							}
						} // end .ppk file
					}
				}
			}
			return keyList;
		}

		internal PpkKey GetSSH2Key(byte[] fingerprint)
		{

			/* TODO it would probably be better if we cached the fingerprints and mapped them
			 * to the PwEntry Uuid rather than regenerating the full list to get
			 * a single key as we are doing here. 
			 */

			IEnumerable<PpkKey> ppkKeyList = GetKeyList();
			PpkKey result = null;
			foreach (PpkKey ppkKey in ppkKeyList) {
				if (result == null) {
					try {
						byte[] testFingerprint = ppkKey.GetFingerprint();
						if (testFingerprint.Length == fingerprint.Length) {
							bool match = true;
							for (int i = 0; i < testFingerprint.Length; i++) {
								if (testFingerprint[i] != fingerprint[i]) {
									match = false;
									break;
								}
							}
							if (match) {
								result = ppkKey;
							}
						}
					} catch (Exception ex) {
						Debug.Fail(ex.ToString());
					}
				}
				// dispose all keys except for the one match
				if (ppkKey != result) {
					ppkKey.Dispose();
				}
			}

			return result;
		}

	} // class
} // namespace
