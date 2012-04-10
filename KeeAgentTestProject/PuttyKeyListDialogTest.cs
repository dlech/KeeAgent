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
			pluginHost = KeePassControl.StartKeePass();

			PwEntry withPassEntry = new PwEntry(true, true);
			withPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "with-passphrase"));
			withPassEntry.Binaries.Set("withPass.ppk", new ProtectedBinary(true, Resources.withPassphrase_ppk));
			withPassEntry.Strings.Set(PwDefs.PasswordField, new ProtectedString(true, "KeeAgent"));
			PwEntry withoutPassEntry = new PwEntry(true, true);
			withoutPassEntry.Strings.Set(PwDefs.TitleField, new ProtectedString(true, "without-passphrase"));
			withoutPassEntry.Binaries.Set("withoutPass.ppk", new ProtectedBinary(true, Resources.withoutPassphrase_ppk));

			PwGroup puttyGroup = new PwGroup();
			puttyGroup.Name = "Putty";
			puttyGroup.AddEntry(withPassEntry, true);
			puttyGroup.AddEntry(withoutPassEntry, true);

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
			keeAgent.Initialize(pluginHost);
			MessageBox.Show("Click OK when done");
			keeAgent.Terminate();
		}
	}
}
