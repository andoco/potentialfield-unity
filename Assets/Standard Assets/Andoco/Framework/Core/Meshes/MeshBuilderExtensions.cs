namespace Andoco.Unity.Framework.Core.Meshes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Andoco.Core;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;

    public delegate Color MeshBuilderAssignColorsDelegate(int vertexIndex, Vector3 vertex);
    
    public static class MeshBuilderExtensions
    {
        private static IStandardLog Log = LogManager.GetCurrentClassLogger();

        public static int AddVertex(this IMeshBuilder meshBuilder, Vector3 v)
        {
            meshBuilder.Vertices.Add(v);

            return meshBuilder.Vertices.Count - 1;
        }

        public static int AddColor(this IMeshBuilder meshBuilder, Color c)
        {
            meshBuilder.Colors.Add(c);

            return meshBuilder.Colors.Count - 1;
        }

        public static int AddNormal(this IMeshBuilder meshBuilder, Vector3 n)
        {
            meshBuilder.Normals.Add(n);

            return meshBuilder.Normals.Count - 1;
        }

        public static Mesh BuildMesh(this IMeshBuilder meshBuilder)
        {
            return meshBuilder.BuildMesh(true, true);
        }

        public static GameObject CreateGameObject(this IMeshBuilder meshBuilder, Material material, bool meshCollider = false)
        {
            return meshBuilder.CreateGameObject(null, material, meshCollider);
        }

        public static GameObject CreateGameObject(
            this IMeshBuilder meshBuilder, 
            string name, 
            Material material, 
            bool meshCollider = false,
            bool calculateNormals = true,
            bool calculateBounds = true)
        {
            var go = new GameObject();
            if (!string.IsNullOrEmpty(name))
                go.name = name;
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshFilter.mesh = meshBuilder.BuildMesh(calculateNormals, calculateBounds);
            meshRenderer.material = material;

            if (meshCollider)
                go.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh;

            return go;
        }

        /// <summary>
        /// Builds a single quad in the XZ plane, facing up the Y axis.
        /// </summary>
        /// <param name="meshBuilder">The mesh builder currently being added to.</param>
        /// <param name="offset">A position offset for the quad.</param>
        /// <param name="width">The width of the quad.</param>
        /// <param name="length">The length of the quad.</param>
        public static void BuildQuad(this IMeshBuilder meshBuilder, Vector3 offset, float width, float length)
        {
            meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(Vector3.up);
            
            meshBuilder.Vertices.Add(new Vector3(0.0f, 0.0f, length) + offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
            meshBuilder.Normals.Add(Vector3.up);
            
            meshBuilder.Vertices.Add(new Vector3(width, 0.0f, length) + offset);
            meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
            meshBuilder.Normals.Add(Vector3.up);
            
            meshBuilder.Vertices.Add(new Vector3(width, 0.0f, 0.0f) + offset);
            meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
            meshBuilder.Normals.Add(Vector3.up);
            
            //we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
            int baseIndex = meshBuilder.Vertices.Count - 4;
            
            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
        }
    
        /// <summary>
        /// Builds a single quad based on a position offset and width and length vectors.
        /// </summary>
        /// <param name="meshBuilder">The mesh builder currently being added to.</param>
        /// <param name="offset">A position offset for the quad.</param>
        /// <param name="widthDir">The width vector of the quad.</param>
        /// <param name="lengthDir">The length vector of the quad.</param>
        public static void BuildQuad(this IMeshBuilder meshBuilder, Vector3 offset, Vector3 widthDir, Vector3 lengthDir)
        {
            Vector3 normal = Vector3.Cross(lengthDir, widthDir).normalized;
            
            meshBuilder.Vertices.Add(offset);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(normal);
            
            meshBuilder.Vertices.Add(offset + lengthDir);
            meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
            meshBuilder.Normals.Add(normal);
            
            meshBuilder.Vertices.Add(offset + lengthDir + widthDir);
            meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
            meshBuilder.Normals.Add(normal);
            
            meshBuilder.Vertices.Add(offset + widthDir);
            meshBuilder.UVs.Add(new Vector2(1.0f, 0.0f));
            meshBuilder.Normals.Add(normal);
            
            //we don't know how many verts the meshBuilder is up to, but we only care about the four we just added:
            int baseIndex = meshBuilder.Vertices.Count - 4;
            
            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
            meshBuilder.AddTriangle(baseIndex, baseIndex + 2, baseIndex + 3);
        }
    
        public static void BuildCube(this IMeshBuilder meshBuilder, float width, float height, float length)
        {
            BuildCube(meshBuilder, width, height, length, Vector3.one * 0.5f);
        }
    
        public static void BuildCube(this IMeshBuilder meshBuilder, float width, float height, float length, Vector3 anchor)
        {
            //calculate directional vectors for all 3 dimensions of the cube:
            Vector3 upDir = Vector3.up * height;
            Vector3 rightDir = Vector3.right * width;
            Vector3 forwardDir = Vector3.forward * length;
            
            //calculate the positions of two corners opposite each other on the cube:
            
            //positions that will place the pivot at the corner of the cube:
            Vector3 nearCorner = Vector3.zero;
            Vector3 farCorner = upDir + rightDir + forwardDir;
    
            //shift the pivot by the anchor
            Vector3 pivotOffset = Vector3.Scale((rightDir + forwardDir + upDir), anchor);
            farCorner -= pivotOffset;
            nearCorner -= pivotOffset;
                    
            //build the 3 quads that originate from nearCorner:
            BuildQuad(meshBuilder, nearCorner, forwardDir, rightDir);
            BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
            BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);
            
            //build the 3 quads that originate from farCorner:
            BuildQuad(meshBuilder, farCorner, -rightDir, -forwardDir);
            BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
            BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
        }
    
        /// <summary>
        /// Builds a single triangle.
        /// </summary>
        /// <param name="meshBuilder">The mesh builder currently being added to.</param>
        /// <param name="corner0">The vertex position at index 0 of the triangle.</param>
        /// <param name="corner1">The vertex position at index 1 of the triangle.</param>
        /// <param name="corner2">The vertex position at index 2 of the triangle.</param>
        public static void BuildTriangle(this IMeshBuilder meshBuilder, Vector3 corner0, Vector3 corner1, Vector3 corner2)
        {
            Vector3 normal = Vector3.Cross((corner1 - corner0), (corner2 - corner0)).normalized;
            
            meshBuilder.Vertices.Add(corner0);
            meshBuilder.UVs.Add(new Vector2(0.0f, 0.0f));
            meshBuilder.Normals.Add(normal);
            
            meshBuilder.Vertices.Add(corner1);
            meshBuilder.UVs.Add(new Vector2(0.0f, 1.0f));
            meshBuilder.Normals.Add(normal);
            
            meshBuilder.Vertices.Add(corner2);
            meshBuilder.UVs.Add(new Vector2(1.0f, 1.0f));
            meshBuilder.Normals.Add(normal);
            
            int baseIndex = meshBuilder.Vertices.Count - 3;
            
            meshBuilder.AddTriangle(baseIndex, baseIndex + 1, baseIndex + 2);
        }
    
        public static void BuildFacade(this IMeshBuilder meshBuilder, float width, float height, float length)
        {
            BuildFacade(meshBuilder, width, height, length, Vector3.one * 0.5f);
        }
    
        public static void BuildFacade(this IMeshBuilder meshBuilder, float width, float height, float length, Vector3 anchor)
        {
            //build the walls:
            
            //calculate directional vectors for the walls:
            Vector3 upDir = Vector3.up * height;
            Vector3 rightDir = Vector3.right * width;
            Vector3 forwardDir = Vector3.forward * length;
            
            Vector3 farCorner = upDir + rightDir + forwardDir;
            Vector3 nearCorner = Vector3.zero;
            
            //shift the pivot by the anchor.
            Vector3 pivotOffset = Vector3.Scale((rightDir + forwardDir + upDir), anchor);
            farCorner -= pivotOffset;
            nearCorner -= pivotOffset;
            
            //build the quads for the walls:
            BuildQuad(meshBuilder, nearCorner, rightDir, upDir);
            BuildQuad(meshBuilder, nearCorner, upDir, forwardDir);
            
            BuildQuad(meshBuilder, farCorner, -upDir, -rightDir);
            BuildQuad(meshBuilder, farCorner, -forwardDir, -upDir);
        }
    
        public static void BuildRoof(this IMeshBuilder meshBuilder, float width, float length, float roofHeight, float roofOverhangFront, float roofOverhangSide, float roofBias)
        {
            BuildRoof(meshBuilder, width, length, roofHeight, roofOverhangFront, roofOverhangSide, roofBias, Vector3.one * 0.5f);
        }
    
        public static void BuildRoof(this IMeshBuilder meshBuilder, float width, float length, float roofHeight, float roofOverhangFront, float roofOverhangSide, float roofBias, Vector3 anchor)
        {
            //calculate directional vectors for the walls:
            Vector3 upDir = Vector3.up * roofHeight;
            Vector3 rightDir = Vector3.right * width;
            Vector3 forwardDir = Vector3.forward * length;
    
            Vector3 farCorner = rightDir + forwardDir;
            Vector3 nearCorner = Vector3.zero;
            
            //shift the pivot by the anchor.
            Vector3 pivotOffset = Vector3.Scale(rightDir + forwardDir + upDir, anchor);
            farCorner -= pivotOffset;
            nearCorner -= pivotOffset;
    
            //build the roof:
            
            //calculate the position of the roof peak at the near end of the house:
            Vector3 roofPeak = Vector3.up * roofHeight + rightDir * 0.5f - pivotOffset;
            
            //calculate the positions at the tops of the walls at the same end of the house:
            Vector3 wallTopLeft = - pivotOffset;
            Vector3 wallTopRight = rightDir - pivotOffset;
            
            //build triangles at the tops of the walls:
            BuildTriangle(meshBuilder, wallTopLeft, roofPeak, wallTopRight);
            BuildTriangle(meshBuilder, wallTopLeft + forwardDir, wallTopRight + forwardDir, roofPeak + forwardDir);
            
            //calculate the directions from the roof peak to the sides of the house:
            Vector3 dirFromPeakLeft = wallTopLeft - roofPeak;
            Vector3 dirFromPeakRight = wallTopRight - roofPeak;
            
            //extend the directions by a length of m_RoofOverhangSide:
            dirFromPeakLeft += dirFromPeakLeft.normalized * roofOverhangSide;
            dirFromPeakRight += dirFromPeakRight.normalized * roofOverhangSide;
            
            //offset the roofpeak position to put it at the beginning of the front overhang:
            roofPeak -= Vector3.forward * roofOverhangFront;
            
            //extend the forward directional vecter ot make it long enough for and overhang at either end:
            forwardDir += Vector3.forward * roofOverhangFront * 2.0f;
            
            //shift the roof slightly upward to stop it intersecting the top of the walls:
            roofPeak += Vector3.up * roofBias;
            
            //build the quads for the roof:
            BuildQuad(meshBuilder, roofPeak, forwardDir, dirFromPeakLeft);
            BuildQuad(meshBuilder, roofPeak, dirFromPeakRight, forwardDir);
            
            BuildQuad(meshBuilder, roofPeak, dirFromPeakLeft, forwardDir);
            BuildQuad(meshBuilder, roofPeak, forwardDir, dirFromPeakRight);
        }

        public static void BuildCylinder(this IMeshBuilder meshBuilder, int numSegments, float radius, float height)
        {
            meshBuilder.BuildCylinder(numSegments, 1, radius, height);
        }

        public static void BuildCylinder(this IMeshBuilder meshBuilder, int numSegments, int numLayers, float radius, float height)
        {
            var angleDelta = Mathf.PI * 2f / numSegments;
            var layerHeight = height / numLayers;

            var bottom = new Vector3(0f, 0f, 0f);
            var top = new Vector3(0f, height, 0f);

            meshBuilder.Vertices.Add(bottom);
            meshBuilder.Vertices.Add(top);

            for (int j = 0; j < numLayers; j++)
            {
                // The vertical position of the bottom of the current layer.
                var h = j * layerHeight;

                for (int i = 0; i <= numSegments; i++)
                {
                    var a = angleDelta * -i;
                    
                    var p1 = new Vector3(Mathf.Cos(a) * radius, h, Mathf.Sin(a) * radius);
                    var p2 = p1 + Vector3.up * layerHeight;
                    
                    meshBuilder.Vertices.Add(p1);
                    meshBuilder.Vertices.Add(p2);

                    var numVerts = meshBuilder.Vertices.Count;

                    if (i > 0)
                    {
                        // side
                        meshBuilder.AddTriangle(numVerts - 2, numVerts - 3, numVerts - 4);
                        meshBuilder.AddTriangle(numVerts - 2, numVerts - 1, numVerts - 3);

                        // bottom
                        if (j == 0)
                            meshBuilder.AddTriangle(numVerts - 4, 0, numVerts - 2);

                        // top
                        if (j == numLayers - 1)
                            meshBuilder.AddTriangle(numVerts - 1, 1, numVerts - 3);
                    }
                }
            }
        }

        /// <summary>
        /// Adds vertices for a circle.
        /// </summary>
        /// <remarks>
        /// No triangles are added by this method.
        /// </remarks>
        /// <param name="meshBuilder">Mesh builder.</param>
        /// <param name="numSegments">Number segments.</param>
        /// <param name="radius">Radius.</param>
        public static void AddCircle(this IMeshBuilder meshBuilder, int numSegments, float radius)
        {
            var startIndex = meshBuilder.Vertices.Count;

            // Rotate anticlockwise so edges go from left to right.
            var angleDelta = -Mathf.PI * 2f / numSegments;

            for (int i=0; i < numSegments; i++)
            {
                var a = angleDelta * -i;
                var p = new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);

                meshBuilder.Vertices.Add(p);
                meshBuilder.Normals.Add(p.normalized);
            }
        }

        public static void AddPolygon(this IMeshBuilder meshBuilder, Vector2[] poly)
        {
            Triangulator triangulator = new Triangulator(poly);

            int[] tris = triangulator.Triangulate();

            for(int i=0;i<poly.Length;i++)            
            {
                var v = new Vector3(poly[i].x, 0f, poly[i].y);
                meshBuilder.AddVertex(v);
                meshBuilder.UVs.Add(new Vector2(v.x, v.z));
            }

            meshBuilder.SetTriangles(tris);
        }
            
        public static void ExtrudeToCenter(this IMeshBuilder meshBuilder, IMeshSelection meshSelection)
        {
            var p = Vector3.zero;

            for (int i=0; i < meshSelection.Count; i++)
            {
                p += meshBuilder.Vertices[meshSelection[i]];
            }
            p = p / meshSelection.Count;

            meshBuilder.Vertices.Add(p);
            meshBuilder.UVs.Add(Vector2.zero);
//            meshBuilder.Colors.Add(new Color(1f, 1f, 1f, 1f));

            for (int i=0; i < meshSelection.Count - 1; i++)
            {
                var i1 = meshSelection[i];
                var i2 = meshBuilder.Vertices.Count - 1;
                var i3 = meshSelection[i + 1];
                meshBuilder.AddTriangle(i1, i2, i3);
            }

            // Add the final face.
            meshBuilder.AddTriangle(meshBuilder.Vertices.Count - 2, meshBuilder.Vertices.Count - 1, meshSelection[0]);
        }

        public static void BuildQuad(this IMeshBuilder meshBuilder, int bottomIndex, int topIndex)
        {
            var bl = bottomIndex;
            var tl = topIndex;
            var tr = topIndex + 1;
            var br = bottomIndex + 1;

            Log.Trace("Building quad: verts=[{0},{1},{2},{3}], indices=[{4},{5},{6},{7}]",
                meshBuilder.Vertices[bl],
                meshBuilder.Vertices[tl],
                meshBuilder.Vertices[tr],
                meshBuilder.Vertices[br],
                bl,
                tl,
                tr,
                br);

            meshBuilder.AddTriangle(bl, tl, br);
            meshBuilder.AddTriangle(br, tl, tr);
        }

        public static IMeshSelection ExtrudeEdge(
            this IMeshBuilder meshBuilder,
            IMeshSelection edge,
            Func<int, Vector3, Vector3> extrudeTranslationFunc)
        {
            Log.Trace("Will extrude edge {0}. Total vertices in mesh = {1}", edge, meshBuilder.Vertices.Count);

            var lowerEdge = new MeshSelection(edge);
            var extrudedEdge = new MeshSelection();

            // Check if we're dealing with a closed selection loop.
            var isClosed = lowerEdge.IsClosed;
            if (isClosed)
                Log.Trace("The edge has the same start and end point {0}", lowerEdge[0]);

            // Extrude new vertices from the current source vertices.
            for (int i = 0; i < lowerEdge.Count; i++)
            {
                var extrudeIdx = lowerEdge[i];
                var p = meshBuilder.Vertices[extrudeIdx];

                Log.Trace("extrudeIdx = {0}", extrudeIdx);
                // TODO: Need to skip duplicate indexes when the start/end range is wrapped.

                var translation = extrudeTranslationFunc(i, p);
                var newVertexIdx = meshBuilder.AddVertex(p + translation);
                extrudedEdge.Add(newVertexIdx);

                // TODO: This currently assumes the normal won't be changed by the translation. Need fix
                // support rotation as well.
                meshBuilder.Normals.Add(meshBuilder.Normals[extrudeIdx]);
            }

            if (isClosed)
                extrudedEdge.Add(extrudedEdge[0]);

            Log.Trace("Building {0} quads", edge.Count);

            Assert.AreEqual(lowerEdge.Count, extrudedEdge.Count, "Number of extruded indices should match the number of lower indices");

            Log.Trace("lowerEdge = {0}", lowerEdge);
            Log.Trace("extrudedEdge = {0}", extrudedEdge);

            // Build quads between the start vertices and the extruded vertices, using the virtual
            // selection which includes the closing vertex index.
            for (int i = 0; i < lowerEdge.VirtualCount - 1; i++)
            {
                var bl = lowerEdge.GetAtVirtualIndex(i);
                var tl = extrudedEdge.GetAtVirtualIndex(i);
                var tr = extrudedEdge.GetAtVirtualIndex(i + 1);
                var br = lowerEdge.GetAtVirtualIndex(i + 1);

                Log.Trace("Building quad: verts=[{0},{1},{2},{3}], indices=[{4},{5},{6},{7}]",
                    meshBuilder.Vertices[bl],
                    meshBuilder.Vertices[tl],
                    meshBuilder.Vertices[tr],
                    meshBuilder.Vertices[br],
                    bl,
                    tl,
                    tr,
                    br);

                meshBuilder.AddTriangle(bl, tl, br);
                meshBuilder.AddTriangle(br, tl, tr);
            }

            return extrudedEdge;
        }

        public static void ExtrudeSection(
            this IMeshBuilder meshBuilder, 
            IMeshSelection currentSelection, 
            int sectionStart, 
            int sectionEnd, 
            Func<int, Vector3, Vector3> extrudeTranslationFunc)
        {
            var sectionSelection = currentSelection.SelectRange(sectionStart, sectionEnd);

            sectionSelection = meshBuilder.ExtrudeEdge(
                sectionSelection,
                extrudeTranslationFunc);

            var origStart = currentSelection.GetAtVirtualIndex(sectionStart);
            var origEnd = currentSelection.GetAtVirtualIndex(sectionEnd);

            for (int i = 0; i < sectionSelection.Count; i++)
            {
                var currentSelectionIndex = (sectionStart + i).Wrap(currentSelection.Count - 1);
                currentSelection[currentSelectionIndex] = sectionSelection[i];
            }
                
            // If the section selection covers the entire current selection, we don't need to
            // insert any indices to maintain the link back to the current selection.
            if (origStart == sectionEnd)
                return;
            
            if (sectionStart <= sectionEnd)
            {
                currentSelection.Insert(origEnd, sectionEnd + 1);
                currentSelection.Insert(origStart, sectionStart);
            }
            else
            {
                currentSelection.Insert(origStart, sectionStart);
                currentSelection.Insert(origEnd, sectionEnd + 1);
            }
        }

        public static void AddMissingColors(this IMeshBuilder meshBuilder, Color color)
        {
            var numMissing = meshBuilder.Vertices.Count - meshBuilder.Colors.Count;

            for (int i = 0; i < numMissing; i++)
            {
                meshBuilder.Colors.Add(color);
            }
        }

        public static void AssignColors(this IMeshBuilder meshBuilder, int startIndex, int endIndex, Color color)
        {
            for (int i = 0; i < meshBuilder.Vertices.Count; i++)
            {
                meshBuilder.Colors[i] = color;
            }
        }

        public static void AssignColors(this IMeshBuilder meshBuilder, MeshBuilderAssignColorsDelegate colorFunc)
        {
            meshBuilder.Colors.Clear();

            for (int i = 0; i < meshBuilder.Vertices.Count; i++)
            {
                meshBuilder.AddColor(colorFunc(i, meshBuilder.Vertices[i]));
            }
        }

        public static void AssignRandomColorToVertices(this IMeshBuilder meshBuilder)
        {
            meshBuilder.AddMissingColors(Color.white);

            for (int i = 0; i < meshBuilder.Colors.Count; i++)
            {
                meshBuilder.Colors[i] = ColorHelper.RandomRGB();
            }
        }

        public static void AssignRandomColorToTriangles(this IMeshBuilder meshBuilder)
        {
            meshBuilder.AddMissingColors(Color.white);

            var triangles = meshBuilder.Indices;

            Color c = ColorHelper.RandomRGB();

            for (int i = 0; i < triangles.Count; i++)
            {
                if (i % 3 == 0)
                    c = ColorHelper.RandomRGB();

                meshBuilder.Colors[triangles[i]] = c;
            }
        }

        public static void SplitVertices(this IMeshBuilder meshBuilder)
        {
            var triangles = meshBuilder.Indices.ToArray();

            meshBuilder.Normals.Clear();
            meshBuilder.UVs.Clear();
            meshBuilder.Colors.Clear();

            var vertices = new Vector3[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                vertices[i] = meshBuilder.Vertices[triangles[i]];
                triangles[i] = i;
            }

            meshBuilder.SetVertices(vertices);
            meshBuilder.SetTriangles(triangles);
        }
    }
}