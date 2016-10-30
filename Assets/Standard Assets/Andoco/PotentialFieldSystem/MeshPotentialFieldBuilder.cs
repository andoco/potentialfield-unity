using System.Linq;
using Andoco.Unity.Framework.Core.Meshes;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.PotentialField
{
    /// <summary>
    /// Configures a potential field using the vertices of a mesh as the node positions, and the triangle edges as node arcs.
    /// </summary>
    public class MeshPotentialFieldBuilder : MonoBehaviour
    {
        [Inject]
        private IPotentialFieldSystem potentialField;

        public void Build(Mesh mesh, Vector3 offset)
        {
            var graph = mesh.BuildSimpleGraph();
            var positions = mesh.vertices.Select(v => v + offset).ToArray();
            potentialField.SetGraph(graph, positions);
        }
    }
}
