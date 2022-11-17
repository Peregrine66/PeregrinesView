using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Peregrine.WPF.Model
{
    /// <summary>
    /// Type-safe Clone() / AssignTo() methods for all perModelBase descendent types
    /// </summary>
    public static class perModelBaseHelper
    {
        private static readonly Dictionary<Type, ReadOnlyCollection<PropertyInfo>> TypeProperties = new Dictionary<Type, ReadOnlyCollection<PropertyInfo>>();

        /// <summary>
        /// Get a list of all Read / Write properties for the specified type.
        /// Cache the results in a dictionary so we only need to process each type once.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ReadOnlyCollection<PropertyInfo> GetReadWritePropertiesForType(Type type)
        {
            if (!TypeProperties.ContainsKey(type))
            {
                var readWriteProps = type
                    .GetProperties()
                    .Where(p => p.CanRead && p.CanWrite)
                    .ToList()
                    .AsReadOnly();

                TypeProperties[type] = readWriteProps;
            }

            return TypeProperties[type];
        }

        public static T CloneAllFields<T>(this T source) where T : perModelBase
        {
            if (!(Activator.CreateInstance(source.GetType()) is T result))
            {
                return null;
            }

            source.AssignToAllFields(result);
            return result;
        }

        public static void AssignToAllFields<T>(this T source, T target) where T : perModelBase
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // Copy all read / write properties of the source item
            var propertiesToSet = GetReadWritePropertiesForType(source.GetType());

            foreach (var prop in propertiesToSet)
            {
                prop.SetValue(target, prop.GetValue(source));
            }

            target.RaisePropertyChanged(string.Empty);
        }

        public static T CloneExcludingId<T>(this T source) where T : perModelBase
        {
            if (!(Activator.CreateInstance(source.GetType()) is T result))
            {
                return null;
            }

            source.AssignToExcludingId(result);

            return result;
        }

        public static void AssignToExcludingId<T>(this T source, T target) where T : perModelBase
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            // Copy all read / write properties of the source item, except for Id
            var propertiesToSet = GetReadWritePropertiesForType(source.GetType())
                .Where(p => !p.Name.ToLower().Equals("id"));

            foreach (var prop in propertiesToSet)
            {
                prop.SetValue(target, prop.GetValue(source));
            }

            target.RaisePropertyChanged(string.Empty);
        }
    }
}