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

namespace WindowsTweaker
{
    /// <summary>
    /// Interaction logic for ContextMenuShortcutCreator.xaml
    /// </summary>
    public partial class ContextMenuShortcutCreator : Window
    {
        public ContextMenuShortcutCreator()
        {
            InitializeComponent();
        }
        RegistryKey hkcr = Registry.ClassesRoot;
        RegistryKey hkcrFrndlyTxt;


        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Executable files|*.exe|Batch Files|*.bat|COM Files|*.com|Command Files|*.cmd";
            if (openFileDialog1.ShowDialog() == true)
            {
               txtLocation.Text = openFileDialog1.FileName;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtLocation.Text.Length == 0 || txtShortcut.Text.Length == 0)
            {
                MessageBox.Show("Please Enter Text.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            else
            {
                hkcrFrndlyTxt = hkcr.CreateSubKey(@"Directory\Background\Shell\" + txtShortcut.Text + "\\command");
                if (rbtnPath.IsChecked == true)
                {
                    hkcrFrndlyTxt.SetValue("", txtLocation.Text);
                }
                else
                {
                    hkcrFrndlyTxt.SetValue("", "iexplore.exe " + txtLocation.Text);
                }
               this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void rbtnPath_Click(object sender, RoutedEventArgs e)
        {
            lblExample.Visibility = Visibility.Hidden;
            lblLocation.Text = "Path:";
            txtLocation.IsReadOnly = txtLocation.IsReadOnlyCaretVisible = true;
            btnBrowse.Visibility = Visibility.Visible;
        }

        private void rbtnUrl_Click(object sender, RoutedEventArgs e)
        {
            lblLocation.Text = "URL:";
            lblExample.Visibility = Visibility.Visible;
            btnBrowse.Visibility = Visibility.Hidden;
            txtLocation.IsReadOnly = txtLocation.IsReadOnlyCaretVisible = false;
        }
    }
}
