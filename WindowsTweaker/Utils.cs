using System.IO;
using System.Linq;
using Microsoft.Win32;
using System;

namespace WindowsTweaker {

    internal static class Utils {
        public static Func<int?, bool> IntToBool = (int? val) => !(val == 0 || val == null);

        public static Func<bool?, int> BoolToInt = (bool? val) => val == true ? 1 : 0;

        public static Func<bool?, int> ReversedBoolToInt = (bool? val) => val == true ? 0 : 1;

        public static Func<string, bool> StringToBool = (String val) => val != null && val.Equals("1");

        public static Func<bool?, String> BoolToString = (bool? val) => val == true ? "1" : "0";

        public static void SafeDeleteRegistryValue(RegistryKey regKey, String keyName) {
            // Can't extend Microsoft.Win32.RegistryKey since its sealed, hence has to put this in Utils.cs
            try {
                regKey.DeleteValue(keyName);
            } catch (ArgumentException) {
            }
        }

        public static bool IsEmptyDirectory(String path) {
            return !Directory.EnumerateFiles(path).Any();
        }
    }
}