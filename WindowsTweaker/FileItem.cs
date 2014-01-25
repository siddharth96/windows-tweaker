using System;
using System.Windows.Media;

namespace WindowsTweaker {
    class FileItem {
        public FileItem(string name, ImageSource iconAssociated) {
            this.name = name;
            this.iconAssociated = iconAssociated;
        }

        private String name;
        private ImageSource iconAssociated;

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public ImageSource IconAssociated {
            get { return iconAssociated; }
            set { iconAssociated = value; }
        }
    }
}
