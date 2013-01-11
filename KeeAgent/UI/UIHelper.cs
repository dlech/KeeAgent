using System.Windows.Forms;
using KeePass.Plugins;
using System;
using System.Runtime.InteropServices;

namespace KeeAgent.UI
{
	public class UIHelper
	{
		private IPluginHost mPluginHost;
        
		public UIHelper(IPluginHost pluginHost)
		{
			this.mPluginHost = pluginHost;
		}

		/// <summary>
		/// Shows message using the preferred notification method.
		/// Currently, this is only via the notification icon.
		/// </summary>
		/// <param name="aMessage">The message to display</param>
		public void ShowBalloonNotification(string aMessage) {
			MethodInvoker invoker = delegate() {
				mPluginHost.MainWindow.MainNotifyIcon.ShowBalloonTip(
					5000, Translatable.KeeAgent,
					aMessage, ToolTipIcon.Info);
			};
			InvokeMainWindow(invoker, true);
		}

		/// <summary>
		/// Invokes a method on the main window thread
		/// </summary>
		/// <param name="aInvoker">the method to invoke</param>
    /// <param name="aAsync">set to true to invoke asynchronously</param>
		private void InvokeMainWindow(MethodInvoker aInvoker, bool aAsync) {
			if (this.mPluginHost.MainWindow.InvokeRequired) {
        if (aAsync) {
          mPluginHost.MainWindow.BeginInvoke(aInvoker);
        } else {
          mPluginHost.MainWindow.Invoke(aInvoker);
        }
			} else {
				aInvoker.Invoke();
			}
		}
	}
}
