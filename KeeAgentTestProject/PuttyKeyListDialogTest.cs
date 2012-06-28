using KeeAgent.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KeePassLib;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using KeeAgent;
using System.Windows.Forms;
using KeeAgentTestProject.Properties;
using KeePassLib.Security;
using System.Security.Cryptography;
using System.Threading;

namespace KeeAgentTestProject
{


    /// <summary>
    ///This is a test class for PuttyKeyListDialogTest and is intended
    ///to contain all PuttyKeyListDialogTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PuttyKeyListDialogTest
    {
        private static IPluginHost pluginHost;

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            pluginHost = KeePassControl.StartKeePass(true, true, 2);

            PwEntry withPassEntry = new PwEntry(true, true);
            withPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "with-passphrase"));
            withPassEntry.Binaries.Set("withPass.ppk", new ProtectedBinary(true, Resources.withPassphrase_ppk));
            withPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));

            PwEntry withBadPassEntry = new PwEntry(true, true);
            withBadPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "with-bad-passphrase"));
            withBadPassEntry.Binaries.Set("withBadPass.ppk", new ProtectedBinary(true, Resources.withPassphrase_ppk));
            withBadPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "BadPass"));

            PwEntry withoutPassEntry = new PwEntry(true, true);
            withoutPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "without-passphrase"));
            withoutPassEntry.Binaries.Set("withoutPass.ppk", new ProtectedBinary(true, Resources.withoutPassphrase_ppk));

            PwEntry dsaPassEntry = new PwEntry(true, true);
            dsaPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "dsa-with-passphrase"));
            dsaPassEntry.Binaries.Set("dsaWithPass.ppk", new ProtectedBinary(true, Resources.dsa_ppk));
            dsaPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));

            PwEntry nonStandardLengthPassEntry = new PwEntry(true, true);
            nonStandardLengthPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "4095-bits"));
            nonStandardLengthPassEntry.Binaries.Set("4095-bits.ppk", new ProtectedBinary(true, Resources._4095_bits_ppk));
            nonStandardLengthPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));

            PwGroup rsaGroup = new PwGroup(true, true, "RSA", PwIcon.Key);
            rsaGroup.AddEntry(withPassEntry, true);
            rsaGroup.AddEntry(withBadPassEntry, true);
            rsaGroup.AddEntry(withoutPassEntry, true);

            PwGroup dsaGroup = new PwGroup(true, true, "DSA", PwIcon.Key);
            dsaGroup.AddEntry(dsaPassEntry, true);
            dsaGroup.AddEntry(nonStandardLengthPassEntry, true);

            PwGroup puttyGroup = new PwGroup(true, true, "Putty", PwIcon.Key);
            puttyGroup.AddGroup(rsaGroup, true);
            puttyGroup.AddGroup(dsaGroup, true);

            pluginHost.Database.RootGroup.AddGroup(puttyGroup, true);

            pluginHost.MainWindow.Invoke(new MethodInvoker(delegate()
            {
                pluginHost.MainWindow.UpdateUI(false, null, true, puttyGroup, true, puttyGroup, false);
            }
            ));
        }

        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            KeePassControl.ExitAll();
        }

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
        ///A test for PuttyKeyListDialog 
        ///</summary>
        [TestMethod()]
        public void PuttyKeyListDialogGeneralTest()
        {
            Plugin keeAgent = new KeeAgentExt();
            KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
            {
                keeAgent.Initialize(pluginHost);
            });

            while (KeePass.Program.MainForm != null &&
                KeePass.Program.MainForm.Visible == true) {
                Thread.Sleep(500);
            }

            KeePassControl.InvokeMainWindow((MethodInvoker)delegate()
            {
                keeAgent.Terminate();
            });
        }
    }
}
