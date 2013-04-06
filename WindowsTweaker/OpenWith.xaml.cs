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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace WindowsTweaker
{
    /// <summary>
    /// Interaction logic for OpenWith.xaml
    /// </summary>
    public partial class OpenWith : Window
    {
        RegistryKey hkcr;
        RegistryKey hkcrApplications;
        RegistryKey hkcrShell;
        RegistryKey hkcrOpen;
        RegistryKey hkcrCmd;
        RegistryKey newItem;
        RegistryKey hkcrEdit;
        RegistryKey hkcrRead;
        bool firstTime;
        List<string> valuesList;
        List<string> checkBoxContents = new List<string>();
        OpenFileDialog openFileDialog = new OpenFileDialog();
        string[] subKeyNames;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lstOpenWith.ItemsSource = null;
                lstOpenWith.ItemsSource = LoadItems();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public OpenWith()
        {
            setContents();
            InitializeComponent();
            firstTime = false;
        }

        private void setContents()
        {
            try
            {

                hkcr = Registry.ClassesRoot;
                hkcrApplications = hkcr.OpenSubKey("Applications", true);
                hkcrShell = hkcrOpen = hkcrCmd = newItem = hkcrRead = hkcrEdit = null;

                valuesList = new List<string>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < valuesList.Count; i++)
                {
                    RegistryKey tmp = hkcrApplications.OpenSubKey(valuesList[i], true);
                    string[] tmpVal = tmp.GetValueNames();
                    CheckedListItem item = (CheckedListItem)lstOpenWith.Items[checkBoxContents.IndexOf(valuesList[i])];
                    if (item.IsChecked)
                    {
                        if (keySearch(tmpVal, "NoOpenWith"))
                        {
                            tmp.DeleteValue("NoOpenWith");
                        }
                    }
                    else
                    {
                        tmp.SetValue("NoOpenWith", "");
                    }
                }
                int n = valuesList.Count;
                valuesList.RemoveRange(0, n);
                firstTime = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                openFileDialog.Filter = "Executable files|*.exe|Batch files|*.bat|Command files|*.com|Jar Files|*.jar|All files|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    string installPath = string.Format("\"{0}\" \"%1\"", openFileDialog.FileName);
                    string keyName = openFileDialog.SafeFileName;
                    if (!keySearch(subKeyNames, keyName))
                    {
                        newItem = hkcrApplications.CreateSubKey(keyName);
                        hkcrShell = newItem.CreateSubKey("shell");
                        hkcrOpen = hkcrShell.CreateSubKey("Open");
                        hkcrCmd = hkcrOpen.CreateSubKey("command");
                        hkcrCmd.SetValue("", installPath);
                        MessageBox.Show("Item successfully added!");
                        lstOpenWith.ItemsSource = null;
                        lstOpenWith.ItemsSource = LoadItems();
                    }
                    else
                    {
                        MessageBox.Show("This item is already present", "Hey!!! that\'s invalid :/", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            catch (NullReferenceException ne)
            {
                MessageBox.Show(ne.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnMoreInfo_Click(object sender, RoutedEventArgs e)
        {
            MoreInformation moreInfo;
            try
            {
                string keyName = ((CheckedListItem)lstOpenWith.SelectedItem).Name;
                string[] fileInfo = null;
                string[] fName = null;
                newItem = hkcrApplications.OpenSubKey(keyName);
                hkcrShell = newItem.OpenSubKey("shell");
                hkcrOpen = hkcrShell.OpenSubKey("Open");
                if (hkcrOpen == null)
                    hkcrEdit = hkcrShell.OpenSubKey("edit");
                if (hkcrOpen == null && hkcrEdit == null)
                    hkcrRead = hkcrShell.OpenSubKey("Read");
                if (hkcrOpen != null)
                    hkcrCmd = hkcrOpen.OpenSubKey("command");
                else if (hkcrEdit != null)
                    hkcrCmd = hkcrEdit.OpenSubKey("command");
                else if (hkcrRead != null)
                    hkcrCmd = hkcrRead.OpenSubKey("command");
                string fPath = (string)hkcrCmd.GetValue("");
                moreInfo = new MoreInformation();
                if (fPath[0] == '\"' && (fPath.Contains("\"%1\"") || fPath.Contains("\"%L\"") || fPath.Contains("%1")))
                {
                    fileInfo = fPath.Split('\"');
                    fName = fileInfo[1].Split('\\');
                    moreInfo.FileLocation = fileInfo[1];
                }
                else if (!fPath.Contains("\"") || fPath.Contains("\"%1\"") || fPath.Contains("%1"))
                {
                    int space_cnt = 0;
                    for (int i = 0; i < fPath.Length; i++)
                    {
                        if (fPath[i] == ' ')
                            space_cnt++;
                    }
                    if (space_cnt == 1)
                        fileInfo = fPath.Split(' ');
                    else if (space_cnt > 1 && fPath.Contains('/'))
                    {
                        fileInfo = fPath.Split('/');
                    }
                    else if (space_cnt > 1 && !fPath.Contains('/'))
                    {
                        fileInfo = fPath.Split('\"');
                    }
                    fName = fileInfo[0].Split('\\');
                    moreInfo.FileLocation = fileInfo[0];
                }

                moreInfo.FileName = fName[fName.Length - 1];
                moreInfo.ShowDialog();
                hkcrShell = hkcrOpen = hkcrCmd = newItem = hkcrEdit = hkcrRead = null;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Unable to read the path.", "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ObservableCollection<CheckedListItem> LoadItems()
        {
            ObservableCollection<CheckedListItem> subkeyNameOBJ = new ObservableCollection<CheckedListItem>();
            try
            {
                subKeyNames = hkcrApplications.GetSubKeyNames();
                RegistryKey regTmp = null;
                for (int i = 0; i < subKeyNames.Length; i++)
                {
                    hkcrShell = hkcrOpen = hkcrCmd = null;
                    regTmp = hkcrApplications.OpenSubKey(subKeyNames[i], true);
                    string[] values = regTmp.GetValueNames();
                    string[] subKeys = regTmp.GetSubKeyNames();
                    if (subKeys.Length > 0 && !keySearch(values, "NoOpenWith"))
                    {
                        subkeyNameOBJ.Add(new CheckedListItem(subKeyNames[i], true));
                        checkBoxContents.Add(subKeyNames[i]);
                    }
                    else if (subKeys.Length > 0 && keySearch(values, "NoOpenWith"))
                    {
                        subkeyNameOBJ.Add(new CheckedListItem(subKeyNames[i], false));
                        checkBoxContents.Add(subKeyNames[i]);
                    }
                }
                firstTime = true;
                lstOpenWith.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Oh my God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return subkeyNameOBJ;
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

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            lstOpenWith.SelectedIndex = checkBoxContents.IndexOf(chk.Content.ToString());
            if (firstTime && !valuesList.Contains(chk.Content.ToString()))
            {
                valuesList.Add(chk.Content.ToString());
            }
        }
    }
}
