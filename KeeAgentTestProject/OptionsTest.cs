//
//  OptionsTest.cs
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

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using KeeAgent;
using KeePass.Plugins;
using KeePassPluginDevTools.Control;
using NUnit.Framework;

namespace KeeAgentTestProject
{
  [TestFixture]
  public class OptionsTest
  {
    [TestFixtureTearDown()]
    public static void MyClassCleanup()
    {
      KeePassControl.ExitAll();
    }

    /// <summary>
    /// Tests if options are saved when KeePass is closed and reopened
    /// </summary>
    [Test]
    public void TestOptionsPersistance()
    {
      // used for passing Options objects to/from AppDomain
      const string optionsPropertyName = "KEEAGENT_OPTIONS";

      /* test case options */
           
      bool requestedLoggingEnabled = true;
      string requestedLogFileName =
          Environment.GetEnvironmentVariable("TEMP");
      if (string.IsNullOrEmpty(requestedLogFileName)) {
        requestedLogFileName =
            Environment.GetEnvironmentVariable("TMP");
      }
      if (string.IsNullOrEmpty(requestedLogFileName)) {
        requestedLogFileName = "/tmp";
      }
      Assert.That (Directory.Exists (requestedLogFileName));

      // verify that requested options are not default to ensure a
      // valid test
      Options requestedOptions = new Options();
      Assert.AreNotEqual(requestedOptions.LoggingEnabled,
          requestedLoggingEnabled);
      Assert.AreNotEqual(requestedOptions.LogFileName,
          requestedLogFileName);

      requestedOptions.LoggingEnabled = requestedLoggingEnabled;
      requestedOptions.LogFileName = requestedLogFileName;


      /* first instance of KeePass is used to set options to 
       * requested values */

      using (KeePassAppDomain testDomain1 = new KeePassAppDomain()) {
        testDomain1.StartKeePass(true, false, 1, true);
        testDomain1.SetData(optionsPropertyName, requestedOptions);
        testDomain1.DoCallBack(delegate()
        {
          KeePass.Program.MainForm.Invoke((MethodInvoker)delegate()
          {
            KeeAgentExt td1KeeAgentExt = new KeeAgentExt();
            IPluginHost td1PluginHost =
                KeePass.Program.MainForm.PluginHost;
            Options td1RequestedOptions = (Options)AppDomain
                .CurrentDomain.GetData(optionsPropertyName);
            td1KeeAgentExt.Initialize(td1PluginHost);
            td1KeeAgentExt.Options.Notification =
                td1RequestedOptions.Notification;
            td1KeeAgentExt.Options.LoggingEnabled =
                td1RequestedOptions.LoggingEnabled;
            td1KeeAgentExt.Options.LogFileName =
                td1RequestedOptions.LogFileName;
            var extType = td1KeeAgentExt.GetType ();
            var saveGlobalOptionsMethod =
              extType.GetMethod ("SaveGlobalOptions",
                                 BindingFlags.Instance | BindingFlags.NonPublic);
            saveGlobalOptionsMethod.Invoke (td1KeeAgentExt, null);
            td1PluginHost.MainWindow.SaveConfig();
            AppDomain.CurrentDomain.SetData(optionsPropertyName,
                td1KeeAgentExt.Options);
          });
        });
      }

      /* second instance of KeePass reads options to verify that they
       * were saved in the .config file */

      using (KeePassAppDomain testDomain2 = new KeePassAppDomain()) {
        testDomain2.StartKeePass(true, false, 1, false);
        testDomain2.DoCallBack(delegate()
        {
          KeePass.Program.MainForm.Invoke((MethodInvoker)delegate()
          {
            KeeAgentExt td2KeeAgentExt = new KeeAgentExt();
            IPluginHost td2PluginHost =
                KeePass.Program.MainForm.PluginHost;
            td2KeeAgentExt.Initialize(td2PluginHost);
            AppDomain.CurrentDomain.SetData(optionsPropertyName,
                td2KeeAgentExt.Options);
          });
        });
        Options actual = (Options)testDomain2.GetData(optionsPropertyName);
        Assert.AreEqual(requestedLoggingEnabled, actual.LoggingEnabled);
        Assert.AreEqual(requestedLogFileName, actual.LogFileName);
      }
    }
  }
}
