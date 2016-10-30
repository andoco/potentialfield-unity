namespace Andoco.Unity.Framework.Core.Meshes
{
    using System;
    using System.Collections.ObjectModel;

    public interface IMeshSelection
    {
        int this[int i] { get; set; }

        int Count { get; }

        /// <summary>
        /// Gets the number of indexes in the selection, plus 1 if the selection is marked as closed.
        /// </summary>
        int VirtualCount { get; }

        /// <summary>
        /// Gets a value indicating whether the selection is marked as closed, meaning that the vertex index at position 0 should also
        /// be considered the final vertex index.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        bool IsClosed { get; }

        void Add(int index);

        void Clear();

        /// <summary>
        /// Marks the selection as closed, meaning that the first index will also be considered to be the final index.
        /// </summary>
        void Close();

        void Duplicate(int index);

        /// <summary>
        /// Gets the vertex index value at the specified index, or at the index zero if the specified index is equal to the number of real vertex indices.
        /// </summary>
        /// <returns>The vertex index at the virtual index position.</returns>
        /// <param name="index">Virtual index number.</param>
        int GetAtVirtualIndex(int index);

        void Insert(int vertexIndex, int index);

        void Remove(int index);

        void Replace(int vertexIndex1, int vertexIndex2);

        IMeshSelection SelectRange(int startIndex, int endIndex);

        void Swap(int index1, int index2);
    }
}