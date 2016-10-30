namespace Andoco.Unity.Framework.Core
{
    using System.Collections;

    public class SimpleGraph
    {
        //  The arcs array is simply all the arcs in the graph listed in order of the nodes they extend from
        //  (so all the arcs from node 0 are listed first, in order, then all the arcs from node 1, etc). The nodes
        //  array is then just a list of offsets into the arcs, each element containing the starting point in the
        //  arcs array for that particular node. The number of arcs from a node can be calculated by subtracting
        //  its offset from the offset of the following node. This necessitates an additional dummy node at the
        //  end of the array that simply contains the total number of arcs.
        public readonly int[] nodes;
        public readonly int[] arcs;
    
        public SimpleGraph(int numNodes, int numArcs)
        {
            nodes = new int[numNodes + 1];
            nodes[nodes.Length - 1] = numArcs;
            arcs = new int[numArcs];
        }
    
        /// <summary>
        /// Gets the number nodes in the graph.
        /// </summary>
        public int NumNodes { get { return nodes.Length - 1; } }
    
        /// <summary>
        /// Gets the total number arcs in the graph.
        /// </summary>
        public int NumArcs { get { return arcs.Length; } }
    
        /// <summary>
        /// Returns the number of arcs that extend from a given node.
        /// </summary>
        /// <returns>The number of arcs.</returns>
        /// <param name="nodeNum">The node that the arcs extend from.</param>
        public int NumArcsForNode(int nodeNum)
        {
            return nodes[nodeNum + 1] - nodes[nodeNum];
        }
    
        /// <summary>
        /// Gets the node at the end of an arc.
        /// </summary>
        /// <returns>The index of the connected node.</returns>
        /// <param name="nodeNum">The index of the node the arc originates from.</param>
        /// <param name="arcIndex">The index of the arc originating from the node.</param>
        public int GetNodeArc(int nodeNum, int arcIndex)
        {
            return arcs[nodes[nodeNum] + arcIndex];
        }
    
        /// <summary>
        /// Gets all a node's arcs in order as an array.
        /// </summary>
        /// <returns>The node arcs.</returns>
        public int[] GetNodeArcs(int nodeNum)
        {
            int[] result = new int[NumArcsForNode(nodeNum)];
            System.Array.Copy(arcs, nodes[nodeNum], result, 0, NumArcsForNode(nodeNum));
            return result;
        }

        /// <summary>
        /// Checks if an arc exists from <paramref name="nodeNum1"/> to <paramref name="nodeNum2"/>.
        /// </summary>
        public bool HasNodeArc(int nodeNum1, int nodeNum2)
        {
            var numArcs = nodes[nodeNum1 + 1] - nodes[nodeNum1];

            for (int i = 0; i < numArcs; i++)
            {
                if (arcs[nodes[nodeNum1] + i] == nodeNum2)
                {
                    return true;
                }
            }

            return false;
        }
    
        /// <summary>
        /// Sets the target node for an arc.
        /// </summary>
        /// <param name="nodeNum">The index of the node the arc originates from.</param>
        /// <param name="arcIndex">The index of the arc.</param>
        /// <param name="newTargetNode">The index of the node we want the arc to connect to.</param>
        public void SetNodeArc(int nodeNum, int arcIndex, int newTargetNode)
        {
            arcs[nodes[nodeNum] + arcIndex] = newTargetNode;
        }
    }
}