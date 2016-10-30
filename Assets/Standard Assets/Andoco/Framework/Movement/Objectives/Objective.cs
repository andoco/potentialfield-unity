namespace Andoco.Unity.Framework.Movement.Objectives
{
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;

    public class Objective : IObjective
    {
        public Objective(Vector3 vector, object data = null)
        {
            this.Kind = ObjectiveKind.Vector;
            this.TargetVector = vector;
            this.Data = data;
        }

        public Objective(Transform transform, object data = null)
        {
            this.Kind = ObjectiveKind.Transform;
            this.TargetTransform = transform;
            this.Data = data;
        }

        public object Data { get; private set; }

        public ObjectiveKind Kind { get; private set; }

        public Vector3 TargetVector { get; set; }

        public Transform TargetTransform { get; private set; }

        public Vector3 TargetPosition
        {
            get
            {
                switch (this.Kind)
                {
                    case ObjectiveKind.Transform:
                        return this.TargetTransform.position;
                    case ObjectiveKind.Vector:
                        return this.TargetVector;
                    default:
                        throw new System.InvalidOperationException(string.Format("Unkown ObjectiveKind {0}", this.Kind));
                }
            }
        }

        public override string ToString()
        {
            var targetDescription = this.Kind == ObjectiveKind.Transform
                ? string.Format("TargetTransform={0}", this.TargetTransform)
                : string.Format("TargetVector={0}", this.TargetVector);
            
            return string.Format("[Objective: Kind={0}, {1}, TargetPosition={2}, Data={3}]", Kind, targetDescription, TargetPosition, Data);
        }

        public bool Validate()
        {
            if (this.Kind == ObjectiveKind.Transform)
            {
                return ObjectValidator.Validate(this.TargetTransform);
            }

            return true;
        }
    }
}
