using UnityEngine;

namespace TopDownShooter.Helpers
{
    public static class PathHelper
    {
        public static Vector3 GetConstraintVelocityToFollowPath(Vector3 position, Vector3 target, Vector3 velocity)
        {
            var difference = position - target;
            var direction = difference.normalized;

            var left = Vector3.Cross(direction, Vector3.up);
            var sideForce = left * Vector3.Cross(direction, velocity).y;

            return sideForce;
        }

        public static Vector3 GetNextPointToFollowWithSomeDistanceEnforced(Vector3 position, Vector3[] points, float delta)
        {
            var nearestPoint = position;

            if (points.Length == 0)
                return nearestPoint;

            var nextPoint = points[1];
            var vectorHere = (nextPoint - position);
            if (vectorHere.magnitude > delta || points.Length == 2)
                return nextPoint;

            var nextPointAfter = points[2];

            var vectorThere = (nextPointAfter - nextPoint);
            var lengthToAdd = Mathf.Max(0, delta - vectorHere.magnitude);
            return nextPoint + vectorThere.normalized * lengthToAdd;
        }
    }
}