using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Peregrine.Library
{
    // Equivalent to KeyValuePair<object, string> but with more memorable property names for use with ComboBox controls
    // Bind ItemsSource to IEnumerable<ValueDisplayPair<>>, set DisplayMemberPath = Display, SelectedValuePath = Value, bind to SelectedValue
    public abstract class perValueDisplayPair
    {
        public object Value { get; protected set; }
        public string Display { get; protected set; }
    }

    // Equivalent to KeyValuePair<T, string> 
    public class perValueDisplayPair<T> : perValueDisplayPair
    {
        internal perValueDisplayPair(T value, string display)
        {
            Value = value;
            Display = display;
        }

        public new T Value { get; }
    }

    public static class perValueDisplayPairFactory
    {
        /// <summary>
        /// Create a ValueDisplayPair item from an object (using ToString() for Display value)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static perValueDisplayPair<T> CreateValueDisplayPair<T>(this T value)
        {
            return typeof(T).IsEnum
                ? new perValueDisplayPair<T>(value, (value as Enum).Description())
                : new perValueDisplayPair<T>(value, value.ToString());
        }

        /// <summary>
        /// Create a ValueDisplayPair item from an object and a string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public static perValueDisplayPair<T> CreateValueDisplayPair<T>(this T value, string display)
        {
            return new perValueDisplayPair<T>(value, display);
        }

        /// <summary>
        /// Create a collection of ValueDisplayPair items from a collection of objects, 
        /// using the getDisplay function to generate the Display value for each object.
        /// Items are optionally sorted by the Display strings (case insensitive)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="getDisplay"></param>
        /// <param name="sortItems"></param>
        /// <param name="includeDefaultItem"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<perValueDisplayPair<T>> CreateSortedValuePairList<T>(this IEnumerable<T> items, Func<T, string> getDisplay, bool sortItems = true, bool includeDefaultItem = false)
        {
            if (items == null)
                return null;

            var result = items.Select(i => i.CreateValueDisplayPair(getDisplay.Invoke(i))).ToList();

            if (includeDefaultItem)
                result.Add(CreateValueDisplayPair(default(T), string.Empty));

            if (sortItems)
            result.Sort((p1, p2) => string.Compare(p1.Display, p2.Display, StringComparison.InvariantCultureIgnoreCase));

            return new ReadOnlyCollection<perValueDisplayPair<T>>(result);
        }
    }
}