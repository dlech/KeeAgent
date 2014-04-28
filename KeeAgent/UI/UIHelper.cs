//
//  UIHelper.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2013  David Lechner
//
//  This program is free software; you can redistribute it and/or
//  modify it under the terms of the GNU General Public License
//  as published by the Free Software Foundation; either version 2
//  of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, see <http://www.gnu.org/licenses>

using System.Windows.Forms;
using KeePass.Plugins;

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
        if (mPluginHost.MainWindow.MainNotifyIcon != null) {
          mPluginHost.MainWindow.MainNotifyIcon.ShowBalloonTip(
            5000, Translatable.KeeAgent,
            aMessage, ToolTipIcon.Info);
        }
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
