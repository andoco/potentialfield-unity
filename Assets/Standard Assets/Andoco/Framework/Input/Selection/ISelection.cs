namespace Andoco.Unity.Framework.Input.Selection
{
    using UnityEngine;

    public interface ISelection
    {
        bool HasSelection { get; }

        GameObject Selected { get; }

        void Select(GameObject go);

        void Deselect();

        void Direct(Vector3 position, Transform target = null);

        void Trigger();

        void TriggerFlag(string flag);

        bool ValidateDirectedTarget();
    }
}
