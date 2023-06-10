using UnityEngine;
using UnityEngine.AI;

namespace TopDownShooter.Enemy.Traits.PathFollowing
{
    public class BasicPathing : BasePathing
    {
        public int AreaMask = NavMesh.AllAreas;

        protected virtual void Awake()
        {
            Path = new NavMeshPath();
        }

        protected virtual void Update()
        {
            if (DebugSettings.DrawEnemyPathLines)
            {
                DrawPathDebugLines();
            }
        }

        public override void CalculatePathTo(Vector3 target)
        {
            NavMesh.CalculatePath(transform.position, target, AreaMask, Path);
        }

        private void DrawPathDebugLines()
        {
            var corners = Path.corners;

            for (int i = 0; i < corners.Length - 1; i++)
            {
                Debug.DrawLine(corners[i], corners[i + 1], Color.red);
            }
        }
    }
}