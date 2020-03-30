using System;
using System.Linq;

namespace Peregrine.Library.Collections
{
    // A Collection class where the Lowest / Highest sorting value is kept at the top of the heap
    // The remaining items are kept in an optimised manner, without being precisely in sorted order

    public interface IperHeap<T> where T : IComparable<T>
    {
        int Count { get; }
        bool Any();

        void Add(T item);
        void Add(params T[] items);
        T Remove();
        T Peek();
    }

    // ================================================================================

    public class perMinHeap<T> : perBaseHeap<T> where T : IComparable<T>
    {
        public perMinHeap()
            : base(HeapType.Min)
        {
        }
    }

    // ================================================================================

    public class perMaxHeap<T> : perBaseHeap<T> where T : IComparable<T>
    {
        public perMaxHeap()
            : base(HeapType.Max)
        {
        }
    }

    // ================================================================================

    public abstract class perBaseHeap<T>: IperHeap<T> where T : IComparable<T>
    {
        protected enum HeapType
        {
            Min,
            Max
        }

        private readonly HeapType _heapType;
        private int _capacity = 15;

        protected perBaseHeap(HeapType heapType)
        {
            _heapType = heapType;
            Reset();
        }

        private T[] Heap { get; set; }
        public int Count { get; private set; }

        public bool Any()
        {
            return Count > 0;
        }

        public void Reset()
        {
            Count = 0;
            _capacity = 15;
            Heap = new T[_capacity];
        }

        // Add a new item to the heap, then rearrange the items so that the highest / lowest item is at the top
        public void Add(T item)
        {
            // grow the heap array if necessary
            if (Count == _capacity)
            {
                _capacity = _capacity * 2 + 1;
                var newHeap = new T[_capacity];
                Array.Copy(Heap, 0, newHeap, 0, Count);
                Heap = newHeap;
            }

            Heap[Count] = item;
            Count++;

            // The new item was added in last element of array (index = Count-1)
            // Now rearrange the heap, staring from the last item, so that each parent node is sorted higher than both of its children
            var currentIndex = Count - 1;
            while (currentIndex > 0)
            {
                var parentIndex = (currentIndex - 1) / 2;

                if (IsBetter(currentIndex, parentIndex))
                {
                    SwapItems(parentIndex, currentIndex);
                    currentIndex = parentIndex;
                }
                else
                    break; // once no swap is required then the heap arrangement is valid
            }
        }

        public void Add(params T[] items)
        {
            foreach (var item in items)
                Add(item);
        }

        // Return the highest / lowest value from the top of the heap, then re-arrange the remaining
        // items so that the next highest / lowest item is moved to the top.
        public T Remove()
        {
            if (Count == 0)
                throw new InvalidOperationException($"{nameof(Remove)}() called on an empty heap");

            var result = Heap[0];

            Count--;
            if (Count > 0)
                SwapItems(0, Count);

            Heap[Count] = default(T);

            // The Last item in the heap was swapped with the removed item (index 0), which is then effectively discarded as the heap count is reduced by 1.
            // Now rearrange the heap, starting from the top item, comparing it to each of its children, swapping to promote the best to that slot.
            // Repeat as necessary with the demoted item.
            var currentIndex = 0;

            while (true)
            {
                var bestItemIndex = currentIndex;
                var leftChildIndex = currentIndex * 2 + 1;
                var rightChildIndex = currentIndex * 2 + 2;

                if (leftChildIndex < Count && IsBetter(leftChildIndex, currentIndex))
                    bestItemIndex = leftChildIndex;

                if (rightChildIndex < Count && IsBetter(rightChildIndex, bestItemIndex))
                    bestItemIndex = rightChildIndex;

                if (bestItemIndex == currentIndex)
                    break;

                SwapItems(currentIndex, bestItemIndex);
                currentIndex = bestItemIndex;
            }

            return result;
        }

        // Return the highest / lowest item without removing it.
        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException($"{nameof(Peek)}() called on an empty heap");

            return Heap[0];
        }

        // returns true if the item at indexA ranks better than the item at indexB
        private bool IsBetter(int indexA, int indexB)
        {
            if (indexA >= Count || indexB >= Count)
                return false;

            var compare = Heap[indexA].CompareTo(Heap[indexB]);

            return (_heapType == HeapType.Min && compare < 0) || (_heapType == HeapType.Max && compare > 0);
        }

        private void SwapItems(int indexA, int indexB)
        {
            if (indexA == indexB)
                return;

            var temp = Heap[indexA];
            Heap[indexA] = Heap[indexB];
            Heap[indexB] = temp;
        }
        
        public override string ToString()
        {
            return string.Join(" ", Heap.Take(Count));
        }
    }
}
