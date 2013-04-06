using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsTweaker
{
    class CheckedListItem
    {
        private string name;
        private bool isChecked;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        public CheckedListItem(string name, bool isChecked)
        {
            Name = name;
            IsChecked = isChecked;
        }
    }
}
