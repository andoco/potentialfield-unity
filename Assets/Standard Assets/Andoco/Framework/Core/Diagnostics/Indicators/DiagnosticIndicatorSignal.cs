namespace Andoco.Unity.Framework.Core.DiagnosticsIndicators
{
    using UnityEngine;
    using Andoco.Core.Signals;
    
    public class DiagnosticIndicatorSignal : Signal<DiagnosticIndicatorSignal.Data>
    {
        public void DispatchSet(GameObject target, string key, string text)
        {
            var data = new DiagnosticIndicatorSignal.Data
            {
                Action = ActionKind.Set,
                Target = target,
                Key = key,
                Text = text
            };

            this.Dispatch(data);
        }

        public void DispatchSet(string key, string text)
        {
            this.DispatchSet(null, key, text);
        }

        public void DispatchUnset(GameObject target, string key)
        {
            var data = new DiagnosticIndicatorSignal.Data
            {
                Action = ActionKind.Unset,
                Target = target,
                Key = key
            };
            
            this.Dispatch(data);
        }

        public void DispatchUnset(string key)
        {
            this.DispatchUnset(null, key);
        }

        public struct Data
        {
            public ActionKind Action { get; set; }

            public GameObject Target { get; set; }

            public string Key { get; set; }
    
            public string Text { get; set; }
        }

        public enum ActionKind
        {
            None,
            Set,
            Unset
        }
    }
}
