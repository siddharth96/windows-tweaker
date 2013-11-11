using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private readonly RegistryKey HKCU = Registry.CurrentUser;
        private readonly RegistryKey HKLM = Registry.LocalMachine;
        private readonly RegistryKey HKCR = Registry.ClassesRoot;
        private readonly WindowsVer.Windows windowsOS = WindowsVer.Instance.GetName();

        private void OnTabClicked(object sender, RoutedEventArgs e) {
            String tagVal = ((TabItem)sender).Tag.ToString();
            switch (tagVal) {
                case Constants.EXPLORER:
                    LoadExplorerTab();
                    break;
                case Constants.SYSTEM:
                    break;
                case Constants.DISPLAY:
                    break;
                case Constants.RIGHT_CLICK:
                    break;
                case Constants.PLACES:
                    break;
                case Constants.TASKS:
                    break;
                case Constants.FEATURES:
                    break;
                case Constants.LOGON:
                    break;
                case Constants.RESTRICTIONS:
                    break;
                case Constants.MAINTENANCE:
                    break;
                case Constants.UTILITIES:
                    break;
            }
        }

        private void LoadExplorerTab() {

            //Drive Letters
            using (RegistryKey hkcuCvExplorer = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
                int showDriveLetterVal = (int)hkcuCvExplorer.GetValue(Constants.SHOW_DRIVE_LETTERS, 0);
                switch (showDriveLetterVal) {
                    case 0:
                        rbtnShowDriveLetterAfterName.IsChecked = true;
                        break;
                    case 2:
                        rbtnHideDriveLetter.IsChecked = true;
                        break;
                    case 4:
                        rbtnShowDriveLetterBeforeName.IsChecked = true;
                        break;
                    default:
                        rbtnShowDriveLetterAfterName.IsChecked = true;
                        break;
                }
            }

            using (RegistryKey hkcuExAdvanced = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Advanced
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowMenuBar, hkcuExAdvanced, Constants.ALWAYS_SHOW_MENU);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideExtension, hkcuExAdvanced, Constants.HIDE_FILE_EXT);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkSelectItems, hkcuExAdvanced, Constants.AUTO_CHK_SELECT);

                int? val = (int?)hkcuExAdvanced.GetValue(Constants.HIDDEN);
                chkShowHiddenFilesFolders.IsChecked = val == 1;

                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkRestoreFoldersAtLogin, hkcuExAdvanced, Constants.PERSIST_BROWSERS);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowOSfiles, hkcuExAdvanced, Constants.SUPER_HIDDEN);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowNtfsInColor, hkcuExAdvanced, Constants.COMPRESSED_COLOR);

                // Properties
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideSecurity, hkcuExAdvanced, Constants.NO_SECURITY_TAB);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowHideCustomize, hkcuExAdvanced, Constants.NO_CUSTOMIZE_TAB);
            }

            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {

                // Libraries
                if (windowsOS > WindowsVer.Windows.XP) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.LIBRARY);
                    if (key != null) {
                        rbtnShowLibraries.IsChecked = true;
                    } else {
                        rbtnHideLibraries.IsChecked = true;
                    }
                } else {
                    tabItemLibrary.Visibility = Visibility.Collapsed;
                }

            }

            // Etc
            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkAddRecycleBinToMyComputer, hklmNamespace, Constants.RECYCLE_BIN);
            }

            RegistryKey hkcrCLSID = HKCR.OpenSubKey(@"CLSID");
            RegistryKey hklmCPNamespace = HKLM.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
            if (hkcrCLSID != null && hklmCPNamespace != null) {
                RegistryKey keyInHKCR = hkcrCLSID.OpenSubKey(Constants.REGEDIT);
                RegistryKey keyInHKLM = hklmCPNamespace.OpenSubKey(Constants.REGEDIT);
                if (keyInHKCR != null && keyInHKLM != null) {
                    chkAddRegeditToControlPanel.IsChecked = true;
                    keyInHKLM.Close();
                    keyInHKCR.Close();
                }
                hkcrCLSID.Close();
                hklmCPNamespace.Close();
            }
        }

        private void UpdateRegistryFromExplorer() {
            //Drive Letters
            using (RegistryKey hkcuCvExplorer = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
                int val = 0;
                if (rbtnShowDriveLetterAfterName.IsChecked == true) {
                    val = 0;
                } else if (rbtnHideDriveLetter.IsChecked == true) {
                    val = 2;
                } else if (rbtnShowDriveLetterBeforeName.IsChecked == true) {
                    val = 4;
                } else {
                    val = 0;
                }
                hkcuCvExplorer.SetValue(Constants.SHOW_DRIVE_LETTERS, val);
            }

            using (RegistryKey hkcuExAdvanced = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Advanced
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowMenuBar, hkcuExAdvanced, Constants.ALWAYS_SHOW_MENU);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideExtension, hkcuExAdvanced, Constants.HIDE_FILE_EXT);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkSelectItems, hkcuExAdvanced, Constants.AUTO_CHK_SELECT);

                int val = chkShowHiddenFilesFolders.IsChecked == true ? 1 : 2;
                hkcuExAdvanced.SetValue(Constants.HIDDEN, val);

                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkRestoreFoldersAtLogin, hkcuExAdvanced, Constants.PERSIST_BROWSERS);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowOSfiles, hkcuExAdvanced, Constants.SUPER_HIDDEN);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowNtfsInColor, hkcuExAdvanced, Constants.COMPRESSED_COLOR);

                // Properties
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideSecurity, hkcuExAdvanced, Constants.NO_SECURITY_TAB);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowHideCustomize, hkcuExAdvanced, Constants.NO_CUSTOMIZE_TAB);
            }

            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {

                // Libraries
                if (windowsOS > WindowsVer.Windows.XP) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.LIBRARY, true);
                    if (rbtnShowLibraries.IsChecked == true)
                        UIRegistryHandler.SetRegistryKeyFromBool(true, hklmNamespace, Constants.LIBRARY);
                    else if (rbtnHideLibraries.IsChecked == true)
                        UIRegistryHandler.SetRegistryKeyFromBool(false, hklmNamespace, Constants.LIBRARY);
                }
            }

            // Etc
            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace")) {
                UIRegistryHandler.SetRegistryKeyFromUICheckBox(chkAddRecycleBinToMyComputer, hklmNamespace, Constants.RECYCLE_BIN);
            }

            RegistryKey hkcrCLSID = HKCR.OpenSubKey(@"CLSID");
            RegistryKey hklmCPNamespace = HKLM.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
            if (chkAddRegeditToControlPanel.IsChecked == true) {
                if (hkcrCLSID == null)
                    hkcrCLSID = HKCR.CreateSubKey(@"CLSID");
                if (hklmCPNamespace == null)
                    hklmCPNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
                UIRegistryHandler.SetRegistryKeyFromBool(true, hkcrCLSID, Constants.REGEDIT);
                UIRegistryHandler.SetRegistryKeyFromBool(true, hklmCPNamespace, Constants.REGEDIT);
                hkcrCLSID.Close();
                hklmCPNamespace.Close();
            } else {
                if (hkcrCLSID != null) { 
                    UIRegistryHandler.SetRegistryKeyFromBool(false, hkcrCLSID, Constants.REGEDIT);
                    hkcrCLSID.Close();
                }
                if (hklmCPNamespace != null) { 
                    UIRegistryHandler.SetRegistryKeyFromBool(false, hklmCPNamespace, Constants.REGEDIT);
                    hklmCPNamespace.Close();
                }
            }
        }
    }
}
