using Peregrine.WPF.ViewModel;
using RadioButtonBindingDemo.Enums;

namespace RadioButtonBindingDemo
{
    public class MainViewModel : perViewModelBase
    {
        private DemoEnum1 _e1 = DemoEnum1.B1;

        public DemoEnum1 E1
        {
            get => _e1;
            set => Set(nameof(E1), ref _e1, value);
        }

        private DemoEnum2 _e2 = DemoEnum2.C2;

        public DemoEnum2 E2
        {
            get => _e2;
            set => Set(nameof(E2), ref _e2, value);
        }
    }
}