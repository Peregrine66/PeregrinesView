using Peregrine.Library.Collections;
using System;

namespace MinMaxHeapDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var rawItems = new[] { 45, 47, 19, 22, 117, 94, 58, 145, 1229, 14 };

            Console.WriteLine("Min Heap");
            Console.WriteLine("--------");

            perMinHeap<int> minHeap = new perMinHeap<int>();

            foreach (var item in rawItems)
            {
                Console.WriteLine("Enqueue " + item);
                minHeap.Enqueue(item);
                Console.WriteLine("MinHeap = " + minHeap);
            }

            Console.WriteLine();

            while (minHeap.Any())
            {
                var item = minHeap.Dequeue();
                Console.WriteLine("Dequeue " + item);
                Console.WriteLine("MinHeap = " + minHeap);
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Max Heap");
            Console.WriteLine("--------");

            perMaxHeap<int> maxHeap = new perMaxHeap<int>();

            foreach (var item in rawItems)
            {
                Console.WriteLine("Enqueue " + item);
                maxHeap.Enqueue(item);
                Console.WriteLine("MaxHeap = " + maxHeap);
            }

            Console.WriteLine();

            while (maxHeap.Any())
            {
                var item = maxHeap.Dequeue();
                Console.WriteLine("Dequeue " + item);
                Console.WriteLine("MaxHeap = " + maxHeap);
            }

            Console.WriteLine();
            Console.WriteLine("Press [Return] to continue");
            Console.ReadLine();
        }
    }
}
