using System;

namespace StaffManager.Model
{
    public class Department : smModelBase, IComparable<Department>
    {
        public string Description { get; set; }

        // Sort Departments by Id so that we can sort Person models by DepartmentId (they don't know the department name)
        // and have them coming out in the corresponding order.
        public int CompareTo(Department other)
        {
            return Id.CompareTo(other.Id);
        }

        protected override string ValidateProperty(string propertyName)
        {
            var result = base.ValidateProperty(propertyName);

            if (nameof(Description).Equals(propertyName))
            {
                if (string.IsNullOrWhiteSpace(Description))
                    result = "Description is required";
            }

            return result;
        }
    }

    public class MockDepartment : Department
    {
        public MockDepartment()
        {
            Description = "Description";
        }
    }
}