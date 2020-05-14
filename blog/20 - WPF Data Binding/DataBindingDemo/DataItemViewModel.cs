using System.Windows.Media;
using Peregrine.WPF.ViewModel;

namespace DataBindingDemo
{
    public class DataItemViewModel: perViewModelBase
    {
        public DataItemViewModel(int index)
        {
            Name = "Item " + index;
            Description = "Description for " + Name;
        }

        public string Name { get; }

        private string _description;

            public string Description
            {
                get => _description;
                set
                {
                    if (Set(nameof(Description), ref _description, value))
                    {
                        RaisePropertyChanged(nameof(DescriptionValidColor));
                    }
                }
            }

            public Color DescriptionValidColor => Description.Length < 30 ? Colors.Red : Colors.LimeGreen;
    }
}
