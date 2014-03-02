using System.IO;
using System.Windows.Media;

namespace WindowsTweaker.Models {
    internal class FileItem {
        public FileItem(string fullName, ImageSource iconAssociated, object tag=null) {
            this.iconAssociated = iconAssociated;
            this.fullName = fullName;
            this.isChecked = false;
            this.tag = tag;
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
        private object tag;

        public string Name {
            get { return name ?? Path.GetFileNameWithoutExtension(fullName); }
            set { name = value; }
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

        public object Tag {
            get { return tag; }
            set { tag = value; }
        }
    }
}