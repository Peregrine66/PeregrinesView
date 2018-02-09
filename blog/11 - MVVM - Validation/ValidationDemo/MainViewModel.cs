using Peregrine.Library;
using Peregrine.WPF.ViewModel;
using System.Collections.Generic;
using System.Linq;
using ValidationDemo.Model;

namespace ValidationDemo
{
    public class MainViewModel : perViewModelBase
    {
        public MainViewModel()
        {
            Model = new PersonModel();
            AllAgeBands = perEnumExtender.MakeValueDisplayPairs<AgeBand>();
        }

        public PersonModel Model { get; }

        public IEnumerable<perValueDisplayPair<AgeBand>> AllAgeBands { get; }
    }
}