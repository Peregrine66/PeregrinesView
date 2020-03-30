using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Peregrine.Library
{
    public static class perEnumHelper
    {
        public static string Description(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .ToList();
            return attributes.Any() ? attributes.First().Description : value.ToString();
        }

        // c# doesn't support where T: Enum - this is the best compromise
        public static IReadOnlyCollection<T> GetValues<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            var itemType = typeof(T);

            if (!itemType.IsEnum)
                throw new ArgumentException("Type '" + itemType.Name + "' is not an enum");

            var fields = itemType.GetFields().Where(field => field.IsLiteral);
            return new ReadOnlyCollection<T>(fields.Select(field => field.GetValue(itemType)).Cast<T>().ToList());
        }

        public static IReadOnlyCollection<perValueDisplayPair<T>> MakeValueDisplayPairs<T>(bool sortByDisplay = false) where T : struct, IComparable, IFormattable, IConvertible
        {
            var itemType = typeof(T);

            if (!itemType.IsEnum)
                throw new ArgumentException("Type '" + itemType.Name + "' is not an enum");

            var result = GetValues<T>().CreateSortedValuePairList(e => (e as Enum).Description(), sortByDisplay);

            return result;
        }
    }
}