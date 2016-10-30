namespace Andoco.Unity.Framework.Core
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Unity trace listener. DEBUG flag must be set for these to work.
    /// </summary>
    public class UnityTraceListener: TraceListener
    {
        private readonly bool throwOnFail;

        public UnityTraceListener(bool throwOnFail = false)
            : base("UnityTraceListener")
        {
            this.throwOnFail = throwOnFail;
        }

        public override void Fail(string message)
        {
            var text = String.Format("Application failure: {0}", message);
            UnityEngine.Debug.LogError(text);

            if (this.throwOnFail)
            {
                throw new ApplicationException(text);
            }
        }
        
        public override void Write(string message)
        {
            UnityEngine.Debug.Log(message);
        }
        
        public override void WriteLine(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
