using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {
    internal static class RightClickAddDeleteTask {
        internal static bool Add(string shrtCtName, string shrtCtPathInputVal) {
            string shrtCtPathToSave = Utils.ExtractFilePath(shrtCtPathInputVal);
            if (shrtCtPathToSave != null && File.Exists(shrtCtPathToSave)) {
                shrtCtPathToSave = shrtCtPathInputVal;
            } else {
                bool isUri = IsUri(shrtCtPathInputVal, ref shrtCtPathToSave);
                if (!isUri)
                    return false;
                shrtCtPathToSave = Constants.InternetExplorer + " " + shrtCtPathToSave;
            }
            using (RegistryKey hkcrFrndlyTxt =
                    Registry.ClassesRoot.CreateSubKey(@"Directory\Background\Shell\" + shrtCtName + @"\command")) {
                hkcrFrndlyTxt.SetValue("", shrtCtPathToSave);
            }
            return true;
        }

        private static bool IsUri(string val, ref string formattedUri) {
            Uri uriPath;
            if (!Uri.TryCreate(val, UriKind.Absolute, out uriPath)) return false;
            formattedUri = uriPath.AbsoluteUri;
            return true;
        }

        internal static void Delete(FileItem fileItem, ObservableCollection<FileItem> rightClickFileItems) {
            using (RegistryKey hkcrFrndlyTxt = Registry.ClassesRoot.CreateSubKey(@"Directory\Background\Shell")) {
                if (hkcrFrndlyTxt == null && hkcrFrndlyTxt.OpenSubKey(fileItem.Tag.ToString()) != null) return;
                hkcrFrndlyTxt.DeleteSubKeyTree(fileItem.Tag.ToString());
                rightClickFileItems.Remove(fileItem);
            }
        }

        internal static ObservableCollection<FileItem> All() {
            ObservableCollection<FileItem> rightClickListItems = new ObservableCollection<FileItem>();
            using (RegistryKey hkcrDirShell = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\Shell")) {
                if (hkcrDirShell == null)
                   return null;
                foreach (string subKeyName in hkcrDirShell.GetSubKeyNames()) {
                    if (subKeyName.Equals("cmd", StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    RegistryKey regKey = hkcrDirShell.OpenSubKey(subKeyName + @"\command");
                    if (regKey == null) continue;
                    string val = (string) regKey.GetValue("");
                    if (val.ToLower().Contains(Constants.InternetExplorer)) {
                        val = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                            @"Internet Explorer\" + Constants.InternetExplorer);
                    }
                    if (!File.Exists(val))
                        continue;
                    System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(val);
                    ImageSource imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle, new Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
                    FileItem fileItem = new FileItem(val, imgSource, subKeyName);
                    fileItem.Name = subKeyName;
                    rightClickListItems.Add(fileItem);
                }
            }
            return rightClickListItems;
        }
    }
}
