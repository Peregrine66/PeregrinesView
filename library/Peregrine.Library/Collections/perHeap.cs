using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Peregrine.Library.Collections
{
    /// <summary>
    /// a binary heap arranged with the lowest ranking item at the top of the heap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class perMinHeap<T> : perBaseHeap<T> where T : IComparable<T>
    {
        public perMinHeap()
            : base(HeapType.Min)
        {
        }
    }

    // ================================================================================

    /// <summary>
    /// a binary heap arranged with the highest ranking item at the top of the heap
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class perMaxHeap<T> : perBaseHeap<T> where T : IComparable<T>
    {
        public perMaxHeap()
            : base(HeapType.Max)
        {
        }
    }

    // ================================================================================

    public abstract class perBaseHeap<T> where T : IComparable<T>
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
            Clear();
        }

        /// <summary>
        ///  Clear all items from the heap, and reset status
        /// </summary>
        public void Clear()
        {
            Count = 0;
            Heap = new T[_capacity];
        }

        private T[] Heap { get; set; }

        /// <summary>
        /// How many items currently in the heap
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Are there any items in the heap?
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return Count > 0;
        }

        /// <summary>
        /// Add a new item to the heap and re-order so the best item is at the top of the heap
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
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

            // clean up the heap, moving the new item to the appropriate level
            var currentIndex = Count - 1;
            while (currentIndex > 0)
            {
                var parentIndex = (currentIndex - 1) / 2;
                if (RequiresSwap(currentIndex, parentIndex))
                {
                    SwapItems(parentIndex, currentIndex);
                    currentIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Add multiple items to the heap
        /// </summary>
        /// <param name="items"></param>
        public void Enqueue(params T[] items)
        {
            foreach (var item in items)
            {
                Enqueue(item);
            }
        }

        /// <summary>
        /// Remove an item from the heap and re-order so the best remaining item is at the top of the heap
        /// </summary>
        /// <remarks>
        /// Note that by rule, the two items in level 1 (array indexes 1 & 2), if they exist, each rank better than all of their children,
        /// and so one of these will be the new best item in the heap.
        /// </remarks>
        /// <returns></returns>
        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException($"{nameof(Dequeue)}() called on an empty heap");
            }

            var result = Heap[0];

            Count--;
            if (Count > 0)
            {
                SwapItems(0, Count);
            }

            Heap[Count] = default(T);

            // re-sort the heap as the previous last item in the array was swapped with the removed item (in index 0)
            var currentIndex = 0;

            // Keep walking the heap, placing best of three items (current + its two children) in highest position,
            // until no further swaps are required
            while (true)
            {
                var bestItemIndex = currentIndex;
                var leftChildIndex = currentIndex * 2 + 1;
                var rightChildIndex = currentIndex * 2 + 2;

                if (leftChildIndex < Count && RequiresSwap(leftChildIndex, currentIndex))
                {
                    bestItemIndex = leftChildIndex;
                }

                if (rightChildIndex < Count && RequiresSwap(rightChildIndex, bestItemIndex))
                {
                    bestItemIndex = rightChildIndex;
                }

                if (bestItemIndex == currentIndex)
                {
                    break;
                }

                SwapItems(currentIndex, bestItemIndex);
                currentIndex = bestItemIndex;
            }

            return result;
        }

        /// <summary>
        /// Returns the item at the top of the heap without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException($"{nameof(Peek)}() called on an empty heap");
            }

            return Heap[0];
        }

        /// <summary>
        /// Swap the two items at indexA and indexB
        /// </summary>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        private void SwapItems(int indexA, int indexB)
        {
            if (indexA == indexB)
            {
                return;
            }

            var temp = Heap[indexA];
            Heap[indexA] = Heap[indexB];
            Heap[indexB] = temp;
        }

        /// <summary>
        /// returns true if the item in indexA ranks worse than the item in indexB
        /// </summary>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <returns></returns>
        private bool RequiresSwap(int indexA, int indexB)
        {
            if (indexA >= Count || indexB >= Count)
            {
                return false;
            }

            var compare = Heap[indexA].CompareTo(Heap[indexB]);

            return (_heapType == HeapType.Min && compare < 0) || (_heapType == HeapType.Max && compare > 0);
        }

        public override string ToString()
        {
            return string.Join(" ", Heap.Take(Count));
        }
    }
}