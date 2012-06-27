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
        private static IPluginHost pluginHost;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            pluginHost = KeePassControl.StartKeePass(true, true, 2);
        }

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
            const string optionsPropertyName = "KEEAGENT_OPTIONS";

            /* these values should all be different from the default values */
            NotificationOptions requestedNotification =
                NotificationOptions.AlwaysAsk;
            bool requestedLoggingEnabled = true;
            string requestedLogFileName = Environment.GetEnvironmentVariable("TEMP");
            if (string.IsNullOrEmpty(requestedLogFileName)) {
                requestedLogFileName = Environment.GetEnvironmentVariable("TMP");
            }
            Assert.IsNotNull(requestedLogFileName);

            KeeAgentExt target = new KeeAgentExt();
            KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
            {
                target.Initialize(pluginHost);
                Assert.AreNotEqual(requestedNotification, target.options.Notification);
                target.options.Notification = requestedNotification;
                Assert.AreNotEqual(requestedLoggingEnabled, target.options.LoggingEnabled);
                target.options.LoggingEnabled = requestedLoggingEnabled;
                Assert.AreNotEqual(requestedLogFileName, target.options.LogFileName);
                target.options.LogFileName = requestedLogFileName;
                target.saveOptions();
                pluginHost.MainWindow.SaveConfig();
            });
            KeePassControl.ExitAll();

            KeePassAppDomain testDomain = new KeePassAppDomain();
            testDomain.StartKeePass(true, false, 1, false);
            // TODO get this working
            testDomain.InvokeMainForm(delegate
            {
                //KeeAgentExt target2 = new KeeAgentExt();
                //IPluginHost pluginHost2 = KeePass.Program.MainForm.PluginHost;
                //target2.Initialize(pluginHost2);
                //AppDomain.CurrentDomain.SetData(optionsPropertyName, target2.options);
            });            
            Options actual = (Options)testDomain.GetData(optionsPropertyName);
            Assert.AreEqual(requestedNotification, actual.Notification);
            Assert.AreEqual(requestedLoggingEnabled, actual.LoggingEnabled);
            Assert.AreEqual(requestedLogFileName, actual.LogFileName);
        }
    }
}
