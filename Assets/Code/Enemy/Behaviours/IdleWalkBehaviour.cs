using System.Linq;
using TopDownShooter.Enemy.Traits.PathFollowing;
using UnityEngine;

namespace TopDownShooter.Enemy.Behaviours
{
    public class IdleWalkBehaviour : BaseBehaviour
    {
        public override int Priority => _behaviourPriority;
        [SerializeField] private int _behaviourPriority = 0;

        [SerializeField] private float _secondsToChangeIdleTarget = 3;
        [SerializeField] private float _idleTargetDistance = 5;
        [SerializeField] private float _minRepathDistance = 0.2f;

        [SerializeField] private float _minMovementDistanceVelocity = 0f;
        [SerializeField] private float _minMovementDistance = 0.5f;
        [SerializeField] private float _maxMovementDistanceVelocity = 1f;
        [SerializeField] private float _maxMovementDistance = 1f;
        [SerializeField] private float _speed = 2f;

        private int _updatesSincePathRecalculated;
        private Vector3 _idleMovementTarget;
        private float _secondsSinceLastIdleMovementTarget;
        private BasePathing _pathing;

        private Vector3 _input;
        private Vector3 _sideForce;
        private Vector3 _faceDirection;
        private Vector3 _movementTarget;

        public override void Initialize(EnemyAI enemyAI)
        {
            base.Initialize(enemyAI);
            _pathing = enemyAI.Pathing;
        }

        public override bool CanEnter()
        {
            return true;
        }

        public override bool CanExit()
        {
            return true;
        }

        public override void OnEnter()
        {
            _secondsSinceLastIdleMovementTarget = -1;
            _updatesSincePathRecalculated = -1;
            _enemyAI.CharacterController.Speed = _speed;
        }

        public override void OnExit()
        {
            _secondsSinceLastIdleMovementTarget = 0;
            _updatesSincePathRecalculated = 0;
        }

        public override void ActiveBehaviourUpdate()
        {
            ChangeTargetIfNecessary();
            RegeneratePathIfNecessary();
            FollowPath();

            _secondsSinceLastIdleMovementTarget += Time.fixedDeltaTime;
            _updatesSincePathRecalculated++;
        }

        private void ChangeTargetIfNecessary()
        {
            if (_secondsSinceLastIdleMovementTarget < 0 || _secondsSinceLastIdleMovementTarget > _secondsToChangeIdleTarget)
            {
                _secondsSinceLastIdleMovementTarget = 0;
                var offset = Random.insideUnitCircle * _idleTargetDistance;
                _idleMovementTarget = transform.position + new Vector3(offset.x, 0, offset.y);
                _updatesSincePathRecalculated = -1;
            }
        }

        private void RegeneratePathIfNecessary()
        {
            var path = _pathing.Path;

            // did we just enter this state?
            var shouldRegeneratePath = _updatesSincePathRecalculated < 0;
            // or is there no valid path
            shouldRegeneratePath |= path.corners.Length < 2;

            if (path.corners.Length >= 2)
            {
                // did we just move far enough?
                var distToFirst = Vector3.Distance(path.corners.First(), transform.position);
                var distToLast = Vector3.Distance(path.corners.Last(), transform.position);

                shouldRegeneratePath |= distToFirst > _minRepathDistance;
                shouldRegeneratePath |= distToLast > _minRepathDistance;
            }

            if (shouldRegeneratePath)
            {
                _enemyAI.Pathing.CalculatePathTo(_idleMovementTarget);
                _updatesSincePathRecalculated = 0;
            }
        }

        private void FollowPath()
        {
            var path = _pathing.Path;
            var position = transform.position;
            var velocity = _enemyAI.CharacterController.GetLastVelocity();
            var target = PathHelper.GetNextPointToFollowWithSomeDistanceEnforced(position, path.corners, 0.5f);

            var difference = target - position;
            var distance = difference.magnitude;
            var direction = difference.normalized;

            var straighteningVelocity = PathHelper.GetConstraintVelocityToFollowPath(position, target, velocity);

            var mappedDistance = MathHelper.ClampAndLinearMap(distance, _minMovementDistance, _minMovementDistanceVelocity, _maxMovementDistance, _maxMovementDistanceVelocity);
            var forwardVelocity = direction * mappedDistance;

            var (effects, _) = _enemyAI.SumMovementWeights();

            _sideForce = straighteningVelocity;
            _faceDirection = direction;
            _input = (straighteningVelocity + forwardVelocity + effects).normalized;
            _movementTarget = target;

            _enemyAI.SetFacingDirection(_faceDirection);
            _enemyAI.SetTargetVelocity(_input);
        }

        public override void DrawDebugLines()
        {
            base.DrawDebugLines();
            Debug.DrawRay(transform.position, _input, Color.blue);
            Debug.DrawRay(transform.position, _sideForce, Color.green);
            Debug.DrawRay(transform.position, _faceDirection, Color.magenta);
            Debug.DrawLine(transform.position, _movementTarget, Color.grey);
        }
    }
}