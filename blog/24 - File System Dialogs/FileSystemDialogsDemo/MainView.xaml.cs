using Peregrine.WPF.ViewModel.IoC;

namespace FileSystemDialogsDemo
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
            DataContext = perIoC.GetInstance<MainViewModel>();
        }
    }
}
