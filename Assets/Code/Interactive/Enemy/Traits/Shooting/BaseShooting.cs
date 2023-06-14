using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Traits.Shooting
{
    public abstract class BaseShooting : MonoBehaviour
    {
        [SerializeReference] protected BaseWeapon _weapon;

        protected bool _enabled;

        public virtual void SetShootingEnable(bool enabled)
        {
            _enabled = enabled;
        }
    }
}