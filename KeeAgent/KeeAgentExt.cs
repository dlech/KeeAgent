using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using KeePass.Plugins;
using dlech.PageantSharp;
using KeeAgent.UI;

namespace KeeAgent
{
	public sealed class KeeAgentExt : Plugin
	{
		private IPluginHost pluginHost;
		private PageantWindow pageant;

		public override bool Initialize(IPluginHost host)
		{
			try {
				this.pageant = new PageantWindow();
			} catch (Exception ex) {
				// TODO show error message
				return false;
			}
			this.pluginHost = host;

			InvokeMainWindow(new MethodInvoker(AddMenuItems));			

			return true;
		}

		public override void Terminate()
		{
			if (this.pageant != null) {
				this.pageant.Dispose();
			}
		}

		private void AddMenuItems()
		{
			/* create childern menu items */
			ToolStripMenuItem keeAgentListPuttyKeysMenuItem = new ToolStripMenuItem();
			keeAgentListPuttyKeysMenuItem.Text = Translatable.ShowPuttyKeysMenuItem;
			keeAgentListPuttyKeysMenuItem.ToolTipText = Translatable.ShowPuttyKeysMenuItemToolTip;
			keeAgentListPuttyKeysMenuItem.Click += new EventHandler(keeAgentListPuttyKeysMenuItem_Click);

			/* create parent menu item */
			ToolStripMenuItem keeAgentMenuItem = new ToolStripMenuItem();
			keeAgentMenuItem.Text = Translatable.KeeAgentMenuItem;
			keeAgentMenuItem.DropDownItems.Add(keeAgentListPuttyKeysMenuItem);

			/* add new items to tools menu */
			ToolStripMenuItem toolsMenu = this.pluginHost.MainWindow.ToolsMenu;
			toolsMenu.DropDownItems.Add(keeAgentMenuItem);
		}

		private void keeAgentListPuttyKeysMenuItem_Click(object source, EventArgs e)
		{
			PuttyKeyListDialog dialog = new PuttyKeyListDialog(this.pluginHost.Database);
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
	}
}
