using GalaSoft.MvvmLight.Ioc;
using StaffManager.ServiceImplementations;

namespace StaffManager
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
