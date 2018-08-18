using System.Collections.Generic;

namespace Peregrine.WPF.Model
{
    /// <summary>
    /// Base class for a model objects
    /// </summary>
    public abstract class perModelBase: perObservableObject
    {
        private static readonly Dictionary<string, HashSet<string>> _propertyDependencies = new Dictionary<string, HashSet<string>>();

        protected perModelBase()
        {
            PropertyChanged += (s, e) => CheckValidationDepencies(e.PropertyName);
        }
        
        /// <summary>
        /// Invoke PropertyChanged (to refresh validation) for the dependent property when source property is updated
        /// </summary>
        /// <param name="sourceProperyName"></param>
        /// <param name="dependentPropertyName"></param>
        protected static void AddPropertyDependency(string sourceProperyName, string dependentPropertyName)
        {
            if (!_propertyDependencies.ContainsKey(sourceProperyName))
                _propertyDependencies[sourceProperyName] = new HashSet<string>();

            _propertyDependencies[sourceProperyName].Add(dependentPropertyName);
        }

        private void CheckValidationDepencies(string sourcePropertyName)
        {
            if (!_propertyDependencies.ContainsKey(sourcePropertyName))
                return;

            foreach(var dependentPropertyName in _propertyDependencies[sourcePropertyName])
                RaisePropertyChanged(dependentPropertyName);
        }
    }
}