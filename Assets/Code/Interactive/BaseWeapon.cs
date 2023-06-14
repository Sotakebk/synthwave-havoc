using UnityEngine;

namespace TopDownShooter.Interactive {
    public class BaseWeapon : MonoBehaviour
    {
        [SerializeField] protected float _secondsPerBullet = 0.1f;
        [SerializeField] protected bool _overrideBulletProperties = true;
        [SerializeField] protected float _damage = 1f;
        [SerializeField] protected float _startVelocity = 10f;
        [SerializeField] protected float _minStartVelocity = 8f;
        [SerializeField] protected float _knockbackForce = 0.3f;
        [SerializeField] protected float _enemyKnockbackForce = 0.3f;
        [SerializeReference] protected Transform _outputSource;
        [SerializeReference] protected Rigidbody _sourceRigidbody;
        [SerializeReference] protected GameObject _bulletPrefab;

        protected float _currentTimeout = 0;

        public Transform OutputSource => _outputSource;
        
        protected virtual void FixedUpdate()
        {
            if (_currentTimeout > 0)
                _currentTimeout -= Time.fixedDeltaTime;
        }

        public virtual bool TryShoot()
        {
            if (_currentTimeout > 0)
                return false;

            _currentTimeout += _secondsPerBullet;

            var bullet = Instantiate(_bulletPrefab, _outputSource.position, _outputSource.rotation);

            if (_overrideBulletProperties)
            {
                var bulletComponent = bullet.GetComponent<Bullet>();
                if(bulletComponent != null)
                {
                    bulletComponent.Damage = _damage;
                    bulletComponent.KnockbackForce = _enemyKnockbackForce;
                }
            }

            var shootingDirection = _outputSource.transform.forward;
            var bulletVelocity = _startVelocity * shootingDirection;
            if(_sourceRigidbody != null)
            {
                bulletVelocity += _sourceRigidbody.velocity;
            }

            if(bulletVelocity.magnitude < _minStartVelocity)
            {
                bulletVelocity += (_minStartVelocity - bulletVelocity.magnitude) * shootingDirection;
            }

            var knockbackDirection = -bulletVelocity.normalized;
            _sourceRigidbody?.AddForce(_knockbackForce * knockbackDirection, ForceMode.Impulse);
            bullet.GetComponent<Rigidbody>().velocity = bulletVelocity;
            return true;
        }
    }
}