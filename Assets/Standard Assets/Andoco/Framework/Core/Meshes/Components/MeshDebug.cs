using UnityEngine;
using System.Collections;
using Andoco.Unity.Framework.Core.Meshes;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshDebug : MonoBehaviour
{
    private Vector3 vertexColorIndicatorScale;
    private GUIStyle vertexIndexLabelStyle;
    private Edge[] edges;

    public float vertexColorIndicatorSize = 0.1f;
    public bool vertexIndexLabels = true;
    public int vertexIndexLabelSize = 15;
    public Color vertexIndexLabelColor = Color.red;

    public bool vertexGizmos;
    public bool edgeGizmos;

    void Awake()
    {
        this.vertexColorIndicatorScale = Vector3.one * this.vertexColorIndicatorSize;
        this.vertexIndexLabelStyle = new GUIStyle();
    }

    void OnDrawGizmos()
    {
        var tx = this.transform;
        
        var mesh = this.GetComponent<MeshFilter>().sharedMesh;

        var verts = mesh.vertices;
        var norms = mesh.normals;
        var cols = mesh.colors;
        var triangles = mesh.triangles;

        if (this.vertexGizmos)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                var p = tx.position + verts[i];
                var n = norms[i];

                Color normCol;

                if (i == 0)
                    normCol = Color.green;
                else if (i == verts.Length - 1)
                    normCol = Color.red;
                else
                    normCol = Color.white;

                Gizmos.color = normCol;
                Gizmos.DrawWireSphere(p, 0.1f);
                Gizmos.DrawLine(p, p + n);

                Gizmos.color = cols == null || cols.Length == 0 ? Color.white : cols[i];
                Gizmos.DrawWireCube(p + n, this.vertexColorIndicatorScale);

                if (this.vertexIndexLabels)
                {
                    #if UNITY_EDITOR
                    this.vertexIndexLabelStyle.fontSize = this.vertexIndexLabelSize;
                    this.vertexIndexLabelStyle.normal.textColor = this.vertexIndexLabelColor;

                    Handles.Label(p + n, i.ToString(), this.vertexIndexLabelStyle);
                    #endif
                }
            }
        }

        if (this.edgeGizmos)
        {
            if (this.edges == null)
                this.edges = mesh.GetEdges();
            
            var origin = tx.position;
            Gizmos.color = Color.yellow;

            foreach (var e in edges)
            {
                var midpoint = Vector3.Lerp(e.v1, e.v2, 0.5f);
                Gizmos.DrawSphere(origin + midpoint, 0.1f);
            }
        }
    }
}
