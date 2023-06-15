using UnityEngine;

namespace TopDownShooter.Interactive.Enemy.Traits
{
    public class HurtOnTouch : MonoBehaviour
    {
        [SerializeField] private float _damageOnTouch;
        [SerializeField] private float _secondsTimeout = 0.5f;
        [SerializeField] private LayerMask _collisionMask;

        private float _currentTimeout;

        private void FixedUpdate()
        {
            if (_currentTimeout > 0)
                _currentTimeout -= Time.fixedDeltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & _collisionMask.value) == 0)
                return;

            var component = collision.gameObject.GetComponentInParent<BaseLivingEntity>();
            if (component == null)
                return;

            OnCollisionWithPlayer(component);
        }
        private void OnCollisionStay(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & _collisionMask.value) == 0)
                return;

            var component = collision.gameObject.GetComponentInParent<BaseLivingEntity>();
            if (component == null)
                return;

            OnCollisionWithPlayer(component);
        }

        private void OnCollisionWithPlayer(BaseLivingEntity player)
        {
            if (_currentTimeout > 0)
                return;

            _currentTimeout += _secondsTimeout;

            player.ModifyHealth(-_damageOnTouch);
        }

    }
}