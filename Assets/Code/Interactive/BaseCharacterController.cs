using UnityEngine;

namespace TopDownShooter.Interactive
{
    [RequireComponent(typeof(Rigidbody))]
    public class BaseCharacterController : MonoBehaviour
    {
        [Header("Animated model")]
        [SerializeReference] protected Transform _model;

        [Header("Movement")]
        [SerializeField] private float _speed = 12;

        [SerializeField] protected float _velocitySmoothTime = 0.25f;
        [SerializeField] protected float _velocityRedirectionCoefficient = 0.05f;
        [SerializeField] protected float _maxVelocity = 10f;

        protected Rigidbody _rigidbody;

        protected Vector3 _currentVelocity;
        protected Vector3 _targetVelocity;
        protected Vector3 _velocityFromRedirection;
        protected Vector3 _acceleration;
        protected Vector3 _facingDirection;
        protected float _directionCoefficient;

        #region public API

        public void SetTargetVelocity(Vector3 velocity)
        {
            _targetVelocity = velocity;
        }

        public void SetFacingDirection(Vector3 direction)
        {
            var rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), Vector3.up);

            _model.rotation = rotation;
            _facingDirection = _model.forward;
        }

        public Vector3 GetLastVelocity() => _rigidbody.velocity;

        public float Speed { get => _speed; set => _speed = value; }

        #endregion public API

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = _maxVelocity;
        }

        protected virtual void Update()
        {
            if (DebugSettings.DrawEnemyMovementLines)
                DrawMovementLines();
        }

        protected virtual void FixedUpdate()
        {
            HandleMovement();
        }

        protected virtual void HandleMovement()
        {
            _currentVelocity = Vector3.SmoothDamp(_currentVelocity, _targetVelocity, ref _acceleration, _velocitySmoothTime, float.MaxValue, Time.fixedDeltaTime);

            var rigidbodyVelocity = _rigidbody.velocity;
            _directionCoefficient = Vector3.SignedAngle(rigidbodyVelocity, _targetVelocity, Vector3.forward);
            _directionCoefficient = Mathf.Abs(_directionCoefficient / 180f);
            _velocityFromRedirection = rigidbodyVelocity * _currentVelocity.magnitude * _directionCoefficient;
            _velocityFromRedirection *= _velocityRedirectionCoefficient;
            _velocityFromRedirection *= Time.fixedDeltaTime;

            _rigidbody.AddForce(_currentVelocity * _speed + _velocityFromRedirection, ForceMode.Acceleration);
        }

        protected virtual void DrawMovementLines()
        {
            Debug.DrawRay(transform.position, _targetVelocity, Color.green);
            Debug.DrawRay(transform.position, _currentVelocity, Color.cyan);
            Debug.DrawRay(transform.position, _acceleration, Color.blue);
            Debug.DrawRay(transform.position, _velocityFromRedirection, Color.magenta);
        }
    }
}