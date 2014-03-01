using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WindowsTweaker.Models;

namespace WindowsTweaker.AppTasks {
    internal class FileReader {
        private readonly string _folderPath;
        private readonly List<string> _ignoreExtensionList;
        private readonly Dictionary<string, bool> _fileDictionary;
        private readonly Dictionary<string, Models.Tuple<string, bool>> _fileWithTitleDictionary;

        public FileReader(string folderPath) {
            this._folderPath = folderPath;
        }

        public FileReader(string folderPath, List<string> ignoreExtensionList) {
            this._folderPath = folderPath;
            this._ignoreExtensionList = ignoreExtensionList;
        }

        public FileReader(Dictionary<string, bool> fileDictionary) {
            this._fileDictionary = fileDictionary;
        }

        public FileReader(Dictionary<string, Models.Tuple<string, bool>> fileWithTitleDictionary) {
            this._fileWithTitleDictionary = fileWithTitleDictionary;
        }

        public Dictionary<string, Models.Tuple<string, bool>> FileWithTitleDictionary {
            get { return _fileWithTitleDictionary; }
        }

        public Dictionary<string, bool> FileDictionary {
            get { return _fileDictionary; }
        }

        public string FolderPath {
            get { return _folderPath; }
        }

        public List<string> IgnoreExtensionList {
            get { return _ignoreExtensionList; }
        }

        public ObservableCollection<FileItem> GetAllFiles() {
            ObservableCollection<FileItem> fileItems = null;
            DirectoryInfo directory = new DirectoryInfo(this._folderPath);
            if (!directory.Exists)
                throw new FileNotFoundException();
            fileItems = new ObservableCollection<FileItem>();
            foreach (FileInfo fileInfo in directory.GetFiles()) {
                if (_ignoreExtensionList != null && _ignoreExtensionList.Contains(fileInfo.Extension)) {
                    continue;
                }

                Icon fileIcon = Icon.ExtractAssociatedIcon(fileInfo.FullName);
                ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(fileIcon.Handle, new Int32Rect(0, 0,
                    fileIcon.Width, fileIcon.Height), BitmapSizeOptions.FromEmptyOptions());
                imageSource.Freeze();
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
                    imageSource.Freeze();
                    return new FileItem(filePath, imageSource, isChecked, name);
                }
            }
            catch (ArgumentException) {
                //Invalid filePath
            }
            return null;
        }

        public ObservableCollection<FileItem> GetAsFileItemListCollection() {
            if (_fileDictionary == null) return null;
            ObservableCollection<FileItem> fileItemCollection = null;
            fileItemCollection = new ObservableCollection<FileItem>();
            foreach (FileItem fileItem in _fileDictionary.Keys.Select(filePath => 
                GetFileItem(filePath, _fileDictionary[filePath], null)).Where(fileItem => fileItem != null)) {
                    fileItemCollection.Add(fileItem);
                }
            return fileItemCollection;
        }

        public ObservableCollection<FileItem> GetAsFileItemCollectionWithUserTitle() {
            if (_fileWithTitleDictionary == null) return null;
            ObservableCollection<FileItem> toggleViewFileItemCollection = null;
            toggleViewFileItemCollection = new ObservableCollection<FileItem>();
            foreach (FileItem fileItem in from filePath in _fileWithTitleDictionary.Keys 
                let titleAndIsChecked = _fileWithTitleDictionary[filePath] 
                select GetFileItem(filePath, titleAndIsChecked.y, titleAndIsChecked.x) 
                into fileItem where fileItem != null select fileItem) {
                    toggleViewFileItemCollection.Add(fileItem);
                }
            return toggleViewFileItemCollection;
        }
    }
}