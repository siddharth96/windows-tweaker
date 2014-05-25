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
    internal class SendToTask
    {
        internal SendToTask(MainWindow mainWindow) {
            _window = mainWindow;
        }

        private readonly MainWindow _window;
        private string _cachedSendToPath;

        internal string GetSendToFolderPath(WindowsVer.Windows windowsOs) {
            if (!String.IsNullOrEmpty(_cachedSendToPath))
                return _cachedSendToPath;
            if (windowsOs == WindowsVer.Windows.Xp)
                _cachedSendToPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo";
            else
                _cachedSendToPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\SendTo";
            return _cachedSendToPath;
        }

        internal bool AddFolder(WindowsVer.Windows windowsOs) {
            string sendToPath = GetSendToFolderPath(windowsOs);
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true) {
                string folderPath = folderBrowserDialog.FileName;
                CreateShortcut(sendToPath, folderPath);
                return true;
            }
            return false;
        }

        internal bool AddFile(WindowsVer.Windows windowsOs) {
            string sendToPath = GetSendToFolderPath(windowsOs);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                string folderPath = openFileDialog.FileName;
                CreateShortcut(sendToPath, folderPath);
                return true;
            }
            return false;
        }

        internal void CreateShortcut(string sendToPath, string fullPath) {
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            IWshShortcut shortcut;
            if (WindowsVer.Is64BitOs()) {
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

        internal void Delete(ListBox lstBoxSendTo, Message message) {
            if (lstBoxSendTo.SelectedItems.Count == 0) {
                message.Error(_window.FindResource("SelectedOnItem") as string);
                return;
            }
            FileItem[] sendToListItems = new FileItem[lstBoxSendTo.SelectedItems.Count];
            lstBoxSendTo.SelectedItems.CopyTo(sendToListItems, 0);
            string msg = sendToListItems.Length == 1
                ? _window.FindResource("SureToDelete") + " \"" + sendToListItems[0].Name + "\" " + _window.FindResource("FromSendToMenu")
                : _window.FindResource("SureToDeleteMultiple") + " " + sendToListItems.Length + " " + _window.FindResource("FromSendToMenuMultiple");
            InfoBox infoBox = new InfoBox(msg, _window.FindResource("Delete").ToString(), 
                _window.FindResource("WarningMsgTitle").ToString(), InfoBox.DialogType.Question);
            if (infoBox.ShowDialog() != true) return;
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
                ? _window.FindResource("FailedToDelete") + " " + failedFiles.SentenceJoin() +
                  " " + _window.FindResource("AreNotShortcutsMultiple")
                : _window.FindResource("FailedToDelete") + " " + failedFiles.SentenceJoin() +
                  " " + _window.FindResource("AreNotShortcuts");
            message.Error(failedMsg);
        }
    }
}