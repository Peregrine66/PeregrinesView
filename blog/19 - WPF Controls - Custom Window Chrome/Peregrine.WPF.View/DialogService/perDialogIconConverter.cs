using Peregrine.WPF.ViewModel.DialogService.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Peregrine.WPF.View.DialogService
{
    public class perDialogIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is perDialogIcon))
                return null;

            switch ((perDialogIcon) value)
            {
                case perDialogIcon.Asterisk:
                    return AsteriskIcon;
                case perDialogIcon.Error:
                    return ErrorIcon;
                case perDialogIcon.Exclamation:
                    return ExclamationIcon;
                case perDialogIcon.Hand:
                    return HandIcon;
                case perDialogIcon.Information:
                    return InformationIcon;
                case perDialogIcon.Question:
                    return QuestionIcon;
                case perDialogIcon.Stop:
                    return StopIcon;
                case perDialogIcon.Warning:
                    return WarningIcon;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public UIElement AsteriskIcon { get; set; }
        public UIElement ErrorIcon { get; set; }
        public UIElement ExclamationIcon { get; set; }
        public UIElement HandIcon { get; set; }
        public UIElement InformationIcon { get; set; }
        public UIElement QuestionIcon { get; set; }
        public UIElement StopIcon { get; set; }
        public UIElement WarningIcon { get; set; }
    }
}