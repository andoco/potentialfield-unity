namespace Andoco.Unity.Framework.Core
{
    [System.Serializable]
    public class InterpolatedInt
    {
        public int start;
        public int end;
        public Interpolate.EaseType easeType;

        public int GetValue(float t)
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

            return (int)value;
        }
    }
}
