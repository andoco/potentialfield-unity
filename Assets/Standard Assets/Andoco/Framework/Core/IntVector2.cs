namespace Andoco.Unity.Framework.Core
{
	[System.Serializable]
    public struct IntVector2
    {
        public int x;
        public int y;

        public IntVector2(int nx, int ny)
        {
            x = nx;
            y = ny;
        }

        public static IntVector2 operator +(IntVector2 val1, IntVector2 val2)
        {
            return new IntVector2(val1.x + val2.x, val1.y + val2.y);
        }
        
        public static IntVector2 operator -(IntVector2 val1, IntVector2 val2)
        {
            return new IntVector2(val1.x - val2.x, val1.y - val2.y);
        }
        
        public static IntVector2 operator -(IntVector2 val)
        {
            return new IntVector2(-val.x, -val.y);
        }
        
        public static IntVector2 operator *(IntVector2 val, int scalar)
        {
            return new IntVector2(val.x * scalar, val.y * scalar);
        }
        
        public static IntVector2 operator /(IntVector2 val, int scalar)
        {
            return new IntVector2(val.x / scalar, val.y / scalar);
        }
        
        #region Comparison
        
        public static bool operator ==(IntVector2 val1, IntVector2 val2)
        {
            return val1.x == val2.x && val1.y == val2.y;
        }
        
        public static bool operator !=(IntVector2 val1, IntVector2 val2)
        {
            return val1.x != val2.x || val1.y != val2.y;
        }
        
        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            return (IntVector2)obj == this;
        }

        public override int GetHashCode()
        {
            var hash = 13;
            hash = (hash * 7) + this.x.GetHashCode();
            hash = (hash * 7) + this.y.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return string.Format("[IntVector2 x={0}, y={1}]", x, y);
        }
    }
}