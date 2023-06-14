using System.Linq;
using UnityEngine;

namespace TopDownShooter.Interactive
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        [SerializeField] protected float _damage;
        [SerializeField] protected float _knockbackForce;
        [SerializeField] protected LayerMask _hitEntityMask;
        [SerializeField] protected float _secondsToLive = 10f;

        public float Damage { get => _damage; set => _damage = value; }

        public float KnockbackForce { get => _knockbackForce; set => _knockbackForce = value; }

        protected float _timeLived = 0;

        protected virtual void Awake()
        {
            _timeLived = 0;
        }

        protected virtual void FixedUpdate()
        {
            _timeLived += Time.fixedDeltaTime;
            if (_timeLived > _secondsToLive)
                DestroySelf();
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            TryToDealDamage(collision);
            DestroySelf();
        }

        protected virtual void TryToDealDamage(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & _hitEntityMask.value) == 0)
                return;

            var component = collision.gameObject.GetComponentInParent<BaseLivingEntity>();
            if (component == null)
                return;

            component.ModifyHealth(-Damage);
            var rigidbody = GetComponent<Rigidbody>();
            collision.gameObject.GetComponent<Rigidbody>()
                .AddForceAtPosition(
                    rigidbody.velocity.normalized * _knockbackForce,
                    collision.contacts.Aggregate(Vector3.zero, (sum, next) => sum += next.point),
                    ForceMode.VelocityChange);
        }

        protected virtual void DestroySelf()
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}