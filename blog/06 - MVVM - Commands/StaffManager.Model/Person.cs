namespace StaffManager.Model
{
    public class Person: smModelBase
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int DepartmentId { get; set; }
        public bool IsManager { get; set; }
    }
}
