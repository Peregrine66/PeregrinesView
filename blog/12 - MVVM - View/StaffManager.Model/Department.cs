using System;

namespace StaffManager.Model
{
    public class Department : smModelBase, IComparable<Department>
    {
        public string Description { get; set; }

        public int CompareTo(Department other)
        {
            return Id.CompareTo(other.Id);
        }
    }
}