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

        public static string ExtractFilePath(string fPath) {
            string[] fileInfo = null;
            if (fPath[0] == '\"' && (fPath.Contains("\"%1\"") || fPath.Contains("\"%L\"") || fPath.Contains("%1"))) {
                fileInfo = fPath.Split('\"');
                return fileInfo[1];
            }
            else if (!fPath.Contains("\"") || fPath.Contains("\"%1\"") || fPath.Contains("%1")) {
                int space_cnt = 0;
                for (int i = 0; i < fPath.Length; i++) {
                    if (fPath[i] == ' ')
                        space_cnt++;
                }
                if (space_cnt == 1)
                    fileInfo = fPath.Split(' ');
                else if (space_cnt > 1) {
                    if (fPath.Contains('/')) {
                        fileInfo = fPath.Split('/');
                    }
                    else {
                        fileInfo = fPath.Split('\"');
                    }
                }
                return fileInfo[0];
            }
            else {
                fileInfo = fPath.Split(' ');
                string probableFilePath = fileInfo[0];
                if (probableFilePath.StartsWith("\"") && probableFilePath.EndsWith("\"")) {
                    return probableFilePath.Substring(1, probableFilePath.Length - 2);
                }
            }
            return null;
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
    }
}