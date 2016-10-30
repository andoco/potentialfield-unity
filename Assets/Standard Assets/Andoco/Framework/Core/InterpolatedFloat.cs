namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    [System.Serializable]
    public class InterpolatedFloat
    {
        public float start;
        public float end;
        public Interpolate.EaseType easeType;

        public float GetValue(float t)
        {
            var easeFunc = Interpolate.Ease(this.easeType);

            var a = this.start;
            var b = this.end;

            if (a > b)
            {
                t = 1f - t;

                var tmp = a;
                a = b;
                b = tmp;
            }

            var start = a;
            var distance = b - a;

            // start, distance, elapsed, duration.
            var value = easeFunc(start, distance, t, 1f);

            return value;
        }

        public override string ToString()
        {
            return string.Format("[InterpolatedFloat start={0}, end={1}, easeType={2}]", this.start, this.end, this.easeType);
        }
    }
}
