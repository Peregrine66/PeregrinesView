using System;

namespace StaffManager.Model
{
    public class Person: smModelBase, IComparable<Person>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public bool IsManager { get; set; }
        
        public int CompareTo(Person other)
        {
            var result = DepartmentId.CompareTo(other.DepartmentId);

            if (result == 0)
                result = other.IsManager.CompareTo(IsManager); // sort True before False

            if (result == 0)
                result = string.Compare(LastName, other.LastName, StringComparison.InvariantCultureIgnoreCase);

            if (result == 0)
                result = string.Compare(FirstName, other.FirstName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }
    }
}
