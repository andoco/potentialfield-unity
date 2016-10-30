using UnityEngine;
using System.Collections;

namespace Andoco.Unity.Framework.Core.Meshes
{
    public class GridMeshBuilder : MonoBehaviour
    {
        public Vector2 cellSize;
        public Vector2 gridSize;
        public Material gridMaterial;
        public bool buildOnStart;
        public bool buildGameObject;
        public Vector3 gameObjectPositionOffset;
        public bool addMeshCollider;

        public Mesh BuiltMesh { get; private set; }

        public GameObject BuiltGameObject { get; private set; }

        void Start()
        {
            if (buildOnStart)
            {
                Build();
            }
        }

        public void Build()
        {
            var meshBuilder = new MeshBuilder();
    
            meshBuilder.BuildGrid(cellSize.x, cellSize.y, (int)gridSize.x);
            BuiltMesh = meshBuilder.BuildMesh();
    
            if (buildGameObject)
            {
                var go = BuiltMesh.CreateGameObject("Grid", gridMaterial, addMeshCollider);
                go.transform.position = transform.position + gameObjectPositionOffset;
                BuiltGameObject = go;
            }
        }
    }
}
