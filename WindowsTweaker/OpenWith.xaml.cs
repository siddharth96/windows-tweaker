using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace WindowsTweaker {
    /// <summary>
    ///     Interaction logic for OpenWith.xaml
    /// </summary>
    public partial class OpenWith : Window {
        public OpenWith() {
            Dictionary<string, bool> openWithFileDictionary = new Dictionary<string, bool>();
            InitializeComponent();
            openWithFileDictionary = LoadOpenWithItems();
            if (openWithFileDictionary.Any()) {
                FileReader fileReader = new FileReader(openWithFileDictionary);
                ObservableCollection<ToggleViewFileItem> fileItemList = fileReader.GetAsToggleViewFileItemCollection("Show", "Hide");
                lstOpenWithBox.ItemsSource = fileItemList;
            }
        }


        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private Dictionary<string, bool> LoadOpenWithItems() {
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