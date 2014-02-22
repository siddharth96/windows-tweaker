using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for MoreInfo.xaml
    /// </summary>
    public partial class MoreInfo : Window {

        public MoreInfo() {
            InitializeComponent();
        }

        public MoreInfo(string appTitle, string filePath, ImageSource imgSource) {
            InitializeComponent();
            txtAppTitle.Text = appTitle;
            txtAppFileName.Text = System.IO.Path.GetFileName(filePath);
            txtAppFilePath.Text = filePath;
            imgProgramIcon.Source = imgSource;
        }
    }
}
