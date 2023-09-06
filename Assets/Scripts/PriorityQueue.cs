using System.Collections.Generic;
using System;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> queue;

    public PriorityQueue()
    {
        queue = new List<T>();
    }

    public void Enqueue(T item)
    {
        queue.Add(item);
        BubbleUp();
    }

    public bool TryDequeue(out T item)
    {
        if (queue.Count == 0)
        {
            item = default(T);
            return false;
        }

        item = queue[0];
        MoveLastItemToTheTop();
        SinkDown();
        return true;
    }

    public T Dequeue()
    {
        var item = queue[0];
        MoveLastItemToTheTop();
        SinkDown();
        return item;
    }

    private void BubbleUp() // Implementation of the Min Heap Bubble Up operation
    {
        var childIndex = queue.Count - 1;
        while (childIndex > 0)
        {
            var parentIndex = (childIndex - 1) / 2;
            if (queue[childIndex].CompareTo(queue[parentIndex]) >= 0)
                break;
            Swap(childIndex, parentIndex);
            childIndex = parentIndex;
        }
    }

    private void MoveLastItemToTheTop()
    {
        var lastIndex = queue.Count - 1;
        queue[0] = queue[lastIndex];
        queue.RemoveAt(lastIndex);
    }

    private void SinkDown() // Implementation of the Min Heap Sink Down operation
    {
        var lastIndex = queue.Count - 1;
        var parentIndex = 0;

        while (true)
        {
            var firstChildIndex = parentIndex * 2 + 1;
            if (firstChildIndex > lastIndex)
            {
                break;
            }
            var secondChildIndex = firstChildIndex + 1;
            if (secondChildIndex <= lastIndex && queue[secondChildIndex].CompareTo(queue[firstChildIndex]) < 0)
            {
                firstChildIndex = secondChildIndex;
            }
            if (queue[parentIndex].CompareTo(queue[firstChildIndex]) < 0)
            {
                break;
            }
            Swap(parentIndex, firstChildIndex);
            parentIndex = firstChildIndex;
        }
    }

    private void Swap(int index1, int index2)
    {
        var tmp = queue[index1];
        queue[index1] = queue[index2];
        queue[index2] = tmp;
    }
}