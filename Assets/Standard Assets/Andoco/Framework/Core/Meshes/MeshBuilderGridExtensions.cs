namespace Andoco.Unity.Framework.Core.Meshes
{
    using System;
    using UnityEngine;

	public struct QuadCorners
	{
		public Vector3 c0, c1, c2, c3;
	}

    public static class MeshBuilderGridExtensions
    {
        public static void BuildSimpleGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount)
        {
            BuildSimpleGrid(meshBuilder, cellWidth, cellLength, segmentCount, (x, y) => 0f);
        }

        public static void BuildSimpleGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount, Func<int, int, float> heightFunc)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                float z = cellLength * i;
                
                for (int j = 0; j < segmentCount; j++)
                {
                    float x = cellWidth * j;
                    
                    Vector3 offset = new Vector3(x, heightFunc(j, i), z);

                    meshBuilder.BuildQuad(offset, cellWidth, cellLength);
                }
            }
        }
        
        public static void BuildGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount)
        {
            //Loop through the rows:
            for (int i = 0; i <= segmentCount; i++)
            {
                //incremented values for the Z position and V coordinate:
                float z = cellLength * i;
                float v = (1.0f / segmentCount) * i;
                
                //Loop through the collumns:
                for (int j = 0; j <= segmentCount; j++)
                {
                    //incremented values for the X position and U coordinate:
                    float x = cellWidth * j;
                    float u = (1.0f / segmentCount) * j;
                    
                    //The position offset for this quad:
                    Vector3 offset = new Vector3(x, 0f, z);

                    //build quads that share vertices:
                    Vector2 uv = new Vector2(u, v);
                    bool buildTriangles = i > 0 && j > 0;
                    
                    BuildQuadForGrid(meshBuilder, offset, uv, buildTriangles, segmentCount + 1);
                }
            }
        }

        /// <summary>
        /// Builds a single quad as part of a mesh grid.
        /// </summary>
        /// <param name="meshBuilder">The mesh builder currently being added to.</param>
        /// <param name="position">A position offset for the quad. Specifically the position of the corner vertex of the quad.</param>
        /// <param name="uv">The UV coordinates of the quad's corner vertex.</param>
        /// <param name="buildTriangles">Should triangles be built for this quad? This value should be false if this is the first quad in any row or collumn.</param>
        /// <param name="vertsPerRow">The number of vertices per row in this grid.</param>
        public static void BuildQuadForGrid(this IMeshBuilder meshBuilder, Vector3 position, Vector2 uv, bool buildTriangles, int vertsPerRow)
        {
            meshBuilder.Vertices.Add(position);
            meshBuilder.UVs.Add(uv);

            if (buildTriangles)
            {
                int baseIndex = meshBuilder.Vertices.Count - 1;

                int index0 = baseIndex;
                int index1 = baseIndex - 1;
                int index2 = baseIndex - vertsPerRow;
                int index3 = baseIndex - vertsPerRow - 1;

                meshBuilder.AddTriangle(index0, index2, index1);
                meshBuilder.AddTriangle(index2, index3, index1);
            }
        }

        /// <summary>
        /// Builds a grid using quads with shared vertices, and the origin in the bottom-left corner.
        /// </summary>
        public static void BuildSmoothGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int segmentCount, Func<int, int, float> heightFunc, Func<int, int, QuadCorners, Color> colorFunc = null)
        {
            meshBuilder.BuildSmoothGrid(cellWidth, cellLength, segmentCount, segmentCount, heightFunc, colorFunc);
        }

        /// <summary>
        /// Builds a grid using quads with shared vertices, and the origin in the bottom-left corner.
        /// </summary>
		public static void BuildSmoothGrid(this IMeshBuilder meshBuilder, float cellWidth, float cellLength, int columns, int rows, Func<int, int, float> heightFunc, Func<int, int, QuadCorners, Color> colorFunc = null)
        {
			if (colorFunc == null)
				colorFunc = (i, j, quad) => Color.white;

            int baseIndex = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    var x = cellWidth * j;
                    var z = cellLength * i;

					var h0 = heightFunc(j, i);
					var h1 = heightFunc(j, i + 1);
					var h2 = heightFunc(j + 1, i + 1);
					var h3 = heightFunc(j + 1, i);

					QuadCorners quad;
					quad.c0 = new Vector3(x, h0, z);
					quad.c1 = new Vector3(x, h1, z + cellLength);
					quad.c2 = new Vector3(x + cellWidth, h2, z + cellLength);
					quad.c3 = new Vector3(x + cellWidth, h3, z);

					var c = colorFunc(j, i, quad);

                    meshBuilder.Vertices.Add(quad.c0);
					meshBuilder.Colors.Add(c);
					meshBuilder.Vertices.Add(quad.c1);
					meshBuilder.Colors.Add(c);
					meshBuilder.Vertices.Add(quad.c2);
					meshBuilder.Colors.Add(c);
					meshBuilder.Vertices.Add(quad.c3);
					meshBuilder.Colors.Add(c);
                    
                    meshBuilder.Normals.Add(Vector3.up);
                    meshBuilder.Normals.Add(Vector3.up);
                    meshBuilder.Normals.Add(Vector3.up);
                    meshBuilder.Normals.Add(Vector3.up);

                    meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
                    meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);

                    baseIndex += 4;
                }
            }
        }
    }
}