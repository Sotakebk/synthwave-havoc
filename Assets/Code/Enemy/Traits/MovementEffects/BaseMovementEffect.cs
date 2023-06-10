using UnityEngine;

namespace TopDownShooter.Enemy.Traits.MovementEffects
{
    public abstract class BaseMovementEffect : MonoBehaviour
    {
        public abstract Vector3 GetMovementEffect();
    }
}