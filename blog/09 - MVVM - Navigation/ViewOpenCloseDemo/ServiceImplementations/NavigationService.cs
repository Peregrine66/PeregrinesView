using System.Windows;
using ViewOpenCloseDemo.ServiceContracts;

namespace ViewOpenCloseDemo.ServiceImplementations
{
    public class NavigationService : INavigationService
    {
        public void ShowDataItem(DataItem dataItem)
        {
            var view = new DataItemView();
            var viewModel = new DataItemViewModel(dataItem);
            view.DataContext = viewModel;
            view.Owner = Application.Current.MainWindow;
            view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            view.ShowDialog();
        }
    }
}