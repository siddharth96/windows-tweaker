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
    /// Interaction logic for RemoveContextMenuItem.xaml
    /// </summary>
    public partial class RemoveContextMenuItem : Window
    {
        public RemoveContextMenuItem()
        {
            InitializeComponent();
        }

        RegistryKey hkcr = Registry.ClassesRoot;
        RegistryKey hkcrFrndlyTxt;
        RegistryKey hkcrValue;
        

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            ListItem[] item = new ListItem[lstContextItems.SelectedItems.Count];
            lstContextItems.SelectedItems.CopyTo(item, 0);
            if (item.Length >= 1)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    hkcrFrndlyTxt.DeleteSubKeyTree(item[i].Name);
                }
            }
            ContextListReload();
        }

        private void ContextListReload()
        {
            hkcrFrndlyTxt = hkcr.CreateSubKey(@"Directory\Background\Shell");
            string[] names = hkcrFrndlyTxt.GetSubKeyNames();
            string value;
            ObservableCollection<ListItem> contextItems = new ObservableCollection<ListItem>();
            System.Windows.Media.ImageSource imgSource = null;
            for (int i = 0; i < names.Length; i++)
            {
                if (names[i].Contains("cmd"))
                    continue;
                hkcrValue = hkcr.CreateSubKey(@"Directory\Background\Shell\" + names[i] + "\\command");
                value = hkcrValue.GetValue("").ToString();
                if (value.Contains("iexplore.exe"))
                    value = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\Internet Explorer\\iexplore.exe";
                if (!System.IO.File.Exists(value))
                    continue;
                System.Drawing.Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(value);
                imgSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new System.Windows.Int32Rect(0, 0, icon.Width, icon.Height), BitmapSizeOptions.FromEmptyOptions());
                contextItems.Add(new ListItem(names[i], imgSource));
            }
            lstContextItems.ItemsSource = contextItems;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ContextListReload();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
