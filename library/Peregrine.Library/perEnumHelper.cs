using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Peregrine.Library
{
    public static class perEnumHelper
    {
        public static string Description(this Enum value)
        {
            // if this is a Flags enum, value may contain multiple items
            var values = value.ToString().Split(',').Select(s => s.Trim()).ToList();
            var enumType = value.GetType();

            var result = string.Join(" | ", values.Select(enumValue => enumType.GetMember(enumValue)
                                                                           .FirstOrDefault()
                                                                           ?.GetCustomAttribute<DescriptionAttribute>()
                                                                           ?.Description
                                                                       ?? enumValue.ToString()));

            return result;
        }

        // c# doesn't support where T: Enum - this is the best compromise
        public static IReadOnlyCollection<T> GetValues<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            var itemType = typeof(T);

            if (!itemType.IsEnum)
            {
                throw new ArgumentException("Type '" + itemType.Name + "' is not an enum");
            }

            var fields = itemType
                .GetFields()
                .Where(field => field.IsLiteral)
                .ToList();

            return fields
                .Select(field => field.GetValue(itemType))
                .Cast<T>()
                .ToList()
                .AsReadOnly();
        }

        public static IReadOnlyCollection<perValueDisplayPair<T>> MakeValueDisplayPairs<T>(bool sortByDisplay = false) where T : struct, IComparable, IFormattable, IConvertible
        {
            var result = GetValues<T>()
                .Select(e => e.CreateValueDisplayPair())
                .ToList();

            if (sortByDisplay)
            {
                result.Sort((p1, p2) => string.Compare(p1.Display, p2.Display, StringComparison.InvariantCultureIgnoreCase));
            }

            return result.AsReadOnly();
        }

        public static IReadOnlyCollection<perValueDisplayPair<T>> MakeValueDisplayPairsWithExclude<T>(Func<T, bool> exclude, bool sortByDisplay = false) where T : struct, IComparable, IFormattable, IConvertible
        {
            var result = GetValues<T>()
                .Where(e=> !exclude(e))
                .Select(e => e.CreateValueDisplayPair())
                .ToList();

            if (sortByDisplay)
            {
                result.Sort((p1, p2) => string.Compare(p1.Display, p2.Display, StringComparison.InvariantCultureIgnoreCase));
            }

            return result.AsReadOnly();
        }

        public static IReadOnlyCollection<perValueDisplayPair<T>> MakeValueDisplayPairsWithInclude<T>(Func<T, bool> include, bool sortByDisplay = false) where T : struct, IComparable, IFormattable, IConvertible
        {
            var result = GetValues<T>()
                .Where(include)
                .Select(e => e.CreateValueDisplayPair())
                .ToList();

            if (sortByDisplay)
            {
                result.Sort((p1, p2) => string.Compare(p1.Display, p2.Display, StringComparison.InvariantCultureIgnoreCase));
            }

            return result.AsReadOnly();
        }
    }
}