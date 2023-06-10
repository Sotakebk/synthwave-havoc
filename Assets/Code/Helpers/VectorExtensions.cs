using UnityEngine;

namespace TopDownShooter.Helpers
{
    public static class VectorExtensions
    {
        public static Vector2 GetXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 ClampToUnitSphere(this Vector3 v)
        {
            if (v.sqrMagnitude > 1f)
                return v.normalized;
            return v;
        }

        public static Vector3 ToVector3XZ(this Vector2 v)
        {
            return new Vector3(v.x, 0, v.y);
        }
    }
}