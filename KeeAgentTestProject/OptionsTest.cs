using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using KeeAgent;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

namespace KeeAgentTestProject
{
  [TestClass]
  public class OptionsTest
  {
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      KeePassControl.ExitAll();
    }

    /// <summary>
    /// Tests if options are saved when keypass is closed and reopened
    /// </summary>
    [TestMethod]
    public void TestOptionsPersistance()
    {
      // used for passing Options objects to/from AppDomain
      const string optionsPropertyName = "KEEAGENT_OPTIONS";

      /* test case options */

      NotificationOptions requestedNotification =
          NotificationOptions.AlwaysAsk;
      bool requestedLoggingEnabled = true;
      string requestedLogFileName =
          Environment.GetEnvironmentVariable("TEMP");
      if (string.IsNullOrEmpty(requestedLogFileName)) {
        requestedLogFileName =
            Environment.GetEnvironmentVariable("TMP");
      }
      Assert.IsNotNull(requestedLogFileName);

      // verify that requested options are not default to ensure a
      // valid test
      Options requestedOptions = new Options();
      Assert.AreNotEqual(requestedOptions.Notification,
          requestedNotification);
      Assert.AreNotEqual(requestedOptions.LoggingEnabled,
          requestedLoggingEnabled);
      Assert.AreNotEqual(requestedOptions.LogFileName,
          requestedLogFileName);

      requestedOptions.Notification = requestedNotification;
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
            td1KeeAgentExt.SaveOptions();
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
        Assert.AreEqual(requestedNotification, actual.Notification);
        Assert.AreEqual(requestedLoggingEnabled, actual.LoggingEnabled);
        Assert.AreEqual(requestedLogFileName, actual.LogFileName);
      }
    }
  }
}
