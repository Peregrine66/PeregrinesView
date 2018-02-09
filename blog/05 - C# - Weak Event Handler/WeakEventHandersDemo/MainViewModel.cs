using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows.Input;
using System.Windows.Threading;
using Peregrine.WPF.ViewModel.WeakPropertyChanged;

namespace WeakEventHandlerDemo
{
    public class MainViewModel: ViewModelBase
    {
        public MainViewModel()
        {
            TimerTick();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(0.1)
            };

            timer.Tick += (s,e) => TimerTick();
            timer.Start();

            ForceGarbageCollectCommand = new RelayCommand(OnForceGarbageCollect);
            CreateBlankChildWindowCommand = new RelayCommand(OnCreateBlankChildWindow);
            BadCreateChildWindowWithTimeCommand = new RelayCommand(OnBadCreateChildWindowWithTime);
            GoodCreateChildWindowWithTimeCommand = new RelayCommand(OnGoodCreateChildWindowWithTime);
            ClearWeakListenersCommand = new RelayCommand(OnClearWeakListeners);
        }

        private void OnForceGarbageCollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void TimerTick()
        {
            CurrentDateTime = DateTime.Now;
            CurrentTotalMemory = GC.GetTotalMemory(false);
        }

        private DateTime _currentDateTime;

        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            set { Set(nameof(CurrentDateTime), ref _currentDateTime, value); }
        }

        private long _currentTotalMemory;

        public long CurrentTotalMemory
        {
            get { return _currentTotalMemory; }
            set { Set(nameof(CurrentTotalMemory), ref _currentTotalMemory, value); }
        }

        public ICommand ForceGarbageCollectCommand { get; }

        public ICommand CreateBlankChildWindowCommand { get; }

        private void OnCreateBlankChildWindow()
        {
            var childView = new ChildView();
            var childViewModel = new ChildViewModel();
            childView.DataContext = childViewModel;
            childView.Show();
        }

        public ICommand BadCreateChildWindowWithTimeCommand { get; }

        private void OnBadCreateChildWindowWithTime()
        {
            var childView = new ChildView();
            var childViewModel = new ChildViewModel();
            PropertyChanged += (s, e) =>
            {
                if (nameof(CurrentDateTime).Equals(e.PropertyName))
                    childViewModel.CurrentDateTime = CurrentDateTime;
            };
            childView.DataContext = childViewModel;
            childView.Show();
        }

        public ICommand GoodCreateChildWindowWithTimeCommand { get; }

        private void OnGoodCreateChildWindowWithTime()
        {
            var childView = new ChildView();
            var childViewModel = new ChildViewModel();

            perWeakPropertyChangedEventHandler.Register(this, nameof(CurrentDateTime), childViewModel, (l, s, e) => l.CurrentDateTime = CurrentDateTime);

            childView.DataContext = childViewModel;
            childView.Show();
        }

        public ICommand ClearWeakListenersCommand { get; }

        private void OnClearWeakListeners()
        {
            perWeakPropertyChangedEventHandler.UnregisterSource(this);
        }
    }
}
