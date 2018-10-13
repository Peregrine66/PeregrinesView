using System;
using System.Diagnostics;
using GalaSoft.MvvmLight;

namespace WeakEventHandlerDemo
{
    public class ChildViewModel: ViewModelBase
    {
        private static int _instanceCount = 1;

        public ChildViewModel()
        {
            InstanceId = _instanceCount++;
        }

        ~ChildViewModel()
        {
            // confirmation of this object being destroyed
            Debug.WriteLine("ChildViewModel Destructor - InstanceID = " + InstanceId);
        }

        // dummy array so we can clearly see the allocated memory change when this object is created / destroyed
        public int[] Unused = new int[10 * 1000 * 1000];

        public int InstanceId { get; }

        private DateTime _currentDateTime;

        public DateTime CurrentDateTime
        {
            get => _currentDateTime;
            set => Set(nameof(CurrentDateTime), ref _currentDateTime, value);
        }
    }
}
