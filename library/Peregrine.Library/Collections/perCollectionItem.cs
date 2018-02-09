namespace Peregrine.Library.Collections
{
    public interface IPERCollectionItem<out T>
    {
        T Data { get; }
        void MarkForDeletion();
    }
}
