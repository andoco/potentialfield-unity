namespace Andoco.Unity.Framework.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class TypeExtensions
    {
        /// <summary>
        /// Finds all objects in the scene that are assignable to the type <paramref name="contractType"/>.
        /// </summary>
        /// <returns>The assignable objects in scene.</returns>
        /// <param name="contractType">Contract type.</param>
        /// <param name="includeBehaviours">Searches for matching <see cref="MonoBehaviour"/> instances on GameObjects.</param>
        /// <param name="includeFields">Searches for public fields on <see cref="MonoBehaviour"/> instances.</param>
        public static IEnumerable<object> FindAssignableObjectsInScene(this Type contractType, bool includeBehaviours = true, bool includeFields = true)
        {
            var behaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

            for (int i=0; i < behaviours.Length; i++)
            {
                var behaviour = behaviours[i];
                var behaviourType = behaviour.GetType();
                
                if (includeBehaviours && contractType.IsAssignableFrom(behaviourType))
                {
                    yield return behaviour;
                }

                if (includeFields)
                {
                    var fields = behaviourType.GetFields();

                    for (int j=0; j < fields.Length; j++)
                    {
                        var field = fields[j];
                        
                        if (contractType.IsAssignableFrom(field.FieldType))
                        {
                            var instance = field.GetValue(behaviour);
                            yield return instance;
                        }
                    }
                }
            }
        }
    }
}
