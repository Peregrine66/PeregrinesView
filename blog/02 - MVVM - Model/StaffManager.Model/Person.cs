using System;

namespace StaffManager.Model
{
    public class Person: smModelBase, IComparable<Person>
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public bool IsManager { get; set; }

        // non-data properties, to be removed ...
        public string DisplayName => FirstName + " " + LastName;
        public Department Department { get; set; }
        public bool IsSelected { get; set; }

        public int CompareTo(Person other)
        {
            var result = DepartmentId.CompareTo(other.DepartmentId);
            if (result == 0)
                result = other.IsManager.CompareTo(IsManager); // sort true before false
            if (result == 0)
                result = string.Compare(LastName, other.LastName, StringComparison.InvariantCultureIgnoreCase);
            return result;
        }

    }
}
