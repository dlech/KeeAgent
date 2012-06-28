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

    //Use ClassCleanup to run code after all tests in a class have run
    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      KeePassControl.ExitAll();
    }

    /// <summary>
    ///A test for Initialize and Terminate
    ///</summary>
    [TestMethod()]
    public void InitializeTest()
    {
      const string initalizeResultName = "KEEAGENT_INIT_RESULT";

      KeePassAppDomain testDomain1 = new KeePassAppDomain();
      testDomain1.StartKeePass(true, true, 1, true);
      testDomain1.DoCallBack(delegate()
      {
        IPluginHost td1PluginHost = KeePass.Program.MainForm.PluginHost;
        try {
          KeeAgentExt td1KeeAgentExt = new KeeAgentExt();
          KeePass.Program.MainForm.Invoke((MethodInvoker)delegate()
          {
            bool td1InitalizeResult = td1KeeAgentExt.Initialize(td1PluginHost);
            td1KeeAgentExt.Terminate();
            AppDomain.CurrentDomain.SetData(initalizeResultName,
              td1InitalizeResult);
          });
        } catch (Exception ex) {

        }
      });

      bool expected = true;
      bool actual = (bool)testDomain1.GetData(initalizeResultName);
      Assert.AreEqual(expected, actual);
    }

    /// <summary>
    ///A test for creating and loading plgx
    ///</summary>
    [TestMethod()]
    public void PlgxTest()
    {
      FileInfo assmFile = new FileInfo(
       Assembly.GetExecutingAssembly().Location);
      DirectoryInfo projectDir = new DirectoryInfo(
        Path.Combine(assmFile.Directory.FullName, @"..\..\..\KeeAgent"));

      /* copy required dll */
      string pageantSharpDllSource =
        Path.Combine(assmFile.Directory.FullName, "PageantSharp.dll");
      string pageantSharpDllDest =
        Path.Combine(projectDir.FullName, "PageantSharp.dll");
      File.Copy(pageantSharpDllSource, pageantSharpDllDest);

      // use try-catch-finally to make sure dll is deleted when done
      try {
        /* create .plgx file */
        string plgxFilePath =
          Path.Combine(projectDir.Parent.FullName, "KeeAgent.plgx");
        File.Delete(plgxFilePath);
        PlgxBuildOptions buildOptions = new PlgxBuildOptions();
        buildOptions.projectPath = projectDir.FullName;
        buildOptions.dotnetVersion = "4.0";
        buildOptions.os = "Windows";
        KeePassControl.CreatePlgx(buildOptions);

        Assert.IsTrue(File.Exists(plgxFilePath));


        KeePassAppDomain testDomain1 = new KeePassAppDomain();
        testDomain1.StartKeePass(true, true, 1, true);
        testDomain1.LoadPlgx(plgxFilePath);

        testDomain1.DoCallBack(delegate()
        {
          // TODO Is there anything we can do here to test that LoadPlgx was
          // successful? Right now, KeePass shows an error dialog if it was
          // not, so the user should know that it failed even though the test
          // results will show that it passed.
        });
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
      } finally {
        File.Delete(pageantSharpDllDest);
      }
    }
  }
}
