using UnityEngine;
using System.Collections;

namespace Andoco.Unity.Framework.Core.Meshes
{
    public class VertexColor : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public Color color;

        void Start()
        {
            if (this.meshFilter == null)
                this.meshFilter = this.GetComponent<MeshFilter>();
        }

        void Update()
        {
            var mesh = this.meshFilter.mesh;
            var colors = new Color[mesh.vertexCount];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = this.color;
            }

            mesh.colors = colors;
        }
    }

}
