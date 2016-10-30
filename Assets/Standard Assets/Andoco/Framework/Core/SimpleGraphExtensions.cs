using System.Collections;
using System.Collections.Generic;
using Andoco.Core;
using Andoco.Core.Pooling;
using System;

namespace Andoco.Unity.Framework.Core
{
    public static class SimpleGraphExtensions
    {
        public static IList<int> GetNodesBreadthFirst(this SimpleGraph graph, int startNode, Func<int, bool> filter, Func<IList<int>, int, bool> terminator)
        {
            var results = ListPool<int>.Take();
            var open = ListPool<int>.Take();

            open.Add(startNode);

            var currentDepth = 0;
            var depthSize = 1;
            var nextDepthSize = 0;

            while (open.Count > 0)
            {
                var node = open.Dequeue();

                if (filter(node) && !results.Contains(node))
                    results.Add(node);

                depthSize--;

                var numArcs = graph.NumArcsForNode(node);

                for (int i = 0; i < numArcs; i++)
                {
                    var neighbour = graph.GetNodeArc(node, i);
                    if (!open.Contains(neighbour))
                    {
                        open.Add(neighbour);
                        nextDepthSize++;
                    }
                }

                // If we've visited all nodes at current depth, get ready for the next depth level.
                if (depthSize == 0)
                {
                    currentDepth++;
                    depthSize = nextDepthSize;
                    nextDepthSize = 0;
                }

                // Stop traversal if the termination condition is met.
                if (terminator(results, currentDepth))
                    break;
            }

            return results;
        }
    }
}