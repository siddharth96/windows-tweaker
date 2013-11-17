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

        private void LoadSystemTab() {
            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                //Shutdown Configuration
                String val;
                try {
                    val = (String)hkcuDesktop.GetValue(Constants.AUTO_END_TASKS);
                    rbtnShutdownImmediately.IsChecked = Utils.StringToBool(val);
                } catch (InvalidCastException) {
                    rbtnShutdownImmediately.Visibility = Visibility.Collapsed;
                }

                val = (String)hkcuDesktop.GetValue(Constants.WAIT_TO_KILL_APP_TIMEOUT);
                if (val != null) {
                    rbtnShutdownAfterWaiting.IsChecked = !rbtnShutdownImmediately.IsChecked;
                } else {
                    if (rbtnShutdownImmediately.IsChecked != true)
                        rbtnShutdownAfterWaiting.IsChecked = true;
                }

            }

            using (RegistryKey hklmWinInstaller = HKLM.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer")) {
                // Windows Installer
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableWinInstaller, hklmWinInstaller, Constants.DISABLE_MSI);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkElevatedInstall, hklmWinInstaller, Constants.ALWAYS_INSTALL_ELEVATED);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableSysRestoreInstall, hklmWinInstaller, Constants.LIMIT_SYSTEM_RESTORE);
            }

            using (RegistryKey hklmCVWinNT = HKLM.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")) {
                // Registration
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtOwnerName, hklmCVWinNT, Constants.REGISTERED_OWNER);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtCompanyName, hklmCVWinNT, Constants.REGISTERED_ORG);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtProductId, hklmCVWinNT, Constants.PRODUCT_ID);
            }

            using (RegistryKey hklmOEM = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                // Edit Information
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtManufacturer, hklmOEM, Constants.MANUFACTURER);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtModel, hklmOEM, Constants.MODEL);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtSupportPhone, hklmOEM, Constants.SUPPORT_PHONE);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtSupportUrl, hklmOEM, Constants.SUPPORT_URL);

                // Logo
                String logoUrl = (String)hklmOEM.GetValue(Constants.LOGO);
                if (logoUrl != null && logoUrl.Length > 0) {
                    btnDeleteLogo.IsEnabled = true;
                    try {
                        // TODO: Add Image Box
                    } catch (UriFormatException) {
                        // TODO : Append c: to path
                    }
                }
            }
        }

        private void UpdateRegistryFromSystem() {

            //Shutdown Configuration            
            if (rbtnShutdownImmediately.Visibility == Visibility.Visible) {
                using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                    String val = Utils.BoolToString(rbtnShutdownImmediately.IsChecked);
                    hkcuDesktop.SetValue(Constants.AUTO_END_TASKS, val);
                    if (rbtnShutdownAfterWaiting.IsChecked == true) {
                        // TODO : set timeout
                    }
                }
            }

            using (RegistryKey hklmWinInstaller = HKLM.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer")) {
                //RegistryKey hklmPWindows = HKLM.CreateSubKey(@"Software\Policies\Microsoft\Windows");

                bool allFalse = false;
                // Windows Installer
                if (chkDisableWinInstaller.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.DISABLE_MSI, 2);
                } else {
                    Utils.SafeDeleteRegistryValue(hklmWinInstaller, Constants.DISABLE_MSI);
                }

                if (chkElevatedInstall.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.ALWAYS_INSTALL_ELEVATED, 1);
                } else {
                    Utils.SafeDeleteRegistryValue(hklmWinInstaller, Constants.ALWAYS_INSTALL_ELEVATED);
                }

                if (chkDisableSysRestoreInstall.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.LIMIT_SYSTEM_RESTORE, 1);
                } else {
                    Utils.SafeDeleteRegistryValue(hklmWinInstaller, Constants.LIMIT_SYSTEM_RESTORE);
                }

                // TODO : Cleanup the leftovers
            }

            using (RegistryKey hklmCVWinNT = HKLM.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")) {
                // Registration
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtOwnerName, hklmCVWinNT, Constants.REGISTERED_OWNER);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtCompanyName, hklmCVWinNT, Constants.REGISTERED_ORG);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtProductId, hklmCVWinNT, Constants.PRODUCT_ID);
            }

            using (RegistryKey hklmOEM = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                // Edit Information
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtManufacturer, hklmOEM, Constants.MANUFACTURER);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtModel, hklmOEM, Constants.MODEL);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtSupportPhone, hklmOEM, Constants.SUPPORT_PHONE);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtSupportUrl, hklmOEM, Constants.SUPPORT_URL);
            }


        }

        private void LoadDisplayTab() {

            // Display Settings
            using (RegistryKey hkcuWinMet = HKCU.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                String val = (String)hkcuWinMet.GetValue(Constants.MIN_ANIMATE);
                chkWindowAnim.IsChecked = Utils.StringToBool(val);
            }

            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                String val = (String)hkcuDesktop.GetValue(Constants.DRAG_FULL_WIN);
                chkShowWindowDrag.IsChecked = Utils.StringToBool(val);

                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkWindowVersion, hkcuDesktop, Constants.PAINT_DESKTOP_VER);

                // Alt-Tab
                // TODO : Alt-Tab implementation
            }

            // Selection Color
            using (RegistryKey hkcuColors = HKCU.CreateSubKey(@"Control Panel\Colors")) {
                String val = (String)hkcuColors.GetValue(Constants.SELECTION_COLOR);
                String[] rgb = val.Split(' ');
                Color selectionColor;
                if (rgb.Length == 3) {
                    selectionColor = Color.FromRgb(Byte.Parse(rgb[0]), Byte.Parse(rgb[1]), Byte.Parse(rgb[2]));
                } else {
                    selectionColor = Constants.DEFAULT_SELECTION_COLOR;
                }
                rectSelectionColor.Fill = new SolidColorBrush(selectionColor);
            }
        }

        private void UpdateRegistryFromDisplay() {
            // Display Settings
            using (RegistryKey hkcuWinMet = HKCU.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                String val = Utils.BoolToString(chkWindowAnim.IsChecked);
                hkcuWinMet.SetValue(Constants.MIN_ANIMATE, val);
            }

            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                String val = Utils.BoolToString(chkShowWindowDrag.IsChecked);
                hkcuDesktop.SetValue(Constants.DRAG_FULL_WIN, val);

                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkWindowVersion, hkcuDesktop, Constants.PAINT_DESKTOP_VER);

                // Alt-Tab
                // TODO : Alt-Tab implementation
            }

            // Selection Color
            using (RegistryKey hkcuColors = HKCU.CreateSubKey(@"Control Panel\Colors")) {
                Color selectionColor = ((SolidColorBrush)rectSelectionColor.Fill).Color;

                String val = String.Format("{0} {1} {2}", selectionColor.R, selectionColor.G, selectionColor.B);
                hkcuColors.SetValue(Constants.SELECTION_COLOR, val);
            }
        }

        /// <summary>
        /// Checks the name of RegistryKey for Copy-To, Move-To and Send-To Registry Keys, and corrects them if they are wrong.
        /// These names are case-sensitive, eg., "Copy To" will work, but "Copy to" as registry-key's value will fail,
        /// and unfortunately some 3rd party tweaking softwares put in the latter one in the Registry.
        /// </summary>
        private void ValidateAndFixKeys(RegistryKey rootKey) {
            String[] subKeyNames = rootKey.GetSubKeyNames();
            foreach (String keyName in subKeyNames) {
                RegistryKey keyToValidate = rootKey.OpenSubKey(keyName, true);
                String copyToVal = (String)keyToValidate.GetValue(Constants.COPY_TO_ID);
                if (copyToVal != null && !keyToValidate.Name.Equals(Constants.COPY_TO)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.COPY_TO);
                    keyToValidate.SetValue("", Constants.COPY_TO_ID);
                    continue;
                }
                String moveToVal = (String)keyToValidate.GetValue(Constants.MOVE_TO_ID);
                if (moveToVal != null && !keyToValidate.Name.Equals(Constants.MOVE_TO)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.MOVE_TO);
                    keyToValidate.SetValue("", Constants.MOVE_TO_ID);
                    continue;
                }
                String sendToVal = (String)keyToValidate.GetValue(Constants.SEND_TO_ID);
                if (sendToVal != null && !keyToValidate.Name.Equals(Constants.SEND_TO)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.SEND_TO);
                    keyToValidate.SetValue("", Constants.SEND_TO_ID);
                    continue;
                }
            }
        }

        private void LoadRightClickTab() {
            // General
            using (RegistryKey hkcrContextMenuHandlers = HKCR.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers")) {
                ValidateAndFixKeys(hkcrContextMenuHandlers);
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkCopyToFolder, hkcrContextMenuHandlers, Constants.COPY_TO);
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkMoveToFolder, hkcrContextMenuHandlers, Constants.MOVE_TO);
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkSendTo, hkcrContextMenuHandlers, Constants.SEND_TO);
            }

            using (RegistryKey hkcrFileShell = HKCR.CreateSubKey(@"*\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkOpenWithNotepad, hkcrFileShell, Constants.OPEN_NOTEPAD);
            }

            using (RegistryKey hkcrShell = HKCR.CreateSubKey(@"Directory\Background\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkControlPanelInDesktopMenu, hkcrShell, Constants.CONTROL_PANEL);
            }

            using (RegistryKey hkcrDirShell = HKCR.CreateSubKey(@"Directory\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkOpenCmdPrompt, hkcrDirShell, Constants.OPEN_CMD);
            }

            using (RegistryKey hkcrDriveShell = HKCR.CreateSubKey(@"Drive\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkAddDefragInMenu, hkcrDriveShell, Constants.RUN_AS);
            }

            using (RegistryKey hklmExAdvanced = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkEncryptAndDecrypt, hklmExAdvanced, Constants.ENCRYPT_CTX_MENU);
            }

            using (RegistryKey hklmClasses = HKLM.CreateSubKey(@"Software\Classes")) {
                using (RegistryKey hklmDotTxt = HKLM.CreateSubKey(@"Software\Classes\.txt")) {
                    String txtFile = (String)hklmDotTxt.GetValue("");
                    RegistryKey hklmTxt = hklmClasses.CreateSubKey(txtFile);
                    RegistryKey hklmDotTextShell = hklmTxt.OpenSubKey(Constants.SHELL, true);

                    string[] subKeysShell = hklmDotTextShell.GetSubKeyNames();
                    RegistryKey hklmTextFile = hklmClasses.CreateSubKey(Constants.TEXT_FILE);
                    RegistryKey hklmTextShell = hklmTextFile.OpenSubKey(Constants.SHELL, true);
                    chkCopyContents.IsChecked = hklmDotTextShell.GetValue(Constants.COPY_CONTENTS) != null
                        || hklmTextShell.GetValue(Constants.COPY_CONTENTS) != null;
                    hklmTxt.Close();
                    hklmDotTextShell.Close();
                    hklmTextFile.Close();
                    hklmTextShell.Close();
                }
            }
        }

        private void UpdateRegistryFromRightClick() {
        }
    }
}
