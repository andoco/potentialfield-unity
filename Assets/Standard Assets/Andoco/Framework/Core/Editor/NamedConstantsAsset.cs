namespace Andoco.Unity.Framework.Core.Editor
{
    using UnityEngine;
    using UnityEditor;

    public class YourClassAsset
    {
        [MenuItem("Assets/Create/NamedConstants")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<NamedConstants>();
        }
    }
}
