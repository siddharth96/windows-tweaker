using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace WindowsTweaker
{
    /// <summary>
    ///     Interaction logic for HelpCheckBox.xaml
    /// </summary>
    public partial class HelpCheckBox : UserControl
    {
        public HelpCheckBox()
        {
            InitializeComponent();
        }

        public String HelpText
        {
            get { return txtHelp.Text; }
            set { txtHelp.Text = value; }
        }

        public String Text
        {
            get { return txtChkBx.Text; }
            set { txtChkBx.Text = value; }
        }

        private void OnHelpBoxMouseEnter(object sender, MouseEventArgs e)
        {
            popHelp.IsOpen = true;
        }
    }
}