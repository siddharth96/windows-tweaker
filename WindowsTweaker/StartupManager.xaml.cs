using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WindowsTweaker.AppTasks;
using WindowsTweaker.Models;
using Microsoft.Win32;
using WPFSpark;
using Image = System.Drawing.Image;

namespace WindowsTweaker {
    /// <summary>
    ///     Interaction logic for StartupManager.xaml
    /// </summary>
    public partial class StartupManager : Window {
        public StartupManager() {
            InitializeComponent();
            Dictionary<string, Models.Tuple<string, bool>> startupItemDictionary = StartupManagerTask.LoadStartupItems();
            if (!startupItemDictionary.Any()) return;
            FileReader fileReader = new FileReader(startupItemDictionary);
            ObservableCollection<FileItem> fileItemList =
                fileReader.GetAsFileItemCollectionWithUserTitle();
            lstStartupItems.ItemsSource = fileItemList;
        }

        

        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            StartupManagerTask.Add(fileItem);
        }

        private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            StartupManagerTask.Remove(fileItem);
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