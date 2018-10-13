using System;
using System.Globalization;
using System.Windows.Data;

namespace Peregrine.WPF.View.Converters
{
    public class perGenericBooleanConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolVal))
                return null;

            return ((bool?) boolVal).GetValueOrDefault() ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsEqual(value, TrueValue))
                return true;
            if (IsEqual(value, FalseValue))
                return false;
            return null;
        }

        private static bool IsEqual(object x, object y)
        {
            if (Equals(x, y))
                return true;

            var c = x as IComparable;
            return c?.CompareTo(y) == 0;
        }
    }

    public class perGenericBooleanConverter<T> : perGenericBooleanConverter
    {
        public new T TrueValue
        {
            get => (T) base.TrueValue;
            set => base.TrueValue = value;
        }

        public new T FalseValue
        {
            get => (T) base.FalseValue;
            set => base.FalseValue = value;
        }
    }
}