using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WindowsTweaker.AppTasks {
    internal class Message {
        internal Message(Panel msgContainer, TextBlock txtMsg) {
            this._msgContainer = msgContainer;
            this._txtMsg = txtMsg;
        }

        private readonly Panel _msgContainer;
        private readonly TextBlock _txtMsg;

        internal void Hide() {
            _msgContainer.Visibility = Visibility.Collapsed;
            _txtMsg.Text = String.Empty;
        }

        internal void Success(string txt) {
            _msgContainer.Visibility = Visibility.Visible;
            _msgContainer.Background = new SolidColorBrush(Colors.ForestGreen);
            _txtMsg.Text = txt;
        }

        internal void Error(string txt) {
            _msgContainer.Visibility = Visibility.Visible;
            _msgContainer.Background = new SolidColorBrush(Colors.Firebrick);
            _txtMsg.Text = txt;
        }

        internal void Notify(string txt) {
            _msgContainer.Visibility = Visibility.Visible;
            _msgContainer.Background = new SolidColorBrush(Colors.DodgerBlue);
            _txtMsg.Text = txt;
        }
    }
}