using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Input;

namespace WindowsTweaker.Resources {
    public partial class WindowDictionary : ResourceDictionary {
        public WindowDictionary() {
            InitializeComponent();
        }

        private bool isResizing = false;
        [Flags()]
        private enum ResizeType {
            Width, Height
        }
        private ResizeType resizeType;


        private void OnWindowResizeInitiateWE(object sender, System.Windows.Input.MouseEventArgs e) {
            isResizing = true;
            resizeType = ResizeType.Width;
        }
        private void OnWindowResizeInitiateNS(object sender, System.Windows.Input.MouseEventArgs e) {
            isResizing = true;
            resizeType = ResizeType.Height;
        }

        private void OnWindowResizeEnd(object sender, System.Windows.Input.MouseEventArgs e) {
            isResizing = false;

            // Make sure capture is released.
            Rectangle rect = (Rectangle)sender;
            rect.ReleaseMouseCapture();
        }

        private void OnWindowResizing(object sender, System.Windows.Input.MouseEventArgs e) {
            Rectangle rect = (Rectangle)sender;
            Window win = (Window)rect.TemplatedParent;

            if (isResizing) {
                rect.CaptureMouse();
                if (resizeType == ResizeType.Width) {
                    double width = e.GetPosition(win).X + 5;
                    if (width > 0) win.Width = width;
                }
                if (resizeType == ResizeType.Height) {
                    double height = e.GetPosition(win).Y + 5;
                    if (height > 0) win.Height = height;
                }
            }
        }

        private void OnTitleBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            win.DragMove();
        }

        private void OnTitlebarDoubleClick(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) {
                Window win = (Window)
                    ((FrameworkElement)sender).TemplatedParent;
                if (win.WindowState == WindowState.Normal)
                    win.WindowState = WindowState.Maximized;
                else
                    win.WindowState = WindowState.Normal;
            }
        }

        private void OnButtonCloseClick(object sender, RoutedEventArgs e) {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            win.Close();
        }

        private void OnMaximizeButtonClick(object sender, RoutedEventArgs e) {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            if (win.WindowState == WindowState.Normal)
                win.WindowState = WindowState.Maximized;
            else
                win.WindowState = WindowState.Normal;
        }

        private void OnMinimizeButtonClick(object sender, RoutedEventArgs e) {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            win.WindowState = WindowState.Minimized;
        }
    }
}