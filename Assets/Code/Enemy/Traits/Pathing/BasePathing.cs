using UnityEngine;
using UnityEngine.AI;

namespace TopDownShooter.Enemy.Traits.PathFollowing
{
    public abstract class BasePathing : MonoBehaviour
    {
        public NavMeshPath Path { get; protected set; }

        public abstract void CalculatePathTo(Vector3 target);
    }
}