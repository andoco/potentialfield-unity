using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class SimpleGraphFieldNodeRef : IFieldNodeRef
    {
        public SimpleGraphFieldNodeRef(Vector3 pos, int node)
        {
            this.Position = pos;
            this.Node = node;
        }

        public Vector3 Position { get; set; }

        public int Node { get; set; }

        public override string ToString()
        {
            return string.Format("[SimpleGraphFieldNodeRef: Position={0}, Node={1}]", Position, Node);
        }
    }
}
