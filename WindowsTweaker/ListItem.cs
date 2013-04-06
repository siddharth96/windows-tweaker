using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WindowsTweaker
{
    class ListItem
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ImageSource iconAssociated;
        public ImageSource IconAssociated
        {
            get { return iconAssociated; }
            set { iconAssociated = value; }
        }

        public ListItem(String name, ImageSource iconAssociated)
        {
            Name = name;
            IconAssociated = iconAssociated;
        }
    }
}
