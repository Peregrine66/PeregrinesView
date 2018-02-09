namespace ViewOpenCloseDemo
{
    public class DataItem
    {
        public DataItem(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public int Id { get; }
        public string Description { get; }
    }
}
