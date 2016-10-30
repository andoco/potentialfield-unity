namespace Andoco.Unity.Framework.Core.DiagnosticsIndicators
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
        
    public class DiagnosticIndicators : MonoBehaviour
    {
        [Inject]
        private DiagnosticIndicatorSignal
            signal;
    
        private readonly Dictionary<string, string> lines = new Dictionary<string, string>();
        private Text text;
    
        public GameObject canvasPrefab;
    
        [Inject]
        public void OnPostInject()
        {
            var canvas = (GameObject)GameObject.Instantiate(this.canvasPrefab);
            canvas.transform.SetParent(this.transform, false);
            this.text = canvas.GetComponentInChildren<Text>();
    
            this.signal.AddListener(this.OnDiagnosticSignal);
        }

        private readonly StringBuilder sb = new StringBuilder();
    
        private void OnDiagnosticSignal(DiagnosticIndicatorSignal.Data data)
        {
            switch (data.Action)
            {
                case DiagnosticIndicatorSignal.ActionKind.Set:
                    this.lines[data.Key] = data.Text;
                    break;
                case DiagnosticIndicatorSignal.ActionKind.Unset:
                    this.lines.Remove(data.Key);
                    break;
                default:
                    throw new System.InvalidOperationException(string.Format("Unexpected {0} value {1}", typeof(DiagnosticIndicatorSignal.ActionKind), data.Action));
            }

            this.sb.Length = 0;
    
            foreach (var item in this.lines)
            {
                this.sb.AppendLine(string.Format("{0} : {1}", item.Key, item.Value));
            }

//            this.sb.Remove(this.sb.Length - System.Environment.NewLine.Length, System.Environment.NewLine.Length);
    
            this.text.text = this.sb.ToString();
        }
    }
}
