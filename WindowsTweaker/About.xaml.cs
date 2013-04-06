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
using System.Diagnostics;

namespace WindowsTweaker
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        private void lnkFeedback_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("http://xinbox.com/sidmkp96");
            }
            catch (System.ComponentModel.Win32Exception fn)
            {
                MessageBox.Show("Unable to load address because a component is missing :" + fn.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lnkHomepage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("https://sites.google.com/site/suresoftwares");
            }
            catch (System.ComponentModel.Win32Exception fn)
            {
                MessageBox.Show("Unable to load address because a component is missing :" + fn.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("https://www.facebook.com/windows7tweaker");
            }
            catch (System.ComponentModel.Win32Exception fn)
            {
                MessageBox.Show("Unable to load address because a component is missing :" + fn.Message, "Oh My God!!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
