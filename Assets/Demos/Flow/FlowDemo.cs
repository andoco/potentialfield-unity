using System.Collections.Generic;
using Andoco.Core;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.PotentialField;
using UnityEngine;
using Zenject;

public class FlowDemo : MonoBehaviour
{
    [Inject]
    IPotentialFieldSystem potentialFieldSys;

    public GridSpace gridSpace;
    public PotentialLayerMask potentialLayers;

    [Inject]
    void OnPostInject()
    {
        BuildGraph();

        AddPropagatingSource(new IntVector2(0, 0), 1f);

        AddBlockingSource(new IntVector2(2, 2));
        AddBlockingSource(new IntVector2(3, 2));
        AddBlockingSource(new IntVector2(4, 2));
        AddBlockingSource(new IntVector2(5, 2));
        AddBlockingSource(new IntVector2(2, 3));
        AddBlockingSource(new IntVector2(2, 4));
        AddBlockingSource(new IntVector2(2, 5));
    }

    void BuildGraph()
    {
        var gb = new SimpleGraphBuilder();
        var positions = new List<Vector3>();

        for (int i = 0; i < gridSpace.NumCells; i++)
        {
            positions.Add(gridSpace.GetBounds(i).center);
            gb.NewNode();

            var neighbours = new List<int>();

            foreach (var dir in CompassDirection.PrincipalDirections)
            {
                int neighbour;
                if (gridSpace.TryGetNeighbour(i, dir, out neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }

            gb.AddArcs(neighbours.ToArray());
        }

        var graph = gb.Build();

        potentialFieldSys.SetGraph(graph, positions.ToArray());
    }

    void AddPropagatingSource(IntVector2 gridPos, float potential)
    {
        var source = potentialFieldSys.AddNodeSource(this.gameObject, "demo", potentialLayers);
        source.Node = potentialFieldSys[gridSpace.GetIndex(gridPos)];
        source.Potential = potential;
        source.Flow = PotentialFlowKind.Propagate;
    }

    void AddBlockingSource(IntVector2 gridPos)
    {
        var source = potentialFieldSys.AddNodeSource(this.gameObject, "demo", potentialLayers);
        source.Node = potentialFieldSys[gridSpace.GetIndex(gridPos)];
        source.Flow = PotentialFlowKind.Block;
    }
}
