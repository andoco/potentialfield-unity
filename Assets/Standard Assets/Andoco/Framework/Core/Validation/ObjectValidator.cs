using UnityEngine;

namespace Andoco.Unity.Framework.Core
{
    public static class ObjectValidator
    {
        private static IObjectValidator validator;

        static ObjectValidator()
        {
            SetValidator(new DefaultObjectValidator());
        }

        public static void SetValidator(IObjectValidator newValidator)
        {
            validator = newValidator;
        }

        /// <summary>
        /// Validates the supplied object to check if it can be safely used. References to invalid objects should be discarded.
        /// </summary>
        public static bool Validate(Object obj)
        {
            return validator.Validate(obj);
        }
    }
}
