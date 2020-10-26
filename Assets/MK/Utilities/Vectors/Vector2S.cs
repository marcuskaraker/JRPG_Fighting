using UnityEngine;

namespace MK
{
    /// <summary>
    /// A Vector2 that can be serialized.
    /// </summary>
    public struct Vector2S
    {
        public float x;
        public float y;

        public Vector3 Vector3 { get { return new Vector3(x, y, 0); } }
        public Vector2 Vector2 { get { return new Vector2(x, y); } }

        /// <summary>
        /// Returns a Vector3 where the y value is 0 and the z value is the y of the Vector2S.
        /// </summary>
        public Vector3 VectorPlane { get { return new Vector3(x, 0, y); } }

        public Vector2S(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector3(Vector2S v) { return new Vector3(v.x, v.y, 0); }
        public static implicit operator Vector2(Vector2S v) { return new Vector2(v.x, v.y); }
        public static implicit operator Vector2S(Vector3 v) { return new Vector2S(v.x, v.y); }
        public static implicit operator Vector2S(Vector2 v) { return new Vector2S(v.x, v.y); }

        public static Vector2S Lerp(Vector2S a, Vector2S b, float t)
        {
            return new Vector2S
            (
                Mathf.Lerp(a.x, b.x, t),
                Mathf.Lerp(a.y, b.y, t)
            );
        }     

        /// <summary>
        /// Shorthand for writing Vector2(1, 0).
        /// </summary>
        public static Vector2S right { get { return new Vector2S(1, 0); } }
        /// <summary>
        /// Shorthand for writing Vector2(-1, 0).
        /// </summary>
        public static Vector2S left { get { return new Vector2S(-1, 0); } }
        /// <summary>
        /// Shorthand for writing Vector2(0, -1).
        /// </summary>
        public static Vector2S down { get { return new Vector2S(0, -1); } }
        /// <summary>
        /// Shorthand for writing Vector2(0, 1).
        /// </summary>
        public static Vector2S up { get { return new Vector2S(0, 1); } }
        /// <summary>
        /// Shorthand for writing Vector2(1, 1).
        /// </summary>
        public static Vector2S one { get { return new Vector2S(1, 1); } }
        /// <summary>
        /// Shorthand for writing Vector2(0, 0).
        /// </summary>
        public static Vector2S zero { get { return new Vector2S(0, 0); } }

        public static Vector2S operator +(Vector2S a, Vector2S b) { return new Vector2S(a.x + b.x, a.y + b.y); }
        public static Vector2S operator -(Vector2S a) { return new Vector2S(-a.x, -a.y); }
        public static Vector2S operator -(Vector2S a, Vector2S b) { return new Vector2S(a.x - b.x, a.y - b.y); }
        public static Vector2S operator *(float d, Vector2S a) { return new Vector2S(a.x * d, a.y * d); }
        public static Vector2S operator *(Vector2S a, float d) { return new Vector2S(a.x * d, a.y * d); }
        public static Vector2S operator *(Vector2S a, Vector2S b) { return new Vector2S(a.x * b.x, a.y * b.y); }
        public static Vector2S operator /(Vector2S a, float d) { return new Vector2S(a.x / d, a.y / d); }
        public static Vector2S operator /(Vector2S a, Vector2S b) { return new Vector2S(a.x / b.x, a.y / b.y); }
        public static bool operator ==(Vector2S lhs, Vector2S rhs) { return Vector2.SqrMagnitude(lhs - rhs) < 9.99999944E-11f; }
        public static bool operator !=(Vector2S lhs, Vector2S rhs) { return Vector2.SqrMagnitude(lhs - rhs) > 9.99999944E-11f; }
    }
}