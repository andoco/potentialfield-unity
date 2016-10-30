using UnityEditor;
using UnityEngine;
using Andoco.Unity.Framework.Core.Editor;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldDataAsset
    {
        [MenuItem("Assets/Create/PotentialFieldData")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<PotentialFieldData>();
        }
    }
}
