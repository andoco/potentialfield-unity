namespace Andoco.Unity.Framework.Core
{
    using Andoco.Core;
    using UnityEngine;

    public class UnityTime : ITime
    {
        public float CurrentTime
        {
            get
            {
                return Time.time;
            }
        }
    }
}
