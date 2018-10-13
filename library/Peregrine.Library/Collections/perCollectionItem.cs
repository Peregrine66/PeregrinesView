namespace Peregrine.Library.Collections
{
    public interface IperCollectionItem<out T>
    {
        T Data { get; }
        void MarkForDeletion();
    }
}
