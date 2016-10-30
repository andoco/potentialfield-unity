namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

	public static class MonoBehaviourExtensions
	{
	    #region Logging

		public static void Log(this MonoBehaviour monoBehaviour, string msg, bool enabled = true)
		{
			if (enabled)
				Debug.Log(string.Format("{0} {1} {2}", Time.time, monoBehaviour, msg), monoBehaviour);
		}

	    public static void Warn(this MonoBehaviour monoBehaviour, string msg, bool enabled)
	    {
	        Debug.LogWarning(string.Format("{0} {1} {2}", Time.time, monoBehaviour, msg), monoBehaviour);
	    }

	    #endregion
	}
}