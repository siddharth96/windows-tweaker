using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsTweaker {
    class FileReader {
        private String folderPath;
        private List<String> ignoreExtensionList;

        public FileReader(string folderPath) {
            this.folderPath = folderPath;
        }

        public FileReader(String folderPath, List<String> ignoreExtensionList) {
            this.folderPath = folderPath;
            this.ignoreExtensionList = ignoreExtensionList;
        }

        public String FolderPath {
            get { return folderPath; }
        }

        public List<String> IgnoreExtensionList {
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

        public static ObservableCollection<FileItem> GetAsFileItemListCollection(Dictionary<String, bool> fileList)
        {
            ObservableCollection<FileItem> fileItemCollection = null;
            if (fileList != null) {
                fileItemCollection = new ObservableCollection<FileItem>();
                foreach (String filePath in fileList.Keys) {
                    Icon fileIcon = Icon.ExtractAssociatedIcon(filePath);
                    ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(fileIcon.Handle, new Int32Rect(0, 0,
                        fileIcon.Width, fileIcon.Height), BitmapSizeOptions.FromEmptyOptions());
                    fileItemCollection.Add(new FileItem(filePath, imageSource, fileList[filePath]));
                }
            }
            return fileItemCollection;
        }
    }
}
