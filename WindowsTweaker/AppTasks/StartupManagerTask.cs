using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using WindowsTweaker.Models;
using Microsoft.Win32;

namespace WindowsTweaker.AppTasks {
    internal static class StartupManagerTask {
        public enum AddStatus {
            Success, AlreadyPresent, Failed
        }

        public static AddStatus Add(string filePath, bool? onlyCurrentUser) {
            RegistryKey hkcrRun = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            if (Contains(hkcrRun, filePath)) {
                hkcrRun.Close();
                return AddStatus.AlreadyPresent;
            }
            RegistryKey hkcrRunDisabled =
                Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-");
            if (Contains(hkcrRunDisabled, filePath)) {
                hkcrRun.Close();
                hkcrRunDisabled.Close();
                return AddStatus.AlreadyPresent;
            }
            RegistryKey hklmRun = Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            if (Contains(hklmRun, filePath)) {
                hkcrRun.Close();
                hkcrRunDisabled.Close();
                hklmRun.Close();
                return AddStatus.AlreadyPresent;
            }
            RegistryKey hklmRunDisabled =
                Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-");
            if (Contains(hklmRunDisabled, filePath)) {
                hkcrRun.Close();
                hkcrRunDisabled.Close();
                hklmRun.Close();
                hklmRunDisabled.Close();
                return AddStatus.AlreadyPresent;
            }
            RegistryKey regKey = onlyCurrentUser == true ? hkcrRun : hklmRun;
            regKey.SetValue(Path.GetFileNameWithoutExtension(filePath), filePath);
            hkcrRun.Close();
            hkcrRunDisabled.Close();
            hklmRun.Close();
            hklmRunDisabled.Close();
            return AddStatus.Success;
        }

        public static bool Contains(RegistryKey regKey, string filePath) {
            if (regKey == null) return false;
            string[] valueKeys = regKey.GetValueNames();
            return (from valueKey in valueKeys select (string) regKey.GetValue(valueKey) into value where
                        value != null select Utils.ExtractFilePath(value)).Any(containedFilePath => containedFilePath
                            != null && containedFilePath.Equals(filePath, StringComparison.InvariantCultureIgnoreCase));
        }

        public static void Toggle(FileItem fileItem, bool newState) {
            RegistryKey hkcrRun = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            RegistryKey hkcrRunDisabled =
                Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-");
            bool isSuccess = Utils.MoveRegistryKey(hkcrRunDisabled, hkcrRun, fileItem.Name);
            if (!isSuccess) {
                RegistryKey hklmRun =
                    Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                RegistryKey hklmRunDisabled =
                    Registry.LocalMachine.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-");
                if (newState) {
                    Utils.MoveRegistryKey(hklmRunDisabled, hklmRun, fileItem.Name);
                }
                else {
                    Utils.MoveRegistryKey(hklmRun, hklmRunDisabled, fileItem.Name);
                }
                hklmRun.Close();
                hklmRunDisabled.Close();
            }
            hkcrRun.Close();
            hkcrRunDisabled.Close();
        }

        public static void ShowDetail(Image img) {
            FileItem fileItem = img.Tag as FileItem;
            if (fileItem == null) return;
            MoreInfo moreInfo = new MoreInfo(fileItem.Name, fileItem.FullName, fileItem.IconAssociated);
            moreInfo.ShowDialog();
        }

        public static Dictionary<string, Models.Tuple<string, bool>> LoadStartupItems() {
            StartupItemHolderClass startupItemHolderClass = new StartupItemHolderClass();
            using (RegistryKey hkcrRun = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hkcrRun, true);
            }
            using (RegistryKey hkcrRunDisabled =
                    Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hkcrRunDisabled, false);
            }
            using (RegistryKey hklmRun = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hklmRun, true);
            }
            using (RegistryKey hklmRunDisabled = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hklmRunDisabled, false);
            }
            return startupItemHolderClass.StartupItemDictionary;
        }


        private class StartupItemHolderClass {
            private Dictionary<string, Models.Tuple<string, bool>> startupItemDictionary;

            public Dictionary<string, Models.Tuple<string, bool>> StartupItemDictionary {
                get { return startupItemDictionary; }
            }

            public StartupItemHolderClass() {
                this.startupItemDictionary = new Dictionary<string, Models.Tuple<string, bool>>();
            }

            public void UpdateStartupDictionaryForKey(RegistryKey registryKey, bool isChecked) {
                if (registryKey == null) return;
                String[] valueNames = registryKey.GetValueNames();
                foreach (string valueName in valueNames) {
                    string filePath = (string) registryKey.GetValue(valueName);
                    filePath = Utils.ExtractFilePath(filePath);
                    if (filePath == null) continue;
                    Models.Tuple<string, bool> tuple = new Models.Tuple<string, bool>(valueName, isChecked);
                    if (startupItemDictionary.ContainsKey(filePath)) {
                        Models.Tuple<string, bool> val = startupItemDictionary[filePath];
                        tuple.y = tuple.y || val.y;
                    }
                    startupItemDictionary[filePath] = tuple;
                }
            }
        }
    }
}
