using System.Drawing;
using System.Windows;
using WindowsTweaker.AppTasks;

namespace WindowsTweaker {
    /// <summary>
    /// Interaction logic for InfoBox.xaml
    /// </summary>
    public partial class InfoBox : Window {

        public enum DialogType {
            Information, Error, Question, Warning
        }

        public InfoBox(string msg, string okText) {
            InitializeComponent();
            txtMsg.Text = msg;
            btnOk.Content = okText;
        }

        public InfoBox(string msg, string okText, string caption) : this(msg, okText) {
            infoBoxWindow.Title = caption;
        }

        public InfoBox(string msg, string okText, string caption, DialogType dialogType)
            : this(msg, okText, caption) {
            switch (dialogType) {
                case DialogType.Error:
                    imgInfoBox.Source = SystemIcons.Error.ToBitmap().ToImageSource();
                    break;
                case DialogType.Information:
                    imgInfoBox.Source = SystemIcons.Information.ToBitmap().ToImageSource();
                    break;
                case DialogType.Question:
                    imgInfoBox.Source = SystemIcons.Question.ToBitmap().ToImageSource();
                    break;
                case DialogType.Warning:
                    imgInfoBox.Source = SystemIcons.Warning.ToBitmap().ToImageSource();
                    break;
            }
        }

        public void HideCancelButton() {
            btnCancel.Visibility = Visibility.Collapsed;
        }

        public InfoBox(string msg, string okText, string cancelText, string caption, DialogType dialogType) 
            : this(msg, okText, caption, dialogType) {
            btnCancel.Content = cancelText;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
