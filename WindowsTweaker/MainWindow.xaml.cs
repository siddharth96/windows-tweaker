using System.IO;
using System.Linq;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFFolderBrowser;

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
        public readonly Color DEFAULT_SELECTION_COLOR = Color.FromArgb(255, 0, 102, 204);

        private void OnTabClicked(object sender, RoutedEventArgs e) {
            String tagVal = ((TabItem) sender).Tag.ToString();
            switch (tagVal) {
                case Constants.Explorer:
                    LoadExplorerTab();
                    break;

                case Constants.System:
                    break;

                case Constants.Display:
                    break;

                case Constants.RightClick:
                    break;

                case Constants.Places:
                    break;

                case Constants.Tasks:
                    LoadTaskTab();
                    break;

                case Constants.Features:
                    break;

                case Constants.Logon:
                    break;

                case Constants.Restrictions:
                    LoadRestrictionsTab();
                    break;

                case Constants.Maintenance:
                    break;

                case Constants.Utilities:
                    break;
            }
        }
        
        private void LoadRestrictionsTab() {

            using (RegistryKey hkcuExplorer = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Explorer
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideFileMenu, hkcuExplorer, Constants.NoFileMenu);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideFolderOpt, hkcuExplorer, Constants.NoFolderOption);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkRightClick, hkcuExplorer, Constants.NoViewContextMenu);
                //START MENU          
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideShutDownOpt, hkcuExplorer, Constants.NoClose);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideRecentDocs, hkcuExplorer, Constants.NoRecentDocsMenu);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideChangesStartMenu, hkcuExplorer, Constants.NoChangeStartMenu);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideLogOff, hkcuExplorer, Constants.NoLogOff);
                //System            
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableDeletionPrinters, hkcuExplorer, Constants.NoDeletePrinter);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableAddNewPrinter, hkcuExplorer, Constants.NoAddPrinter);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableWindUpdate, hkcuExplorer, Constants.NoWindowUpdate);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableWindowRegEditTool, hkcuExplorer, Constants.DisbaleRegistryTools);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableEditSettUnderMyComp, hkcuExplorer, Constants.NoPropertiesMyComputer);
            }
            using (RegistryKey hklmSystem = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                // Explorer
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideAppearanceOpt, hklmSystem, Constants.NoDispAppearancePage);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideChangeScreensaver, hklmSystem, Constants.NoDispScrSavPage);
                //START MENU      
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideControlPanel, hklmSystem, Constants.NoDispCpl);
                //System        
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableChangeToVirtualMem, hklmSystem, Constants.NoVirtMemPage);
            }
            using (RegistryKey hkcuExAdvanced = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Explorer
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideThumbNailCach, hkcuExAdvanced, Constants.DisableThumbnailCache);
                // Taskbar
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkTaskBarAnim, hkcuExAdvanced, Constants.TaskBarAnimations);
                if (windowsOS != WindowsVer.Windows.XP) {
                    UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowIconsTaskBar, hkcuExAdvanced, Constants.TaskBarSmallIcons);
                }

            }
            if (windowsOS > WindowsVer.Windows.XP && windowsOS < WindowsVer.Windows.Eight) {
                using (RegistryKey hkcuDWM = HKCU.CreateSubKey(@"Software\Policies\Microsoft\Windows\DWM")) {
                    // Explorer
                    UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHide3DFlip, hkcuDWM, Constants.DisAllowFlip_3D);
                }
            }
            else {
                chkHide3DFlip.Visibility = Visibility.Collapsed;
            }
            using (RegistryKey hkcuSystem = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                //System
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableTaskManager, hkcuSystem, Constants.DisableTaskMgr);
            }
            using (RegistryKey hklmParams = HKLM.CreateSubKey(@"SYSTEM\CurrentControlSet\services\LanmanServer\Parameters")) {
                //System
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableAdminShares, hklmParams, Constants.AutoShreWks, true);
            }
        }

        private void UpdateRegistryRestrictions() {

            using (RegistryKey hkcuExplorer = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Explorer
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideFileMenu, hkcuExplorer, Constants.NoFileMenu);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideFolderOpt, hkcuExplorer, Constants.NoFolderOption);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkRightClick, hkcuExplorer, Constants.NoViewContextMenu);
                //START MENU          
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideShutDownOpt, hkcuExplorer, Constants.NoClose);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideRecentDocs, hkcuExplorer, Constants.NoRecentDocsMenu);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideChangesStartMenu, hkcuExplorer, Constants.NoChangeStartMenu);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideLogOff, hkcuExplorer, Constants.NoLogOff);
                //System            
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableDeletionPrinters, hkcuExplorer, Constants.NoDeletePrinter);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableAddNewPrinter, hkcuExplorer, Constants.NoAddPrinter);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableWindUpdate, hkcuExplorer, Constants.NoWindowUpdate);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableWindowRegEditTool, hkcuExplorer, Constants.DisbaleRegistryTools);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableEditSettUnderMyComp, hkcuExplorer, Constants.NoPropertiesMyComputer);
            }
            using (RegistryKey hklmSystem = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                // Explorer
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideAppearanceOpt, hklmSystem, Constants.NoDispAppearancePage);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideChangeScreensaver, hklmSystem, Constants.NoDispScrSavPage);
                //START MENU      
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideControlPanel, hklmSystem, Constants.NoDispCpl);
                //System        
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableChangeToVirtualMem, hklmSystem, Constants.NoVirtMemPage);
            }
            using (RegistryKey hkcuExAdvanced = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideThumbNailCach, hkcuExAdvanced, Constants.DisableThumbnailCache);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkTaskBarAnim, hkcuExAdvanced, Constants.TaskBarAnimations);
                if (windowsOS != WindowsVer.Windows.XP) {
                    UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowIconsTaskBar, hkcuExAdvanced, Constants.TaskBarSmallIcons);
                }

            }
            if (windowsOS > WindowsVer.Windows.XP && windowsOS < WindowsVer.Windows.Eight) {
                using (RegistryKey hkcuDWM = HKCU.CreateSubKey(@"Software\Policies\Microsoft\Windows\DWM")) {
                    UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHide3DFlip, hkcuDWM, Constants.DisAllowFlip_3D);
                }
            }
            using (RegistryKey hkcuSystem = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                //System
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableTaskManager, hkcuSystem, Constants.DisableTaskMgr);
            }
            using (RegistryKey hklmParams = HKLM.CreateSubKey(@"SYSTEM\CurrentControlSet\services\LanmanServer\Parameters")) {
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkDisableAdminShares, hklmParams, Constants.AutoShreWks, true);
            }
        }

        private void LoadExplorerTab() {
            //Drive Letters
            using (RegistryKey hkcuCvExplorer = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
                int showDriveLetterVal = (int) hkcuCvExplorer.GetValue(Constants.ShowDriveLetters, 0);
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
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowMenuBar, hkcuExAdvanced, Constants.AlwaysShowMenu);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideExtension, hkcuExAdvanced, Constants.HideFileExt);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkSelectItems, hkcuExAdvanced, Constants.AutoChkSelect);

                int? val = (int?) hkcuExAdvanced.GetValue(Constants.Hidden);
                chkShowHiddenFilesFolders.IsChecked = val == 1;

                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkRestoreFoldersAtLogin, hkcuExAdvanced, Constants.PersistBrowsers);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowOSfiles, hkcuExAdvanced, Constants.SuperHidden);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowNtfsInColor, hkcuExAdvanced, Constants.CompressedColor);

                // Properties
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideSecurity, hkcuExAdvanced, Constants.NoSecurityTab);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkShowHideCustomize, hkcuExAdvanced, Constants.NoCustomizeTab);
            }

            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {
                // Libraries
                if (windowsOS > WindowsVer.Windows.XP) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.Library);
                    if (key != null) {
                        rbtnShowLibraries.IsChecked = true;
                    }
                    else {
                        rbtnHideLibraries.IsChecked = true;
                    }
                }
                else {
                    tabItemLibrary.Visibility = Visibility.Collapsed;
                }
            }

            // Etc
            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkAddRecycleBinToMyComputer, hklmNamespace, Constants.RecycleBin);
            }

            RegistryKey hkcrCLSID = HKCR.OpenSubKey(@"CLSID");
            RegistryKey hklmCPNamespace = HKLM.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
            if (hkcrCLSID != null && hklmCPNamespace != null) {
                RegistryKey keyInHKCR = hkcrCLSID.OpenSubKey(Constants.Regedit);
                RegistryKey keyInHKLM = hklmCPNamespace.OpenSubKey(Constants.Regedit);
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
                }
                else if (rbtnHideDriveLetter.IsChecked == true) {
                    val = 2;
                }
                else if (rbtnShowDriveLetterBeforeName.IsChecked == true) {
                    val = 4;
                }
                else {
                    val = 0;
                }
                hkcuCvExplorer.SetValue(Constants.ShowDriveLetters, val);
            }

            using (RegistryKey hkcuExAdvanced = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Advanced
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowMenuBar, hkcuExAdvanced, Constants.AlwaysShowMenu);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideExtension, hkcuExAdvanced, Constants.HideFileExt);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkSelectItems, hkcuExAdvanced, Constants.AutoChkSelect);

                int val = chkShowHiddenFilesFolders.IsChecked == true ? 1 : 2;
                hkcuExAdvanced.SetValue(Constants.Hidden, val);

                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkRestoreFoldersAtLogin, hkcuExAdvanced, Constants.PersistBrowsers);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowOSfiles, hkcuExAdvanced, Constants.SuperHidden);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowNtfsInColor, hkcuExAdvanced, Constants.CompressedColor);

                // Properties
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideSecurity, hkcuExAdvanced, Constants.NoSecurityTab);
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkShowHideCustomize, hkcuExAdvanced, Constants.NoCustomizeTab);
            }

            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {
                // Libraries
                if (windowsOS > WindowsVer.Windows.XP) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.Library, true);
                    if (rbtnShowLibraries.IsChecked == true)
                        UIRegistryHandler.SetRegistryKeyFromBool(true, hklmNamespace, Constants.Library);
                    else if (rbtnHideLibraries.IsChecked == true)
                        UIRegistryHandler.SetRegistryKeyFromBool(false, hklmNamespace, Constants.Library);
                }
            }

            // Etc
            using (RegistryKey hklmNamespace = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace")) {
                UIRegistryHandler.SetRegistryKeyFromUICheckBox(chkAddRecycleBinToMyComputer, hklmNamespace, Constants.RecycleBin);
            }

            RegistryKey hkcrCLSID = HKCR.OpenSubKey(@"CLSID");
            RegistryKey hklmCPNamespace =
                HKLM.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
            if (chkAddRegeditToControlPanel.IsChecked == true) {
                if (hkcrCLSID == null)
                    hkcrCLSID = HKCR.CreateSubKey(@"CLSID");
                if (hklmCPNamespace == null)
                    hklmCPNamespace =
                        HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
                UIRegistryHandler.SetRegistryKeyFromBool(true, hkcrCLSID, Constants.Regedit);
                UIRegistryHandler.SetRegistryKeyFromBool(true, hklmCPNamespace, Constants.Regedit);
                hkcrCLSID.Close();
                hklmCPNamespace.Close();
            }
            else {
                if (hkcrCLSID != null) {
                    UIRegistryHandler.SetRegistryKeyFromBool(false, hkcrCLSID, Constants.Regedit);
                    hkcrCLSID.Close();
                }
                if (hklmCPNamespace != null) {
                    UIRegistryHandler.SetRegistryKeyFromBool(false, hklmCPNamespace, Constants.Regedit);
                    hklmCPNamespace.Close();
                }
            }
        }

        private void LoadSystemTab() {
            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                //Shutdown Configuration
                String val;
                try {
                    val = (String) hkcuDesktop.GetValue(Constants.AutoEndTasks);
                    rbtnShutdownImmediately.IsChecked = Utils.StringToBool(val);
                }
                catch (InvalidCastException) {
                    rbtnShutdownImmediately.Visibility = Visibility.Collapsed;
                }

                val = (String) hkcuDesktop.GetValue(Constants.WaitToKillAppTimeout);
                if (val != null) {
                    rbtnShutdownAfterWaiting.IsChecked = !rbtnShutdownImmediately.IsChecked;
                }
                else {
                    if (rbtnShutdownImmediately.IsChecked != true)
                        rbtnShutdownAfterWaiting.IsChecked = true;
                }
            }

            using (RegistryKey hklmWinInstaller = HKLM.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer")) {
                // Windows Installer
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableWinInstaller, hklmWinInstaller, Constants.DisableMsi);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkElevatedInstall, hklmWinInstaller, Constants.AlwaysInstallElevated);
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkDisableSysRestoreInstall, hklmWinInstaller, Constants.LimitSystemRestore);
            }

            using (RegistryKey hklmCVWinNT = HKLM.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")) {
                // Registration
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtOwnerName, hklmCVWinNT, Constants.RegisteredOwner);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtCompanyName, hklmCVWinNT, Constants.RegisteredOrg);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtProductId, hklmCVWinNT, Constants.ProductId);
            }

            using (RegistryKey hklmOEM = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                // Edit Information
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtManufacturer, hklmOEM, Constants.Manufacturer);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtModel, hklmOEM, Constants.Model);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtSupportPhone, hklmOEM, Constants.SupportPhone);
                UIRegistryHandler.SetUITextBoxFromRegistryValue(txtSupportUrl, hklmOEM, Constants.SupportUrl);

                // Logo
                String logoUrl = (String) hklmOEM.GetValue(Constants.Logo);
                if (logoUrl != null && logoUrl.Length > 0) {
                    btnDeleteLogo.IsEnabled = true;
                    try {
                        // TODO: Add Image Box
                    }
                    catch (UriFormatException) {
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
                    hkcuDesktop.SetValue(Constants.AutoEndTasks, val);
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
                    hklmWinInstaller.SetValue(Constants.DisableMsi, 2);
                }
                else {
                    Utils.SafeDeleteRegistryValue(hklmWinInstaller, Constants.DisableMsi);
                }

                if (chkElevatedInstall.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.AlwaysInstallElevated, 1);
                }
                else {
                    Utils.SafeDeleteRegistryValue(hklmWinInstaller, Constants.AlwaysInstallElevated);
                }

                if (chkDisableSysRestoreInstall.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.LimitSystemRestore, 1);
                }
                else {
                    Utils.SafeDeleteRegistryValue(hklmWinInstaller, Constants.LimitSystemRestore);
                }

                // TODO : Cleanup the leftovers
            }

            using (RegistryKey hklmCVWinNT = HKLM.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")) {
                // Registration
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtOwnerName, hklmCVWinNT, Constants.RegisteredOwner);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtCompanyName, hklmCVWinNT, Constants.RegisteredOrg);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtProductId, hklmCVWinNT, Constants.ProductId);
            }

            using (RegistryKey hklmOEM = HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                // Edit Information
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtManufacturer, hklmOEM, Constants.Manufacturer);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtModel, hklmOEM, Constants.Model);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtSupportPhone, hklmOEM, Constants.SupportPhone);
                UIRegistryHandler.SetRegistryValueFromUITextBox(txtSupportUrl, hklmOEM, Constants.SupportUrl);
            }
        }

        private void LoadDisplayTab() {
            // Display Settings
            using (RegistryKey hkcuWinMet = HKCU.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                String val = (String) hkcuWinMet.GetValue(Constants.MinAnimate);
                chkWindowAnim.IsChecked = Utils.StringToBool(val);
            }

            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                String val = (String) hkcuDesktop.GetValue(Constants.DragFullWin);
                chkShowWindowDrag.IsChecked = Utils.StringToBool(val);

                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkWindowVersion, hkcuDesktop, Constants.PaintDesktopVer);

                // Alt-Tab
                // TODO : Alt-Tab implementation
            }

            // Selection Color
            using (RegistryKey hkcuColors = HKCU.CreateSubKey(@"Control Panel\Colors")) {
                String val = (String) hkcuColors.GetValue(Constants.SelectionColor);
                String[] rgb = val.Split(' ');
                Color selectionColor;
                if (rgb.Length == 3) {
                    selectionColor = Color.FromRgb(Byte.Parse(rgb[0]), Byte.Parse(rgb[1]), Byte.Parse(rgb[2]));
                }
                else {
                    selectionColor = DEFAULT_SELECTION_COLOR;
                }
                rectSelectionColor.Fill = new SolidColorBrush(selectionColor);
            }
        }

        private void UpdateRegistryFromDisplay() {
            // Display Settings
            using (RegistryKey hkcuWinMet = HKCU.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                String val = Utils.BoolToString(chkWindowAnim.IsChecked);
                hkcuWinMet.SetValue(Constants.MinAnimate, val);
            }

            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                String val = Utils.BoolToString(chkShowWindowDrag.IsChecked);
                hkcuDesktop.SetValue(Constants.DragFullWin, val);

                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkWindowVersion, hkcuDesktop, Constants.PaintDesktopVer);

                // Alt-Tab
                // TODO : Alt-Tab implementation
            }

            // Selection Color
            using (RegistryKey hkcuColors = HKCU.CreateSubKey(@"Control Panel\Colors")) {
                Color selectionColor = ((SolidColorBrush) rectSelectionColor.Fill).Color;

                String val = String.Format("{0} {1} {2}", selectionColor.R, selectionColor.G, selectionColor.B);
                hkcuColors.SetValue(Constants.SelectionColor, val);
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
                String copyToVal = (String) keyToValidate.GetValue(Constants.CopyToId);
                if (copyToVal != null && !keyToValidate.Name.Equals(Constants.CopyTo)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.CopyTo);
                    keyToValidate.SetValue("", Constants.CopyToId);
                    continue;
                }
                String moveToVal = (String) keyToValidate.GetValue(Constants.MoveToId);
                if (moveToVal != null && !keyToValidate.Name.Equals(Constants.MoveTo)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.MoveTo);
                    keyToValidate.SetValue("", Constants.MoveToId);
                    continue;
                }
                String sendToVal = (String) keyToValidate.GetValue(Constants.SendToId);
                if (sendToVal != null && !keyToValidate.Name.Equals(Constants.SendTo)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.SendTo);
                    keyToValidate.SetValue("", Constants.SendToId);
                    continue;
                }
            }
        }

        private void LoadPlacesTab() {
            // Power Button
            using (RegistryKey hkcuExAdvanced = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                int val = (int) hkcuExAdvanced.GetValue(Constants.StartPowerBtnAction);
                switch (val) {
                    case 2:
                        cmboBxPowerBtnAction.SelectedIndex = 0;
                        break;
                    case 64:
                        cmboBxPowerBtnAction.SelectedIndex = 1;
                        break;
                    case 16:
                        cmboBxPowerBtnAction.SelectedIndex = 2;
                        break;
                    case 4:
                        cmboBxPowerBtnAction.SelectedIndex = 3;
                        break;
                    case 512:
                        cmboBxPowerBtnAction.SelectedIndex = 4;
                        break;
                    case 1:
                        cmboBxPowerBtnAction.SelectedIndex = 5;
                        break;
                    case 256:
                        cmboBxPowerBtnAction.SelectedIndex = 6;
                        break;
                }
            }
        }

        private void LoadRightClickTab() {
            // General
            using (RegistryKey hkcrContextMenuHandlers = HKCR.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers")) {
                ValidateAndFixKeys(hkcrContextMenuHandlers);
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkCopyToFolder, hkcrContextMenuHandlers, Constants.CopyTo);
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkMoveToFolder, hkcrContextMenuHandlers, Constants.MoveTo);
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkSendTo, hkcrContextMenuHandlers, Constants.SendTo);
            }

            using (RegistryKey hkcrFileShell = HKCR.CreateSubKey(@"*\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkOpenWithNotepad, hkcrFileShell, Constants.OpenNotepad);
            }

            using (RegistryKey hkcrShell = HKCR.CreateSubKey(@"Directory\Background\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkControlPanelInDesktopMenu, hkcrShell, Constants.ControlPanel);
            }

            using (RegistryKey hkcrDirShell = HKCR.CreateSubKey(@"Directory\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkOpenCmdPrompt, hkcrDirShell, Constants.OpenCmd);
            }

            using (RegistryKey hkcrDriveShell = HKCR.CreateSubKey(@"Drive\shell")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryKey(chkAddDefragInMenu, hkcrDriveShell, Constants.RunAs);
            }

            using (
                RegistryKey hklmExAdvanced =
                    HKLM.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkEncryptAndDecrypt, hklmExAdvanced, Constants.EncryptCtxMenu);
            }

            using (RegistryKey hklmClasses = HKLM.CreateSubKey(@"Software\Classes")) {
                using (RegistryKey hklmDotTxt = HKLM.CreateSubKey(@"Software\Classes\.txt")) {
                    String txtFile = (String) hklmDotTxt.GetValue("");
                    RegistryKey hklmTxt = hklmClasses.CreateSubKey(txtFile);
                    RegistryKey hklmDotTextShell = hklmTxt.OpenSubKey(Constants.Shell, true);

                    if (hklmDotTextShell != null) {
                        string[] subKeysShell = hklmDotTextShell.GetSubKeyNames();
                        RegistryKey hklmTextFile = hklmClasses.CreateSubKey(Constants.TextFile);
                        RegistryKey hklmTextShell = hklmTextFile.OpenSubKey(Constants.Shell, true);
                        if (hklmTextShell != null) {
                            chkCopyContents.IsChecked = hklmDotTextShell.GetValue(Constants.CopyContents) != null
                                                        || hklmTextShell.GetValue(Constants.CopyContents) != null;
                        }
                        else {
                            chkCopyContents.IsChecked = false;
                        }
                        hklmTxt.Close();
                        hklmDotTextShell.Close();
                        hklmTextFile.Close();
                        if (hklmTextShell != null)
                            hklmTextShell.Close();
                    }
                }
            }
        }

        private void UpdateRegistryFromRightClick() {}

        private void OnButtonSetupGodModeClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true) {
                String selectedFolderName = folderBrowserDialog.FileName;
                if (Utils.IsEmptyDirectory(selectedFolderName)) {
                    if (Directory.GetParent(selectedFolderName) != null) {
                        String godModeFolderPath = selectedFolderName + Constants.GodModeKey;
                        if (Directory.Exists(godModeFolderPath))
                            Directory.Delete(godModeFolderPath);
                            DirectoryInfo selectedFolderDirectoryInfo = new DirectoryInfo(selectedFolderName);
                            String parentDir = selectedFolderDirectoryInfo.Parent.FullName;
                            try {
                                selectedFolderDirectoryInfo.Delete(true);
                                Directory.CreateDirectory(godModeFolderPath);
                                MessageBox.Show("God Mode folder has been successfully created in " + parentDir +
                                                "\n\nPlease note that if on clicking the folder you get an error, " +
                                                "then you need to refresh that window for changes to be reflected.",
                                    Constants.SuccessMsgTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            catch (UnauthorizedAccessException ex) {
                                MessageBox.Show("Permission Denied!", Constants.ErrorMsgTitle, MessageBoxButton.OK, 
                                    MessageBoxImage.Error);
                            }
                    }
                    else {
                        MessageBox.Show("You can't make " + selectedFolderName + " a \'God\' folder. " +
                                        "\n Please select an empty folder or create a new one", Constants.WarningMsgTitle,
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else {
                    MessageBox.Show(selectedFolderName + " is not an empty folder.\n You must create an " +
                                                  "empty folder, to set God Mode", Constants.WarningMsgTitle, 
                                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }


        private void LoadTaskTab() {
            dateTimePickerScheduleShutdown.Value = DateTime.Now;
        }

        private void OnScheduleShutdownButtonClick(object sender, RoutedEventArgs e) {
            DateTime? selectedDateTime = dateTimePickerScheduleShutdown.Value;
            if (selectedDateTime.HasValue) {
                String param = null;
                switch (cmboBxShutdownAction.SelectedIndex) {
                    case 0: param = "/s";
                        break;
                    case 1: param = "/r";
                        break;
                }
                DateTime nowTime = DateTime.Now;
                TimeSpan gap = selectedDateTime.Value.Subtract(nowTime);
                long timeout = (gap.Days * 86400) + (gap.Hours * 3600) + (gap.Minutes * 60) + gap.Seconds;
                if (timeout >= 0) {
                    String shutdwnComd = String.Format("shutdown {0} /t {1}", param, timeout);
                    Utils.ExecuteCmd(shutdwnComd);
                    MessageBox.Show("Shutdown has been scheduled on " + selectedDateTime.Value.ToString("MMMM d, yyyy ") + " at " + selectedDateTime.Value.ToString("h:m tt"),
                        Constants.SuccessMsgTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else {
                    MessageBox.Show("Invalid Timeout!\n The time can\'t be less than the current time. Also the time can\'t" +
                                    " exceed the limit of 10 years.", Constants.AlertMsgTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else {
                MessageBox.Show("Please select a date!", Constants.WarningMsgTitle, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void OnCancelShutdownButtonClick(object sender, RoutedEventArgs e) {
            Utils.ExecuteCmd("shutdown /a");
            MessageBox.Show("A scheduled shutdown has been cancelled", Constants.SuccessMsgTitle, MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OnButtonBrowseSpecialFolderParentClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDlg = new WPFFolderBrowserDialog();
            folderBrowserDlg.Title = "Select a Parent Folder";
            if (folderBrowserDlg.ShowDialog() == true) {
                String parentPath = folderBrowserDlg.FileName;
                String createCmd = String.Format("md \"\\\\.\\{0}\\{1}\"", parentPath, cmboBxSpecialFolderNames.SelectionBoxItem);
                Utils.ExecuteCmd(createCmd);
                MessageBox.Show(cmboBxSpecialFolderNames.SelectionBoxItem + " created Successfully at " + parentPath,
                    Constants.SuccessMsgTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnButtonDeleteSpecialFolderClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true) {
                String path = folderBrowserDialog.FileName;
                String createCmd = String.Format("rd \"\\\\.\\{0}\"", path);
                Utils.ExecuteCmd(createCmd);
            }
        }
    }
}