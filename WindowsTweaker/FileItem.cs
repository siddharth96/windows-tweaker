using System;
using System.IO;
using System.Windows.Media;

namespace WindowsTweaker {
    internal class FileItem {
        public FileItem(String fullName, ImageSource iconAssociated) {
            this.iconAssociated = iconAssociated;
            this.fullName = fullName;
            this.isChecked = false;
        }

        public FileItem(String fullName, ImageSource iconAssociated, bool isChecked) {
            this.iconAssociated = iconAssociated;
            this.fullName = fullName;
            this.isChecked = isChecked;
        }

        private String fullName;
        private ImageSource iconAssociated;
        private bool isChecked;

        public string Name {
            get { return Path.GetFileNameWithoutExtension(fullName); }
        }

        public ImageSource IconAssociated {
            get { return iconAssociated; }
            set { iconAssociated = value; }
        }

        public string FullName {
            get { return fullName; }
            set { fullName = value; }
        }

        public bool IsChecked {
            get { return isChecked; }
            set { isChecked = value; }
        }
    }
}