using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WindowsTweaker.AppTasks;
using WindowsTweaker.Models;
using WPFSpark;

namespace WindowsTweaker {
    /// <summary>
    ///     Interaction logic for StartupManager.xaml
    /// </summary>
    public partial class StartupManager : Window {
        public StartupManager() {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker) this.FindResource("backgroundWorker");
        }

        private readonly BackgroundWorker backgroundWorker;

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            backgroundWorker.RunWorkerAsync();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e) {
            if (backgroundWorker.IsBusy) {
                backgroundWorker.CancelAsync();
            }
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            if (e.Cancelled) return;
            ObservableCollection<FileItem> fileItemList = e.Result as ObservableCollection<FileItem>;
            if (fileItemList != null) {
                lstStartupItems.ItemsSource = fileItemList;
                txtLoading.Visibility = Visibility.Collapsed;
                lstStartupItems.Visibility = Visibility.Visible;
            }
            else {
                string msg = e.Result as string;
                txtLoading.Text = msg ?? "An error occured";
            }
        }

        private void OnDoWork(object sender, DoWorkEventArgs e) {
            Dictionary<string, Models.Tuple<string, bool>> startupItemDictionary = StartupManagerTask.LoadStartupItems();
            if (backgroundWorker.CancellationPending) {
                e.Cancel = true;
                return;
            }
            if (startupItemDictionary.Any()) {
                FileReader fileReader = new FileReader(startupItemDictionary);
                if (backgroundWorker.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
                ObservableCollection<FileItem> fileItemList = fileReader.GetAsFileItemCollectionWithUserTitle();
                e.Result = fileItemList;
            }
            else {
                e.Result = "Nothing to show";
            }
        }

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            StartupManagerTask.Toggle(fileItem, true);
        }

        private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            StartupManagerTask.Toggle(fileItem, false);
        }

        private void OnMoreInfoImageTouched(object sender, TouchEventArgs e) {
            System.Windows.Controls.Image img = e.OriginalSource as System.Windows.Controls.Image;
            if (img == null) return;
            StartupManagerTask.ShowDetail(img);
            e.Handled = true;
        }

        private void OnMoreInfoImageMouseDown(object sender, MouseButtonEventArgs e) {
            System.Windows.Controls.Image img = e.OriginalSource as System.Windows.Controls.Image;
            if (img == null) return;
            StartupManagerTask.ShowDetail(img);
            e.Handled = true;
        }
    }
}