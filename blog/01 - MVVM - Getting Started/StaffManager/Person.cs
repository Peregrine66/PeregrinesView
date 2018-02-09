using System;
using System.ComponentModel;

namespace StaffManager
{
    public class Person : INotifyPropertyChanged, IComparable<Person>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _id;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                RaisePropertyChanged("FirstName");
                RaisePropertyChanged("DisplayName");
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                RaisePropertyChanged("LastName");
                RaisePropertyChanged("DisplayName");
            }
        }

        public string DisplayName => FirstName + " " + LastName;

        private string _department;

        public string Department
        {
            get { return _department; }
            set
            {
                _department = value;
                RaisePropertyChanged("Department");
            }
        }

        private bool _isManager;

        public bool IsManager
        {
            get { return _isManager; }
            set
            {
                _isManager = value;
                RaisePropertyChanged("IsManager");
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public int CompareTo(Person other)
        {
            var result = string.Compare(Department, other.Department, StringComparison.InvariantCultureIgnoreCase);
            if (result == 0)
                result = other.IsManager.CompareTo(IsManager); // sort true before false
            if (result == 0)
                result = string.Compare(LastName, other.LastName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }
    }
}