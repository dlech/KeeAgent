using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using dlech.PageantSharp;
using KeeAgent.Properties;
using KeeAgent.UI;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Cryptography;
using KeePassLib.Security;
using KeePassLib.Utility;

namespace KeeAgent
{
    public sealed class KeeAgentExt : Plugin
    {
        internal IPluginHost pluginHost;
        private WinPageant pageant;
        private ToolStripMenuItem keeAgentMenuItem;
        private UIHelper uiHelper;

        public override bool Initialize(IPluginHost host)
        {
            bool result;

            this.pluginHost = host;
            this.uiHelper = new UIHelper(this.pluginHost);

            try {
                // TODO check OS - currently only works on Windows
                this.pageant = new WinPageant(GetPpkKeyList, GetSSH2Key);
                result = true;
            } catch (Exception) {
                ShowPageantRunningErrorMessage();
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
            InvokeMainWindow(new MethodInvoker(RemoveMenuItems));
        }

        public override Image SmallIcon
        {
            get { return Resources.KeeAgentIcon; }
        }

        /// <summary>
        /// Returns url for automatic updating of plugin
        /// </summary> 
        public new string UpdateUrl //using new instead of override for KP 2.17 compatibility
        {
            get { return "http://updates.lechnology.com/KeePassPlugins"; }
        }


        private void ShowPageantRunningErrorMessage()
        {
            MessageService.ShowWarning(new object[] { Translatable.ErrPageantRunning });
        }

        private void AddMenuItems()
        {
            /* get Tools menu */
            ToolStripMenuItem toolsMenu = this.pluginHost.MainWindow.ToolsMenu;

            /* create parent menu item */
            keeAgentMenuItem = new ToolStripMenuItem();
            keeAgentMenuItem.Text = Translatable.KeeAgent;

            if (pageant != null) {
                /* create children menu items */
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
            KeyListDialog dialog = new KeyListDialog(this);
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

        internal IEnumerable<PpkKey> GetPpkKeyList()
        {
            return (IEnumerable<PpkKey>)GetKeeAgentKeyList();
        }

        internal IEnumerable<KeeAgentKey> GetKeeAgentKeyList()
        {
            pluginHost.MainWindow.NotifyUserActivity();

            List<KeeAgentKey> keyList = new List<KeeAgentKey>();

            if (this.pluginHost != null && this.pluginHost.Database != null) {

                foreach (PwEntry entry in this.pluginHost.Database.RootGroup.GetEntries(true)) {
                    foreach (KeyValuePair<string, ProtectedBinary> bin in entry.Binaries) {

                        /* handle PuTTY Private Key files */

                        if (bin.Key.EndsWith(".ppk")) {
                            try {
                                SecureString passphrase = new SecureString();
                                byte[] passphraseBytes = entry.Strings.Get(PwDefs.PasswordField).ReadUtf8();                                 
                                /* convert passphrase from KeePass protected format to .NET protected format */
                                for (int i = 0; i < passphraseBytes.Length; i++) {
                                    passphrase.AppendChar((char)(passphraseBytes[i]));
                                }
                                Array.Clear(passphraseBytes, 0, passphraseBytes.Length);

                                PpkFile.GetPassphraseCallback getPassphrase = delegate()
                                {
                                    return passphrase;
                                };

                                PpkFile.WarnOldFileFormatCallback warnUser = delegate()
                                {
                                    // we will warn user a different way... won't we???
                                };

                                PpkKey ppkKey = PpkFile.ParseData(bin.Value.ReadData(), getPassphrase, warnUser);
                                KeeAgentKey key = new KeeAgentKey(ppkKey, entry.Uuid, bin.Key);
                                keyList.Add(key);
                            } catch (Exception ex) {
                                MessageService.ShowWarning("Error while loading key from ", entry.Strings.Get(PwDefs.TitleField).ReadString(), ex.ToString());
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
            pluginHost.MainWindow.NotifyUserActivity();

            this.uiHelper.ShowNotification(Translatable.NotifyKeyFetched);

            /* TODO it would probably be better if we cached the fingerprints and mapped them
             * to the PwEntry Uuid rather than regenerating the full list to get
             * a single key as we are doing here. 
             */

            IEnumerable<PpkKey> ppkKeyList = GetPpkKeyList();
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
