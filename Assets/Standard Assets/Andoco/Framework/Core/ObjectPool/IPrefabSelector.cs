namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;

    public interface IPrefabSelector
    {
        GameObject Select(string name);
    }
}