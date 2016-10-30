namespace Andoco.Unity.Framework.Input.Selection
{
    using System.Collections;
    using Andoco.Core.Diagnostics.Logging;
    using UnityEngine;
    using UnityEngine.Events;
    using Zenject;

    /// <summary>
    /// Controllable provides methods to select and control a gameobject.
    /// </summary>
    public class Selectable : MonoBehaviour
    {
        private IStandardLog log;

        [Inject]
        private SelectedSignal selectedSignal;

        [Inject]
        private DeselectedSignal deselectedSignal;

        [Inject]
        private DirectedSignal directedSignal;

        [Inject]
        private TriggeredSignal triggeredSignal;

        [Tooltip("Can the object be directed to handle an input position")]
        public bool isDirectectable;

        [Tooltip("Can the object be triggered by an input event, e.g. a tap")]
        public bool isTriggerable;

        public bool selected;

        public Vector3 directedPosition;
        public Transform directedTarget;

        public UnityEvent selectedBeginEvent;
        public UnityEvent selectedEndEvent;

        void Awake()
        {
            this.log = LogManager.GetCurrentClassLogger(this.name);
        }

        void Recycled()
        {
            this.selected = false;
            this.directedPosition = Vector3.zero;
            this.directedTarget = null;
        }

        public void Select()
        {
            this.log.Trace("Selected");
            this.selected = true;
            this.selectedSignal.Dispatch(this.gameObject);
            this.selectedBeginEvent.Invoke();
        }

        public void Deselect()
        {
            this.log.Trace("Deselected");
            this.selected = false;
            this.deselectedSignal.Dispatch(this.gameObject);
            this.selectedEndEvent.Invoke();
        }

        public void Direct(Vector3 position, Transform target = null)
        {
            this.log.Trace("Directed to {0}", position);
            this.directedPosition = position;
            this.directedTarget = target;
            this.directedSignal.Dispatch(this.gameObject);
        }

        public void Trigger()
        {
            this.log.Trace("Triggered");
            this.triggeredSignal.Dispatch(this.gameObject);
        }
    }
}
