using FlagsCheckBoxBindingDemo.Enums;
using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using System.Collections.Generic;

namespace FlagsCheckBoxBindingDemo
{
    public class MainViewModel : perViewModelBase
    {
        public MainViewModel()
        {
            Enum1Items = perEnumHelper.MakeValueDisplayPairsWithInclude<DemoFlagsEnum1>(e => e >= DemoFlagsEnum1.A1 && e <= DemoFlagsEnum1.E1);
            Enum2Items = perEnumHelper.MakeValueDisplayPairsWithExclude<DemoFlagsEnum2>(e =>
                e == DemoFlagsEnum2.Enum2None
                || e == DemoFlagsEnum2.Enum2GroupAll
                || e == DemoFlagsEnum2.Enum2GroupA2AndB2
                || e == DemoFlagsEnum2.Enum2GroupD2AndE2);
        }

        public IReadOnlyCollection<perValueDisplayPair<DemoFlagsEnum1>> Enum1Items { get; }

        public IReadOnlyCollection<perValueDisplayPair<DemoFlagsEnum2>> Enum2Items { get; }

        private DemoFlagsEnum1 _e1 = DemoFlagsEnum1.A1 | DemoFlagsEnum1.D1;

        public DemoFlagsEnum1 E1
        {
            get => _e1;
            set
            {
                if (Set(nameof(E1), ref _e1, value))
                {
                    RaisePropertyChanged(nameof(E1Description));
                }
            }
        }

        public string E1Description => E1.Description();

        private DemoFlagsEnum2 _e2 = DemoFlagsEnum2.C2 | DemoFlagsEnum2.F2 | DemoFlagsEnum2.E2;

        public DemoFlagsEnum2 E2
        {
            get => _e2;
            set
            {
                if (Set(nameof(E2), ref _e2, value))
                {
                    RaisePropertyChanged(nameof(E2Description));
                }
            }
        }

        public string E2Description => E2.Description();
    }
}