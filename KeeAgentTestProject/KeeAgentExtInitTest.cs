using KeeAgent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using KeePass.UI;
using KeePass.Forms;
using KeePassLib.Cryptography;
using KeePassLib.Utility;
using KeePassLib;
using KeePassLib.Serialization;
using KeePass.App.Configuration;
using KeePass.App;
using KeePass.Ecas;
using KeePass.Util;

namespace KeeAgentTestProject
{


    /// <summary>
    ///This is a test class for KeeAgentExtTest and is intended
    ///to contain all KeeAgentExtTest Unit Tests
    ///</summary>
    [TestClass()]
    public class KeeAgentExtInitTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void InitializeTest()
        {
            IPluginHost host = KeePassControl.StartKeePass();
            try {
                KeeAgentExt target = new KeeAgentExt();                
                KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
                {
                    bool expected = true;
                    bool actual;
                    actual = target.Initialize(host);
                    target.Terminate();
                    Assert.AreEqual(expected, actual);
                });                
            } catch (Exception ex) {
                Assert.Fail(ex.ToString());
            }
        }
               

        /// <summary>
        ///A test for persistance of options
        ///</summary>
        [TestMethod()]
        public void OptionsPersistanceTest()
        {
            /* expected values */
            NotificationOptions expectedNotification = NotificationOptions.AlwaysAsk;
            bool expectedLoggingEnabled = true;
            string expectedLogFileName = @"C:\TEMP";


            IPluginHost host = KeePassControl.StartKeePass();
            KeeAgentExt target = new KeeAgentExt();
            KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
            {
                target.Initialize(host);
                target.options.Notification = expectedNotification;
                target.options.LoggingEnabled = expectedLoggingEnabled;
                target.options.LogFileName = expectedLogFileName;
                target.saveOptions();
                host.MainWindow.SaveConfig();            
            });
            host = KeePassControl.StartKeePass(true, false, 1);
            KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
            {
                target.Initialize(host);
            });

            Assert.AreEqual(expectedNotification, target.options.Notification);
            Assert.AreEqual(expectedLoggingEnabled, target.options.LoggingEnabled);
            Assert.AreEqual(expectedLogFileName, target.options.LogFileName);

            KeePassControl.ExitAll();
        }

        /// <summary>
        ///A test for creating and loading plgx
        ///</summary>
        [TestMethod()]
        public void PlgxTest()
        {
            /* create .plgx file */
            FileInfo assmFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo projectDir = new DirectoryInfo(Path.Combine(assmFile.Directory.FullName, @"..\..\..\KeeAgent"));
            string plgxFilePath = Path.Combine(projectDir.Parent.FullName, "KeeAgent.plgx");
            File.Delete(plgxFilePath);
            PlgxBuildOptions buildOptions = new PlgxBuildOptions();
            buildOptions.projectPath = projectDir.FullName;
            buildOptions.dotnetVersion = "4.0";
            buildOptions.os = "Windows";
            KeePassControl.CreatePlgx(buildOptions);
            Assert.IsTrue(File.Exists(plgxFilePath));

            // TODO loading plgx this way (below) does not work
            //MessageBox.Show("start keepass");
            //KeePassControl.LoadPlgx(plgxFilePath);
        }
    }
}
