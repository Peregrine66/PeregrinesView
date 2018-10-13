using GalaSoft.MvvmLight.Ioc;

namespace StaffManager
{
    public partial class MainView
    {
        public MainView()
        {
            DataContext = SimpleIoc.Default.GetInstance<MainViewModel>();
            InitializeComponent();
        }
    }
}
