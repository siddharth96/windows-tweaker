using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace WindowsTweaker {
    /// <summary>
    ///     Interaction logic for StartupManager.xaml
    /// </summary>
    public partial class StartupManager : Window {
        public StartupManager() {
            InitializeComponent();
            Dictionary<string, Tuple<string, bool>> startupItemDictionary = LoadStartupItems();
            if (startupItemDictionary.Any()) {
                FileReader fileReader = new FileReader(startupItemDictionary);
                ObservableCollection<ToggleViewFileItem> fileItemList =
                    fileReader.GetAsToggleViewFileItemCollectionWithUserTitle("On", "Off");
                lstStartupItems.ItemsSource = fileItemList;
            }
        }

        private Dictionary<string, Tuple<string, bool>> LoadStartupItems() {
            StartupItemHolderClass startupItemHolderClass = new StartupItemHolderClass();
            using (RegistryKey hkcrRun = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hkcrRun, true);
            }
            using (RegistryKey hkcrRunDisabled = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hkcrRunDisabled, false);
            }
            using (RegistryKey hklmRun = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hklmRun, false);
            }
            using (RegistryKey hklmRunDisabled = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-")) {
                startupItemHolderClass.UpdateStartupDictionaryForKey(hklmRunDisabled, false);
            }
            return startupItemHolderClass.StartupItemDictionary;
        }


        private class StartupItemHolderClass {
            private Dictionary<string, Tuple<string, bool>> startupItemDictionary;

            public Dictionary<string, Tuple<string, bool>> StartupItemDictionary {
                get { return startupItemDictionary; }
            }

            public StartupItemHolderClass() {
                this.startupItemDictionary = new Dictionary<string, Tuple<string, bool>>();
            }

            public void UpdateStartupDictionaryForKey(RegistryKey registryKey, bool isChecked) {
                if (registryKey != null) {
                    String[] valueNames = registryKey.GetValueNames();
                    foreach (string valueName in valueNames) {
                        string filePath = (string) registryKey.GetValue(valueName);
                        filePath = Utils.ExtractFilePath(filePath);
                        if (filePath != null) {
                            Tuple<string, bool> tuple = new Tuple<string, bool>(valueName, isChecked);
                            startupItemDictionary.Add(filePath, tuple);
                        }
                    }
                }
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}