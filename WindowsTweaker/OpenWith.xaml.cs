using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            Dictionary<string, bool> openWithFileDictionary = OpenWithTask.LoadOpenWithItems();
            if (openWithFileDictionary.Any()) {
                FileReader fileReader = new FileReader(openWithFileDictionary);
                ObservableCollection<FileItem> fileItemList = fileReader.GetAsFileItemListCollection();
                lstOpenWithBox.ItemsSource = fileItemList;
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void OnToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            StartupManagerTask.Add(fileItem);
        }

        private void OnToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = e.Source as ToggleSwitch;
            if (toggleSwitch == null) return;
            FileItem fileItem = toggleSwitch.Tag as FileItem;
            if (fileItem == null) return;
            OpenWithTask.Remove(fileItem);
        }

        private void OnMoreInfoImageTouched(object sender, TouchEventArgs e)
        {
            System.Windows.Controls.Image img = e.OriginalSource as System.Windows.Controls.Image;
            if (img == null) return;
            OpenWithTask.ShowDetail(img);
            e.Handled = true;
        }

        private void OnMoreInfoImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image img = e.OriginalSource as System.Windows.Controls.Image;
            if (img == null) return;
            OpenWithTask.ShowDetail(img);
            e.Handled = true;
        }
    }
}