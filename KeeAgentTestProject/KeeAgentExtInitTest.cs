using KeeAgent;
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
using NUnit.Framework;
using System.Collections.Generic;

namespace KeeAgentTestProject
{
  /// <summary>
  ///This is a test class for KeeAgentExtTest and is intended
  ///to contain all KeeAgentExtTest Unit Tests
  ///</summary>
  [TestFixture()]
  public class KeeAgentExtInitTest
  {

    //Use ClassCleanup to run code after all tests in a class have run
    [TestFixtureTearDown()]
    public static void Cleanup()
    {
      KeePassControl.ExitAll();
    }

    /// <summary>
    ///A test for Initialize and Terminate
    ///</summary>
    [Test()]
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
          } catch (Exception) {
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
    [Test()]
    public void PlgxTest()
    {
      FileInfo assmFile = new FileInfo(
       Assembly.GetExecutingAssembly().Location);
      DirectoryInfo projectDir = new DirectoryInfo(
        Path.Combine(assmFile.Directory.FullName, @"..\..\..\KeeAgent"));
      string tempFile = Path.GetTempFileName();

      var deleteFileList = new List<string>();

      /* copy dll files */

      foreach (var file in Directory.GetFiles(assmFile.Directory.FullName)) {
        var info = new FileInfo(file);
        if (!Path.GetFileNameWithoutExtension(info.Name)
          .Equals("KeePass", StringComparison.OrdinalIgnoreCase) &&
          info.Extension.Equals(".dll", StringComparison.OrdinalIgnoreCase)) {
                    
          string destFile = Path.Combine(projectDir.FullName, info.Name);
          File.Delete(destFile);
          File.Copy(file, destFile);
          deleteFileList.Add(destFile);
        }
      }
      string preBuildExeSource =
        Path.Combine(projectDir.FullName,
        @"..\PreBuild\bin\Debug\PreBuild.exe");
      string preBuildExeDest =
        Path.Combine(projectDir.FullName, "PreBuild.exe");
      File.Delete(preBuildExeDest);
      File.Copy(preBuildExeSource, preBuildExeDest);
      deleteFileList.Add(preBuildExeDest);

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

        Assert.IsTrue(File.Exists(plgxFilePath), ".plgx file was not created");
        deleteFileList.Add(plgxFilePath);

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
        foreach (var file in deleteFileList) {
          File.Delete(file);
        }
      }

      /* check to make sure prebuild worked correctly */
      string versionLine = File.ReadAllLines(tempFile)[0];
      string expectedVersion = PwDefs.VersionString;
      int secondDot = expectedVersion.IndexOf(".", 3);
      if (secondDot > 0) {
        expectedVersion = expectedVersion.Substring(0, secondDot);
      }
      Assert.IsTrue(versionLine.Contains(expectedVersion),
        "PreBuild did not detect correct KeePass version.\n" +
        "Saw: " + versionLine + "\n" +
        "Expected: " + expectedVersion);
      File.Delete(tempFile);
    }
  }
}
