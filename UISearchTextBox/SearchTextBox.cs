using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace UISearchTextBox {
    public enum SearchMode {
        Instant,
        Delayed
    }

    public class SearchTextBox : TextBox {
        public static DependencyProperty LabelTextProperty =
            DependencyProperty.Register(
                "LabelText",
                typeof (string),
                typeof (SearchTextBox));

        public static DependencyProperty LabelTextColorProperty =
            DependencyProperty.Register(
                "LabelTextColor",
                typeof (Brush),
                typeof (SearchTextBox));

        public static DependencyProperty SearchModeProperty =
            DependencyProperty.Register(
                "SearchMode",
                typeof (SearchMode),
                typeof (SearchTextBox),
                new PropertyMetadata(SearchMode.Instant));

        private static DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "HasText",
                typeof (bool),
                typeof (SearchTextBox),
                new PropertyMetadata());

        public static DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        private static DependencyPropertyKey IsMouseLeftButtonDownPropertyKey =
            DependencyProperty.RegisterReadOnly(
                "IsMouseLeftButtonDown",
                typeof (bool),
                typeof (SearchTextBox),
                new PropertyMetadata());

        private static DependencyProperty IsMouseLeftButtonDownProperty =
            IsMouseLeftButtonDownPropertyKey.DependencyProperty;

        public static DependencyProperty SearchEventTimeDelayProperty =
            DependencyProperty.Register(
                "SearchEventTimeDelay",
                typeof (Duration),
                typeof (SearchTextBox),
                new FrameworkPropertyMetadata(
                    new Duration(new TimeSpan(0, 0, 0, 0, 500)),
                    new PropertyChangedCallback(OnSearchEventTimeDelayChanged)));

        public static readonly RoutedEvent SearchEvent = EventManager.RegisterRoutedEvent(
            "Search", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (SearchTextBox));

        public static readonly RoutedEvent SearchDismissEvent = EventManager.RegisterRoutedEvent(
            "SearchDismiss", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (SearchTextBox));

        private DispatcherTimer searchEventDelayTimer;

        public SearchTextBox() : base() {
            this.Cursor = Cursors.IBeam;
            searchEventDelayTimer = new DispatcherTimer {Interval = SearchEventTimeDelay.TimeSpan};
            searchEventDelayTimer.Tick += searchEventDelayTimer_Tick;
        }

        private void searchEventDelayTimer_Tick(object sender, EventArgs e) {
            searchEventDelayTimer.Stop();
            RaiseSearchEvent();
        }

        private static void OnSearchEventTimeDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            SearchTextBox txtSearch = d as SearchTextBox;
            if (txtSearch == null) return;
            txtSearch.searchEventDelayTimer.Interval = ((Duration) e.NewValue).TimeSpan;
            txtSearch.searchEventDelayTimer.Stop();
        }

        private void RaiseSearchEvent() {
            RoutedEventArgs args = new RoutedEventArgs(SearchEvent);
            RaiseEvent(args);
        }

        private void RaiseSearchDismissEvent() {
            RoutedEventArgs args = new RoutedEventArgs(SearchDismissEvent);
            RaiseEvent(args);
        }

        private void DismissSearch() {
            this.Text = "";
            RaiseSearchDismissEvent();
        }

        static SearchTextBox() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SearchTextBox),
                new FrameworkPropertyMetadata(typeof (SearchTextBox)));
        }

        protected override void OnTextChanged(TextChangedEventArgs e) {
            base.OnTextChanged(e);
            HasText = this.Text.Length != 0;
            if (SearchMode == SearchMode.Instant) {
                searchEventDelayTimer.Stop();
                searchEventDelayTimer.Start();
            }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            Border iconBorder = GetTemplateChild("PART_SearchIconBorder") as Border;
            if (iconBorder == null) return;
            iconBorder.Cursor = Cursors.Arrow;
            iconBorder.MouseLeftButtonDown += iconBorder_MouseLeftButtonDown;
            iconBorder.MouseLeftButtonUp += iconBorder_MouseLeftButtonUp;
            iconBorder.MouseLeave += iconBorder_MouseLeave;
        }

        private void iconBorder_MouseLeave(object sender, MouseEventArgs e) {
            IsMouseLeftButtonDown = false;
        }

        private void iconBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (!IsMouseLeftButtonDown) return;
            if (HasText) {
                switch (SearchMode) {
                    case SearchMode.Instant:
                        DismissSearch();
                        break;
                    case SearchMode.Delayed:
                        RaiseSearchEvent();
                        break;
                }
            }
            IsMouseLeftButtonDown = false;
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            if (e.Key == Key.Escape && SearchMode == SearchMode.Instant) {
                DismissSearch();
            }
            else if ((e.Key == Key.Enter || e.Key == Key.Return) && SearchMode == SearchMode.Delayed) {
                RaiseSearchEvent();
            }
            else {
                base.OnKeyDown(e);
            }
        }

        private void iconBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            IsMouseLeftButtonDown = true;
        }

        public string LabelText {
            get { return (string) GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public Brush LabelTextColor {
            get { return (Brush) GetValue(LabelTextColorProperty); }
            set { SetValue(LabelTextColorProperty, value); }
        }

        public SearchMode SearchMode {
            get { return (SearchMode) GetValue(SearchModeProperty); }
            set { SetValue(SearchModeProperty, value); }
        }

        public bool HasText {
            get { return (bool) GetValue(HasTextProperty); }
            private set { SetValue(HasTextPropertyKey, value); }
        }

        public bool IsMouseLeftButtonDown {
            get { return (bool) GetValue(IsMouseLeftButtonDownProperty); }
            set { SetValue(IsMouseLeftButtonDownPropertyKey, value); }
        }

        public Duration SearchEventTimeDelay {
            get { return (Duration) GetValue(SearchEventTimeDelayProperty); }
            set { SetValue(SearchEventTimeDelayProperty, value); }
        }

        public event RoutedEventHandler Search {
            add { AddHandler(SearchEvent, value); }
            remove { RemoveHandler(SearchEvent, value); }
        }

        public event RoutedEventHandler SearchDismiss {
            add { AddHandler(SearchDismissEvent, value); }
            remove { RemoveHandler(SearchDismissEvent, value); }
        }
    }
}