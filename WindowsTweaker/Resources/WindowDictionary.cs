using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace WindowsTweaker.Resources
{
    public partial class WindowDictionary : ResourceDictionary
    {
        public WindowDictionary()
        {
            InitializeComponent();
        }

        private bool isResizing = false;
        [Flags()]
        private enum ResizeType
        {
            Width, Height
        }
        private ResizeType resizeType;


        private void window_ResizeInitiateWE(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isResizing = true;
            resizeType = ResizeType.Width;
        }
        private void windowResizeInitiateNS(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isResizing = true;
            resizeType = ResizeType.Height;
        }

        private void window_ResizeEnd(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isResizing = false;

            // Make sure capture is released.
            Rectangle rect = (Rectangle)sender;
            rect.ReleaseMouseCapture();
        }

        private void window_Resizing(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            Window win = (Window)rect.TemplatedParent;

            if (isResizing)
            {
                rect.CaptureMouse();
                if (resizeType == ResizeType.Width)
                {
                    double width = e.GetPosition(win).X + 5;
                    if (width > 0) win.Width = width;
                }
                if (resizeType == ResizeType.Height)
                {
                    double height = e.GetPosition(win).Y + 5;
                    if (height > 0) win.Height = height;
                }
            }
        }

        private void titleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            win.DragMove();
        }

        private void titlebar_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                Window win = (Window)
                    ((FrameworkElement)sender).TemplatedParent;
                if (win.WindowState == WindowState.Normal)
                    win.WindowState = WindowState.Maximized;
                else
                    win.WindowState = WindowState.Normal;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            win.Close();
        }

        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            if (win.WindowState == WindowState.Normal)
                win.WindowState = WindowState.Maximized;
            else
                win.WindowState = WindowState.Normal;
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            Window win = (Window)
                ((FrameworkElement)sender).TemplatedParent;
            win.WindowState = WindowState.Minimized;
        }
    }
}
