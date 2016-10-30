using UnityEngine;

namespace Andoco.Unity.Framework.Core
{
    public class DefaultObjectValidator : IObjectValidator
    {
        public bool Validate(Object obj)
        {
            if (obj == null)
                return false;

            GameObject go;

            if (obj is GameObject)
                go = (GameObject)obj;
            else if (obj is Component)
                go = ((Component)obj).gameObject;
            else
                throw new System.ArgumentOutOfRangeException("obj", obj, "A GameObject must be accessible from the supplied instance in order to validate the object");

            return go.activeInHierarchy;
        }
    }
}
