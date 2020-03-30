using System.Collections.Generic;

namespace Peregrine.WPF.Model
{
    /// <summary>
    /// Base class for a model objects
    /// </summary>
    public abstract class perModelBase: perObservableObject
    {
        private static readonly Dictionary<string, HashSet<string>> PropertyDependencies = new Dictionary<string, HashSet<string>>();

        protected perModelBase()
        {
            PropertyChanged += (s, e) => CheckValidationDependencies(e.PropertyName);
        }

        /// <summary>
        /// Invoke PropertyChanged (to refresh the validation) for the dependent property when source property is updated
        /// </summary>
        /// <param name="sourcePropertyName"></param>
        /// <param name="dependentPropertyName"></param>
        protected static void AddValidationDependency(string sourcePropertyName, string dependentPropertyName)
        {
            if (!PropertyDependencies.ContainsKey(sourcePropertyName))
                PropertyDependencies[sourcePropertyName] = new HashSet<string>();

            PropertyDependencies[sourcePropertyName].Add(dependentPropertyName);
        }

        private void CheckValidationDependencies(string sourcePropertyName)
        {
            if (!PropertyDependencies.ContainsKey(sourcePropertyName))
                return;

            foreach(var dependentPropertyName in PropertyDependencies[sourcePropertyName])
                RaisePropertyChanged(dependentPropertyName);
        }
    }
}