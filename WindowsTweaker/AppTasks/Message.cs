using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaker.AppTasks {
    internal class Message {
        internal Message(Panel msgContainer, TextBlock txtMsg) {
            this.msgContainer = msgContainer;
            this.txtMsg = txtMsg;
        }

        private Panel msgContainer;
        private TextBlock txtMsg;

        internal void Hide() {
            msgContainer.Visibility = Visibility.Hidden;
            txtMsg.Text = String.Empty;
        }

        internal void Success(string txt) {
            msgContainer.Visibility = Visibility.Visible;
            msgContainer.Background = new SolidColorBrush(Colors.ForestGreen);
            txtMsg.Text = txt;
        }

        internal void Error(string txt) {
            msgContainer.Visibility = Visibility.Visible;
            msgContainer.Background = new SolidColorBrush(Colors.Firebrick);
            txtMsg.Text = txt;
        }
    }
}