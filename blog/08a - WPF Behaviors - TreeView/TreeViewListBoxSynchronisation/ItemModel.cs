namespace TreeViewListBoxSynchronisation
{
    public class ItemModel
    {
        public ItemModel(int id, string caption, int level, int parentId = 0)
        {
            Id = id;
            Caption = caption;
            ParentId = parentId;
            Level = level;
        }

        public int Id { get; }
        public string Caption { get; }
        public int ParentId { get; }
        public int Level { get; }

        public override string ToString()
        {
            return $"{Id} / {ParentId} - {Caption}";
        }
    }
}