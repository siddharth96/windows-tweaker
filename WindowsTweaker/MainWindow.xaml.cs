using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using WindowsTweaker.AppTasks;
using WindowsTweaker.Models;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFFolderBrowser;
using File = System.IO.File;

namespace WindowsTweaker {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            message = new Message(msgContainerBorder, txtMsg);
            sendToBackgroundWorker = (BackgroundWorker)this.FindResource("sendToBackgroundWorker");
        }

        private readonly RegistryKey HKCU = Registry.CurrentUser;
        private readonly RegistryKey HKLM = Registry.LocalMachine;
        private readonly RegistryKey HKCR = Registry.ClassesRoot;
        private readonly WindowsVer.Windows windowsOS = WindowsVer.Instance.GetName();
        private readonly Color DEFAULT_SELECTION_COLOR = Color.FromArgb(255, 0, 102, 204);
        private readonly Message message;
        private readonly BackgroundWorker sendToBackgroundWorker;

        #region Common Code
        private void OnTabLoaded(object sender, RoutedEventArgs e) {
            string tagVal = ((TabItem) sender).Tag.ToString();
            switch (tagVal) {
                case Constants.Explorer:
                    LoadExplorerTab();
                    break;

                case Constants.System:
                    LoadSystemTab();
                    break;

                case Constants.Display:
                    break;

                case Constants.RightClick:
                    LoadRightClickTab();
                    break;

                case Constants.Places:
                    break;

                case Constants.Tasks:
                    LoadTaskTab();
                    break;

                case Constants.Features:
                    LoadFeaturesTab();
                    break;


                case Constants.Logon:
                    LoadLogonTab();
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

        private void OnCheckBoxClick(object sender, RoutedEventArgs e) {
            CheckBox chkBox = (CheckBox)sender;
            chkBox.Tag = Constants.HasUserInteracted;
        }

        private void OnMessageViewCloseGridMouseDown(object sender, MouseButtonEventArgs e) {
            message.Hide();
        }

        private void OnMessageViewCloseTouchDown(object sender, TouchEventArgs e) {
            message.Hide();
        }

        private void StartProcess(object sender, RoutedEventArgs e) {
            try {
                Process.Start(((Hyperlink)sender).Tag.ToString());
            } catch (Win32Exception) {
                if (((Hyperlink)sender).Tag.ToString().Contains("SystemPropertiesProtection")) {
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\system32\Restore\rstrui.exe");
                } else
                    message.Error("This option is not available in your version of Windows");
            } catch (Exception) { }
        }
        #endregion

        #region Logon
        private void LoadLogonTab()
        {
            using (RegistryKey hklmWinLogon = HKLM.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon"))
            {
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkEnableAutoLogin, hklmWinLogon,"");
            }
        }
        #endregion

        #region Restrictions
        private void LoadRestrictionsTab()
        {


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
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkHideThumbNailCache, hkcuExAdvanced, Constants.DisableThumbnailCache);
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
                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkHideThumbNailCache, hkcuExAdvanced, Constants.DisableThumbnailCache);
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
        #endregion

        #region Explorer
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
        #endregion

        #region System
        private void LoadSystemTab() {
            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                //Shutdown Configuration
                string val;
                try {
                    val = (string) hkcuDesktop.GetValue(Constants.AutoEndTasks);
                    rbtnShutdownImmediately.IsChecked = Utils.StringToBool(val);
                }
                catch (InvalidCastException) {
                    rbtnShutdownImmediately.Visibility = Visibility.Collapsed;
                }

                val = (string) hkcuDesktop.GetValue(Constants.WaitToKillAppTimeout);
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
                string logoUrl = (string) hklmOEM.GetValue(Constants.Logo);
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
                    string val = Utils.BoolToString(rbtnShutdownImmediately.IsChecked);
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
        #endregion

        #region Display
        private void LoadDisplayTab() {
            // Display Settings
            using (RegistryKey hkcuWinMet = HKCU.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                string val = (string) hkcuWinMet.GetValue(Constants.MinAnimate);
                chkWindowAnim.IsChecked = Utils.StringToBool(val);
            }

            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                string val = (string) hkcuDesktop.GetValue(Constants.DragFullWin);
                chkShowWindowDrag.IsChecked = Utils.StringToBool(val);

                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkWindowVersion, hkcuDesktop, Constants.PaintDesktopVer);

                // Alt-Tab
                // TODO : Alt-Tab implementation
            }

            // Selection Color
            using (RegistryKey hkcuColors = HKCU.CreateSubKey(@"Control Panel\Colors")) {
                string val = (string) hkcuColors.GetValue(Constants.SelectionColor);
                string[] rgb = val.Split(' ');
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
                string val = Utils.BoolToString(chkWindowAnim.IsChecked);
                hkcuWinMet.SetValue(Constants.MinAnimate, val);
            }

            using (RegistryKey hkcuDesktop = HKCU.CreateSubKey(@"Control Panel\Desktop")) {
                string val = Utils.BoolToString(chkShowWindowDrag.IsChecked);
                hkcuDesktop.SetValue(Constants.DragFullWin, val);

                UIRegistryHandler.SetRegistryValueFromUICheckBox(chkWindowVersion, hkcuDesktop, Constants.PaintDesktopVer);

                // Alt-Tab
                // TODO : Alt-Tab implementation
            }

            // Selection Color
            using (RegistryKey hkcuColors = HKCU.CreateSubKey(@"Control Panel\Colors")) {
                Color selectionColor = ((SolidColorBrush) rectSelectionColor.Fill).Color;

                string val = String.Format("{0} {1} {2}", selectionColor.R, selectionColor.G, selectionColor.B);
                hkcuColors.SetValue(Constants.SelectionColor, val);
            }
        }
        #endregion

        #region Places
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
        #endregion

        #region Right-Click
        /// <summary>
        /// Checks the name of RegistryKey for Copy-To, Move-To and Send-To Registry Keys, and corrects them if they are wrong.
        /// These names are case-sensitive, eg., "Copy To" will work, but "Copy to" as registry-key's value will fail,
        /// and unfortunately some 3rd party tweaking softwares put in the latter one in the Registry.
        /// </summary>
        private void ValidateAndFixKeys(RegistryKey rootKey)
        {
            string[] subKeyNames = rootKey.GetSubKeyNames();
            foreach (string keyName in subKeyNames)
            {
                RegistryKey keyToValidate = rootKey.OpenSubKey(keyName, true);
                string copyToVal = (string)keyToValidate.GetValue(Constants.CopyToId);
                if (copyToVal != null && !keyToValidate.Name.Equals(Constants.CopyTo))
                {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.CopyTo);
                    keyToValidate.SetValue("", Constants.CopyToId);
                    continue;
                }
                string moveToVal = (string)keyToValidate.GetValue(Constants.MoveToId);
                if (moveToVal != null && !keyToValidate.Name.Equals(Constants.MoveTo))
                {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.MoveTo);
                    keyToValidate.SetValue("", Constants.MoveToId);
                    continue;
                }
                string sendToVal = (string)keyToValidate.GetValue(Constants.SendToId);
                if (sendToVal != null && !keyToValidate.Name.Equals(Constants.SendTo))
                {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.SendTo);
                    keyToValidate.SetValue("", Constants.SendToId);
                    continue;
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
                    string txtFile = (string) hklmDotTxt.GetValue("");
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

            // Send To
            LoadSendTo();
        }

        private void UpdateRegistryFromRightClick() {}
        #endregion

        #region Places -> GodMode
        private void OnButtonSetupGodModeClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true) {
                string selectedFolderName = folderBrowserDialog.FileName;
                if (Utils.IsEmptyDirectory(selectedFolderName)) {
                    if (Directory.GetParent(selectedFolderName) != null) {
                        string godModeFolderPath = selectedFolderName + Constants.GodModeKey;
                        if (Directory.Exists(godModeFolderPath))
                            Directory.Delete(godModeFolderPath);
                            DirectoryInfo selectedFolderDirectoryInfo = new DirectoryInfo(selectedFolderName);
                            string parentDir = selectedFolderDirectoryInfo.Parent.FullName;
                            try {
                                selectedFolderDirectoryInfo.Delete(true);
                                Directory.CreateDirectory(godModeFolderPath);
                                message.Success("Successfully created folder in " + parentDir +
                                                ". Please note that if on clicking the folder you get an error, " +
                                                "then you need to refresh that window for changes to be reflected.");
                            }
                            catch (UnauthorizedAccessException ex) {
                                message.Error("Permission Denied!");
                            }
                    }
                    else {
                        message.Error("You can't make " + selectedFolderName + " a \'God\' folder. " +
                                      "Please select an empty folder or create a new one");
                    }
                }
                else {
                    message.Error(selectedFolderName + " is not an empty folder. You must create an " +
                                  "empty folder, to set God Mode");
                }
            }
        }
        #endregion

        #region Task
        private void LoadTaskTab() {
            dateTimePickerScheduleShutdown.Value = DateTime.Now;
        }

        private void OnScheduleShutdownButtonClick(object sender, RoutedEventArgs e) {
            DateTime? selectedDateTime = dateTimePickerScheduleShutdown.Value;
            if (selectedDateTime.HasValue) {
                string param = null;
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
                    string shutdwnComd = String.Format("shutdown {0} /t {1}", param, timeout);
                    ProcessWrapper.ExecuteDosCmd(shutdwnComd);
                    message.Success("Shutdown has been scheduled on " + selectedDateTime.Value.ToString("MMMM d, yyyy ") 
                        + " at " + selectedDateTime.Value.ToString("hh:mm tt"));
                }
                else {
                    message.Error("Invalid Timeout!\n The time can\'t be less than the current time. Also the time can\'t" +
                                  " exceed the limit of 10 years.");
                }
            }
            else {
                message.Error("Please select a date!");
            }
        }

        private void OnCancelShutdownButtonClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteDosCmd("shutdown /a");
            message.Success("Previously scheduled shutdown has been cancelled");
        }
        #endregion

        #region Task -> Special Folder
        private void OnButtonBrowseSpecialFolderParentClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDlg = new WPFFolderBrowserDialog();
            folderBrowserDlg.Title = "Select a Parent Folder";
            if (folderBrowserDlg.ShowDialog() == true) {
                string parentPath = folderBrowserDlg.FileName;
                string createCmd = String.Format("md \"\\\\.\\{0}\\{1}\"", parentPath, cmboBxSpecialFolderNames.SelectionBoxItem);
                ProcessWrapper.ExecuteDosCmd(createCmd);
               message.Success(cmboBxSpecialFolderNames.SelectionBoxItem + " created successfully at " + parentPath);
            }
        }

        private void OnButtonDeleteSpecialFolderClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == true) {
                string path = folderBrowserDialog.FileName;
                string createCmd = String.Format("rd \"\\\\.\\{0}\"", path);
                ProcessWrapper.ExecuteDosCmd(createCmd);
            }
        }
        #endregion

        #region Places -> Open With
        private void OnButtonOpenWithDialogClick(object sender, RoutedEventArgs e) {
            OpenWith openWithDialog = new OpenWith();
            openWithDialog.ShowDialog();
        }

        private void OnAddToOpenWithImageMouseDown(object sender, MouseButtonEventArgs e) {
            ShowAddToOpenWithPopup();
        }

        private void OnAddToOpenWithImageTouchEnd(object sender, TouchEventArgs e) {
            ShowAddToOpenWithPopup();
        }

        private void ShowAddToOpenWithPopup() {
            if (!popupAddToOpenWith.IsOpen) {
                popupAddToOpenWith.IsOpen = true;
            }
        }

        private void OnBrowseFileForOpenWithBtnClick(object sender, RoutedEventArgs e) {
            popupAddToOpenWith.IsOpen = false;
            string filePath = Utils.GetUserSelectedFilePath();
            if (filePath != null) {
                txtOpenWithFilePath.Text = filePath;
            }
            popupAddToOpenWith.IsOpen = true;
        }

        private void OnAddToOpenWithBtnClick(object sender, RoutedEventArgs e) {
            string filePath = txtOpenWithFilePath.Text.Trim();
            if (filePath.Length == 0) return;
            if (File.Exists(filePath)) {
                OpenWithTask.AddStatus status = OpenWithTask.Add(filePath);
                switch (status) {
                    case OpenWithTask.AddStatus.Success:
                        message.Success("Successfully added to open-with");
                        break;
                    case OpenWithTask.AddStatus.AlreadyPresent:
                        message.Success("This file is already present in open-with");
                        break;
                    case OpenWithTask.AddStatus.Failed:
                        message.Error("Failed to add this file to open-with");
                        break;
                }
            }
            else {
                message.Error("The specified file doesn't exist.");
            }
            txtOpenWithFilePath.Text = String.Empty;
        }

        private void OnCancelAddToOpenWithBtnClick(object sender, RoutedEventArgs e) {
            popupAddToOpenWith.IsOpen = false;
        }
        #endregion

        #region Places -> Startup Manager
        private void OnLaunchStartupManagerBtnClick(object sender, RoutedEventArgs e) {
            StartupManager startupManager = new StartupManager();
            startupManager.ShowDialog();
        }

        private void ShowAddToStartupPopup() {
            if (!popupAddToStartup.IsOpen) {
                popupAddToStartup.IsOpen = true;
            }
        }

        private void OnBrowseFileForStartupBtnClick(object sender, RoutedEventArgs e) {
            popupAddToStartup.IsOpen = false;
            string filePath = Utils.GetUserSelectedFilePath();
            if (filePath != null) {
                txtStartupFilePath.Text = filePath;
            }
            popupAddToStartup.IsOpen = true;
        }

        private void OnAddToStartupBtnClick(object sender, RoutedEventArgs e) {
            string filePath = txtStartupFilePath.Text.Trim();
            bool? onlyCurrentUser = rbtnStartupForCurrentUser.IsChecked;
            if (onlyCurrentUser != true) {
                onlyCurrentUser = rbtnStartupForAllUser.IsChecked;
            }
            if (filePath.Length == 0) return;
            if (File.Exists(filePath)) {
                StartupManagerTask.AddStatus status = StartupManagerTask.Add(filePath, onlyCurrentUser);
                switch (status) {
                    case StartupManagerTask.AddStatus.Success:
                        message.Success("Successfully added to startup");
                        break;
                    case StartupManagerTask.AddStatus.AlreadyPresent:
                        message.Error("This file is already present in startup");
                        break;
                    case StartupManagerTask.AddStatus.Failed:
                        message.Error("Failed to add this file to startup");
                        break;
                }
            }
            else {
                message.Error("The specified file doesn't exist");
            }
            txtStartupFilePath.Text = String.Empty;
        }

        private void OnCancelAddToStartupBtnClick(object sender, RoutedEventArgs e) {
            popupAddToStartup.IsOpen = false;
        }

        private void OnAddToStartupImageMouseDown(object sender, MouseButtonEventArgs e) {
            ShowAddToStartupPopup();
        }

        private void OnAddToStartupImageTouchEnd(object sender, TouchEventArgs e) {
            ShowAddToStartupPopup();
        }
        #endregion

        #region Right-Click -> Send To
        private void LoadSendTo() {
            if (lstSendTo.ItemsSource != null || sendToBackgroundWorker.IsBusy) return;
            lstSendTo.Visibility = Visibility.Hidden;
            sendToBackgroundWorker.RunWorkerAsync();
        }

        private void ReloadSendTo() {
            lstSendTo.Visibility = Visibility.Hidden;
            txtSendToLoading.Visibility = Visibility.Hidden;
            sendToBackgroundWorker.RunWorkerAsync();
        }

        private void OnSendToWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            ObservableCollection<FileItem> sendToFileItems = e.Result as ObservableCollection<FileItem>;
            if (sendToFileItems == null) return;
            lstSendTo.ItemsSource = sendToFileItems;
            txtSendToLoading.Visibility = Visibility.Hidden;
            lstSendTo.Visibility = Visibility.Visible;
        }

        private void OnSendToWorkerStarted(object sender, DoWorkEventArgs e) {
            string sendToPath = SendToTask.GetFolderPath(windowsOS);
            FileReader fileReader = new FileReader(sendToPath, new List<string>() {".ini"});
            ObservableCollection<FileItem> sendToFileItems = fileReader.GetAllFiles();
            e.Result = sendToFileItems;
        }

        private void OnButtonAddFolderToSendToClick(object sender, RoutedEventArgs e) {
            try {
                if (SendToTask.AddFolder(windowsOS)) {
                    ReloadSendTo();
                }
            }
            catch (BadImageFormatException) {
                message.Error("Because of an error, add operations for operations have been disabled");
                btnAddFileToSendTo.IsEnabled = btnAddFolderToSendTo.IsEnabled = false;
            }
            catch (FileNotFoundException) {
                message.Error("File path is not valid, hence not creating any shortcut");
            }
        }

        private void OnButtonAddFileToSendToClick(object sender, RoutedEventArgs e) {
            try {
                if (SendToTask.AddFile(windowsOS)) {
                    ReloadSendTo();
                }
            } catch (BadImageFormatException) {
                message.Error("Because of an error, add operations for operations have been disabled");
                btnAddFileToSendTo.IsEnabled = btnAddFolderToSendTo.IsEnabled = false;
            } catch (FileNotFoundException) {
                message.Error("File path is not valid, hence not creating any shortcut");
            }
        }

        private void OnButtonDeleteFromSendToClick(object sender, RoutedEventArgs e) {
            SendToTask.Delete(lstSendTo, message);
            ReloadSendTo();
        }

        private void OnLinkEditSendToExplorerClick(object sender, RoutedEventArgs e) {
            string sendToPath = SendToTask.GetFolderPath(windowsOS);
            try {
                Process.Start(sendToPath);
            } catch (Win32Exception) {
                message.Error("The SendTo Path is different on your Windows");
            }
        }
        #endregion

        #region Right-Click -> Add Items
        private void OnButtonBrowseFileForContextMenu(object sender, RoutedEventArgs e) {
            string filePath = Utils.GetUserSelectedFilePath();
            if (filePath != null)
                txtShrtCtPath.Text = filePath;
        }

        private void OnButtonContextMenuShortcutClick(object sender, RoutedEventArgs e) {
            AddToContextMenu();
        }

        private void AddToContextMenu() {
            string shrtCtName = txtShrtCtName.Text.Trim();
            if (shrtCtName.Length == 0) {
                message.Error("Please enter a name for the shortcut");
                return;
            }  
            if (shrtCtName == "cmd") {
                message.Error("Can't create shortcut with name \"cmd\" since it is reserved by Windows. Please try a different name.");
            }
            string shrtCtPath = txtShrtCtPath.Text.Trim();
            if (shrtCtPath.Length == 0) {
                message.Error("Please enter a valid file-path or url");
                return;
            }
            bool result = RightClickAddDeleteTask.Add(shrtCtName, shrtCtPath);
            if (!result)
                message.Error("Please enter a valid file-path or url");
            else {
                message.Success("Successfully added " + shrtCtName + " to Right-Click");
                ClearAddToContextMenuInput();
            }
        }

        private void ClearAddToContextMenuInput() {
            txtShrtCtPath.Text = txtShrtCtName.Text = String.Empty;
        }

        private void OnButtonDeleteFromContextMenuClick(object sender, RoutedEventArgs e) {
            ShowContextMenuItems();
        }

        private void ShowContextMenuItems() {
            ObservableCollection<FileItem> rightClickListItems = RightClickAddDeleteTask.All();
            if (rightClickListItems != null && rightClickListItems.Any()) {
                lstRightClickShortcuts.ItemsSource = rightClickListItems;
                popupRightClickList.IsOpen = true;
            } else {
                popupRightClickList.IsOpen = false;
                message.Error("You haven't added anything to the Right-Click");
            }
        }

        private void OnButtonDismissContextMenuListClick(object sender, RoutedEventArgs e) {
            popupRightClickList.IsOpen = false;
        }

        private void OnImageDeleteItemFromContextMenuMouseDown(object sender, MouseButtonEventArgs e) {
            DeleteItemFromContextMenu(e.Source);
        }

        private void OnImageDeleteItemFromContextMenuTouchEnd(object sender, TouchEventArgs e) {
            DeleteItemFromContextMenu(e.Source);
        }

        private void DeleteItemFromContextMenu(object source) {
            Image image = source as Image;
            if (image == null || image.Tag == null) return;
            FileItem fileItem = image.Tag as FileItem;
            if (fileItem == null || fileItem.Tag == null) return;
            ObservableCollection<FileItem> rightClickItems =
                lstRightClickShortcuts.ItemsSource as ObservableCollection<FileItem>;
            RightClickAddDeleteTask.Delete(fileItem, rightClickItems);
            if (!rightClickItems.Any()) {
                popupRightClickList.IsOpen = false;
            }
            message.Success("Sucessfully deleted " + fileItem.Tag + " from Right-Click menu");
        }
        #endregion

        #region Features
        private void LoadFeaturesTab() {
            // System Beep
            using (RegistryKey hkcuSound = HKCU.OpenSubKey(@"Control Panel\Sound")) {
                if (hkcuSound != null) {
                    string val = (string) hkcuSound.GetValue(Constants.Beep);
                    if (val != null) {
                        switch (val) {
                            case Constants.Yes:
                                chkSystemBeep.IsChecked = false;
                                break;
                            case Constants.No:
                                chkSystemBeep.IsChecked = true;
                                break;
                            default:
                                // It has some garbage value, so fixing it
                                chkSystemBeep.IsChecked = false;
                                hkcuSound.SetValue(Constants.Beep, Constants.Yes);
                                break;
                        }
                    } else {
                        chkSystemBeep.IsChecked = false;
                    }
                } else {
                    chkSystemBeep.IsChecked = false;
                }
            }

            // Windows DVD Burner
            using (RegistryKey hkcuExplorer = HKCU.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                UIRegistryHandler.SetUICheckBoxFromRegistryValue(chkWinDvdBurner, hkcuExplorer, Constants.NoDvdBurning, true);
            }
        }

        private void OnLinkActivateAdminClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteProcess("net", "user administrator /active:yes");
            message.Success("Successfully activated the Administrator account");
        }

        private void OnLinkDeactivateAdminAccountClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteProcess("net", "user administrator /active:no");
            message.Success("Successfully de-activated the Administrator account");
        }

        private void UpdateRegistryFromFeatures() {
            // System Beeps
            if (chkSystemBeep.Tag != null && (chkSystemBeep.Tag as Byte?) == Constants.HasUserInteracted) {
                using (RegistryKey hkcuSound = HKCU.CreateSubKey(@"Control Panel\Sound")) {
                    string val = chkSystemBeep.IsChecked == true ? Constants.No : Constants.Yes;
                    hkcuSound.SetValue(Constants.Beep, val);
                }
            }

            // Windows DVD Burner
            if (chkWinDvdBurner.Tag != null && (chkWinDvdBurner.Tag as Byte?) == Constants.HasUserInteracted) {
                using (RegistryKey hkcuExplorer = HKCU.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                    if (chkWinDvdBurner.IsChecked == true) {
                        if (hkcuExplorer.GetValue(Constants.NoDvdBurning) != null) {
                            hkcuExplorer.DeleteValue(Constants.NoDvdBurning);
                        }
                    } else {
                        hkcuExplorer.SetValue(Constants.NoDvdBurning, 1);    
                    }   
                }
            }
        }
        #endregion
    }
}