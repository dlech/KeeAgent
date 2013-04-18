using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PreBuild
{
  /// <summary>
  /// Program to run as the PreBuild command when creating a KeePass plgx.
  /// Currently, this just provides a work-around for UpdateUrl only being
  /// supported in KeePass >= 2.18
  /// 
  /// Usage is:
  /// 
  /// --plgx-build-pre:"cmd /c """{PLGX_TEMP_DIR}PreBuild.exe""" {PLGX_TEMP_DIR}"
  ///  
  /// </summary>
  /// <remarks>Change the /c to /k for debugging (/k leaves the console window
  /// open after program if finished running).</remarks>
  public class Program
  {
    public static int Main(string[] args)
    {
      int version = GetKeePassVersion();
      Console.WriteLine("Detected version 2." + version);
      if (args.Length >= 1) {
        // there seems to be a problem using triple-quotes on the argument
        // so we leave them out and if the temp dir contains spaces, we receive
        // it as multiple args then glue it back together with spaces
        string tempDir = string.Join(" ", args);
        Console.WriteLine("Temp Dir: " + tempDir);

        // Only KeePass version >= v2.18 supports UpdateUrl
        // Since there is not a way to pass conditional compilation symbols to
        // the compiler in KeePass, we manually remove the directive lines to
        // get the same effect
        if (version >= 18) {
          string updateUrlFile = Path.Combine(tempDir, "UpdateUrl.cs");
          Console.WriteLine("Modifying: " + updateUrlFile);
          string[] fileContents = File.ReadAllLines(updateUrlFile);
          for (int i = 0, len = fileContents.Length; i < len; i++) {
            if (fileContents[i].StartsWith("#")) {
              fileContents[i] = "\r\n";
            }
          }
          File.WriteAllLines(updateUrlFile, fileContents);
          return 0;
        }
      } else {
        Console.WriteLine("requires argument");
      }      
      return 1;
    }

    /// <summary>
    /// Gets the version of the currently running instance of KeePass 
    /// </summary>
    /// <returns>the x in KeePass 2.x</returns>
    public static int GetKeePassVersion()
    {
      int version = 0;
      // search for running KeePass 2.x process
      Process[] keepassProcs = Process.GetProcessesByName("KeePass");
      foreach (Process proc in keepassProcs) {
        foreach (ProcessModule module in proc.Modules) {
          if (module.ModuleName == "KeePass.exe") {
            FileVersionInfo versionInfo = module.FileVersionInfo;
            if (versionInfo.FileMajorPart == 2) {
              // get minor version (x in 2.x)
              version = versionInfo.FileMinorPart;
              // KeePass version < 2.19 has version as 2.1.8.0, etc,
              // instead of 2.19.0.0
              if (version == 0 || version == 1) {
                version = version * 10 + versionInfo.FileBuildPart;
              }
            }
            break;
          }
        }
      }
      // If there was not a process running, maybe the assembly in in the 
      // current working directory. This is the case when debugging / running
      // tests.
      if (version == 0) {
        try {
          var keepassAssm = Assembly.UnsafeLoadFrom("keepass.exe");
          var assmVersion = keepassAssm.GetName().Version;
          if (assmVersion.Major == 2) {
            version = assmVersion.Minor;
            if (version == 0 || version == 1) {
              version = version * 10 + assmVersion.Build;
            }
          }
        } catch (Exception) { }
      }
      return version;
    }
  }
}
