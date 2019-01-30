using Peregrine.WPF.ViewModel.Helpers;
using Peregrine.WPF.ViewModel.IoC;

namespace StaffManager
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            Bootstrapper.Run();
        }

        public MainViewModel MainViewModel
        {
            get
            {
                var result = perIoC.GetInstance<MainViewModel>();

                // no loaded event when in designer
                if (perViewModelHelper.IsInDesignMode)
                    result.LoadDataCommand.Execute(null);

                return result;
            }
        }
    }
}
