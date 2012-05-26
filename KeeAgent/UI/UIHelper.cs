using System.Windows.Forms;
using KeePass.Plugins;

namespace KeeAgent.UI
{
	public class UIHelper
	{
		private IPluginHost pluginHost;

		public UIHelper(IPluginHost pluginHost)
		{
			this.pluginHost = pluginHost;
		}


		/// <summary>
		/// Shows message using the preferred notification method.
		/// Currently, this is only via the notification icon.
		/// </summary>
		/// <param name="message">The message to display</param>
		public void ShowNotification(string message) {
			MethodInvoker invoker = delegate() {
				this.pluginHost.MainWindow.MainNotifyIcon.ShowBalloonTip(
					5000, Translatable.KeeAgent,
					message, ToolTipIcon.Info);
			};
			InvokeMainWindow(invoker);
		}

		/// <summary>
		/// Invokes a method on the main window thread
		/// </summary>
		/// <param name="invoker">the method to invoke</param>
		private void InvokeMainWindow(MethodInvoker invoker) {
			if (this.pluginHost.MainWindow.InvokeRequired) {
				this.pluginHost.MainWindow.Invoke(invoker);
			} else {
				invoker.Invoke();
			}
		}
	}
}
