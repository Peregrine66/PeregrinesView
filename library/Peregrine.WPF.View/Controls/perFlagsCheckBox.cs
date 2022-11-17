using System;
using System.Windows;
using System.Windows.Controls;

namespace Peregrine.WPF.View.Controls
{
    /// <summary>
    /// An extended CheckBox control for selecting a specific item from a Flags Enum type
    /// </summary>
    public class perFlagsCheckBox : CheckBox
    {
        public perFlagsCheckBox()
        {
            Checked += (s, e) => IsCheckedChange();
            Unchecked += (s, e) => IsCheckedChange();
        }

        /// <summary>
        /// The overall selection from multiple perFlagsCheckBox controls
        /// Bind this against a property of the Flags Enum type
        /// </summary>
        public Enum FlagsEnumValue
        {
            get => (Enum) GetValue(FlagsEnumValueProperty);
            set => SetValue(FlagsEnumValueProperty, value);
        }

        public static readonly DependencyProperty FlagsEnumValueProperty =
            DependencyProperty.Register("FlagsEnumValue", typeof(Enum), typeof(perFlagsCheckBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnFlagsEnumChanged));

        private static void OnFlagsEnumChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!(obj is perFlagsCheckBox fcb))
            {
                return;
            }

            fcb.IsChecked = fcb.FlagsEnumValue.HasFlag(fcb.CheckedValue);
        }

        /// <summary>
        /// The value corresponding to this perFlagsCheckBox
        /// Bind this against a specific element from the Flags Enum
        /// </summary>
        public Enum CheckedValue
        {
            get => (Enum) GetValue(CheckedValueProperty);
            set => SetValue(CheckedValueProperty, value);
        }

        public static readonly DependencyProperty CheckedValueProperty =
            DependencyProperty.Register("CheckedValue", typeof(Enum), typeof(perFlagsCheckBox), new FrameworkPropertyMetadata(null));

        // update the FlagsEnum value when this perFlagsCheckBox is checked or unchecked
        private void IsCheckedChange()
        {
            if (FlagsEnumValue == null)
            {
                return;
            }

            // convert the flags enum value and checked value to the underlying type of the
            // enum (usually Int32), so that we can perform bitwise arithmetic on them.
            var enumType = FlagsEnumValue.GetType();
            var underlyingType = Enum.GetUnderlyingType(enumType);
            dynamic flagsEnumValueAsUnderlyingType = Convert.ChangeType(FlagsEnumValue, underlyingType);
            dynamic checkedValueAsUnderlyingType = Convert.ChangeType(CheckedValue, underlyingType);

            var newFlagsValueEnumAsUnderlyingType = IsChecked.GetValueOrDefault()
                ? flagsEnumValueAsUnderlyingType | checkedValueAsUnderlyingType
                : flagsEnumValueAsUnderlyingType & ~checkedValueAsUnderlyingType;

            if (newFlagsValueEnumAsUnderlyingType == flagsEnumValueAsUnderlyingType)
            {
                return;
            }

            // convert the new value back to the enum type
            FlagsEnumValue = Enum.ToObject(enumType, newFlagsValueEnumAsUnderlyingType);
        }
    }
}