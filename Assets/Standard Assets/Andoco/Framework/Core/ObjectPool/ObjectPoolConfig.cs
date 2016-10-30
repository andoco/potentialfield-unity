namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;

    [System.Serializable]
    public class ObjectPoolConfig
    {
        public bool PoolEnabled = true;

        public AutoPoolInfo[] autoPoolPrefabs;
    }
	
}