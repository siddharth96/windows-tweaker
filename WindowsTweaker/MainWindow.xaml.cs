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
using System.Diagnostics;
using System.IO;
using IWshRuntimeLibrary;
using WPFFolderBrowser;
using System.ComponentModel;
using System.Xml;

namespace WindowsTweaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegistryKey hklm = Registry.LocalMachine;
        RegistryKey hklmSystem;
        RegistryKey hklmCVWinNT;
        RegistryKey hklmWinLogon;
        RegistryKey hklmParams;
        RegistryKey hklmPWindows;
        RegistryKey hklmInstaller;
        RegistryKey hklmExplorer;
        RegistryKey hklmClasses;
        RegistryKey hklmDotTxt;
        RegistryKey hklmTextFile;
        RegistryKey hklmOEM;
        RegistryKey hklmAutoUpdate;
        RegistryKey hklmCVExplorer;
        RegistryKey hklmExAdvanced;
        RegistryKey hklmTxt;
        RegistryKey hklmCdRom;
        RegistryKey hklmNamespaces;
        RegistryKey hklmCrashControl;
        RegistryKey hklmBootAnimation;
        RegistryKey hklmBootOptFunc;
        RegistryKey hklmSSMgr;
        RegistryKey hklmNamespace;
        RegistryKey hklmCPNamespace;
        
        RegistryKey hkcu = Registry.CurrentUser;
        RegistryKey hkcuSound;
        RegistryKey hkcuDesktop;
        RegistryKey hkcuWinMet;
        RegistryKey hkcuExplorer;
        RegistryKey hkcuSystem;
        RegistryKey hkcuCVExplorer;
        RegistryKey hkcuExAdvanced;
        RegistryKey hkcuDWM;
        RegistryKey hkcuAutoplayHandlers;
        RegistryKey hkcuColors;
        RegistryKey hkcuPDesktop;
        RegistryKey hkcuWinErrReporting;
        RegistryKey hkcuConsent;
        RegistryKey hklmShellIcons;

        RegistryKey hkcr = Registry.ClassesRoot;
        RegistryKey hkcrContextMenuHandlers;
        RegistryKey hkcrUnknown;
        RegistryKey hkcrNoEnd;
        RegistryKey hkcrCPcommand;
        RegistryKey hkcrShell;
        RegistryKey hkcrOpenWithNotepad;
        RegistryKey hkcrFileShell;
        RegistryKey hkcrCLSID;
        RegistryKey hkcrDirShell;
        RegistryKey hkcrDriveShell;

        OpenFileDialog openFileDialog = new OpenFileDialog();
        WPFFolderBrowserDialog folderBrowserDialog = new WPFFolderBrowserDialog();
        Color selectionColor;
        ColorPickerDialog colorDialog;

        const int tabCount=11;
        bool[] tabOpened = new bool[tabCount];
        bool hklmInstaller_Del = false;
        bool btnEnabled = false;
        bool is64bitMachine = IntPtr.Size == 8 ? true : false;
        int[] comboLstEvntRec = new int[noOfComboBxs];
        const int noOfTxtBxs = 14-1;//-1 Indicates absence of Passwordbox as it doesn't supports TextChangedEvent
        int[] varTbEventRecord = new int[noOfTxtBxs];
        const int noOfComboBxs = 4;
        const int noOfNumUpDown = 4;
        int[] varNUDEventRecord = new int[noOfNumUpDown];
        enum WindowsOS { WindowsXP, WindowsVista, Windows7, Windows8 };
        WindowsOS osName;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                for (int i = 0; i < tabCount; i++) tabOpened[i] = false;
                for (int i = 0; i < noOfComboBxs; i++)
                    comboLstEvntRec[i] = 0;
                Version ver = Environment.OSVersion.Version;
                if (ver.Major == 5 && ver.Minor == 1)
                    osName = WindowsOS.WindowsXP;
                else if (ver.Major == 6)
                {
                    switch (ver.Minor)
                    {
                        case 0: osName = WindowsOS.WindowsVista;
                            break;
                        case 1: osName = WindowsOS.Windows7;
                            break;
                        case 2: osName = WindowsOS.Windows8;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string rv = PasswdInRegsitry.ReadPasswd();
                if (rv != null)
                {
                    PasswordPrompt passPromptDlg = new PasswordPrompt();
                    passPromptDlg.ShowDialog();
                    if (!passPromptDlg.IsAuthorised)
                        Environment.Exit(1);
                    passPromptDlg.Close();
                }
                btnApply.IsEnabled = btnOk.IsEnabled = btnEnabled;
                this.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void helpThumbCache_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Note that it is not advisable to disable thumbnail image caching; as you navigate into image folders without thumbnail caching it would take a long time to list files which otherwise would be reading tiny thumbnail images from thumbs.db -- a 'tiny database' file.\nIn effect, thumbs.db accelerates image file listing when thumbnail view is active.\nGeneral Advice: Leave thumbnail caching as is, but use Disk Cleanup from time to time. \n - Description provided by our user Emilio", "Hmm... So you need more Information?", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpCompProp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Disables access to \"Advanced System Settings\", \"System Protection\" \n and \"Remote Settings\" in Computer Properties.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpAdminShare_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("The administrative shares are the default network shares created by Windows. These default shares share every hard drive partition in the system. These shares will allow anyone who can authenticate as any member of the local Administrators group to access your hard drives by typing in:\n eg.\\Network_Folder\\C$ to access your c:\\", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpSecurtiyTabs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("If checked then the current user opening the Properties dialog box for all file system objects, including folders, files, shortcuts, and drives, will not be able to access the \"Security\" tab. As a result, users will be able to neither change the security settings nor view a list of all users that have access to the resource.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpCustTab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("If checked then all users opening the Properties dialog box for all file system objects, including folders, files, shortcuts, and drives, will not be able to access the \"Customize\" tab. As a result, users will not be able change the folder's display and icon settings.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpSysPntInstlr_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("If checked then, the Windows Installer does not generate System Restore checkpoints when installing applications.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void lnkCompProp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("control", "system");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lnkErrDlgPic_Click(object sender, RoutedEventArgs e)
        {
            popErrorDialog.IsOpen = true;
        }

        private void lnkGP_AutoHow_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Click on this \"Configure Autoplay via Group Policy Editor\" button. \nNow,in the left sidebar navigate to:\nUser Configuration->Administrative Templates->Windows Components->Autoplay Policies\n\nNow check out the options available in the right hand area.\n\nPlease note that Group Policy is NOT available in Windows 7 Home Premium/Basic/Starter Edition(s) and in Windows Vista Home Basic Edition(s).", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void lnkWinUpdt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("control", "update");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lnkActivateAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo("net", "user administrator /active:yes");
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                Process adminProc = new Process();
                adminProc.StartInfo = info;
                adminProc.Start();
                MessageBox.Show("Administrator account activated.", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lnkDeactivateAdmin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo("net", "user administrator /active:no");
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;
                Process adminProc = new Process();
                adminProc.StartInfo = info;
                adminProc.Start();
                MessageBox.Show("Administrator account de-activated.", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void helpShiftDesc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("When logging in Automatically, Shift key can be pressed to skip the logon, and thus person can log into another account!\n\nCheck this option to prevent this \"doorstep\".", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpAutoDesc_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Once logged in, the user can then logout by selecting Logout from Start Menu or by pressing Ctrl+Alt+Del and then selecting Log Off and then log into another account.\n\n\n\nCheck this option to prevent this \"doorstep\".", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void helpSysAttribHow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("In \"Folder Options\" on selection of \n\t\"Show hidden files,folders and drives\", \n and de-selection of \n\t\"Hide protected operating system files(Recommended)\",\nreveals all the hidden Files+the hidden System files.\n\nIf you wish to hide a file/folder with same level of double security, you can use this option to do so.\n If you\'re familiar with DOS command \"attrib\" then this is similar to applying \n\t attrib +h +s <filename/foldername>\non a specific file/folder.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void helpHow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("If Displaying of System File is turned-off then in the \"Folder Options\"\n1.Check \"Show Hidden Files,Folders and Drives\"\n2.Uncheck \"Hide protected operating system files(Recommended)\"\nand finally save these settings. \n\nNow,use the Unhide buttons to navigate to the respective file or folder and then unhide it.\n\nPlease note that this won't work for Windows\' System Files/Folders.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            saveEdits();
            if (MessageBox.Show("Your settings have been successfully saved.\nWould you like to Logoff now?", "Logoff", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                shutDownCmd();
            }
        }        

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            saveEdits(); 
            if (MessageBox.Show("Your settings have been successfully saved.\nWould you like to Logoff now?", "Logoff", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                shutDownCmd();
            }
            Environment.Exit(1);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }

        private void shutDownCmd()
        {
            try
            {
                ProcessStartInfo shutProc = new ProcessStartInfo("shutdown.exe", "/l");
                shutProc.RedirectStandardOutput = true;
                shutProc.UseShellExecute = false;
                shutProc.CreateNoWindow = true;
                Process shutdown = new Process();
                shutdown.StartInfo = shutProc;
                shutdown.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Unable to logoff because the shutdown.exe isn't accessible!", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void saveEdits()
        {
            if (tabOpened[0])
                setSystem();
            if (tabOpened[1])
                setExplorer();
            if (tabOpened[2])
                setDisplay();
            if (tabOpened[3])
                setRightClick();
            if (tabOpened[4])
                setPlaces();
            if (tabOpened[6])
                setFeatures();
            if (tabOpened[7])
                setAutoLogon();
            if (tabOpened[8])
                setRestriction();
            if (tabOpened[9])
                setMaintenance();

        }

        private void setRightClick()
        {
            try
            {
                string[] subkeyhkcrCntxt = hkcrContextMenuHandlers.GetSubKeyNames();
                RegistryKey regKey = null;

                if (chkCPcommand.IsChecked == true)
                {
                    if (hkcrCPcommand == null)
                    {
                        hkcrCPcommand = hkcr.CreateSubKey(@"Directory\Background\shell\Control Panel\command");
                        hkcrCPcommand.SetValue("", "rundll32.exe shell32.dll,Control_RunDLL");
                    }
                }
                else if (hkcrCPcommand != null)
                    hkcrShell.DeleteSubKeyTree("Control Panel", false);

                if (chkOpenNotepad.IsChecked == true)
                {
                    if (hkcrOpenWithNotepad == null)
                    {
                        hkcrOpenWithNotepad = hkcr.CreateSubKey(@"*\shell\Open With Notepad\command");
                        hkcrOpenWithNotepad.SetValue("", "notepad.exe %1");
                    }
                }
                else if (hkcrCPcommand != null)
                    hkcrFileShell.DeleteSubKeyTree("Open With Notepad", false);

                if (chkCopyTo.IsChecked == true)
                {
                    regKey = hkcrContextMenuHandlers.CreateSubKey("Copy To");
                    regKey.SetValue("", "{C2FBB630-2971-11D1-A18C-00C04FD75D13}");
                }
                else
                {
                    regKey = hkcrContextMenuHandlers.OpenSubKey("Copy To", true);
                    if (regKey != null)
                        hkcrContextMenuHandlers.DeleteSubKey("Copy To");
                }

                if (chkMoveTo.IsChecked == true)
                {
                    regKey = hkcrContextMenuHandlers.CreateSubKey("Move To");
                    regKey.SetValue("", "{C2FBB631-2971-11D1-A18C-00C04FD75D13}");
                }
                else
                {
                    regKey = hkcrContextMenuHandlers.OpenSubKey("Move To", true);
                    if (regKey != null)
                        hkcrContextMenuHandlers.DeleteSubKey("Move To");
                }

                if (chkSendTo.IsChecked == true)
                {
                    regKey = hkcrContextMenuHandlers.CreateSubKey("SendTo");
                    regKey.SetValue("", "{7BA4C740-9E81-11CF-99D3-00AA004AE837}");
                }
                else
                {
                    regKey = hkcrContextMenuHandlers.OpenSubKey("SendTo", true);
                    if (regKey != null)
                        hkcrContextMenuHandlers.DeleteSubKey("SendTo");
                }

                if (chkEncrNDec.IsChecked == true)
                    hklmExAdvanced.SetValue("EncryptionContextMenu", 1);
                else
                    hklmExAdvanced.SetValue("EncryptionContextMenu", 0);

                if (chkCmdPromp.IsChecked == true)
                {
                    if (!keySearch(hkcrDirShell.GetSubKeyNames(), "Open Command Prompt here"))
                    {
                        RegistryKey key = hkcrDirShell.CreateSubKey(@"Open Command Prompt here\command");
                        key.SetValue("", "cmd.exe /k cd %1");
                        key = hkcrDriveShell.CreateSubKey(@"Open Command Prompt here\command");
                        key.SetValue("", "cmd.exe /k cd %1");
                    }
                }

                else
                {
                    if (keySearch(hkcrDirShell.GetSubKeyNames(), "Open Command Prompt here"))
                        hkcrDirShell.DeleteSubKeyTree("Open Command Prompt here");
                    if (keySearch(hkcrDriveShell.GetSubKeyNames(), "Open Command Prompt here"))
                        hkcrDriveShell.DeleteSubKeyTree("Open Command Prompt here");
                }

                if (chkDfrg.IsChecked == true)
                {
                    if (!keySearch(hkcrDriveShell.GetSubKeyNames(), "runas"))
                    {
                        RegistryKey key = hkcrDriveShell.CreateSubKey("runas");
                        key.SetValue("", "Defragment");
                        key = key.CreateSubKey("command");
                        key.SetValue("", "defrag %1 -v");
                    }
                }

                else if (keySearch(hkcrDriveShell.GetSubKeyNames(), "runas"))
                {
                    hkcrDriveShell.DeleteSubKeyTree("runas");
                }

                if (osName == WindowsOS.Windows7 || osName == WindowsOS.Windows8)
                {
                    if (chkClip.IsChecked == true)
                        SetClip();
                    else
                        RemoveClip();
                }
            }
            catch (Exception) { }
        }

        private void SetClip()
        {
            try
            {
                string txtFile = (string)hklmDotTxt.GetValue("");
                hklmTxt = hklmClasses.CreateSubKey(txtFile);
                RegistryKey shell = hklmTxt.CreateSubKey("shell");
                RegistryKey copycontents = shell.CreateSubKey("copycontents");
                copycontents.SetValue("", "Copy All Contents");
                RegistryKey command = copycontents.CreateSubKey("command");
                command.SetValue("", "cmd.exe /c clip < \"%1\"");

                hklmTextFile = hklmClasses.CreateSubKey("textfile");
                shell = hklmTextFile.CreateSubKey("shell");
                copycontents = shell.CreateSubKey("copycontents");
                copycontents.SetValue("", "Copy All Contents");
                command = copycontents.CreateSubKey("command");
                command.SetValue("", "cmd.exe /c clip < \"%1\"");
            }
            catch (Exception fn)
            {
                MessageBox.Show(fn.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void RemoveClip()
        {
            try
            {
                hklmTextFile = hklmClasses.OpenSubKey("textfile", true);
                if (hklmTextFile != null)
                {
                    RegistryKey shell = hklmTextFile.OpenSubKey("shell", true);
                    if (shell != null)
                    {
                        string[] subKeysShell = shell.GetSubKeyNames();

                        if (keySearch(subKeysShell, "copycontents"))
                            shell.DeleteSubKeyTree("copycontents");

                        string txtFile = (string)hklmDotTxt.GetValue("");
                        hklmTxt = hklmClasses.OpenSubKey(txtFile, true);
                        shell = hklmTxt.OpenSubKey("shell", true);
                        subKeysShell = shell.GetSubKeyNames();

                        if (keySearch(subKeysShell, "copycontents"))
                            shell.DeleteSubKeyTree("copycontents");
                    }
                }
            }
            catch (Exception){}
        }
        

        private void setDisplay()
        {
            try
            {
                //Display
                if (chkWindowAnimation.IsChecked == true)
                    hkcuWinMet.SetValue("MinAnimate", "1");
                else
                    hkcuWinMet.SetValue("MinAnimate", "0");

                if (chkWinDragContnt.IsChecked == true)
                    hkcuDesktop.SetValue("DragFullWindows", "1");
                else
                    hkcuDesktop.SetValue("DragFullWindows", "0");

                if (chkWinVersion.IsChecked == true)
                    hkcuDesktop.SetValue("PaintDesktopVersion", 1);
                else
                    hkcuDesktop.SetValue("PaintDesktopVersion", 0);

                //Alt Tab            
                hkcuDesktop.SetValue("CoolSwitchColumns", nudAltTabIcons.Value.ToString());
                hkcuDesktop.SetValue("CoolSwitchRows", nudAltTabRows.Value.ToString());

                //Selection Color
                string val = selectionColor.R + " " + selectionColor.G + " " + selectionColor.B;
                hkcuColors.SetValue("HotTrackingColor", val);
            }
            catch (Exception) { }
        }

        private void setPlaces()
        {
            try
            {
                //Default Application
                RegistryKey shell = hkcrUnknown.CreateSubKey("shell");
                if (rbtnWidEndOpen.IsChecked == true)
                {
                    RegistryKey open = shell.OpenSubKey("open", true);
                    if (open != null)
                        shell.DeleteSubKeyTree("open");
                }
                else if (rbtnWidEndApp.IsChecked == true)
                {
                    RegistryKey open = shell.CreateSubKey("open");
                    RegistryKey cmd = open.CreateSubKey("command");
                    if (txtWidEndApp.Text.Contains("\"%1\"") || txtWidEndApp.Text.Contains("%1"))
                        cmd.SetValue("", txtWidEndApp.Text);
                    else
                        cmd.SetValue("", txtWidEndApp.Text + " \"%1\"");
                }

                if (rbtnWidNoEndOpen.IsChecked == true)
                {
                    if (hkcrNoEnd != null)
                    {
                        hkcr.DeleteSubKeyTree(".");
                        hkcrNoEnd = null;
                    }
                }
                else if (rbtnWidNoEndApp.IsChecked == true)
                {
                    if (hkcrNoEnd == null)
                        hkcrNoEnd = hkcr.CreateSubKey(".");
                    shell = hkcrNoEnd.CreateSubKey("shell");
                    RegistryKey open = shell.CreateSubKey("open");
                    RegistryKey cmd = open.CreateSubKey("command");
                    if (txtWidNoEndApp.Text.Contains("\"%1\"") || txtWidNoEndApp.Text.Contains("%1"))
                        cmd.SetValue("", txtWidNoEndApp.Text);
                    else
                        cmd.SetValue("", txtWidNoEndApp.Text + " \"%1\"");
                }

                //Power Button
                int val = comboBxPwrBtn.SelectedIndex;
                switch (val)
                {
                    case 0: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 2);
                        break;
                    case 1: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 64);
                        break;
                    case 2: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 16);
                        break;
                    case 3: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 4);
                        break;
                    case 4: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 512);
                        break;
                    case 5: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 1);
                        break;
                    case 6: hkcuExAdvanced.SetValue("Start_PowerButtonAction", 256);
                        break;
                }
            }
            catch (Exception) { }
        }

        private void setMaintenance()
        {
            try
            {
                //Memory

                if (chkUnloadDLL.IsChecked == true)
                    hklmCVExplorer.SetValue("AlwaysUnloadDLL", 1);
                else
                {
                    string[] valueshklmCVExplorer = hklmCVExplorer.GetValueNames();
                    if (keySearch(valueshklmCVExplorer, "AlwaysUnloadDLL"))
                        hklmCVExplorer.DeleteValue("AlwaysUnloadDLL");
                }
                if (chkGPUpdate.IsChecked == true)
                {
                    hklmSystem.SetValue("SynchronousMachineGroupPolicy", 0);
                    hklmSystem.SetValue("SynchronousUserGroupPolicy", 0);
                }
                else
                {
                    string[] valueshklmSystem = hklmSystem.GetValueNames();
                    if (keySearch(valueshklmSystem, "SynchronousMachineGroupPolicy"))
                        hklmSystem.DeleteValue("SynchronousMachineGroupPolicy");
                    if (keySearch(valueshklmSystem, "SynchronousUserGroupPolicy"))
                        hklmSystem.DeleteValue("SynchronousUserGroupPolicy");
                }

                //Auto Reboot

                if (comboBxAutoReboot.SelectedIndex == 0)
                    hklmCrashControl.SetValue("AutoReboot", 1);
                else
                    hklmCrashControl.SetValue("AutoReboot", 0);

                if (chkRecEvent.IsChecked == true)
                    hklmCrashControl.SetValue("LogEvent", 1);
                else
                    hklmCrashControl.SetValue("LogEvent", 0);

                //Error Reporting

                if (chkErrDialog.IsChecked == true)
                    hkcuWinErrReporting.SetValue("DontShowUI", 1);
                else
                    hkcuWinErrReporting.SetValue("DontShowUI", 0);

                switch (comboBoxErrType.SelectedIndex)
                {
                    case 0:
                        hkcuWinErrReporting.SetValue("Disabled", 0);
                        hkcuConsent.SetValue("DefaultConsent", 1);
                        break;
                    case 1:
                        hkcuWinErrReporting.SetValue("Disabled", 0);
                        hkcuConsent.SetValue("DefaultConsent", 2);
                        break;
                    case 2:
                        hkcuWinErrReporting.SetValue("Disabled", 0);
                        hkcuConsent.SetValue("DefaultConsent", 3);
                        break;
                    case 3:
                        hkcuWinErrReporting.SetValue("Disabled", 1);
                        break;
                }

                //Startup maintenance
                if (chkLKGC.IsChecked == true)
                    hklmWinLogon.SetValue("ReportBootOk", "0");
                else
                {
                    string[] valueshklmWinLogon = hklmWinLogon.GetValueNames();
                    if (keySearch(valueshklmWinLogon, "ReportBootOk"))
                        hklmWinLogon.DeleteValue("ReportBootOk");
                }

                if (chkBootDfrg.IsChecked == true)
                    hklmBootOptFunc.SetValue("Enable", "Y");
                else
                    hklmBootOptFunc.SetValue("Enable", "N");

                int val = (int)nudHDScnTym.Value;
                hklmSSMgr.SetValue("AutoChkTimeOut", val);
            }
            catch (Exception) { }
        }

        private void setRestriction()
        {
            try
            {
                string[] valueshkcuDWM = hkcuDWM.GetValueNames();
                //Explorer
                if (chkScreenSvr.IsChecked == true)
                    hklmSystem.SetValue("NoDispScrSavPage", 1);
                else
                    hklmSystem.SetValue("NoDispScrSavPage", 0);

                if (chkAppearance.IsChecked == true)
                    hklmSystem.SetValue("NoDispAppearancePage", 1);
                else
                    hklmSystem.SetValue("NoDispAppearancePage", 0);

                if (chkContext.IsChecked == true)
                    hkcuExplorer.SetValue("NoViewContextMenu", 1);
                else
                    hkcuExplorer.SetValue("NoViewContextMenu", 0);

                if (chkFolderOp.IsChecked == true)
                    hkcuExplorer.SetValue("NoFolderOptions", 1);
                else
                    hkcuExplorer.SetValue("NoFolderOptions", 0);

                if (chkFile.IsChecked == true)
                    hkcuExplorer.SetValue("NoFileMenu", 1);
                else
                    hkcuExplorer.SetValue("NoFileMenu", 0);


                if (chkThumbCache.IsChecked == true)
                    hkcuExAdvanced.SetValue("DisableThumbnailCache", 1);
                else
                    hkcuExAdvanced.SetValue("DisableThumbnailCache", 0);

                if (osName != WindowsOS.WindowsXP)
                {
                    if (chkFlip3D.IsChecked == true)
                        hkcuDWM.SetValue("DisallowFlip3D", 1);
                    else
                    {
                        if (keySearch(valueshkcuDWM, "DisallowFlip3D"))
                            hkcuDWM.DeleteValue("DisallowFlip3D");
                    }
                }

                //StartMenu
                if (chkShutdown.IsChecked == true)
                    hkcuExplorer.SetValue("NoClose", 1);
                else
                    hkcuExplorer.SetValue("NoClose", 0);

                if (chkStartChnge.IsChecked == true)
                    hkcuExplorer.SetValue("NoChangeStartMenu", 1);
                else
                    hkcuExplorer.SetValue("NoChangeStartMenu", 0);

                if (chkRecentDoc.IsChecked == true)
                    hkcuExplorer.SetValue("NoRecentDocsMenu", 1);
                else
                    hkcuExplorer.SetValue("NoRecentDocsMenu", 0);

                if (chkLogOff.IsChecked == true)
                    hkcuExplorer.SetValue("NoLogoff", 1);
                else
                    hkcuExplorer.SetValue("NoLogoff", 0);

                if (chkControlPanel.IsChecked == true)
                    hklmSystem.SetValue("NoDispCPL", 1);
                else
                    hklmSystem.SetValue("NoDispCPL", 0);

                //System
                if (chkRegedit.IsChecked == true)
                    hkcuExplorer.SetValue("DisableRegistryTools", 1);
                else
                    hkcuExplorer.SetValue("DisableRegistryTools", 0);

                if (chkVirtMem.IsChecked == true)
                    hklmSystem.SetValue("NoVirtMemPage", 1);
                else
                    hklmSystem.SetValue("NoVirtMemPage", 0);

                if (chkWinUpdt.IsChecked == true)
                    hkcuExplorer.SetValue("NoWindowsUpdate", 1);
                else
                    hkcuExplorer.SetValue("NoWindowsUpdate", 0);

                if (chkPrinterDel.IsChecked == true)
                    hkcuExplorer.SetValue("NoDeletePrinter", 1);
                else
                    hkcuExplorer.SetValue("NoDeletePrinter", 0);

                if (chkPrinterAdd.IsChecked == true)
                    hkcuExplorer.SetValue("NoAddPrinter", 1);
                else
                    hkcuExplorer.SetValue("NoAddPrinter", 0);

                if (chkTaskMgr.IsChecked == true)
                    hkcuSystem.SetValue("DisableTaskMgr", 1);
                else
                    hkcuSystem.SetValue("DisableTaskMgr", 0);

                if (chkMyCompProp.IsChecked == true)
                    hkcuExplorer.SetValue("NoPropertiesMyComputer", 1);
                else
                    hkcuExplorer.SetValue("NoPropertiesMyComputer", 0);

                if (chkAdminShare.IsChecked == true)
                {
                    hklmParams.SetValue("AutoShareWks", 0);
                }
                else
                {
                    string[] valueshklmParams = hklmParams.GetValueNames();
                    if (keySearch(valueshklmParams, "AutoShareWks"))
                        hklmParams.DeleteValue("AutoShareWks");
                }

                //TaskBar
                if (chkTaskbarAnim.IsChecked == true)
                    hkcuExAdvanced.SetValue("TaskbarAnimations", 0);
                else
                    hkcuExAdvanced.SetValue("TaskbarAnimations", 1);

                if (osName != WindowsOS.WindowsXP)
                {

                    if (chkTaskIconSize.IsChecked == true)
                        hkcuExAdvanced.SetValue("TaskbarSmallIcons", 1);
                    else
                        hkcuExAdvanced.SetValue("TaskbarSmallIcons", 0);
                }
            }
            catch (Exception) { }
        }

        private void setAutoLogon()
        {
            try
            {
                //Auto Logon
                string[] valueshklmWinLogon = hklmWinLogon.GetValueNames();
                if (chkAutoLogin.IsChecked == true)
                {
                    hklmWinLogon.SetValue("AutoAdminLogon", "1");
                    hklmWinLogon.SetValue("DefaultUserName", txtAutoUsrNm.Text);
                    hklmWinLogon.SetValue("DefaultDomainName", txtAutoDomain.Text);
                    hklmWinLogon.SetValue("DefaultPassword", txtAutoPaswrd.Password);
                }
                else
                {
                    hklmWinLogon.SetValue("AutoAdminLogon", "0");
                    if (keySearch(valueshklmWinLogon, "DefaultUserName"))
                        hklmWinLogon.DeleteValue("DefaultUserName");
                    if (keySearch(valueshklmWinLogon, "DefaultDomainName"))
                        hklmWinLogon.DeleteValue("DefaultDomainName");
                    if (keySearch(valueshklmWinLogon, "DefaultPassword"))
                        hklmWinLogon.DeleteValue("DefaultPassword");
                }

                if (chkShift.IsChecked == true)
                    hklmWinLogon.SetValue("IgnoreShiftOverride", "1");
                else
                {
                    if (keySearch(valueshklmWinLogon, "IgnoreShiftOverride"))
                        hklmWinLogon.DeleteValue("IgnoreShiftOverride");
                }

                if (chkLogOffAuto.IsChecked == true)
                    hklmWinLogon.SetValue("ForceAutoLogon", "1");
                else
                {
                    if (keySearch(valueshklmWinLogon, "ForceAutoLogon"))
                        hklmWinLogon.DeleteValue("ForceAutoLogon");
                }

                //Startup Sound
                if (chkStartupSnd.IsChecked == true)
                {
                    string[] valueshklmBootAnim = hklmBootAnimation.GetValueNames();
                    if (keySearch(valueshklmBootAnim, "DisableStartupSound"))
                        hklmBootAnimation.DeleteValue("DisableStartupSound");
                }
                else
                    hklmBootAnimation.SetValue("DisableStartupSound", 1);

                //Screen Saver
                if (chkScreenSvrPol.IsChecked == true)
                    hkcuPDesktop.SetValue("ScreenSaverIsSecure", 1);
                else
                {
                    string[] valueshkcuPDesktop = hkcuPDesktop.GetValueNames();
                    if (keySearch(valueshkcuPDesktop, "ScreenSaverIsSecure"))
                    {
                        hkcuPDesktop.DeleteValue("ScreenSaverIsSecure");
                    }
                }

                //Require ctrl_alt_del
                if (chkCAD.IsChecked == true)
                    hklmWinLogon.SetValue("DisableCAD", 0);
                else
                    hklmWinLogon.SetValue("DisableCAD", 1);

                if (chkLastUsrName.IsChecked == true)
                    hklmSystem.SetValue("dontdisplaylastusername", 1);
                else
                    hklmSystem.SetValue("dontdisplaylastusername", 0);

                if (chkShutDwnLogOn.IsChecked == true)
                    hklmSystem.SetValue("shutdownwithoutlogon", 1);
                else
                    hklmSystem.SetValue("shutdownwithoutlogon", 0);

                //Message
                if (chkLegalNotice.IsChecked == true)
                {
                    hklmSystem.SetValue("LegalNoticeCaption", txtLogHeader.Text);
                    hklmSystem.SetValue("LegalNoticeText", txtContent.Text);
                }
                else
                {
                    hklmSystem.SetValue("LegalNoticeCaption", "");
                    hklmSystem.SetValue("LegalNoticeText", "");
                }
            }
            catch (NullReferenceException ne)
            {
                MessageBox.Show("Unable to read Values:\n" + ne.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception) { }
        }

        private void setFeatures()
        {
            try
            {
                //Autoplay
                if (rbtnEnableAuto.IsChecked == true)
                {
                    hkcuExplorer.SetValue("NoDriveTypeAutoRun", 145);
                    hkcuAutoplayHandlers.SetValue("DisableAutoplay", 0);
                    hklmCdRom.SetValue("AutoRun", 1);
                }
                else if (rbtnEnableCDUsbAuto.IsChecked == true)
                {
                    hkcuExplorer.SetValue("NoDriveTypeAutoRun", 181);
                    hkcuAutoplayHandlers.SetValue("DisableAutoplay", 0);
                    hklmCdRom.SetValue("AutoRun", 1);
                }
                else if (rbtnOffAuto.IsChecked == true)
                {
                    hkcuExplorer.SetValue("NoDriveTypeAutoRun", 255);
                    hkcuAutoplayHandlers.SetValue("DisableAutoplay", 1);
                    hklmCdRom.SetValue("AutoRun", 0);
                }

                //Windows Update
                switch (comboBoxWinUpdt.SelectedIndex)
                {
                    case 0: hklmAutoUpdate.SetValue("AUOptions", 4);
                        break;
                    case 1: hklmAutoUpdate.SetValue("AUOptions", 3);
                        break;
                    case 2: hklmAutoUpdate.SetValue("AUOptions", 2);
                        break;
                    case 3: hklmAutoUpdate.SetValue("AUOptions", 1);
                        break;
                }

                if (chkRecomedUpdts.IsChecked == true)
                    hklmAutoUpdate.SetValue("IncludeRecommendedUpdates", 1);
                else
                    hklmAutoUpdate.SetValue("IncludeRecommendedUpdates", 0);

                if (chkWinUpdtInstalEle.IsChecked == true)
                    hklmAutoUpdate.SetValue("ElevateNonAdmins", 1);
                else
                    hklmAutoUpdate.SetValue("ElevateNonAdmins", 0);

                if (chkDetailNotify.IsChecked == true)
                    hklmAutoUpdate.SetValue("EnableFeaturedSoftware", 1);
                else
                    hklmAutoUpdate.SetValue("EnableFeaturedSoftware", 0);

                //System beeps

                if (chkBeeps.IsChecked == true)
                    hkcuSound.SetValue("Beep", "no");
                else
                    hkcuSound.SetValue("Beep", "yes");

                //DVD burning Feature
                string[] valueshkcuExplorer = hkcuExplorer.GetValueNames();
                if (chkDVDburn.IsChecked == true)
                {
                    if (keySearch(valueshkcuExplorer, "NoCDBurning"))
                        hkcuExplorer.DeleteValue("NoCDBurning");
                }
                else
                    hkcuExplorer.SetValue("NoCDBurning", 1);
            }
            catch (Exception) { }
        }

        private void setExplorer()
        {
            try
            {
                //Drive Letters
                if (rbtnDrvLtrB4Name.IsChecked == true)
                    hkcuCVExplorer.SetValue("ShowDriveLettersFirst", 4);
                else if (rbtnDrvLtrAftrName.IsChecked == true)
                    hkcuCVExplorer.SetValue("ShowDriveLettersFirst", 0);
                else if (rbtnNoDrvLtr.IsChecked == true)
                    hkcuCVExplorer.SetValue("ShowDriveLettersFirst", 2);

                //Libraries
                if (osName != WindowsOS.WindowsXP)
                {
                    string[] subkeyshklmNamespaces = hklmNamespaces.GetSubKeyNames();
                    if (rbtnShowLib.IsChecked == true)
                    {
                        if (!keySearch(subkeyshklmNamespaces, "{031E4825-7B94-4dc3-B131-E946B44C8DD5}"))
                        {
                            RegistryKey libKey = hklmNamespaces.CreateSubKey("{031E4825-7B94-4dc3-B131-E946B44C8DD5}");
                            libKey.SetValue("", "UsersLibraries");
                            libKey.SetValue("Removal Message", "@shell32.dll,-9047");
                        }
                    }
                    else if (rbtnHideLib.IsChecked == true)
                    {
                        if (keySearch(subkeyshklmNamespaces, "{031E4825-7B94-4dc3-B131-E946B44C8DD5}"))
                        {
                            hklmNamespaces.DeleteSubKeyTree("{031E4825-7B94-4dc3-B131-E946B44C8DD5}");
                        }
                    }
                }

                //Notification Area

                if (chkNotificationArea.IsChecked == true)
                {
                    byte[] b = { 00, 00, 00, 00 };
                    hkcuExplorer.SetValue("NoTrayItemsDisplay", b);
                }
                else
                {
                    byte[] b = { 01, 00, 00, 00 };
                    hkcuExplorer.SetValue("NoTrayItemsDisplay", b);
                }

                if (chkBalloonTip.IsChecked == true && chkBalloonTip.IsEnabled)
                    hkcuExplorer.SetValue("TaskbarNoNotification", 0);
                else
                    hkcuExplorer.SetValue("TaskbarNoNotification", 1);

                if (chkLowDiskSp.IsChecked == true)
                    hkcuExplorer.SetValue("NoLowDiskSpaceChecks", 0);
                else
                    hkcuExplorer.SetValue("NoLowDiskSpaceChecks", 1);

                //Explorer Advanced

                if (chkCheckBoxes.IsChecked == true)
                    hkcuExAdvanced.SetValue("AutoCheckSelect", 1);
                else
                    hkcuExAdvanced.SetValue("AutoCheckSelect", 0);

                if (chkFileExt.IsChecked == true)
                    hkcuExAdvanced.SetValue("HideFileExt", 1);
                else
                    hkcuExAdvanced.SetValue("HideFileExt", 0);

                if (chkHidden.IsChecked == true)
                    hkcuExAdvanced.SetValue("Hidden", 1);
                else
                    hkcuExAdvanced.SetValue("Hidden", 2);

                if (chkMenuBar.IsChecked == true)
                    hkcuExAdvanced.SetValue("AlwaysShowMenus", 1);
                else
                    hkcuExAdvanced.SetValue("AlwaysShowMenus", 0);

                if (chkPersistBrowsers.IsChecked == true)
                    hkcuExAdvanced.SetValue("PersistBrowsers", 1);
                else
                    hkcuExAdvanced.SetValue("PersistBrowsers", 0);

                if (chkShowSysFiles.IsChecked == true)
                    hkcuExAdvanced.SetValue("ShowSuperHidden", 1);
                else
                    hkcuExAdvanced.SetValue("ShowSuperHidden", 0);

                if (chkCompColor.IsChecked == true)
                    hkcuExAdvanced.SetValue("ShowCompColor", 1);
                else
                    hkcuExAdvanced.SetValue("ShowCompColor", 0);

                if (chkSecurityTabs.IsChecked == true)
                    hkcuExplorer.SetValue("NoSecurityTab", 1);
                else
                {
                    string[] valueshkcuExplorer = hkcuExplorer.GetValueNames();
                    if (keySearch(valueshkcuExplorer, "NoSecurityTab"))
                        hkcuExplorer.DeleteValue("NoSecurityTab");
                }

                if (chkCustTab.IsChecked == true)
                    hklmExplorer.SetValue("NoCustomizeThisFolder", 1);
                else
                    hklmExplorer.SetValue("NoCustomizeThisFolder", 0);

                //Etcetra
                if (chkRecycleBin.IsChecked == true)
                {
                    if (!keySearch(hklmNamespace.GetSubKeyNames(), "{645FF040-5081-101B-9F08-00AA002F954E}"))
                        hklmNamespace.CreateSubKey("{645FF040-5081-101B-9F08-00AA002F954E}");
                }

                else if (keySearch(hklmNamespace.GetSubKeyNames(), "{645FF040-5081-101B-9F08-00AA002F954E}"))
                    hklmNamespace.DeleteSubKeyTree("{645FF040-5081-101B-9F08-00AA002F954E}");

                if (chkRegeditToCP.IsChecked == true)
                {
                    if (!keySearch(hkcrCLSID.GetSubKeyNames(), "{77708248-f839-436b-8919-527c410f48b9}"))
                    {
                        RegistryKey cp = hkcrCLSID.CreateSubKey("{77708248-f839-436b-8919-527c410f48b9}");
                        cp.SetValue("", "Registry Editor");
                        cp.SetValue("InfoTip", "Starts the Registry Editor");
                        cp.SetValue("System.ControlPanel.Category", "5");
                        cp = cp.CreateSubKey("DefaultIcon");
                        cp.SetValue("", @"%SYSTEMROOT%\regedit.exe");
                        cp = hkcrCLSID.CreateSubKey(@"{77708248-f839-436b-8919-527c410f48b9}\Shell\Open\command");
                        cp.SetValue("", Environment.ExpandEnvironmentVariables("%SystemRoot%\\regedit.exe"));
                        cp = hklmCPNamespace.CreateSubKey("{77708248-f839-436b-8919-527c410f48b9}");
                        cp.SetValue("", "Add Registry Editor to Control Panel");
                    }
                }

                else
                {
                    if (keySearch(hkcrCLSID.GetSubKeyNames(), "{77708248-f839-436b-8919-527c410f48b9}"))
                    {
                        hkcrCLSID.DeleteSubKeyTree("{77708248-f839-436b-8919-527c410f48b9}");
                    }

                    if (keySearch(hklmCPNamespace.GetSubKeyNames(), "{77708248-f839-436b-8919-527c410f48b9}"))
                    {
                        hklmCPNamespace.DeleteSubKeyTree("{77708248-f839-436b-8919-527c410f48b9}");
                    }
                }
            }
            catch (Exception) { }
        }

        private void LoadSystemTabKeys()
        {
            try
            {
                hkcuDesktop = hkcu.CreateSubKey(@"Control Panel\Desktop");

                hklmInstaller = hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows\Installer");
                hklmCVWinNT = hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion");
                hklmOEM = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\OEMInformation");
                hklmPWindows = hklm.CreateSubKey(@"Software\Policies\Microsoft\Windows");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tabSystem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[0])
            {
                tabOpened[0] = true;
                LoadSystemTabKeys();
                examineSystem();
            }
        }

        private void examineSystem()
        {
            try
            {
                int val = 0;
                string[] valueshkcuDesktop = hkcuDesktop.GetValueNames();
                string[] valueshklmInstaller = hklmInstaller.GetValueNames();
                string[] valueshklmOEM = hklmOEM.GetValueNames();
                string[] valueshklmCVWinNT = hklmCVWinNT.GetValueNames();

                //Shutdown Configuration
                try
                {
                    if (keySearch(valueshkcuDesktop, "AutoEndTasks"))
                    {
                        string valStr = (string)hkcuDesktop.GetValue("AutoEndTasks");
                        rbtnDntWait.IsChecked = getStrStatus(valStr);
                    }
                }
                catch (InvalidCastException)
                {
                    rbtnDntWait.Visibility = Visibility.Collapsed;
                }

                if (keySearch(valueshkcuDesktop, "WaitToKillAppTimeout"))
                {
                    string valStr = (string)hkcuDesktop.GetValue("WaitToKillAppTimeout");
                    nudTime.Value = int.Parse(valStr);
                    rbtnWait.IsChecked = !rbtnDntWait.IsChecked;
                }
                else
                {
                    if (rbtnDntWait.IsChecked == false)
                        rbtnWait.IsChecked = true;
                }

                //Windows Installer
                if (keySearch(valueshklmInstaller, "DisableMSI"))
                {
                    val = (int)hklmInstaller.GetValue("DisableMSI");
                    chkWinInstalr.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmInstaller, "AlwaysInstallElevated"))
                {
                    val = (int)hklmInstaller.GetValue("AlwaysInstallElevated");
                    chkAdminPrev.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmInstaller, "LimitSystemRestoreCheckpointing"))
                {
                    val = (int)hklmInstaller.GetValue("LimitSystemRestoreCheckpointing");
                    chkSysPntInstlr.IsChecked = getStatus(val);
                }

                //Registration
                if (keySearch(valueshklmCVWinNT, "RegisteredOwner"))
                    txtOwnerName.Text = (string)hklmCVWinNT.GetValue("RegisteredOwner");

                if (keySearch(valueshklmCVWinNT, "RegisteredOrganization"))
                    txtCompanyName.Text = (string)hklmCVWinNT.GetValue("RegisteredOrganization");

                if (keySearch(valueshklmCVWinNT, "ProductId"))
                    txtProductID.Text = (string)hklmCVWinNT.GetValue("ProductId");

                //Information
                if (keySearch(valueshklmOEM, "Manufacturer"))
                {
                    txtManufacturer.Text = (string)hklmOEM.GetValue("Manufacturer");
                }
                if (keySearch(valueshklmOEM, "Model"))
                {
                    txtModel.Text = (string)hklmOEM.GetValue("Model");
                }
                if (keySearch(valueshklmOEM, "SupportPhone"))
                {
                    txtSupportPh.Text = (string)hklmOEM.GetValue("SupportPhone");
                }
                if (keySearch(valueshklmOEM, "SupportURL"))
                {
                    txtSupportUrl.Text = (string)hklmOEM.GetValue("SupportURL");
                }
                if (keySearch(valueshklmOEM, "Logo"))
                {
                    string valStr = (string)hklmOEM.GetValue("Logo");
                    if (valStr.Length > 0)
                    {
                        btnDelLogo.IsEnabled = true;
                        try
                        {
                            imgProperty.Source = new BitmapImage(new Uri(valStr, UriKind.Absolute));
                        }
                        catch (UriFormatException)
                        {
                            valStr = "C:" + valStr;
                            imgProperty.Source = new BitmapImage(new Uri(valStr, UriKind.Absolute));
                        }
                    }
                }
            }
            catch (NullReferenceException nullExcep) { MessageBox.Show("Unable to read Values:\n" + nullExcep.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception) {}
        }

        private void setSystem()
        {
            try
            {
                //Shutdown Configuration
                if (rbtnDntWait.Visibility == Visibility.Visible)
                {
                    if (rbtnDntWait.IsChecked == true)
                        hkcuDesktop.SetValue("AutoEndTasks", "1");
                    else
                        hkcuDesktop.SetValue("AutoEndTasks", "0");

                    if (rbtnWait.IsChecked == true)
                        hkcuDesktop.SetValue("WaitToKillAppTimeout", nudTime.Value.ToString());
                }

                //Windows Installer
                string[] valueshklmInstaller = null;
                if (!hklmInstaller_Del)
                    valueshklmInstaller = hklmInstaller.GetValueNames();

                if (chkWinInstalr.IsChecked == true)
                {
                    if (hklmInstaller_Del == true)
                    {
                        hklmInstaller = hklmPWindows.CreateSubKey("Installer");
                        valueshklmInstaller = hklmInstaller.GetValueNames();
                        hklmInstaller_Del = false;
                    }
                    hklmInstaller.SetValue("DisableMSI", 2);
                }
                else
                {
                    if (!hklmInstaller_Del && keySearch(valueshklmInstaller, "DisableMSI"))
                        hklmInstaller.DeleteValue("DisableMSI");
                }

                if (chkAdminPrev.IsChecked == true)
                {
                    if (hklmInstaller_Del == true)
                    {
                        hklmInstaller = hklmPWindows.CreateSubKey("Installer");
                        valueshklmInstaller = hklmInstaller.GetValueNames();
                        hklmInstaller_Del = false;
                    }
                    hklmInstaller.SetValue("AlwaysInstallElevated", 1);
                }
                else
                {
                    if (!hklmInstaller_Del && keySearch(valueshklmInstaller, "AlwaysInstallElevated"))
                        hklmInstaller.DeleteValue("AlwaysInstallElevated");
                }

                if (chkSysPntInstlr.IsChecked == true)
                {
                    if (hklmInstaller_Del == true)
                    {
                        hklmInstaller = hklmPWindows.CreateSubKey("Installer");
                        valueshklmInstaller = hklmInstaller.GetValueNames();
                        hklmInstaller_Del = false;
                    }
                    hklmInstaller.SetValue("LimitSystemRestoreCheckpointing", 1);
                }

                else
                {
                    if (!hklmInstaller_Del && keySearch(valueshklmInstaller, "LimitSystemRestoreCheckpointing"))
                        hklmInstaller.DeleteValue("LimitSystemRestoreCheckpointing");
                }

                if (chkSysPntInstlr.IsChecked == false && chkAdminPrev.IsChecked == false && chkWinInstalr.IsChecked == false && !hklmInstaller_Del)
                {
                    hklm.DeleteSubKey(@"Software\Policies\Microsoft\Windows\Installer");
                    hklmInstaller_Del = true;
                }

                //System Registration
                hklmCVWinNT.SetValue("RegisteredOwner", txtOwnerName.Text);
                hklmCVWinNT.SetValue("RegisteredOrganization", txtCompanyName.Text);
                hklmCVWinNT.SetValue("ProductId", txtProductID.Text);

                //Edit Information
                hklmOEM.SetValue("Manufacturer", txtManufacturer.Text);
                hklmOEM.SetValue("Model", txtModel.Text);
                hklmOEM.SetValue("SupportPhone", txtSupportPh.Text);
                hklmOEM.SetValue("SupportURL", txtSupportUrl.Text);
            }
            catch (Exception) { }
        }

        private void btnSelectLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                openFileDialog.Filter = "Bitmap Images|*.bmp";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == true)
                {
                    hklmOEM.SetValue("Logo", openFileDialog.FileName);
                    imgProperty.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                    btnDelLogo.IsEnabled = true;
                    MessageBox.Show("Image Successfully applied!\n\nIf your Computer\'s property windows is already open then please close and re-open the window for the changes to be reflected.", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception){}
        }

        private void btnDelLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] valueshklmOEM = hklmOEM.GetValueNames();
                if (keySearch(valueshklmOEM, "Logo"))
                {
                    hklmOEM.SetValue("Logo", "");
                    btnDelLogo.IsEnabled = false;
                    imgProperty.Source = null;
                    MessageBox.Show("Image Successfully Removed!!", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tabExplorer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[1])
            {
                tabOpened[1] = true;
                if (osName != WindowsOS.Windows8)
                    gridMetroShortcut.Visibility = Visibility.Collapsed;
                LoadExplorerKeys();
                examineExplorer();
            }
        }

        private void examineExplorer()
        {
            //Drive Letters
            try
            {
                int val = 0;
                string[] valueshkcuCVExplorer = hkcuCVExplorer.GetValueNames();
                if (keySearch(valueshkcuCVExplorer, "ShowDriveLettersFirst"))
                {
                    val = (int)hkcuCVExplorer.GetValue("ShowDriveLettersFirst");
                    switch (val)
                    {
                        case 0: rbtnDrvLtrAftrName.IsChecked = true;
                            break;
                        case 2: rbtnNoDrvLtr.IsChecked = true;
                            break;
                        case 4: rbtnDrvLtrB4Name.IsChecked = true;
                            break;
                        default: rbtnDrvLtrAftrName.IsChecked = true;
                            break;
                    }
                }
                else
                {
                    rbtnDrvLtrAftrName.IsChecked = true;
                }

                //Libraries
                if (osName == WindowsOS.WindowsXP)
                {
                    gbLib.Visibility = Visibility.Collapsed;
                }
                else
                {
                    string[] subkeyshklmNamespaces = hklmNamespaces.GetSubKeyNames();
                    if (keySearch(subkeyshklmNamespaces, "{031E4825-7B94-4dc3-B131-E946B44C8DD5}"))
                    {
                        rbtnShowLib.IsChecked = true;
                    }
                    else
                        rbtnHideLib.IsChecked = true;
                }

                //Notification Area

                string[] valueshkcuExplorer = hkcuExplorer.GetValueNames();

                if (keySearch(valueshkcuExplorer, "TaskbarNoNotification"))
                {
                    val = (int)hkcuExplorer.GetValue("TaskbarNoNotification");
                    chkBalloonTip.IsChecked = !getStatus(val);
                }
                else
                    chkBalloonTip.IsChecked = true;

                if (keySearch(valueshkcuExplorer, "NoTrayItemsDisplay"))
                {
                    byte[] b = (byte[])hkcuExplorer.GetValue("NoTrayItemsDisplay");
                    if (b[0] == 1)
                        chkNotificationArea.IsChecked = chkBalloonTip.IsEnabled = false;
                    else
                        chkNotificationArea.IsChecked = chkBalloonTip.IsEnabled = true;
                }
                else
                    chkNotificationArea.IsChecked = true;

                if (keySearch(valueshkcuExplorer, "NoLowDiskSpaceChecks"))
                {
                    val = (int)hkcuExplorer.GetValue("NoLowDiskSpaceChecks");
                    chkLowDiskSp.IsChecked = !getStatus(val);
                }
                else
                    chkLowDiskSp.IsChecked = true;

                //Explorer Advanced
                
                string[] valueshkcuExAdvanced = hkcuExAdvanced.GetValueNames();               
                string[] valueshklmExplorer = hklmExplorer.GetValueNames();

                if (keySearch(valueshkcuExAdvanced, "AlwaysShowMenus"))
                {
                    val = (int)hkcuExAdvanced.GetValue("AlwaysShowMenus");
                    chkMenuBar.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExAdvanced, "HideFileExt"))
                {
                    val = (int)hkcuExAdvanced.GetValue("HideFileExt");
                    chkFileExt.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExAdvanced, "AutoCheckSelect"))
                {
                    val = (int)hkcuExAdvanced.GetValue("AutoCheckSelect");
                    chkCheckBoxes.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExAdvanced, "Hidden"))
                {
                    val = (int)hkcuExAdvanced.GetValue("Hidden");
                    if (val == 1)
                        chkHidden.IsChecked = true;
                    else
                        chkHidden.IsChecked = false;
                }

                if (keySearch(valueshkcuExAdvanced, "PersistBrowsers"))
                {
                    val = (int)hkcuExAdvanced.GetValue("PersistBrowsers");
                    chkPersistBrowsers.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExAdvanced, "ShowSuperHidden"))
                {
                    val = (int)hkcuExAdvanced.GetValue("ShowSuperHidden");
                    chkShowSysFiles.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExAdvanced, "ShowCompColor"))
                {
                    val = (int)hkcuExAdvanced.GetValue("ShowCompColor");
                    chkCompColor.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoSecurityTab"))
                {
                    val = (int)hkcuExplorer.GetValue("NoSecurityTab");
                    chkSecurityTabs.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmExplorer, "NoCustomizeThisFolder"))
                {
                    val = (int)hklmExplorer.GetValue("NoCustomizeThisFolder");
                    chkCustTab.IsChecked = getStatus(val);
                }

                //Etcetra
                chkRecycleBin.IsChecked= keySearch(hklmNamespace.GetSubKeyNames(),"{645FF040-5081-101B-9F08-00AA002F954E}");
                chkRegeditToCP.IsChecked = keySearch(hkcrCLSID.GetSubKeyNames(), "{77708248-f839-436b-8919-527c410f48b9}") && keySearch(hklmCPNamespace.GetSubKeyNames(), "{77708248-f839-436b-8919-527c410f48b9}");
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception) { }
        }

        private void LoadExplorerKeys()
        {
            try
            {
                hkcuCVExplorer = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer");
                hklmNamespaces = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Desktop\NameSpace");
                hkcuExplorer = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
                hkcuExAdvanced = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                hklmExplorer = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
                hklmNamespace = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\MyComputer\NameSpace");
                hklmCPNamespace = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ControlPanel\NameSpace");
                hkcrCLSID = hkcr.CreateSubKey(@"CLSID");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }        

        private void tabDisplay_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[2])
            {
                LoadDisplayKeys();
                ExamineDisplay();
                tabOpened[2] = true;
            }
        }

        private void ExamineDisplay()
        {
            string[] valueshkcuColors = hkcuColors.GetValueNames();
            string[] valueshkcuWinMet = hkcuWinMet.GetValueNames();
            string[] valueshkcuDesktop = hkcuDesktop.GetValueNames();

            //Display
            if (keySearch(valueshkcuWinMet, "MinAnimate"))
            {
                string valStr = (string)hkcuWinMet.GetValue("MinAnimate");
                chkWindowAnimation.IsChecked = getStrStatus(valStr);
            }

            if (keySearch(valueshkcuDesktop, "DragFullWindows"))
            {
                string valStr = (string)hkcuDesktop.GetValue("DragFullWindows");
                chkWinDragContnt.IsChecked = getStrStatus(valStr);
            }
            if (keySearch(valueshkcuDesktop, "PaintDesktopVersion"))
            {
                int val = (int)hkcuDesktop.GetValue("PaintDesktopVersion");
                chkWinVersion.IsChecked = getStatus(val);
            }

            //Alt Tab

            try
            {
                nudAltTabIcons.Value = int.Parse((string)hkcuDesktop.GetValue("CoolSwitchColumns"));
                nudAltTabRows.Value = int.Parse((string)hkcuDesktop.GetValue("CoolSwitchRows"));
            }
            catch (Exception fe)
            {
                MessageBox.Show("Error reading values of ALT+Tab!!\n\n" + fe.Message + "\n\nSetting them to default state", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                nudAltTabIcons.Value = 7;
                nudAltTabRows.Value = 3;
                hkcuDesktop.SetValue("CoolSwitchColumns", "7");
                hkcuDesktop.SetValue("CoolSwitchRows", "3");
            }

            //Selection Color
            try
            {
                if (keySearch(valueshkcuColors, "HotTrackingColor"))
                {
                    string val = (string)hkcuColors.GetValue("HotTrackingColor");
                    string[] rgb = val.Split(' ');
                    selectionColor = Color.FromArgb(255, byte.Parse(rgb[0]), byte.Parse(rgb[1]), byte.Parse(rgb[2]));
                    rectSelectionColor.Fill = new SolidColorBrush(selectionColor);
                }
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
        
        private void LoadDisplayKeys()
        {
            try
            {
                hkcuWinMet = hkcu.CreateSubKey(@"Control Panel\Desktop\WindowMetrics");
                hkcuDesktop = hkcu.CreateSubKey(@"Control Panel\Desktop");
                hkcuColors = hkcu.CreateSubKey(@"Control Panel\Colors");
                hklmShellIcons = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RenameKeys(string[] keys)
        {
            try
            {
                foreach (string keyName in keys)
                {
                    RegistryKey keyToBeRenamed = hkcrContextMenuHandlers.OpenSubKey(keyName, true);
                    string[] valuesInKeys = keyToBeRenamed.GetValueNames();

                    foreach (string val in valuesInKeys)
                    {
                        string getVal = (string)keyToBeRenamed.GetValue(val);
                        if (getVal.Equals("{C2FBB630-2971-11D1-A18C-00C04FD75D13}")) //copy to
                        {
                            if (!keyToBeRenamed.Name.Equals("Copy To"))
                            {
                                hkcrContextMenuHandlers.DeleteSubKey(keyName);
                                keyToBeRenamed = hkcrContextMenuHandlers.CreateSubKey("Copy To");
                                keyToBeRenamed.SetValue("", "{C2FBB630-2971-11D1-A18C-00C04FD75D13}");
                                break;
                            }
                        }
                        else if (getVal.Equals("{C2FBB631-2971-11D1-A18C-00C04FD75D13}")) //move to
                        {
                            if (!keyToBeRenamed.Name.Equals("Move To"))
                            {
                                hkcrContextMenuHandlers.DeleteSubKey(keyName);
                                keyToBeRenamed = hkcrContextMenuHandlers.CreateSubKey("Move To");
                                keyToBeRenamed.SetValue("", "{C2FBB631-2971-11D1-A18C-00C04FD75D13}");
                                break;
                            }
                        }
                        else if (getVal.Equals("{7BA4C740-9E81-11CF-99D3-00AA004AE837}")) //send to
                        {
                            if (!keyToBeRenamed.Name.Equals("SendTo"))
                            {
                                hkcrContextMenuHandlers.DeleteSubKey(keyName);
                                keyToBeRenamed = hkcrContextMenuHandlers.CreateSubKey("SendTo");
                                keyToBeRenamed.SetValue("", "{7BA4C740-9E81-11CF-99D3-00AA004AE837}");
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void ExamineRightClick()
        {
            try
            {
                string[] subkeyhkcrCntxt = hkcrContextMenuHandlers.GetSubKeyNames();
                string[] valueshklmExAd = hklmExAdvanced.GetValueNames();
                int val = 0;
                RenameKeys(subkeyhkcrCntxt); // check the name of "move to" and "copy to" keys,
                //if different then rename for program to r/w
                subkeyhkcrCntxt = hkcrContextMenuHandlers.GetSubKeyNames();
                if (keySearch(subkeyhkcrCntxt, "Copy To"))
                    chkCopyTo.IsChecked = true;
                else
                    chkCopyTo.IsChecked = false;

                if (keySearch(subkeyhkcrCntxt, "Move To"))
                    chkMoveTo.IsChecked = true;
                else
                    chkMoveTo.IsChecked = false;

                if (keySearch(subkeyhkcrCntxt, "SendTo"))
                    chkSendTo.IsChecked = true;
                else
                    chkSendTo.IsChecked = false;

                chkOpenNotepad.IsChecked = keySearch(hkcrFileShell.GetSubKeyNames(), "Open With Notepad");
                chkCPcommand.IsChecked = keySearch(hkcrShell.GetSubKeyNames(), "Control Panel");
                chkCmdPromp.IsChecked = keySearch(hkcrDirShell.GetSubKeyNames(), "Open Command Prompt here");
                chkDfrg.IsChecked = keySearch(hkcrDriveShell.GetSubKeyNames(), "runas");

                if (keySearch(valueshklmExAd, "EncryptionContextMenu"))
                {
                    val = (int)hklmExAdvanced.GetValue("EncryptionContextMenu");
                    chkEncrNDec.IsChecked = getStatus(val);
                }

                string txtFile = (string)hklmDotTxt.GetValue("");
                hklmTxt = hklmClasses.CreateSubKey(txtFile);
                RegistryKey shell = hklmTxt.OpenSubKey("shell", true);
                string[] subKeysShell = shell.GetSubKeyNames();
                hklmTextFile = hklmClasses.CreateSubKey("textfile");
                shell = hklmTextFile.OpenSubKey("shell", true);
                string[] subKeysShell2 = shell.GetSubKeyNames();
                if (keySearch(subKeysShell, "copycontents") || keySearch(subKeysShell2, "copycontents"))
                    chkClip.IsChecked = true;
                else
                    chkClip.IsChecked = false;
            }
            catch (NullReferenceException)
            {
                chkClip.Visibility = Visibility.Collapsed;
            }
            catch (Exception) { }
        }
        private void tabRightClick_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[3])
            {
                this.Cursor = Cursors.Wait;
                tabOpened[3] = true;
                LoadRightClickKeys();
                ExamineSendTo();
                ExamineRightClick();
                if (osName == WindowsOS.WindowsXP)
                    gbContextProg.Visibility = Visibility.Collapsed;
                this.Cursor = Cursors.Arrow;
            }
        }

        private void LoadRightClickKeys()
        {
            try
            {
                hkcrCPcommand = hkcr.OpenSubKey(@"Directory\Background\shell\Control Panel\command", true);
                hkcrOpenWithNotepad = hkcr.OpenSubKey(@"*\shell\Open With Notepad\command", true);
                hklmExAdvanced = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                hklmClasses = hklm.CreateSubKey(@"Software\Classes");
                hklmDotTxt = hklm.CreateSubKey(@"Software\Classes\.txt");
                hkcrContextMenuHandlers = hkcr.OpenSubKey(@"AllFilesystemObjects\shellex\ContextMenuHandlers", true);
                hkcrShell = hkcr.CreateSubKey(@"Directory\Background\shell");
                hkcrFileShell = hkcr.CreateSubKey(@"*\shell");
                hkcrDirShell = hkcr.CreateSubKey(@"Directory\shell");
                hkcrDriveShell = hkcr.CreateSubKey(@"Drive\shell");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExamineSendTo()
        {
            try
            {
                string sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\SendTo";
                if (osName == WindowsOS.WindowsXP)
                    sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo";
                lstSendTo.ItemsSource = SendTo.LoadSendToItems(sendToPath);
            }
            catch (Exception) { }
        }

        private void ReloadSendToList(string sendToPath)
        {
            lstSendTo.ItemsSource = null;
            lstSendTo.ItemsSource = SendTo.LoadSendToItems(sendToPath);
        }

        private void btnInsrtFoldrSendTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                sendToPath += @"\Microsoft\Windows\SendTo\"; 
                if (osName == WindowsOS.WindowsXP)
                    sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo\";
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    string name = folderBrowserDialog.FileName;
                    int count = 0;
                    for (int i = 0; i < name.Length; i++)
                    {
                        if (name.Contains('\\'))
                            count++;
                    }
                    string[] parentPath = name.Split('\\');
                    string parent = null;
                    for (int i = 0; i < parentPath.Length - 1; i++)
                    {
                        parent += parentPath[i] + "\\";
                    }
                    string folderName = parentPath[parentPath.Length - 1];
                    if (!is64bitMachine)
                    {
                        WshShellClass wshShell = new WshShellClass();
                        IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(sendToPath + folderName + ".lnk");
                        shortcut.TargetPath = name;
                        shortcut.IconLocation = name;
                        shortcut.Save();
                    }
                    else
                    {
                        WshShell wshShell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(sendToPath + folderName + ".lnk");
                        shortcut.TargetPath = name;
                        shortcut.IconLocation = name;
                        shortcut.Save();
                    }
                    ReloadSendToList(sendToPath);
                    MessageBox.Show(folderName + " successfully added to Send To Menu!", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (BadImageFormatException dllerr)
            {
                MessageBox.Show(dllerr.Message + " \n\nHence the \"Insert New File\" and \"Insert New Folder\" options have been disabled.\n\nHowever, you can use the option \"Edit in Explorer\" to customize the Send To Menu.", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                btnInsrtFileSendTo.IsEnabled = btnInsrtFoldrSendTo.IsEnabled = false;
            }
            catch (System.IO.FileNotFoundException fab)
            {
                MessageBox.Show(fab.Message + "\n\nThis means that either the setup was corrupt or this software hasn't been installed correctly", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                btnInsrtFileSendTo.IsEnabled = btnInsrtFoldrSendTo.IsEnabled = false;
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnInsrtFileSendTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\SendTo\";
                if (osName == WindowsOS.WindowsXP)
                    sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo\";
                openFileDialog.Filter = "Executable Files|*.exe|Batch Files|*.bat|COM Files|*.com";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    string fileName = openFileDialog.SafeFileName;
                    int indx = fileName.LastIndexOf('.');
                    fileName = fileName.Remove(indx);
                    if (!is64bitMachine)
                    {
                        WshShellClass wshShell = new WshShellClass();
                        IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(sendToPath + fileName + ".lnk");
                        shortcut.TargetPath = filePath;
                        shortcut.IconLocation = filePath;
                        shortcut.Save();
                    }
                    else
                    {
                        WshShell wshShell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(sendToPath + fileName + ".lnk");
                        shortcut.TargetPath = filePath;
                        shortcut.IconLocation = filePath;
                        shortcut.Save();
                    }
                    ReloadSendToList(sendToPath);
                    MessageBox.Show(fileName + " successfully added to Send To Menu!", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (BadImageFormatException dllerr)
            {
                MessageBox.Show(dllerr.Message + " \n\nHence the \"Insert New File\" and \"Insert New Folder\" options have been disabled.\n\nHowever, you can use the option \"Edit in Explorer\" to customize the Send To Menu.", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                btnInsrtFileSendTo.IsEnabled = btnInsrtFoldrSendTo.IsEnabled = false;
            }
            catch (System.IO.FileNotFoundException fab)
            {
                MessageBox.Show(fab.Message + "\n\nThis means that either the setup was corrupt or this software hasn't been installed correctly", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                btnInsrtFileSendTo.IsEnabled = btnInsrtFoldrSendTo.IsEnabled = false;
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelSendTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                sendToPath += @"\Microsoft\Windows\SendTo\";
                if (osName == WindowsOS.WindowsXP)
                    sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo\";
                ListItem[] item = new ListItem[lstSendTo.SelectedItems.Count];
                lstSendTo.SelectedItems.CopyTo(item, 0);
                if (item.Length == 1)
                {
                    if (MessageBox.Show("Are you sure you want to delete \"" + item[0].Name + "\" from Send To Menu?", "Mind You!!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        if (DelItems(item[0]))
                            ReloadSendToList(sendToPath);
                    }
                }
                else if (item.Length > 1)
                {
                    if (MessageBox.Show("Are you sure you want to delete all these " + item.Length.ToString() + " items from Send To Menu?", "Mind You!!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        for (int i = 0; i < item.Length; i++)
                        {
                            if (DelItems(item[i]))
                                ReloadSendToList(sendToPath);
                        }
                    }
                }
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool DelItems(ListItem item)
        {
            try
            {
                string name = item.Name;
                string sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                sendToPath += @"\Microsoft\Windows\SendTo\";
                if (osName == WindowsOS.WindowsXP)
                    sendToPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo\";
                string fullPath = sendToPath + name + ".lnk";
                FileInfo file = new FileInfo(fullPath);
                if (!file.Exists)
                {
                    fullPath = sendToPath + name + ".pif";
                    file = new FileInfo(fullPath);
                    if (!file.Exists)
                    {
                        MessageBox.Show("Sorry but this is an Important File of your OS, hence you aren\'t allowed to delete this!!", "I\'m So Sowiee!!", MessageBoxButton.OK, MessageBoxImage.Stop);
                        return false;
                    }
                }
                file.Delete();
                return true;
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (IOException io)
            {
                MessageBox.Show(io.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void tabPlaces_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[4])
            {
                tabOpened[4] = true;
                if (osName != WindowsOS.Windows7 && osName != WindowsOS.Windows8)
                    gbFinder.Visibility = Visibility.Collapsed;
                LoadPlacesKeys();
                ExaminePlaces();
            }
        }

        private void LoadPlacesKeys()
        {
            try
            {
                hkcrUnknown = hkcr.CreateSubKey(@"Unknown");
                hkcrNoEnd = hkcr.CreateSubKey(@".");
                hkcuExAdvanced = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExaminePlaces()
        {
            try
            {
                //Default Opening
                RegistryKey shell = hkcrUnknown.CreateSubKey("shell");
                RegistryKey open = shell.OpenSubKey("open", true);
                bool presentEnd = false, presentNoEnd = false;
                if (open != null)
                {
                    RegistryKey cmd = open.OpenSubKey("command", true);
                    if (cmd != null)
                    {
                        presentEnd = true;
                        txtWidEndApp.Text = (string)cmd.GetValue("");
                        rbtnWidEndOpen.IsChecked = false;
                        rbtnWidEndApp.IsChecked = true;
                    }
                }
                else if (!presentEnd)
                {
                    rbtnWidEndOpen.IsChecked = true;
                    rbtnWidEndApp.IsChecked = false;
                }

                if (hkcrNoEnd != null)
                {
                    shell = hkcrNoEnd.OpenSubKey("shell", true);
                    open = shell.OpenSubKey("open", true);
                    if (open != null)
                    {
                        RegistryKey cmd = open.OpenSubKey("command", true);
                        if (cmd != null)
                        {
                            presentNoEnd = true;
                            txtWidNoEndApp.Text = (string)cmd.GetValue("");
                            rbtnWidNoEndOpen.IsChecked = false;
                            rbtnWidNoEndApp.IsChecked = true;
                        }
                    }
                }
                else if (!presentNoEnd)
                {
                    rbtnWidNoEndOpen.IsChecked = true;
                    rbtnWidNoEndApp.IsChecked = false;
                }

                //Power Button
                string[] valueshkcuExAdvanced = hkcuExAdvanced.GetValueNames();
                if (keySearch(valueshkcuExAdvanced, "Start_PowerButtonAction"))
                {
                    int val = (int)hkcuExAdvanced.GetValue("Start_PowerButtonAction");
                    switch (val)
                    {
                        case 2: comboBxPwrBtn.SelectedIndex = 0;
                            break;
                        case 64: comboBxPwrBtn.SelectedIndex = 1;
                            break;
                        case 16: comboBxPwrBtn.SelectedIndex = 2;
                            break;
                        case 4: comboBxPwrBtn.SelectedIndex = 3;
                            break;
                        case 512: comboBxPwrBtn.SelectedIndex = 4;
                            break;
                        case 1: comboBxPwrBtn.SelectedIndex = 5;
                            break;
                        case 256: comboBxPwrBtn.SelectedIndex = 6;
                            break;
                    }
                }
                else
                    comboBxPwrBtn.SelectedIndex = 0;
            
            }
            catch (NullReferenceException)
            {
                RegistryKey shell = hkcrUnknown.CreateSubKey("shell");
                rbtnWidEndOpen.IsChecked = rbtnWidNoEndOpen.IsChecked = true;
            }
            catch (Exception)
            {}
 
        }

        private void tabTasks_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[5])
            {
                dtPkrShutdown.SelectedDate = DateTime.Now;
                dtPkrShutdown.DisplayDateStart = DateTime.Now;
                dtPkrShutdown.DisplayDateEnd = DateTime.Now.AddYears(10);
                nudHour.Value = DateTime.Now.Hour;
                nudMinutes.Value = DateTime.Now.Minute;
                nudSeconds.Value = DateTime.Now.Second;
                tabOpened[5] = true;
            }
        }

        private void tabFeatures_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[6])
            {
                tabOpened[6] = true;
                LoadFeaturesKey();
                ExamineFeatures();
                if (osName == WindowsOS.WindowsXP)
                    lblWinUpdt.Visibility = Visibility.Collapsed;
            }
        }

        private void ExamineFeatures()
        {
            try
            {
                int val = 0;
                string[] valueshkcuExplorer = hkcuExplorer.GetValueNames();
                string[] valueshklmCdRom = hklmCdRom.GetValueNames();
                string[] valueshkcuAutoHandle = hkcuAutoplayHandlers.GetValueNames();

                if (keySearch(valueshkcuExplorer, "NoDriveTypeAutoRun") && keySearch(valueshkcuAutoHandle, "DisableAutoplay") && keySearch(valueshklmCdRom, "AutoRun"))
                {
                     val = (int)hkcuExplorer.GetValue("NoDriveTypeAutoRun");
                    int val2 = (int)hkcuAutoplayHandlers.GetValue("DisableAutoplay");
                    int val3 = (int)hklmCdRom.GetValue("AutoRun");
                    if (val2 == 1 || val3 == 0)
                        rbtnOffAuto.IsChecked = true;
                    else
                    {
                        switch (val)
                        {
                            case 145: rbtnEnableAuto.IsChecked = true;
                                break;
                            case 181: rbtnEnableCDUsbAuto.IsChecked = true;
                                break;
                            case 255: rbtnOffAuto.IsChecked = true;
                                break;
                        }
                    }
                }
                else if (!keySearch(valueshkcuExplorer, "NoDriveTypeAutoRun") && keySearch(valueshkcuAutoHandle, "DisableAutoplay") && keySearch(valueshklmCdRom, "AutoRun"))
                {
                    int val2 = (int)hkcuAutoplayHandlers.GetValue("DisableAutoplay");
                    int val3 = (int)hklmCdRom.GetValue("AutoRun");
                    if (val2 == 1 || val3 == 0)
                        rbtnOffAuto.IsChecked = true;
                    else
                        rbtnEnableAuto.IsChecked = true;
                }
                else if (!keySearch(valueshkcuExplorer, "NoDriveTypeAutoRun") && !keySearch(valueshkcuAutoHandle, "DisableAutoplay") && keySearch(valueshklmCdRom, "AutoRun"))
                {
                    int val3 = (int)hklmCdRom.GetValue("AutoRun");
                    if (val3 == 1)
                        rbtnEnableAuto.IsChecked = true;
                    else
                        rbtnOffAuto.IsChecked = true;
                }

                //Windows Update

                string[] valueshklmAutoUpdt = hklmAutoUpdate.GetValueNames();
                
                if (keySearch(valueshklmAutoUpdt, "AUOptions"))
                {
                    val = (int)hklmAutoUpdate.GetValue("AUOptions");
                    switch (val)
                    {
                        case 1: comboBoxWinUpdt.SelectedIndex = 3;
                            break;
                        case 2: comboBoxWinUpdt.SelectedIndex = 2;
                            break;
                        case 3: comboBoxWinUpdt.SelectedIndex = 1;
                            break;
                        case 4: comboBoxWinUpdt.SelectedIndex = 0;
                            break;
                    }
                }
                else
                {
                    String itm = "Please choose an option:";
                    comboBoxWinUpdt.Items.Add((Object)itm);
                    comboBoxWinUpdt.SelectedItem = (Object)itm;
                }

                if (keySearch(valueshklmAutoUpdt, "IncludeRecommendedUpdates"))
                {
                    val = (int)hklmAutoUpdate.GetValue("IncludeRecommendedUpdates");
                    chkRecomedUpdts.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmAutoUpdt, "ElevateNonAdmins"))
                {
                    val = (int)hklmAutoUpdate.GetValue("ElevateNonAdmins");
                    chkWinUpdtInstalEle.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmAutoUpdt, "EnableFeaturedSoftware"))
                {
                    val = (int)hklmAutoUpdate.GetValue("EnableFeaturedSoftware");
                    chkDetailNotify.IsChecked = getStatus(val);
                }

                //System beeps
                string[] valueshkcuSound = hkcuSound.GetValueNames();
                if (keySearch(valueshkcuSound, "Beep"))
                {
                    string valStr = (string)hkcuSound.GetValue("Beep");
                    if (valStr.Equals("yes"))
                        chkBeeps.IsChecked = false;
                    else if (valStr.Equals("no"))
                        chkBeeps.IsChecked = true;
                    else
                    {
                        MessageBox.Show("An error was detected in your registry.\n\n However, it has beed fixed now!", "You can trust me :)", MessageBoxButton.OK, MessageBoxImage.Warning);
                        chkBeeps.IsChecked = false;
                        hkcuSound.SetValue("Beep", "yes");
                    }
                }

                // DVD burning feature
                if (keySearch(valueshkcuExplorer, "NoCDBurning"))
                {
                    val = (int)hkcuExplorer.GetValue("NoCDBurning");
                    chkDVDburn.IsChecked = !getStatus(val);
                }
                else
                    chkDVDburn.IsChecked = true;
            }
            catch (NullReferenceException ne1)
            {
                MessageBox.Show("Unable to read Values:\n" + ne1.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFeaturesKey()
        {
            try
            {
                if (hkcuExplorer == null)
                    hkcuExplorer = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
                hkcuAutoplayHandlers = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\AutoplayHandlers");
                hklmCdRom = hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\cdrom");
                hklmAutoUpdate = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update");
                hkcuSound = hkcu.CreateSubKey(@"Control Panel\Sound");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tabLogon_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[7])
            {
                LoadLogonKeys();
                ExamineLogon();
                tabOpened[7] = true;
            }
        }

        private void ExamineLogon()
        { 
            try
            {
                int iVal = 0;

                //Auto Logon
                string[] valueshklmWinLogon = hklmWinLogon.GetValueNames();
                if (keySearch(valueshklmWinLogon, "AutoAdminLogon"))
                {
                    string val = (string)hklmWinLogon.GetValue("AutoAdminLogon");
                    if (val.Equals("0"))
                        chkAutoLogin.IsChecked = false;
                    else if (val.Equals("1"))
                    {
                        chkAutoLogin.IsChecked = true;
                        txtAutoUsrNm.Text = (string)hklmWinLogon.GetValue("DefaultUserName");
                        txtAutoPaswrd.Password = (string)hklmWinLogon.GetValue("DefaultPassword");
                        txtAutoDomain.Text = (string)hklmWinLogon.GetValue("DefaultDomainName");
                    }
                }
                if (keySearch(valueshklmWinLogon, "IgnoreShiftOverride"))
                {
                    string val = (string)hklmWinLogon.GetValue("IgnoreShiftOverride");
                    chkShift.IsChecked = getStrStatus(val);
                }
                if (keySearch(valueshklmWinLogon, "ForceAutoLogon"))
                {
                    string val = (string)hklmWinLogon.GetValue("ForceAutoLogon");
                    chkLogOffAuto.IsChecked = getStrStatus(val);
                }

                //Startup Sound

                string[] valuesBootAnim = hklmBootAnimation.GetValueNames();
                if (keySearch(valuesBootAnim, "DisableStartupSound"))
                {
                    iVal = (int)hklmBootAnimation.GetValue("DisableStartupSound");
                    chkStartupSnd.IsChecked = !getStatus(iVal);
                }
                else
                    chkStartupSnd.IsChecked = true;

                //Screen Saver

                string[] valueshkcuPDesktop = hkcuPDesktop.GetValueNames();
                if (keySearch(valueshkcuPDesktop, "ScreenSaverIsSecure"))
                {
                     iVal = (int)hkcuPDesktop.GetValue("ScreenSaverIsSecure");
                    chkScreenSvrPol.IsChecked = getStatus(iVal);
                }

                //Require ctrl_alt_del

                string[] valueshklmSystem = hklmSystem.GetValueNames();
                if (keySearch(valueshklmWinLogon, "DisableCAD"))
                {
                    iVal = (int)hklmWinLogon.GetValue("DisableCAD");
                    chkCAD.IsChecked = !getStatus(iVal);
                }
                if (keySearch(valueshklmSystem, "dontdisplaylastusername"))
                {
                    iVal = (int)hklmSystem.GetValue("dontdisplaylastusername");
                    chkLastUsrName.IsChecked = getStatus(iVal);
                }
                if (keySearch(valueshklmSystem, "shutdownwithoutlogon"))
                {
                    iVal = (int)hklmSystem.GetValue("shutdownwithoutlogon");
                    chkShutDwnLogOn.IsChecked = getStatus(iVal);
                }

                //Message

                if (IntPtr.Size == 8) 
                {
                    imgLogOnMsg.Visibility = Visibility.Visible;
                }
                string content = "", caption = "";
                if (keySearch(valueshklmSystem, "LegalNoticeCaption"))
                    caption = (string)hklmSystem.GetValue("LegalNoticeCaption");

                if (keySearch(valueshklmSystem, "LegalNoticeText"))
                    content = (string)hklmSystem.GetValue("LegalNoticeText");

                if (content.Length > 0 || caption.Length > 0)
                {
                    chkLegalNotice.IsChecked = true;
                    txtLogHeader.Text = caption;
                    txtContent.Text = content;
                }
                else
                {
                    chkLegalNotice.IsChecked =  false;
                    txtLogHeader.Text = txtContent.Text = "";
                }
            }
            catch (NullReferenceException autex) { MessageBox.Show("Unable to read Values:\n" + autex.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception) { }
        }

        private void LoadLogonKeys()
        {
            try
            {
                hklmWinLogon = hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon");
                hklmBootAnimation = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Authentication\LogonUI\BootAnimation");
                hkcuPDesktop = hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\Control Panel\Desktop");
                hklmSystem = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tabRestrictions_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[8])
            {
                if (osName == WindowsOS.WindowsXP)
                {
                    chkFlip3D.Visibility = Visibility.Collapsed;
                    chkTaskIconSize.Visibility = Visibility.Collapsed;
                }
                LoadRestrictionsKeys();
                ExamineRestriction();
                tabOpened[8] = true;
            }
        }

        private void ExamineRestriction()
        {
            try
            {
                string[] valueshkcuEx = hkcuExplorer.GetValueNames();
                string[] valueshklmSys = hklmSystem.GetValueNames();
                string[] valueshkcuWinMet = hkcuWinMet.GetValueNames();
                string[] valueshkcuExAdvanced = hkcuExAdvanced.GetValueNames();
                string[] valueshkcuDesktop = hkcuDesktop.GetValueNames();
                string[] valueshkcuDWM = null;
                if (hkcuDWM != null)
                    valueshkcuDWM = hkcuDWM.GetValueNames();
                string[] valueshkcuExplorer = hkcuExplorer.GetValueNames();
                string[] valueshklmSystem = hklmSystem.GetValueNames();
                string[] valueshkcuSystem = hkcuSystem.GetValueNames();
                string[] valueshklmParams = hklmParams.GetValueNames();
                int val = 0;
                //Explorer
                if (keySearch(valueshklmSys, "NoDispScrSavPage"))
                {
                    val = (int)hklmSystem.GetValue("NoDispScrSavPage");
                    chkScreenSvr.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmSys, "NoDispAppearancePage"))
                {
                    val = (int)hklmSystem.GetValue("NoDispAppearancePage");
                    chkAppearance.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuEx, "NoViewContextMenu"))
                {
                    val = (int)hkcuExplorer.GetValue("NoViewContextMenu");
                    chkContext.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuEx, "NoFolderOptions"))
                {
                    val = (int)hkcuExplorer.GetValue("NoFolderOptions");
                    chkFolderOp.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuEx, "NoFileMenu"))
                {
                    val = (int)hkcuExplorer.GetValue("NoFileMenu");
                    chkFile.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExAdvanced, "DisableThumbnailCache"))
                {
                    val = (int)hkcuExAdvanced.GetValue("DisableThumbnailCache");
                    chkThumbCache.IsChecked = getStatus(val);
                }

                if (osName != WindowsOS.WindowsXP)
                {
                    if (keySearch(valueshkcuDWM, "DisallowFlip3D"))
                    {
                        val = (int)hkcuDWM.GetValue("DisallowFlip3D");
                        chkFlip3D.IsChecked = getStatus(val);
                    }
                    else
                        chkFlip3D.IsChecked = false;
                }

                //StartMenu
                if (keySearch(valueshkcuExplorer, "NoClose"))
                {
                    val = (int)hkcuExplorer.GetValue("NoClose");
                    chkShutdown.IsChecked = getStatus(val);
                }
                if (keySearch(valueshkcuExplorer, "NoChangeStartMenu"))
                {
                    val = (int)hkcuExplorer.GetValue("NoChangeStartMenu");
                    chkStartChnge.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoRecentDocsMenu"))
                {
                    val = (int)hkcuExplorer.GetValue("NoRecentDocsMenu");
                    chkRecentDoc.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoLogoff"))
                {
                    val = (int)hkcuExplorer.GetValue("NoLogoff");
                    chkLogOff.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmSystem, "NoDispCPL"))
                {
                    val = (int)hklmSystem.GetValue("NoDispCPL");
                    chkControlPanel.IsChecked = getStatus(val);
                }

                //System

                if (keySearch(valueshkcuExplorer, "DisableRegistryTools"))
                {
                    val = (int)hkcuExplorer.GetValue("DisableRegistryTools");
                    chkRegedit.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmSystem, "NoVirtMemPage"))
                {
                    val = (int)hklmSystem.GetValue("NoVirtMemPage");
                    chkVirtMem.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoWindowsUpdate"))
                {
                    val = (int)hkcuExplorer.GetValue("NoWindowsUpdate");
                    chkWinUpdt.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoDeletePrinter"))
                {
                    val = (int)hkcuExplorer.GetValue("NoDeletePrinter");
                    chkPrinterDel.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoAddPrinter"))
                {
                    val = (int)hkcuExplorer.GetValue("NoAddPrinter");
                    chkPrinterAdd.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuSystem, "DisableTaskMgr"))
                {
                    val = (int)hkcuSystem.GetValue("DisableTaskMgr");
                    chkTaskMgr.IsChecked = getStatus(val);
                }

                if (keySearch(valueshkcuExplorer, "NoPropertiesMyComputer"))
                {
                    val = (int)hkcuExplorer.GetValue("NoPropertiesMyComputer");
                    chkMyCompProp.IsChecked = getStatus(val);
                }

                if (keySearch(valueshklmParams, "AutoShareWks"))
                {
                    val = (int)hklmParams.GetValue("AutoShareWks");
                    chkAdminShare.IsChecked = !getStatus(val);
                }

                //Taskbar
                if (keySearch(valueshkcuExAdvanced, "TaskbarAnimations"))
                {
                    val = (int)hkcuExAdvanced.GetValue("TaskbarAnimations");
                    chkTaskbarAnim.IsChecked = !getStatus(val);
                }

                if (osName != WindowsOS.WindowsXP)
                {
                    if (keySearch(valueshkcuExAdvanced, "TaskbarSmallIcons"))
                    {
                        val = (int)hkcuExAdvanced.GetValue("TaskbarSmallIcons");
                        chkTaskIconSize.IsChecked = getStatus(val);
                    }
                }
            }
            catch (NullReferenceException nullEx) { MessageBox.Show("Unable to read Values:\n" + nullEx.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error); }
            catch (Exception) { }
        }

        private void LoadRestrictionsKeys()
        {
            try
            {
                hkcuExplorer = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\Explorer");
                if (osName != WindowsOS.WindowsXP)
                    hkcuDWM = hkcu.CreateSubKey(@"Software\Policies\Microsoft\Windows\DWM");
                hklmSystem = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                hkcuWinMet = hkcu.CreateSubKey(@"Control Panel\Desktop\WindowMetrics");
                hkcuExAdvanced = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");
                hkcuDesktop = hkcu.CreateSubKey(@"Control Panel\Desktop");
                hkcuSystem = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                hklmParams = hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\services\LanmanServer\Parameters");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tabMaintenance_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[9])
            {
                tabOpened[9] = true;
                LoadMaintenanceKeys();
                ExamineMaintenance();
            }
        }

        private void ExamineMaintenance()
        {
            try
            {
                //Memory

                int val = 0;
                string[] valueshklmCVExplorer = hklmCVExplorer.GetValueNames();
                string[] valueshklmSystem = hklmSystem.GetValueNames();
                if (keySearch(valueshklmCVExplorer, "AlwaysUnloadDLL"))
                {
                    val = (int)hklmCVExplorer.GetValue("AlwaysUnloadDLL");
                    chkUnloadDLL.IsChecked = getStatus(val);
                }
                if (keySearch(valueshklmSystem, "SynchronousMachineGroupPolicy") && keySearch(valueshklmSystem, "SynchronousUserGroupPolicy"))
                {
                    val = (int)hklmSystem.GetValue("SynchronousMachineGroupPolicy");
                    int val2 = (int)hklmSystem.GetValue("SynchronousUserGroupPolicy");
                    chkGPUpdate.IsChecked = !getStatus(val) && !getStatus(val2);
                }

                //Auto Reboot

                string[] valueshklmCrashControl = hklmCrashControl.GetValueNames();
                string[] valueshkcuWinErrReporting = hkcuWinErrReporting.GetValueNames();
                string[] valueshkcuConsent = hkcuConsent.GetValueNames();
               
                if (keySearch(valueshklmCrashControl, "AutoReboot"))
                {
                    val = (int)hklmCrashControl.GetValue("AutoReboot");
                    if (val == 0)
                        comboBxAutoReboot.SelectedIndex = 1;
                    else
                        comboBxAutoReboot.SelectedIndex = 0;
                }
                if (keySearch(valueshklmCrashControl, "LogEvent"))
                {
                    val = (int)hklmCrashControl.GetValue("LogEvent");
                    chkRecEvent.IsChecked = getStatus(val);
                }

                //Error Reporting

                if (keySearch(valueshkcuWinErrReporting, "DontShowUI"))
                {
                    val = (int)hkcuWinErrReporting.GetValue("DontShowUI");
                    chkErrDialog.IsChecked = getStatus(val);
                }
                if ((keySearch(valueshkcuConsent, "DefaultConsent") && keySearch(valueshkcuWinErrReporting, "Disabled")))
                {
                    val = (int)hkcuWinErrReporting.GetValue("Disabled");
                    if (val == 0)
                        setErrorReportComboBox();
                    else
                        comboBoxErrType.SelectedIndex = 3;
                }
                if ((keySearch(valueshkcuConsent, "DefaultConsent") && !keySearch(valueshkcuWinErrReporting, "Disabled")))
                {
                    setErrorReportComboBox();
                }
                else if ((!keySearch(valueshkcuConsent, "DefaultConsent") && !keySearch(valueshkcuWinErrReporting, "Disabled")))
                {
                    comboBoxErrType.SelectedIndex = 1;
                }

                //Startup maintenance
                string[] valueshklmWinLogon = hklmWinLogon.GetValueNames();
                string[] valueshklmBootOptFunc = hklmBootOptFunc.GetValueNames();
                string[] valueshklmSSMgr = hklmSSMgr.GetValueNames();
                if (keySearch(valueshklmWinLogon, "ReportBootOk"))
                {
                    string vals = (string)hklmWinLogon.GetValue("ReportBootOk");
                    chkLKGC.IsChecked = !getStrStatus(vals);
                }

                if (keySearch(valueshklmBootOptFunc, "Enable"))
                {
                    string vals = (string)hklmBootOptFunc.GetValue("Enable");
                    if (vals.Equals("Y"))
                        chkBootDfrg.IsChecked = true;
                    else
                        chkBootDfrg.IsChecked = false;
                }
                else
                    chkBootDfrg.IsChecked = true;
                try
                {
                    if (keySearch(valueshklmSSMgr, "AutoChkTimeOut"))
                    {
                         val = (int)hklmSSMgr.GetValue("AutoChkTimeOut");
                        nudHDScnTym.Value = (int)val;
                    }
                    else
                        nudHDScnTym.Value = 10;
                }
                catch (ArgumentOutOfRangeException)
                {
                    hklmSSMgr.SetValue("AutoChkTimeOut", 10);
                    nudHDScnTym.Value = 10;
                }
            }
            catch (NullReferenceException nes)
            {
                MessageBox.Show("Unable to read Values:\n" + nes.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void setErrorReportComboBox()
        {
            try
            {
                int val = (int)hkcuConsent.GetValue("DefaultConsent");
                comboBoxErrType.SelectedIndex = val - 1;
            }
            catch (Exception)
            {
                hkcuConsent.SetValue("DefaultConsent", 2);
                comboBoxErrType.SelectedIndex = 1;
            }
        }

        private void LoadMaintenanceKeys()
        {
            try
            {
                hklmSystem = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
                hklmCVExplorer = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer");
                hklmCrashControl = hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\CrashControl");
                hkcuWinErrReporting = hkcu.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting");
                hkcuConsent = hkcu.CreateSubKey(@"Software\Microsoft\Windows\Windows Error Reporting\Consent");
                hklmWinLogon = hklm.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Winlogon");
                hklmBootOptFunc = hklm.CreateSubKey(@"Software\Microsoft\Dfrg\BootOptimizeFunction");
                hklmSSMgr = hklm.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void tabUtilities_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!tabOpened[10])
            {
                tabOpened[10] = true;
            }
        }

        private bool keySearch(string[] keyArr, string itm)
        {
            for (int i = 0; i < keyArr.Length; i++)
            {
                string tmp = keyArr[i];
                if (tmp.Equals(itm))
                    return true;
            }
            return false;
        }

        private bool getStatus(int val)
        {
            return val == 0 ? false : true;
        }
        private bool getStrStatus(string str)
        {
            return str.Equals("1") ? true : false;
        }

        private void enableMainButtonsByCheckboxes(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!btnEnabled)
                {
                    btnOk.IsEnabled = true;
                    btnApply.IsEnabled = true;
                    btnEnabled = true;
                }
                CheckBox checkBox = (CheckBox)sender;
                if (checkBox.Name.Equals("chkBootDfrg") && checkBox.IsChecked == false)
                {
                    if (MessageBox.Show("This is a highly Sensitive Tweak.\nProceed at your own risk!", "Warning!!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        checkBox.IsChecked = false;
                    else
                        checkBox.IsChecked = true;
                }
                else if ((checkBox.Name.Equals("chkContext") || checkBox.Name.Equals("chkFolderOp") || checkBox.Name.Equals("chkLogOff") || checkBox.Name.Equals("chkShutdown") || checkBox.Name.Equals("chkControlPnl") || checkBox.Name.Equals("chkRegEdit") || checkBox.Name.Equals("chkTaskMgr") || checkBox.Name.Equals("chkWinInstalr") || checkBox.Name.Equals("chkLKGC")) && checkBox.IsChecked == true)
                {
                    if (MessageBox.Show("This is a highly Sensitive Tweak.\nProceed at your own risk!", "Warning!!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                        checkBox.IsChecked = true;
                    else
                        checkBox.IsChecked = false;
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lnkSenToEx_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\SendTo");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                if (osName == WindowsOS.WindowsXP)
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SendTo");
                else
                    MessageBox.Show("The SendTo path is different in your Windows than expected :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void enableOkANDapplyBtnsBYRadioBtns(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!btnEnabled)
            {
                btnOk.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnEnabled = true;
            }
        }

        private void toggleBalloonTipChkBox(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!chkNotificationArea.IsChecked==true)
            {
                chkBalloonTip.IsEnabled = false;
            }
            else if (!chkBalloonTip.IsEnabled)
                chkBalloonTip.IsEnabled = true;
        }

        private void btnLnkGpEdt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("gpedit.msc");
            }

            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void enableOkAndApplyBtnsByComboBoxes(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ComboBox comboBx = (ComboBox)sender;
            if (comboBx.Name.Equals("comboBxPwrBtn"))
                comboLstEvntRec[0]++;
            else if (comboBx.Name.Equals("comboBoxWinUpdt"))
                comboLstEvntRec[1]++;
            else if (comboBx.Name.Equals("comboBxAutoReboot"))
                comboLstEvntRec[2]++;
            else if (comboBx.Name.Equals("comboBoxErrType"))
                comboLstEvntRec[3]++;
            if (!btnEnabled && (comboLstEvntRec[0] >= 2 || comboLstEvntRec[1] >= 2 || comboLstEvntRec[2] >= 2 || comboLstEvntRec[3] >= 2))
            {
                btnOk.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnEnabled = true;
            }
        }

        private void comboBoxWinUpdt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboLstEvntRec[1]++;
            switch (comboBoxWinUpdt.SelectedIndex)
            {
                case 0: imgWinUpdate.Source = new BitmapImage(new Uri("Images/yess.ico", UriKind.Relative));
                    break;
                case 1: imgWinUpdate.Source = new BitmapImage(new Uri("Images/ok.ico", UriKind.Relative));
                    break;
                case 2: imgWinUpdate.Source = new BitmapImage(new Uri("Images/ok.ico", UriKind.Relative));
                    break;
                case 3: imgWinUpdate.Source = new BitmapImage(new Uri("Images/noo.ico", UriKind.Relative));
                    break;
                default: imgWinUpdate.Source = null;
                    break;
            }
        }

        private void lnkAdDskCln_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("cleanmgr.exe", "/sageset:65535 & /sagerun:65535");
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void StartProcess(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(((Hyperlink)sender).Tag.ToString());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                if (((Hyperlink)sender).Tag.ToString().Contains("SystemPropertiesProtection"))
                {
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Windows) + @"\system32\Restore\rstrui.exe");
                }
                else
                    MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception) { }
        }

        private void lnkRemoteShutdown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo shutProc = new ProcessStartInfo("shutdown.exe", "/i");
                shutProc.RedirectStandardOutput = true;
                shutProc.UseShellExecute = false;
                shutProc.CreateNoWindow = true;
                Process shutdown = new Process();
                shutdown.StartInfo = shutProc;
                shutdown.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("This option is not available in your version of Windows :(", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void chkAutoLogin_StateChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            gbAutoLogin.IsEnabled = (bool)chkAutoLogin.IsChecked;
            if (gbAutoLogin.IsEnabled)
            {
                txtAutoUsrNm.Text = Environment.UserName;
                txtAutoDomain.Text = Environment.UserDomainName;
            }
        }

        private void ToggleLegalNoticeTextBoxes(object sender, System.Windows.RoutedEventArgs e)
        {
            txtLogHeader.IsEnabled = txtContent.IsEnabled = (bool)chkLegalNotice.IsChecked;
        }

        private void imgLogOnMsg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Due to some Virtualization issues of 32-bit applications on 64-bit systems, you may experience a difficulty with this tweak.\n After saving the message here, you will see the message on the Log On screen, but when you will open this section during the next run of this program, it won\'t be able to read that message from your registry, though the message will still appear on your Log On screen.\nThis is due to virtualization of 32-bit applications on 64-bit systems.\n So, if you wish to edit the already saved message you will have to type and save a new message here.\n However, in case you wish to remove the saved message, just\n\t1. Tick the \"Show a Message Before User Logs on\"\n\t2. Now, Untick the \"Show a Message Before User Logs on\", \n  to remove the Log On message.\n\nPlease note that this small tweak would NOT harm your registry, so you can fearlessly try this tweak.\n\nSpecial thanks to our customer Mr. Rich for reporting this issue. ", "Caution for 64-bit systems!!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void btnLnkFoldrHide_Click(object sender, RoutedEventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == true)
            {
                try
                {
                    string fldr = folderBrowserDialog.FileName;
                    string hideComd = String.Format("attrib +h +s \"{0}\"", fldr);
                    executeCmd(hideComd);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnLnkFileHide_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "All Files|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] files = openFileDialog.FileNames;
                    foreach (string file in files)
                    {
                        string hideComd = String.Format("attrib +h +s \"{0}\"", file);
                        executeCmd(hideComd);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnLnkFoldrUnhide_Click(object sender, RoutedEventArgs e)
        {
           if (folderBrowserDialog.ShowDialog() == true)
            {
                try
                {
                    string fldr = folderBrowserDialog.FileName;
                    string unHideComd = String.Format("attrib -h -s \"{0}\"", fldr);
                    executeCmd(unHideComd);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnLnkFileUnhide_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "All Files|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string[] files = openFileDialog.FileNames;
                    foreach (string file in files)
                    {
                        string unHideComd = String.Format("attrib -h -s \"{0}\"", file);
                        executeCmd(unHideComd);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnSpclFoldrNm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                folderBrowserDialog.Title = "Select a Parent Folder";
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    string parentPath = folderBrowserDialog.FileName;
                    string createCmd = String.Format("md \"\\\\.\\{0}\\{1}\"", parentPath, comboBxSpclFoldrNm.SelectionBoxItem);
                    executeCmd(createCmd);
                    MessageBox.Show(comboBxSpclFoldrNm.SelectionBoxItem + " created Successfully at " + parentPath, "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSpclFoldrNmDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    string path = folderBrowserDialog.FileName;
                    string createCmd = String.Format("rd \"\\\\.\\{0}\"", path);
                    executeCmd(createCmd);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void executeCmd(string comd)
        {
            ProcessStartInfo hideProc = new ProcessStartInfo("cmd", "/c " + comd);
            hideProc.RedirectStandardOutput = true;
            hideProc.UseShellExecute = false;
            hideProc.CreateNoWindow = true;
            Process proc = new Process();
            proc.StartInfo = hideProc;
            proc.Start();
        }

        private void btnShowPopup_Click(object sender, RoutedEventArgs e)
        {
            popOptions.IsOpen = true;
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void btnPassword_Click(object sender, RoutedEventArgs e)
        {           
            string rv = PasswdInRegsitry.ReadPasswd();
            if (rv != null)
            {
                PasswordPrompt passwordPrompt = new PasswordPrompt(); 
                passwordPrompt.ShowDialog();
                if (passwordPrompt.IsAuthorised)
                {
                    PasswordConfig passwordConfig = new PasswordConfig();
                    passwordConfig.ShowDialog();
                }
                passwordPrompt.Close();
            }
            else
            {
                PasswordConfig passwordConfig = new PasswordConfig();
                passwordConfig.ShowDialog();
            }
        }

        private void btnWidEndApp_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "Executable Files|*.exe|Batch Files|*.bat|Command Files|*.com;*.cmd|Jar Files|*.jar";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
                txtWidEndApp.Text = openFileDialog.FileName + " \"%1\"";        
        }

        private void btnWidNoEndApp_Click(object sender, RoutedEventArgs e)
        {
            openFileDialog.Filter = "Executable Files|*.exe|Batch Files|*.bat|Command Files|*.com;*.cmd|Jar Files|*.jar";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == true)
                txtWidNoEndApp.Text = openFileDialog.FileName + " \"%1\"";        
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    string name = folderBrowserDialog.FileName;
                    DirectoryInfo finder = new DirectoryInfo(name);
                    string[] parentPath = name.Split('\\');
                    string parent = null;
                    for (int i = 0; i < parentPath.Length - 1; i++)
                    {
                        parent += parentPath[i] + "\\";
                    }
                    string folderName = parentPath[parentPath.Length - 1];
                    string folder = folderName;
                    folderName += ".{ED7BA470-8E54-465E-825C-99712043E01C}";
                    finder.Delete(true);
                    DirectoryInfo finderFolder = new DirectoryInfo(parent);
                    finderFolder.CreateSubdirectory(folderName);
                    MessageBox.Show("Folder " + folder + " created in " + parent + "\n\nPlease note that if on clicking the Finder folder you get an error, then you need to refresh that window for changes to be reflected.", "Mission Accomplished!!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error); }        
        }

        private void btnOpenWithCustomize_Click(object sender, RoutedEventArgs e)
        {
            OpenWith openWithBox = new OpenWith();
            openWithBox.ShowDialog();
        }

        private void enableOkAndApplyBtnBYTextBoxes(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
        	TextBox tb = (TextBox)sender;
            if (tb.Name.Equals("txtLogHeader"))
                varTbEventRecord[0]++;
            else if (tb.Name.Equals("txtContent"))
                varTbEventRecord[1]++;
            if (tb.Name.Equals("txtOwnerName"))
                varTbEventRecord[2]++;
            else if (tb.Name.Equals("txtCompanyName"))
                varTbEventRecord[3]++;
            else if (tb.Name.Equals("txtProductID"))
                varTbEventRecord[4]++;
            else if (tb.Name.Equals("txtManufacturer"))
                varTbEventRecord[5]++;
            else if (tb.Name.Equals("txtModel"))
                varTbEventRecord[6]++;
            else if (tb.Name.Equals("txtSupportPh"))
                varTbEventRecord[7]++;
            else if (tb.Name.Equals("txtSupportUrl"))
                varTbEventRecord[8]++;
            else if (tb.Name.Equals("txtWidEndApp"))
                varTbEventRecord[9]++;
            else if (tb.Name.Equals("txtWidNoEndApp"))
                varTbEventRecord[10]++;
            else if (tb.Name.Equals("txtAutoUsrNm"))
                varTbEventRecord[11]++;
            else if (tb.Name.Equals("txtAutoDomain"))
                varTbEventRecord[12]++;
            if (!btnEnabled && (varTbEventRecord[0] >= 2 || varTbEventRecord[1] >= 2 || varTbEventRecord[2] >= 2 || varTbEventRecord[3] >= 2 || varTbEventRecord[4] >= 2 || varTbEventRecord[5] >= 2 || varTbEventRecord[6] >= 2 || varTbEventRecord[7] >= 2 || varTbEventRecord[8] >= 2
                || varTbEventRecord[9] >= 2 || varTbEventRecord[10] >= 2 || varTbEventRecord[11] >= 2 || varTbEventRecord[12] >= 2))
            {
                btnOk.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnEnabled = true;
            }
        }

        private void enableButtonsByNUD(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Xceed.Wpf.Toolkit.IntegerUpDown nud = sender as Xceed.Wpf.Toolkit.IntegerUpDown;
            if (nud.Name.Equals("nudTime"))
                varNUDEventRecord[0]++;
            else if (nud.Name.Equals("nudAltTabIcons"))
                varNUDEventRecord[1]++;
            else if (nud.Name.Equals("nudAltTabRows"))
                varNUDEventRecord[2]++;
            else if (nud.Name.Equals("nudHDScnTym"))
                varNUDEventRecord[3]++;
            if (!btnEnabled && (varNUDEventRecord[0] >= 3 || varNUDEventRecord[1] >= 3 || varNUDEventRecord[2] >= 3 || varNUDEventRecord[3] >= 3))
            {
                btnOk.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnEnabled = true;
            }
        }

        private void btnShowColor_Click(object sender, RoutedEventArgs e)
        {
            ShowColorDialog();
        }

        private void ShowColorDialog()
        {
            colorDialog = new ColorPickerDialog(selectionColor);
            if (colorDialog.ShowDialog() == true)
            {
                selectionColor = colorDialog.SelectedColour;
                rectSelectionColor.Fill = new SolidColorBrush(selectionColor);
                if (!btnEnabled)
                {
                    btnOk.IsEnabled = true;
                    btnApply.IsEnabled = true;
                    btnEnabled = true;
                }
            }
        }

        private void btnDefSelColor_Click(object sender, RoutedEventArgs e)
        {
            if (!btnEnabled)
            {
                btnOk.IsEnabled = true;
                btnApply.IsEnabled = true;
                btnEnabled = true;
            }
            selectionColor = Color.FromArgb(255, 0, 102, 204);
            rectSelectionColor.Fill = new SolidColorBrush(selectionColor);
        }

        private void rectSelectionColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowColorDialog();
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            string param = null;
            switch (comboBxShutTym.SelectedIndex)
            {
                case 0: param = "/s";
                    break;
                case 1: param = "/r";
                    break;
            }
            try
            {
                DateTime dateSel = (DateTime)dtPkrShutdown.SelectedDate;
                DateTime endDate = new DateTime(dateSel.Year, dateSel.Month, dateSel.Day, (int)nudHour.Value, (int)nudMinutes.Value, (int)nudSeconds.Value);
                DateTime nowTime = DateTime.Now;
                TimeSpan gap = endDate.Subtract(nowTime);
                long timeout = (gap.Days * 86400) + (gap.Hours * 3600) + (gap.Minutes * 60) + gap.Seconds;
                if (timeout >= 0)
                {
                    string shutDwnComd = String.Format("shutdown {0} /t {1}", param, timeout.ToString());
                    executeCmd(shutDwnComd);
                }
                else
                {
                    MessageBox.Show("Invalid Timeout!\n The time can\'t be less than the current time. Also the time can\'t exceed the limit of 10 years.", "Mind You!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelShut_Click(object sender, RoutedEventArgs e)
        {
            executeCmd("shutdown /a");
        }

        private void btnStartupMgr_Click(object sender, RoutedEventArgs e)
        {
            StartupManager startupMgr = new StartupManager();
            startupMgr.ShowDialog();
        }

        private void btnAddRightItem_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuShortcutCreator creator = new ContextMenuShortcutCreator();
            creator.ShowDialog();
        }

        private void btnRmvCust_Click(object sender, RoutedEventArgs e)
        {
            RemoveContextMenuItem remove = new RemoveContextMenuItem();
            remove.ShowDialog();
        }

        private void btnCreateShortcut_Click(object sender, RoutedEventArgs e)
        {
            string ShortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\";
            string shutdownTarget = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\System32\\shutdown.exe";
            if (!is64bitMachine)
            {
                WshShellClass wshShell = new WshShellClass();
                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(ShortcutPath + txtShutdownShortcut.Text + ".lnk");
                shortcut.TargetPath = shutdownTarget;
                shortcut.IconLocation = shutdownTarget;
                shortcut.Arguments = "/s /t 60";
                shortcut.Save();
            }
            else
            {
                WshShell wshShell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(ShortcutPath + txtShutdownShortcut.Text + ".lnk");
                shortcut.TargetPath = shutdownTarget;
                shortcut.IconLocation = shutdownTarget;
                shortcut.Arguments = "/s /t 60";
                shortcut.Save();
            }
        }

        private void helpClip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("If checked, then a new \"Copy All Contents\" menu in Right-Click option is shown for Text Files. \n\nThis enables you to copy the text inside the files, without having to Open & Copy the entire text.", "Description", MessageBoxButton.OK, MessageBoxImage.Information);
            e.Handled = true;
        }

        private void btnUpdateChk_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.WorkerSupportsCancellation = true;
            updateWorker.DoWork += new DoWorkEventHandler(updateWorker_DoWork);
            updateWorker.RunWorkerAsync();
        }

        void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool exceptionOccured = false;
            string downloadUrl = "";
            Version newVersion = null;
            string xmlUrl = "https://sites.google.com/site/suresoftwares/downloads/update_portable_info.xml";
            XmlTextReader xmlTextReader = null;
            try
            {
                xmlTextReader = new XmlTextReader(xmlUrl);
                xmlTextReader.MoveToContent();
                string elementName = "";
                if ((xmlTextReader.NodeType == XmlNodeType.Element) && (xmlTextReader.Name.Equals("tweaker")))
                {
                    while (xmlTextReader.Read())
                    {
                        if (xmlTextReader.NodeType == XmlNodeType.Element)
                            elementName = xmlTextReader.Name;
                        else
                        {
                            if ((xmlTextReader.NodeType == XmlNodeType.Text) && xmlTextReader.HasValue)
                            {
                                switch (elementName)
                                {
                                    case "version":
                                        newVersion = new Version(xmlTextReader.Value);
                                        break;
                                    case "url":
                                        downloadUrl = xmlTextReader.Value;
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                exceptionOccured = true;
                MessageBox.Show("Update check failed, please check your internet connection", "An Error occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (xmlTextReader != null)
                    xmlTextReader.Close();
            }
            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (applicationVersion.CompareTo(newVersion) < 0 && !exceptionOccured)
            {
                if (MessageBox.Show("A newer version " + newVersion.Major + "." + newVersion.Minor + " of Windows Tweaker is available for download!\n Would you like to download it?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    System.Diagnostics.Process.Start(downloadUrl);
            }
            else if (!exceptionOccured)
            {
                MessageBox.Show("Windows Tweaker is up-to-date!", "No need to Update", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }         
    }
}