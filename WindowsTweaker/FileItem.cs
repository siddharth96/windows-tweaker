using System;
using System.IO;
using System.Windows.Media;

namespace WindowsTweaker {
    internal class FileItem {
        public FileItem(string fullName, ImageSource iconAssociated) {
            this.iconAssociated = iconAssociated;
            this.fullName = fullName;
            this.isChecked = false;
        }

        public FileItem(string fullName, ImageSource iconAssociated, bool isChecked) {
            this.iconAssociated = iconAssociated;
            this.fullName = fullName;
            this.isChecked = isChecked;
        }

        public FileItem(string fullName, ImageSource iconAssociated, bool isChecked, string name) {
            this.fullName = fullName;
            this.iconAssociated = iconAssociated;
            this.isChecked = isChecked;
            this.name = name;
        }

        private string fullName;
        private ImageSource iconAssociated;
        private bool isChecked;
        private string name;

        public string Name {
            get { return name ?? Path.GetFileNameWithoutExtension(fullName); }
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