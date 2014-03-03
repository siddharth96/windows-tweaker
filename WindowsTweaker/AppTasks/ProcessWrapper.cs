using System.Diagnostics;

namespace WindowsTweaker.AppTasks {
    internal static class ProcessWrapper {

        internal static void ExecuteDosCmd(string cmd) {
            ExecuteProcess("cmd", "/c " + cmd);
        }

        internal static void ExecuteProcess(string processName, string processParam=null) {
            ProcessStartInfo processStartInfo = processParam == null ? new ProcessStartInfo(processName) :
                new ProcessStartInfo(processName, processParam);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;
            Process proc = new Process {StartInfo = processStartInfo};
            proc.Start();
        }
    }
}
