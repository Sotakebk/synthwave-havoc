using TopDownShooter.Enemy.Behaviours;
using TopDownShooter.Enemy.Traits.MovementEffects;
using TopDownShooter.Enemy.Traits.PathFollowing;
using TopDownShooter.Enemy.Traits.Vision;
using UnityEngine;

namespace TopDownShooter.Enemy
{
    [RequireComponent(typeof(EnemyCharacterController))]
    [RequireComponent(typeof(NoneBehaviour))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI traits")]

        [SerializeReference] protected BaseVision _vision;

        [SerializeReference] protected BasePathing _pathing;

        [SerializeField] protected MovementEffectWeightPair[] _movementEffectsWithWeights;

        public BaseVision Vision => _vision;
        public BasePathing Pathing => _pathing;
        public BaseCharacterController CharacterController => _characterController;

        private BaseCharacterController _characterController;
        private Vector3 _faceDirection;
        private Vector3 _smoothFaceDirection;

        protected BaseBehaviour[] _behaviours;
        protected NoneBehaviour _noneBehaviour;
        protected BaseBehaviour _currentBehaviour;

        protected virtual void Awake()
        {
            _characterController = GetComponent<BaseCharacterController>();
            _noneBehaviour = GetComponent<NoneBehaviour>();
            _behaviours = GetComponents<BaseBehaviour>();
            foreach(var behaviour in _behaviours)
            {
                behaviour.Initialize(this);
            }
        }

        protected virtual void Start()
        {
            EnterBehaviour(_noneBehaviour);
        }

        protected virtual BaseBehaviour GetBestFittingBehaviour()
        {
            BaseBehaviour highestPossibleBehaviour = null;
            int curentBehaviourPriority = int.MinValue;
            foreach(var behaviour in _behaviours)
            {
                if (behaviour.CanEnter() && behaviour.Priority > curentBehaviourPriority)
                {
                    curentBehaviourPriority = behaviour.Priority;
                    highestPossibleBehaviour = behaviour;
                }
            }

            return highestPossibleBehaviour;
        }

        protected virtual void Update()
        {
            _smoothFaceDirection = Vector3.Slerp(_smoothFaceDirection, _faceDirection, Mathf.Clamp01(5f * Time.deltaTime));

            _characterController.SetFacingDirection(_smoothFaceDirection);

            if (DebugSettings.DrawEnemyMovementLines)
            {
                DrawAIMovementLines();
            }
        }

        protected virtual void FixedUpdate()
        {
            SetTargetVelocity(Vector3.zero);
            _vision.UpdateVision();
            foreach(var behaviour in _behaviours)
            {
                behaviour.BehaviourUpdate();
            }

            if (_currentBehaviour.CanExit())
            {
                var bestFittingBehaviour = GetBestFittingBehaviour();
                if (bestFittingBehaviour != null && bestFittingBehaviour != _currentBehaviour)
                {
                    EnterBehaviour(bestFittingBehaviour);
                }
            }
            _currentBehaviour.ActiveBehaviourUpdate();
        }

        public virtual void EnterBehaviour(BaseBehaviour behaviour) {
            _currentBehaviour?.OnExit();
            _currentBehaviour = behaviour;
            _currentBehaviour.OnEnter();
        }

        protected virtual void DrawAIMovementLines()
        {
            _currentBehaviour.DrawDebugLines();
        }

        public virtual (Vector3 movement, float totalWeight) SumMovementWeights()
        {
            var movement = Vector3.zero;
            var weight = 0f;
            foreach (var pair in _movementEffectsWithWeights)
            {
                var thisWeight = pair.Weight;
                movement += pair.Effect.GetMovementEffect() * thisWeight;
                weight += thisWeight;
            }

            return (movement, weight);
        }

        public virtual void SetFacingDirection(Vector3 direction)
        {
            _faceDirection = direction;
        }

        public virtual void SetTargetVelocity(Vector3 velocity)
        {
            _characterController.SetTargetVelocity(velocity);
        }
    }
}