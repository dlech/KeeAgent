using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using KeePass.Plugins;
using KeeAgent;
using System.Windows.Forms;
using KeePassPluginTestUtil;
using System.Threading;
using IOProtocolExt;
using System.IO;
using System.Reflection;
using KeePassLib.Serialization;
using NUnit.Framework;
using dlech.SshAgentLib;
using System.Security;

namespace KeeAgentTestProject
{
  [TestFixture]
  public class IOProtocolExtTest
  {

    [TestFixtureSetUp()]
    public static void Setup()
    {
      // copy WinSCP to working directory

      string solutionDir = Path.GetFullPath(
        Path.Combine(Environment.CurrentDirectory, "../../.."));
      string winSCPSourceDir = Path.Combine(solutionDir, "IOProtocolExt",
        "WinSCP");

      Assert.IsTrue(File.Exists(Path.Combine(winSCPSourceDir, "WinSCP.exe")));
      Assert.IsTrue(File.Exists(Path.Combine(winSCPSourceDir, "WinSCP.com")));

      Assembly assembly = Assembly.GetAssembly(typeof(KeePass.Program));
      string keepassDir = Path.GetDirectoryName(assembly.Location);

      string winSCPDestDir = Path.Combine(keepassDir, "IOProtocolExt_WinSCP");
      if (Directory.Exists(winSCPDestDir)) {
        Directory.Delete(winSCPDestDir, true);
        int count = 0;
        while (Directory.Exists(winSCPDestDir)) {
          // wait for directory to delete
          count++;
          Assert.IsTrue(count < 20,
            "Failed to delete IOProtocolExt_WinSCP folder.");
          Thread.Sleep(100);
        }
      }
      Directory.CreateDirectory(winSCPDestDir);
      foreach (string filePath in Directory.GetFiles(winSCPSourceDir)) {
        string fileName = Path.GetFileName(filePath);
        string sourceFile = Path.Combine(winSCPSourceDir, fileName);
        string destFile = Path.Combine(winSCPDestDir, fileName);
        File.Copy(sourceFile, destFile);
      }
    }

    [TestFixtureTearDown()]
    public static void TearDown()
    {
      KeePassControl.ExitAll();
    }

    /// <summary>
    /// Tests interaction of KeeAgent with IOProtocolExt
    /// </summary>
    [Test]
    public void TestIOProtocolExt()
    {
      using (KeePassAppDomain testDomain1 = new KeePassAppDomain()) {
        testDomain1.StartKeePass(true, false, 1, true);
        testDomain1.DoCallBack(delegate()
        {
          KeePass.Program.MainForm.Invoke((MethodInvoker)delegate()
          {
            /* initialize plugins */

            KeeAgentExt td1KeeAgentExt = new KeeAgentExt();
            IOProtocolExtExt td1IOProtocolExt = new IOProtocolExtExt();

            IPluginHost td1PluginHost = KeePass.Program.MainForm.PluginHost;
            td1KeeAgentExt.Initialize(td1PluginHost);
            td1IOProtocolExt.Initialize(td1PluginHost);
            KeePass.Program.MainForm.FormClosing += delegate(
              Object source, FormClosingEventArgs args)
            {
              td1KeeAgentExt.Terminate();
              td1IOProtocolExt.Terminate();
            };

            /* load ssh key */
            var keyFileDir = "../../../SshAgentLib/SshAgentLibTests/Resources";
            var keyFilePath = Path.GetFullPath(Path.Combine(keyFileDir, "rsa_with_passphrase"));
            Assert.That(File.Exists(keyFilePath), "Cannot locate key file: " + keyFilePath);

            KeyFormatter.GetPassphraseCallback getPassphraseCallback =
              delegate()
              {
                var passphrase = "passphrase";
                var securePassphrase = new SecureString();
                foreach (char c in passphrase) {
                  securePassphrase.AppendChar(c);
                }
                return securePassphrase;
              };
            td1KeeAgentExt.mAgent.AddKeyFromFile(keyFilePath, getPassphraseCallback);

            /* run test */
            IOConnectionInfo ioConnectionInfo = new IOConnectionInfo();
            ioConnectionInfo.Path = "sftp://satest/test.kdbx";
            ioConnectionInfo.UserName = "tc";
            bool fileExists = IOConnection.FileExists(ioConnectionInfo);
            Assert.IsTrue(fileExists, "Is satest VM running?");



            /* the problem we are checking for is that IOConnection.FileExists
             * does not lock up. Originally, in KeeAgent, WinPagent ran on main
             * thread and caused lock-up here. So, we are looking to see if the
             * test times out or not. */
          });
        });
      }
    }

  }
}