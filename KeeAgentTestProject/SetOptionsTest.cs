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
    public class SetOptionsTest
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

        [TestMethod]
        public void TestSettingOptions()
        {
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

            AppDomain testDomain = AppDomain.CreateDomain("TestSettingOptions Domain", 
                AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);
            Thread testDomainThread = new Thread((ThreadStart)delegate()
            {
                testDomain.ExecuteAssembly(Assembly.GetAssembly(typeof(KeePass.Program)).Location);
            });
            testDomainThread.SetApartmentState(ApartmentState.STA);
            testDomainThread.Start();            
            testDomain.DoCallBack(delegate()
            {
                while (KeePass.Program.MainForm == null || !KeePass.Program.MainForm.Visible) {
                    
                }
                //Thread.Sleep(2000);
                KeeAgentExt target2 = new KeeAgentExt();
                KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
                {
                    // TODO figure out how to pass data back to test
                });
            });
            
            
        }
    }
}
