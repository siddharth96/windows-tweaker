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
    ///     Interaction logic for OpenWith.xaml
    /// </summary>
    public partial class OpenWith : Window {
        public OpenWith() {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker) this.FindResource("backgroundWorker");
        }

        private readonly BackgroundWorker backgroundWorker;

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            OpenWithTask.Toggle(fileItem, true);
        }

        private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            OpenWithTask.Toggle(fileItem, false);
        }

        private void OnMoreInfoImageTouched(object sender, TouchEventArgs e) {
            System.Windows.Controls.Image img = e.OriginalSource as System.Windows.Controls.Image;
            if (img == null) return;
            OpenWithTask.ShowDetail(img);
            e.Handled = true;
        }

        private void OnMoreInfoImageMouseDown(object sender, MouseButtonEventArgs e) {
            System.Windows.Controls.Image img = e.OriginalSource as System.Windows.Controls.Image;
            if (img == null) return;
            OpenWithTask.ShowDetail(img);
            e.Handled = true;
        }

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
                lstOpenWithBox.ItemsSource = fileItemList;
                txtLoading.Visibility = Visibility.Collapsed;
                lstOpenWithBox.Visibility = Visibility.Visible;
            }
            else {
                string msg = e.Result as string;
                txtLoading.Text = msg ?? this.FindResource("ErrorOccurred") as string;
            }
        }

        private void OnDoWork(object sender, DoWorkEventArgs e) {
            Dictionary<string, bool> openWithFileDictionary = OpenWithTask.LoadOpenWithItems();
            if (backgroundWorker.CancellationPending) {
                e.Cancel = true;
                return;
            }
            if (openWithFileDictionary.Any()) {
                FileReader fileReader = new FileReader(openWithFileDictionary);
                if (backgroundWorker.CancellationPending) {
                    e.Cancel = true;
                    return;
                }
                ObservableCollection<FileItem> fileItemList = fileReader.GetAsFileItemListCollection();
                e.Result = fileItemList;
            }
            else {
                e.Result = this.FindResource("EmptyData") as string;
            }
        }
    }
}