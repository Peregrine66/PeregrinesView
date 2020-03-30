using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using System.Collections.Generic;
using ValidationDemo.Model;
using ValidationDemo.Model.Enums;

namespace ValidationDemo
{
    public class MainViewModel : perViewModelBase
    {
        public MainViewModel()
        {
            Model = new PersonModel();
            AllAgeBands = perEnumHelper.MakeValueDisplayPairs<AgeBand>();
        }

        public PersonModel Model { get; }

        public IEnumerable<perValueDisplayPair<AgeBand>> AllAgeBands { get; }
    }
}