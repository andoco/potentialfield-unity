namespace Andoco.Unity.Framework.Core.DiagnosticsIndicators
{
    using System.Collections.Generic;
    using System.Text;
    using Andoco.Core.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;
    
    public class ScreenDiagnosticIndicators : MonoBehaviour
    {
        [Inject]
        private DiagnosticIndicatorSignal signal;

        private Canvas canvas;
        private readonly IDictionary<GameObject, IDictionary<string, string>> goLines = new Dictionary<GameObject, IDictionary<string, string>>();
        private readonly IDictionary<GameObject, Text> goTextMapping = new Dictionary<GameObject, Text>();
        private readonly List<GameObject> destroyedObjects = new List<GameObject>();
        private readonly StringBuilder sb = new StringBuilder();

        public GameObject textPrefab;

        [Tooltip("Offsets the target world position used as the reference point for the indicator position")]
        public Vector3 indicatorOffset = Vector3.up * 3f;
    
        [Inject]
        public void OnPostInject()
        {
            this.canvas = this.transform.Find("Canvas").GetComponent<Canvas>();

            this.signal.AddListener(this.OnDiagnosticSignal);
        }

        void LateUpdate()
        {
            foreach (var item in this.goTextMapping)
            {
                var go = item.Key;
                var text = item.Value;

                if (go == null)
                {
                    this.destroyedObjects.Add(go);
                    continue;
                }

                if (!go.activeInHierarchy || !go.GetComponentInChildren<Renderer>().isVisible)
                {
                    text.gameObject.SetActive(false);
                    continue;
                }

                if (!text.gameObject.activeSelf)
                {
                    text.gameObject.SetActive(true);
                }

                var canvasRect = this.canvas.GetComponent<RectTransform>();
                var viewportPos = Camera.main.WorldToViewportPoint(go.transform.position + this.indicatorOffset);

//                text.rectTransform.anchorMin = pos;
//                text.rectTransform.anchorMax = pos;

                var pos =new Vector2(
                    ((viewportPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                    ((viewportPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

                text.rectTransform.anchoredPosition = pos;
            }

            foreach (var go in this.destroyedObjects)
            {
                this.goLines.Remove(go);

                GameObject.Destroy(this.goTextMapping[go].gameObject);
                this.goTextMapping.Remove(go);
            }

            this.destroyedObjects.Clear();
        }

        private void OnDiagnosticSignal(DiagnosticIndicatorSignal.Data data)
        {
            switch (data.Action)
            {
                case DiagnosticIndicatorSignal.ActionKind.Set:
                    this.AddLine(data.Target, data.Key, data.Text);
                    break;
                case DiagnosticIndicatorSignal.ActionKind.Unset:
                    this.RemoveLine(data.Target, data.Key);
                    break;
                default:
                    throw new System.InvalidOperationException(string.Format("Unexpected {0} value {1}", typeof(DiagnosticIndicatorSignal.ActionKind), data.Action));
            }

            this.UpdateCanvasText(data.Target);
        }

        private void AddLine(GameObject target, string key, string text)
        {
            IDictionary<string, string> lines;

            if (!this.goLines.TryGetValue(target, out lines))
            {
                lines = new Dictionary<string, string>();
                this.goLines.Add(target, lines);
            }

            lines[key] = text;
        }

        private void RemoveLine(GameObject target, string key)
        {
            IDictionary<string, string> lines;
            
            if (this.goLines.TryGetValue(target, out lines))
            {
                lines.Remove(key);
            }
        }

        private string FormatLines(IDictionary<string, string> lines)
        {
            foreach (var item in lines)
            {
                this.sb.AppendLine(string.Format("{0} : {1}", item.Key, item.Value));
            }

            var formatted = this.sb.ToString();

            this.sb.Length = 0;

            return formatted;
        }

        private void UpdateCanvasText(GameObject target)
        {
            IDictionary<string, string> lines;
            if (!this.goLines.TryGetValue(target, out lines))
            {
                return;
            }

            Text text;
            if (!this.goTextMapping.TryGetValue(target, out text))
            {
                var textGo = (GameObject)GameObject.Instantiate(this.textPrefab, Vector3.zero, Quaternion.identity);
                textGo.transform.SetParent(this.canvas.transform, false);

                text = textGo.GetComponent<Text>();

                this.goTextMapping.Add(target, text);
            }

            var formattedText = this.FormatLines(lines);
            text.text = formattedText;
        }
    }
}
