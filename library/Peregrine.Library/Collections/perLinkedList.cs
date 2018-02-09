using System;
using System.Collections.Generic;
using System.Linq;

namespace Peregrine.Library.Collections
{
    public class perLinkedList<T> where T: class
    {
        private class perLinkedListNode : IPERCollectionItem<T>
        {
            private readonly perLinkedList<T> _list;
            public perLinkedListNode(T data, perLinkedList<T> list)
            {
                Data = data;
                _list = list;
            }

            public T Data { get; }

            public perLinkedListNode Next { get; set; }
            public perLinkedListNode Previous { get; set; }

            public bool IsMarkedForDeletion { get; internal set; }

            public void MarkForDeletion()
            {
                IsMarkedForDeletion = true;
                _list.MarkForDeletion(this);
            }
        }

        private readonly Stack<perLinkedListNode> _nodesToDelete = new Stack<perLinkedListNode>();

        private perLinkedListNode Head { get; set; }
        private perLinkedListNode Tail { get; set; }

        public bool IsEmpty => Head == null;

        public void AddNodeAtHead(T data)
        {
            var newNode = new perLinkedListNode(data, this)
            {
                Next = Head
            };
            Head = newNode;

            if (Tail == null)
                Tail = newNode;
        }

        public void AddNodeAtTail(T data)
        {
            var newNode = new perLinkedListNode(data, this);

            if (Tail == null)
            {
                Tail = newNode;
                Head = newNode;
                return;
            }

            var previousLast = Tail;
            previousLast.Next = newNode;
            newNode.Previous = previousLast;
            Tail = newNode;
        }

        private void MarkForDeletion(perLinkedListNode nodeToDelete)
        {
            var node = nodeToDelete;

            if (_iteratorCount > 0)
            {
                node.IsMarkedForDeletion = true;
                _nodesToDelete.Push(node);
            }
            else
                Delete(node);
        }

        private void Delete(perLinkedListNode nodeToDelete)
        {
            if (_iteratorCount > 0)
                throw new InvalidOperationException("You may not delete a node when iterating. Use MarkForDeletion instead.");

            var previousNode = nodeToDelete.Previous;
            var nextNode = nodeToDelete.Next;

            if (previousNode != null)
                previousNode.Next = nextNode;

            if (nextNode != null)
                nextNode.Previous = previousNode;

            if (nodeToDelete == Head)
                Head = nextNode;

            if (nodeToDelete == Tail)
                Tail = previousNode;

            nodeToDelete.Next = null;
            nodeToDelete.Previous = null;
        }

        private int _iteratorCount;

        public IEnumerator<IPERCollectionItem<T>> GetEnumerator()
        {
            _iteratorCount++;

            try
            {
                var node = Head;

                while (node != null)
                {
                    if (!node.IsMarkedForDeletion)
                        yield return node;

                    node = node.Next;
                }
            }
            finally
            {
                _iteratorCount--;

                // cleanup any nodes marked for deletion during this iteration of the list
                while (_iteratorCount == 0 && _nodesToDelete.Any())
                {
                    var nodeToRemove = _nodesToDelete.Pop();
                    Delete(nodeToRemove);
                }
            }
        }
    }
}