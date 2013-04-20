using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using KeePass.Plugins;
using KeeAgent;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using KeePassPluginDevTools.Control;
using System.IO;

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
    /// Tests if options are saved when keypass is closed and reopened
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
