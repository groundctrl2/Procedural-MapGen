using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A min-heap priority queue of tile indices, ordered by entropy
/// </summary>
public class MinPriorityQueue : IEnumerable<int>
{
    /// <summary>
    /// Internal node storing entropy and tile index
    /// </summary>
    private class Node : IComparable<Node>
    {
        public float Entropy { get; set; } // Will be updated in-place
        public int Index { get; }

        /// <summary>
        /// Represents an item in the priority queue with entropy and tile index
        /// </summary>
        public Node(float entropy, int index)
        {
            Entropy = entropy;
            Index = index;
        }

        /// <summary>
        /// Sets comparisons to be between entropy values
        /// </summary>
        public int CompareTo(Node other)
        {
            return Entropy.CompareTo(other.Entropy);
        }
    }

    private List<Node> pq; // The priority queue
    private int[] indexMap; // maps tile index to positions in pq list

    /// <summary>
    /// Initializes an empty min priority queue
    /// </summary>
    public MinPriorityQueue(int totalTileCount)
    {
        pq = new List<Node> { null }; // Index 0 unused
        indexMap = new int[totalTileCount];
        Array.Fill(indexMap, -1); // -1 means not in heap
    }

    /// <summary>
    /// Returns true if this priority queue is empty
    /// </summary>
    public bool IsEmpty() => pq.Count == 1;

    /// <summary>
    /// Returns the number of nodes on this priority queue
    /// </summary>
    public int Size() => pq.Count - 1;

    /// <summary>
    /// Returns smallest node's index on this priority queue
    /// </summary>
    public int Min()
    {
        if (IsEmpty()) throw new InvalidOperationException("Priority queue underflow");
        return pq[1].Index;
    }

    /// <summary>
    /// Adds a new node to this priority queue
    /// </summary>
    public void Insert(float entropy, int index)
    {
        if (indexMap[index] != -1) return; // Already in heap

        Node node = new Node(entropy, index);
        pq.Add(node);
        int i = Size();
        indexMap[index] = i;
        Swim(i);
    }

    /// <summary>
    /// Removes and returns smallest (entropy) node on priority queue
    /// </summary>
    public int DelMin()
    {
        if (IsEmpty()) throw new InvalidOperationException("Priority queue underflow");

        int min = pq[1].Index;
        int last = Size();
        Swap(1, last);
        pq.RemoveAt(last);
        indexMap[min] = -1;

        if (!IsEmpty())
            Sink(1);

        return min;
    }

    /// <summary>
    /// Updates the entropy of an existing node, then reorders heap (pq) accordingly
    /// </summary>
    public void Update(float newEntropy, int index)
    {
        if (index < 0 || index >= indexMap.Length || indexMap[index] == -1) return; // Not in heap

        int i = indexMap[index];
        float oldEntropy = pq[i].Entropy;
        pq[i].Entropy = newEntropy;

        if (newEntropy < oldEntropy)
            Swim(i);
        else
            Sink(i);
    }

    /// <summary>
    /// Restores pq (heap) order by swimming up the node
    /// </summary>
    private void Swim(int k)
    {
        while (k > 1 && pq[k / 2].CompareTo(pq[k]) > 0)
        {
            Swap(k, k / 2);
            k /= 2;
        }
    }

    /// <summary>
    /// Restores the pq (heap) order by sinking down the node
    /// </summary>
    private void Sink(int k)
    {
        int n = Size();
        while (2 * k <= n)
        {
            int j = 2 * k;
            if (j < n && pq[j].CompareTo(pq[j + 1]) > 0) j++;
            if (pq[k].CompareTo(pq[j]) <= 0) break;
            Swap(k, j);
            k = j;
        }
    }

    /// <summary>
    /// Swaps two nodes in the heap
    /// </summary>
    private void Swap(int i, int j)
    {
        (pq[i], pq[j]) = (pq[j], pq[i]); // Tuple swap instead of temp
        indexMap[pq[i].Index] = i;
        indexMap[pq[j].Index] = j;
    }

    /// <summary>
    /// Returns an enumerator that yields positions in ascending entropy order
    /// </summary>
    public IEnumerator<int> GetEnumerator()
    {
        MinPriorityQueue copy = new MinPriorityQueue(indexMap.Length);
        for (int i = 1; i < pq.Count; i++)
            copy.Insert(pq[i].Entropy, pq[i].Index);

        while (!copy.IsEmpty())
            yield return copy.DelMin();
    }

    /// <summary>
    /// Required for IEnumerable
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Returns the entropy of the node with the given index, or throws if not in heap
    /// </summary>
    public float GetEntropy(int index)
    {
        if (index < 0 || index >= indexMap.Length || indexMap[index] == -1)
            throw new ArgumentException($"Index {index} is not in the priority queue.");

        return pq[indexMap[index]].Entropy;
    }
}
