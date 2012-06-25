using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using KeeAgent;
using System.Windows.Forms;

namespace KeeAgentTestProject
{
    /// <summary>
    /// This test should be called immedeatly after SetOptionsTest
    /// </summary>
    [TestClass]
    public class LoadOptionsTest
    {
        private static IPluginHost pluginHost;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            pluginHost = KeePassControl.StartKeePass(true, false, 2);
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            KeePassControl.ExitAll();
        }

        [TestMethod]
        public void TestLoadingOptions()
        {
            /* these values should match the requested values in
             * SetOptionsTest.cs
             */
            NotificationOptions expectedNotification =
                NotificationOptions.AlwaysAsk;
            bool expectedLoggingEnabled = true;
            string expectedLogFileName = Environment.GetEnvironmentVariable("TEMP");

            if (string.IsNullOrEmpty(expectedLogFileName)) {
                expectedLogFileName = Environment.GetEnvironmentVariable("TMP");
            }
            Assert.IsNotNull(expectedLogFileName);

            KeeAgentExt target = new KeeAgentExt();
            KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
            {
                target.Initialize(pluginHost);
                Assert.AreEqual(expectedNotification, target.options.Notification);
                Assert.AreEqual(expectedLoggingEnabled, target.options.LoggingEnabled);
                Assert.AreEqual(expectedLogFileName, target.options.LogFileName);
            });
        }
    }
}
