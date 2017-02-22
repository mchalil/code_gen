using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SchematicScriptCreator
{
    static class lsShell
    {
        public static void ExecuteCommand(string cd, string command, TextBox tbOutputWin_no, string additionalCmdLine, MainWindow main)
        {
            int exitCode;
            //TextBlock tbTutputWin = outputWin as TextBlock;
            ProcessStartInfo processInfo;
            Process process;
            main.DisplayInOutputText(Environment.NewLine + "The cmd is :" + command + Environment.NewLine);

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command + " " + additionalCmdLine);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            main.DisplayInOutputText(processInfo.WorkingDirectory + Environment.NewLine);
            processInfo.WorkingDirectory = cd;
            process = Process.Start(processInfo);
            //    process.WaitForExit();
            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            main.DisplayInOutputText(output + Environment.NewLine);
            main.DisplayInOutputText(error + Environment.NewLine);
            exitCode = process.ExitCode;
            main.DisplayInOutputText(Environment.NewLine + "The cmd exit code is : " + exitCode);

            process.Close();
        }
    }
}
