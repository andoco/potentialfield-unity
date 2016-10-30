using UnityEditor;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    [CustomEditor(typeof(PotentialFieldNavigator))]
    public class PotentialFieldNavigatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
