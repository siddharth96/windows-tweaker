using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WindowsTweaker
{
    class ToggleViewFileItem: FileItem {
        private string showText;
        private string hideText;
        public ToggleViewFileItem(string fullName, ImageSource iconAssociated, string showText, string hideText) : base(fullName, iconAssociated) {
            this.showText = showText;
            this.hideText = hideText;
        }

        public ToggleViewFileItem(string fullName, ImageSource iconAssociated, bool isChecked, string showText, string hideText) : 
            base(fullName, iconAssociated, isChecked) {
            this.showText = showText;
            this.hideText = hideText;
        }

        public ToggleViewFileItem(string fullName, ImageSource iconAssociated, bool isChecked, string name, string showText, string hideText) :
            base(fullName, iconAssociated, isChecked, name) {
            this.showText = showText;
            this.hideText = hideText;
        }

        public ToggleViewFileItem(FileItem fileItem, string showText, string hideText) :
            base(fileItem.FullName, fileItem.IconAssociated, fileItem.IsChecked, fileItem.Name) {
            this.showText = showText;
            this.hideText = hideText;
        }

        public string ShowText {
            get { return showText; }
            set { showText = value; }
        }

        public string HideText {
            get { return hideText; }
            set { hideText = value; }
        }
    }
}
