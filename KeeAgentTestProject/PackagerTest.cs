using KeeAgent.Packager;
using System;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace KeeAgentTestProject
{   
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
  [TestFixture()]
  public class PackagerTest
  {    
    /// <summary>
    ///A test for Packager
    ///</summary>
    [Test()]
    public void MainTest()
    {
      string solutionDirectory = Path.GetFullPath ("../..");
      solutionDirectory = Path.GetDirectoryName(solutionDirectory);
      solutionDirectory = "\"" + solutionDirectory + "\"";
      string[] args = { solutionDirectory };
      int expected = 0;
      int actual;
      actual = Program.Main(args);
      Assert.AreEqual(expected, actual);
    }
  }
}
