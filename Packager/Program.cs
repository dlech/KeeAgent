using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.InteropServices;
using System.Reflection;

namespace KeeAgent.Packager
{
  public class Program
  {
    public static int Main(string[] args)
    {
      if (args.Length != 1) {
        Console.WriteLine("Requires solution directory as only arg.");
      }
      try {
        string solutionDirectory = args[0].Replace("\"", "");

        // create file name from assembly info
        Assembly keeAgentAssm = Assembly.GetAssembly(
          typeof(KeeAgent.KeeAgentExt));      
        string outputFileName = keeAgentAssm.GetName().Name + "_v" +
          keeAgentAssm.GetName().Version.ToString(
          (keeAgentAssm.GetName().Version.Revision == 0) ? 3 : 4) + ".zip";
        outputFileName = Path.Combine(solutionDirectory, outputFileName);

        File.Delete(outputFileName);
        using (ZipFile zipFile = ZipFile.Create(outputFileName))
        {

          zipFile.BeginUpdate();

          /* add plgx file */
          string plgxFile = keeAgentAssm.GetName().Name + ".plgx";
          string plgxPath = Path.Combine(solutionDirectory, plgxFile);
          if (!File.Exists(plgxPath))
          {
            Console.WriteLine("Could not find file " + plgxPath + "\n" +
              "Probably need to run build under Release configuration");
            return 1;
          }
          zipFile.Add(plgxPath, plgxFile);

          /* add readme and changelog */
          string readmeFile = "README.txt";
          zipFile.Add(Path.Combine(solutionDirectory, readmeFile), readmeFile);
          string changelogFile = "CHANGELOG.txt";
          zipFile.Add(Path.Combine(solutionDirectory, changelogFile),
            changelogFile);

          zipFile.CommitUpdate();
        }

        if (File.Exists(outputFileName)) {
          return 0;
        }
      } catch (Exception ex) {
        Console.WriteLine(ex.ToString());
      }
      return 1;
    }
  }
}
