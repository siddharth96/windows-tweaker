using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using System.Linq;

namespace WindowsTweaker.AppTasks
{
    public sealed class ColorPickerDialog : CommonDialog
    {
        private Color? _selectedColour;
        private DockPanel _colourPickerDockPanel;
        private Window _colourPickerWindow;

        public ColorPickerDialog(Color selectedColour)
        {
            Title = "Choose Colour";
            _selectedColour = selectedColour;
            Reset();
        }

        public override void Reset()
        {
            _colourPickerDockPanel = CreateColourPickerDockPanel();
            _colourPickerWindow = CreateColourPickerWindow();
        }

        protected override bool RunDialog(System.IntPtr hwndOwner)
        {
            bool dialogResult = false;
            _colourPickerWindow.Title = Title;
            bool? windowResult = _colourPickerWindow.ShowDialog();
            if (windowResult.HasValue && windowResult.Value)
            {
                _selectedColour = _colourPickerDockPanel.Children.OfType<ColorCanvas>().First().SelectedColor;
                dialogResult = true;
            }
            return dialogResult;
        }

        private DockPanel CreateColourPickerDockPanel()
        {
            DockPanel dockPanel = new DockPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(4),
            };

            dockPanel.Children.Add(new ColorCanvas() { SelectedColor = SelectedColour, Margin = new Thickness(4) });

            dockPanel.Children.Add(new Button()
            {
                IsDefault = true,
                Padding = new Thickness(16, 4, 16, 4),
                Margin = new Thickness(4),
                Content = "Select Color",
                HorizontalAlignment = HorizontalAlignment.Center
            });

            dockPanel.Children.OfType<ColorCanvas>().First().SetValue(DockPanel.DockProperty, Dock.Top);
            dockPanel.Children.OfType<Button>().First().Click += new RoutedEventHandler(ColourPickerDialogOkay_Click);

            return dockPanel;
        }

        private void ColourPickerDialogOkay_Click(object sender, RoutedEventArgs e)
        {
            _colourPickerWindow.DialogResult = true;
        }

        private Window CreateColourPickerWindow()
        {
            return new Window()
            {
                Title = Title,
                Content = _colourPickerDockPanel,
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight
            };
        }

        public string Title { get; set; }

        public Color SelectedColour
        {
            get
            {
                return _selectedColour.HasValue ? _selectedColour.Value : Colors.Transparent;
            }
        }

    }
}
