namespace Andoco.Unity.Framework.Input.Selection
{
    using System.Collections;
    using Andoco.BehaviorTree.Signals;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using Zenject;

    public class SelectionController : MonoBehaviour, ISelection
    {
        [Inject]
        private FlagSignal flagSignal;

        public GameObject selected;

        public bool HasSelection
        {
            get
            {
                return this.selected != null;
            }
        }

        public GameObject Selected
        {
            get
            {
                return this.selected;
            }
        }

        public void Select(GameObject go)
        {
            this.selected = go;

            var controllable = this.selected.GetComponent<Selectable>();

            if (controllable != null)
            {
                controllable.Select();
            }
        }

        public void Deselect()
        {
            var controllable = this.selected.GetComponent<Selectable>();

            this.selected = null;

            if (controllable != null)
            {
                controllable.Deselect();
            }
        }

        public void Direct(Vector3 position, Transform target = null)
        {
            var controllable = this.selected.GetComponent<Selectable>();

            if (controllable != null)
            {
                controllable.Direct(position, target);
            }
        }

        public void Trigger()
        {
            var controllable = this.selected.GetComponent<Selectable>();

            if (controllable != null)
            {
                controllable.Trigger();
            }
        }

        public bool ValidateDirectedTarget()
        {
            var controllable = this.selected.GetComponent<Selectable>();

            if (controllable != null)
            {
                return ObjectValidator.Validate(controllable.directedTarget);
            }

            return false;
        }

        public void TriggerFlag(string flag)
        {
            this.flagSignal.DispatchFlag(this.selected, flag);
        }
    }
}
