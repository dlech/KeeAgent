using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KeePass.Plugins;
using KeeAgent;
using System.Windows.Forms;
using KeePassPluginTestUtil;
using System.Threading;
using IOProtocolExt;
using System.IO;
using System.Reflection;
using KeePassLib.Serialization;

namespace KeeAgentTestProject
{
  [TestClass]
  public class IOProtocolExtTest
  {

    [ClassInitialize()]
    public static void MyClassInitalize(TestContext context)
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
      if (Directory.Exists(winSCPDestDir))
      {
        Directory.Delete(winSCPDestDir, true);
        int count = 0;
        while (Directory.Exists(winSCPDestDir))
        {
          // wait for directory to delete
          count++;
          Assert.IsTrue(count < 20,
            "Failed to delete IOProtocolExt_WinSCP folder.");
          Thread.Sleep(100);
        }
      }
      Directory.CreateDirectory(winSCPDestDir);
      foreach (string filePath in Directory.GetFiles(winSCPSourceDir))
      {
        string fileName = Path.GetFileName(filePath);
        string sourceFile = Path.Combine(winSCPSourceDir, fileName);
        string destFile = Path.Combine(winSCPDestDir, fileName);
        File.Copy(sourceFile, destFile);
      }
    }

    [ClassCleanup()]
    public static void MyClassCleanup()
    {
      KeePassControl.ExitAll();
    }

    /// <summary>
    /// Tests interaction of KeeAgent with IOProtocolExt
    /// </summary>
    [TestMethod]
    public void TestIOProtocolExt()
    {
      using (KeePassAppDomain testDomain1 = new KeePassAppDomain())
      {
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
            KeePass.Program.MainForm.FormClosed += delegate(
              Object source, FormClosedEventArgs args)
            {
              td1KeeAgentExt.Terminate();
              td1IOProtocolExt.Terminate();
            };

            /* run test */ 

            IOConnectionInfo ioConnectionInfo = new IOConnectionInfo();
            ioConnectionInfo.Path = "sftp://keeagent-test/test.kdbx";
            ioConnectionInfo.UserName = "keeagent";
            ioConnectionInfo.Password = "keeagent";            
            IOConnection.FileExists(ioConnectionInfo);

            /* the problem we are checking for is that IOConnection.FileExists
             * does not lock up. Originally, in KeeAgent, WinPagent ran on 
             * main thread and caused lock-up here. So, no Assert here, rather
             * just looking to see if test times out or not. */            
          });          
        });
        //while (testDomain1.KeePassIsRunning)
        //{
        //  Thread.Sleep(500);
        //}
      }
    }

  }
}