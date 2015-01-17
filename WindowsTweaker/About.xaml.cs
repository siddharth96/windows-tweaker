using System;
using System.Diagnostics;
using System.Windows;
using WindowsTweaker.AppTasks;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window {
        public About() {
            InitializeComponent();
            txtVersion.Text = this.FindResource("Version") + " " + Utils.GetAppVersion();
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
