using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ext_programs
{
    static class lsShell
    {
        public static string ExecuteCommand(string cd, string command, string additionalCmdLine)
        {
            string outmsg = "";
            int exitCode;
            //TextBlock tbTutputWin = outputWin as TextBlock;
            ProcessStartInfo processInfo;
            Process process;
            outmsg += Environment.NewLine + "The cmd is : ";
            outmsg += command + Environment.NewLine;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.WorkingDirectory = cd;
            process = Process.Start(processInfo);
            // Warning: This approach can lead to deadlocks, see Edit #2
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            outmsg += output + Environment.NewLine;
            outmsg += error + Environment.NewLine;
            exitCode = process.ExitCode;

            outmsg += Environment.NewLine + "The cmd exit code is : " + exitCode;
            process.Close();
            return outmsg;
        }
    }
}
