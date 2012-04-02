using KeeAgent.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KeePassLib;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using KeeAgent;
using System.Windows.Forms;

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
