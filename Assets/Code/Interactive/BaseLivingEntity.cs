using UnityEngine;

namespace TopDownShooter.Interactive
{
    public abstract class BaseLivingEntity : MonoBehaviour
    {
        [SerializeField] protected float _maxHealth = 10;
        [SerializeField] protected float _maxStamina = 10;
        [SerializeField] protected float _healthRegeneratedPerSecond = 0.5f;
        [SerializeField] protected float _staminaRegeneratedPerSecond = 0.5f;

        protected float _health;
        protected float _stamina;

        public float MaxHealth => _maxHealth;
        public float MaxStamina => _maxStamina;
        public float HealthRegeneratedPerSecond => _healthRegeneratedPerSecond;
        public float StaminaRegeneratedPerSecond => _staminaRegeneratedPerSecond;
        public float Health => _health;
        public float Stamina => _stamina;

        protected virtual void Awake()
        {
            _health = _maxHealth;
            _stamina = _maxStamina;
        }

        protected virtual void FixedUpdate()
        {
            if (_healthRegeneratedPerSecond != 0)
                ModifyHealth(_healthRegeneratedPerSecond * Time.fixedDeltaTime);

            if (_staminaRegeneratedPerSecond != 0)
                ModifyStamina(_staminaRegeneratedPerSecond * Time.fixedDeltaTime);
        }

        public virtual void ModifyHealth(float delta)
        {
            _health = Mathf.Clamp(_health + delta, 0, _maxHealth);

            if (_health == 0)
                Die();
        }

        public virtual void ModifyStamina(float delta)
        {
            _stamina = Mathf.Clamp(_stamina + delta, 0, _maxStamina);
        }

        protected abstract void Die();
    }
}