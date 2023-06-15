using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Traits.MovementEffects
{
    public abstract class BaseMovementEffect : MonoBehaviour
    {
        public abstract Vector3 GetMovementEffect();
    }
}