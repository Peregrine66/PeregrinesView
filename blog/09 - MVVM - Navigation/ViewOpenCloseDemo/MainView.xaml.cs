using GalaSoft.MvvmLight.Ioc;

namespace ViewOpenCloseDemo
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = SimpleIoc.Default.GetInstance<MainViewModel>();
        }
    }
}
