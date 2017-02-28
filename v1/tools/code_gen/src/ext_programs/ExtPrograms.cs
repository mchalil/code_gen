using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ext_programs
{
    public class ExtPrograms
    {
        public string version;
        string current_dir;
        public ExtPrograms(string currentDir)
        {
            string app_name = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string app_path = Path.GetDirectoryName(app_name);
            sigma_name = Path.Combine(app_path, "..\\sigma\\sigma.exe");
            version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            current_dir = currentDir;
        }
        private int copyFiles(string sourcePath, string targetPath)
        {
            int n = 0;
            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath,"*.v");

                // Copy the files and overwrite destination files if they already exist.
                foreach (string s in files)
                {
                    // Use static Path methods to extract only the file name from the path.
                    string fileName = System.IO.Path.GetFileName(s);
                    string destFile = System.IO.Path.Combine(targetPath, fileName);
                    System.IO.File.Move(s, destFile);
                    n++;
                }
            }
            return n;
        }
        string sigma_name;
        public string PerformPartition(string schFile)
        {
            string outstr = "";
            outstr = lsShell.ExecuteCommand(current_dir, sigma_name + " -schematic " + schFile +  " -partition", "");
            return outstr;
        }
        public string PerformHdlGeneration(string schFile)
        {
            string outstr = "";
            outstr = lsShell.ExecuteCommand(current_dir + "\\hw", sigma_name + " -schematic " + schFile + " -generate", "");
            // int fileCount = copyFiles(current_dir, current_dir + "//hw");
            outstr += " files generated ";
            return outstr;
        }
    }
}
