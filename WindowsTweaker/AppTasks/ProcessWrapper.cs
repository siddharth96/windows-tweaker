using System;
using System.Diagnostics;

namespace WindowsTweaker.AppTasks {
    internal static class ProcessWrapper {

        internal static void ExecuteDosCmd(string cmd) {
            ExecuteProcess("cmd", "/c " + cmd);
        }

        internal static void ExecuteProcess(string processName, string processParam=null) {
            if ((processName.EndsWith(".exe") || processName.ToLower() == "cmd") && !String.IsNullOrEmpty(processName)) {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(processName, processParam) 
                    {RedirectStandardOutput = true, UseShellExecute = false, CreateNoWindow = true};
                Process proc = new Process {StartInfo = processStartInfo};
                proc.Start();
            } else {
                Process.Start(processName);
            }
        }
    }
}
