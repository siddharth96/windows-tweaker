using System;

namespace WindowsTweaker
{
    internal static class Constants
    {
        public const String Explorer = "explorer";
        public const String System = "system";
        public const String Display = "display";
        public const String RightClick = "rightclick";
        public const String Places = "places";
        public const String Tasks = "tasks";
        public const String Features = "features";
        public const String Logon = "logon";
        public const String Restrictions = "restrictions";
        public const String Maintenance = "maintenance";
        public const String Utilities = "utilities";
        public const String ErrorMsgTitle = "On My God!!";
        public const String WarningMsgTitle = "Hey!!";
        public const String AlertMsgTitle = "Mind You!!";
        public const String SuccessMsgTitle = "Mission Accomplished!!";
        public const byte HasUserInteracted = 1;

        // Explorer Tab Keys/Value

        // Drive Letter Display
        public const String ShowDriveLetters = "ShowDriveLettersFirst";

        // Advanced
        public const String AlwaysShowMenu = "AlwaysShowMenus";

        public const String HideFileExt = "HideFileExt";
        public const String AutoChkSelect = "AutoCheckSelect";
        public const String Hidden = "Hidden";
        public const String PersistBrowsers = "PersistBrowsers";
        public const String SuperHidden = "ShowSuperHidden";
        public const String CompressedColor = "ShowCompColor";

        // Property Dialog
        public const String NoSecurityTab = "NoSecurityTab";

        public const String NoCustomizeTab = "NoCustomizeThisFolder";

        // Libraries
        public const String Library = "{031E4825-7B94-4dc3-B131-E946B44C8DD5}";

        //etc
        public const String RecycleBin = "{645FF040-5081-101B-9F08-00AA002F954E}";

        public const String Regedit = "{77708248-f839-436b-8919-527c410f48b9}";

        // System Tab Key/Values

        // Shutdown Configuration
        public const String AutoEndTasks = "AutoEndTasks";

        public const String WaitToKillAppTimeout = "WaitToKillAppTimeout";

        // Windows Installer
        public const String DisableMsi = "DisableMSI";

        public const String AlwaysInstallElevated = "AlwaysInstallElevated";
        public const String LimitSystemRestore = "LimitSystemRestoreCheckpointing";

        // Registration
        public const String RegisteredOwner = "RegisteredOwner";

        public const String RegisteredOrg = "RegisteredOrganization";
        public const String ProductId = "ProductId";

        // Information
        public const String Manufacturer = "Manufacturer";

        public const String Model = "Model";
        public const String SupportPhone = "SupportPhone";
        public const String SupportUrl = "SupportURL";

        // Logo
        public const String Logo = "Logo";

        // Display Tab Key/Values

        // Display Settings
        public const String MinAnimate = "MinAnimate";

        public const String DragFullWin = "DragFullWindows";
        public const String PaintDesktopVer = "PaintDesktopVersion";

        // ALT Tab
        public const String SwitchCols = "CoolSwitchColumns";

        public const String SwitchRows = "CoolSwitchRows";

        // Selection Color
        public const String SelectionColor = "HotTrackingColor";

        // Right Click Keys/Values

        // General
        public const String CopyToId = "{C2FBB630-2971-11D1-A18C-00C04FD75D13}";

        public const String MoveToId = "{C2FBB631-2971-11D1-A18C-00C04FD75D13}";
        public const String SendToId = "{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
        public const String CopyTo = "Copy To";
        public const String MoveTo = "Move To";
        public const String SendTo = "SendTo";
        public const String OpenNotepad = "Open With Notepad";
        public const String ControlPanel = "Control Panel";
        public const String OpenCmd = "Open Command Prompt here";
        public const String RunAs = "runas";
        public const String EncryptCtxMenu = "EncryptionContextMenu";
        public const String CopyContents = "copycontents";
        public const String Shell = "shell";
        public const String Open = "Open";
        public const String Edit = "edit";
        public const String Read = "Read";
        public const String Cmd = "command";
        public const String TextFile = "textfile";
        public const String ControlPanelCreatePath = "rundll32.exe shell32.dll,Control_RunDLL";

        //Restrictions

        //Explorer
        public const String NoFileMenu = "NoFileMenu";
        public const String NoFolderOption = "NoFolderOptions";
        public const String NoViewContextMenu="NoViewContextMenu";
        public const String NoDispAppearancePage="NoDispAppearancePage";
        public const String NoDispScrSavPage = "NoDispScrSavPage";
        public const String DisableThumbnailCache="DisableThumbnailCache";
        public const String DisAllowFlip_3D= "DisallowFlip3D";
        //Start Menu
        public const String NoClose="NoClose";
        public const String NoRecentDocsMenu="NoRecentDocsMenu";
        public const String NoChangeStartMenu="NoChangeStartMenu";
        public const String NoLogOff="NoLogoff";
        public const String NoDispCpl="NoDispCPL";
        //System
        public const String NoDeletePrinter="NoDeletePrinter";
        public const String NoAddPrinter="NoAddPrinter";
        public const String NoWindowUpdate="NoWindowsUpdate";
        public const String NoVirtMemPage="NoVirtMemPage";
        public const String DisbaleRegistryTools="DisableRegistryTools";
        public const String DisableTaskMgr="DisableTaskMgr";
        public const String NoPropertiesMyComputer="NoPropertiesMyComputer";
        //Special case 
        public const String AutoShreWks = "AutoShareWks";

        //Taskbar
        public const String TaskBarAnimations = "TaskbarAnimations";
        public const String TaskBarSmallIcons="TaskbarSmallIcons";


        // Places
        // God Mode
        public const String GodModeKey = ".{ED7BA470-8E54-465E-825C-99712043E01C}";
        // Power Button
        public const String StartPowerBtnAction = "Start_PowerButtonAction";
        // Open With
        public const String NoOpenWith = "NoOpenWith";
    }
}