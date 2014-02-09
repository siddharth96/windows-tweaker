using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for OpenWith.xaml
    /// </summary>
    public partial class OpenWith : Window {
        public OpenWith() {
            InitializeComponent();
        }


        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            LoadItems();
        }

        private void LoadItems() {
            using (RegistryKey hkcrApplications = Registry.ClassesRoot.OpenSubKey("Applications", true)) {
                String[] subKeyNames = hkcrApplications.GetSubKeyNames();
                foreach (string subKeyName in subKeyNames) {
                    RegistryKey regKey = hkcrApplications.OpenSubKey(subKeyName, true);

                }
            }
        }
    }
}
