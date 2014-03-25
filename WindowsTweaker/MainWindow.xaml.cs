using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WindowsTweaker.AppTasks;
using WindowsTweaker.Models;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFFolderBrowser;
using Xceed.Wpf.Toolkit;
using File = System.IO.File;
using MessageBox = System.Windows.MessageBox;

namespace WindowsTweaker {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            _message = new Message(msgContainerBorder, txtMsg);
            _sendToBackgroundWorker = (BackgroundWorker) this.FindResource("sendToBackgroundWorker");
            _selectionColor = _defaultSelectionColor;
        }

        private readonly RegistryKey _hkcu = Registry.CurrentUser;
        private readonly RegistryKey _hklm = Registry.LocalMachine;
        private readonly RegistryKey _hkcr = Registry.ClassesRoot;
        private readonly WindowsVer.Windows _windowsOs = WindowsVer.Instance.GetName();
        private readonly Color _defaultSelectionColor = Color.FromArgb(255, 0, 102, 204);
        private readonly Message _message;
        private readonly BackgroundWorker _sendToBackgroundWorker;
        private Color _selectionColor;

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
                    LoadDisplayTab();
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
                    LoadMaintenanceTab();
                    break;

                case Constants.Utilities:
                    break;
            }
        }

        private void OnCheckBoxClick(object sender, RoutedEventArgs e) {
            CheckBox chkBox = (CheckBox)sender;
            chkBox.SetInteracted();
            if (chkBox.Equals(chkEnableAutoLogin)) {
                ToggleAutoLoginUiState();
            } else if (chkBox.Equals(chkEnableLoginMsg)) {
                ToggleLoginMessageUiState();
            }
        }

        private void OnMessageViewCloseGridMouseDown(object sender, MouseButtonEventArgs e) {
            _message.Hide();
        }

        private void OnMessageViewCloseTouchDown(object sender, TouchEventArgs e) {
            _message.Hide();
        }

        private void StartProcess(object sender, RoutedEventArgs e) {
            try {
                string tagVal = ((Hyperlink) sender).Tag.ToString();
                if (tagVal.Contains(",")) {
                    string[] tagArr = tagVal.Split(',');
                    ProcessWrapper.ExecuteProcess(tagArr[0], tagArr[1]);
                } else if (tagVal.StartsWith("http")) {
                    Process.Start(tagVal);
                } else {
                    ProcessWrapper.ExecuteProcess(tagVal);
                }
            } catch (Win32Exception) {
                if (((Hyperlink)sender).Tag.ToString().Contains("SystemPropertiesProtection")) {
                    ProcessWrapper.ExecuteProcess(Environment.GetFolderPath(Environment.SpecialFolder.Windows) 
                        + @"\system32\Restore\rstrui.exe");
                } else
                    _message.Error("This option is not available in your version of Windows");
            }
        }
        #endregion

        #region Logon
        private void LoadLogonTab() {
            // Auto Login
            using (RegistryKey hklmWinLogon = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon")) {
                chkEnableAutoLogin.SetCheckedState(hklmWinLogon, Constants.AutoAdminLogon);
                if (chkEnableAutoLogin.IsChecked == true) {
                    txtAutoLoginUserName.SetText(hklmWinLogon, Constants.DefaultUserName);
                    txtAutoLoginPasswd.SetPassword(hklmWinLogon, Constants.DefaultPassword);
                    txtAutoLoginDomainName.SetText(hklmWinLogon, Constants.DefaultDomainName);
                }
                ToggleAutoLoginUiState();

                chkPreventShiftPress.SetCheckedStateFromString(hklmWinLogon, Constants.IgnoreShiftOverride);
                chkAutoLogonAfterLogoff.SetCheckedStateFromString(hklmWinLogon, Constants.ForceAutoLogon);
            }

            // Startup Sound
            using (RegistryKey hklmBootAnimation = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\BootAnimation")) {
                chkLoginSound.SetCheckedState(hklmBootAnimation, Constants.DisableStartupSound, true);
            }

            // Screensaver Lock
            using (RegistryKey hkcuPDesktop = _hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\Control Panel\Desktop")) {
                chkScreenSaverLock.SetCheckedState(hkcuPDesktop, Constants.ScreenSaverIsSecure);
            }

            using (RegistryKey hklmSystem = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                // Miscellaneous
                if (hklmSystem.GetValue(Constants.DisableCtrlAltDlt) != null) {
                    chkRequireCtrlAltDlt.SetCheckedState(hklmSystem, Constants.DisableCtrlAltDlt, true);    
                } else {
                    chkRequireCtrlAltDlt.IsChecked = false;
                }
                chkRestrictLastLoginUser.SetCheckedState(hklmSystem, Constants.NoLastUserName);
                chkShowShutdownBtn.SetCheckedState(hklmSystem, Constants.ShutdownWithoutLogon);

                // Login Message
                txtLoginMsgTitle.SetText(hklmSystem, Constants.LoginMsgTitle);
                txtLoginMsgContent.SetText(hklmSystem, Constants.LoginMsgContent);
                if (!String.IsNullOrEmpty(txtLoginMsgTitle.Text) || !String.IsNullOrEmpty(txtLoginMsgContent.Text)) {
                    chkEnableLoginMsg.IsChecked = true;
                } else {
                    chkEnableLoginMsg.IsChecked = false;
                    txtLoginMsgTitle.Text = txtLoginMsgContent.Text = String.Empty;
                    txtLoginMsgContent.IsEnabled = txtLoginMsgTitle.IsEnabled = false;
                }
            }
        }

        private void UpdateRegistryFromLogon() {
            // Auto Login
            using (RegistryKey hklmWinLogon = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon")) {
                hklmWinLogon.SetValue(chkEnableAutoLogin, Constants.AutoAdminLogon);
                hklmWinLogon.SetValue(txtAutoLoginUserName, Constants.DefaultUserName);
                hklmWinLogon.SetValue(txtAutoLoginPasswd, Constants.DefaultPassword);
                hklmWinLogon.SetValue(txtAutoLoginDomainName, Constants.DefaultDomainName);
                hklmWinLogon.SetValue(chkPreventShiftPress, Constants.IgnoreShiftOverride);
                hklmWinLogon.SetValue(chkAutoLogonAfterLogoff, Constants.ForceAutoLogon);
            }

            // Startup Sound
            using (RegistryKey hklmBootAnimation = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\BootAnimation")) {
                hklmBootAnimation.SetValue(chkLoginSound, Constants.DisableStartupSound, true);
            }

            // Screensaver Lock
            using (RegistryKey hkcuPDesktop = _hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\Control Panel\Desktop")) {
                hkcuPDesktop.SetValue(chkScreenSaverLock, Constants.ScreenSaverIsSecure);
            }

            using (RegistryKey hklmSystem = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                // Miscellaneous
                hklmSystem.SetValue(chkRequireCtrlAltDlt, Constants.DisableCtrlAltDlt, true);
                hklmSystem.SetValue(chkRestrictLastLoginUser, Constants.NoLastUserName);
                hklmSystem.SetValue(chkShowShutdownBtn, Constants.ShutdownWithoutLogon);

                // Login Message
                hklmSystem.SetValue(txtLoginMsgTitle, Constants.LoginMsgTitle);
                hklmSystem.SetValue(txtLoginMsgContent, Constants.LoginMsgContent);
            }
        }

        private void ToggleLoginMessageUiState() {
            if (chkEnableLoginMsg.IsChecked == true) {
                txtLoginMsgTitle.IsEnabled = txtLoginMsgContent.IsEnabled = true;
            }
            else {
                txtLoginMsgContent.Text = txtLoginMsgTitle.Text = String.Empty;
                txtLoginMsgContent.IsEnabled = txtLoginMsgTitle.IsEnabled = false;
            }
        }

        private void ToggleAutoLoginUiState() {
            if (chkEnableAutoLogin.IsChecked == true) {
                borderAutoLogin.IsEnabled = true;
                txtAutoLoginDomainName.Text = Environment.UserDomainName;
                txtAutoLoginUserName.Text = Environment.UserName;
                txtAutoLoginPasswd.Password = String.Empty;
            } else {
                borderAutoLogin.IsEnabled = false;
                txtAutoLoginDomainName.Text = txtAutoLoginUserName.Text = txtAutoLoginPasswd.Password = String.Empty;
            }
        }

        #endregion

        #region Restrictions
        private void LoadRestrictionsTab()
        {


            using (RegistryKey hkcuExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Explorer
                chkHideFileMenu.SetCheckedState(hkcuExplorer, Constants.NoFileMenu);
                chkHideFolderOpt.SetCheckedState(hkcuExplorer, Constants.NoFolderOption);
                chkRightClick.SetCheckedState(hkcuExplorer, Constants.NoViewContextMenu);
                chkTasbarAndStartMenuRightClick.SetCheckedState(hkcuExplorer, Constants.NoTrayContextMenu);
                //START MENU          
                chkHideShutDownOpt.SetCheckedState(hkcuExplorer, Constants.NoClose);
                chkHideRecentDocs.SetCheckedState(hkcuExplorer, Constants.NoRecentDocsMenu);
                chkHideChangesStartMenu.SetCheckedState(hkcuExplorer, Constants.NoChangeStartMenu);
                chkHideLogOff.SetCheckedState(hkcuExplorer, Constants.NoLogOff);
                //System            
                chkDisableDeletionPrinters.SetCheckedState(hkcuExplorer, Constants.NoDeletePrinter);
                chkDisableAddNewPrinter.SetCheckedState(hkcuExplorer, Constants.NoAddPrinter);
                chkDisableWindUpdate.SetCheckedState(hkcuExplorer, Constants.NoWindowUpdate);
                chkDisableWindowRegEditTool.SetCheckedState(hkcuExplorer, Constants.DisbaleRegistryTools);
                chkDisableEditSettUnderMyComp.SetCheckedState(hkcuExplorer, Constants.NoPropertiesMyComputer);
            }
            using (RegistryKey hklmSystem = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                // Explorer
                chkHideAppearanceOpt.SetCheckedState(hklmSystem, Constants.NoDispAppearancePage);
                chkHideChangeScreensaver.SetCheckedState(hklmSystem, Constants.NoDispScrSavPage);
                //START MENU      
                chkHideControlPanel.SetCheckedState(hklmSystem, Constants.NoDispCpl);
                //System        
                chkDisableChangeToVirtualMem.SetCheckedState(hklmSystem, Constants.NoVirtMemPage);
            }
            using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Explorer
                chkHideThumbNailCache.SetCheckedState(hkcuExAdvanced, Constants.DisableThumbnailCache);
                // Taskbar
                chkTaskBarAnim.SetCheckedState(hkcuExAdvanced, Constants.TaskBarAnimations);
                if (_windowsOs != WindowsVer.Windows.Xp) {
                    hkcuExAdvanced.SetValue(chkShowIconsTaskBar, Constants.TaskBarSmallIcons);
                }
                chkTaskBarNoTooltip.SetCheckedState(hkcuExAdvanced, Constants.ShowInfoTip, true);

            }
            if (_windowsOs > WindowsVer.Windows.Xp && _windowsOs < WindowsVer.Windows.Eight) {
                using (RegistryKey hkcuDWM = _hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\DWM")) {
                    // Explorer
                    chkHide3DFlip.SetCheckedState(hkcuDWM, Constants.DisAllowFlip_3D);
                }
            }
            else {
                chkHide3DFlip.Visibility = Visibility.Collapsed;
            }
            using (RegistryKey hkcuSystem = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                //System
                chkDisableTaskManager.SetCheckedState(hkcuSystem, Constants.DisableTaskMgr);
            }
            using (RegistryKey hklmParams = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\LanmanServer\Parameters")) {
                //System
                chkDisableAdminShares.SetCheckedState(hklmParams, Constants.AutoShreWks, true);
            }
        }

        private void UpdateRegistryRestrictions() {

            using (RegistryKey hkcuExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Explorer
                hkcuExplorer.SetValue(chkHideFileMenu, Constants.NoFileMenu);
                hkcuExplorer.SetValue(chkHideFolderOpt, Constants.NoFolderOption);
                hkcuExplorer.SetValue(chkRightClick, Constants.NoViewContextMenu);
                hkcuExplorer.SetValue(chkTasbarAndStartMenuRightClick, Constants.NoTrayContextMenu);
                //START MENU          
                hkcuExplorer.SetValue(chkHideShutDownOpt, Constants.NoClose);
                hkcuExplorer.SetValue(chkHideRecentDocs, Constants.NoRecentDocsMenu);
                hkcuExplorer.SetValue(chkHideChangesStartMenu, Constants.NoChangeStartMenu);
                hkcuExplorer.SetValue(chkHideLogOff, Constants.NoLogOff);
                //System            
                hkcuExplorer.SetValue(chkDisableDeletionPrinters, Constants.NoDeletePrinter);
                hkcuExplorer.SetValue(chkDisableAddNewPrinter, Constants.NoAddPrinter);
                hkcuExplorer.SetValue(chkDisableWindUpdate, Constants.NoWindowUpdate);
                hkcuExplorer.SetValue(chkDisableWindowRegEditTool, Constants.DisbaleRegistryTools);
                hkcuExplorer.SetValue(chkDisableEditSettUnderMyComp, Constants.NoPropertiesMyComputer);
            }
            using (RegistryKey hklmSystem = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                // Explorer
                hklmSystem.SetValue(chkHideAppearanceOpt, Constants.NoDispAppearancePage);
                hklmSystem.SetValue(chkHideChangeScreensaver, Constants.NoDispScrSavPage);
                //START MENU      
                hklmSystem.SetValue(chkHideControlPanel, Constants.NoDispCpl);
                //System        
                hklmSystem.SetValue(chkDisableChangeToVirtualMem, Constants.NoVirtMemPage);
            }
            using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                hkcuExAdvanced.SetValue(chkHideThumbNailCache, Constants.DisableThumbnailCache);
                hkcuExAdvanced.SetValue(chkTaskBarAnim, Constants.TaskBarAnimations);
                if (_windowsOs != WindowsVer.Windows.Xp) {
                    hkcuExAdvanced.SetValue(chkShowIconsTaskBar, Constants.TaskBarSmallIcons);
                }

            }
            if (_windowsOs > WindowsVer.Windows.Xp && _windowsOs < WindowsVer.Windows.Eight) {
                using (RegistryKey hkcuDWM = _hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\DWM")) {
                    hkcuDWM.SetValue(chkHide3DFlip, Constants.DisAllowFlip_3D);
                }
            }
            using (RegistryKey hkcuSystem = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                //System
                hkcuSystem.SetValue(chkDisableTaskManager, Constants.DisableTaskMgr);
            }
            using (RegistryKey hklmParams = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\LanmanServer\Parameters")) {
                hklmParams.SetValue(chkDisableAdminShares, Constants.AutoShreWks, true);
            }
        }
        #endregion

        #region Explorer
        private void LoadExplorerTab() {
            //Drive Letters
            using (RegistryKey hkcuCvExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
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

            using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Advanced
                chkShowMenuBar.SetCheckedState(hkcuExAdvanced, Constants.AlwaysShowMenu);
                chkHideExtension.SetCheckedState(hkcuExAdvanced, Constants.HideFileExt);
                chkSelectItems.SetCheckedState(hkcuExAdvanced, Constants.AutoChkSelect);
                chkAutoExpand.SetCheckedState(hkcuExAdvanced, Constants.NavPaneExpandToCurrentFolder);

                int? val = (int?) hkcuExAdvanced.GetValue(Constants.Hidden);
                chkShowHiddenFilesFolders.IsChecked = val == 1;

                chkRestoreFoldersAtLogin.SetCheckedState(hkcuExAdvanced, Constants.PersistBrowsers);
                chkShowOSfiles.SetCheckedState(hkcuExAdvanced, Constants.SuperHidden);
                chkShowNtfsInColor.SetCheckedState(hkcuExAdvanced, Constants.CompressedColor);

                // Properties
                chkHideSecurity.SetCheckedState(hkcuExAdvanced, Constants.NoSecurityTab);
                chkShowHideCustomize.SetCheckedState(hkcuExAdvanced, Constants.NoCustomizeTab);
            }

            using (RegistryKey hklmNamespace = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {
                // Libraries
                if (_windowsOs > WindowsVer.Windows.Xp) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.Library);
                    if (key != null) {
                        rbtnShowLibraries.IsChecked = true;
                    }
                    else {
                        rbtnHideLibraries.IsChecked = true;
                    }
                }
                else {
                    tabLibrary.Visibility = Visibility.Collapsed;
                }
            }

            // Etc
            using (RegistryKey hklmNamespace = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace")) {
                chkAddRecycleBinToMyComputer.SetCheckedStateFromSubKey(hklmNamespace, Constants.RecycleBin);
            }

            RegistryKey hkcrCLSID = _hkcr.OpenSubKey(@"CLSID");
            RegistryKey hklmCPNamespace = _hklm.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
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

            if (_windowsOs >= WindowsVer.Windows.Eight) {
                // Start Screen
                using (RegistryKey hkcuAccess = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent")) {
                    int? desktopBkgOnStartVal = (int?) hkcuAccess.GetValue(Constants.DesktopBkgOnStart);
                    switch (desktopBkgOnStartVal) {
                        case 0xdb:
                            chkDesktopBkgOnStart.IsChecked = true;
                            break;
                        case 0xdd:
                            chkDesktopBkgOnStart.IsChecked = false;
                            break;
                        default:
                            chkDesktopBkgOnStart.IsChecked = false;
                            break;
                    }
                }
                using (RegistryKey hkcuStartScreen = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\StartPage")) {
                    chkNavigateToDesktopOnClose.SetCheckedState(hkcuStartScreen, Constants.OpenDesktop, true);
                    chkStartOnDisplay.SetCheckedState(hkcuStartScreen, Constants.MonitorOverride);
                    chkAppsViewOnStart.SetCheckedState(hkcuStartScreen, Constants.MakeAllAppsDefault);
                    chkSearchEverywhere.SetCheckedState(hkcuStartScreen, Constants.GlobalSearchInApps);
                    chkListDesktopApps.SetCheckedState(hkcuStartScreen, Constants.DesktopFirst);
                }

                // Navigation
                using (RegistryKey hkcuEdgeUi = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\ImmersiveShell\EdgeUi")) {
                    chkCharmsTR.SetCheckedState(hkcuEdgeUi, Constants.DisableTrCorner, true);
                    chkAppsTL.SetCheckedState(hkcuEdgeUi, Constants.DisableTlCorner, true);
                }

                using (RegistryKey hkcuAdvanced = _hkcr.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                    chkReplaceCmdPromptWithPs.SetCheckedState(hkcuAdvanced, Constants.DontUsePowerShellOnWinX, true);
                }
            }
        }

        private void UpdateRegistryFromExplorer() {
            //Drive Letters
            using (RegistryKey hkcuCvExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
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

            using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                // Advanced
                hkcuExAdvanced.SetValue(chkShowMenuBar, Constants.AlwaysShowMenu);
                hkcuExAdvanced.SetValue(chkHideExtension, Constants.HideFileExt);
                hkcuExAdvanced.SetValue(chkSelectItems, Constants.AutoChkSelect);

                int val = chkShowHiddenFilesFolders.IsChecked == true ? 1 : 2;
                hkcuExAdvanced.SetValue(Constants.Hidden, val);

                hkcuExAdvanced.SetValue(chkRestoreFoldersAtLogin, Constants.PersistBrowsers);
                hkcuExAdvanced.SetValue(chkShowOSfiles, Constants.SuperHidden);
                hkcuExAdvanced.SetValue(chkShowNtfsInColor, Constants.CompressedColor);

                // Properties
                hkcuExAdvanced.SetValue(chkHideSecurity, Constants.NoSecurityTab);
                hkcuExAdvanced.SetValue(chkShowHideCustomize, Constants.NoCustomizeTab);
            }

            using (RegistryKey hklmNamespace = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {
                // Libraries
                if (_windowsOs > WindowsVer.Windows.Xp) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.Library, true);
                    if (rbtnShowLibraries.IsChecked == true)
                        hklmNamespace.SetSubKey(Constants.Library, true);
                    else if (rbtnHideLibraries.IsChecked == true)
                        hklmNamespace.SetSubKey(Constants.Library, false);
                }
            }

            // Etc
            using (RegistryKey hklmNamespace = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace")) {
                hklmNamespace.SetSubKey(chkAddRecycleBinToMyComputer, Constants.RecycleBin);
            }

            RegistryKey hkcrCLSID = _hkcr.OpenSubKey(@"CLSID");
            RegistryKey hklmCPNamespace =
                _hklm.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
            if (chkAddRegeditToControlPanel.IsChecked == true) {
                if (hkcrCLSID == null)
                    hkcrCLSID = _hkcr.CreateSubKey(@"CLSID");
                if (hklmCPNamespace == null)
                    hklmCPNamespace =
                        _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
                hkcrCLSID.SetSubKey(Constants.Regedit, true);
                hklmCPNamespace.SetSubKey(Constants.Regedit, true);
                hkcrCLSID.Close();
                hklmCPNamespace.Close();
            }
            else {
                if (hkcrCLSID != null) {
                    hkcrCLSID.SetSubKey(Constants.Regedit, false);
                    hkcrCLSID.Close();
                }
                if (hklmCPNamespace != null) {
                    hklmCPNamespace.SetSubKey(Constants.Regedit, false);
                    hklmCPNamespace.Close();
                }
            }

            if (_windowsOs >= WindowsVer.Windows.Eight) {
                // Start Screen
                if (chkDesktopBkgOnStart.HasUserInteracted()) {
                    using (RegistryKey hkcuAccess = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Accent")) {
                        int desktopBkgOnStartVal = chkDesktopBkgOnStart.IsChecked == true ? 0xdb : 0xdd;
                        hkcuAccess.SetValue(Constants.DesktopBkgOnStart, desktopBkgOnStartVal);
                    }
                }
                using (RegistryKey hkcuStartScreen = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\StartPage")) {
                    hkcuStartScreen.SetValue(chkNavigateToDesktopOnClose, Constants.OpenDesktop, true);
                    hkcuStartScreen.SetValue(chkStartOnDisplay, Constants.MonitorOverride);
                    hkcuStartScreen.SetValue(chkAppsViewOnStart, Constants.MakeAllAppsDefault);
                    hkcuStartScreen.SetValue(chkSearchEverywhere, Constants.GlobalSearchInApps);
                    hkcuStartScreen.SetValue(chkListDesktopApps, Constants.DesktopFirst);
                }

                // Navigation
                using (RegistryKey hkcuEdgeUi = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\ImmersiveShell\EdgeUi")) {
                    hkcuEdgeUi.SetValue(chkCharmsTR, Constants.DisableTrCorner, true);
                    hkcuEdgeUi.SetValue(chkAppsTL, Constants.DisableTlCorner, true);
                }

                using (RegistryKey hkcuAdvanced = _hkcr.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                    hkcuAdvanced.SetValue(chkReplaceCmdPromptWithPs, Constants.DontUsePowerShellOnWinX, true);
                }
            }
        }
        #endregion

        #region System
        private void LoadSystemTab() {
            using (RegistryKey hkcuDesktop = _hkcu.CreateSubKey(@"Control Panel\Desktop")) {
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

            using (RegistryKey hklmWinInstaller = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer")) {
                // Windows Installer
                chkDisableWinInstaller.SetCheckedState(hklmWinInstaller, Constants.DisableMsi);
                chkElevatedInstall.SetCheckedState(hklmWinInstaller, Constants.AlwaysInstallElevated);
                chkDisableSysRestoreInstall.SetCheckedState(hklmWinInstaller, Constants.LimitSystemRestore);
            }

            using (RegistryKey hklmCVWinNT = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")) {
                // Registration
                txtOwnerName.SetText(hklmCVWinNT, Constants.RegisteredOwner);
                txtCompanyName.SetText(hklmCVWinNT, Constants.RegisteredOrg);
                txtProductId.SetText(hklmCVWinNT, Constants.ProductId);
            }

            using (RegistryKey hklmOEM = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                // Edit Information
                txtManufacturer.SetText(hklmOEM, Constants.Manufacturer);
                txtModel.SetText(hklmOEM, Constants.Model);
                txtSupportPhone.SetText(hklmOEM, Constants.SupportPhone);
                txtSupportUrl.SetText(hklmOEM, Constants.SupportUrl);

                // Logo
                string logoUrl = (string) hklmOEM.GetValue(Constants.Logo);
                if (!String.IsNullOrEmpty(logoUrl)) {
                    btnDeleteLogo.IsEnabled = true;
                    try {
                        imgProperty.Source = new BitmapImage(new Uri(logoUrl, UriKind.Absolute));
                    }
                    catch (UriFormatException) {
                        Uri uriLogo;
                        if (Uri.TryCreate("C:" + logoUrl, UriKind.Absolute, out uriLogo)) {
                            imgProperty.Source = new BitmapImage(uriLogo);
                        }
                    }
                }
            }
        }


        private void OnButtonSelectLogoClick(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Filter = "Bitmap Images|*.bmp",
                Multiselect = false
            };
            if (openFileDialog.ShowDialog() == true) {
                using (RegistryKey hklmOEM = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                    hklmOEM.SetValue(Constants.Logo, openFileDialog.FileName);
                    imgProperty.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                    btnDeleteLogo.IsEnabled = true;
                    _message.Success("Image has been successfully applied. If your Computer\'s property windows is already open then" +
                                    " please close & re-open the window for the changes to be reflected.");
                }
            }
        }

        private void OnButtonDeleteLogoClick(object sender, RoutedEventArgs e) {
            using (RegistryKey hklmOEM = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                string logoUrl = (string) hklmOEM.GetValue(Constants.Logo);
                if (!String.IsNullOrEmpty(logoUrl)) {
                    hklmOEM.SetValue(Constants.Logo, "");
                    btnDeleteLogo.IsEnabled = false;
                    imgProperty.Source = null;
                    _message.Success("Image has been successfully removed");
                }
            }
        }

        private void UpdateRegistryFromSystem() {
            //Shutdown Configuration
            if (rbtnShutdownImmediately.Visibility == Visibility.Visible) {
                using (RegistryKey hkcuDesktop = _hkcu.CreateSubKey(@"Control Panel\Desktop")) {
                    string val = Utils.BoolToString(rbtnShutdownImmediately.IsChecked);
                    hkcuDesktop.SetValue(Constants.AutoEndTasks, val);
                    if (rbtnShutdownAfterWaiting.IsChecked == true) {
                        // TODO : set timeout
                    }
                }
            }

            using (RegistryKey hklmWinInstaller = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer")) {
                //RegistryKey hklmPWindows = HKLM.CreateSubKey(@"Software\Policies\Microsoft\Windows");

                // Windows Installer
                if (chkDisableWinInstaller.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.DisableMsi, 2);
                } else {
                    hklmWinInstaller.DeleteValue(Constants.DisableMsi, false);
                }

                if (chkElevatedInstall.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.AlwaysInstallElevated, 1);
                } else {
                    hklmWinInstaller.DeleteValue(Constants.AlwaysInstallElevated, false);
                }

                if (chkDisableSysRestoreInstall.IsChecked == true) {
                    hklmWinInstaller.SetValue(Constants.LimitSystemRestore, 1);
                } else {
                    hklmWinInstaller.DeleteValue(Constants.LimitSystemRestore, false);
                }
            }

            using (RegistryKey hklmCVWinNT = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion")) {
                // Registration
                hklmCVWinNT.SetValue(txtOwnerName, Constants.RegisteredOwner);
                hklmCVWinNT.SetValue(txtCompanyName, Constants.RegisteredOrg);
                hklmCVWinNT.SetValue(txtProductId, Constants.ProductId);
            }

            using (RegistryKey hklmOEM = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                // Edit Information
                hklmOEM.SetValue(txtManufacturer, Constants.Manufacturer);
                hklmOEM.SetValue(txtModel, Constants.Model);
                hklmOEM.SetValue(txtSupportPhone, Constants.SupportPhone);
                hklmOEM.SetValue(txtSupportUrl, Constants.SupportUrl);
            }
        }
        #endregion

        #region Display
        private void LoadDisplayTab() {
            // Display Settings
            using (RegistryKey hkcuWinMet = _hkcu.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                chkWindowAnim.SetCheckedStateFromString(hkcuWinMet, Constants.MinAnimate);
                iudWinBorderWidth.SetValueFromString(hkcuWinMet, Constants.BorderWidth, Constants.DefaultWinBorder,
                    Constants.MaxBorderVal, Constants.MinBorderVal);
                iudIconHorizSpace.SetValueFromString(hkcuWinMet, Constants.IconSpacing, Constants.DefaultIconSpacing,
                    Constants.MaxIconSpacingVal, Constants.MinIconSpacingVal);
                iudIconVerSpace.SetValueFromString(hkcuWinMet, Constants.IconVerticalSpacing, Constants.DefaultIconSpacing,
                    Constants.MaxIconSpacingVal, Constants.MinIconSpacingVal);
            }

            using (RegistryKey hkcuDesktop = _hkcu.CreateSubKey(@"Control Panel\Desktop")) {
                chkShowWindowDrag.SetCheckedStateFromString(hkcuDesktop, Constants.DragFullWin);
                chkWindowVersion.SetCheckedState(hkcuDesktop, Constants.PaintDesktopVer);

                // Alt-Tab
                iudAltTabRow.SetValueFromString(hkcuDesktop, Constants.SwitchCols, "7", 15, 1);
                iudNumAltTabRow.SetValueFromString(hkcuDesktop, Constants.SwitchRows, "3", 10, 2);
            }

            // Selection Color
            using (RegistryKey hkcuColors = _hkcu.CreateSubKey(@"Control Panel\Colors")) {
                string val = (string) hkcuColors.GetValue(Constants.SelectionColor);
                string[] rgb = val.Split(' ');
                _selectionColor = rgb.Length == 3 ? Color.FromRgb(Byte.Parse(rgb[0]), Byte.Parse(rgb[1]), Byte.Parse(rgb[2])) : _defaultSelectionColor;
                rectSelectionColor.Fill = new SolidColorBrush(_selectionColor);
            }

            // Explorer
            using (RegistryKey hkcrSharingHandler = _hkcr.CreateSubKey(@"Network\SharingHandler")) {
                string sharingHandlerVal = (string)hkcrSharingHandler.GetValue("");
                chkHandIcon.IsChecked = !String.IsNullOrEmpty(sharingHandlerVal) &&
                                        sharingHandlerVal == Constants.SharedFolderIcon;
            }

            using (RegistryKey hklmExplorer = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                chkOldStyleFileSort.SetCheckedState(hklmExplorer, Constants.OldStyleFileSort);
            }

            // Icon Title
            using (RegistryKey hkcuWinMet = _hkcu.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                string iconTitleWrapVal = (string) hkcuWinMet.GetValue(Constants.IconTitleWrap, "1");
                switch (iconTitleWrapVal) {
                    case "0":
                        rbtnWrapText.IsChecked = true;
                        rbtnTruncateText.IsChecked = false;
                        break;
                    default:
                        rbtnTruncateText.IsChecked = true;
                        rbtnWrapText.IsChecked = false;
                        break;
                }
            }
        }

        private void UpdateRegistryFromDisplay() {
            // Display Settings
            using (RegistryKey hkcuWinMet = _hkcu.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                hkcuWinMet.SetStringValue(chkWindowAnim, Constants.MinAnimate);
                hkcuWinMet.SetValue(Constants.BorderWidth, iudWinBorderWidth.Value.ToString());
                hkcuWinMet.SetValue(Constants.IconSpacing, iudIconHorizSpace.Value.ToString());
                hkcuWinMet.SetValue(Constants.IconVerticalSpacing, iudIconVerSpace.Value.ToString());
            }

            using (RegistryKey hkcuDesktop = _hkcu.CreateSubKey(@"Control Panel\Desktop")) {
                hkcuDesktop.SetStringValue(chkShowWindowDrag, Constants.DragFullWin);
                hkcuDesktop.SetValue(chkWindowVersion, Constants.PaintDesktopVer);

                // Alt-Tab
                hkcuDesktop.SetValue(Constants.SwitchCols, iudAltTabRow.Value.ToString());
                hkcuDesktop.SetValue(Constants.SwitchRows, iudNumAltTabRow.Value.ToString());
            }

            // Selection Color
            using (RegistryKey hkcuColors = _hkcu.CreateSubKey(@"Control Panel\Colors")) {
                Color selectionColor = ((SolidColorBrush) rectSelectionColor.Fill).Color;

                string val = String.Format("{0} {1} {2}", selectionColor.R, selectionColor.G, selectionColor.B);
                hkcuColors.SetValue(Constants.SelectionColor, val);
            }

            // Explorer
            if (chkHandIcon.HasUserInteracted()) {
                using (RegistryKey hkcrSharingHandler = _hkcr.CreateSubKey(@"Network\SharingHandler")) {
                    hkcrSharingHandler.SetValue("", chkHandIcon.IsChecked == true ? Constants.SharedFolderIcon : "");
                }
            }

            using (RegistryKey hklmExplorer = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                chkOldStyleFileSort.SetCheckedState(hklmExplorer, Constants.OldStyleFileSort);
            }
        }
        #endregion

        #region Display -> Selection Color
        private void OnSelectionRectangleMouseDown(object sender, MouseButtonEventArgs e) {
            ShowColorDialog();
        }

        private void OnSelectionRectangleTouchEnd(object sender, TouchEventArgs e) {
            ShowColorDialog();
        }

        private void OnButtonSelectionColorClick(object sender, RoutedEventArgs e) {
            ShowColorDialog();
        }

        private void OnButtonUseDefaultSelectionColorClick(object sender, RoutedEventArgs e) {
            rectSelectionColor.Fill = new SolidColorBrush(_selectionColor);
        }

        private void ShowColorDialog() {
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog(_selectionColor);
            if (colorPickerDialog.ShowDialog() == true) {
                _selectionColor = colorPickerDialog.SelectedColour;
                rectSelectionColor.Fill = new SolidColorBrush(_selectionColor);
            }
        }
        #endregion

        #region Places
        private void LoadPlacesTab() {
            // Power Button
            using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
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

            // Default Opening
            using (RegistryKey hkcrNoEnd = _hkcr.CreateSubKey(@".")) {
                string prgForNoExt = String.Empty;
                using (RegistryKey hkcrOpen = hkcrNoEnd.OpenSubKey(@"shell\open\command")) {
                    if (hkcrOpen != null) {
                        prgForNoExt = (string)hkcrOpen.GetValue("");
                    }
                    if (!String.IsNullOrEmpty(prgForNoExt)) {
                        txtPrgForNoExt.Text = prgForNoExt;
                        rbtnPrgForNoExt.IsChecked = true;
                        rbtnOpenDlgForNoExt.IsChecked = false;
                    } else {
                        rbtnPrgForNoExt.IsChecked = false;
                        rbtnOpenDlgForNoExt.IsChecked = false;
                    }
                }
            }
            using (RegistryKey hkcrUnknown = _hkcr.CreateSubKey(@"Unknown")) {
                string prgForUnkownExt = String.Empty;
                using (RegistryKey hkcrOpen = hkcrUnknown.OpenSubKey(@"shell\open\command")) {
                    if (hkcrOpen != null) {
                        prgForUnkownExt = (string) hkcrOpen.GetValue("");
                    }
                    if (!String.IsNullOrEmpty(prgForUnkownExt)) {
                        txtPrgForUnknownExt.Text = prgForUnkownExt;
                        rbtnPrgForUnknownExt.IsChecked = true;
                        rbtnOpenDlgForUnknownExt.IsChecked = false;
                    } else {
                        rbtnPrgForUnknownExt.IsChecked = false;
                        rbtnOpenDlgForUnknownExt.IsChecked = false;
                    }
                }
            }
        }

        private void UpdateFromPlaces() {
            int val = cmboBxPowerBtnAction.SelectedIndex;
            using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                switch (val) {
                    case 0 : hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 2);
                        break;
                    case 1: hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 64);
                        break;
                    case 2: hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 16);
                        break;
                    case 3: hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 4);
                        break;
                    case 4: hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 512);
                        break;
                    case 5: hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 1);
                        break;
                    case 6: hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 256);
                        break;
                }
            }
        }
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
                            _message.Success("Successfully created folder in " + parentDir +
                                            ". Please note that if on clicking the folder you get an error, " +
                                            "then you need to refresh that window for changes to be reflected.");
                        } catch (UnauthorizedAccessException ex) {
                            _message.Error("Permission Denied!");
                        }
                    } else {
                        _message.Error("You can't make " + selectedFolderName + " a \'God\' folder. " +
                                      "Please select an empty folder or create a new one");
                    }
                } else {
                    _message.Error(selectedFolderName + " is not an empty folder. You must create an " +
                                  "empty folder, to set God Mode");
                }
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
                        _message.Success("Successfully added to open-with");
                        break;
                    case OpenWithTask.AddStatus.AlreadyPresent:
                        _message.Success("This file is already present in open-with");
                        break;
                    case OpenWithTask.AddStatus.Failed:
                        _message.Error("Failed to add this file to open-with");
                        break;
                }
            } else {
                _message.Error("The specified file doesn't exist.");
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
                        _message.Success("Successfully added to startup");
                        break;
                    case StartupManagerTask.AddStatus.AlreadyPresent:
                        _message.Error("This file is already present in startup");
                        break;
                    case StartupManagerTask.AddStatus.Failed:
                        _message.Error("Failed to add this file to startup");
                        break;
                }
            } else {
                _message.Error("The specified file doesn't exist");
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

        #region Places -> Default Opening
        private void OnButtonBrowseProgramForNoExtensionClick(object sender, RoutedEventArgs e) {
            SetFileValueInTextBox(txtPrgForNoExt);
        }

        private void OnButtonBrowseProgramForUnkownExtensionClick(object sender, RoutedEventArgs e) {
            SetFileValueInTextBox(txtPrgForUnknownExt);
        }

        private static void SetFileValueInTextBox(TextBox txtBlk) {
            string filePath = Utils.GetUserSelectedFilePath();
            if (filePath != null) {
                txtBlk.Text = filePath.Contains("%1") ? filePath : filePath + " %1";
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
            using (RegistryKey hkcrContextMenuHandlers = _hkcr.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers")) {
                ValidateAndFixKeys(hkcrContextMenuHandlers);
                chkCopyToFolder.SetCheckedStateFromSubKey(hkcrContextMenuHandlers, Constants.CopyTo);
                chkMoveToFolder.SetCheckedStateFromSubKey(hkcrContextMenuHandlers, Constants.MoveTo);
                chkSendTo.SetCheckedStateFromSubKey(hkcrContextMenuHandlers, Constants.SendTo);
            }

            using (RegistryKey hkcrFileShell = _hkcr.CreateSubKey(@"*\shell")) {
                chkOpenWithNotepad.SetCheckedStateFromSubKey(hkcrFileShell, Constants.OpenNotepad);
            }

            using (RegistryKey hkcrShell = _hkcr.CreateSubKey(@"Directory\Background\shell")) {
                chkControlPanelInDesktopMenu.SetCheckedStateFromSubKey(hkcrShell, Constants.ControlPanel);
            }

            bool openCmdDirVal, openCmdDriveVal;
            using (RegistryKey hkcrDirShell = _hkcr.CreateSubKey(@"Directory\shell")) {
                openCmdDirVal = hkcrDirShell.HasValueInShellCommand(Constants.OpenCmdPromptVal);
            }

            using (RegistryKey hkcrDriveShell = _hkcr.CreateSubKey(@"Drive\shell")) {
                openCmdDriveVal = hkcrDriveShell.HasValueInShellCommand(Constants.OpenCmdPromptVal);
                chkAddDefragInMenu.SetCheckedStateFromSubKey(hkcrDriveShell, Constants.RunAs);
            }
            chkOpenCmdPrompt.IsChecked = openCmdDirVal || openCmdDriveVal;

            using (
                RegistryKey hklmExAdvanced =
                    _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                chkEncryptAndDecrypt.SetCheckedState(hklmExAdvanced, Constants.EncryptCtxMenu);
            }

            using (RegistryKey hklmClasses = _hklm.CreateSubKey(@"Software\Classes")) {
                using (RegistryKey hklmDotTxt = _hklm.CreateSubKey(@"Software\Classes\.txt")) {
                    string txtFile = (string) hklmDotTxt.GetValue("");
                    RegistryKey hklmTxt = hklmClasses.CreateSubKey(txtFile);
                    RegistryKey hklmDotTextShell = hklmTxt.OpenSubKey(Constants.Shell, true);

                    if (hklmDotTextShell != null) {
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

        #region Right-Click -> Send To
        private void LoadSendTo() {
            if (lstSendTo.ItemsSource != null || _sendToBackgroundWorker.IsBusy) return;
            lstSendTo.Visibility = Visibility.Hidden;
            _sendToBackgroundWorker.RunWorkerAsync();
        }

        private void ReloadSendTo() {
            lstSendTo.Visibility = Visibility.Hidden;
            txtSendToLoading.Visibility = Visibility.Hidden;
            _sendToBackgroundWorker.RunWorkerAsync();
        }

        private void OnSendToWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            ObservableCollection<FileItem> sendToFileItems = e.Result as ObservableCollection<FileItem>;
            if (sendToFileItems == null) return;
            lstSendTo.ItemsSource = sendToFileItems;
            txtSendToLoading.Visibility = Visibility.Hidden;
            lstSendTo.Visibility = Visibility.Visible;
        }

        private void OnSendToWorkerStarted(object sender, DoWorkEventArgs e) {
            string sendToPath = SendToTask.GetFolderPath(_windowsOs);
            FileReader fileReader = new FileReader(sendToPath, new List<string>() { ".ini" });
            ObservableCollection<FileItem> sendToFileItems = fileReader.GetAllFiles();
            e.Result = sendToFileItems;
        }

        private void OnButtonAddFolderToSendToClick(object sender, RoutedEventArgs e) {
            try {
                if (SendToTask.AddFolder(_windowsOs)) {
                    ReloadSendTo();
                }
            } catch (BadImageFormatException) {
                _message.Error("Because of an error, add operations for operations have been disabled");
                btnAddFileToSendTo.IsEnabled = btnAddFolderToSendTo.IsEnabled = false;
            } catch (FileNotFoundException) {
                _message.Error("File path is not valid, hence not creating any shortcut");
            }
        }

        private void OnButtonAddFileToSendToClick(object sender, RoutedEventArgs e) {
            try {
                if (SendToTask.AddFile(_windowsOs)) {
                    ReloadSendTo();
                }
            } catch (BadImageFormatException) {
                _message.Error("Because of an error, add operations for operations have been disabled");
                btnAddFileToSendTo.IsEnabled = btnAddFolderToSendTo.IsEnabled = false;
            } catch (FileNotFoundException) {
                _message.Error("File path is not valid, hence not creating any shortcut");
            }
        }

        private void OnButtonDeleteFromSendToClick(object sender, RoutedEventArgs e) {
            SendToTask.Delete(lstSendTo, _message);
            ReloadSendTo();
        }

        private void OnLinkEditSendToExplorerClick(object sender, RoutedEventArgs e) {
            string sendToPath = SendToTask.GetFolderPath(_windowsOs);
            try {
                Process.Start(sendToPath);
            } catch (Win32Exception) {
                _message.Error("The SendTo Path is different on your Windows");
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
                _message.Error("Please enter a name for the shortcut");
                return;
            }
            if (shrtCtName == "cmd") {
                _message.Error("Can't create shortcut with name \"cmd\" since it is reserved by Windows. Please try a different name.");
            }
            string shrtCtPath = txtShrtCtPath.Text.Trim();
            if (shrtCtPath.Length == 0) {
                _message.Error("Please enter a valid file-path or url");
                return;
            }
            bool result = RightClickAddDeleteTask.Add(shrtCtName, shrtCtPath);
            if (!result) {
                string msg = String.Format("Unable to identify \"{0}\" as a valid file-path or url." +
                                           "\n Do you still want to proceed anyways and create a shortcut?" +
                                           "\n PS : You might want to do this if this file is added to your PATH.", shrtCtPath);
                if (MessageBox.Show(msg, Constants.WarningMsgTitle, MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    RightClickAddDeleteTask.AddToRegistry(shrtCtName, shrtCtPath);
                    result = true;
                }
            }
            if (!result) {
                _message.Error("Please enter a valid file-path or url");
            } else {
                _message.Success("Successfully added " + shrtCtName + " to Right-Click");
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
                _message.Error("You haven't added anything to the Right-Click");
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
            _message.Success("Sucessfully deleted " + fileItem.Tag + " from Right-Click menu");
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
                    _message.Success("Shutdown has been scheduled on " + selectedDateTime.Value.ToString("MMMM d, yyyy ") 
                        + " at " + selectedDateTime.Value.ToString("hh:mm tt"));
                }
                else {
                    _message.Error("Invalid Timeout!\n The time can\'t be less than the current time. Also the time can\'t" +
                                  " exceed the limit of 10 years.");
                }
            }
            else {
                _message.Error("Please select a date!");
            }
        }

        private void OnCancelShutdownButtonClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteDosCmd("shutdown /a");
            _message.Success("Previously scheduled shutdown has been cancelled");
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
               _message.Success(cmboBxSpecialFolderNames.SelectionBoxItem + " created successfully at " + parentPath);
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

        #region Features
        private void LoadFeaturesTab() {
            // System Beep
            using (RegistryKey hkcuSound = _hkcu.OpenSubKey(@"Control Panel\Sound")) {
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

            using (RegistryKey hkcuExplorer = _hkcu.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Windows DVD Burner
                chkWinDvdBurner.SetCheckedState(hkcuExplorer, Constants.NoDvdBurning, true);
                
                // AutoPlay
                RegistryKey hkcuAutoplayHandlers = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers");
                RegistryKey hklmCdRom = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\cdrom");

                int? driveAutoRunVal = (int?) hkcuExplorer.GetValue(Constants.NoDriveAutoPlay);
                int? disableAutoPlayVal = (int?) hkcuAutoplayHandlers.GetValue(Constants.DisableAutoplay);
                int? autoRunVal = (int?) hklmCdRom.GetValue(Constants.AutoRun);
                if (driveAutoRunVal.HasValue && disableAutoPlayVal.HasValue && autoRunVal.HasValue) {
                    if (disableAutoPlayVal == 1 || autoRunVal == 0) {
                        rbtnDisableAutoPlay.IsChecked = true;
                    } else {
                        switch (driveAutoRunVal) {
                            case 145:
                                rbtnEnableAutoPlay.IsChecked = true;
                                break;
                            case 181:
                                rbtnEnableCdUsbAutoPlay.IsChecked = true;
                                break;
                            case 255:
                                rbtnDisableAutoPlay.IsChecked = true;
                                break;
                        }
                    }
                } else if (!driveAutoRunVal.HasValue && disableAutoPlayVal.HasValue && autoRunVal.HasValue) {
                    if (disableAutoPlayVal == 1 || autoRunVal == 0) {
                        rbtnDisableAutoPlay.IsChecked = true;
                    } else {
                        rbtnEnableAutoPlay.IsChecked = true;
                    }
                } else if (!driveAutoRunVal.HasValue && !disableAutoPlayVal.HasValue && autoRunVal.HasValue) {
                    if (autoRunVal == 1) {
                        rbtnEnableAutoPlay.IsChecked = true;
                    } else {
                        rbtnDisableAutoPlay.IsChecked = true;
                    }
                }
                hkcuAutoplayHandlers.Close();
                hklmCdRom.Close();
            }

            // Windows Update
            using (RegistryKey hklmAutoUpdate = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update")) {
                chkRecommenedUpdates.SetCheckedState(hklmAutoUpdate, Constants.IncludeRecommendedUpdates);
                chkAllowAllInstallUpdate.SetCheckedState(hklmAutoUpdate, Constants.ElevateNonAdmins);
                chkNotifyNewMSSw.SetCheckedState(hklmAutoUpdate, Constants.EnableFeaturedSoftware);
                int? autoUpdateVal = (int?) hklmAutoUpdate.GetValue(Constants.AutoUpdateOptions);
                switch (autoUpdateVal) {
                    case 1:
                        cmboBxWinUpdate.SelectedIndex = 3;
                        break;
                    case 2:
                        cmboBxWinUpdate.SelectedIndex = 2;
                        break;
                    case 3:
                        cmboBxWinUpdate.SelectedIndex = 1;
                        break;
                    case 4:
                        cmboBxWinUpdate.SelectedIndex = 0;
                        break;
                    default:
                        if (!cmboBxWinUpdate.Items.Contains(Constants.DefaultWindowsUpdateText)) {
                            cmboBxWinUpdate.Items.Add(Constants.DefaultWindowsUpdateText);
                            cmboBxWinUpdate.SelectedItem = Constants.DefaultWindowsUpdateText;
                        }
                        break;
                }
            }
        }

        private void OnLinkActivateAdminClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteProcess("net", "user administrator /active:yes");
            _message.Success("Successfully activated the Administrator account");
        }

        private void OnLinkDeactivateAdminAccountClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteProcess("net", "user administrator /active:no");
            _message.Success("Successfully de-activated the Administrator account");
        }

        private void UpdateRegistryFromFeatures() {
            // System Beeps
            if (chkSystemBeep.HasUserInteracted()) {
                using (RegistryKey hkcuSound = _hkcu.CreateSubKey(@"Control Panel\Sound")) {
                    string val = chkSystemBeep.IsChecked == true ? Constants.No : Constants.Yes;
                    hkcuSound.SetValue(Constants.Beep, val);
                }
            }

            using (RegistryKey hkcuExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Windows DVD Burner
                if (chkWinDvdBurner.HasUserInteracted()) {
                    if (chkWinDvdBurner.IsChecked == true) {
                        if (hkcuExplorer.GetValue(Constants.NoDvdBurning) != null) {
                            hkcuExplorer.DeleteValue(Constants.NoDvdBurning);
                        }
                    }
                    else {
                        hkcuExplorer.SetValue(Constants.NoDvdBurning, 1);
                    }
                }
                // AutoPlay
                RegistryKey hkcuAutoplayHandlers = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers");
                RegistryKey hklmCdRom = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\cdrom");
                if (rbtnEnableAutoPlay.IsChecked == true) {
                    hkcuExplorer.SetValue(Constants.NoDriveAutoPlay, 145);
                    hkcuAutoplayHandlers.SetValue(Constants.DisableAutoplay, 0);
                    hklmCdRom.SetValue(Constants.AutoRun, 1);
                } else if (rbtnEnableCdUsbAutoPlay.IsChecked == true) {
                    hkcuExplorer.SetValue(Constants.NoDriveAutoPlay, 181);
                    hkcuAutoplayHandlers.SetValue(Constants.DisableAutoplay, 0);
                    hklmCdRom.SetValue(Constants.AutoRun, 1);
                } else if (rbtnDisableAutoPlay.IsChecked == true) {
                    hkcuExplorer.SetValue(Constants.NoDriveAutoPlay, 255);
                    hkcuAutoplayHandlers.SetValue(Constants.DisableAutoplay, 1);
                    hklmCdRom.SetValue(Constants.AutoRun, 0);
                }
                hkcuAutoplayHandlers.Close();
                hklmCdRom.Close();
            }

            // Windows Update
            using (RegistryKey hklmAutoUpdate = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update")) {
                hklmAutoUpdate.SetValue(chkRecommenedUpdates, Constants.IncludeRecommendedUpdates);
                hklmAutoUpdate.SetValue(chkAllowAllInstallUpdate, Constants.ElevateNonAdmins);
                hklmAutoUpdate.SetValue(chkNotifyNewMSSw, Constants.EnableFeaturedSoftware);
                switch (cmboBxWinUpdate.SelectedIndex) {
                    case 0: hklmAutoUpdate.SetValue(Constants.AutoUpdateOptions, 4);
                        break;
                    case 1: hklmAutoUpdate.SetValue(Constants.AutoUpdateOptions, 3);
                        break;
                    case 2: hklmAutoUpdate.SetValue(Constants.AutoUpdateOptions, 2);
                        break;
                    case 3: hklmAutoUpdate.SetValue(Constants.AutoUpdateOptions, 1);
                        break;
                }
            }
        }
        #endregion

        #region Maintenance
        private void LoadMaintenanceTab() {
            // Memory
            using (RegistryKey hklmCVExplorer = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
                chkUnloadUnsedDLL.SetCheckedState(hklmCVExplorer, Constants.AlwaysUnloadDll);
            }
            using (RegistryKey hklmSystem = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                int? machineVal = (int?) hklmSystem.GetValue(Constants.MachineGroupPolicy);
                int? userVal = (int?) hklmSystem.GetValue(Constants.UserGroupPolicy);
                chkDisableGroupPolicy.IsChecked = Utils.ReversedIntToBool(machineVal) && Utils.ReversedIntToBool(userVal);
            }

            // Auto Reboot
            using (RegistryKey hklmCrashControl = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\CrashControl")) {
                chkRecordCrashEvent.SetCheckedState(hklmCrashControl, Constants.LogEvent);
                int? val = (int?) hklmCrashControl.GetValue(Constants.AutoReboot);
                cmboBxWinCrashAction.SelectedIndex = val == 0 ? 1 : 0;
            }

            // Startup Settings
            using (RegistryKey hklmWinLogon = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon")) {
                chkDisableLKGC.SetCheckedStateFromString(hklmWinLogon, Constants.ReportBootOk, true);
            }
            using (RegistryKey hklmBootOptFunc = _hklm.CreateSubKey(@"Software\Microsoft\Dfrg\BootOptimizeFunction")) {
                string val = (string) hklmBootOptFunc.GetValue(Constants.Enable);
                chkEnableBootDefrag.IsChecked = val == null || val.Equals("Y");
            }

            // Error
            using (RegistryKey hkcuWinErrReporting = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting")) {
                chkHideErrReporting.SetCheckedState(hkcuWinErrReporting, Constants.DontShowUi);
                using (RegistryKey hkcuConsent = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting\Consent")) {
                    int? defaultConsentVal = (int?) hkcuConsent.GetValue(Constants.DefaultConsent);
                    int? disabledVal = (int?) hkcuWinErrReporting.GetValue(Constants.Disabled);
                    if (defaultConsentVal == null && disabledVal == null) {
                        cmboBxErrReportTye.SelectedIndex = 1;
                    } else if (disabledVal == 0 || disabledVal == null) {
                        switch (defaultConsentVal) {
                            case 1:
                                cmboBxErrReportTye.SelectedIndex = 0;
                                break;
                            case 2:
                                cmboBxErrReportTye.SelectedIndex = 1;
                                break;
                            case 3:
                                cmboBxErrReportTye.SelectedIndex = 2;
                                break;
                            default:
                                cmboBxErrReportTye.SelectedIndex = 0;
                                break;
                        }
                    } else if(disabledVal == 1) {
                        cmboBxErrReportTye.SelectedIndex = 3;
                    }
                }
            }
        }

        private void UpdateSettingsFromMaintenance() {
            // Memory
            using (RegistryKey hklmCVExplorer = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
                hklmCVExplorer.SetValue(chkUnloadUnsedDLL, Constants.AlwaysUnloadDll);
            }
            if (chkDisableGroupPolicy.HasUserInteracted()) {
                using (RegistryKey hklmSystem = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System")) {
                    if (chkDisableGroupPolicy.IsChecked == true) {
                        hklmSystem.SetValue(Constants.MachineGroupPolicy, 0);
                        hklmSystem.SetValue(Constants.UserGroupPolicy, 0);
                    } else {
                        hklmSystem.DeleteValue(Constants.MachineGroupPolicy, false);
                        hklmSystem.DeleteValue(Constants.UserGroupPolicy, false);
                    }
                }
            }

            // Auto Reboot
            using (RegistryKey hklmCrashControl = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\CrashControl")) {
                hklmCrashControl.SetValue(chkRecordCrashEvent, Constants.LogEvent);
                hklmCrashControl.SetValue(Constants.AutoReboot, cmboBxWinCrashAction.SelectedIndex == 0 ? 1 : 0);
            }

            // Startup Settings
            using (RegistryKey hklmWinLogon = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon")) {
                hklmWinLogon.SetStringValue(chkDisableLKGC, Constants.ReportBootOk, true);
            }

            if (chkEnableBootDefrag.HasUserInteracted()) {
                using (RegistryKey hklmBootOptFunc = _hklm.CreateSubKey(@"Software\Microsoft\Dfrg\BootOptimizeFunction")) {
                    hklmBootOptFunc.SetValue(Constants.Enable, chkEnableBootDefrag.IsChecked == true ? "Y" : "N");
                }
            }

            // Error
            using (RegistryKey hkcuWinErrReporting = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting")) {
                hkcuWinErrReporting.SetValue(chkHideErrReporting, Constants.DontShowUi);
                using (RegistryKey hkcuConsent = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting\Consent")) {
                    switch (cmboBxErrReportTye.SelectedIndex) {
                        case 0:
                            hkcuWinErrReporting.SetValue(Constants.Disabled, 0);
                            hkcuConsent.SetValue(Constants.DefaultConsent, 1);
                            break;
                        case 1:
                            hkcuWinErrReporting.SetValue(Constants.Disabled, 0);
                            hkcuConsent.SetValue(Constants.DefaultConsent, 2);
                            break;
                        case 2:
                            hkcuWinErrReporting.SetValue(Constants.Disabled, 0);
                            hkcuConsent.SetValue(Constants.DefaultConsent, 3);
                            break;
                        case 3:
                            hkcuWinErrReporting.SetValue(Constants.Disabled, 1);
                            break;
                    }
                }
            }
        }
        #endregion

        #region Search
        private void OnSearchTextBoxKeyUp(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            WatermarkTextBox txtSearchBox = sender as WatermarkTextBox;
            if (txtSearchBox == null) return;
            String searchTxt = txtSearchBox.Text.Trim();
            if (searchTxt.Length < 3) {
                _message.Error("Minimum 3 characters should be entered");
                return;
            }
            _message.Hide();
            new Search.Search(searchTxt);
        }
        #endregion
    }
}