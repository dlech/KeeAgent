//
//  OptionsTest.cs
//
//  Author(s):
//      David Lechner <david@lechnology.com>
//
//  Copyright (C) 2012-2013  David Lechner
//
//  This program is free software; you can redistribute it and/or
//  modify it under the terms of the GNU General Public License
//  as published by the Free Software Foundation; either version 2
//  of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, see <http://www.gnu.org/licenses>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using dlech.SshAgentLib;
using IOProtocolExt;
using KeeAgent;
using KeePass.Plugins;
using KeePassLib.Serialization;
using KeePassPluginDevTools.Control;
using NUnit.Framework;

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
        Path.Combine(Environment.CurrentDirectory, "..", "..", ".."));
      string winSCPSourceDir = Path.Combine(solutionDir, "lib", "IOProtocolExt",
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

            var extType = td1KeeAgentExt.GetType ();
            var mAgentField = extType.GetField ("mAgent",
                                                BindingFlags.Instance | BindingFlags.NonPublic);
            var agent = mAgentField.GetValue (td1KeeAgentExt) as IAgent;

            // test not valid in client mode
            Assert.That(agent, Is.AssignableTo<Agent>());

            // override confirm callback so we don't have to click OK
            (agent as Agent).ConfirmUserPermissionCallback =
              delegate(ISshKey aKey)
              {
                return true;
              };

            /* load ssh key */
            var keyFileDir = Path.GetFullPath (Path.Combine (
              "..", "..", "..", "SshAgentLib", "SshAgentLibTests", "Resources"));
            var keyFilePath = Path.Combine(keyFileDir, "rsa_with_passphrase");
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
            var constraints = new List<Agent.KeyConstraint>();
            constraints.addConfirmConstraint();            
            agent.AddKeyFromFile(keyFilePath, getPassphraseCallback, constraints);

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