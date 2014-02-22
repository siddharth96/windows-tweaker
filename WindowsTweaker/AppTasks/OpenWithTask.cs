using System.Collections.Generic;
using System.Windows.Controls;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {
    internal static class OpenWithTask {

        public static void Add(FileItem fileItem) {}

        public static void Remove(FileItem fileItem) {}

        public static void ShowDetail(Image img) {
            FileItem fileItem = img.Tag as FileItem;
            if (fileItem == null) return;
            MoreInfo moreInfo = new MoreInfo(fileItem.Name, fileItem.FullName, fileItem.IconAssociated);
            moreInfo.ShowDialog();
        }

        public static Dictionary<string, bool> LoadOpenWithItems() {
            Dictionary<string, bool> openWithFileDictionary = new Dictionary<string, bool>();
            using (RegistryKey hkcrApplications = Registry.ClassesRoot.OpenSubKey("Applications", true)) {
                string[] subKeyNames = hkcrApplications.GetSubKeyNames();
                RegistryKey hkcrShell, hkcrOpen, hkcrEdit, hkcrCmd;
                foreach (string subKeyName in subKeyNames) {
                    hkcrShell = hkcrOpen = hkcrEdit = hkcrCmd = null;
                    RegistryKey regKey = hkcrApplications.OpenSubKey(subKeyName, true);
                    if (regKey.SubKeyCount > 0) {
                        bool isChecked = regKey.GetValue(Constants.NoOpenWith) == null;
                        hkcrShell = regKey.OpenSubKey(Constants.Shell);
                        if (hkcrShell != null) {
                            RegistryKey commandParentKey = null;
                            hkcrOpen = hkcrShell.OpenSubKey(Constants.Open);
                            if (hkcrOpen != null)
                                commandParentKey = hkcrOpen;
                            else {
                                hkcrEdit = hkcrShell.OpenSubKey(Constants.Edit);
                                if (hkcrEdit != null)
                                    commandParentKey = hkcrEdit;
                                else {
                                    commandParentKey = hkcrShell.OpenSubKey(Constants.Read);
                                }
                            }
                            if (commandParentKey != null) {
                                hkcrCmd = commandParentKey.OpenSubKey(Constants.Cmd);
                                if (hkcrCmd != null) {
                                    string fPath = (string) hkcrCmd.GetValue("");
                                    if (fPath != null) {
                                        fPath = Utils.ExtractFilePath(fPath);
                                        if (fPath != null) {
                                            openWithFileDictionary[fPath] = isChecked;
                                        }
                                    }
                                }
                            } // end of commandParent != null
                        } // end of hkcr != null
                    } // end of regKey.Count > 0
                } // end for
            } // end using
            return openWithFileDictionary;
        }
    }
}
