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
using System.Diagnostics;

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
    public static void Cleanup()
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

      using (KeePassAppDomain testDomain1 = new KeePassAppDomain()) {
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
            // TODO do we want to pass this exception back to test?
          }
        });

        bool expected = true;
        bool actual = (bool)testDomain1.GetData(initalizeResultName);
        Assert.AreEqual(expected, actual);
      }
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
      string tempFile = Path.GetTempFileName();

      /* copy required files */

      string pageantSharpDllSource =
        Path.Combine(assmFile.Directory.FullName, "PageantSharp.dll");
      string pageantSharpDllDest =
        Path.Combine(projectDir.FullName, "PageantSharp.dll");
      File.Delete(pageantSharpDllDest);
      File.Copy(pageantSharpDllSource, pageantSharpDllDest);

      string preBuildExeSource =
        Path.Combine(projectDir.FullName,
        @"..\PreBuild\bin\Debug\PreBuild.exe");
      string preBuildExeDest =
        Path.Combine(projectDir.FullName, "PreBuild.exe");
      File.Delete(preBuildExeDest);
      File.Copy(preBuildExeSource, preBuildExeDest);

      string plgxFilePath =
          Path.Combine(projectDir.Parent.FullName, "KeeAgent.plgx");

      // use try-catch-finally to make sure manually copied files are deleted
      // when done
      try {
        /* create .plgx file */

        File.Delete(plgxFilePath);

        PlgxBuildOptions buildOptions = new PlgxBuildOptions();
        buildOptions.projectPath = projectDir.FullName;
        buildOptions.dotnetVersion = "4.0";
        buildOptions.os = "Windows";
        buildOptions.preBuild =
          "\"cmd /c \"\"\"{PLGX_TEMP_DIR}PreBuild.exe\"\"\" {PLGX_TEMP_DIR}" +
          " > " + tempFile + "\"";
        KeePassControl.CreatePlgx(buildOptions);
                
        Assert.IsTrue(File.Exists(plgxFilePath),
          ".plgx file was not created");
        
        using (KeePassAppDomain testDomain1 = new KeePassAppDomain()) {
          testDomain1.StartKeePass(true, true, 1, true);
          testDomain1.LoadPlgx(plgxFilePath);

          testDomain1.DoCallBack(delegate()
          {            
            // TODO Is there anything we can do here to test that LoadPlgx was
            // successful? Right now, KeePass shows an error dialog if it was
            // not, so the user should know that it failed even though the test
            // results will show that it passed.
          });
        }
      } catch (Exception ex) {
        Assert.Fail(ex.ToString());
      } finally {
        File.Delete(pageantSharpDllDest);
        File.Delete(preBuildExeDest);
        File.Delete(plgxFilePath);
      }

      /* check to make sure prebuild worked correctly */
      string versionLine = File.ReadAllLines(tempFile)[0];
      Assert.IsTrue(versionLine.Contains(PwDefs.VersionString),
        "PreBuild did not detect correct KeePass version.\n" + 
        "Saw: " + versionLine + "\n" +
        "Expected: " + PwDefs.VersionString);
      File.Delete(tempFile);
    }
  }
}
