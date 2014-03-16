using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WindowsTweaker.Models;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using WPFFolderBrowser;

namespace WindowsTweaker.AppTasks
{
    internal static class SendToTask
    {
        private static string _cachedSendToPath;

        internal static Func<WindowsVer.Windows, string> GetFolderPath = (windowsOs) =>   _cachedSendToPath ?? (_cachedSendToPath = windowsOs == WindowsVer.Windows.XP
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo"
                : Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\SendTo");

        internal static bool AddFolder(WindowsVer.Windows windowsOs) {
            string sendToPath = GetFolderPath(windowsOs);
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true) {
                string folderPath = folderBrowserDialog.FileName;
                CreateShortcut(sendToPath, folderPath);
                return true;
            }
            return false;
        }

        internal static bool AddFile(WindowsVer.Windows windowsOs) {
            string sendToPath = GetFolderPath(windowsOs);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                string folderPath = openFileDialog.FileName;
                CreateShortcut(sendToPath, folderPath);
                return true;
            }
            return false;
        }

        internal static void CreateShortcut(string sendToPath, string fullPath) {
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            IWshShortcut shortcut;
            if (WindowsVer.Is64BitMachine) {
                WshShell wshShell = new WshShell();
                shortcut = (IWshShortcut)wshShell.CreateShortcut(Path.Combine(sendToPath, fileName + ".lnk"));
            } else {
                WshShellClass wshShell = new WshShellClass();
                shortcut = (IWshShortcut)wshShell.CreateShortcut(Path.Combine(sendToPath, fileName + ".lnk"));
            }
            shortcut.TargetPath = fullPath;
            shortcut.IconLocation = fullPath;
            shortcut.Save();
        }

        internal static void Delete(ListBox lstBoxSendTo, Message message) {
            if (lstBoxSendTo.SelectedItems.Count == 0) {
                message.Error("Please select at least one item to delete");
                return;
            }
            FileItem[] sendToListItems = new FileItem[lstBoxSendTo.SelectedItems.Count];
            lstBoxSendTo.SelectedItems.CopyTo(sendToListItems, 0);
            string msg = sendToListItems.Length == 1
                ? "Are you sure you want to delete \"" + sendToListItems[0].Name + "\" from Send To Menu?"
                : "Are you sure you want to delete all these " + sendToListItems.Length + " items from Send To Menu?";
            if (MessageBox.Show(msg, Constants.WarningMsgTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes) return;
            List<string> failedFiles = new List<string>();
            foreach (FileItem sendToFileItem in sendToListItems) {
                string filePath = sendToFileItem.FullName;
                try {
                    if (Path.GetExtension(filePath).Equals(".lnk", StringComparison.InvariantCultureIgnoreCase) ||
                        Path.GetExtension(filePath).Equals(".pif", StringComparison.InvariantCultureIgnoreCase)) {
                        if (!System.IO.File.Exists(filePath)) {
                            failedFiles.Add(sendToFileItem.Name);
                            continue;
                        }
                    }
                    else {
                        failedFiles.Add(sendToFileItem.Name);
                        continue;
                    }
                    System.IO.File.Delete(filePath);
                }
                catch (IOException) {
                    failedFiles.Add(sendToFileItem.Name);
                }
                catch (NullReferenceException) {
                    failedFiles.Add(sendToFileItem.Name);
                }
            }
            if (!failedFiles.Any()) return;
            string failedMsg = failedFiles.Count > 1
                ? "Failed to delete " + failedFiles.SentenceJoin() +
                  " because they are either System files or are not shortcuts"
                : "Failed to delete " + failedFiles.SentenceJoin() +
                  " because it is either a System file or is not a valid shortcut.";
            message.Error(failedMsg);
        }
    }
}