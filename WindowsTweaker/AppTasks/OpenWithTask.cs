using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {
    internal static class OpenWithTask {
        public enum AddStatus {
            Success, AlreadyPresent, Failed
        }

        public static AddStatus Add(string filePath) {
            using (RegistryKey hkcrApplications = Registry.ClassesRoot.CreateSubKey("Applications")) {
                string fileName = Path.GetFileName(filePath);
                if (fileName == null) return AddStatus.Failed;
                RegistryKey regKey = hkcrApplications.OpenSubKey(fileName);
                if (regKey != null) return AddStatus.AlreadyPresent;
                regKey = hkcrApplications.CreateSubKey(fileName);
                RegistryKey hkcrShell = regKey.CreateSubKey(Constants.Shell);
                RegistryKey hkcrOpen = hkcrShell.CreateSubKey(Constants.Open);
                RegistryKey hkcrCmd = hkcrOpen.CreateSubKey(Constants.Cmd);
                hkcrCmd.SetValue("", filePath);
                return AddStatus.Success;
            }
        }

        public static void Toggle(FileItem fileItem, bool newState) {
            using (RegistryKey hkcrApplications = Registry.ClassesRoot.CreateSubKey("Applications"))
            {
                string fileName = System.IO.Path.GetFileName(fileItem.FullName);
                if (fileName == null) return;
                RegistryKey regKey = hkcrApplications.OpenSubKey(fileName, true);
                if (regKey == null) return;
                if (newState) {
                    regKey.DeleteValue(Constants.NoOpenWith);
                }
                else {
                    regKey.SetValue(Constants.NoOpenWith, "");
                }
            }
        }

        public static void ShowDetail(Image img) {
            FileItem fileItem = img.Tag as FileItem;
            if (fileItem == null) return;
            MoreInfo moreInfo = new MoreInfo(fileItem.Name, fileItem.FullName, fileItem.IconAssociated);
            moreInfo.ShowDialog();
        }

        public static Dictionary<string, bool> LoadOpenWithItems() {
            Dictionary<string, bool> openWithFileDictionary = new Dictionary<string, bool>();
            using (RegistryKey hkcrApplications = Registry.ClassesRoot.OpenSubKey("Applications")) {
                string[] subKeyNames = hkcrApplications.GetSubKeyNames();
                RegistryKey hkcrShell, hkcrOpen, hkcrEdit, hkcrCmd;
                foreach (string subKeyName in subKeyNames) {
                    RegistryKey regKey = hkcrApplications.OpenSubKey(subKeyName);
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
                                    string fPath = hkcrCmd.GetValue("") as string;
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
