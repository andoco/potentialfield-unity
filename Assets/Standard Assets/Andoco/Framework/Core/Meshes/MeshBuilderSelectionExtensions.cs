namespace Andoco.Unity.Framework.Core.Meshes
{
    using System;

    public static class MeshBuilderSelectionExtensions
    {
        public static IMeshSelection SelectAll(this IMeshBuilder meshBuilder)
        {
            var selection = new MeshSelection();

            for (int i = 0; i < meshBuilder.Vertices.Count; i++)
            {
                selection.Add(i);
            }

            return selection;
        }
    }
}
