using GalaSoft.MvvmLight.Command;
using Peregrine.Library;
using Peregrine.WPF.ViewModel.DialogService.Enums;
using System.Windows;

namespace Peregrine.WPF.View.DialogService
{
    public partial class perDialog 
    {
        public perDialog()
        {
            ButtonClickCommand = new RelayCommand<perDialogButton>(OnButtonClick);
            InitializeComponent();
        }

        public perDialogIcon DialogIcon
        {
            get => (perDialogIcon)GetValue(MyPropertyProperty);
            set => SetValue(MyPropertyProperty, value);
        }

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(perDialogIcon), typeof(perDialog), new PropertyMetadata(perDialogIcon.Information));

        public object DialogContent
        {
            get => GetValue(DialogContentProperty);
            set => SetValue(DialogContentProperty, value);
        }

        public static readonly DependencyProperty DialogContentProperty =
            DependencyProperty.Register("DialogContent", typeof(object), typeof(perDialog), new PropertyMetadata(null));

        public perValueDisplayPair<perDialogButton>[] Buttons
        {
            get => (perValueDisplayPair<perDialogButton>[])GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public static readonly DependencyProperty ButtonsProperty =
            DependencyProperty.Register("Buttons", typeof(perValueDisplayPair<perDialogButton>[]), typeof(perDialog), new PropertyMetadata(null));

        public perDialogButton SelectedButton
        {
            get => (perDialogButton)GetValue(SelectedButtonProperty);
            set => SetValue(SelectedButtonProperty, value);
        }

        public static readonly DependencyProperty SelectedButtonProperty =
            DependencyProperty.Register("SelectedButton", typeof(perDialogButton), typeof(perDialog), new PropertyMetadata(perDialogButton.Ok));


        public RelayCommand<perDialogButton> ButtonClickCommand { get; }

        private void OnButtonClick(perDialogButton btn)
        {
            SelectedButton = btn;
            DialogResult = true;
        }
    }
}
