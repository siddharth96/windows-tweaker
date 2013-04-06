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
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
namespace WindowsTweaker
{
    /// <summary>
    /// Interaction logic for StartupManager.xaml
    /// </summary>
    public partial class StartupManager : Window
    {
        RegistryKey hkcu;
        RegistryKey hkcuRun;
        RegistryKey hkcuRun_;
        RegistryKey hklm;
        RegistryKey hklmRun;
        RegistryKey hklmRun_;
        List<string> valChange=new List<string>();
        List<string> allValues=new List<string>();
        List<int> p = new List<int>();
        bool bk;
        string keypath;
        public StartupManager()
        {
            bk = false;
            InitializeComponent();
        }
        private ObservableCollection<CheckedListItem> LoadItems()
        {
            ObservableCollection<CheckedListItem> startupList = new ObservableCollection<CheckedListItem>();
            try
            {   
			    hkcu=Registry.CurrentUser;
                hkcuRun=hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                hkcuRun_ = hkcu.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-");
                hklm=Registry.LocalMachine;
                hklmRun = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                hklmRun_ = hklm.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run-");
                if (hkcuRun != null)
                {
                    string[] vals;
                    vals = hkcuRun.GetValueNames();
                    foreach (string val in vals)
                    {
                        if (val.Length > 0 && !allValues.Contains(val))
                        {
                            startupList.Add(new CheckedListItem(val, true));
                            allValues.Add(val);
                            p.Add(0);
                        }
                    }
                }
                if (hkcuRun_ != null)
                {
                    string[] value;
                    value = hkcuRun_.GetValueNames();
                    foreach (string a in value)
                    {
                        if (a.Length > 0 && !allValues.Contains(a))
                        {
                            startupList.Add(new CheckedListItem(a, false));
                            allValues.Add(a);
                            p.Add(0);
                        }
                    }
                }

                if (hklmRun != null)
                {
                    string[] hkval = hklmRun.GetValueNames();
                    foreach (string b in hkval)
                    {
                        if (b.Length > 0 && !allValues.Contains(b))
                        {startupList.Add(new CheckedListItem(b, true));
                            allValues.Add(b);
                            p.Add(1);
                        }
                    }
                }
                if (hklmRun_ != null)
                {
                    string[] hklval = hklmRun_.GetValueNames();
                    foreach (string c in hklval)
                    {
                        if (c.Length > 0 && !allValues.Contains(c))
                        {startupList.Add(new CheckedListItem(c, false));
                            allValues.Add(c);
                            p.Add(1);
                        }
                    }
                }
                bk = true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"An error occured",MessageBoxButton.OK,MessageBoxImage.Error);            
            }
            return startupList;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog=new OpenFileDialog();
                openFileDialog.Filter = "Executable Files|*.exe|Batch Files|*.bat|Command Files|*.cmd|COM Files|*.com";
                if (openFileDialog.ShowDialog() == true)
                {
                    string path = openFileDialog.FileName;
                    string filename = openFileDialog.SafeFileName;
                    string[] n = filename.Split('.');
                    filename = "";
                    for (int i = 0; i < n.Length - 1; i++)
                    {
                        filename += n[i];
                    }
                    hkcuRun.SetValue(filename, path);
                    lstStartupItems.ItemsSource = null;
                    allValues.RemoveRange(0, allValues.Count);
                    p.RemoveRange(0, p.Count);
                    lstStartupItems.ItemsSource= LoadItems();
                }
            }
            catch (Exception )
            {
                MessageBox.Show("\nAn Unexpected Error has Occured!", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
               /* mailSend = true;
                ShowReportDlg(eff);
                if (mailSend)
                {
                    MessageBox.Show("Error report has been send.", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MailBar.CloseMailBar();
                }
                this.Visible = false;
          */  }
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

        private void btnMoreInfo_Click(object sender, RoutedEventArgs e)
        {
            MoreInformation moreInfoBox=new MoreInformation();
            try
            {
                string keyName = ((CheckedListItem)lstStartupItems.SelectedItem).Name;
                if (keySearch(hkcuRun.GetValueNames(), keyName))
                {
                    moreInfoBox.FileName = keyName;
                    moreInfoBox.FileLocation = (string)hkcuRun.GetValue(keyName);
                }
                else if (keySearch(hkcuRun_.GetValueNames(), keyName))
                {
                    moreInfoBox.FileName = keyName;
                    moreInfoBox.FileLocation = (string)hkcuRun_.GetValue(keyName);
                }
                else if (keySearch(hklmRun.GetValueNames(), keyName))
                {
                    moreInfoBox.FileName = keyName;
                    moreInfoBox.FileLocation = (string)hklmRun.GetValue(keyName);
                }
                else if (keySearch(hklmRun_.GetValueNames(), keyName))
                {
                    moreInfoBox.FileName = keyName;
                    moreInfoBox.FileLocation = (string)hklmRun_.GetValue(keyName);
                }
                moreInfoBox.ShowDialog();
            
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Unable to read the path.", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            List<int> num=new List<int>();

            try
            {
                int path;
                bool chkd; int index;
                for (int i = 0; i < valChange.Count; i++)
                {
                    num.Add(allValues.IndexOf(valChange[i]));
                }
                for (int i = 0; i < valChange.Count; i++)
                {
                    path = p[num[i]];
                    index = allValues.IndexOf(valChange[i]);
                   // chkd = clb.GetItemChecked(index);
                    CheckedListItem item = (CheckedListItem)lstStartupItems.Items[allValues.IndexOf(valChange[i])];
                    chkd = item.IsChecked;
                    if (chkd)
                    {
                        if (path == 0 && keySearch(hkcuRun_.GetValueNames(), valChange[i]))
                        {
                            keypath = (string)hkcuRun_.GetValue(valChange[i]);
                            
                            hkcuRun.SetValue((string)allValues[num[i]], keypath);
                            hkcuRun_.DeleteValue((string)allValues[num[i]], true);
                        }

                        else if (path == 1 && keySearch(hklmRun_.GetValueNames(), valChange[i]))
                        {
                            keypath = (string)hklmRun_.GetValue(valChange[i]);
                            hklmRun.SetValue((string)allValues[num[i]], keypath);
                            hklmRun_.DeleteValue((string)allValues[num[i]], true);
                        }

                    }
                    else
                    {
                        if (path == 0 && keySearch(hkcuRun.GetValueNames(), valChange[i]))
                        {
                            keypath = (string)hkcuRun.GetValue(valChange[i]);
                            hkcuRun_.SetValue((string)allValues[num[i]], keypath);
                            hkcuRun.DeleteValue((string)allValues[num[i]]);
                        }
                        else if (path == 1 && keySearch(hklmRun.GetValueNames(), valChange[i]))
                        {
                            keypath = (string)hklmRun.GetValue(valChange[i]);
                            hklmRun_.SetValue((string)allValues[num[i]], keypath);
                            hklmRun.DeleteValue((string)allValues[num[i]]);
                        }
                    }
                }
                this.Close();
            }
            catch (Exception )
            {
                MessageBox.Show("\nAn Unexpected Error has Occured", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
                /*mailSend = true;
                ShowReportDlg(eff);
                if (mailSend)
                {
                    MessageBox.Show("Error report has been send.", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MailBar.CloseMailBar();
                }
                this.Visible = false;
            */
            }
        
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lstStartupItems.ItemsSource = null;
                lstStartupItems.ItemsSource = LoadItems();
                lstStartupItems.SelectedIndex = 0;
            } 
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"An error occured",MessageBoxButton.OK,MessageBoxImage.Error);            
            }
        }
        

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            lstStartupItems.SelectedIndex = allValues.IndexOf(chk.Content.ToString());
            if (bk && !valChange.Contains(chk.Content.ToString()))
            {
                valChange.Add(chk.Content.ToString());
            }
        }
    }
}
