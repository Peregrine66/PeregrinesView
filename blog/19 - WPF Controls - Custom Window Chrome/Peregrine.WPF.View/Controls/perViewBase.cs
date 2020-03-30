using Peregrine.Library;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Peregrine.WPF.View.Controls
{
    public class perViewBase : Window
    {
        private Button _helpButton;

        static perViewBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(perViewBase), new FrameworkPropertyMetadata(typeof(perViewBase)));
        }

        public perViewBase()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }

        public override void OnApplyTemplate()
        {
            if (Template.FindName("PART_TitleBar", this) is Border titleBar)
            {
                titleBar.MouseLeftButtonDown += (s, e) => DragMove();
                titleBar.MouseLeftButtonDown += (s, e) =>
                {
                    if (e.ClickCount == 2
                        && ResizeMode == ResizeMode.CanResize)
                        ToggleWindowState();
                };
            }

            if (Template.FindName("PART_CloseButton", this) is Button closeButton)
                closeButton.Click += (s, e) => OnCloseButtonClick();

            if (Template.FindName("PART_MinimizeButton", this) is Button minimizeButton)
                minimizeButton.Click += (s, e) => WindowState = WindowState.Minimized;

            if (Template.FindName("PART_MaximizeButton", this) is Button maximizeButton)
                maximizeButton.Click += (s, e) => ToggleWindowState();

            _helpButton = Template.FindName("PART_HelpButton", this) as Button;
            if (_helpButton != null)
            {
                _helpButton.Click += (s, e) => new Process { StartInfo = { FileName = HelpFilePath } }.Start();
                SetHelpButtonVisibility();
            }

            var resizeGripNames = new[]
            {
                "LeftResizeGrip", "RightResizeGrip", "TopResizeGrip", "BottomResizeGrip",
                "TopLeftResizeGrip", "BottomLeftResizeGrip", "TopRightResizeGrip", "BottomRightResizeGrip"
            };

            foreach (var resizeGripName in resizeGripNames)
            {
                if (!(Template.FindName("PART_" + resizeGripName, this) is Rectangle resizeGrip))
                    continue;

                resizeGrip.MouseLeftButtonDown += Resize_Init;
                resizeGrip.MouseLeftButtonUp += Resize_End;
                resizeGrip.MouseMove += Resizing_View;
            }
        }

        protected virtual void OnCloseButtonClick()
        {
            Close();
        }

        private void ToggleWindowState()
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        public bool CanClose
        {
            get => (bool)GetValue(CanCloseProperty);
            set => SetValue(CanCloseProperty, value);
        }

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(perViewBase), new PropertyMetadata(false));

        public string HelpFileName
        {
            get => (string)GetValue(HelpFileNameProperty);
            set => SetValue(HelpFileNameProperty, value);
        }

        public static readonly DependencyProperty HelpFileNameProperty =
            DependencyProperty.Register("HelpFileName", typeof(string), typeof(perViewBase), new PropertyMetadata(null, HelpFileNameChanged));

        private static void HelpFileNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var view = sender as perViewBase;

            view?.SetHelpButtonVisibility();
        }

        private void SetHelpButtonVisibility()
        {
            if (_helpButton != null)
                _helpButton.Visibility = string.IsNullOrWhiteSpace(HelpFileName) || !File.Exists(HelpFilePath)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        protected string HelpFilePath
        {
            get
            {
                var exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                return string.IsNullOrWhiteSpace(exePath) ? string.Empty : System.IO.Path.Combine(exePath, HelpFileName);
            }
        }

        public UIElement TitleBarContent
        {
            get => (UIElement)GetValue(TitleBarContentProperty);
            set => SetValue(TitleBarContentProperty, value);
        }

        public static readonly DependencyProperty TitleBarContentProperty =
            DependencyProperty.Register("TitleBarContent", typeof(UIElement), typeof(perViewBase), new PropertyMetadata(null));

        bool _isResizing;

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Rectangle resizeGrip))
                return;

            _isResizing = true;
            resizeGrip.CaptureMouse();
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            _isResizing = false;

            if (!(sender is Rectangle resizeGrip))
                return;

            resizeGrip.ReleaseMouseCapture();
        }

        private void Resizing_View(object sender, MouseEventArgs e)
        {
            if (_isResizing)
            {
                if (!(sender is Rectangle resizeGrip))
                    return;

                if (!(resizeGrip.Tag is perViewBase view))
                    return;

                var width = e.GetPosition(view).X;
                var height = e.GetPosition(view).Y;

                if (resizeGrip.Name.CaseInsensitiveContains("right"))
                {
                    width += 5;
                    if (width > 0)
                        view.Width = width;
                }

                if (resizeGrip.Name.CaseInsensitiveContains("left"))
                {
                    width -= 5;
                    view.Left += width;
                    width = view.Width - width;

                    if (width > 0)
                        view.Width = width;
                }

                if (resizeGrip.Name.CaseInsensitiveContains("bottom"))
                {
                    height += 5;

                    if (height > 0)
                        view.Height = height;
                }

                if (resizeGrip.Name.CaseInsensitiveContains("top"))
                {
                    height -= 5;
                    view.Top += height;
                    height = view.Height - height;

                    if (height > 0)
                        view.Height = height;
                }
            }
        }
    }
}
