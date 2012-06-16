using System;
using System.Diagnostics;
using System.IO;

namespace PreBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 1) {
                int version = 0;
                // search for running keepass 2.x process
                Process[] keepassProcs = Process.GetProcessesByName("KeePass");
                foreach (Process proc in keepassProcs) {
                    foreach (ProcessModule module in proc.Modules) {
                        if (module.ModuleName == "KeePass.exe") {
                            FileVersionInfo versionInfo = module.FileVersionInfo;
                            if (versionInfo.FileMajorPart == 2) {
                                // get minor version (x in 2.x)
                                version = versionInfo.FileMinorPart;
                                // keepass version < 2.19 has version as 2.1.8.0, etc, instead of 2.19.0.0
                                if (version == 0 || version == 1) {
                                    version = version * 10 + versionInfo.FileBuildPart;
                                }                                
                            }
                        }
                    }                    
                }
                Console.WriteLine("Detected version 2." + version);

                // there seems to be a problem using triple-quotes on the argument
                // so we do without and just glue it back together here.
                string tempDir = string.Join(" ", args);
                Console.WriteLine("Temp Dir: " + tempDir);

                // KeePass version < v2.18 does not support update url
                // so we removed that code from the temp dir before it is compiled
                if (version >= 18) {                                     
                    string updateUrlFile = Path.Combine(tempDir, "UpdateUrl.cs");
                    Console.WriteLine("Modifying: " + updateUrlFile);
                    string[] fileContents = File.ReadAllLines(updateUrlFile);
                    for (int i=0, len=fileContents.Length; i<len; i++) {
                        if (fileContents[i].StartsWith("#")) {
                            fileContents[i] = "\r\n";
                        }
                    }
                    File.WriteAllLines(updateUrlFile, fileContents);
                } 
            } else {
                Console.WriteLine("requires 1 argument");
            }
        }
    }
}
