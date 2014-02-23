using System;
using System.Windows;
using System.Windows.Media;

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
