namespace Andoco.Unity.Framework.Core.Meshes
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Core.Graph;

    public class MeshGraphData
    {
        public Vector3 vertex;
        public List<Edge> edges;
    }

    public static class MeshExtensions
    {
        /// <summary>
        /// Gets all the edges of the triangles in the mesh.
        /// </summary>
        /// <returns>The edges of the mesh.</returns>
        /// <param name="mesh">The mesh to find the triangle edges for.</param>
        public static Edge[] GetEdges(this Mesh mesh)
        {
            HashSet<Edge> edges = new HashSet<Edge>();

            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                var v1 = mesh.vertices[mesh.triangles[i]];
                var v2 = mesh.vertices[mesh.triangles[i + 1]];
                var v3 = mesh.vertices[mesh.triangles[i + 2]];
                edges.Add(new Edge(v1, v2));
                edges.Add(new Edge(v1, v3));
                edges.Add(new Edge(v2, v3));
            }

            return edges.ToArray();
        }

        /// <summary>
        /// Builds a graph of the mesh with one node per vertex, and an undirected edge for each triangle edge.
        /// </summary>
        /// <returns>The graph.</returns>
        /// <param name="mesh">The mesh to build the graph for.</param>
        public static IGraph BuildGraph(this Mesh mesh)
        {
            var verts = mesh.vertices;
            var tris = mesh.triangles;
            var edges = mesh.GetEdges();

            var graph = new Graph();
            var vertexNodeMap = new Dictionary<Vector3, IGraphNode>();

            foreach (var v in verts)
            {
                var node = new GraphNode("");
                var data = new MeshGraphData { vertex = v };
                node.Data.Add(data);
                graph.AddNode(node);

                vertexNodeMap.Add(v, node);
            }

            foreach (var node in graph.Nodes)
            {
                var nodeData = node.Data.Get<MeshGraphData>();

                // Get the mesh edges that connect to the vertex of the current node.
                nodeData.edges = edges.Where(e => e.IsEndpoint(nodeData.vertex)).ToList();

                // Add graph edges to the connected mesh vertices.
                foreach (var edge in nodeData.edges)
                {
                    var otherVertex = edge.GetOppositeVertex(nodeData.vertex);
                    var otherNode = vertexNodeMap[otherVertex];

                    if (!node.IsConnectedTo(otherNode))
                    {
                        graph.AddUndirectedEdge(node, otherNode);
                    }
                }
            }

            return graph;
        }

        /// <summary>
        /// Builds a <see cref="SimpleGraph"/> of the mesh with one node per vertex and a directed arc for each connecting triangle edge.
        /// </summary>
        /// <remarks>
        /// The mesh must not have co-incident vertices to work properly. Imported meshes usually have duplicate vertices so that UVs and
        /// normals can be stored. Setting the normals mode to "calculate", with a smoothing angle of 180 degrees, will remove the duplicates.
        /// </remarks>
        /// <returns>The graph.</returns>
        /// <param name="mesh">The mesh to build the graph for.</param>
        public static SimpleGraph BuildSimpleGraph(this Mesh mesh)
        {
            var verts = mesh.vertices;
            var tris = mesh.triangles;

            var builder = new SimpleGraphBuilder();

            for (int i=0; i < verts.Length; i++)
            {
                builder.NewNode();
                var arcs = new HashSet<int>();

                for (int j=0; j < tris.Length; j+=3)
                {
                    var i0 = tris[j];
                    var i1 = tris[j+1];
                    var i2 = tris[j+2];

                    if (i0 == i)
                    {
                        arcs.Add(i1);
                        arcs.Add(i2);
                    }
                    else if (i1 == i)
                    {
                        arcs.Add(i0);
                        arcs.Add(i2);
                    }
                    else if (i2 == i)
                    {
                        arcs.Add(i0);
                        arcs.Add(i1);
                    }
                }

                builder.AddArcs(arcs.ToArray());
            }

            return builder.Build();
        }

        /// <summary>
        /// Splits the vertices and normals of each triangle in the mesh so that none are shared by any two triangles.
        /// </summary>
        /// <remarks>This achieves a flat-shaded lighting effect on the mesh.</remarks>
        /// <param name="mesh">Mesh.</param>
        public static void SplitVertices(this Mesh mesh)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            var newVerts = new Vector3[triangles.Length];
            var newNorms = new Vector3[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                newVerts[i] = vertices[triangles[i]];
                triangles[i] = i;
            }

            for (int i = 0; i < triangles.Length; i+=3)
            {
                Vector3 normal, point;
                Math3d.PlaneFrom3Points(out normal, out point, newVerts[triangles[i]], newVerts[triangles[i + 1]], newVerts[triangles[i + 2]]);

                newNorms[i] = normal;
                newNorms[i + 1] = normal;
                newNorms[i + 2] = normal;
            }

            mesh.vertices = newVerts;
            mesh.triangles = triangles;
            mesh.normals = newNorms;
        }

        public static Mesh Duplicate(this Mesh mesh)
        {
            var newMesh = new Mesh();
            newMesh.vertices = mesh.vertices;
            newMesh.triangles = mesh.triangles;
            newMesh.normals = mesh.normals;
            newMesh.uv = mesh.uv;
            newMesh.colors = mesh.colors;

            return newMesh;
        }

        /// <summary>
        /// Flips the normals so that the faces will appear in the opposite direction.
        /// </summary>
        /// <returns>A duplicate mesh with the flipped normals.</returns>
        /// <param name="sourceMesh">Source mesh to flip the normals for.</param>
        public static Mesh FlipNormals(this Mesh sourceMesh)
        {
            var mesh = sourceMesh.Duplicate();

            Vector3[] normals = mesh.normals;
            for (int i=0;i<normals.Length;i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m=0;m<mesh.subMeshCount;m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i=0;i<triangles.Length;i+=3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }

            return mesh;
        }

        /// <summary>
        /// Adds the reverse faces for a mesh, so that the mesh will appear double-sided.
        /// </summary>
        /// <returns>A duplicate mesh with reverse faces.</returns>
        /// <param name="sourceMesh">Source mesh to add reverse faces for.</param>
        public static Mesh AddReverseFaces(this Mesh sourceMesh)
        {
            // IMPORTANT: The order of the faces matters. Faces added later in the mesh
            // will look stronger than those added before. We want the flipped faces to
            // appear weaker, so we add them first.

            var combine = new CombineInstance[2];
            combine[0].mesh = sourceMesh.FlipNormals();
            combine[1].mesh = sourceMesh;

            var finalMesh = new Mesh();
            finalMesh.CombineMeshes(combine, true, false);

            return finalMesh;
        }

        public static Mesh Scale(this Mesh sourceMesh, float scale)
        {
            var verts = sourceMesh.vertices;
            var mesh = sourceMesh.Duplicate();

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] *= scale;
            }

            mesh.vertices = verts;
            mesh.RecalculateBounds();

            return mesh;
        }

        /// <summary>
        /// Creates a new GameObject to render the mesh.
        /// </summary>
        public static GameObject CreateGameObject(
            this Mesh mesh, 
            string name, 
            Material material, 
            bool meshCollider = false)
        {
            GameObject go = null;

            if (string.IsNullOrEmpty(name))
                go = new GameObject();
            else
                go = new GameObject(name);
            
            var meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material = material;

            if (meshCollider)
                go.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;

            return go;
        }
    }
}