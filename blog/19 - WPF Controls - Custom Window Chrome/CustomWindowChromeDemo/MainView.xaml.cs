using Peregrine.WPF.ViewModel;
using System.Windows;
using Peregrine.WPF.ViewModel.Helpers;

namespace CustomWindowChromeDemo
{
    public partial class MainView
    { 
        public MainView()
        {
            InitializeComponent();
        }

        protected override async void OnCloseButtonClick()
        {
            if (!(DataContext is IAllowClose vm) || await vm.AllowCloseAsync().ConfigureAwait(false))
            {
                perDispatcherHelper.InvokeOnUIContext(Close);
            }
        }

        private void btnShowHollowWindow_Click(object sender, RoutedEventArgs e)
        {
            var hollowView = new HollowView
            {
                Width = 300,
                Height = 300,
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            hollowView.Show();
        }
    }
}
