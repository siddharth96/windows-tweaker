using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {

    internal static class Utils {
        public static Func<int?, bool> IntToBool = (int? val) => !(val == 0 || !val.HasValue);
        
        public static Func<int?, bool> ReversedIntToBool = (int? val) => val == 0;

        public static Func<bool?, int> BoolToInt = (bool? val) => val == true ? 1 : 0;

        public static Func<bool?, int> ReversedBoolToInt = (bool? val) => val == true ? 0 : 1;

        public static Func<string, bool> StringToBool = (string val) => val != null && val.Equals("1");

        public static bool ReversedStringToBool(string val) {
            if (val == null)
                return false;
            return !val.Equals("1");
        }

        public static Func<bool?, string> BoolToString = (bool? val) => val == true ? "1" : "0";

        public static Func<bool?, string> ReversedBoolToString = (bool? val) => val == true ? "0" : "1";

        public static bool IsEmptyDirectory(string path) {
            return !Directory.EnumerateFiles(path).Any();
        }

        // Regex link : http://stackoverflow.com/a/7804916/1293716
        private static readonly Regex FilePathRegex = new Regex(
            @"(([a-z]:|\\\\[a-z0-9_.$]+\\[a-z0-9_.$]+)?(\\?(?:[^\\/:*?""<>|\r\n]+\\)+)[^\\/:*?""<>|\r\n]+)",
            RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

        public static string ExtractFilePath(string fPath) {
            try {
                Match filePathMatch = FilePathRegex.Match(fPath);
                if (!filePathMatch.Success) return null;
                string val = filePathMatch.Value;
                return val;
            }
            catch (ArgumentException) {
                // Syntax error in regular expression
                return null;
            }
        }

        public static bool Move(this RegistryKey sourceKey, RegistryKey destinationKey, string keyName) {
            string keyVal = (string) sourceKey.GetValue(keyName);
            if (keyVal == null) return false;
            sourceKey.DeleteValue(keyName);
            destinationKey.SetValue(keyName, keyVal);
            return true;
        }

        public static string GetUserSelectedFilePath(string filter = "Executable files|*.exe|Batch files|*.bat|Command files|*.com|Jar Files|*.jar|All files|*.*") {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = filter,
                Multiselect = false
            };
            return openFileDialog.ShowDialog() == true && !String.IsNullOrEmpty(openFileDialog.FileName) ?
                openFileDialog.FileName.Trim() : null;
        }

        public static string[] GetUserSelectedFilePathList(string filter = "Executable files|*.exe|Batch files|*.bat|Command files|*.com|Jar Files|*.jar|All files|*.*") {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = filter,
                Multiselect = true
            };
            return openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Any() ?
                openFileDialog.FileNames : null;
        }

        public static string GetUserSelectedFolder() {
            WPFFolderBrowser.WPFFolderBrowserDialog folderBrowser = new WPFFolderBrowser.WPFFolderBrowserDialog();
            return folderBrowser.ShowDialog() == true && !String.IsNullOrEmpty(folderBrowser.FileName) ? 
                folderBrowser.FileName : null;
        }

        internal static string SentenceJoin(this List<string> lst, string separator="and") {
            if (lst == null || !lst.Any())
                return String.Empty;
            int lenLst = lst.Count;
            switch (lenLst) {
                case 1:
                    return lst[0];
                case 2:
                    return lst[0] + " " + separator + " " + lst[1];
                default:
                    return String.Join(", ", lst.Take(lenLst - 1)) + " " + separator + " " + lst[lenLst - 1];
            }
        }

        internal static bool HasValueInShellCommand(this RegistryKey regKey, string valName) {
            if (regKey == null)
                return false;
            return regKey.GetSubKeyNames().Select(subKeyName => regKey.OpenSubKey(subKeyName + @"\" + Constants.Cmd)).
                Any(subKey => subKey != null && (subKey.GetValue("") as string) == valName);
        }

        public static bool HasValue(this RegistryKey regKey, string filePath) {
            if (regKey == null) return false;
            string[] valueKeys = regKey.GetValueNames();
            return (from valueKey in valueKeys
                select (string) regKey.GetValue(valueKey)
                into value
                where
                    value != null
                select ExtractFilePath(value)).Any(containedFilePath => containedFilePath
                                                                        != null &&
                                                                        containedFilePath.Equals(filePath,
                                                                            StringComparison.InvariantCultureIgnoreCase));
        }

        internal static string GetConfigFilePath() {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            folderPath = Path.Combine(folderPath, Constants.ConfigDirectoryName);
            if (!Directory.Exists(folderPath)) {
                try {
                    Directory.CreateDirectory(folderPath);
                } catch (IOException) { }
            }
            return folderPath != null ? Path.Combine(folderPath, Constants.ConfigFileName) : null;
        }

        internal static Func<string, string> GetHideCmd = val => String.Format("attrib +h +s \"{0}\"", val);

        internal static Func<string, string> GetUnhideCmd = val => String.Format("attrib -h -s \"{0}\"", val);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Bitmap bitmap) {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap)) {
                throw new Win32Exception();
            }
            return wpfBitmap;
        }

        public static int? RepairAsNullableIntFromString(RegistryKey regKey, string keyName) {
            try {
                string strKeyVal = (string) regKey.GetValue(keyName);
                int val = (int) Double.Parse(strKeyVal);
                regKey.DeleteValue(keyName);
                regKey.SetValue(keyName, val);
                return val;
            }
            catch (InvalidCastException) {
                return null;
            }
            catch (FormatException) {
                return null;
            }
        }

        public static string RepairAsStringFromInt(RegistryKey regKey, string keyName) {
            // Data type is incorrect, repair it by changing from REG_DWORD to REG_SZ
            int? intValNullable = regKey.GetValue(keyName) as int?;
            if (!intValNullable.HasValue) return null;
            int intVal = (int)intValNullable;
            string val = intVal.ToString(CultureInfo.InvariantCulture);
            regKey.DeleteValue(keyName);
            regKey.SetValue(keyName, val);
            return val;
        }

        public static void CreateBlankIconFile() {
            String filePath = Utils.GetBlankShortcutIconPath();
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Constants.BlankIconResourceName)) {
                using (FileStream resourceFile = new FileStream(filePath, FileMode.CreateNew)) {
                    stream.CopyTo(resourceFile);
                }
            }
        }

        public static string GetBlankShortcutIconPath() {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Windows Tweaker",
                        "blank.ico");
        }

        public static string GetAppVersion() {
            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return applicationVersion.Major + "." + applicationVersion.Minor + 
                (applicationVersion.Build > 0 ? ("." + applicationVersion.Build) : "");
        }
    }
}