namespace CustomWindowChromeDemo
{
    public partial class HollowView
    {
        // Some very non-MVVM code in this portion of demo project ...

        public HollowView()
        {
            InitializeComponent();
            MouseLeftButtonDown += (s, e) => DragMove();
        }

        private void CloseButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
