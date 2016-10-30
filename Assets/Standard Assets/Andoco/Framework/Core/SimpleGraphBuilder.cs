namespace Andoco.Unity.Framework.Core
{
    using System.Collections;
    using System.Collections.Generic;
    
    // This simplifies the construction of a SimpleGraph by allowing the nodes and arcs to be added
    // incrementally without prior knowledge of how many of each there will be.
    // The basic plan is to start a new node, then add as many arcs as necessary, then start another
    // new node, etc. When the temporary graph is complete, the Build function will construct
    // the equivalent SimpleGraph.
    
    public class SimpleGraphBuilder
    {
        private List<List<int>> nodes;
        private int arcTotal;

        public SimpleGraphBuilder()
        {
            nodes = new List<List<int>>();
        }
    
        /// <summary>
        /// Starts a new node.
        /// </summary>
        public void NewNode()
        {
            nodes.Add(new List<int>());
        }
    
        /// <summary>
        /// Add arcs to the current node.
        /// </summary>
        /// <param name="targetNodes">Target nodes.</param>
        public void AddArcs(params int[] targetNodes)
        {
            var currNode = nodes[nodes.Count - 1];

            for (int i = 0; i < targetNodes.Length; i++)
            {
                currNode.Add(targetNodes[i]);
            }
    
            arcTotal += targetNodes.Length;
        }
    
        /// <summary>
        /// Build a SimpleGraph from the temporary graph contained by the builder.
        /// </summary>
        public SimpleGraph Build()
        {
            var result = new SimpleGraph(nodes.Count, arcTotal);
            int currArc = 0;
    
            for (int i = 0; i < nodes.Count; i++)
            {
                result.nodes[i] = currArc;
                var currNode = nodes[i];
                currNode.CopyTo(result.arcs, currArc);
                currArc += currNode.Count;
            }
    
            return result;
        }
    }
}