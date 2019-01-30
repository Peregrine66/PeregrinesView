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
            var result = (DepartmentId).CompareTo(other.DepartmentId); 

            if (result == 0)
                result = other.IsManager.CompareTo(IsManager); // sort true before false

            if (result == 0)
                result = string.Compare(LastName, other.LastName, StringComparison.InvariantCultureIgnoreCase);

            if (result == 0)
                result = string.Compare(FirstName, other.FirstName, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        protected override string ValidateProperty(string propertyName)
        {
            var result = base.ValidateProperty(propertyName);

            if (nameof(FirstName).Equals(propertyName))
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    result = "First Name is required";
                }
            }
            else if (nameof(LastName).Equals(propertyName))
            {
                if (string.IsNullOrWhiteSpace(LastName))
                {
                    result = "First Name is required";
                }
            }

            return result;
        }
    }

    public class MockPerson: Person
    {
        public MockPerson()
        {
            FirstName = "First Name";
            LastName = "Last Name";
            DepartmentId = 1;
            IsManager = true;
        }
    }
}
