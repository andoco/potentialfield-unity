using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Andoco.Unity.Framework.PotentialField
{
    [CustomEditor(typeof(PotentialFieldSystem))]
    public class PotentialFieldSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
