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
    /// Interaction logic for MoreInformation.xaml
    /// </summary>
    public partial class MoreInformation : Window
    {
        public MoreInformation()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get
            {
                return txtFileName.Text;
            }
            set
            {
                txtFileName.Text = value;
            }
        }

        public string FileLocation
        {
            get
            {
                return txtFileLocation.Text;
            }
            set
            {
                txtFileLocation.Text = value;
            }
        }
    }
}
