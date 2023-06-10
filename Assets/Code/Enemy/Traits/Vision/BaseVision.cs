using UnityEngine;

namespace TopDownShooter.Enemy.Traits.Vision
{
    public abstract class BaseVision : MonoBehaviour
    {
        public Vector3 LastSeenPlayerPosition { get; protected set; }
        public Vector3 LastSeenPlayerVelocity { get; protected set; }

        public abstract void UpdateVision();

        public abstract float SecondsSincePlayerVisible { get; protected set; }
        public abstract float SecondsSincePlayerNotVisible { get; protected set; }
        public abstract bool IsPlayerVisible { get; protected set; }
        public abstract bool WasPlayerVisible { get; protected set; }
    }
}