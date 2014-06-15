using System;
using System.Diagnostics;
using System.Windows;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window {
        public About() {
            InitializeComponent();
            Version applicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            txtVersion.Text = this.FindResource("Version") + " " + applicationVersion;
        }

        private void OnVisitUsLinkClick(object sender, RoutedEventArgs e) {
            if (String.IsNullOrEmpty(Keys.SiteDomain)) return;;
            Process.Start(Keys.SiteDomain);
        }

        private void OnLikeOnFacebookClick(object sender, RoutedEventArgs e) {
            Process.Start(Keys.FbPageUrl);
        }

        private void OnViewSourceClick(object sender, RoutedEventArgs e) {
            Process.Start(Keys.RepoUrl);
        }
    }
}
