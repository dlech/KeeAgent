using KeeAgent;
using System;
using KeePass.Plugins;
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
using System.Linq;
using KeePassPluginDevTools.Control;

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
  }
}
