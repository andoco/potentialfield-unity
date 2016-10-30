using UnityEditor;

namespace Andoco.Unity.Framework.PotentialField
{
	[CustomEditor(typeof(PotentialSteerNavigator))]
	public class PotentialSteerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
		}
	}
}
