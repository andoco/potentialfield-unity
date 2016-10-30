using UnityEngine;
using System.Collections;

namespace Andoco.Unity.Framework.Core.Meshes
{

    public class FlatShading : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public Color vertexColor;
        public bool reverseFaces;
        public bool splitVertices;

        void Start()
        {
            if (this.meshFilter == null)
                this.meshFilter = this.GetComponent<MeshFilter>();

            var mesh = this.meshFilter.mesh;

            if (this.reverseFaces)
            {
                mesh = mesh.AddReverseFaces();
            }

            if (this.splitVertices)
            {
                mesh.SplitVertices();
            }

            this.meshFilter.mesh = mesh;
        }

        void Update()
        {
            var mesh = this.meshFilter.mesh;
            var colors = new Color[mesh.vertexCount];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = this.vertexColor;
            }

            mesh.colors = colors;
        }
    }
}
