using UnityEngine;

namespace FutureGames.Lib.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 With(this Vector3 t,
            float? x = null,
            float? y = null,
            float? z = null)
        {
            return new Vector3(
                x ?? t.x,
                y ?? t.y,
                z ?? t.z);
        }
    }
}