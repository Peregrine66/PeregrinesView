namespace WpfColorspaceDemo
{
    public partial class MainView
    {
        public MainView()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}
