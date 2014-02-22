using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal class FileReader {
        private string folderPath;
        private List<string> ignoreExtensionList;
        private Dictionary<string, bool> fileDictionary;
        private Dictionary<string, Models.Tuple<string, bool>> fileWithTitleDictionary;

        public FileReader(string folderPath) {
            this.folderPath = folderPath;
        }

        public FileReader(string folderPath, List<string> ignoreExtensionList) {
            this.folderPath = folderPath;
            this.ignoreExtensionList = ignoreExtensionList;
        }

        public FileReader(Dictionary<string, bool> fileDictionary) {
            this.fileDictionary = fileDictionary;
        }

        public FileReader(Dictionary<string, Models.Tuple<string, bool>> fileWithTitleDictionary) {
            this.fileWithTitleDictionary = fileWithTitleDictionary;
        }

        public Dictionary<string, Models.Tuple<string, bool>> FileWithTitleDictionary {
            get { return fileWithTitleDictionary; }
        }

        public Dictionary<string, bool> FileDictionary {
            get { return fileDictionary; }
        }

        public string FolderPath {
            get { return folderPath; }
        }

        public List<string> IgnoreExtensionList {
            get { return ignoreExtensionList; }
        }

        public ObservableCollection<FileItem> GetAllFiles() {
            ObservableCollection<FileItem> fileItems = null;
            DirectoryInfo directory = new DirectoryInfo(this.folderPath);
            if (!directory.Exists)
                throw new FileNotFoundException();
            fileItems = new ObservableCollection<FileItem>();
            foreach (FileInfo fileInfo in directory.GetFiles()) {
                if (ignoreExtensionList != null && ignoreExtensionList.Contains(fileInfo.Extension)) {
                    continue;
                }

                Icon fileIcon = Icon.ExtractAssociatedIcon(fileInfo.FullName);
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(fileIcon.Handle, new Int32Rect(0, 0,
                    fileIcon.Width, fileIcon.Height), BitmapSizeOptions.FromEmptyOptions());
                fileItems.Add(new FileItem(fileInfo.FullName, imageSource));
            }
            return fileItems;
        }

        private FileItem GetFileItem(string filePath, bool isChecked, string name) {
            try {
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Exists) {
                    Icon fileIcon = Icon.ExtractAssociatedIcon(filePath);
                    ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(fileIcon.Handle,
                        new Int32Rect(0, 0,
                            fileIcon.Width, fileIcon.Height), BitmapSizeOptions.FromEmptyOptions());
                    return new FileItem(filePath, imageSource, isChecked, name);
                }
            }
            catch (ArgumentException) {
                //Invalid filePath
            }
            return null;
        }

        public ObservableCollection<FileItem> GetAsFileItemListCollection() {
            ObservableCollection<FileItem> fileItemCollection = null;
            if (fileDictionary != null) {
                fileItemCollection = new ObservableCollection<FileItem>();
                foreach (string filePath in fileDictionary.Keys) {
                    FileItem fileItem = GetFileItem(filePath, fileDictionary[filePath], null);
                    if (fileItem != null) {
                        fileItemCollection.Add(fileItem);
                    }
                }
            }
            return fileItemCollection;
        }

        public ObservableCollection<FileItem> GetAsFileItemCollectionWithUserTitle() {
            ObservableCollection<FileItem> toggleViewFileItemCollection = null;
            if (fileWithTitleDictionary != null) {
                toggleViewFileItemCollection = new ObservableCollection<FileItem>();
                foreach (string filePath in fileWithTitleDictionary.Keys) {
                    Models.Tuple<string, bool> titleAndIsChecked = fileWithTitleDictionary[filePath];
                    FileItem fileItem = GetFileItem(filePath, titleAndIsChecked.y, titleAndIsChecked.x);
                    if (fileItem != null) {
                        toggleViewFileItemCollection.Add(fileItem);
                    }
                }
            }
            return toggleViewFileItemCollection;
        }
    }
}