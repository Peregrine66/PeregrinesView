using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;

namespace Peregrine.WPF.Model
{
    public class perObservableObject: ObservableObject, IDataErrorInfo
    {
        private HashSet<string> InvalidProperties { get; } = new HashSet<string>();

        public virtual bool IsValid => !InvalidProperties.Any();

        public bool HasError(string propertyName) => InvalidProperties.Contains(propertyName);

        protected virtual string ValidateProperty(string propertyName) => string.Empty;

        public string this[string columnName]
        {
            get
            {
                if (nameof(IsValid).Equals(columnName))
                    return string.Empty;

                var result = ValidateProperty(columnName);

                var errorStateChanged = string.IsNullOrWhiteSpace(result)
                    ? InvalidProperties.Remove(columnName)
                    : InvalidProperties.Add(columnName);

                if (errorStateChanged)
                    RaisePropertyChanged(nameof(IsValid));

                return result;
            }
        }

        public string Error => string.Empty;
    }
}
