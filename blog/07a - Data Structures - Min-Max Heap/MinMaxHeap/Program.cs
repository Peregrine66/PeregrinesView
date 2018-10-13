using Peregrine.Library.Collections;
using System;

namespace MinMaxHeapDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var RawItems = new[] { 45, 47, 19, 22, 117, 94, 58, 145, 1229, 14 };

            Console.WriteLine("Min Heap");
            Console.WriteLine("--------");

            IperHeap<int> minHeap = new perMinHeap<int>();

            foreach (var item in RawItems)
            {
                Console.WriteLine("Add " + item);
                minHeap.Add(item);
                Console.WriteLine("MinHeap = " + minHeap);
            }

            Console.WriteLine();

            while (minHeap.Any())
            {
                var item = minHeap.Remove();
                Console.WriteLine("Removed " + item);
                Console.WriteLine("MinHeap = " + minHeap);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Max Heap");
            Console.WriteLine("--------");

            IperHeap<int> maxHeap = new perMaxHeap<int>();

            foreach (var item in RawItems)
            {
                Console.WriteLine("Add " + item);
                maxHeap.Add(item);
                Console.WriteLine("MaxHeap = " + maxHeap);
            }

            Console.WriteLine();

            while (maxHeap.Any())
            {
                var item = maxHeap.Remove();
                Console.WriteLine("Removed " + item);
                Console.WriteLine("MaxHeap = " + maxHeap);
            }

            Console.WriteLine();
            Console.WriteLine("Press [Return] to continue");
            Console.ReadLine();
        }
    }
}
