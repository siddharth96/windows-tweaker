using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {

    internal static class Utils {
        public static Func<int?, bool> IntToBool = (int? val) => !(val == 0 || val == null);

        public static Func<bool?, int> BoolToInt = (bool? val) => val == true ? 1 : 0;

        public static Func<bool?, int> ReversedBoolToInt = (bool? val) => val == true ? 0 : 1;

        public static Func<string, bool> StringToBool = (string val) => val != null && val.Equals("1");

        public static Func<bool?, string> BoolToString = (bool? val) => val == true ? "1" : "0";

        public static void SafeDeleteRegistryValue(RegistryKey regKey, string keyName) {
            // Can't extend Microsoft.Win32.RegistryKey since its sealed, hence has to put this in Utils.cs
            try {
                regKey.DeleteValue(keyName);
            } catch (ArgumentException) {
            }
        }

        public static bool IsEmptyDirectory(string path) {
            return !Directory.EnumerateFiles(path).Any();
        }

        public static void ExecuteCmd(string cmd) {
            ProcessStartInfo hideProc = new ProcessStartInfo("cmd", "/c " + cmd);
            hideProc.RedirectStandardOutput = true;
            hideProc.UseShellExecute = false;
            hideProc.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = hideProc;
            proc.Start();
        }


        // Regex link : http://social.msdn.microsoft.com/Forums/en-US/e3f2bb04-1b6a-4e06-9fcd-07e86ae4bd5f/regular-expression-to-validate-file-path?forum=regexp
        private static readonly Regex FilePathRegex = new Regex(
            @"(?:(?:(?:\b[a-z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)\\|
	              \\?[^\\/:*?""<>|\r\n]+\\?)               
	              (?:[^\\/:*?""<>|\r\n]+\\)*               
	              [^\\/:*?""<>|\r\n]*)",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public static string ExtractFilePath(string fPath) {
            Match filePathMatch = FilePathRegex.Match(fPath);
            if (!filePathMatch.Success) return null;
            string val = filePathMatch.Value;
            return val;
        }

        public static bool MoveRegistryKey(RegistryKey sourceKey, RegistryKey destinationKey, string keyName) {
            string keyVal = (string) sourceKey.GetValue(keyName);
            if (keyVal != null) {
                sourceKey.DeleteValue(keyName);
                destinationKey.SetValue(keyName, keyVal);
                return true;
            }
            return false;
        }

        public static string GetUserSelectedFilePath() {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Executable files|*.exe|Batch files|*.bat|Command files|*.com|Jar Files|*.jar|All files|*.*"
            };
            if (openFileDialog.ShowDialog() == true) {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}