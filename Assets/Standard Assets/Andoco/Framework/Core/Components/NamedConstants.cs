using System;
using System.Collections.Generic;
using Andoco.Core.Pooling;
using UnityEngine;

namespace Andoco.Unity.Framework.Core
{
    public class NamedConstants : ScriptableObject
    {
        public Value[] values;

        public IList<Value> GetGroup(string name)
        {
            var results = ListPool<Value>.Take();

            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (string.Equals(values[i].Group, name, StringComparison.OrdinalIgnoreCase))
                    {
                        results.Add(values[i]);
                    }
                }
            }

            return results;
        }

        public Value GetValue(string groupName, string valueName)
        {
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (
                        string.Equals(values[i].Group, groupName, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(values[i].Name, valueName, StringComparison.OrdinalIgnoreCase))
                    {
                        return values[i];
                    }
                }
            }

            return null;
        }

        [Serializable]
        public class Value
        {
            [SerializeField]
            private string name;

            [SerializeField]
            private string group;

            [SerializeField]
            private ConstType type;

            [SerializeField]
            private string stringValue;

            [SerializeField]
            private int intValue;

            [SerializeField]
            private float floatValue;

            public string Name { get { return name; } }
            public string Group { get { return group; } }
            public ConstType Type { get { return type; } }
            public string StringValue { get { return stringValue; } }
            public int IntValue { get { return intValue; } }
            public float FloatValue { get { return floatValue; } }
        }

        public enum ConstType
        {
            String,
            Int,
            Float
        }
    }
}
