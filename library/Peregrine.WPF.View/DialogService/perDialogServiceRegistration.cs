using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Peregrine.Library.Collections;

namespace Peregrine.WPF.View.DialogService
{
    public static class perDialogServiceRegistration
    {
        private static readonly perWeakWeakDictionary<object, DependencyObject> ContextRegistration = new perWeakWeakDictionary<object, DependencyObject>();
        private static readonly perWeakDictionary<string, DependencyObject> DialogContent = new perWeakDictionary<string, DependencyObject>();

        public static readonly DependencyProperty RegisterProperty = DependencyProperty.RegisterAttached(
            "Register",
            typeof(object),
            typeof(perDialogServiceRegistration),
            new PropertyMetadata(null, OnRegisterPropertyChanged));

        private static void OnRegisterPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.OldValue != null)
                ContextRegistration.Remove(args.OldValue);

            if (args.NewValue != null)
                ContextRegistration[args.NewValue] = source;
        }

        public static void SetRegister(Control element, object context)
        {
            element.SetValue(RegisterProperty, context);
        }

        public static object GetRegister(Control element)
        {
            return element.GetValue(RegisterProperty);
        }

        // get the view that has registered the specified object as its data context 
        internal static Control GetAssociatedControl(object dataContext)
        {
            if (dataContext == null)
                throw new ArgumentNullException(nameof(dataContext));

            var result = ContextRegistration.ContainsKey(dataContext) ? ContextRegistration[dataContext] : null;

            return result != null
                ? result as Control
                : null;
        }

        public static readonly DependencyProperty DialogContentProperty = DependencyProperty.RegisterAttached(
            "DialogContent",
            typeof(object),
            typeof(perDialogServiceRegistration),
            new PropertyMetadata(null, OnDialogContentChanged));

        private static string GetKey(DependencyObject source, string tag = null)
        {
            return source.GetHashCode() + (string.IsNullOrWhiteSpace(tag) ? string.Empty : "~" + tag);
        }

        private static void OnDialogContentChanged(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            var elements = args.NewValue as IEnumerable<FrameworkElement>;

            if (elements != null)
            {
                foreach (var element in elements)
                    AddElement(source, element);
            }
            else
            {
                var element = args.NewValue as FrameworkElement;

                if (element != null)
                    AddElement(source, element);
            }
        }

        private static void AddElement(DependencyObject source, FrameworkElement element)
        {
            var tag = element?.Tag?.ToString() ?? string.Empty;
            var key = GetKey(source, tag);

            DialogContent.Remove(key);

            if (element != null)
                DialogContent[key] = element;
        }

        public static void SetDialogContent(this DependencyObject obj, object value)
        {
            obj.SetValue(DialogContentProperty, value);
        }

        public static object GetDialogContent(this DependencyObject obj)
        {
            return obj.GetValue(DialogContentProperty);
        }

        internal static FrameworkElement GetDialogContentForHost(DependencyObject hostControl, string tag = null)
        {
            if (hostControl == null)
                throw new ArgumentNullException(nameof(hostControl));

            var key = GetKey(hostControl, tag);
            var result = DialogContent.ContainsKey(key) ? DialogContent[key] : null;
            return result as FrameworkElement;
        }
    }
}