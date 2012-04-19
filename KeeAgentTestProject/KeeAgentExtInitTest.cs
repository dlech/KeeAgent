using KeeAgent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KeePass.Plugins;
using KeePassPluginTestUtil;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Threading;

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
				bool expected = true;
				bool actual;
				actual = target.Initialize(host);
				target.Terminate();
				Assert.AreEqual(expected, actual);
			} catch (Exception ex) {
				Assert.Fail(ex.ToString());
			}
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
			KeePassControl.CreatePlgx(projectDir.FullName, null, "4.0", "Windows", null, null, null);
			Assert.IsTrue(File.Exists(plgxFilePath));

			// TODO loading plgx this way (below) does not work
			//MessageBox.Show("start keepass");
			//KeePassControl.LoadPlgx(plgxFilePath);
		}
	}
}
