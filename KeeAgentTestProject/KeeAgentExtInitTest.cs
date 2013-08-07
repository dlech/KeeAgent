//
//  KeeAgentExtInitTest.cs
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
using System.Windows.Forms;
using KeeAgent;
using KeePass.Plugins;
using KeePassPluginDevTools.Control;
using NUnit.Framework;

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
