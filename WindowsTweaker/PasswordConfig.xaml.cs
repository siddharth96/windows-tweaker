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

namespace WindowsTweaker
{
    /// <summary>
    /// Interaction logic for PasswordConfig.xaml
    /// </summary>
    public partial class PasswordConfig : Window
    {
        public PasswordConfig()
        {
            InitializeComponent();
        }

        private void DoTask()
        {
            if (chkPasswdPol.IsChecked == true)
            {
                if (txtPasswd.Password.Equals(txtRePasswd.Password) && txtPasswd.Password.Length >= 8)
                {
                    PasswdInRegsitry.SetPassword(PasswdInRegsitry.HashPasswd(txtPasswd.Password));
                    this.Close();
                }
                else if (txtPasswd.Password.Length < 8)
                {
                    MessageBox.Show("Your password should have atleast 8-characters", "Mind You!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Passwords don\'t match, Please Re-enter your password", "Mind You!!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                PasswdInRegsitry.RemovePasswd();
                this.Close();
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DoTask();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            txtPasswd.Password = txtRePasswd.Password = "";
            string rv = PasswdInRegsitry.ReadPasswd();
            if (rv != null)
                chkPasswdPol.IsChecked = true;
            else
                chkPasswdPol.IsChecked = false;
        }
    }
}
