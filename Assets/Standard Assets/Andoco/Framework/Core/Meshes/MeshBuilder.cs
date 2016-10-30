namespace Andoco.Unity.Framework.Core.Meshes
{
	using System.Collections.Generic;
    using System.Collections.ObjectModel;
	using System.Linq;
    using Andoco.Core.Diagnostics.Logging;
	using UnityEngine;

	public class MeshBuilder : IMeshBuilder
	{
        private static readonly IStandardLog log = LogManager.GetCurrentClassLogger();

		//indices for the triangles:
		private List<int> indices = new List<int>();

		public IList<Vector3> Vertices { get; private set; }
		
		public IList<Vector3> Normals { get; private set; }
		
		public IList<Vector2> UVs { get; private set; }

		public IList<Color> Colors { get; private set; }

        public ReadOnlyCollection<int> Indices { get; private set; }

		public MeshBuilder()
		{
			this.Vertices = new List<Vector3>();
			this.Normals = new List<Vector3>();
			this.UVs = new List<Vector2>();
			this.Colors = new List<Color>();
            this.Indices = this.indices.AsReadOnly();
		}

		public void AddTriangle(int index0, int index1, int index2)
		{
			indices.Add(index0);
			indices.Add(index1);
			indices.Add(index2);
		}

        public void SetTriangles(IEnumerable<int> triangles)
        {
            this.indices.Clear();
            this.indices.AddRange(triangles);
        }

        public void SetVertices(IEnumerable<Vector3> vertices)
        {
            this.Vertices.Clear();
            ((List<Vector3>)this.Vertices).AddRange(vertices);
        }

        public Mesh BuildMesh(bool calculateNormals, bool calculateBounds)
		{
			log.Trace("Building mesh for {0} vertices, {1} colors, {2} triangles", this.Vertices.Count, this.Colors.Count, this.indices.Count);

			var mesh = new Mesh();

			mesh.vertices = this.Vertices.ToArray();
			mesh.triangles = this.indices.ToArray();

			if (this.Normals.Count == this.Vertices.Count)
				mesh.normals = this.Normals.ToArray();

			if (this.UVs.Count == this.Vertices.Count)
				mesh.uv = this.UVs.ToArray();

			if (this.Colors.Count == this.Vertices.Count)
				mesh.colors = this.Colors.ToArray();
			
            // Have the mesh recalculate its bounding box (required for proper rendering).
            if (calculateBounds)
                mesh.RecalculateBounds();
            
            if (calculateNormals)
                mesh.RecalculateNormals();

			return mesh;
		}

		public void Clear()
		{
			this.Vertices.Clear();
			this.Normals.Clear();
			this.UVs.Clear();
			this.Colors.Clear();
			this.indices.Clear();
		}
	}
}