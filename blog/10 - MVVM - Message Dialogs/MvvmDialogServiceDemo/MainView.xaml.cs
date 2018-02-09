using GalaSoft.MvvmLight.Ioc;

namespace MvvmDialogServiceDemo
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
