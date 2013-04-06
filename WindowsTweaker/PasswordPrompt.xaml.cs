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
    /// Interaction logic for PasswordPrompt.xaml
    /// </summary>
    public partial class PasswordPrompt : Window
    {
        private bool isAuthorised = false;

        public PasswordPrompt()
        {
            InitializeComponent();
        }

        public bool IsAuthorised
        {
            get
            {
                return isAuthorised;
            }
        }

        private void DoTask()
        {
            string passwdIPHashVal = PasswdInRegsitry.HashPasswd(txtPasswdBox.Password);
            string readPass = PasswdInRegsitry.ReadPasswd();
            if (readPass.Equals(passwdIPHashVal))
            {
                isAuthorised = true;
                this.Close();
            }
            else
            {
                isAuthorised = false;
                MessageBox.Show("Invalid Password!!", "Mind You!!", MessageBoxButton.OK, MessageBoxImage.Error);
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
            txtPasswdBox.Password = "";
            txtPasswdBox.Focus();
        }

        private void txtPasswdBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                DoTask();
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }
    }
}
