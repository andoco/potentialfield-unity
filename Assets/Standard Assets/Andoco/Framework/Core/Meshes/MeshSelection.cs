namespace Andoco.Unity.Framework.Core.Meshes
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Andoco.Core;

    public class MeshSelection : IMeshSelection
    {
        private readonly List<int> selectedIndices;

        public MeshSelection()
        {
            this.selectedIndices = new List<int>();
        }

        public MeshSelection(IMeshSelection source)
            : this()
        {
            for (int i = 0; i < source.Count; i++)
            {
                this.Add(source[i]);
            }

            this.IsClosed = source.IsClosed;
        }

        public int this[int i]
        {
            get
            {
                return this.selectedIndices[i];
            }
            set
            {
                this.selectedIndices[i] = value;
            }
        }

        public int Count { get { return this.selectedIndices.Count; } }

        public int VirtualCount
        {
            get
            {
                return this.IsClosed
                    ? this.Count + 1
                    : this.Count;
            }
        }

        public bool IsClosed { get; private set; }

        public void Add(int index)
        {
            if (this.IsClosed)
                throw new InvalidOperationException("Cannot add index {0} because the current selection is already closed".FormatWith(index));

            if (this.selectedIndices.Count > 1 && index == this.selectedIndices[0])
            {
                this.IsClosed = true;
            }
            else
            {
                this.selectedIndices.Add(index);
            }
        }

        public void Clear()
        {
            this.selectedIndices.Clear();
            this.IsClosed = false;
        }

        public void Close()
        {
            this.IsClosed = true;
        }

        public void Duplicate(int index)
        {
            this.selectedIndices.Insert(index, this.selectedIndices[index]);
        }

        public int GetAtVirtualIndex(int index)
        {
            if (this.IsClosed && index == this.selectedIndices.Count)
                return this.selectedIndices[0];

            return this.selectedIndices[index];
        }

        public void Insert(int vertexIndex, int index)
        {
            this.selectedIndices.Insert(index, vertexIndex);
        }

        public void Remove(int index)
        {
            this.selectedIndices.Remove(index);
        }

        public void Replace(int vertexIndex1, int vertexIndex2)
        {
            var index = this.selectedIndices.IndexOf(vertexIndex1);
            this.selectedIndices[index] = vertexIndex2;
        }

        public IMeshSelection SelectRange(int startIndex, int endIndex)
        {
            var newSelection = new MeshSelection();

            int remaining;
            if (endIndex >= startIndex)
            {
                remaining = endIndex - startIndex + 1;
            }
            else
            {
                remaining = (this.selectedIndices.Count - startIndex) + endIndex + 1;
            }

            var index = startIndex;
            for (int i = 0; i < remaining; i++)
            {
                UnityEngine.Assertions.Assert.IsTrue(index < this.selectedIndices.Count);

                newSelection.Add(this.selectedIndices[index]);
                index = (index + 1).Wrap(this.selectedIndices.Count - 1);
            }

            return newSelection;
        }

        public void Swap(int vertexIndex1, int vertexIndex2)
        {
            var index1 = this.selectedIndices.IndexOf(vertexIndex1);
            var index2 = this.selectedIndices.IndexOf(vertexIndex2);

            this.selectedIndices[index1] = vertexIndex2;
            this.selectedIndices[index2] = vertexIndex1;
        }

        public override string ToString()
        {
            var formattedIndices = string.Join(",", this.selectedIndices.Select(x => x.ToString()).ToArray());
            return string.Format("[MeshSelection: Count={0}, IsClosed={1}, Indices={2}]", this.Count, this.IsClosed, formattedIndices);
        }
    }
}