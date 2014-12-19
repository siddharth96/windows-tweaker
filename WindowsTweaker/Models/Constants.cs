using WindowsTweaker.AppTasks;

namespace WindowsTweaker.Models {
    internal static class Constants {
        public const string Explorer = "explorer";
        public const string System = "system";
        public const string Display = "display";
        public const string RightClick = "rightclick";
        public const string Places = "places";
        public const string Tasks = "tasks";
        public const string Features = "features";
        public const string Logon = "logon";
        public const string Restrictions = "restrictions";
        public const string Maintenance = "maintenance";
        public const string Utilities = "utilities";
        public const byte HasUserInteracted = 1;
        public const string InternetExplorer = "iexplore.exe";
        public const string ConfigDirectoryName = "Windows Tweaker";
        public const string ConfigFileName = "WindowsTweaker.config.xml";
        public const string EnglishSearchInputFileName = "WindowsTweaker.Search.englishTermAndUiElementMap.json";
        public const string GermanSearchInputFileName = "WindowsTweaker.Search.germanTermAndUiElementMap.json";
        public const string RussianSearchInputFileName = "WindowsTweaker.Search.russianTermAndUiElementMap.json";
        public const string BlankIconResourceName = "WindowsTweaker.Images.blank.ico";

        // Explorer Tab Keys/Value
        // Drive Letter Display
        public const string ShowDriveLetters = "ShowDriveLettersFirst";
        // Advanced
        public const string AlwaysShowMenu = "AlwaysShowMenus";
        public const string HideFileExt = "HideFileExt";
        public const string AutoChkSelect = "AutoCheckSelect";
        public const string Hidden = "Hidden";
        public const string PersistBrowsers = "PersistBrowsers";
        public const string SuperHidden = "ShowSuperHidden";
        public const string CompressedColor = "ShowCompColor";
        public const string NavPaneExpandToCurrentFolder = "NavPaneExpandToCurrentFolder";
        public const string NoNewAppAlert = "NoNewAppAlert";
        // Property Dialog
        public const string NoSecurityTab = "NoSecurityTab";
        public const string NoCustomizeTab = "NoCustomizeThisFolder";
        // Libraries
        public const string Library = "{031E4825-7B94-4dc3-B131-E946B44C8DD5}";
        // Start Screen
        public const string DesktopBkgOnStart = "MotionAccentId_v1.00";
        public const string OpenDesktop = "OpenAtLogon";
        public const string MonitorOverride = "MonitorOverride";
        public const string MakeAllAppsDefault = "MakeAllAppsDefault";
        public const string GlobalSearchInApps = "GlobalSearchInApps";
        public const string DesktopFirst = "DesktopFirst";
        // Navigation
        public const string DisableTrCorner = "DisableTRCorner";
        public const string DisableTlCorner = "DisableTLCorner";
        public const string DontUsePowerShellOnWinX = "DontUsePowerShellOnWinX";
        // Etc
        public const string RecycleBin = "{645FF040-5081-101B-9F08-00AA002F954E}";
        public const string Regedit = "{77708248-f839-436b-8919-527c410f48b9}";

        // System Tab Key/Values
        // Shutdown Configuration
        public const string AutoEndTasks = "AutoEndTasks";
        public const string WaitToKillAppTimeout = "WaitToKillAppTimeout";
        // Windows Installer
        public const string DisableMsi = "DisableMSI";
        public const string AlwaysInstallElevated = "AlwaysInstallElevated";
        public const string LimitSystemRestore = "LimitSystemRestoreCheckpointing";
        // Registration
        public const string RegisteredOwner = "RegisteredOwner";
        public const string RegisteredOrg = "RegisteredOrganization";
        public const string ProductId = "ProductId";
        // Information
        public const string Manufacturer = "Manufacturer";
        public const string Model = "Model";
        public const string SupportPhone = "SupportPhone";
        public const string SupportUrl = "SupportURL";
        // Logo
        public const string Logo = "Logo";

        // Display Tab Key/Values
        // Display Settings
        public const string MinAnimate = "MinAnimate";
        public const string DragFullWin = "DragFullWindows";
        public const string PaintDesktopVer = "PaintDesktopVersion";
        public const string BorderWidth = "BorderWidth";
        public const string DefaultWinBorder = "-15";
        public const int MinBorderVal = -750;
        public const int MaxBorderVal = 0;
        public const string IconSpacing = "IconSpacing";
        public const string IconVerticalSpacing = "IconVerticalSpacing";
        public static readonly string DefaultIconSpacing = WindowsVer.Instance.GetName() >= WindowsVer.Windows.Seven ? "-1125" : "-75";
        public const int MinIconSpacingVal = -2730;
        public const int MaxIconSpacingVal = -480;
        // Explorer
        public const string SharedFolderIcon = "ntshrui.dll";
        public const string OldStyleFileSort = "NoStrCmpLogical";
        // Icon Title
        public const string IconTitleWrap = "IconTitleWrap";
        // ALT Tab
        public const string SwitchCols = "CoolSwitchColumns";
        public const string SwitchRows = "CoolSwitchRows";
        // Selection Color
        public const string SelectionColor = "HotTrackingColor";
        // Shortcut Arrow
        public const string ShortcutArrowRegistryKey = "29";

        // Right Click Keys/Values
        // General
        public const string CopyToId = "{C2FBB630-2971-11D1-A18C-00C04FD75D13}";
        public const string MoveToId = "{C2FBB631-2971-11D1-A18C-00C04FD75D13}";
        public const string SendToId = "{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
        public const string CopyTo = "Copy To";
        public const string MoveTo = "Move To";
        public const string SendTo = "SendTo";
        public const string OpenNotepad = "Open With Notepad";
        public const string ControlPanel = "Control Panel";
        public const string OpenCmdPromptVal = "cmd.exe /k cd %1";
        public const string RunAs = "runas";
        public const string EncryptCtxMenu = "EncryptionContextMenu";
        public const string Shell = "shell";
        public const string Open = "Open";
        public const string Edit = "edit";
        public const string Read = "Read";
        public const string Cmd = "command";
        public const string TextFile = "textfile";
        public const string ControlPanelCreatePath = "rundll32.exe shell32.dll,Control_RunDLL";
        public const string DefragVal = "defrag %1 -v";

        //Restrictions Tab Key/Values
        //Explorer
        public const string NoFileMenu = "NoFileMenu";
        public const string NoFolderOption = "NoFolderOptions";
        public const string NoViewContextMenu = "NoViewContextMenu";
        public const string NoTrayContextMenu = "NoTrayContextMenu";
        public const string NoDispAppearancePage = "NoDispAppearancePage";
        public const string NoDispScrSavPage = "NoDispScrSavPage";
        public const string DisableThumbnailCache = "DisableThumbnailCache";
        public const string DisAllowFlip_3D = "DisallowFlip3D";
        public const string NoWinKeys = "NoWinKeys";
        public const string DisablePasswordReveal = "DisablePasswordReveal";
        //Start Menu
        public const string NoClose = "NoClose";
        public const string NoRecentDocsMenu = "NoRecentDocsMenu";
        public const string NoChangeStartMenu = "NoChangeStartMenu";
        public const string NoLogOff = "NoLogoff";
        public const string NoDispCpl = "NoDispCPL";
        //System
        public const string NoDeletePrinter = "NoDeletePrinter";
        public const string NoAddPrinter = "NoAddPrinter";
        public const string NoWindowUpdate = "NoWindowsUpdate";
        public const string NoVirtMemPage = "NoVirtMemPage";
        public const string DisbaleRegistryTools = "DisableRegistryTools";
        public const string DisableTaskMgr = "DisableTaskMgr";
        public const string NoPropertiesMyComputer = "NoPropertiesMyComputer";
        //Special case 
        public const string AutoShreWks = "AutoShareWks";
        //Taskbar
        public const string TaskBarAnimations = "TaskbarAnimations";
        public const string TaskBarSmallIcons = "TaskbarSmallIcons";
        public const string ShowInfoTip = "ShowInfoTip";


        // Places Tab Key/Values
        // God Mode
        public const string GodModeKey = ".{ED7BA470-8E54-465E-825C-99712043E01C}";
        // Power Button
        public const string StartPowerBtnAction = "Start_PowerButtonAction";
        // Open With
        public const string NoOpenWith = "NoOpenWith";

        // Features Tab Key/Values
        // AutoPlay
        public const string NoDriveAutoPlay = "NoDriveTypeAutoRun";
        public const string DisableAutoplay = "DisableAutoplay";
        public const string AutoRun = "AutoRun";
        // Windows Update
        public const string AutoUpdateOptions = "AUOptions";
        public const string IncludeRecommendedUpdates = "IncludeRecommendedUpdates";
        public const string ElevateNonAdmins = "ElevateNonAdmins";
        public const string EnableFeaturedSoftware = "EnableFeaturedSoftware";
        public const string DefaultWindowsUpdateText = "Please choose an option:";
        // System Beep
        public const string Beep = "Beep";
        public const string Yes = "yes";
        public const string No = "no";
        // Windows DVD Burner
        public const string NoDvdBurning = "NoCDBurning";

        // Logon Tab Key/Values
        // Automatic Logon
        public const string AutoAdminLogon = "AutoAdminLogon";
        public const string DefaultUserName = "DefaultUserName";
        public const string DefaultPassword = "DefaultPassword";
        public const string DefaultDomainName = "DefaultDomainName";
        public const string IgnoreShiftOverride = "IgnoreShiftOverride";
        public const string ForceAutoLogon = "ForceAutoLogon";
        // Startup Sound
        public const string DisableStartupSound = "DisableStartupSound";
        // Screensaver Lock
        public const string ScreenSaverIsSecure = "ScreenSaverIsSecure";
        // Miscellaneous
        public const string DisableCtrlAltDlt = "DisableCAD";
        public const string NoLastUserName = "dontdisplaylastusername";
        public const string ShutdownWithoutLogon = "shutdownwithoutlogon";
        // Login Message
        public const string LoginMsgTitle = "LegalNoticeCaption";
        public const string LoginMsgContent = "LegalNoticeText";

        // Maintenance Tab Key/Values
        // Memory
        public const string AlwaysUnloadDll = "AlwaysUnloadDLL";
        public const string MachineGroupPolicy = "SynchronousMachineGroupPolicy";
        public const string UserGroupPolicy = "SynchronousUserGroupPolicy";
        // Auto Reboot
        public const string LogEvent = "LogEvent";
        public const string AutoReboot = "AutoReboot";
        // Startup Settings
        public const string ReportBootOk = "ReportBootOk";
        public const string Enable = "Enable";
        // Error
        public const string DontShowUi = "DontShowUI";
        public const string DefaultConsent = "DefaultConsent";
        public const string Disabled = "Disabled";

        public const string HungAppTimeout = "HungAppTimeout";
        public const string LinkResolveIgnoreLinkInfo = "LinkResolveIgnoreLinkInfo";
        public const string NoResolveSearch = "NoResolveSearch";
        public const string NoResolveTrack = "NoResolveTrack";
        public const string NoChangingLockScreen = "NoChangingLockScreen";
        public const string NoScreenLock = "NoScreenLock";
    }
}