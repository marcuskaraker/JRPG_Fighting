using UnityEngine;

namespace FutureGames.Lib.Extensions
{
    public static class RectExtensions
    {
        public static Rect With(this Rect t,
            float? x = null,
                float? y = null,
                float? w = null,
                float? h = null)
        {
            return new Rect(
                    x ?? t.x,
                    y ?? t.y,
                    w ?? t.width,
                    h ?? t.height);
        }
    }
}