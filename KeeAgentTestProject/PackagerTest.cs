using KeeAgent.Packager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KeeAgentTestProject
{   
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
  [TestClass()]
  public class PackagerTest
  {    
    /// <summary>
    ///A test for Packager
    ///</summary>
    [TestMethod()]
    public void MainTest()
    {
      string solutionDirectory = ((EnvDTE.DTE)Marshal
        .GetActiveObject("VisualStudio.DTE.10.0")).Solution.FullName;
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
