namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using System.Collections;
    using Zenject;

    public enum InputType
    {
        None,
        Pan,
        Swipe,
        Tap,
        LongPress
    }

    public class TargetInputSource : MonoBehaviour
    {
        [Inject]
        private InputSourceSignal inputSourceSignal;
    
        public InputType inputType;
        public Transform target;
        public SwipeDirection swipeDirection;

        void Start()
        {
            this.DispatchEnabled(true);
        }

        void Spawned()
        {
            this.DispatchEnabled(true);
        }

        void OnDestroy()
        {
            this.DispatchEnabled(false);
        }

        void Recycled()
        {
            this.DispatchEnabled(false);
        }

        private void DispatchEnabled(bool isEnabled)
        {
            this.inputSourceSignal.Dispatch(new InputSourceSignal.Data { Enabled = isEnabled, InputType = this.inputType, Target = this.target, Direction = this.swipeDirection });
        }
    }
}
