using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WindowsTweaker.AppTasks;
using WindowsTweaker.Models;
using WindowsTweaker.Search;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFFolderBrowser;
using File = System.IO.File;
using WinInterop = System.Windows.Interop;

namespace WindowsTweaker {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            _message = new Message(msgContainerBorder, txtMsg);
            string initializeSearch = GetResourceString("InitializingSearch");
            _message.Notify(initializeSearch);
            _sendToBackgroundWorker = (BackgroundWorker) this.FindResource("sendToBackgroundWorker");
            _searchBackgroundWorker = (BackgroundWorker) this.FindResource("searchBackgroundWorker");
            _updateCheckBackgroundWorker = (BackgroundWorker) this.FindResource("updateCheckBackgroundWorker");
            _selectionColor = _defaultSelectionColor;
            _searcher = new Searcher(this);
            FocusSearchCommand.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
            stxtSearchInput.Focus();
            _message.Hide();
            _hasTabLoadedDict = new Dictionary<string, bool>();
            UpdateLanguageMenu();
            _sendToTask = new SendToTask(this);
            mainWindow.SourceInitialized += new EventHandler(OnWindowSourceInitialized);
            cmboBxFileSizes.ItemsSource = CreateFileTask.SizeDataDict;
            CheckForUpdate();
        }

        private readonly RegistryKey _hkcu = Registry.CurrentUser;
        private readonly RegistryKey _hklm = Registry.LocalMachine;
        private readonly RegistryKey _hkcr = Registry.ClassesRoot;
        private readonly WindowsVer.Windows _windowsOs = WindowsVer.Instance.GetName();
        private readonly Color _defaultSelectionColor = Color.FromArgb(255, 0, 102, 204);
        private readonly Message _message;
        private readonly BackgroundWorker _sendToBackgroundWorker;
        private readonly BackgroundWorker _searchBackgroundWorker;
        private readonly BackgroundWorker _updateCheckBackgroundWorker;
        private Color _selectionColor;
        private readonly Searcher _searcher;
        private readonly Dictionary<string, bool> _hasTabLoadedDict;
        private readonly SendToTask _sendToTask;

        public static RoutedCommand FocusSearchCommand = new RoutedCommand();

        #region MaximizeButtonHandling

        // http://blogs.msdn.com/b/llobo/archive/2006/08/01/maximizing-window-_2800_with-windowstyle_3d00_none_2900_-considering-taskbar.aspx
        private void OnWindowSourceInitialized(object sender, EventArgs e) {
            IntPtr handle = (new WinInterop.WindowInteropHelper(this)).Handle;
            var hwndSource = WinInterop.HwndSource.FromHwnd(handle);
            if (hwndSource != null) hwndSource.AddHook(WindowProc);
        }

        private static IntPtr WindowProc(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled) {
            switch (msg) {
                case 0x0024:
                    WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
            }

            return (IntPtr) 0;
        }

        private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam) {
            Minmaxinfo mmi = (Minmaxinfo) Marshal.PtrToStructure(lParam, typeof (Minmaxinfo));

            // Adjust the maximized size and position to fit the work area of the correct monitor
            const int monitorDefaulttonearest = 0x00000002;
            IntPtr monitor = MonitorFromWindow(hwnd, monitorDefaulttonearest);

            if (monitor != IntPtr.Zero) {
                Monitorinfo monitorInfo = new Monitorinfo();
                GetMonitorInfo(monitor, monitorInfo);
                MRect rcWorkArea = monitorInfo.rcWork;
                MRect rcMonitorArea = monitorInfo.rcMonitor;
                mmi.ptMaxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                mmi.ptMaxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                mmi.ptMaxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                mmi.ptMaxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
            }

            Marshal.StructureToPtr(mmi, lParam, true);
        }


        /// <summary>
        ///     MPoint aka POINTAPI
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MPoint {
            /// <summary>
            ///     x coordinate of point.
            /// </summary>
            public int x;

            /// <summary>
            ///     y coordinate of point.
            /// </summary>
            public int y;

            /// <summary>
            ///     Construct a point of coordinates (x,y).
            /// </summary>
            public MPoint(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Minmaxinfo {
            public MPoint ptReserved;
            public MPoint ptMaxSize;
            public MPoint ptMaxPosition;
            public MPoint ptMinTrackSize;
            public MPoint ptMaxTrackSize;
        };


        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class Monitorinfo {
            /// <summary>
            /// </summary>
            public int cbSize = Marshal.SizeOf(typeof (Monitorinfo));

            /// <summary>
            /// </summary>
            public MRect rcMonitor = new MRect();

            /// <summary>
            /// </summary>
            public MRect rcWork = new MRect();

            /// <summary>
            /// </summary>
            public int dwFlags = 0;
        }


        /// <summary> Win32 </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct MRect {
            /// <summary> Win32 </summary>
            public int left;

            /// <summary> Win32 </summary>
            public int top;

            /// <summary> Win32 </summary>
            public int right;

            /// <summary> Win32 </summary>
            public int bottom;

            /// <summary> Win32 </summary>
            public static readonly MRect Empty = new MRect();

            /// <summary> Win32 </summary>
            public int Width {
                get { return Math.Abs(right - left); } // Abs needed for BIDI OS
            }

            /// <summary> Win32 </summary>
            public int Height {
                get { return bottom - top; }
            }

            /// <summary> Win32 </summary>
            public MRect(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }


            /// <summary> Win32 </summary>
            public MRect(MRect rcSrc) {
                left = rcSrc.left;
                top = rcSrc.top;
                right = rcSrc.right;
                bottom = rcSrc.bottom;
            }

            /// <summary> Win32 </summary>
            public bool IsEmpty {
                get {
                    // BUGBUG : On Bidi OS (hebrew arabic) left > right
                    return left >= right || top >= bottom;
                }
            }

            /// <summary> Return a user friendly representation of this struct </summary>
            public override string ToString() {
                if (this == MRect.Empty) {
                    return "MRect {Empty}";
                }
                return "MRect { left : " + left + " / top : " + top + " / right : " + right + " / bottom : " + bottom + " }";
            }

            /// <summary> Determine if 2 MRect are equal (deep compare) </summary>
            public override bool Equals(object obj) {
                if (!(obj is Rect)) {
                    return false;
                }
                return (this == (MRect) obj);
            }

            /// <summary>Return the HashCode for this struct (not garanteed to be unique)</summary>
            public override int GetHashCode() {
                return left.GetHashCode() + top.GetHashCode() + right.GetHashCode() + bottom.GetHashCode();
            }


            /// <summary> Determine if 2 MRect are equal (deep compare)</summary>
            public static bool operator ==(MRect rect1, MRect rect2) {
                return (rect1.left == rect2.left && rect1.top == rect2.top && rect1.right == rect2.right && rect1.bottom == rect2.bottom);
            }

            /// <summary> Determine if 2 MRect are different(deep compare)</summary>
            public static bool operator !=(MRect rect1, MRect rect2) {
                return !(rect1 == rect2);
            }
        }

        [DllImport("user32")]
        internal static extern bool GetMonitorInfo(IntPtr hMonitor, Monitorinfo lpmi);

        /// <summary>
        /// </summary>
        [DllImport("User32")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);
        #endregion

        #region Common Code
        private void OnTabLoaded(object sender, RoutedEventArgs e) {
            string tagVal = ((TabItem) sender).Tag.ToString();
            LoadTab(tagVal);
        }

        private void LoadTab(string tagVal) {
            if (_hasTabLoadedDict.ContainsKey(tagVal) && _hasTabLoadedDict[tagVal])
                return;
            _hasTabLoadedDict[tagVal] = true;
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
                    LoadPlacesTab();
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

        private string GetResourceString(string key) {
            return this.FindResource(key) as string;
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

        private void OnApplyButtonClick(object sender, RoutedEventArgs e) {
            SaveSettings();
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e) {
            SaveSettings();
            Environment.Exit(0);
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private void SaveSettings() {
            var userInteractedTabs = _hasTabLoadedDict.Where(kvp => kvp.Value);
            foreach (var userInteractedTab in userInteractedTabs) {
                switch (userInteractedTab.Key) {
                    case Constants.Explorer:
                        UpdateRegistryFromExplorer();
                        break;
                    case Constants.System:
                        UpdateRegistryFromSystem();
                        break;
                    case Constants.Display:
                        UpdateRegistryFromDisplay();
                        break;
                    case Constants.Places:
                        UpdateRegistryFromPlaces();
                        break;
                    case Constants.RightClick:
                        UpdateRegistryFromRightClick();
                        break;
                    case Constants.Features:
                        UpdateRegistryFromFeatures();
                        break;
                    case Constants.Logon:
                        UpdateRegistryFromLogon();
                        break;
                    case Constants.Restrictions:
                        UpdateRegistryFromRestrictions();
                        break;
                    case Constants.Maintenance:
                        UpdateSettingsFromMaintenance();
                        break;
                }
            }
            InfoBox restartInfoBox = new InfoBox(GetResourceString("RestartBoxText"), GetResourceString("RestartBoxOkBtnTxt"), 
                GetResourceString("RestartBoxCancelTxt"), GetResourceString("RestartBoxHeading"), InfoBox.DialogType.Question);
            if (restartInfoBox.ShowDialog() == true) {
                ProcessWrapper.ExecuteProcess("shutdown.exe", "/r");
                Environment.Exit(0);
            }
        }
        #endregion

        #region Login
        private void LoadLogonTab() {
            // Auto Login
            using (RegistryKey hklmWinLogon = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon")) {
                chkEnableAutoLogin.SetCheckedStateFromString(hklmWinLogon, Constants.AutoAdminLogon);
                if (chkEnableAutoLogin.IsChecked == true) {
                    txtAutoLoginUserName.SetText(hklmWinLogon, Constants.DefaultUserName);
                    txtAutoLoginPasswd.SetPassword(hklmWinLogon, Constants.DefaultPassword);
                    txtAutoLoginDomainName.SetText(hklmWinLogon, Constants.DefaultDomainName);
                }
                ToggleAutoLoginUiState();

                chkPreventShiftPress.SetCheckedStateFromString(hklmWinLogon, Constants.IgnoreShiftOverride);
                chkAutoLogonAfterLogoff.SetCheckedStateFromString(hklmWinLogon, Constants.ForceAutoLogon);
                // Miscellaneous
                chkRequireCtrlAltDlt.SetCheckedState(hklmWinLogon, Constants.DisableCtrlAltDlt, true);
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

            // Lock Screen
            if (_windowsOs >= WindowsVer.Windows.Eight) {
                using (RegistryKey hklmPersonalization = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Personalization")) {
                    chkLockScreenImg.SetCheckedState(hklmPersonalization, Constants.NoChangingLockScreen);
                    chkLockScreen.SetCheckedState(hklmPersonalization, Constants.NoScreenLock);
                }
            } else {
                chkLockScreen.IsEnabled = chkLockScreenImg.IsEnabled = false;
                txtLockScreen.Text += " " + GetResourceString("OnlyWin8AndOnwards");
            }
        }

        private void UpdateRegistryFromLogon() {
            // Auto Login
            using (RegistryKey hklmWinLogon = _hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon")) {
                hklmWinLogon.SetStringValue(chkEnableAutoLogin, Constants.AutoAdminLogon);
                hklmWinLogon.SetValue(txtAutoLoginUserName, Constants.DefaultUserName);
                hklmWinLogon.SetValue(txtAutoLoginPasswd, Constants.DefaultPassword);
                hklmWinLogon.SetValue(txtAutoLoginDomainName, Constants.DefaultDomainName);
                hklmWinLogon.SetStringValue(chkPreventShiftPress, Constants.IgnoreShiftOverride);
                hklmWinLogon.SetStringValue(chkAutoLogonAfterLogoff, Constants.ForceAutoLogon);
                hklmWinLogon.SetValue(chkRequireCtrlAltDlt, Constants.DisableCtrlAltDlt, true);
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
                hklmSystem.SetValue(chkRestrictLastLoginUser, Constants.NoLastUserName);
                hklmSystem.SetValue(chkShowShutdownBtn, Constants.ShutdownWithoutLogon);

                // Login Message
                hklmSystem.SetValue(txtLoginMsgTitle, Constants.LoginMsgTitle);
                hklmSystem.SetValue(txtLoginMsgContent, Constants.LoginMsgContent);
            }

            // Lock Screen
            if (_windowsOs >= WindowsVer.Windows.Eight) {
                using (RegistryKey hklmPersonalization = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Personalization")) {
                    hklmPersonalization.SetValue(chkLockScreenImg, Constants.NoChangingLockScreen);
                    hklmPersonalization.SetValue(chkLockScreen, Constants.NoScreenLock);
                }
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
        private void LoadRestrictionsTab() {
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
                chkTaskBarAnim.SetCheckedState(hkcuExAdvanced, Constants.TaskBarAnimations, true);
                if (_windowsOs > WindowsVer.Windows.Xp) {
                    chkShowIconsTaskBar.SetCheckedState(hkcuExAdvanced, Constants.TaskBarSmallIcons);
                } else {
                    txtShowIconsTaskBar.Text += " " + GetResourceString("OnlyVistaAndOnwards");
                    chkShowIconsTaskBar.IsEnabled = false;
                }
                chkTaskBarNoTooltip.SetCheckedState(hkcuExAdvanced, Constants.ShowInfoTip, true);

            }
            if (_windowsOs > WindowsVer.Windows.Xp && _windowsOs < WindowsVer.Windows.Eight) {
                using (RegistryKey hkcuDWM = _hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\DWM")) {
                    // Explorer
                    chkHide3DFlip.SetCheckedState(hkcuDWM, Constants.DisAllowFlip_3D);
                }
            } else {
                txtDisableFlip3D.Text += " " + GetResourceString("OnlyVistaAnd7");
                chkHide3DFlip.IsEnabled = false;
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

        private void UpdateRegistryFromRestrictions() {

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
                hkcuExAdvanced.SetValue(chkTaskBarAnim, Constants.TaskBarAnimations, true);
                if (_windowsOs > WindowsVer.Windows.Xp) {
                    hkcuExAdvanced.SetValue(chkShowIconsTaskBar, Constants.TaskBarSmallIcons);
                }
                hkcuExAdvanced.SetValue(chkTaskBarNoTooltip, Constants.ShowInfoTip, true);

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

            // Advanced
            if (_windowsOs >= WindowsVer.Windows.Eight) {
                using (RegistryKey hklmExplorer = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Explorer")) {
                    chkNoNewAppAlert.SetCheckedState(hklmExplorer, Constants.NoNewAppAlert);
                }
            } else {
                chkNoNewAppAlert.IsEnabled = false;
                txtNoNewAppAlert.Text += " " + GetResourceString("OnlyWin8AndOnwards");
            }

            using (RegistryKey hklmNamespace = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace")) {
                // Libraries
                if (_windowsOs >= WindowsVer.Windows.Vista) {
                    RegistryKey key = hklmNamespace.OpenSubKey(Constants.Library);
                    if (key != null) {
                        rbtnShowLibraries.IsChecked = true;
                    }
                    else {
                        rbtnHideLibraries.IsChecked = true;
                    }
                } else {
                    txtLibraries.Text += " " + GetResourceString("OnlyVistaAndOnwards");
                    panelLibraries.IsEnabled = false;
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

                using (RegistryKey hkcuAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                    chkReplaceCmdPromptWithPs.SetCheckedState(hkcuAdvanced, Constants.DontUsePowerShellOnWinX, true);
                }
            } else {
                txtStartScreen.Text += " " + GetResourceString("OnlyWin8AndOnwards");
                txtNavTweaks.Text += " " + GetResourceString("OnlyWin8AndOnwards");
                panelStartScreen.IsEnabled  = panelNavTweaks.IsEnabled= false;
            }
        }

        private void UpdateRegistryFromExplorer() {
            //Drive Letters
            using (RegistryKey hkcuCvExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer")) {
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

            // Advanced
            if (_windowsOs >= WindowsVer.Windows.Eight) {
                using (RegistryKey hklmExplorer = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Explorer")) {
                    chkNoNewAppAlert.SetCheckedState(hklmExplorer, Constants.NoNewAppAlert);
                    hklmExplorer.SetValue(chkNoNewAppAlert, Constants.NoNewAppAlert);
                }
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

            RegistryKey hkcrCLSID = _hkcr.OpenSubKey(@"CLSID", true);
            RegistryKey hklmCPNamespace =
                _hklm.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace", true);
            if (chkAddRegeditToControlPanel.IsChecked == true) {
                if (hkcrCLSID == null)
                    hkcrCLSID = _hkcr.CreateSubKey(@"CLSID");
                if (hklmCPNamespace == null)
                    hklmCPNamespace =
                        _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
                using (RegistryKey cp = hkcrCLSID.CreateSubKey(Constants.Regedit)) {
                    cp.SetValue("", "Registry Editor");
                    cp.SetValue("InfoTip", "Start the Registry Editor");
                    cp.SetValue("System.ControlPanel.Category", "5");
                    RegistryKey defaultIcon = cp.CreateSubKey("DefaultIcon");
                    defaultIcon.SetValue("", @"%SYSTEMROOT%\regedit.exe");
                    RegistryKey cmd = hkcrCLSID.CreateSubKey(Constants.Regedit + @"\Shell\Open\command");
                    cmd.SetValue("", Environment.ExpandEnvironmentVariables("%SystemRoot%\\regedit.exe"));
                    cmd.Close();
                    defaultIcon.Close();
                    RegistryKey cpNameSpace = hklmCPNamespace.CreateSubKey(Constants.Regedit);
                    cpNameSpace.SetValue("", "Add Registry Editor to Control Panel");
                    cpNameSpace.Close();
                }

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

                using (RegistryKey hkcuAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
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

                try {
                    val = (string) hkcuDesktop.GetValue(Constants.WaitToKillAppTimeout);
                }
                catch (InvalidCastException) {
                    val = Utils.RepairAsStringFromInt(hkcuDesktop, Constants.WaitToKillAppTimeout);
                }
                if (!String.IsNullOrEmpty(val)) {
                    try {
                        nudAppKillTimeout.Value = (int) Double.Parse(val);
                    }
                    catch (InvalidCastException) {
                        nudAppKillTimeout.Value = 4000;
                    }
                    catch (FormatException) {
                        // Even if there's a format exception, then set it to default
                        // Exception can happen, if there's a alpanumeric string as value o_O
                        nudAppKillTimeout.Value = 4000;
                    }
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
                            try {
                                imgProperty.Source = new BitmapImage(uriLogo);
                            }
                            catch (FileNotFoundException) {
                                imgProperty.Source = null;
                            }
                            catch (DirectoryNotFoundException) {
                                imgProperty.Source = null;
                            }
                            catch (ArgumentException) {
                                imgProperty.Source = null;
                            }
                            catch (NotSupportedException) {
                                imgProperty.Source = null;
                            }
                        }
                    }
                    catch (FileNotFoundException) {
                        imgProperty.Source = null;
                        hklmOEM.DeleteValue(Constants.Logo);
                    }
                    catch (DirectoryNotFoundException) {
                        imgProperty.Source = null;
                        hklmOEM.DeleteValue(Constants.Logo);
                    }
                    catch (ArgumentException) {
                        imgProperty.Source = null;
                    }
                    catch (NotSupportedException) {
                        imgProperty.Source = null;
                        hklmOEM.DeleteValue(Constants.Logo);
                    }
                }
            }

            // Search
            using (RegistryKey hkcuExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                chkPreventStaleShortcutSearch.SetCheckedState(hkcuExplorer, Constants.LinkResolveIgnoreLinkInfo);
                chkPreventStateShortcutDiskSearch.SetCheckedState(hkcuExplorer, Constants.NoResolveSearch);
                chkPreventUseOfNtfsTrack.SetCheckedState(hkcuExplorer, Constants.NoResolveTrack);
            }
        }

        private void OnButtonSelectLogoClick(object sender, RoutedEventArgs e) {
            string filePath = Utils.GetUserSelectedFilePath("Bitmap Images|*.bmp");
            if (filePath == null) return;
            using (RegistryKey hklmOEM = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                hklmOEM.SetValue(Constants.Logo, filePath);
                imgProperty.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                btnDeleteLogo.IsEnabled = true;
                _message.Success(GetResourceString("ImageApplied"));
            }
        }

        private void OnButtonDeleteLogoClick(object sender, RoutedEventArgs e) {
            using (RegistryKey hklmOEM = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation")) {
                string logoUrl = (string) hklmOEM.GetValue(Constants.Logo);
                if (!String.IsNullOrEmpty(logoUrl)) {
                    hklmOEM.SetValue(Constants.Logo, "");
                    btnDeleteLogo.IsEnabled = false;
                    imgProperty.Source = null;
                    _message.Success(GetResourceString("ImageRemoved"));
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
                        hkcuDesktop.SetValue(Constants.WaitToKillAppTimeout, nudAppKillTimeout.Value.ToString());
                    }
                }
            }

            using (RegistryKey hklmWinInstaller = _hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer")) {
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

            // Search
            using (RegistryKey hkcuExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                hkcuExplorer.SetValue(chkPreventStaleShortcutSearch, Constants.LinkResolveIgnoreLinkInfo);
                hkcuExplorer.SetValue(chkPreventStateShortcutDiskSearch, Constants.NoResolveSearch);
                hkcuExplorer.SetValue(chkPreventUseOfNtfsTrack, Constants.NoResolveTrack);
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
                string iconTitleWrapVal = null;
                try {
                    iconTitleWrapVal = (string) hkcuWinMet.GetValue(Constants.IconTitleWrap, "1");
                }
                catch (InvalidCastException) {
                    iconTitleWrapVal = Utils.RepairAsStringFromInt(hkcuWinMet, Constants.IconTitleWrap);
                }
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
                hklmExplorer.SetValue(chkOldStyleFileSort, Constants.OldStyleFileSort);
            }

            // Icon Title
            using (RegistryKey hkcuWinMet = _hkcu.CreateSubKey(@"Control Panel\Desktop\WindowMetrics")) {
                string val = rbtnWrapText.IsChecked == true ? "0" : "1";
                hkcuWinMet.SetValue(Constants.IconTitleWrap, val);
            }
        }

        private void OnShowHelpForOldStyleFileSortClick(object sender, MouseButtonEventArgs e) {
            Process.Start("http://support.microsoft.com/kb/319827");
        }

        private void OnShowHelpForWrapIconTitle(object sender, MouseButtonEventArgs e) {
            Process.Start("http://technet.microsoft.com/en-us/library/cc959648.aspx");
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
            _selectionColor = _defaultSelectionColor;
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
            // God Mode
            if (_windowsOs < WindowsVer.Windows.Vista) {
                lblSetupGodMode.Text += " " + GetResourceString("OnlyVistaAndOnwards");
                panelSetupGodMode.IsEnabled = false;
            }
            // Power Button
            if (_windowsOs <= WindowsVer.Windows.Seven) {
                using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                    int val = (int) hkcuExAdvanced.GetValue(Constants.StartPowerBtnAction, 2);
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
            } else {
                lblPowerBtnAction.Text += " " + GetResourceString("Only7AndBelow");
                cmboBxPowerBtnAction.IsEnabled = false;
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
                        rbtnOpenDlgForNoExt.IsChecked = true;
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
                        rbtnOpenDlgForUnknownExt.IsChecked = true;
                    }
                }
            }
        }

        private void UpdateRegistryFromPlaces() {
            if (_windowsOs <= WindowsVer.Windows.Seven) {
                int val = cmboBxPowerBtnAction.SelectedIndex;
                using (RegistryKey hkcuExAdvanced = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                    switch (val) {
                        case 0:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 2);
                            break;
                        case 1:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 64);
                            break;
                        case 2:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 16);
                            break;
                        case 3:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 4);
                            break;
                        case 4:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 512);
                            break;
                        case 5:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 1);
                            break;
                        case 6:
                            hkcuExAdvanced.SetValue(Constants.StartPowerBtnAction, 256);
                            break;
                    }
                }
            }
        }
        #endregion

        #region Places -> GodMode
        private void OnButtonSetupGodModeClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != true) return;
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
                        _message.Success(GetResourceString("SuccessfullyFolderCreate") + " " + parentDir +
                                         ". " + GetResourceString("PleaseNoteWinRefresh"));
                    }
                    catch (UnauthorizedAccessException) {
                        _message.Error(GetResourceString("PermissionDenied"));
                    }
                    catch (IOException ioe) {
                        _message.Error(ioe.Message);
                    }
                } else {
                    _message.Error(GetResourceString("YoCantMake") + " " + selectedFolderName + " . " + GetResourceString("AGodFolder") +
                                   GetResourceString("PleaseSelectAnEmptyFolderOrCreate"));
                }
            } else {
                _message.Error(selectedFolderName + " " + GetResourceString("IsNotEmptyForGod"));
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
                        _message.Success(GetResourceString("AddedToOpenWith"));
                        break;
                    case OpenWithTask.AddStatus.AlreadyPresent:
                        _message.Success(GetResourceString("FileAlreadyPresentInOpenWith"));
                        break;
                    case OpenWithTask.AddStatus.Failed:
                        _message.Error(GetResourceString("FailedToAddToOpenWith"));
                        break;
                }
            } else {
                _message.Error(GetResourceString("FileNotExist"));
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
                        _message.Success(GetResourceString("AddedToStartup"));
                        break;
                    case StartupManagerTask.AddStatus.AlreadyPresent:
                        _message.Error(GetResourceString("FilePresentInStartup"));
                        break;
                    case StartupManagerTask.AddStatus.Failed:
                        _message.Error(GetResourceString("FailedToAddToStartup"));
                        break;
                }
            } else {
                _message.Error(GetResourceString("FileNotExist"));
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
        private void ValidateAndFixKeys(RegistryKey rootKey) {
            string[] subKeyNames = rootKey.GetSubKeyNames();
            foreach (string keyName in subKeyNames) {
                RegistryKey keyToValidate = rootKey.OpenSubKey(keyName, true);
                string copyToVal = (string) keyToValidate.GetValue(Constants.CopyToId);
                if (copyToVal != null && !keyToValidate.Name.Equals(Constants.CopyTo)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.CopyTo);
                    keyToValidate.SetValue("", Constants.CopyToId);
                    continue;
                }
                string moveToVal = (string) keyToValidate.GetValue(Constants.MoveToId);
                if (moveToVal != null && !keyToValidate.Name.Equals(Constants.MoveTo)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.MoveTo);
                    keyToValidate.SetValue("", Constants.MoveToId);
                    continue;
                }
                string sendToVal = (string) keyToValidate.GetValue(Constants.SendToId);
                if (sendToVal != null && !keyToValidate.Name.Equals(Constants.SendTo)) {
                    keyToValidate.Close();
                    rootKey.DeleteSubKeyTree(keyName);
                    keyToValidate = rootKey.CreateSubKey(Constants.SendTo);
                    keyToValidate.SetValue("", Constants.SendToId);
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

            using (RegistryKey hklmExAdvanced =
                    _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                chkEncryptAndDecrypt.SetCheckedState(hklmExAdvanced, Constants.EncryptCtxMenu);
            }

            // Add Items
            if (_windowsOs < WindowsVer.Windows.Vista) {
                txtAddToRightClick.Text += " " + GetResourceString("Only7AndOnwards");
                panelAddToRightClick.IsEnabled = true;
            }
            // Send To
            LoadSendTo();
        }

        private void UpdateRegistryFromRightClick() {
            // General
            using (RegistryKey hkcrContextMenuHandlers = _hkcr.CreateSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers")) {
                hkcrContextMenuHandlers.SetSubKey(chkCopyToFolder, Constants.CopyTo);
                hkcrContextMenuHandlers.SetSubKey(chkMoveToFolder, Constants.MoveTo);
                hkcrContextMenuHandlers.SetSubKey(chkSendTo, Constants.SendTo);
            }

            using (RegistryKey hkcrFileShell = _hkcr.CreateSubKey(@"*\shell")) {
                hkcrFileShell.SetSubKey(chkOpenWithNotepad, Constants.OpenNotepad);
            }

            using (RegistryKey hkcrShell = _hkcr.CreateSubKey(@"Directory\Background\shell")) {
                hkcrShell.SetSubKey(chkControlPanelInDesktopMenu, Constants.ControlPanel);
            }

            using (RegistryKey hkcrDirShell = _hkcr.CreateSubKey(@"Directory\shell")) {
                if (chkOpenCmdPrompt.HasUserInteracted()) {
                    if (chkOpenCmdPrompt.IsChecked == true) {
                        if (!hkcrDirShell.HasValueInShellCommand(Constants.OpenCmdPromptVal)) {
                            RegistryKey key = hkcrDirShell.CreateSubKey(@"Open Command Prompt here\command");
                            key.SetValue("", Constants.OpenCmdPromptVal);
                        }
                    } else {
                        hkcrDirShell.DeleteSubKeyTree(@"Open Command Prompt here", false);
                    }
                }
            }

            using (RegistryKey hkcrDriveShell = _hkcr.CreateSubKey(@"Drive\shell")) {
                if (chkOpenCmdPrompt.HasUserInteracted()) {
                    if (chkOpenCmdPrompt.IsChecked == true) {
                        if (!hkcrDriveShell.HasValueInShellCommand(Constants.OpenCmdPromptVal)) {
                            RegistryKey key = hkcrDriveShell.CreateSubKey(@"Open Command Prompt here\command");
                            key.SetValue("", Constants.OpenCmdPromptVal);
                            key.Close();
                        }
                    } else {
                        hkcrDriveShell.DeleteSubKeyTree(@"Open Command Prompt here", false);
                    }
                }
                if (chkAddDefragInMenu.IsChecked == true) {
                    RegistryKey key = hkcrDriveShell.CreateSubKey(Constants.RunAs);
                    key.SetValue("", "Defragment");
                    RegistryKey cmdKey = key.CreateSubKey(Constants.Cmd);
                    cmdKey.SetValue("", Constants.DefragVal);
                    cmdKey.Close();
                    key.Close();
                } else {
                    hkcrDriveShell.DeleteSubKeyTree(Constants.RunAs, false);
                }
            }


            using (RegistryKey hklmExAdvanced = _hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced")) {
                hklmExAdvanced.SetValue(chkEncryptAndDecrypt, Constants.EncryptCtxMenu);
            }
        }

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
            string sendToPath = _sendToTask.GetSendToFolderPath(_windowsOs);
            FileReader fileReader = new FileReader(sendToPath, new List<string>() { ".ini" });
            ObservableCollection<FileItem> sendToFileItems = fileReader.GetAllFiles();
            e.Result = sendToFileItems;
        }

        private void OnButtonAddFolderToSendToClick(object sender, RoutedEventArgs e) {
            try {
                if (_sendToTask.AddFolder(_windowsOs)) {
                    ReloadSendTo();
                }
            } catch (BadImageFormatException) {
                _message.Error(GetResourceString("SendToAddDisabled"));
                btnAddFileToSendTo.IsEnabled = btnAddFolderToSendTo.IsEnabled = false;
            } catch (FileNotFoundException) {
                _message.Error(GetResourceString("InvalidFilePath"));
            }
        }

        private void OnButtonAddFileToSendToClick(object sender, RoutedEventArgs e) {
            try {
                if (_sendToTask.AddFile(_windowsOs)) {
                    ReloadSendTo();
                }
            } catch (BadImageFormatException) {
                _message.Error(GetResourceString("SendToAddDisabled"));
                btnAddFileToSendTo.IsEnabled = btnAddFolderToSendTo.IsEnabled = false;
            } catch (FileNotFoundException) {
                _message.Error(GetResourceString("InvalidFilePath"));
            }
        }

        private void OnButtonDeleteFromSendToClick(object sender, RoutedEventArgs e) {
            _sendToTask.Delete(lstSendTo, _message);
            ReloadSendTo();
        }

        private void OnLinkEditSendToExplorerClick(object sender, RoutedEventArgs e) {
            string sendToPath = _sendToTask.GetSendToFolderPath(_windowsOs);
            try {
                Process.Start(sendToPath);
            } catch (Win32Exception) {
                _message.Error(GetResourceString("DifferentSendToPath"));
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
            if (_windowsOs < WindowsVer.Windows.Vista)
                return;
            string shrtCtName = txtShrtCtName.Text.Trim();
            if (shrtCtName.Length == 0) {
                _message.Error(GetResourceString("MissingShortcutName"));
                return;
            }
            if (shrtCtName == "cmd") {
                _message.Error(GetResourceString("CmdNotAvailable"));
            }
            string shrtCtPath = txtShrtCtPath.Text.Trim();
            if (shrtCtPath.Length == 0) {
                _message.Error(GetResourceString("ValidFilePath"));
                return;
            }
            bool result = RightClickAddDeleteTask.Add(shrtCtName, shrtCtPath);
            if (!result) {
                string msg = GetResourceString("UnableToIndentify") + " \"" + shrtCtPath + "\"" + " " + GetResourceString("AsValidFilePath") + "." +
                                           Environment.NewLine + " " + GetResourceString("ProceedAndCreateShortcut") +
                                           Environment.NewLine + " " + GetResourceString("AddedToPath");
                InfoBox infoBox = new InfoBox(msg, GetResourceString("CreateAnyways"), 
                    GetResourceString("WarningMsgTitle"), InfoBox.DialogType.Warning);
                if (infoBox.ShowDialog() == true) {
                    RightClickAddDeleteTask.AddToRegistry(shrtCtName, shrtCtPath);
                    result = true;
                }
            }
            if (!result) {
                _message.Error(GetResourceString("ValidFilePath"));
            } else {
                _message.Success(GetResourceString("SuccessfullyAdded") + " " + shrtCtName + " " + GetResourceString("ToRightClick"));
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
                _message.Error(GetResourceString("EmptyRightLick"));
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
            _message.Success(GetResourceString("SuccessfullyDeleted") + " " + fileItem.Tag + " " + GetResourceString("FromRightClick"));
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
                    _message.Success(GetResourceString("ShutdownScheduled") + " " + selectedDateTime.Value.ToString("MMMM d, yyyy ") 
                        + " " + GetResourceString("At") + " " + selectedDateTime.Value.ToString("hh:mm tt"));
                }
                else {
                    _message.Error(GetResourceString("InvalidTimeout"));
                }
            }
            else {
                _message.Error(GetResourceString("SelectDate"));
            }
        }

        private void OnCancelShutdownButtonClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteDosCmd("shutdown /a");
            _message.Success(GetResourceString("ShutdownCancelled"));
        }
        #endregion

        #region Task -> Special Hiding

        private void OnButtonHideFileFolderClick(object sender, RoutedEventArgs e) {
            switch (cmboxHideSelection.SelectedIndex) {
                case 0:
                    HideFile();
                    break;
                case 1:
                    HideFolder();
                    break;
            }
        }

        private void OnButtonUnhideFileFolderClick(object sender, RoutedEventArgs e) {
            switch (cmboxUnHideSelection.SelectedIndex) {
                case 0:
                    UnhideFile();
                    break;
                case 1:
                    UnhideFolder();
                    break;
            }
        }

        private void HideFolder() {
            string folderName = Utils.GetUserSelectedFolder();
            if (folderName == null) return;
            string cmd = Utils.GetHideCmd(folderName);
            ProcessWrapper.ExecuteDosCmd(cmd);
            _message.Success(GetResourceString("SuccessfullyHidden") + " " + folderName + " " + GetResourceString("WithSysAttribs"));
        }

        private void HideFile() {
            string[] fileList = Utils.GetUserSelectedFilePathList("All files|*.*");
            if (fileList == null) return;
            foreach (string cmd in fileList.Select(fileName => Utils.GetHideCmd(fileName))) {
                ProcessWrapper.ExecuteDosCmd(cmd);
            }
            _message.Success(GetResourceString("SuccessfullyHidden") + " " +
                fileList.Select(Path.GetFileName).ToList().SentenceJoin(GetResourceString("And")) + " " + GetResourceString("WithSysAttribs"));
        }

        private void UnhideFolder() {
            string folderName = Utils.GetUserSelectedFolder();
            if (folderName == null) return;
            string cmd = Utils.GetUnhideCmd(folderName);
            ProcessWrapper.ExecuteDosCmd(cmd);
            _message.Success(folderName + " " + GetResourceString("Is") + " " + GetResourceString("NowVisible"));
        }

        private void UnhideFile() {
            string[] fileList = Utils.GetUserSelectedFilePathList("All files|*.*");
            if (fileList == null) return;
            foreach (string cmd in fileList.Select(fileName => Utils.GetUnhideCmd(fileName))) {
                ProcessWrapper.ExecuteDosCmd(cmd);
            }
            List<string> fileNameList = fileList.Select(Path.GetFileName).ToList();
            string verb = fileNameList.Count > 1 ? GetResourceString("Are") : GetResourceString("Is");
            _message.Success(fileNameList.SentenceJoin(GetResourceString("And")) + " " + verb + " " + GetResourceString("NowVisible"));
        }

        #endregion

        #region Task -> Special Folder
        private void OnButtonBrowseSpecialFolderParentClick(object sender, RoutedEventArgs e) {
            WPFFolderBrowserDialog folderBrowserDlg = new WPFFolderBrowserDialog();
            folderBrowserDlg.Title = GetResourceString("SelectParentFolder");
            if (folderBrowserDlg.ShowDialog() == true) {
                string parentPath = folderBrowserDlg.FileName;
                string createCmd = String.Format("md \"\\\\.\\{0}\\{1}\"", parentPath, cmboBxSpecialFolderNames.SelectionBoxItem);
                ProcessWrapper.ExecuteDosCmd(createCmd);
                _message.Success(cmboBxSpecialFolderNames.SelectionBoxItem + " " + GetResourceString("CreatedSuccessFullyAt") + " " + parentPath);
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

        #region Task -> Create File
        private void OnCreateFileButtonClick(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog() {
                Filter = "All files|*.*"
            };
            if (saveFileDialog.ShowDialog() != true) return;
            string filePath = saveFileDialog.FileName;
            if (filePath == null || String.IsNullOrEmpty(filePath)) return;
            KeyValuePair<String, long> sizeKeyValuePair = (KeyValuePair<String, long>) cmboBxFileSizes.SelectedItem;
            CreateFileTask.Create(filePath, sizeKeyValuePair.Value, _message, this);
        }
        #endregion

        #region Features
        private void LoadFeaturesTab() {
            // System Beep
            using (RegistryKey hkcuSound = _hkcu.CreateSubKey(@"Control Panel\Sound")) {
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

            using (RegistryKey hkcuExplorer = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer")) {
                // Windows DVD Burner
                chkWinDvdBurner.SetCheckedState(hkcuExplorer, Constants.NoDvdBurning, true, true);
                
                // AutoPlay
                RegistryKey hkcuAutoplayHandlers = _hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers");
                RegistryKey hklmCdRom = _hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\cdrom");

                int? driveAutoRunVal, disableAutoPlayVal, autoRunVal;
                try {
                    driveAutoRunVal = (int?) hkcuExplorer.GetValue(Constants.NoDriveAutoPlay);
                }
                catch (InvalidCastException) {
                    driveAutoRunVal = Utils.RepairAsNullableIntFromString(hkcuExplorer, Constants.NoDriveAutoPlay);
                }
                try {
                    disableAutoPlayVal = (int?) hkcuAutoplayHandlers.GetValue(Constants.DisableAutoplay);
                }
                catch (InvalidCastException) {
                    disableAutoPlayVal = Utils.RepairAsNullableIntFromString(hkcuAutoplayHandlers, Constants.DisableAutoplay);
                }
                try {
                    autoRunVal = (int?) hklmCdRom.GetValue(Constants.AutoRun);
                }
                catch (InvalidCastException) {
                    autoRunVal = Utils.RepairAsNullableIntFromString(hklmCdRom, Constants.AutoRun);
                }
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
            _message.Success(GetResourceString("ActivatedAdminAcc"));
        }

        private void OnLinkDeactivateAdminAccountClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteProcess("net", "user administrator /active:no");
            _message.Success(GetResourceString("DeactivatedAdminAcc"));
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

        private void OnConfigureAutoplayViaGrpPolicyBtnClick(object sender, RoutedEventArgs e) {
            ProcessWrapper.ExecuteProcess("gpedit.msc");
        }

        private void OnShowHelpToConfigAutoplayMouseDown(object sender, MouseButtonEventArgs e) {
            // Unable to figure out how to put new-lines in XAML, so doing the ugly way
            String msg;
            ConfigHandler.Language language = ConfigHandler.GetCurrentLanguage();
            switch (language) {
                case ConfigHandler.Language.German:
                    msg = "Klicken Sie auf den \"Configure Autoplay über die Gruppenrichtlinien -Editor\" klicken." +
                          "\nJetzt, in der linken Seitenleiste navigieren Sie zu:" +
                          "\nBenutzerkonfiguration-> Administrative Vorlagen-> Windows-Komponenten-> Autoplay-Richtlinien." +
                          "\n\nNun überprüfen Sie die verfügbaren Optionen in der rechten Hand." +
                          "\n\nBitte beachten Sie, dass Gruppenrichtlinien nicht in Windows 7 Home Premium / Basic / Starter Edition und " +
                          "Windows Vista Home Basic Edition verfügbar.";
                    break;
                case ConfigHandler.Language.Russian:
                    msg = "Нажмите на эту кнопку \"Настроить автозапуск через редакторе групповой политики\"." +
                          "\nТеперь, в левой боковой панели перейдите к:" +
                          "\nКонфигурация пользователя-> Административные шаблоны-> Компоненты Windows-> Политика Autoplay." +
                          "\n\nТеперь проверьте опции, доступные в правой части клавиатуры." +
                          "\n\nПожалуйста, обратите внимание, что групповая политика не доступна в Windows 7 Home Premium / Basic /" +
                          " Starter Edition и в Windows Vista Home Basic Edition.";
                    break;
                default:
                    msg = "Click on this \"Configure Autoplay via Group Policy Editor\" button." +
                          "\nNow, in the left sidebar navigate to:" +
                          "\nUser Configuration->Administrative Templates->Windows Components->Autoplay Policies" +
                          "\n\nNow check out the options available in the right hand area." +
                          "\n\nPlease note that Group Policy is NOT available in Windows 7 Home Premium/Basic/Starter Edition(s) and" +
                          " in Windows Vista Home Basic Edition(s).";
                    break;
            }
            InfoBox infoBox = new InfoBox(msg, GetResourceString("Ok"), GetResourceString("Help"), InfoBox.DialogType.Information);
            infoBox.ShowDialog();
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

        private void OnSearchTextBoxKeyDown(object sender, KeyEventArgs e) {
            if ((e.Key == Key.Down || e.Key == Key.Tab) && popupSearch.IsOpen) {
                lstSearchResults.Focus();
                e.Handled = true;
            }
        }

        private void OnFocusSearchCmdExecuted(object sender, ExecutedRoutedEventArgs e) {
            stxtSearchInput.Focus();
        }

        private void OnSearchEvent(object sender, RoutedEventArgs e) {
            if (_searchBackgroundWorker.IsBusy) {
                _searchBackgroundWorker.CancelAsync();
                return;
            }
            Search(stxtSearchInput.Text);
        }

        private void Search(string searchTxt) {
            if (String.IsNullOrEmpty(searchTxt)) return;
            searchTxt = searchTxt.Trim();
            if (searchTxt.Length < 3) {
                _message.Error(GetResourceString("Min3Chars"));
                return;
            }
            if (searchTxt.Length > 100) {
                searchTxt = searchTxt.Substring(0, 100);
            }
            _message.Notify(GetResourceString("Searching"));
            _searchBackgroundWorker.RunWorkerAsync(searchTxt);
        }

        private void OnSearchWorkerStarted(object sender, DoWorkEventArgs e) {
            string searchTxt = e.Argument.ToString();
            List<SearchItem> searchItems = ShowSearchResults(searchTxt);
            e.Result = new Dictionary<string, object>() {
                {"result", searchItems},
                {"term", searchTxt}
            };
        }

        private void HideSearchResults() {
            popupSearch.IsOpen = false;
            lstSearchResults.ItemsSource = null;
            stxtSearchInput.Text = "";
            _message.Hide();
        }

        private void OnSearchWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) return;
            Dictionary<string, object> retDict = e.Result as Dictionary<string, object>;
            if (retDict == null) return;
            List<SearchItem> searchItems = retDict["result"] as List<SearchItem>;
            if (searchItems == null || !searchItems.Any()) {
                _message.Error(GetResourceString("NothingStart") + " " + retDict["term"] + " " + GetResourceString("NothingEnd"));
                return;
            }
            _message.Success(GetResourceString("ShowingResults") + " " + retDict["term"]);
            popupSearch.IsOpen = true;
            lstSearchResults.ItemsSource = searchItems;
        }

        private List<SearchItem> ShowSearchResults(string searchTxt) {
            List<SearchItem> searchItems = _searcher.Search(searchTxt);
            return searchItems;
        }

        private void OnSearchListItemKeyDown(object sender, KeyEventArgs e) {
            if (!(e.Key == Key.Enter || e.Key == Key.Return)) return;
            ListBoxItem listBoxItem = (ListBoxItem)sender;
            SearchItem searchItem = listBoxItem.Content as SearchItem;
            if (searchItem == null) return;
            NavigateTo(searchItem);
        }

        private void OnSearchListItemMouseDown(object sender, MouseButtonEventArgs e) {
            ListBoxItem listBoxItem = (ListBoxItem) sender;
            SearchItem searchItem = listBoxItem.Content as SearchItem;
            if (searchItem == null) return;
            NavigateTo(searchItem);
        }

        private void NavigateTo(SearchItem searchItem) {
            object mainTabItemObj = this.FindName(searchItem.MainTab);
            object subTabControlObj = this.FindName(searchItem.SubTabControl);
            object subTabItemObj = this.FindName(searchItem.SubTab);
            if (mainTabItemObj == null || subTabControlObj == null || subTabItemObj == null) return;
            TabItem mainTabItem = (TabItem) mainTabItemObj;
            string tagVal = mainTabItem.Tag as string;
            if (!String.IsNullOrEmpty(tagVal)) {
                LoadTab(tagVal);
            }
            mainTab.SelectedItem = mainTabItem;
            TabControl subTabControl = (TabControl) subTabControlObj;
            TabItem subTabItem = (TabItem) subTabItemObj;
            subTabControl.SelectedItem = subTabItem;
            HideSearchResults();
        }

        private void OnSearchDismiss(object sender, RoutedEventArgs e) {
            HideSearchResults();
        }

        #endregion

        #region Utilities

        private void StartProcess(object sender, RoutedEventArgs e) {
            try {
                string tagVal = ((Hyperlink)sender).Tag.ToString();
                if (tagVal.Contains(";")) {
                    string[] tagArr = tagVal.Split(';');
                    ProcessWrapper.ExecuteProcess(tagArr[0], tagArr[1]);
                } else {
                    ProcessWrapper.ExecuteProcess(tagVal);
                }
            } catch (Win32Exception) {
                _message.Error(GetResourceString("OptionNotAvailable"));
            }
        }

        private void RunAsDosCmd(object sender, RoutedEventArgs e) {
            try {
                string tagVal = ((Hyperlink)sender).Tag.ToString();
                ProcessWrapper.ExecuteDosCmd(tagVal);
            } catch (Win32Exception) {
                _message.Error(GetResourceString("OptionNotAvailable"));
            }
        }
        #endregion

        #region Menu
        private void UpdateLanguageMenu() {
            ConfigHandler.Language language = ConfigHandler.GetCurrentLanguage();
            switch (language) {
                case ConfigHandler.Language.English:
                    SetLanguageMenuState(true, false, false);
                    break;
                case ConfigHandler.Language.German:
                    SetLanguageMenuState(false, true, false);
                    break;
                case ConfigHandler.Language.Russian:
                    SetLanguageMenuState(false, false, true);
                    break;
                default:
                    SetLanguageMenuState(true, false, false);
                    break;
            }
        }

        private void SetLanguageMenuState(bool englishMenuState, bool germanMenuState, bool russianMenuState) {
            menuItemEnglish.IsChecked = englishMenuState;
            menuItemGerman.IsChecked = germanMenuState;
            menuItemRussian.IsChecked = russianMenuState;
        }

        private void OnLanguageMenuItemClick(object sender, RoutedEventArgs e) {
            MenuItem languageMenuItem = sender as MenuItem;
            if (languageMenuItem == null)
                return;
            string cultureName = languageMenuItem.Tag.ToString();
            if (cultureName == ConfigHandler.ToLanguageString(ConfigHandler.GetCurrentLanguage())) {
                // Selected language is same as current language, hence do nothing
                languageMenuItem.IsChecked = true;
                return;
            }
            ConfigHandler.SetCulture(cultureName);
            string msg = GetResourceString("RestartForLangChange");
            InfoBox infoBox = new InfoBox(msg, GetResourceString("CloseNLaunch"), GetResourceString("Success"),
                InfoBox.DialogType.Information);
            infoBox.HideCancelButton();
            if (infoBox.ShowDialog() == true) {
                Environment.Exit(0);
            }
        }
        
        private void OnAboutMenuItemClick(object sender, RoutedEventArgs e) {
            About about = new About();
            about.ShowDialog();
        }

        private void OnContactMenuClick(object sender, RoutedEventArgs e) {
            if (String.IsNullOrEmpty(Keys.ContactUrl)) return;
            Process.Start(Keys.ContactUrl);
        }

        private void OnCreateRestorePtMenuClick(object sender, RoutedEventArgs e) {
            try {
                ProcessWrapper.ExecuteProcess("SystemPropertiesProtection.exe");
            }
            catch (Win32Exception) {
                ProcessWrapper.ExecuteProcess(Environment.GetFolderPath(Environment.SpecialFolder.Windows)
                                                + @"\system32\Restore\rstrui.exe");
            }
        }
        #endregion

        #region Update Check

        private void CheckForUpdate() {
            Config config = ConfigHandler.GetConfig();
            SetMenuItemsCheckedState();
            long lastUpdateChkVal = config.LastUpdateChk == null ? 0 : config.LastUpdateChk.Value;
            if (config.UpdateMethod == UpdateCheckTask.Auto && UpdateCheckTask.IsTimeToCheck(lastUpdateChkVal)) {
                ConfigHandler.SetLastUpdateChkVal();
                StartUpdateCheck(true);
            }
        }

        private void StartUpdateCheck(bool failSilently) {
            if (String.IsNullOrEmpty(Keys.UpdateApiUrl) || _updateCheckBackgroundWorker.IsBusy) return;
            _updateCheckBackgroundWorker.RunWorkerAsync(failSilently);
        }

        private TweakerUpdate GetLatestVersionInfo() {
            String response = UpdateCheckTask.GetUpdateInfoFile();
            if (String.IsNullOrEmpty(response)) return null;
            TweakerUpdate tweakerUpdate = UpdateCheckTask.ReadTweakerUpdateInfo(response);
            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            try {
                Version latestVersion = new Version(tweakerUpdate.Version);
                if (applicationVersion.CompareTo(latestVersion) < 0) {
                    return tweakerUpdate;
                }
            } catch (ArgumentException) { } catch (FormatException) { } catch (OverflowException) { }
            return null;
        }

        private void OnUpdateCheckBackgroundWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Dictionary<string, object> retDict = e.Result as Dictionary<string, object>;
            if (retDict == null) return;
            TweakerUpdate tweakerUpdate = retDict["result"] as TweakerUpdate;
            if (tweakerUpdate == null) {
                bool failSilently = (bool) retDict["failSilently"];
                if (!failSilently) {
                    _message.Success(GetResourceString("AlrdyLatest"));
                }
                return;
            }
            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string msg = GetResourceString("NewVersionAvailable") + " " + tweakerUpdate.Version + " " + GetResourceString("IsAvailableForDw") + " " +
                         "(" + GetResourceString("CurrentVersion") + " " + applicationVersion +").";
            InfoBox infoBox = new InfoBox(msg, GetResourceString("Download"), GetResourceString("UpdateAvailable"),
                InfoBox.DialogType.Information);
            if (infoBox.ShowDialog() == true) {
                Process.Start(tweakerUpdate.Url);
            }
        }

        private void OnUpdateWorkerStarted(object sender, DoWorkEventArgs e) {
            TweakerUpdate result = GetLatestVersionInfo();
            bool failSilently = (bool) e.Argument;
            e.Result = new Dictionary<string, object> {
                {"result", result},
                {"failSilently", failSilently}
            };
        }

        private void OnAutomaticUpdateMethodMenuClick(object sender, RoutedEventArgs e) {
            ConfigHandler.SetUpdateChkMethod(UpdateCheckTask.Auto);
            SetMenuItemsCheckedState();
        }

        private void OnManualUpdateMethodMenuClick(object sender, RoutedEventArgs e) {
            ConfigHandler.SetUpdateChkMethod(UpdateCheckTask.Manual);
            SetMenuItemsCheckedState();
        }

        private void SetMenuItemsCheckedState() {
            Config config = ConfigHandler.GetConfig();
            if (config.UpdateMethod == UpdateCheckTask.Auto) {
                menuItemAutoUpdate.IsChecked = true;
                menuItemManualUpdate.IsChecked = false;
            } else {
                menuItemAutoUpdate.IsChecked = false;
                menuItemManualUpdate.IsChecked = true;
            }
        }

        private void OnUpdateCheckMenuClick(object sender, RoutedEventArgs e) {
            StartUpdateCheck(false);
        }
        #endregion

    }
}